import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { DashboardService, EmitterService } from '../../_services';
import { DateTimeService, WidgetHelpersService } from '../../_helpers';
import { mergeMap } from 'rxjs/operators';
import { Router } from '@angular/router';
@Component({
    selector: 'app-notification-trend',
    templateUrl: './notification-trend.component.html',
    styleUrls: ['./notification-trend.component.scss']
})
export class NotificationTrendComponent implements OnInit {
    @Input() widgetname: string;
    @Input() url: string;
    @Input() filters: Array<any>;
    @Input() properties: Array<any>;
    // this widgetid is from widgets Collection and can be duplicate
    // it will be used for common functionality of same component instance type
    @Input() widgetid: number;
    @Input() dashboardid: number;
    @Input() id: number; // this is unique id

    loading: boolean = true;
    notificationTrendWidgetParameters: any;
    setWidgetFormValues: any;
    editWidgetName: boolean = true;
    @Output()
    notificationTrendParent = new EventEmitter<any>();

    public barChartData: Array<any> = [
        { data: [], label: 'No data in Notification Trend' }
    ];

    public barChartLabels: Array<any> = [];
    public barChartOptions: any = {
        responsive: true,
        legend: { position: 'bottom' },
    };
    public barChartLegend: boolean = true;
    public barChartType: string = 'bar';
    public notificationTrendColors: Array<any> = [
        {
            backgroundColor: 'rgba(76,175,80,1)',
            borderColor: 'rgba(76,175,80,1)',
            pointBackgroundColor: 'rgba(76,175,80,1)',
            pointBorderColor: '#fff',
            pointHoverBackgroundColor: '#fff',
            pointHoverBorderColor: 'rgba(76,175,80,0.8)'
        }
    ];

    constructor(
        private dashboardService: DashboardService,
        private emitter: EmitterService,
        private dateTime: DateTimeService,
        private router: Router,
        private widgetHelper: WidgetHelpersService
    ) { }

    ngOnInit() {
        console.log('NotificationTrendComponent', this.widgetname, this.url, this.id, this.widgetid, this.filters, this.properties);
        if (this.router.url.includes('dashboard/public')) {
            this.editWidgetName = false;
        }
        if (this.url) {
            this.emitter.loadingStatus(true);
            this.getChartParametersAndData(this.url);
        }
        // coming from dashboard or public parent components
        this.subscriptionForDataChangesFromParent()
    }

    subscriptionForDataChangesFromParent() {
        this.emitter.getData().subscribe(result => {
            const { type, data } = result;
            if (type === 'notificationTrendChart') {
                let currentWidgetId = data.notificationTrendWidgetParameters.id;
                if (currentWidgetId === this.id) {
                    // updating parameter form widget setValues
                    let notificationTrendFormValues = data.notificationTrendWidgetParameterValues;
                    // TODO: might be issue here. will see later
                    if (notificationTrendFormValues.Filters.daterange) {
                        notificationTrendFormValues.Filters.daterange = this.dateTime.buildRangeDate(notificationTrendFormValues.Filters.daterange);
                    }
                    this.setWidgetFormValues = notificationTrendFormValues;
                    this.updateChart(data.result.body, data, null);
                }
            }
        });
    }
    // invokes on component initialization
    getChartParametersAndData(url) {
        // these are default parameters need to update this logic
        // might have to make both API calls in sequence instead of parallel
        let myWidgetParameters = null;
        this.dashboardService.getWidgetParameters(url).pipe(
            mergeMap((getWidgetParameters: any) => {
                myWidgetParameters = getWidgetParameters;
                // Map Params for widget index when widgets initializes for first time
                let newParams = this.widgetHelper.initWidgetParameters(getWidgetParameters, this.filters, this.properties);

                return this.dashboardService.getWidgetIndex(url, newParams);
            })
        ).subscribe(getWidgetIndex => {
            // populate modal with widget parameters
            let notificationTrendParams;
            if (myWidgetParameters) {
                notificationTrendParams = {
                    type: 'notificationTrendParams',
                    data: {
                        ...myWidgetParameters,
                        widgetname: this.widgetname,
                        url: this.url,
                        filters: this.filters, // this.filter/properties will come from individual widget settings
                        properties: this.properties,
                        widgetid: this.widgetid,
                        dashboardid: this.dashboardid,
                        id: this.id
                    }
                }
                this.notificationTrendWidgetParameters = notificationTrendParams.data;
                // have to use setTimeout if i am not emitting it in dashbaordComponent
                // this.notificationTrendParent.emit(notificationTrendParams);
                // setting initial Paramter form widget values
                this.setWidgetFormValues = this.widgetHelper.setWidgetParameters(myWidgetParameters, this.filters, this.properties);
            }
            // popular chart data
            if (getWidgetIndex) {
                const chartIndexData = getWidgetIndex.body;
                // third params is current widgets settings current only used when
                // widgets loads first time. may update later for more use cases
                this.updateChart(chartIndexData, null, notificationTrendParams.data);
            }
            this.loading = false;
            this.emitter.loadingStatus(false);
        }, error => {
            console.error('NotificationTrendComponent', error);
            this.loading = false;
            this.emitter.loadingStatus(false);
        });
    }

    // events
    public chartClicked(e: any): void {
    }

    public chartHovered(e: any): void {
        // console.log(e);
    }

    openModal() {
        this.notificationTrendParent.emit({
            type: 'openNotificationTrendModal',
            data: {
                notificationTrendWidgetParameters: this.notificationTrendWidgetParameters,
                setWidgetFormValues: this.setWidgetFormValues,
                isNotificationTrendComponent: true
            }
        });
    }
    closeModal() {
        this.emitter.sendNext({ type: 'closeModal' });
    }
    // dashboardComponentData is result of data coming from
    // posting data to parameters widget
    updateChart(chartIndexData, dashboardComponentData, currentWidgetComponentData) {
        let label = 'Series';
        if (dashboardComponentData) {
            let measureIndex = dashboardComponentData.notificationTrendWidgetParameterValues.Properties.measure;
            label = dashboardComponentData.notificationTrendWidgetParameters.measures[measureIndex];
            let charttype = dashboardComponentData.notificationTrendWidgetParameterValues.Properties.charttype;
            setTimeout(() => {
                this.barChartType = charttype;
            });
        }
        if (currentWidgetComponentData) {
            // setting chart label and type on first load
            label = currentWidgetComponentData.measures[Object.keys(currentWidgetComponentData.measures)[0]];
            this.barChartType = Object.keys(currentWidgetComponentData.charttypes)[0];
        }
        if (chartIndexData.length) {
            setTimeout(() => {
                let allLabels = chartIndexData.map(label => label.xvalue);
                let allData = chartIndexData.map(data => data.yvalue);
                this.barChartData = [{ data: allData, label: label }]
                this.barChartLabels.length = 0;
                this.barChartLabels.push(...allLabels);
                this.closeModal();
            });
        }
    }

    widgetnameChange(event) {
        console.log('widgetnameChange', this.id, event);
        this.emitter.sendNext({
            type: 'changeWidgetName',
            data: {
                widgetname: event,
                id: this.id,
                widgetid: this.widgetid
            }
        });
    }
}

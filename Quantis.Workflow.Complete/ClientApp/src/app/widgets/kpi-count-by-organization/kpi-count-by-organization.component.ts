import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { DashboardService, EmitterService } from '../../_services';
import { forkJoin } from 'rxjs';
import { DateTimeService, WidgetHelpersService } from '../../_helpers';
import { mergeMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
@Component({
    selector: 'app-kpi-count-by-organization',
    templateUrl: './kpi-count-by-organization.component.html',
    styleUrls: ['./kpi-count-by-organization.component.scss']
})
export class KpiCountByOrganizationComponent implements OnInit {
    @Input() widgetname: string;
    @Input() url: string;
    @Input() filters: Array<any>;
    @Input() properties: Array<any>;
    @Input() widgetid: number;
    @Input() dashboardid: number;
    @Input() id: number;

    loading: boolean = true;
    kpiCountOrgWidgetParameters: any;
    setWidgetFormValues: any;
    editWidgetName: boolean = true;
    @Output()
    kpiCountOrgParent = new EventEmitter<any>();

    public barChartData: Array<any> = [
        { data: [65, 59, 80, 81, 56, 55, 40], label: 'Series A' }
    ];

    public barChartLabels: Array<any> = [];
    public barChartOptions: any = {
        responsive: true,
        legend: { position: 'bottom' },
    };
    public barChartLegend: boolean = true;
    public barChartType: string = 'bar';
    period: string = '';
    incompletePeriod: boolean = false;

    constructor(
        private dashboardService: DashboardService,
        private emitter: EmitterService,
        private dateTime: DateTimeService,
        private router: Router,
        private toastr: ToastrService,
        private widgetHelper: WidgetHelpersService
    ) { }

    ngOnInit() {
        console.log('KpiCountByOrganizationComponent', this.widgetname, this.url, this.id, this.widgetid, this.filters, this.properties);
        if (this.router.url.includes('dashboard/public')) {
            this.editWidgetName = false;
        }
        if (this.url) {
            this.emitter.loadingStatus(true);
            this.dashboardService.GetOrganizationHierarcy().subscribe(result => {
                this.getChartParametersAndData(this.url, result);
            }, error => {
                console.error('GetOrganizationHierarcy', error);
                this.toastr.error(`Unable to get contracts for ${this.widgetname}`, 'Error!');
            });
        }
        // coming from dashboard or public parent components
        this.subscriptionForDataChangesFromParent()
    }

    subscriptionForDataChangesFromParent() {
        this.emitter.getData().subscribe(result => {
            const { type, data } = result;
            if (type === 'kpiCountByOrgChart') {
                let currentWidgetId = data.kpiCountOrgWidgetParameters.id;
                if (currentWidgetId === this.id) {
                    // updating parameter form widget setValues
                    let kpiCountOrgFormValues = data.kpiCountOrgWidgetParameterValues;
                    if (kpiCountOrgFormValues.Filters.daterange) {
                        this.period = kpiCountOrgFormValues.Filters.daterange;
                        kpiCountOrgFormValues.Filters.daterange = this.dateTime.buildRangeDate(kpiCountOrgFormValues.Filters.daterange);
                    }
                    this.incompletePeriod = kpiCountOrgFormValues.Filters.incompletePeriod;
                    this.setWidgetFormValues = kpiCountOrgFormValues;
                    this.updateChart(data.result.body, data, null);
                }
            }
        });
    }
    // invokes on component initialization
    getChartParametersAndData(url, getOrgHierarcy) {
        // these are default parameters need to update this logic
        // might have to make both API calls in sequence instead of parallel
        let myWidgetParameters = null;
        this.dashboardService.getWidgetParameters(url).pipe(
            mergeMap((getWidgetParameters: any) => {
                myWidgetParameters = getWidgetParameters;
                myWidgetParameters.getOrgHierarcy = getOrgHierarcy;
                // Map Params for widget index when widgets initializes for first time
                let newParams = this.widgetHelper.initWidgetParameters(getWidgetParameters, this.filters, this.properties);
                this.period = newParams.Filters.daterange;
                this.incompletePeriod = newParams.Filters.incompletePeriod;
                return this.dashboardService.getWidgetIndex(url, newParams);
            })
        ).subscribe(getWidgetIndex => {
            // populate modal with widget parameters
            let kpiCountOrgsParams;
            if (myWidgetParameters) {
                kpiCountOrgsParams = {
                    type: 'kpiCountOrgsParams',
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
                this.kpiCountOrgWidgetParameters = kpiCountOrgsParams.data;
                // setting initial Paramter form widget values
                this.setWidgetFormValues = this.widgetHelper.setWidgetParameters(myWidgetParameters, this.filters, this.properties);
            }
            // popular chart data
            if (getWidgetIndex) {
                const chartIndexData = getWidgetIndex.body;
                // third params is current widgets settings current only used when
                // widgets loads first time. may update later for more use cases
                this.updateChart(chartIndexData, null, kpiCountOrgsParams.data);
            }
            this.loading = false;
            this.emitter.loadingStatus(false);
        }, error => {
            this.loading = false;
            this.emitter.loadingStatus(false);
        });
    }

    // events
    public chartClicked(e: any): void {
    }

    public chartHovered(e: any): void {
    }

    openModal() {
        this.kpiCountOrgParent.emit({
            type: 'openKpiCountOrgModal',
            data: {
                kpiCountOrgWidgetParameters: this.kpiCountOrgWidgetParameters,
                setWidgetFormValues: this.setWidgetFormValues,
                isKpiCountOrgComponent: true
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
            let measureIndex = dashboardComponentData.kpiCountOrgWidgetParameterValues.Properties.measure;
            label = dashboardComponentData.kpiCountOrgWidgetParameters.measures[measureIndex];
            let charttype = dashboardComponentData.kpiCountOrgWidgetParameterValues.Properties.charttype;
            setTimeout(() => {
                this.barChartType = charttype;
            });
        }
        if (currentWidgetComponentData) {
            // setting chart label and type on first load
            label = currentWidgetComponentData.measures[Object.keys(currentWidgetComponentData.measures)[0]];
            this.barChartType = Object.keys(currentWidgetComponentData.charttypes)[0];
        }
        let allLabels = chartIndexData.map(label => label.xvalue);
        let allData = chartIndexData.map(data => data.yvalue);
        this.barChartData = [{ data: allData, label: label }]
        this.barChartLabels.length = 0;
        this.barChartLabels.push(...allLabels);
        this.closeModal();
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

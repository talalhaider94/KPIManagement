import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { DashboardService, EmitterService } from '../../_services';
import { DateTimeService, WidgetHelpersService, chartExportTranslations, exportChartButton } from '../../_helpers';
import { mergeMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import * as Highcharts from 'highcharts';
import HC_exporting from 'highcharts/modules/exporting';
import { ToastrService } from 'ngx-toastr';
HC_exporting(Highcharts);
@Component({
    selector: 'app-doughnut-chart',
    templateUrl: './distribution-by-workflow.component.html',
    styleUrls: ['./distribution-by-workflow.component.scss']
})
export class DistributionByWorkflowComponent implements OnInit {
    @Input() widgetname: string;
    @Input() url: string;
    @Input() filters: Array<any>;
    @Input() properties: Array<any>;
    @Input() widgetid: number;
    @Input() dashboardid: number;
    @Input() id: number; // this is unique id

    loading: boolean = false;
    verificaDoughnutChartWidgetParameters: any;
    setWidgetFormValues: any;
    isDashboardModeEdit: boolean = true;
    @Output()
    verificaDoughnutParent = new EventEmitter<any>();

    highcharts = Highcharts;
    myChartUpdateFlag: boolean = true;
    myChartOptions = {
        lang: chartExportTranslations,
        credits: false,
        title: false,
        subtitle: {
            text: ''
        },
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.y}'

                }
            }
        },
        tooltip: {
            enabled: true,
            crosshairs: true
        },
        series: [{
            // name: this.widgetname,
            colorByPoint: true,
            data: [{
                name: 'No Data',
                y: 100
            }]
        }],
        exporting: exportChartButton,
    };
    period: string = '';
    incompletePeriod: boolean = false;
    constructor(
        private dashboardService: DashboardService,
        private emitter: EmitterService,
        private dateTime: DateTimeService,
        private router: Router,
        private toastr: ToastrService,
        private widgetHelper: WidgetHelpersService,
    ) { }

    ngOnInit() {
        console.log('DistributionByWorkflowComponent', this.widgetname, this.url, this.id, this.widgetid, this.filters, this.properties);
        if (this.router.url.includes('dashboard/public')) {
            this.isDashboardModeEdit = false;
            if (this.url) {
                this.loading = true;
                this.emitter.loadingStatus(true);
                this.dashboardService.GetOrganizationHierarcy().subscribe(result => {
                    this.getChartParametersAndData(this.url, result);
                }, error => {
                    console.error('GetOrganizationHierarcy', error);
                    this.toastr.error(`Unable to get contracts for ${this.widgetname}`, 'Error!');
                });
            }
            // coming from dashboard or public parent components
            this.subscriptionForDataChangesFromParent();
        }
        window.dispatchEvent(new Event('resize'));
    }

    subscriptionForDataChangesFromParent() {
        this.emitter.getData().subscribe(result => {
            const { type, data } = result;
            if (type === 'verificaDoughnutChart') {
                let currentWidgetId = data.verificaDoughnutWidgetParameters.id;
                if (currentWidgetId === this.id) {
                    // updating parameter form widget setValues
                    let verificaDoughnutFormValues = data.verificaDoughnutWidgetParameterValues;
                    if (verificaDoughnutFormValues.Filters.daterange) {
                        this.period = verificaDoughnutFormValues.Filters.daterange;
                        verificaDoughnutFormValues.Filters.daterange = this.dateTime.buildRangeDate(verificaDoughnutFormValues.Filters.daterange);
                    }
                    this.incompletePeriod = verificaDoughnutFormValues.Filters.incompletePeriod;
                    this.setWidgetFormValues = verificaDoughnutFormValues;
                    this.updateChart(data.result.body, data, null);
                }
            }
        });
    }
    getChartParametersAndData(url, getOrgHierarcy) {
        // these are default parameters need to update this logic
        // might have to make both API calls in sequence instead of parallel
        let myWidgetParameters = null;
        this.dashboardService.getWidgetParameters(url).pipe(
            mergeMap((getWidgetParameters: any) => {
                myWidgetParameters = getWidgetParameters;
                // Map Params for widget index when widgets initializes for first time
                myWidgetParameters.getOrgHierarcy = getOrgHierarcy;
                const newParams = this.widgetHelper.initWidgetParameters(myWidgetParameters, this.filters, this.properties);
                this.period = newParams.Filters.daterange;
                this.incompletePeriod = newParams.Filters.incompletePeriod;
                /// To be used -> getWidgetIndex method ////
                return this.dashboardService.getWidgetIndex(url, newParams);
            })
        ).subscribe(getWidgetIndex => {
            // populate modal with widget parameters
            console.log('getWidgetIndex', getWidgetIndex);
            console.log('myWidgetParameters', myWidgetParameters);
            let verificaDoughnutChartParams;
            if (myWidgetParameters) {
                verificaDoughnutChartParams = {
                    type: 'verificaDoughnutChartParams',
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
                this.verificaDoughnutChartWidgetParameters = verificaDoughnutChartParams.data;
                // setting initial Paramter form widget values
                this.setWidgetFormValues = this.widgetHelper.initWidgetParameters(myWidgetParameters, this.filters, this.properties);
            }
            // popular chart data
            if (getWidgetIndex) {
                const chartIndexData = getWidgetIndex.body;
                // third params is current widgets settings current only used when
                // widgets loads first time. may update later for more use cases
                this.updateChart(chartIndexData, null, verificaDoughnutChartParams.data);
            }
            this.loading = false;
            this.emitter.loadingStatus(false);
        }, error => {
            this.loading = false;
            this.emitter.loadingStatus(false);
        });
    }

    openModal() {
        this.verificaDoughnutParent.emit({
            type: 'openVerificaDoughnutChartModal',
            data: {
                verificaDoughnutChartWidgetParameters: this.verificaDoughnutChartWidgetParameters,
                setWidgetFormValues: this.setWidgetFormValues,
                isverificaDoughnutComponent: true
            }
        });
    }
    closeModal() {
        this.emitter.sendNext({ type: 'closeModal' });
    }
    // dashboardComponentData is result of data coming from
    // posting data to parameters widget
    updateChart(chartIndexData, dashboardComponentData, currentWidgetComponentData) {
        if (chartIndexData.length) {
            this.myChartOptions.subtitle = {
                text: ''
            }
        } else {
            this.myChartOptions.subtitle = {
                text: `No data in ${this.widgetname}`
            }
        }
        if (chartIndexData.length > 0) {
            const mapData = chartIndexData.map(data => ({ name: data.xvalue, y: data.yvalue }));
            this.myChartOptions.series[0].data = mapData;
        } else {
            this.myChartOptions.series[0].data = [{
                name: 'No Data',
                y: 0
            }];
        }

        this.myChartUpdateFlag = true;
        this.closeModal();
    }

    widgetnameChange(event) {
        console.log('widgetnameChange', this.id, event);
        this.verificaDoughnutParent.emit({
            type: 'changeVerificaDoughnutChartWidgetName',
            data: {
                verificaDoughnutChart: {
                    widgetname: event,
                    id: this.id,
                    widgetid: this.widgetid
                }
            }
        });
    }
    // events
    // public chartClicked(e: any): void {
    // 	console.log('Chart Clicked ->', e.active[0]._index);
    // 	let params = { month: '09', year: '19', key: 'donut_chart' };
    // 	window.open(`/#/workflow/verifica/?m=${params.month}&y=${params.year}&k=${params.key}`, '_blank');
    // }

    public chartClicked(e: any): void {
        if (e.active.length > 0) {
            const chart = e.active[0]._chart;
            const activePoints = chart.getElementAtEvent(e.event);
            if (activePoints.length > 0) {
                // get the internal index of slice in pie chart
                const clickedElementIndex = activePoints[0]._index;
                const label = chart.data.labels[clickedElementIndex];
                // get value by index
                const value = chart.data.datasets[0].data[clickedElementIndex];
                console.log('Chart Clicked ->', clickedElementIndex, label, value);
                window.open(`/#/dashboard/chartdata/?id=${clickedElementIndex}&value=${value}`, '_blank');
            }
        }
    }

    public chartHovered(e: any): void {
        console.log('Chart Hovered ->', e);
    }
}

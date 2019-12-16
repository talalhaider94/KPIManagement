import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { DashboardService, EmitterService } from '../../_services';
import { DateTimeService, WidgetHelpersService, chartExportTranslations, exportChartButton } from '../../_helpers';
import { mergeMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import * as Highcharts from 'highcharts';
import offline from 'highcharts/modules/offline-exporting'
offline(Highcharts);
@Component({
    selector: 'app-barchart',
    templateUrl: './barchart.component.html',
    styleUrls: ['./barchart.component.scss']
})
export class BarchartComponent implements OnInit {
    @Input() widgetname: string;
    @Input() url: string;
    @Input() filters: Array<any>;
    @Input() properties: Array<any>;
    // this widgetid is from widgets Collection and can be duplicate
    // it will be used for common functionality of same component instance type
    @Input() widgetid: number;
    @Input() dashboardid: number;
    @Input() id: number; // this is unique id

    loading: boolean = false;
    barChartWidgetParameters: any;
    setWidgetFormValues: any;
    isDashboardModeEdit: boolean = true;
    @Output()
    barChartParent = new EventEmitter<any>();

    highcharts = Highcharts;

    myChartUpdateFlag: boolean = true;
    myChartOptions = {
        lang: chartExportTranslations,
        credits: true,
        title: false,
        subtitle: {
            text: ''
        },
        chart: {
            type: 'column'
        },
        xAxis: {
            type: 'datetime',
            categories: ['10/18', '11/18', '12/18', '01/19', '02/19'],
            crosshair: true
        },
        plotOptions: {
            series: {
                dataLabels: {
                    enabled: true
                },
                point: {
                    events: {
                        click: function () {
                            console.log('Count Trend Bar Click Event');
                        }
                    }
                }
            }
        },
        tooltip: {
            enabled: true,
            crosshairs: true
        },
        series: [{
            name: 'Sample Data',
            data: [2,4,5,6,7],
            color: '#4dbd74'
        }],
        exporting: exportChartButton
    };
    allLeafNodesIds: any = [];
    period: string = '';
    incompletePeriod: boolean = false;
    constructor(
        private dashboardService: DashboardService,
        private emitter: EmitterService,
        private dateTime: DateTimeService,
        private router: Router,
        private widgetHelper: WidgetHelpersService,
        private toastr: ToastrService,
    ) { }

    ngOnInit() {
        console.log('Barchart Count Trend ==>', this.widgetname, this.url, this.id, this.widgetid, this.filters, this.properties);
        if (this.router.url.includes('dashboard/public')) {
            this.isDashboardModeEdit = false;
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
            this.subscriptionForDataChangesFromParent();
        }
        window.dispatchEvent(new Event('resize'));
    }

    subscriptionForDataChangesFromParent() {
        this.emitter.getData().subscribe(result => {
            const { type, data } = result;
            if (type === 'barChart') {
                let currentWidgetId = data.barChartWidgetParameters.id;
                if (currentWidgetId === this.id) {
                    // updating parameter form widget setValues
                    let barChartFormValues = data.barChartWidgetParameterValues;
                    if (barChartFormValues.Filters.daterange) {
                        this.period = barChartFormValues.Filters.daterange;
                        barChartFormValues.Filters.daterange = this.dateTime.buildRangeDate(barChartFormValues.Filters.daterange);
                    }
                    this.incompletePeriod = barChartFormValues.Filters.incompletePeriod;
                    this.setWidgetFormValues = barChartFormValues;
                    this.updateChart(data.result.body, data, null);
                }
            }
        });
    }

    // invokes on component initialization
    getChartParametersAndData(url, getOrgHierarcy) {
        // these are default parameters need to update this logic
        // might have to make both API calls in sequence instead of parallel
        this.loading = true;
        let myWidgetParameters = null;
        this.dashboardService.getWidgetParameters(url).pipe(
            mergeMap((getWidgetParameters: any) => {
                myWidgetParameters = getWidgetParameters;
                // Map Params for widget index when widgets initializes for first time
                myWidgetParameters.getOrgHierarcy = getOrgHierarcy;
                let newParams = this.widgetHelper.initWidgetParameters(getWidgetParameters, this.filters, this.properties);

                this.period = newParams.Filters.daterange;
                this.incompletePeriod = newParams.Filters.incompletePeriod;
                return this.dashboardService.getWidgetIndex(url, newParams);
            })
        ).subscribe(getWidgetIndex => {
            // populate modal with widget parameters
            let barChartParams;
            if (myWidgetParameters) {
                barChartParams = {
                    type: 'barChartParams',
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
                this.barChartWidgetParameters = barChartParams.data;
                // have to use setTimeout if i am not emitting it in dashbaordComponent
                // this.barChartParent.emit(barChartParams);
                // setting initial Paramter form widget values
                this.setWidgetFormValues = this.widgetHelper.setWidgetParameters(myWidgetParameters, this.filters, this.properties);
            }
            // popular chart data
            if (getWidgetIndex) {
                debugger
                const chartIndexData = getWidgetIndex.body;
                // third params is current widgets settings current only used when
                // widgets loads first time. may update later for more use cases
                this.updateChart(chartIndexData, null, barChartParams.data);
            }
            this.loading = false;
            this.emitter.loadingStatus(false);
        }, error => {
            console.error('Barchart Count Trend', error, this.setWidgetFormValues);
            this.loading = false;
            this.emitter.loadingStatus(false);
        });
    }

    // events
    public chartClicked(e: any): void {
        let params = { month: 'all', year: '19', key: 'bar_chart' };
        window.open(`/#/workflow/verifica/?m=${params.month}&y=${params.year}&k=${params.key}`, '_blank');
    }

    openModal() {
        this.barChartParent.emit({
            type: 'openBarChartModal',
            data: {
                barChartWidgetParameters: this.barChartWidgetParameters,
                setWidgetFormValues: this.setWidgetFormValues,
                isBarChartComponent: true
            }
        });
    }

    closeModal() {
        this.emitter.sendNext({ type: 'closeModal' });
    }

    // dashboardComponentData is result of data coming from
    // posting data to parameters widget
    updateChart(chartIndexData, dashboardComponentData, currentWidgetComponentData) {
        debugger
        //In case of anuual chartIndexData value = "[{"xvalue":"18","yvalue":174,"description":null},{"xvalue":"19","yvalue":109,"description":null}]"
        // (DO NOT USE IT) JUST FOR DEMO
        //In case of perios ChartIndexData value = "[{"xvalue":"01/19","yvalue":57,"description":null},{"xvalue":"02/19","yvalue":51,"description":null},{"xvalue":"03/19","yvalue":1,"description":null},{"xvalue":"10/18","yvalue":61,"description":null},{"xvalue":"11/18","yvalue":56,"description":null},{"xvalue":"12/18","yvalue":57,"description":null}]"
        // (DO NOT USE IT) JUST FOR DEMO
        let label = 'Series';
        if (dashboardComponentData) {
            let measureIndex = dashboardComponentData.barChartWidgetParameterValues.Properties.measure;
            label = dashboardComponentData.barChartWidgetParameters.measures[measureIndex];
            let charttype = dashboardComponentData.barChartWidgetParameterValues.Properties.charttype;
            if (charttype === 'line') {
                this.myChartOptions.chart = {
                    type: 'spline'
                }
            } else {
                this.myChartOptions.chart = {
                    type: 'column'
                }
            }

        }
        if (currentWidgetComponentData) {
            // setting chart label and type on first load
            label = currentWidgetComponentData.measures[Object.keys(currentWidgetComponentData.measures)[0]];
            console.log('CHART TYPE 2', Object.keys(currentWidgetComponentData.charttypes)[0]);
            if (Object.keys(currentWidgetComponentData.charttypes)[0] === 'line') {
                this.myChartOptions.chart = {
                    type: 'spline'
                }
            } else {
                this.myChartOptions.chart = {
                    type: 'column'
                }
            }
        }
        
        chartIndexData = this.dateTime.sortDate(chartIndexData);
        debugger
        let allLabels = chartIndexData.map(label => label.xvalue);
        let allData = chartIndexData.map(data => data.yvalue);
        if (chartIndexData.length) {
            this.myChartOptions.subtitle = {
                text: ''
            }
        } else {
            this.myChartOptions.subtitle = {
                text: `No data in ${this.widgetname}`
            }
        }

        this.myChartOptions.xAxis = {
            type: 'datetime',
            categories: allLabels,
            crosshair: true
        }
        this.myChartOptions.series[0] = {
            name: label,
            data: allData,
            color: '#4dbd74'
        };
        this.myChartUpdateFlag = true;
        this.closeModal();
    }

    widgetnameChange(event) {
        this.emitter.sendNext({
            type: 'changeWidgetName',
            data: {
                widgetname: event,
                id: this.id,
                widgetid: this.widgetid
            }
        });
    }
    // getAllLeafNodesIds(complexJson) {
    //     try {
    //         if (complexJson) {
    //             complexJson.forEach((item: any) => {
    //                 if (item.children) {
    //                     this.getAllLeafNodesIds(item.children);
    //                 } else {
    //                     this.allLeafNodesIds.push(item.id);
    //                 }
    //             });
    //             return this.allLeafNodesIds;
    //         }
    //     } catch(error) {
    //         console.error('getAllLeafNodesIds', error);
    //     }
    // }
}

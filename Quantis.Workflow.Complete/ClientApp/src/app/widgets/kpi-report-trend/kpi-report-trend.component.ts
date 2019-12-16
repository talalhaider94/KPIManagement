import { Component, OnInit, Input, Output, EventEmitter, ViewChild, OnChanges, SimpleChanges } from '@angular/core';
import { DashboardService, EmitterService } from '../../_services';
import { DateTimeService,
     WidgetHelpersService, 
     chartExportTranslations, 
     exportChartButton,
     formatDataLabelForNegativeValues, 
     getDistinctArray, updateChartLabelStyle} from '../../_helpers';
import { mergeMap } from 'rxjs/operators';
import { forkJoin } from 'rxjs';
import { Router } from '@angular/router';
import * as Highcharts from 'highcharts';
import { ToastrService } from 'ngx-toastr';
import HC_exporting from 'highcharts/modules/offline-exporting';
HC_exporting(Highcharts);

@Component({
    selector: 'app-kpi-report-trend',
    templateUrl: './kpi-report-trend.component.html',
    styleUrls: ['./kpi-report-trend.component.scss']
})
export class KpiReportTrendComponent implements OnInit, OnChanges {
    @Input() widgetname: string;
    @Input() url: string;
    @Input() filters: any;
    @Input() properties: any;
    @Input() widgetid: number;
    @Input() dashboardid: number;
    @Input() id: number;

    loading: boolean = false;
    kpiReportTrendWidgetParameters: any;
    kpiReportTrendWidgetParameters1: any;
    setWidgetFormValues: any;
    setWidgetFormValues1: any;
    editWidgetName: boolean = true;
    groupReportCheck: boolean = false;
    @Output()
    kpiReportTrendParent = new EventEmitter<any>();

    public kpiReportTrendChartType: string = 'bar';

    datiGrezzi = [];
    monthVar: any;
    yearVar: any;
    idKpi: any;
    countCampiData = [];
    eventTypes: any = {};
    resources: any = {};
    id_kpi_temp = '';
    loadingModalDati: boolean = false;
    public periodFilter: number;
    campoData: any = []
    fitroDataById: any = [
        {
            event_type_id: '   ',
            resource_id: '',
            time_stamp: ' ',
            raw_data_id: '',
            create_date: ' ',
            data: this.datiGrezzi,
            modify_date: '',
            reader_id: '',
            event_source_type_id: ' ',
            event_state_id: ' ',
            partner_raw_data_id: ' ',
        }
    ]


    contextmenu = false;
    contextmenuX = 0;
    contextmenuY = 0;

    highcharts = Highcharts;
    chartOptions = {
        lang: chartExportTranslations,
        credits: false,
        title: {
            text: 'KPI Report Trend'
        },
        xAxis: {
            type: 'date',
            categories: ['10/18', '11/18', '12/18', '01/19', '02/19', '03/19']
        },
        yAxis: {
            title: {
                text: '#'
            },
            min: 0
        },
        plotOptions: {
            /* column: {
                stacking: 'normal'
            }, */
            series: {
                dataLabels: {
                    enabled: true,
                    formatter: function() {
                        return formatDataLabelForNegativeValues(this.y);
                    }
                }
            }
        },
        tooltip: {
            enabled: true,
            crosshairs: true,
            formatter: function () {
                return this.series.name + '<br>' + 'y: <b>' + this.y + '</b>';
            }
        },
        series: [
            {
                type: 'column',
                name: 'Values',
                data: [{ "y": 0.35081, "color": "#f86c6b" }, { "y": -999, "color": "#379457" }, {"y": 0, "color": "#379457"}, { "y": 0.35702, "color": "#f86c6b" }, { "y": 0.39275, "color": "#379457" }, { "y": 0.38562, "color": "#379457" }],
                color: 'black',
                dataLabels: updateChartLabelStyle()
            },
            {
                type: 'scatter',
                name: 'Target',
                data: [2, 2, 2, 2, 2, 2],
                marker: {
                    fillColor: '#138496'
                },
                dataLabels: {
                    color: '#138496',
                },
            }
        ],
        exporting: exportChartButton
    };
    chartUpdateFlag: boolean = true;
    chartOptions1 = {
        lang: chartExportTranslations,
        credits: false,
        title: {
            text: 'KPI Report Trend'
        },
        plotOptions: {
            /* column: {
                stacking: 'normal'
            }, */
            series: {
                dataLabels: {
                    enabled: true,
                    formatter: function() {
                        return formatDataLabelForNegativeValues(this.y);
                    }
                }
            }
        },
        tooltip: {
            enabled: true,
            crosshairs: true
        },
        xAxis: {
            type: 'date',
            categories: ['10/18', '11/18', '12/18', '01/19', '02/19', '03/19']
        },
        yAxis: {
            title: {
                text: '#'
            },
            min: 0
        },
        series: [
            {
                type: 'column',
                name: 'Values',
                data: [{ "y": 0.35451, "color": "#379457" }, { "y": -999, "color": "#379457" }, {"y": 0, "color": "#379457"}, { "y": 0.35081, "color": "#f86c6b" }, { "y": 0.35702, "color": "#f86c6b" }, { "y": 0.39275, "color": "#379457" }, { "y": 0.38562, "color": "#379457" }],
                color: 'black',
                dataLabels: updateChartLabelStyle()
            },
            {
                type: 'scatter',
                name: 'Target',
                data: [2, 2, 2, 2, 2, 2, 2],
                marker: {
                    fillColor: '#138496'
                },
                dataLabels: {
                    color: '#138496',
                },
            }
        ],
        exporting: exportChartButton
    };
    chartUpdateFlag1: boolean = true;

    contractParties: string = 'N/A';
    contracts: string = 'N/A';
    kpi: string = 'N/A';
    contractParties1: string = 'N/A';
    contracts1: string = 'N/A';
    kpi1: string = 'N/A';
    period: string = '';
    incompletePeriod: boolean = false;
    constructor(
        private dashboardService: DashboardService,
        private emitter: EmitterService,
        private dateTime: DateTimeService,
        private router: Router,
        private widgetHelper: WidgetHelpersService,
        private toastr: ToastrService
    ) { }

    ngOnInit() {

        this.periodFilter = 0;

        console.log('KPI REPORT TREND ==>', this.filters, this.properties);
        this.chartOptions.title = {
            text: this.widgetname,
        };
        this.chartUpdateFlag = true;
        if (this.router.url.includes('dashboard/public')) {
            this.editWidgetName = false;
            if (this.url) {
                this.emitter.loadingStatus(true);
                this.loading = true;

                if (this.filters.hasOwnProperty('contractParties') &&
                    this.filters.hasOwnProperty('contracts') && this.filters.hasOwnProperty('kpi')) {
                    this.getComboxBoxesSet(this.filters.contractParties, this.filters.contracts).subscribe(result => {
                        const [contractParties, contracts, kpis] = result;
                        this.getChartParametersAndData(this.url, { contractParties, contracts, kpis });
                    }, error => {
                        console.error(this.widgetname, error);
                        this.toastr.error('Unable to get KPIs', this.widgetname);
                    });
                } else {
                    this.dashboardService.getContractParties().subscribe(contractParties => {
                        this.getChartParametersAndData(this.url, { contractParties });
                    }, error => {
                        console.error('getContractParties', error);
                        this.toastr.error('Unable to get Contract Parties', this.widgetname);
                    });
                }
                if (this.filters.hasOwnProperty('contractParties1') &&
                    this.filters.hasOwnProperty('contracts1') && this.filters.hasOwnProperty('kpi1')) {
                    this.getComboxBoxesSet(this.filters.contractParties1, this.filters.contracts1).subscribe(result => {
                        const [contractParties, contracts, kpis] = result;
                        this.getChartParametersAndData1(this.url, { contractParties, contracts, kpis });
                    }, error => {
                        console.error(this.widgetname, error);
                        this.toastr.error('Unable to get KPIs', this.widgetname);
                    });
                } else {
                    // This API call here is necessary because we need to display
                    // contract Parties 1 for second chart in case user checked Group report
                    this.dashboardService.getContractParties().subscribe(contractParties => {
                        this.getChartParametersAndData1(this.url, { contractParties });
                    }, error => {
                        console.error('getContractParties', error);
                        this.toastr.error('Unable to get Contract Parties', this.widgetname);
                    });
                }
            }
            // coming from dashboard or public parent components
            this.subscriptionForDataChangesFromParent();
            this.subscriptionForDataChangesFromParent1();
        }
        window.dispatchEvent(new Event('resize'));
    }

    ngOnChanges(changes: SimpleChanges) {
        window.dispatchEvent(new Event('resize'));
    }

    subscriptionForDataChangesFromParent() {
        this.emitter.getData().subscribe(result => {
            const { type, data } = result;
            if (type === 'kpiReportTrendChart') {
                let currentWidgetId = data.kpiReportTrendWidgetParameters.id;
                if (currentWidgetId === this.id) {
                    // updating parameter form widget setValues
                    let kpiReportTrendFormValues = data.kpiReportTrendWidgetParameterValues;
                    if (kpiReportTrendFormValues.Filters.daterange) {
                        this.period = kpiReportTrendFormValues.Filters.daterange;
                        kpiReportTrendFormValues.Filters.daterange = this.dateTime.buildRangeDate(kpiReportTrendFormValues.Filters.daterange);
                    }
                    this.incompletePeriod = kpiReportTrendFormValues.Filters.incompletePeriod;
                    this.setWidgetFormValues = kpiReportTrendFormValues;
                    this.getContractParties(data.kpiReportTrendWidgetParameters, this.setWidgetFormValues);
                    this.getContracts(data.kpiReportTrendWidgetParameters, this.setWidgetFormValues);
                    this.getKPI(data.kpiReportTrendWidgetParameters, this.setWidgetFormValues);
                    this.updateChart(data.result.body, data, null);
                }
            }
        });
    }
    // Danial: TODO: need to check for comboboxes 1 values fro dropdown
    subscriptionForDataChangesFromParent1() {
        this.emitter.getData().subscribe(result => {
            const { type, data } = result;
            if (type === 'kpiReportTrendChart1') {
                let currentWidgetId = data.kpiReportTrendWidgetParameters.id;
                if (currentWidgetId === this.id) {
                    this.groupReportCheck = data.kpiReportTrendWidgetParameterValues.Filters.groupReportCheck;
                    // updating parameter form widget setValues
                    let kpiReportTrendFormValues = data.kpiReportTrendWidgetParameterValues;
                    if (kpiReportTrendFormValues.Filters.daterange) {
                        console.log('kpiReportTrendFormValues.Filters.daterange 1', kpiReportTrendFormValues.Filters.daterange)
                        kpiReportTrendFormValues.Filters.daterange = this.dateTime.buildRangeDate(kpiReportTrendFormValues.Filters.daterange);
                    }
                    this.setWidgetFormValues1 = kpiReportTrendFormValues;
                    debugger
                    this.getContractParties1(data.kpiReportTrendWidgetParameters, this.setWidgetFormValues);
                    this.getContracts1(data.kpiReportTrendWidgetParameters, this.setWidgetFormValues);
                    this.getKPI1(data.kpiReportTrendWidgetParameters, this.setWidgetFormValues);
                    this.updateChart1(data.result.body, data, null);
                }
            }
        });
    }

    // invokes on component initialization
    getChartParametersAndData(url, comboxBoxesResult) {
        // these are default parameters need to update this logic
        // might have to make both API calls in sequence instead of parallel
        let myWidgetParameters = null;
        this.dashboardService.getWidgetParameters(url).pipe(
            mergeMap((getWidgetParameters: any) => {
                myWidgetParameters = getWidgetParameters;
                // Map Params for widget index when widgets initializes for first time
                const newParams = this.widgetHelper.initWidgetParameters(getWidgetParameters, this.filters, this.properties);
                this.period = newParams.Filters.daterange;
                this.incompletePeriod = newParams.Filters.incompletePeriod;
                if (newParams.Filters.hasOwnProperty('kpi1')) {
                    delete newParams.Filters.kpi1;
                }
                return this.dashboardService.getWidgetIndex(url, newParams);
            })
        ).subscribe(getWidgetIndex => {
            // populate modal with widget parameters
            let kpiReportTrendParams;
            if (myWidgetParameters) {
                myWidgetParameters.allContractParties = comboxBoxesResult.contractParties;
                myWidgetParameters.allContracts = comboxBoxesResult.contracts;
                myWidgetParameters.allKpis = comboxBoxesResult.kpis;
                kpiReportTrendParams = {
                    type: 'kpiReportTrendParams',
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
                this.kpiReportTrendWidgetParameters = kpiReportTrendParams.data;
                // setting initial Paramter form widget values
                this.setWidgetFormValues = this.widgetHelper.setWidgetParameters(myWidgetParameters, this.filters, this.properties);
                this.getContractParties(this.kpiReportTrendWidgetParameters, this.setWidgetFormValues);
                this.getContracts(this.kpiReportTrendWidgetParameters, this.setWidgetFormValues);
                this.getKPI(this.kpiReportTrendWidgetParameters, this.setWidgetFormValues);
            }
            // popular chart data
            if (getWidgetIndex) {
                const chartIndexData = getWidgetIndex.body;
                // third params is current widgets settings current only used when
                // widgets loads first time. may update later for more use cases
                this.updateChart(chartIndexData, null, kpiReportTrendParams.data);
            }
            this.loading = false;
            this.emitter.loadingStatus(false);
        }, error => {
            console.error('KPI Report Trend', error);
            this.toastr.error('Unable to get widget data', this.widgetname);
            this.loading = false;
            this.emitter.loadingStatus(false);
        });
    }

    openModal() {
        const chart1Parameters = this.kpiReportTrendWidgetParameters1;
        const chart1SetFormValues = this.setWidgetFormValues1;
        if (chart1Parameters) {

            if (chart1Parameters.hasOwnProperty('allContractParties1')) {
                this.kpiReportTrendWidgetParameters.allContractParties1 = chart1Parameters.allContractParties1;
            }
            if (chart1Parameters.hasOwnProperty('allContracts1')) {
                this.kpiReportTrendWidgetParameters.allContracts1 = chart1Parameters.allContracts1;
            }
            if (chart1Parameters.hasOwnProperty('allKpis1')) {
                this.kpiReportTrendWidgetParameters.allKpis1 = chart1Parameters.allKpis1;
            }
        }

        if (chart1SetFormValues) {
            if (chart1SetFormValues.Filters.hasOwnProperty('contractParties1')) {
                this.setWidgetFormValues.Filters.contractParties1 = chart1SetFormValues.Filters.contractParties1;
            }
            if (chart1SetFormValues.Filters.hasOwnProperty('contracts1')) {
                this.setWidgetFormValues.Filters.contracts1 = chart1SetFormValues.Filters.contracts1;
            }
            if (chart1SetFormValues.Filters.hasOwnProperty('kpi1')) {
                this.setWidgetFormValues.Filters.kpi1 = chart1SetFormValues.Filters.kpi1;
            }
        }

        this.kpiReportTrendParent.emit({
            type: 'openKpiReportTrendModal',
            data: {
                kpiReportTrendWidgetParameters: this.kpiReportTrendWidgetParameters,
                setWidgetFormValues: this.setWidgetFormValues,
                isKpiReportTrendComponent: true
            }
        });
    }
    closeModal() {
        this.emitter.sendNext({ type: 'closeModal' });
    }
    // dashboardComponentData is result of data coming from
    // posting data to parameters widget
    updateChart(chartIndexData, dashboardComponentData, currentWidgetComponentData) {
        if (dashboardComponentData) {
            let charttype = dashboardComponentData.kpiReportTrendWidgetParameterValues.Properties.charttype;
            setTimeout(() => {
                this.kpiReportTrendChartType = charttype;
            });
        }
        if (currentWidgetComponentData) {
            this.kpiReportTrendChartType = Object.keys(currentWidgetComponentData.charttypes)[0];
        }
        if (!chartIndexData.length) {
            this.chartOptions.title = {
                text: `No data in ${this.widgetname}.`,
            };
        } else {
            this.chartOptions.title = {
                text: this.widgetname,
            };
        }
        let targetData = chartIndexData.filter(data => data.zvalue === 'Target');
        let valueData = chartIndexData.filter(data => data.zvalue === 'Value');
        if (valueData.length > 0) {
            this.chartOptions.yAxis.title = {
                text: valueData[0].description.split('|')[1]
            }
        }
        let allChartLabels = getDistinctArray(chartIndexData.map(label => label.xvalue));

        let allTargetData = targetData.map(data => data.yvalue);
        let allValuesData = valueData.map(data => ({
            y: data.yvalue,
            name: data.description,
            color: data.description.includes('non compliant') ? '#f86c6b' : '#379457',
        }));
        this.chartOptions.tooltip = {
            enabled:true,
            crosshairs:true,
            formatter: function () {
                return this.series.name + '<br>'
                + 'y: <b>' + this.y + '</b>';
            }
        }
        this.chartOptions.xAxis = {
            type: 'date',
            categories: allChartLabels
        }
        this.chartOptions.series[0] = {
            type: 'column',
            name: 'Values',
            data: allValuesData,
            color: 'black',
            dataLabels: updateChartLabelStyle()
        };
        this.chartOptions.series[1] = {
            type: 'scatter',
            name: 'Target',
            data: allTargetData,
            marker: {
                fillColor: '#138496'
            },
            dataLabels: {
                color: '#138496',
            },
        };

        this.chartUpdateFlag = true;
        window.dispatchEvent(new Event('resize'));
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

    getComboxBoxesSet(contractParties, contracts) {
        const allContractParties = this.dashboardService.getContractParties();
        const allContracts = this.dashboardService.getContract(0, contractParties);
        const allKpis = this.dashboardService.getKPIs(0, contracts);
        return forkJoin([allContractParties, allContracts, allKpis]);
    }

    updateChart1(chartIndexData, dashboardComponentData, currentWidgetComponentData) {
        if (dashboardComponentData) {
            let charttype = dashboardComponentData.kpiReportTrendWidgetParameterValues.Properties.charttype;
            setTimeout(() => {
                this.kpiReportTrendChartType = charttype;
            });
        }
        if (currentWidgetComponentData) {
            this.kpiReportTrendChartType = Object.keys(currentWidgetComponentData.charttypes)[0];
        }
        if (!chartIndexData.length) {
            this.chartOptions1.title = {
                text: `No data in ${this.widgetname}.`,
            };
        } else {
            this.chartOptions1.title = {
                text: this.widgetname,
            };
        }
        let targetData = chartIndexData.filter(data => data.zvalue === 'Target');
        let valueData = chartIndexData.filter(data => data.zvalue === 'Value');
        if (valueData.length > 0) {
            this.chartOptions1.yAxis.title = {
                text: valueData[0].description.split('|')[1]
            }
        }
        let allChartLabels = chartIndexData.map(label => label.xvalue);

        let allTargetData = targetData.map(data => data.yvalue);
        let allValuesData = valueData.map(data => ({
            y: data.yvalue,
            name: data.description,
            color: data.description.includes('non compliant') ? '#f86c6b' : '#379457',
        }));
        this.chartOptions1.xAxis = {
            type: 'date',
            categories: allChartLabels,
        }
        this.chartOptions1.series[0] = {
            type: 'column',
            name: 'Values',
            data: allValuesData,
            color: 'black',
            dataLabels: updateChartLabelStyle()
        };
        this.chartOptions1.series[1] = {
            type: 'scatter',
            name: 'Target',
            data: allTargetData,
            marker: {
                fillColor: '#138496'
            },
            dataLabels: {
                color: '#138496',
            },
        };

        this.chartUpdateFlag1 = true;
        window.dispatchEvent(new Event('resize'));
        this.closeModal();
    }

    getChartParametersAndData1(url, comboxBoxesResult) {
        // these are default parameters need to update this logic
        // might have to make both API calls in sequence instead of parallel
        let myWidgetParameters = null;
        this.dashboardService.getWidgetParameters(url).pipe(
            mergeMap((getWidgetParameters: any) => {
                myWidgetParameters = getWidgetParameters;
                // Map Params for widget index when widgets initializes for first time
                const newParams = this.widgetHelper.initWidgetParameters(getWidgetParameters, this.filters, this.properties);
                if (newParams.Filters.hasOwnProperty('kpi')) {
                    delete newParams.Filters.kpi;
                }
                if (newParams.Filters.hasOwnProperty('kpi1')) {
                    newParams.Filters.kpi = newParams.Filters.kpi1;
                    delete newParams.Filters.kpi1;
                }
                if (newParams.Filters.hasOwnProperty('groupReportCheck')) {
                    this.groupReportCheck = newParams.Filters.groupReportCheck;
                }
                return this.dashboardService.getWidgetIndex(url, newParams);
            })
        ).subscribe(getWidgetIndex => {
            // populate modal with widget parameters
            let kpiReportTrendParams;
            if (myWidgetParameters) {
                myWidgetParameters.allContractParties1 = comboxBoxesResult.contractParties;
                myWidgetParameters.allContracts1 = comboxBoxesResult.contracts;
                myWidgetParameters.allKpis1 = comboxBoxesResult.kpis;
                kpiReportTrendParams = {
                    type: 'kpiReportTrendParams',
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
                this.kpiReportTrendWidgetParameters1 = kpiReportTrendParams.data;
                // setting initial Paramter form widget values
                this.setWidgetFormValues1 = this.widgetHelper.setWidgetParameters(myWidgetParameters, this.filters, this.properties);
                this.getContractParties1(this.kpiReportTrendWidgetParameters1, this.setWidgetFormValues1);
                this.getContracts1(this.kpiReportTrendWidgetParameters1, this.setWidgetFormValues1);
                this.getKPI1(this.kpiReportTrendWidgetParameters1, this.setWidgetFormValues1);
            }
            // popular chart data
            if (getWidgetIndex) {
                const chartIndexData = getWidgetIndex.body;
                // third params is current widgets settings current only used when
                // widgets loads first time. may update later for more use cases
                this.updateChart1(chartIndexData, null, kpiReportTrendParams.data);
            }
            this.loading = false;
            this.emitter.loadingStatus(false);
        }, error => {
            console.error('KPI Report Trend', error);
            this.toastr.error('Unable to get widget data', this.widgetname);
            this.loading = false;
            this.emitter.loadingStatus(false);
        });
    }

    getContractParties(kpiReportTrendWidgetParameters, setWidgetFormValues) {
        if (kpiReportTrendWidgetParameters && setWidgetFormValues && kpiReportTrendWidgetParameters.allContractParties) {
            const contractParties = kpiReportTrendWidgetParameters.allContractParties;
            const contractPartyKey = setWidgetFormValues.Filters.contractParties;
            if (contractPartyKey) {
                const c = contractParties.find(contractParty => contractParty.key.toString() === contractPartyKey.toString());
                this.contractParties = (c) ? c.value : 'N/A';
            } else {
                this.contractParties = 'N/A';
            }
        } else {
            this.contractParties = 'N/A';
        }
    }

    getContractParties1(kpiReportTrendWidgetParameters, setWidgetFormValues) {
        if (kpiReportTrendWidgetParameters && setWidgetFormValues && kpiReportTrendWidgetParameters.allContractParties1) {
            const contractParties = kpiReportTrendWidgetParameters.allContractParties1;
            const contractPartyKey = setWidgetFormValues.Filters.contractParties1;
            if (contractPartyKey) {
                const c = contractParties.find(contractParty => contractParty.key.toString() === contractPartyKey.toString());
                this.contractParties1 = (c) ? c.value : 'N/A';
            } else {
                this.contractParties1 = 'N/A';
            }

        } else {
            this.contractParties1 = 'N/A';
        }
    }

    getContracts(kpiReportTrendWidgetParameters, setWidgetFormValues) {
        if (kpiReportTrendWidgetParameters && setWidgetFormValues && kpiReportTrendWidgetParameters.allContracts) {
            const contracts = kpiReportTrendWidgetParameters.allContracts;
            const contractKey = setWidgetFormValues.Filters.contracts;
            if (contractKey) {
                const c = contracts.find(contract => contract.key.toString() === contractKey.toString());
                this.contracts = (c) ? c.value : 'N/A';
            } else {
                this.contracts = 'N/A';
            }
        } else {
            this.contracts = 'N/A';
        }
    }

    getContracts1(kpiReportTrendWidgetParameters, setWidgetFormValues) {
        if (kpiReportTrendWidgetParameters && setWidgetFormValues && kpiReportTrendWidgetParameters.allContracts1) {
            const contractParties = kpiReportTrendWidgetParameters.allContracts1;
            const contractPartyKey = setWidgetFormValues.Filters.contracts1;
            if (contractPartyKey) {
                const c = contractParties.find(contractParty => contractParty.key.toString() === contractPartyKey.toString());
                this.contracts1 = (c) ? c.value : 'N/A';
            } else {
                this.contracts1 = 'N/A';
            }
        } else {
            this.contracts1 = 'N/A';
        }
    }

    getKPI(kpiReportTrendWidgetParameters, setWidgetFormValues) {
        if (kpiReportTrendWidgetParameters && setWidgetFormValues && kpiReportTrendWidgetParameters.allKpis) {
            const contractParties = kpiReportTrendWidgetParameters.allKpis;
            const contractPartyKey = setWidgetFormValues.Filters.kpi;
            if (contractPartyKey) {
                const c = contractParties.find(contractParty => contractParty.key.toString() === contractPartyKey.toString());
                this.kpi = (c) ? c.value : 'N/A';
            } else {
                this.kpi = 'N/A';
            }
        } else {
            this.kpi = 'N/A';
        }
    }

    getKPI1(kpiReportTrendWidgetParameters, setWidgetFormValues) {
        if (kpiReportTrendWidgetParameters && setWidgetFormValues && kpiReportTrendWidgetParameters.allKpis1) {
            const contractParties = kpiReportTrendWidgetParameters.allKpis1;
            const contractPartyKey = setWidgetFormValues.Filters.kpi1;
            if (contractPartyKey) {
                const c = contractParties.find(contractParty => contractParty.key.toString() === contractPartyKey.toString());
                this.kpi1 = (c) ? c.value : 'N/A';
            } else {
                this.kpi1 = 'N/A';
            }

        } else {
            this.kpi1 = 'N/A';
        }
    }

    openDrillDownTable() {
        this.kpiReportTrendParent.emit({
            type: 'openKpiReportDrillDownTable',
            data: {
                setWidgetFormValues: this.setWidgetFormValues,
            }
        });
    }

    openDayDrillDownTable() {
        this.kpiReportTrendParent.emit({
            type: 'openDayDrillDownTable',
            data: {
                setWidgetFormValues: this.setWidgetFormValues,
            }
        });
    }


    // public chartClicked(): void {
    //     this.months.length = 0;
    //     this.isLoadedDati2 = 0;

    //     var fromCheck = moment(this.ReportDetailsData.fromdate, 'DD/MM/YYYY');
    //     var toCheck = moment(this.ReportDetailsData.todate, 'DD/MM/YYYY');

    //     var fromMonth = fromCheck.format('M');
    //     var fromYear  = fromCheck.format('YYYY');

    //     var toMonth = toCheck.format('M');
    //     var toYear  = toCheck.format('YYYY');

    //     this.selectedmonth = toMonth;
    //     this.selectedyear = toYear;

    //     this.to_year = toYear;

    //     while(toCheck > fromCheck || fromCheck.format('M') === toCheck.format('M')){
    //         let monthyear = fromCheck.format('M') + '/' + fromCheck.format('YYYY');
    //         this.months.push(monthyear);
    //         fromCheck.add(1,'month');
    //     }

    //     // console.log('Chart Clicked -> ',this.ReportDetailsData.globalruleid, this.ReportDetailsData.fromdate,
    //     // this.ReportDetailsData.todate);

    //     console.log('From Date -> ',fromMonth,fromYear,' - To Date -> ',toMonth,toYear);
    //     console.log('Months -> ',this.months);

    //     this.getdati1(toMonth,toYear);
    // }

    // public chartClicked2(): void {
    //     this.months2.length = 0;
    //     this.isLoadedDati = 0;

    //     var fromCheck = moment(this.ReportDetailsData.fromdate, 'DD/MM/YYYY');
    //     var toCheck = moment(this.ReportDetailsData.todate, 'DD/MM/YYYY');

    //     var fromMonth = fromCheck.format('M');
    //     var fromYear  = fromCheck.format('YYYY');

    //     var toMonth = toCheck.format('M');
    //     var toYear  = toCheck.format('YYYY');

    //     this.selectedmonth = toMonth;
    //     this.selectedyear = toYear;

    //     this.to_year2 = toYear;

    //     while(toCheck > fromCheck || fromCheck.format('M') === toCheck.format('M')){
    //         let monthyear = fromCheck.format('M') + '/' + fromCheck.format('YYYY');
    //         this.months2.push(monthyear);
    //         fromCheck.add(1,'month');
    //     }

    //     console.log('From Date -> ',fromMonth,fromYear,' - To Date -> ',toMonth,toYear);
    //     console.log('Months -> ',this.months2);

    //     // console.log('Chart Clicked2 -> ',this.ReportDetailsData);

    //     this.getdati2(toMonth,toYear);
    // }

    // getdati1(toMonth,toYear) {
    //     this.periodFilter = 1;
    //     let month;
    //     let year;
    //     if(toMonth<10){
    //         month = '0' + toMonth;
    //     }else{
    //         month = toMonth;
    //     }
    //     //year = '2018';
    //     year = toYear;
    //     let kpiId = this.ReportDetailsData.globalruleid;
    //     // let month = '08';
    //     // let year = '2018';
    //     //let kpiId = 39412;
    //     this.loadingModalDati = true;
    //     this.isLoadedDati=1;

    //     console.log('getdati1 -> ',kpiId,month,year);

    //     this.apiService.getKpiRawData(kpiId, month, year).subscribe((dati: any) => {
    //         this.fitroDataById = dati;
    //         //console.log(dati);
    //         Object.keys(this.fitroDataById).forEach(key => {
    //             this.fitroDataById[key].data = JSON.parse(this.fitroDataById[key].data);
    //             switch (this.fitroDataById[key].event_state_id) {
    //                 case 1:
    //                     this.fitroDataById[key].event_state_id = "Originale";
    //                     break;
    //                 case 2:
    //                     this.fitroDataById[key].event_state_id = "Sovrascritto";
    //                     break;
    //                 case 3:
    //                     this.fitroDataById[key].event_state_id = "Eliminato";
    //                     break;
    //                 case 4:
    //                     this.fitroDataById[key].event_state_id = "Correzione";
    //                     break;
    //                 case 5:
    //                     this.fitroDataById[key].event_state_id = "Correzione eliminata";
    //                     break;
    //                 case 6:
    //                     this.fitroDataById[key].event_state_id = "Business";
    //                     break;
    //                 default:
    //                     this.fitroDataById[key].event_state_id = this.fitroDataById[key].event_state_id;
    //                     break;
    //             }
    //             this.fitroDataById[key].event_type_id = this.eventTypes[this.fitroDataById[key].event_type_id] ? this.eventTypes[this.fitroDataById[key].event_type_id] : this.fitroDataById[key].event_type_id;
    //             this.fitroDataById[key].resource_id = this.resources[this.fitroDataById[key].resource_id] ? this.resources[this.fitroDataById[key].resource_id] : this.fitroDataById[key].resource_id;
    //             this.fitroDataById[key].modify_date = moment(this.fitroDataById[key].modify_date).format('DD/MM/YYYY HH:mm:ss');
    //             this.fitroDataById[key].create_date = moment(this.fitroDataById[key].create_date).format('DD/MM/YYYY HH:mm:ss');
    //             this.fitroDataById[key].time_stamp = moment(this.fitroDataById[key].time_stamp).format('DD/MM/YYYY HH:mm:ss');
    //         })
    //         this.getCountCampiData();

    //         let max = this.countCampiData.length;

    //         Object.keys(this.fitroDataById).forEach(key => {
    //             let temp = Object.keys(this.fitroDataById[key].data).length;
    //             if (temp < max) {
    //                 for (let i = 0; i < (max - temp); i++) {
    //                     this.fitroDataById[key].data['empty#' + i] = '##empty##';
    //                 }
    //             }
    //         })
    //         console.log('dati', dati);
    //         this.loadingModalDati = false;
    //     },
    //     error => {
    //         this.loadingModalDati = false;
    //     });
    // }

    // getdati2(toMonth,toYear) {
    //     this.periodFilter = 1;
    //     let month;
    //     let year;
    //     if(toMonth<10){
    //         month = '0' + toMonth;
    //     }else{
    //         month = toMonth;
    //     }
    //     // let month = '10';
    //     //year = '2018';
    //     year = toYear;
    //     let kpiId = this.ReportDetailsData.globalruleid;
    //     //let kpiId = 39412;
    //     this.loadingModalDati2 = true;
    //     this.isLoadedDati2=1;

    //     console.log('getdati2 -> ',kpiId,month,year);

    //     this.apiService.getKpiRawData(kpiId, month, year).subscribe((dati: any) => {
    //         this.fitroDataById2 = dati;
    //         //console.log(dati);
    //         Object.keys(this.fitroDataById2).forEach(key => {
    //             this.fitroDataById2[key].data = JSON.parse(this.fitroDataById2[key].data);
    //             switch (this.fitroDataById2[key].event_state_id) {
    //                 case 1:
    //                     this.fitroDataById2[key].event_state_id = "Originale";
    //                     break;
    //                 case 2:
    //                     this.fitroDataById2[key].event_state_id = "Sovrascritto";
    //                     break;
    //                 case 3:
    //                     this.fitroDataById2[key].event_state_id = "Eliminato";
    //                     break;
    //                 case 4:
    //                     this.fitroDataById2[key].event_state_id = "Correzione";
    //                     break;
    //                 case 5:
    //                     this.fitroDataById2[key].event_state_id = "Correzione eliminata";
    //                     break;
    //                 case 6:
    //                     this.fitroDataById2[key].event_state_id = "Business";
    //                     break;
    //                 default:
    //                     this.fitroDataById2[key].event_state_id = this.fitroDataById2[key].event_state_id;
    //                     break;
    //             }
    //             this.fitroDataById2[key].event_type_id = this.eventTypes[this.fitroDataById2[key].event_type_id] ? this.eventTypes[this.fitroDataById2[key].event_type_id] : this.fitroDataById2[key].event_type_id;
    //             this.fitroDataById2[key].resource_id = this.resources[this.fitroDataById2[key].resource_id] ? this.resources[this.fitroDataById2[key].resource_id] : this.fitroDataById2[key].resource_id;
    //             this.fitroDataById2[key].modify_date = moment(this.fitroDataById2[key].modify_date).format('DD/MM/YYYY HH:mm:ss');
    //             this.fitroDataById2[key].create_date = moment(this.fitroDataById2[key].create_date).format('DD/MM/YYYY HH:mm:ss');
    //             this.fitroDataById2[key].time_stamp = moment(this.fitroDataById2[key].time_stamp).format('DD/MM/YYYY HH:mm:ss');
    //         })
    //         this.getCountCampiData2();

    //         let max = this.countCampiData.length;

    //         Object.keys(this.fitroDataById2).forEach(key => {
    //             let temp = Object.keys(this.fitroDataById2[key].data).length;
    //             if (temp < max) {
    //                 for (let i = 0; i < (max - temp); i++) {
    //                     this.fitroDataById2[key].data['empty#' + i] = '##empty##';
    //                 }
    //             }
    //         })
    //         //console.log('dati', dati);
    //         this.loadingModalDati2 = false;
    //     },
    //     error => {
    //         this.loadingModalDati2 = false;
    //     });
    // }


    // getCountCampiData() {
    //     let maxLength = 0;
    //     this.fitroDataById.forEach(f => {
    //         //let data = JSON.parse(f.data);
    //         if (Object.keys(f.data).length > maxLength) {
    //             maxLength = Object.keys(f.data).length;
    //         }
    //     });
    //     this.countCampiData = [];
    //     for (let i = 1; i <= maxLength; i++) {
    //         this.countCampiData.push(i);
    //     }
    // }

    // getCountCampiData2() {
    //     let maxLength = 0;
    //     this.fitroDataById2.forEach(f => {
    //         //let data = JSON.parse(f.data);
    //         if (Object.keys(f.data).length > maxLength) {
    //             maxLength = Object.keys(f.data).length;
    //         }
    //     });
    //     this.countCampiData = [];
    //     for (let i = 1; i <= maxLength; i++) {
    //         this.countCampiData.push(i);
    //     }
    // }

    // selectedMonth(e){
    //     let stringToSplit = this.monthVar;
    //     let split = stringToSplit.split("/");
    //     let month = split[0];
    //     let year = split[1];

    //     console.log('KPI ID -> ',this.ReportDetailsData.globalruleid,' - Selected Month -> ',month,' - Selected Year -> ',year);

    //     this.selectedmonth = month;
    //     this.selectedyear = year;

    //     this.getdati1(month,year);
    // }

    // selectedMonth2(e){
    //     let stringToSplit = this.monthVar2;
    //     let split = stringToSplit.split("/");
    //     let month = split[0];
    //     let year = split[1];

    //     console.log('KPI ID -> ',this.ReportDetailsData.globalruleid,' - Selected Month -> ',month,' - Selected Year -> ',year);

    //     this.selectedmonth = month;
    //     this.selectedyear = year;

    //     this.getdati2(month,year);
    // }


    chartClicked(e) {
        console.log(e);
    }

}

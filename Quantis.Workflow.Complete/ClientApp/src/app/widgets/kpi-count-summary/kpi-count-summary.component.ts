import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { DateTimeService, WidgetHelpersService } from '../../_helpers';
import { mergeMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { DashboardService, EmitterService } from '../../_services';

@Component({
    selector: 'app-kpi-count-summary',
    templateUrl: './kpi-count-summary.component.html',
    styleUrls: ['./kpi-count-summary.component.scss']
})
export class KpiCountSummaryComponent implements OnInit {
    // INPUT,OUTPUT PARAMS START
    @Input() widgetname: string;
    @Input() url: string;
    @Input() filters: Array<any>;
    @Input() properties: Array<any>;
    @Input() widgetid: number;
    @Input() dashboardid: number;
    @Input() id: number;
    loading: boolean = false;
    kpiCountSummaryWidgetParameters: any;
    setWidgetFormValues: any;
    isDashboardModeEdit: boolean = true;
    sumKPICount: number = 0;
    measure: string = 'Count Summary';
    allMeasuresObj: {number:string};
    @Output() kpiCountSummaryParent = new EventEmitter<any>();
    // INPUT, OUTPUT PARAMS END

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
        console.log('KpiCountSummary ==>', this.widgetname, this.url, this.id, this.widgetid, this.filters, this.properties);
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
            // coming from dashboard component
            this.subscriptionForDataChangesFromParent();
        }
    }

    subscriptionForDataChangesFromParent() {
        this.emitter.getData().subscribe(result => {
            const { type, data } = result;
            if (type === 'kpiCountSummaryChart') {
                let currentWidgetId = data.kpiCountSummaryWidgetParameters.id;
                if (currentWidgetId === this.id) {
                    // updating parameter form widget setValues
                    let kpiCountSummaryFormValues = data.kpiCountSummaryWidgetParameterValues;
                    this.measure = this.allMeasuresObj[data.kpiCountSummaryWidgetParameterValues.Properties.measure];
                    this.period = data.kpiCountSummaryWidgetParameterValues.Filters.date;
                    if (kpiCountSummaryFormValues.Filters.daterange) {
                        kpiCountSummaryFormValues.Filters.daterange = this.dateTime.buildRangeDate(kpiCountSummaryFormValues.Filters.daterange);
                    }
                    this.incompletePeriod = kpiCountSummaryFormValues.Filters.incompletePeriod;
                    this.setWidgetFormValues = kpiCountSummaryFormValues;
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
                this.allMeasuresObj = myWidgetParameters.measures;
                myWidgetParameters.getOrgHierarcy = getOrgHierarcy;
                // Map Params for widget index when widgets initializes for first time
                let newParams = this.widgetHelper.initWidgetParameters(getWidgetParameters, this.filters, this.properties);
                this.measure = this.allMeasuresObj[newParams.Properties.measure];
                this.period = newParams.Filters.date;
                this.incompletePeriod = newParams.Filters.incompletePeriod;
                return this.dashboardService.getWidgetIndex(url, newParams);
            })
        ).subscribe(getWidgetIndex => {
            // populate modal with widget parameters
            let kpiCountSummaryParams;
            if (myWidgetParameters) {
                kpiCountSummaryParams = {
                    type: 'kpiCountSummaryParams',
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
                this.kpiCountSummaryWidgetParameters = kpiCountSummaryParams.data;
                // setting initial Paramter form widget values
                this.setWidgetFormValues = this.widgetHelper.setWidgetParameters(myWidgetParameters, this.filters, this.properties);
            }
            // popular chart data
            if (getWidgetIndex) {
                const chartIndexData = getWidgetIndex.body;
                // third params is current widgets settings current only used when
                // widgets loads first time. may update later for more use cases
                this.updateChart(chartIndexData, null, kpiCountSummaryParams.data);
            }
            this.loading = false;
            this.emitter.loadingStatus(false);
        }, error => {
            this.loading = false;
            this.emitter.loadingStatus(false);
        });
    }

    updateChart(chartIndexData, dashboardComponentData, currentWidgetComponentData) {
        this.sumKPICount = chartIndexData.yvalue || 0;
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

    openModal() {
        this.kpiCountSummaryParent.emit({
            type: 'openKpiSummaryCountModal',
            data: {
                kpiCountSummaryWidgetParameters: this.kpiCountSummaryWidgetParameters,
                setWidgetFormValues: this.setWidgetFormValues,
                isKpiCountSummaryComponent: true
            }
        });
    }

    closeModal() {
        this.emitter.sendNext({ type: 'closeModal' });
    }
}

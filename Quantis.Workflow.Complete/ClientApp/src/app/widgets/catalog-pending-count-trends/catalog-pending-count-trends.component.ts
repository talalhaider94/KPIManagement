import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { CustomTooltips } from '@coreui/coreui-plugin-chartjs-custom-tooltips';
import { DateTimeService, WidgetHelpersService } from '../../_helpers';
import { mergeMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { DashboardService, EmitterService } from '../../_services';

@Component({
    selector: 'app-catalog-pending-count-trends',
    templateUrl: './catalog-pending-count-trends.component.html',
    styleUrls: ['./catalog-pending-count-trends.component.scss']
})
export class CatalogPendingCountTrendsComponent implements OnInit {
    // INPUT,OUTPUT PARAMS START
    @Input() widgetname: string;
    @Input() url: string;
    @Input() filters: Array<any>;
    @Input() properties: Array<any>;
    @Input() widgetid: number;
    @Input() dashboardid: number;
    @Input() id: number;
    loading: boolean = false;
    catalogPendingWidgetParameters: any;
    setWidgetFormValues: any;
    isDashboardModeEdit: boolean = true;
    catalogCount: number = 0;
    // widgetTitle: string = 'Catalog Pending Count Trends';
    measure: string = 'Catalog Pending Count Trends';
    allMeasuresObj: {number:string};
    period: string = '';
    incompletePeriod: boolean = false;
    @Output() catalogPendingParent = new EventEmitter<any>();
    // INPUT, OUTPUT PARAMS END
    constructor(
        private dashboardService: DashboardService,
        private emitter: EmitterService,
        private dateTime: DateTimeService,
        private router: Router,
        private widgetHelper: WidgetHelpersService
    ) { }

    ngOnInit() {
        console.log('CatalogPendingCount ==>', this.widgetname, this.url, this.id, this.widgetid, this.filters, this.properties);
        if (this.router.url.includes('dashboard/public')) {
            this.isDashboardModeEdit = false;
            if (this.url) {
                this.emitter.loadingStatus(true);
                this.getChartParametersAndData(this.url);
            }
            // coming from dashboard component
            this.subscriptionForDataChangesFromParent();
        }
    }

    subscriptionForDataChangesFromParent() {
        this.emitter.getData().subscribe(result => {
            const { type, data } = result;
            if (type === 'catalogPendingChart') {
                let currentWidgetId = data.catalogPendingWidgetParameters.id;
                if (currentWidgetId === this.id) {
                    // updating parameter form widget setValues
                    let catalogPendingFormValues = data.catalogPendingWidgetParameterValues;
                    this.measure = this.allMeasuresObj[data.catalogPendingWidgetParameterValues.Properties.measure];
                    this.period = data.catalogPendingWidgetParameterValues.Filters.date;
                    if (catalogPendingFormValues.Filters.daterange) {
                        catalogPendingFormValues.Filters.daterange = this.dateTime.buildRangeDate(catalogPendingFormValues.Filters.daterange);
                    }
                    this.incompletePeriod = catalogPendingFormValues.Filters.incompletePeriod;
                    this.setWidgetFormValues = catalogPendingFormValues;
                    this.updateChart(data.result.body, data, null);
                }
            }
        });
    }

    // invokes on component initialization
    getChartParametersAndData(url) {
        // these are default parameters need to update this logic
        // might have to make both API calls in sequence instead of parallel
        this.loading = true;
        let myWidgetParameters = null;
        this.dashboardService.getWidgetParameters(url).pipe(
            mergeMap((getWidgetParameters: any) => {
                myWidgetParameters = getWidgetParameters;
                this.allMeasuresObj = myWidgetParameters.measures;
                // Map Params for widget index when widgets initializes for first time
                const newParams = this.widgetHelper.initWidgetParameters(getWidgetParameters, this.filters, this.properties);
                this.measure = this.allMeasuresObj[newParams.Properties.measure];
                this.period = newParams.Filters.date;
                this.incompletePeriod = newParams.Filters.incompletePeriod;
                return this.dashboardService.getWidgetIndex(url, newParams);
            })
        ).subscribe(getWidgetIndex => {
            // populate modal with widget parameters
            console.log('CatalogPendingCountTrendsComponent getWidgetIndex', getWidgetIndex);
            console.log('CatalogPendingCountTrendsComponent myWidgetParameters', myWidgetParameters);

            let catalogPendingParams;
            if (myWidgetParameters) {
                catalogPendingParams = {
                    type: 'catalogPendingParams',
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
                this.catalogPendingWidgetParameters = catalogPendingParams.data;
                // setting initial Paramter form widget values
                this.setWidgetFormValues = this.widgetHelper.setWidgetParameters(myWidgetParameters, this.filters, this.properties);
            }
            // popular chart data
            if (getWidgetIndex) {
                const chartIndexData = getWidgetIndex.body;
                // third params is current widgets settings current only used when
                // widgets loads first time. may update later for more use cases
                this.updateChart(chartIndexData, null, catalogPendingParams.data);
            }
            this.loading = false;
            this.emitter.loadingStatus(false);
        }, error => {
            this.loading = false;
            this.emitter.loadingStatus(false);
        });
    }

    updateChart(chartIndexData, dashboardComponentData, currentWidgetComponentData) {
        this.catalogCount = chartIndexData.yvalue || 0;
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

    openModal() {
        this.catalogPendingParent.emit({
            type: 'openCatalogPendingModal',
            data: {
                catalogPendingWidgetParameters: this.catalogPendingWidgetParameters,
                setWidgetFormValues: this.setWidgetFormValues,
                isCatalogPendingComponent: true
            }
        });
    }

    closeModal() {
        this.emitter.sendNext({ type: 'closeModal' });
    }
}

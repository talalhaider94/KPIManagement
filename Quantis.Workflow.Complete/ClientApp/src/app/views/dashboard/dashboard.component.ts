import { Component, OnInit, ComponentRef, ViewChild, HostListener, Inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { GridsterConfig, GridType, DisplayGrid } from 'angular-gridster2';
import { DashboardService, EmitterService } from '../../_services';
import { DateTimeService } from '../../_helpers';
import { ActivatedRoute } from '@angular/router';
import { DashboardModel, DashboardContentModel, WidgetModel } from '../../_models';
import { Subscription, forkJoin } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../_services/api.service';
// importing chart components
import { DistributionByWorkflowComponent } from '../../widgets/distribution-by-workflow/distribution-by-workflow.component';
import { BarchartComponent } from '../../widgets/barchart/barchart.component';
import { KpiCountSummaryComponent } from '../../widgets/kpi-count-summary/kpi-count-summary.component';
import { CatalogPendingCountTrendsComponent } from '../../widgets/catalog-pending-count-trends/catalog-pending-count-trends.component';
import { DistributionByUserComponent } from '../../widgets/distribution-by-user/distribution-by-user.component';
import { KpiReportTrendComponent } from '../../widgets/kpi-report-trend/kpi-report-trend.component';
import { NotificationTrendComponent } from '../../widgets/notification-trend/notification-trend.component';
import { KpiCountByOrganizationComponent } from '../../widgets/kpi-count-by-organization/kpi-count-by-organization.component';
import { KpiStatusSummaryComponent } from '../../widgets/kpi-status-summary/kpi-status-summary.component';
import { FreeFormReportsWidgetComponent } from '../../widgets/free-form-reports-widget/free-form-reports-widget.component';
import { UUID } from 'angular2-uuid';

@Component({
    templateUrl: 'dashboard.component.html',
    styleUrls: ['dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
    widgetCollection: WidgetModel[];
    options: GridsterConfig;
    dashboardId: number;
    dashboardCollection: DashboardModel;
    dashboardWidgetsArray: DashboardContentModel[] = [];
    emitterSubscription: Subscription; // need to destroy this subscription later
    @ViewChild('widgetParametersModal') public widgetParametersModal: ModalDirective;
    barChartWidgetParameters: any;
    // FORM
    widgetParametersForm: FormGroup;
    submitted: boolean = false;
    dashboardName: string = 'Loading...!';
    // move the component collection to dashboard service to access commonly in multiple components
    // uiidentifier is necessary
    componentCollection = [
        { name: "Distribution by Workflow", componentInstance: DistributionByWorkflowComponent, uiidentifier: "distribution_by_verifica" },
        { name: "Count Trend", componentInstance: BarchartComponent, uiidentifier: "count_trend" },
        { name: "KPI Count Summary", componentInstance: KpiCountSummaryComponent, uiidentifier: "kpi_count_summary" },
        { name: "Catalog Pending Count Trends", componentInstance: CatalogPendingCountTrendsComponent, uiidentifier: "catalog_pending_count_trends" },
        { name: "Distribution by User", componentInstance: DistributionByUserComponent, uiidentifier: "distribution_by_user" },
        { name: "KPI Report Trend", componentInstance: KpiReportTrendComponent, uiidentifier: "kpi_report_trend" },
        { name: "Notification Trend", componentInstance: NotificationTrendComponent, uiidentifier: "notification_trend" },
        { name: "KPI count by Organization", componentInstance: KpiCountByOrganizationComponent, uiidentifier: "kpi_count_by_organization" },
        { name: "KPI Status Summary", componentInstance: KpiStatusSummaryComponent, uiidentifier: "KPIStatusSummary" },
        { name: "Free Form Report", componentInstance: FreeFormReportsWidgetComponent, uiidentifier: "FreeFormReport" },
    ];
    helpText: string = '';
    constructor(
        private dashboardService: DashboardService,
        private apiService: ApiService,
        private _route: ActivatedRoute,
        private emitter: EmitterService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder,
        private dateTime: DateTimeService,
        @Inject(DOCUMENT) private document: Document
    ) { }

    @HostListener('window:scroll', [])
    onWindowScroll() {
        if (document.body.scrollTop > 200 || document.documentElement.scrollTop > 200) {
            this.document.getElementById('widgetsList').classList.add('widgetsPositionFixed');
        } else {
            this.document.getElementById('widgetsList').classList.remove('widgetsPositionFixed');
        }
    }

    outputs = {
        barChartParent: childData => {
            if (childData.type === 'openBarChartModal') {
                this.barChartWidgetParameters = childData.data.barChartWidgetParameters;
                if (this.barChartWidgetParameters) {
                    console.log('CHILD DATA', childData.data.barChartWidgetParameters);
                    setTimeout(() => {
                        this.widgetParametersForm.setValue(childData.data.setWidgetFormValues)
                    });
                }
                this.helpText = this.widgetCollection.find(widget => widget.uiidentifier === 'count_trend').help;
                this.widgetParametersModal.show();
            } else if (childData.type === 'closeModal') {
                this.widgetParametersModal.hide();
            } else if (childData.type === 'changeBarChartWidgetName') {
                let barChartdata = childData.data.barChart;
                let dashboardWidgetsArray = this.dashboardCollection.dashboardwidgets;
                console.log('dashboardWidgetsArray', dashboardWidgetsArray);
                let updatedDashboardWidgetsArray = dashboardWidgetsArray.map(widget => {
                    if (widget.id === barChartdata.id && widget.widgetid === barChartdata.widgetid) {
                        let updatename = {
                            ...widget,
                            widgetname: barChartdata.widgetname,
                        }
                        return updatename;
                    } else {
                        return widget;
                    }
                });
                console.log('updatedDashboardWidgetsArray', updatedDashboardWidgetsArray);
                this.dashboardCollection.dashboardwidgets = updatedDashboardWidgetsArray;
            }
        },
        kpiCountSummaryParent: childData => {
            console.log('kpiCountSummaryParent childData', childData);
            if (childData.type === 'changeKpiCountSummaryWidgetName') {
                let kpiCountSummaryChartdata = childData.data.kpiCountSummaryChart;
                let dashboardWidgetsArray = this.dashboardCollection.dashboardwidgets;
                let updatedDashboardWidgetsArray = dashboardWidgetsArray.map(widget => {
                    if (widget.id === kpiCountSummaryChartdata.id && widget.widgetid === kpiCountSummaryChartdata.widgetid) {
                        let updatename = {
                            ...widget,
                            widgetname: kpiCountSummaryChartdata.widgetname,
                        }
                        return updatename;
                    } else {
                        return widget;
                    }
                });
                this.dashboardCollection.dashboardwidgets = updatedDashboardWidgetsArray;
            }
        },
        verificaDoughnutParent: childData => {
            console.log('verificaDoughnutParent childData', childData);
            if (childData.type === 'changeVerificaDoughnutChartWidgetName') {
                let verificaDoughnutChartdata = childData.data.verificaDoughnutChart;
                let dashboardWidgetsArray = this.dashboardCollection.dashboardwidgets;
                let updatedDashboardWidgetsArray = dashboardWidgetsArray.map(widget => {
                    if (widget.id === verificaDoughnutChartdata.id && widget.widgetid === verificaDoughnutChartdata.widgetid) {
                        let updatename = {
                            ...widget,
                            widgetname: verificaDoughnutChartdata.widgetname,
                        }
                        return updatename;
                    } else {
                        return widget;
                    }
                });
                this.dashboardCollection.dashboardwidgets = updatedDashboardWidgetsArray;
            }
        }
    };

    componentCreated(compRef: ComponentRef<any>) {
        // console.log('Component Created', compRef);
    }

    ngOnInit(): void {
        //Danial: form is unnecessary here in this component now remove it later
        this.widgetParametersForm = this.formBuilder.group({
            GlobalFilterId: [null],
            Properties: this.formBuilder.group({
                charttype: [null],
                aggregationoption: [null],
                measure: [null]
            }),
            Filters: this.formBuilder.group({
                daterange: [null]
            })
        });
        // Grid options
        this.options = {
            gridType: GridType.Fit,
            displayGrid: DisplayGrid.OnDragAndResize,
            pushItems: true,
            swap: false,
            resizable: {
                enabled: true
            },
            draggable: {
                enabled: true,
                ignoreContent: false,
                dropOverItems: false,
                dragHandleClass: "drag-handler",
                ignoreContentClass: "no-drag",
            },
            margin: 10,
            outerMargin: true,
            outerMarginTop: null,
            outerMarginRight: null,
            outerMarginBottom: null,
            outerMarginLeft: null,
            useTransformPositioning: true,
            mobileBreakpoint: 640,
            enableEmptyCellDrop: true,
            emptyCellDropCallback: this.onDrop,
            pushDirections: { north: true, east: true, south: true, west: true },
            itemChangeCallback: this.itemChange.bind(this),
            itemResizeCallback: DashboardComponent.itemResize,
            minCols: 10,
            maxCols: 100,
            minRows: 10,
            maxRows: 100,
        };

        this._route.params.subscribe(params => {
            this.dashboardId = +params["id"];
            this.emitter.loadingStatus(true);
            this.getData(this.dashboardId);
        });
        //Danial: need to improve method to call only dashboard widgets and remove widgets call
        // this.apiService.getSeconds().subscribe((data: any) => {
        // 	var secondsValue = data + '000';
        // 	var seconds = parseInt(secondsValue);
        // 	console.log("Auto Refresh Seconds: ", seconds);

        // 	interval(seconds).subscribe(count => {
        // 		this.getData(this.dashboardId);
        // 	})
        // });
        this.changeWidgetName();
    }

    changeWidgetName() {
        this.emitter.getData().subscribe(data => {
            if (data.type === 'changeWidgetName') {
                // change widget name code start
                let dashboardWidgetsArray = this.dashboardCollection.dashboardwidgets;
                let updatedDashboardWidgetsArray = dashboardWidgetsArray.map(widget => {
                    console.log(widget.id, data.data.id, widget.widgetid, data.data.widgetid);
                    if (widget.id === data.data.id && widget.widgetid === data.data.widgetid) {
                        let updatename = { ...widget, widgetname: data.data.widgetname };
                        return updatename;
                    } else {
                        return widget;
                    }
                });
                this.dashboardCollection.dashboardwidgets = updatedDashboardWidgetsArray;
                // change widget name code end
            }
        })
    }

    getData(dashboardId: number) {
        const getAllWidgets = this.dashboardService.getWidgets();
        const getDashboardWidgets = this.dashboardService.getDashboard(dashboardId);
        forkJoin([getAllWidgets, getDashboardWidgets]).subscribe(result => {
            if (result) {
                const [allWidgets, dashboardData] = result;
                console.log('dashboardData', dashboardData);
                if (allWidgets && allWidgets.length > 0) {
                    this.widgetCollection = allWidgets;
                }
                if (dashboardData) {
                    this.dashboardCollection = dashboardData;
                    this.dashboardName = dashboardData.name;
                    // parsing serialized Json to generate components on the fly
                    // attaching component instance with widget.component key
                    this.parseJson(this.dashboardCollection);
                    // copying array without reference to re-render.
                    this.dashboardWidgetsArray = this.dashboardCollection.dashboardwidgets.slice();
                }
            } else {
                console.log('WHY NO DASHBOARD DATA');
            }
            this.emitter.loadingStatus(false);
        }, error => {
            this.emitter.loadingStatus(false);
            this.toastr.error('Error while fetching dashboards');
            console.error('Get Dashboard Data', error);
        });
    }

    // need to move this to dashboard service as well
    parseJson(dashboardCollection: DashboardModel) {
        // We loop on our dashboardCollection
        dashboardCollection.dashboardwidgets.forEach(widget => {
            // We loop on our componentCollection
            this.componentCollection.forEach(component => {
                // We check if component key in our dashboardCollection
                // is equal to our component name/uiidentifier key in our componentCollection
                if (widget.uiidentifier === component.uiidentifier) {
                    // If it is, we replace our serialized key by our component instance
                    widget.component = component.componentInstance;
                    // this logic needs to be update because in future widget name will be different
                    // need to make this match on the basis on uiidentifier
                    let url = this.widgetCollection.find(myWidget => myWidget.uiidentifier === widget.uiidentifier).url;
                    widget.url = url;
                }
            });
        });
    }

    itemChange() {
        this.dashboardCollection.dashboardwidgets = this.dashboardWidgetsArray;
        // let changedDashboardWidgets: DashboardModel = this.dashboardCollection;
        // this.serialize(changedDashboardWidgets.dashboardwidgets);
    }

    saveDashboard() {
        this.emitter.loadingStatus(true);
        this.dashboardService.updateDashboard(this.dashboardCollection).subscribe(updatedDashboard => {
            this.emitter.loadingStatus(false);
            this.toastr.success('Dashboard saved successfully.');
        }, error => {
            console.error('saveDashboard', error);
            this.emitter.loadingStatus(false);
        });
    }

    serialize(dashboardwidgets) {
        // We loop on our dashboardCollection
        dashboardwidgets.forEach(widget => {
            // We loop on our componentCollection
            this.componentCollection.forEach(component => {
                // We check if component key in our dashboardCollection
                // is equal to our component name key in our componentCollection
                if (widget.widgetname === component.name) {
                    widget.component = component.name;
                }
            });
        });
    }

    onDrop(ev) {
        const componentType = ev.dataTransfer.getData("widgetIdentifier");
        switch (componentType) {
            case "distribution_by_verifica": {
                let doughnutWidget = this.widgetCollection.find(widget => widget.uiidentifier === 'distribution_by_verifica');
                this.dashboardWidgetsArray.push({
                    cols: 4,
                    rows: 4,
                    minItemCols: 2,
                    minItemRows: 4,
                    x: 0,
                    y: 0,
                    component: DistributionByWorkflowComponent,
                    widgetname: doughnutWidget.name,
                    uiidentifier: doughnutWidget.uiidentifier,
                    filters: {}, // need to update this code
                    properties: {},
                    dashboardid: this.dashboardId,
                    widgetid: doughnutWidget.id,
                    id: 0,
                    url: doughnutWidget.url
                });
                this.dashboardCollection.dashboardwidgets = this.dashboardWidgetsArray;
                return this.dashboardWidgetsArray;
            }
            case "count_trend": {
                let countWidget = this.widgetCollection.find(widget => widget.uiidentifier === 'count_trend');
                this.dashboardWidgetsArray.push({
                    cols: 4,
                    rows: 4,
                    minItemCols: 2,
                    minItemRows: 4,
                    x: 0,
                    y: 0,
                    component: BarchartComponent,
                    widgetname: countWidget.name,
                    uiidentifier: countWidget.uiidentifier,
                    filters: {}, // need to update this code
                    properties: {},
                    dashboardid: this.dashboardId,
                    widgetid: countWidget.id,
                    id: 0,
                    url: countWidget.url
                });
                this.dashboardCollection.dashboardwidgets = this.dashboardWidgetsArray;
                return this.dashboardWidgetsArray;
            }
            case "kpi_count_summary": {
                let summaryWidget = this.widgetCollection.find(widget => widget.uiidentifier === 'kpi_count_summary');
                this.dashboardWidgetsArray.push({
                    cols: 3,
                    rows: 3,
                    minItemCols: 3,
                    minItemRows: 3,
                    x: 0,
                    y: 0,
                    component: KpiCountSummaryComponent,
                    widgetname: summaryWidget.name,
                    uiidentifier: summaryWidget.uiidentifier,
                    filters: {}, // need to update this code
                    properties: {},
                    dashboardid: this.dashboardId,
                    widgetid: summaryWidget.id,
                    id: 0,
                    url: summaryWidget.url
                });
                this.dashboardCollection.dashboardwidgets = this.dashboardWidgetsArray;
                return this.dashboardWidgetsArray;
            }
            case "distribution_by_user": {
                let distributionWidget = this.widgetCollection.find(widget => widget.uiidentifier === 'distribution_by_user');
                this.dashboardWidgetsArray.push({
                    cols: 4,
                    rows: 4,
                    minItemCols: 2,
                    minItemRows: 4,
                    x: 0,
                    y: 0,
                    component: DistributionByUserComponent,
                    widgetname: distributionWidget.name,
                    uiidentifier: distributionWidget.uiidentifier,
                    filters: {}, // need to update this code
                    properties: {},
                    dashboardid: this.dashboardId,
                    widgetid: distributionWidget.id,
                    id: 0,
                    url: distributionWidget.url
                });
                this.dashboardCollection.dashboardwidgets = this.dashboardWidgetsArray;
                return this.dashboardWidgetsArray;
            }
            case "catalog_pending_count_trends": {
                let catalogWidget = this.widgetCollection.find(widget => widget.uiidentifier === 'catalog_pending_count_trends');
                this.dashboardWidgetsArray.push({
                    cols: 3,
                    rows: 3,
                    minItemCols: 3,
                    minItemRows: 3,
                    x: 0,
                    y: 0,
                    component: CatalogPendingCountTrendsComponent,
                    widgetname: catalogWidget.name,
                    uiidentifier: catalogWidget.uiidentifier,
                    filters: {}, // need to update this code
                    properties: {},
                    dashboardid: this.dashboardId,
                    widgetid: catalogWidget.id,
                    id: 0,
                    url: catalogWidget.url
                });
                this.dashboardCollection.dashboardwidgets = this.dashboardWidgetsArray;
                return this.dashboardWidgetsArray;
            }
            case "kpi_report_trend": {
                let kpiReportTrendWidget = this.widgetCollection.find(widget => widget.uiidentifier === 'kpi_report_trend');
                this.dashboardWidgetsArray.push({
                    cols: 8,
                    rows: 6,
                    minItemCols: 2,
                    minItemRows: 4,
                    x: 0,
                    y: 0,
                    component: KpiReportTrendComponent,
                    widgetname: kpiReportTrendWidget.name,
                    uiidentifier: kpiReportTrendWidget.uiidentifier,
                    filters: {}, // need to update this code
                    properties: {},
                    dashboardid: this.dashboardId,
                    widgetid: kpiReportTrendWidget.id,
                    id: 0,
                    url: kpiReportTrendWidget.url
                });
                this.dashboardCollection.dashboardwidgets = this.dashboardWidgetsArray;
                return this.dashboardWidgetsArray;
            }
            case "notification_trend": {
                let notificationTrendWidget = this.widgetCollection.find(widget => widget.uiidentifier === 'notification_trend');
                this.dashboardWidgetsArray.push({
                    cols: 4,
                    rows: 4,
                    minItemCols: 2,
                    minItemRows: 4,
                    x: 0,
                    y: 0,
                    component: NotificationTrendComponent,
                    widgetname: notificationTrendWidget.name,
                    uiidentifier: notificationTrendWidget.uiidentifier,
                    filters: {}, // need to update this code
                    properties: {},
                    dashboardid: this.dashboardId,
                    widgetid: notificationTrendWidget.id,
                    id: 0,
                    url: notificationTrendWidget.url
                });
                this.dashboardCollection.dashboardwidgets = this.dashboardWidgetsArray;
                return this.dashboardWidgetsArray;
            }
            case "kpi_count_by_organization": {
                let kpiOragnizationWidget = this.widgetCollection.find(widget => widget.uiidentifier === 'kpi_count_by_organization');
                this.dashboardWidgetsArray.push({
                    cols: 4,
                    rows: 4,
                    minItemCols: 2,
                    minItemRows: 4,
                    x: 0,
                    y: 0,
                    component: NotificationTrendComponent,
                    widgetname: kpiOragnizationWidget.name,
                    uiidentifier: kpiOragnizationWidget.uiidentifier,
                    filters: {}, // need to update this code
                    properties: {},
                    dashboardid: this.dashboardId,
                    widgetid: kpiOragnizationWidget.id,
                    id: 0,
                    url: kpiOragnizationWidget.url
                });
                this.dashboardCollection.dashboardwidgets = this.dashboardWidgetsArray;
                return this.dashboardWidgetsArray;
            }
            case "KPIStatusSummary": {
                let kpiStatusSummaryWidget = this.widgetCollection.find(widget => widget.uiidentifier === 'KPIStatusSummary');
                this.dashboardWidgetsArray.push({
                    cols: 15,
                    rows: 8,
                    minItemCols: 10,
                    minItemRows: 5,
                    x: 0,
                    y: 0,
                    component: KpiStatusSummaryComponent,
                    widgetname: kpiStatusSummaryWidget.name,
                    uiidentifier: kpiStatusSummaryWidget.uiidentifier,
                    filters: {}, // need to update this code
                    properties: {},
                    dashboardid: this.dashboardId,
                    widgetid: kpiStatusSummaryWidget.id,
                    id: 0,
                    url: kpiStatusSummaryWidget.url
                });
                this.dashboardCollection.dashboardwidgets = this.dashboardWidgetsArray;
                return this.dashboardWidgetsArray;
            }
            case "FreeFormReport": {
                let freeFormReportWidget = this.widgetCollection.find(widget => widget.uiidentifier === 'FreeFormReport');
                this.dashboardWidgetsArray.push({
                    cols: 15,
                    rows: 8,
                    minItemCols: 10,
                    minItemRows: 5,
                    x: 0,
                    y: 0,
                    component: FreeFormReportsWidgetComponent,
                    widgetname: freeFormReportWidget.name,
                    uiidentifier: freeFormReportWidget.uiidentifier,
                    filters: {}, // need to update this code
                    properties: {},
                    dashboardid: this.dashboardId,
                    widgetid: freeFormReportWidget.id,
                    id: 0,
                    url: freeFormReportWidget.url
                });
                this.dashboardCollection.dashboardwidgets = this.dashboardWidgetsArray;
                return this.dashboardWidgetsArray;
            }
        }
    }

    changedOptions() {
        this.options.api.optionsChanged();
    }

    removeItem(item) {
        this.dashboardWidgetsArray.splice(
            this.dashboardWidgetsArray.indexOf(item),
            1
        );
        this.itemChange();
    }

    static itemResize(item, itemComponent) {
        // console.info('itemResized', item, itemComponent);
    }

    onWidgetParametersFormSubmit() {
        let formValues = this.widgetParametersForm.value;
        let startDate = this.dateTime.moment(formValues.Filters.daterange[0]).format('MM/YYYY');
        let endDate = this.dateTime.moment(formValues.Filters.daterange[1]).format('MM/YYYY');
        formValues.Filters.daterange = `${startDate}-${endDate}`;
        debugger
        const { url } = this.barChartWidgetParameters;
        this.emitter.loadingStatus(true);
        this.dashboardService.getWidgetIndex(url, formValues).subscribe(result => {
            this.emitter.sendNext({
                type: 'barChart',
                data: {
                    result,
                    barChartWidgetParameters: this.barChartWidgetParameters,
                    barChartWidgetParameterValues: formValues
                }
            });
            this.emitter.loadingStatus(false);
        }, error => {
            console.error('getWidgetIndex', error);
            this.emitter.loadingStatus(false);
        })
    }

    cloneWidget(widget) {
        let cloneWidget = {
            ...widget,
            id: UUID.UUID(),
            x: 0,
            y: 0,
        }
        this.dashboardWidgetsArray.push(cloneWidget);
        this.dashboardCollection.dashboardwidgets = this.dashboardWidgetsArray;
    }
}

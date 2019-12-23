import { Component, OnInit, ComponentRef, ViewChild, HostListener, Inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { GridsterConfig, GridType, DisplayGrid } from 'angular-gridster2';
import { DashboardService, EmitterService, FreeFormReportService,ApiService } from '../../../_services';
import { ActivatedRoute } from '@angular/router';
import { DashboardModel, DashboardContentModel, WidgetModel, ComponentCollection } from '../../../_models';
import { forkJoin } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { DateTimeService, removeNullKeysFromObject, exportChartButton } from '../../../_helpers';
import { TreeViewComponent, NodeSelectEventArgs } from '@syncfusion/ej2-angular-navigations';
// importing chart components
import { DistributionByWorkflowComponent } from '../../../widgets/distribution-by-workflow/distribution-by-workflow.component';
import { BarchartComponent } from '../../../widgets/barchart/barchart.component';
import { KpiCountSummaryComponent } from '../../../widgets/kpi-count-summary/kpi-count-summary.component';
import { CatalogPendingCountTrendsComponent } from '../../../widgets/catalog-pending-count-trends/catalog-pending-count-trends.component';
import { DistributionByUserComponent } from '../../../widgets/distribution-by-user/distribution-by-user.component';
import { KpiReportTrendComponent } from '../../../widgets/kpi-report-trend/kpi-report-trend.component';
import { NotificationTrendComponent } from '../../../widgets/notification-trend/notification-trend.component';
import { KpiCountByOrganizationComponent } from '../../../widgets/kpi-count-by-organization/kpi-count-by-organization.component';
import { KpiStatusSummaryComponent } from '../../../widgets/kpi-status-summary/kpi-status-summary.component';
import { FreeFormReportsWidgetComponent } from '../../../widgets/free-form-reports-widget/free-form-reports-widget.component';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import * as moment from 'moment';
import { chartExportTranslations,getDistinctArray } from '../../../_helpers';
import * as Highcharts from 'highcharts';

@Component({
	selector: 'app-public',
	templateUrl: './public.component.html',
	styleUrls: ['./public.component.scss']
})
export class PublicComponent implements OnInit {
	widgetCollection: WidgetModel[];
	options: GridsterConfig;
	dashboardId: number;
	dashboardCollection: DashboardModel;
	dashboardWidgetsArray: DashboardContentModel[] = [];
	cloneDashboardWidgetsArrayState: any = []; // need to destroy this subscription later
	@ViewChild('widgetParametersModal') public widgetParametersModal: ModalDirective;
	barChartWidgetParameters: any;

	@ViewChild('organizationTree') organizationTree: TreeViewComponent;
	public treeFields: any = {
		dataSource: [],
		id: 'id',
		text: 'name',
		child: 'children',
		title: 'name'
	};
	// preSelectedNodes = ['1075', '1000', '1065', '1055', '1090', '1050', '1005', '1015', '1085', '1080', '1020', '1001'];
	preSelectedNodes = [];
	treeDataFields: Object;
	allLeafNodesIds = [];
	uncheckedNodes = [];
	datiGrezzi = [];
	from_changed;
	to_changed;
	kpiId;
	startDate;
	endDate;
	selectedday;
	selectedmonth;
	selectedyear;
	months = [];
	monthVar;
	isSelectedPeriod=0;
	highcharts = Highcharts;

	isLoadedDati=0;
	loadingModalDati: boolean = false;
	public periodFilter: number;
	countCampiData = [];
    eventTypes: any = {};
	resources: any = {};
	
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
	
	// FORM
	widgetParametersForm: FormGroup;
	submitted: boolean = false;
	// move to Dashboard service
	componentCollection: Array<ComponentCollection> = [
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
	showDateRangeInFilters: boolean = false;
	showCustomDate: boolean = false;
  showEventCol: boolean = true;
	isBarChartComponent: boolean = false;
	isKpiCountSummaryComponent: boolean = false;
	isverificaDoughnutComponent: boolean = false;
	isCatalogPendingComponent: boolean = false;
	isNotificationTrendComponent: boolean = false;
	isKpiReportTrendComponent: boolean = false;
	isKpiCountOrgComponent: boolean = false;
	isDistributionByUserComponent: boolean = false;
	isKpiStatusSummaryComponent: boolean = false;
	isFreeFormReportComponent: boolean = false;
	dashboardName: string = 'Loading...!';
	allContractParties: Array<any> = [{ key: '', value: 'Select Contract Parties' }];
	filterContracts: Array<any> = [{ key: '', value: 'Select Contracts' }];
	filterKpis: Array<any> = [{ key: '', value: `Select KPI's` }];

	allContractParties1: Array<any> = [{ key: '', value: 'Select Contract Parties' }];
	filterContracts1: Array<any> = [{ key: '', value: 'Select Contracts' }];
	filterKpis1: Array<any> = [{ key: '', value: `Select KPI's` }];

	loadingFiltersDropDown: boolean = false;
	loadingModalForm: boolean = false;
	parametersArray: FormArray;
	groupReportCheck: boolean = false;

	dayDrillPeriod;
	isDayDrill=0;
	dayChartUpdateFlag: boolean = true;

	@ViewChild('kpiReportDrillDownTableModal') public kpiReportDrillDownTableModal: ModalDirective;
	@ViewChild('bsiChartModal') public bsiChartModal: ModalDirective;
	kpiReportDrillDownTable: any;
	constructor(
		private dashboardService: DashboardService,
		private apiService: ApiService,
        private toastr: ToastrService,
		private _route: ActivatedRoute,
		private emitter: EmitterService,
		private formBuilder: FormBuilder,
		private dateTime: DateTimeService,
		private _$localeService: BsLocaleService,
		private _freeFormReportService: FreeFormReportService,
		@Inject(DOCUMENT) private document: Document
	) {
		this._$localeService.use('it');
	}

	@HostListener('window:scroll', [])
	onWindowScroll() {
		if (document.body.scrollTop > 200 || document.documentElement.scrollTop > 200) {
			this.document.getElementById('widgetsList').classList.add('widgetsPositionFixed');
			this.document.getElementById('widgetsList').classList.add('w-95p');
		} else {
			this.document.getElementById('widgetsList').classList.remove('widgetsPositionFixed');
			this.document.getElementById('widgetsList').classList.remove('w-95p');
		}
	}

	showWidgetsModalAndSetFormValues(childData, identifier) {
		if(!this.isFreeFormReportComponent){
			this.parametersArray = this.widgetParametersForm.get('Properties').get('parameters') as FormArray;
			while (this.parametersArray.length !== 0) {
				this.parametersArray.removeAt(0)
			}
		}
		if (this.barChartWidgetParameters) {
			if (this.barChartWidgetParameters.allContractParties) {
				/* this.allContractParties= [{ key: '', value: 'Select Contract Parties' }]; */
				this.allContractParties = [{ key: '', value: 'Select Contract Parties' }, ...this.barChartWidgetParameters.allContractParties];
			}
			if (this.barChartWidgetParameters.allContracts) {
				this.filterContracts= [{ key: '', value: 'Select Contracts' }, ... this.barChartWidgetParameters.allContracts];
				// this.filterContracts = [...this.filterContracts];
			}
			if (this.barChartWidgetParameters.allKpis) {
				this.filterKpis= [{ key: '', value: 'Select KPI' }, ...this.barChartWidgetParameters.allKpis];
				// this.filterKpis = [...this.filterKpis];
				this.widgetParametersForm.get('Filters.contracts').enable();
				this.widgetParametersForm.get('Filters.kpi').enable();
			}
			if (this.barChartWidgetParameters.allContractParties1) {
				/* this.allContractParties1 = [...this.allContractParties1, ...this.barChartWidgetParameters.allContractParties1]; */
				this.allContractParties1 = [{ key: '', value: 'Select Contract Parties' }, ...this.barChartWidgetParameters.allContractParties1];
			}
			if (this.barChartWidgetParameters.allContracts1) {
				/* this.filterContracts1 = [...this.filterContracts1, ...this.barChartWidgetParameters.allContracts1]; */
				this.filterContracts1 = [{ key: '', value: 'Select Contracts' }, ...this.barChartWidgetParameters.allContracts1];
			}
			if (this.barChartWidgetParameters.allKpis1) {
				/* this.filterKpis1 = [...this.filterKpis1, ...this.barChartWidgetParameters.allKpis1]; */
				this.filterKpis1 = [{ key: '', value: 'Select KPI' }, ...this.barChartWidgetParameters.allKpis1];
				this.widgetParametersForm.get('Filters.contracts1').enable();
				this.widgetParametersForm.get('Filters.kpi1').enable();
			}
			if (this.barChartWidgetParameters.getOrgHierarcy && this.barChartWidgetParameters.getOrgHierarcy.length > 0) {
				// debugger
				this.treeDataFields = { dataSource: this.barChartWidgetParameters.getOrgHierarcy, id: 'id', text: 'name', title: 'name', child: 'children' };
				// this.preSelectedNodes = this.barChartWidgetParameters.getOrgHierarcy.map (org => org.id);	
				this.preSelectedNodes = [];
				if(Array.isArray(childData.setWidgetFormValues.Filters.organizations)) {
					// debugger
					const allOrganizationIds = childData.setWidgetFormValues.Filters.organizations.map(orgId => orgId.toString());
					setTimeout(() => {
						//this.organizationTree.checkAll();
						this.preSelectedNodes = allOrganizationIds;
						// console.log('ORG TREE CHECKD NODES 11: ', this.organizationTree.getAllCheckedNodes());
						console.log('ORG TREE CHECKD NODES 22: ', this.organizationTree.checkedNodes);
						console.log('this.preSelectedNodes If', this.preSelectedNodes);
					}, 50);
				} else {
					if(childData.setWidgetFormValues.Filters.organizations) {
						let allOrganizationIds;
						allOrganizationIds = childData.setWidgetFormValues.Filters.organizations.split(',');
						setTimeout(() => { this.preSelectedNodes = allOrganizationIds; }, 50);
					}
				}
				// empty leaf node array to aviod pilling up values
				// this.allLeafNodesIds = [];
				// this.getAllLeafNodesIds(this.barChartWidgetParameters.getOrgHierarcy);
				// console.log('this.allLeafNodesIds', this.allLeafNodesIds);
			}
			//Danial: TODO: add isFreeFormReportCheck and getReportQueryDetailByID is no longer being used i suppose
			// need to verify
			if (this.barChartWidgetParameters.getReportQueryDetailByID) {
				const params = this.barChartWidgetParameters.getReportQueryDetailByID.parameters;
				params.map(p => this.addParameters(p)); // pushing in formGroup Controls array 
				// childData.setWidgetFormValues.parameters = params;
			}
			if (this.barChartWidgetParameters.filters && this.barChartWidgetParameters.filters.groupReportCheck) {
				this.groupReportCheck = (this.barChartWidgetParameters.filters.groupReportCheck === 'true');
			}
			
			if(childData.setWidgetFormValues.Properties.hasOwnProperty('measure') && !!childData.setWidgetFormValues.Properties.measure) {
				console.log('childData.setWidgetFormValues', childData.setWidgetFormValues);
				childData.setWidgetFormValues.Properties.measure = childData.setWidgetFormValues.Properties.measure.toString(); 
			}
			this.updateDashboardWidgetsArray(this.barChartWidgetParameters.id, childData.setWidgetFormValues);
			setTimeout(() => {
				console.log('childData.setWidgetFormValues', childData.setWidgetFormValues);
				this.widgetParametersForm.patchValue(childData.setWidgetFormValues)
			});
		}
		this.helpText = this.widgetCollection.find(widget => widget.uiidentifier === identifier).help;
		this.widgetParametersModal.show();
	}

	showPropertyTab(properties) {
		return (Object.keys(properties).length) ? true : false;
	}
	showFilterTab(filters) {
		return (Object.keys(filters).length) ? true : false;
	}

	emptyFormParametersModal(params) {
		if(!params) {
			this.toastr.info('Please refresh page. form parameters are not available');
			return;
		}
	}

	outputs = {
		barChartParent: childData => {
			console.log('barChartParent childData', childData);
			if (childData.type === 'openBarChartModal') {
				// this.barChartWidgetParameters should be a generic name
				this.emptyFormParametersModal(childData.data.barChartWidgetParameters);
				this.barChartWidgetParameters = childData.data.barChartWidgetParameters;
				// setting the isBarChartComponent value to true on openning modal so that their
				// state can be saved in their own instance when closing
				this.isBarChartComponent = childData.data.isBarChartComponent;
				this.showWidgetsModalAndSetFormValues(childData.data, 'count_trend');
			}
		},
		kpiCountSummaryParent: childData => {
			console.log('kpiCountSummaryParent childData', childData);
			if (childData.type === 'openKpiSummaryCountModal') {
				this.barChartWidgetParameters = childData.data.kpiCountSummaryWidgetParameters;
				this.isKpiCountSummaryComponent = childData.data.isKpiCountSummaryComponent;
				this.showWidgetsModalAndSetFormValues(childData.data, 'kpi_count_summary');
			}
		},
		verificaDoughnutParent: childData => {
			console.log('verificaDoughnutParent childData', childData);
			if (childData.type === 'openVerificaDoughnutChartModal') {
				this.barChartWidgetParameters = childData.data.verificaDoughnutChartWidgetParameters;
				this.isverificaDoughnutComponent = childData.data.isverificaDoughnutComponent;
				this.showWidgetsModalAndSetFormValues(childData.data, 'distribution_by_verifica');
			}
		},
		catalogPendingParent: childData => {
			console.log('catalogPendingParent childData', childData);
			if (childData.type === 'openCatalogPendingModal') {
				this.barChartWidgetParameters = childData.data.catalogPendingWidgetParameters;
				this.isCatalogPendingComponent = childData.data.isCatalogPendingComponent;
				this.showWidgetsModalAndSetFormValues(childData.data, 'catalog_pending_count_trends');
			}
		},
		notificationTrendParent: childData => {
			console.log('notificationTrendParent childData', childData);
			if (childData.type === 'openNotificationTrendModal') {
				this.barChartWidgetParameters = childData.data.notificationTrendWidgetParameters;
				this.isNotificationTrendComponent = childData.data.isNotificationTrendComponent;
				this.showWidgetsModalAndSetFormValues(childData.data, 'notification_trend');
			}
		},
		kpiReportTrendParent: childData => {
			console.log('kpiReportTrendParent childData', childData);
			if (childData.type === 'openKpiReportTrendModal') {
				this.emptyFormParametersModal(childData.data.kpiReportTrendWidgetParameters);
				this.barChartWidgetParameters = childData.data.kpiReportTrendWidgetParameters;
				this.isKpiReportTrendComponent = childData.data.isKpiReportTrendComponent;
				this.showWidgetsModalAndSetFormValues(childData.data, 'kpi_report_trend');
			}
			if (childData.type === 'openDayDrillDownTable') {
				this.isDayDrill=0;
				this.bsiChartModal.show();
				this.kpiReportDrillDownTable = childData.data.setWidgetFormValues;

				///////////////////////////////////////////////////

				this.months.length = 0;
				if(this.kpiReportDrillDownTable.Filters.kpi){
					this.kpiId = this.kpiReportDrillDownTable.Filters.kpi;
				}
				else{
					this.kpiId = 0;
				}
				if(this.kpiReportDrillDownTable.Filters.daterange){
					this.startDate = this.kpiReportDrillDownTable.Filters.daterange[0];
					this.endDate = this.kpiReportDrillDownTable.Filters.daterange[1];
				}
				else{
					this.startDate = this.kpiReportDrillDownTable.Filters.startDate;
					this.endDate = this.kpiReportDrillDownTable.Filters.endDate;
				}
				
				
				this.startDate = new Date(this.startDate).toUTCString();
				this.endDate = new Date(this.endDate).toUTCString();

				///////////////////// From Month and Year ////////////////////
				let stringToSplit = this.startDate;
				let split = stringToSplit.split(",");

				let extra = split[1];
				let fromSplit = extra.split(" ");

				let day = fromSplit[1];
				let month = fromSplit[2];
				let fromMonth = moment().month(month).format("M");

				let from_month = +fromMonth;
				from_month=from_month+1;

				let fromYear = fromSplit[3];

				let fromDateString = day+'/'+fromMonth+'/'+fromYear;
				
				///////////////////// To Month and Year ////////////////////

				let stringToSplit2 = this.endDate;
				let split2 = stringToSplit2.split(",");

				let extra2 = split2[1];
				let toSplit = extra2.split(" ");

				let day2 = toSplit[1];
				let month2 = toSplit[2];
				let toMonth = moment().month(month2).format("M");
				let toYear = toSplit[3];

				let to_month = +toMonth;
				to_month=to_month+1;

				console.log(to_month,toYear);

				this.selectedmonth = to_month;
				this.selectedyear = toYear;

				/////////////////////////////////////////////

				let toDateString = day2+'/'+toMonth+'/'+toYear;

				console.log('fromtomonths -> ',fromDateString,toDateString);

				var fromCheck = moment(fromDateString, 'DD/MM/YYYY').add(1, 'M');
        		var toCheck = moment(toDateString, 'DD/MM/YYYY').add(1, 'M');

				while(toCheck > fromCheck || fromCheck.format('M') === toCheck.format('M')){
					let monthyear = fromCheck.format('MM') + '/' + fromCheck.format('YYYY');
					this.months.push(monthyear);
					fromCheck.add(1,'month');
				}

				console.log('months -> ',this.months);

			}
			if (childData.type === 'openKpiReportDrillDownTable') {
				this.kpiReportDrillDownTableModal.show();
				this.kpiReportDrillDownTable = childData.data.setWidgetFormValues;

				///////////////////////////////////////////////////

				this.months.length = 0;
				if(this.kpiReportDrillDownTable.Filters.kpi){
					this.kpiId = this.kpiReportDrillDownTable.Filters.kpi;
				}
				else{
					this.kpiId = 0;
				}
				if(this.kpiReportDrillDownTable.Filters.daterange){
					this.startDate = this.kpiReportDrillDownTable.Filters.daterange[0];
					this.endDate = this.kpiReportDrillDownTable.Filters.daterange[1];
				}
				else{
					this.startDate = this.kpiReportDrillDownTable.Filters.startDate;
					this.endDate = this.kpiReportDrillDownTable.Filters.endDate;
				}
				
				
				this.startDate = new Date(this.startDate).toUTCString();
				this.endDate = new Date(this.endDate).toUTCString();

				///////////////////// From Month and Year ////////////////////
				let stringToSplit = this.startDate;
				let split = stringToSplit.split(",");

				let extra = split[1];
				let fromSplit = extra.split(" ");

				let day = fromSplit[1];
				let month = fromSplit[2];
				let fromMonth = moment().month(month).format("M");

				let from_month = +fromMonth;
				from_month=from_month+1;

				let fromYear = fromSplit[3];

				let fromDateString = day+'/'+fromMonth+'/'+fromYear;
				
				///////////////////// To Month and Year ////////////////////

				let stringToSplit2 = this.endDate;
				let split2 = stringToSplit2.split(",");

				let extra2 = split2[1];
				let toSplit = extra2.split(" ");

				let day2 = toSplit[1];
				let month2 = toSplit[2];
				let toMonth = moment().month(month2).format("M");
				let toYear = toSplit[3];

				let to_month = +toMonth;
				to_month=to_month+1;

				console.log(to_month,toYear);

				this.selectedmonth = to_month;
				this.selectedyear = toYear;

				/////////////////////////////////////////////

				let toDateString = day2+'/'+toMonth+'/'+toYear;

				console.log('fromtomonths -> ',fromDateString,toDateString);

				var fromCheck = moment(fromDateString, 'DD/MM/YYYY').add(1, 'M');
        		var toCheck = moment(toDateString, 'DD/MM/YYYY').add(1, 'M');

				while(toCheck > fromCheck || fromCheck.format('M') === toCheck.format('M')){
					let monthyear = fromCheck.format('MM') + '/' + fromCheck.format('YYYY');
					this.months.push(monthyear);
					fromCheck.add(1,'month');
				}

				console.log('months -> ',this.months);

				/////////////////////////////////////////////
				this.isSelectedPeriod=0;
				this.getdati1(this.kpiId,to_month,toYear)
			}

		},
		kpiCountOrgParent: childData => {
			console.log('kpiCountOrgParent childData', childData);
			if (childData.type === 'openKpiCountOrgModal') {
				this.barChartWidgetParameters = childData.data.kpiCountOrgWidgetParameters;
				this.isKpiCountOrgComponent = childData.data.isKpiCountOrgComponent;
				this.showWidgetsModalAndSetFormValues(childData.data, 'kpi_count_by_organization');
			}
		},
		distributionByUserParent: childData => {
			console.log('distributionByUserParent childData', childData);
			if (childData.type === 'openDistributionByUserModal') {
				this.barChartWidgetParameters = childData.data.distributionByUserWidgetParameters;
				this.isDistributionByUserComponent = childData.data.isDistributionByUserComponent;
				this.showWidgetsModalAndSetFormValues(childData.data, 'distribution_by_user');
			}
		},
		kpiStatusSummaryParent: childData => {
			console.log('kpiStatusSummaryParent childData', childData);
			if (childData.type === 'openKpiStatusSummaryModal') {
				this.barChartWidgetParameters = childData.data.kpiStatusSummaryWidgetParameters;
				this.isKpiStatusSummaryComponent = childData.data.isKpiStatusSummaryComponent;
				this.showWidgetsModalAndSetFormValues(childData.data, 'KPIStatusSummary');
			}
		},
		freeFormReportParent: childData => {
			console.log('freeFormReportParent childData', childData);
			if (childData.type === 'openFreeFormReportModal') {
				this.barChartWidgetParameters = childData.data.freeFormReportWidgetParameters;
				this.isFreeFormReportComponent = childData.data.isFreeFormReportComponent;
				this.showWidgetsModalAndSetFormValues(childData.data, 'FreeFormReport');
			}
		},
	};

	componentCreated(compRef: ComponentRef<any>) {
	} 

	dayChartOptions = {
        lang: chartExportTranslations,
        credits: false,
        title: {
            text: 'KPI Report'
        },
        xAxis: {
            type: 'date',
            categories: []
            // categories: ['10/18', '11/18', '12/18', '01/19', '02/19']
        },
        yAxis: {
            title: {
                text: 'Percent'
            }
        },
        plotOptions: {
            series: {
                dataLabels: {
                    enabled: true,
                },
                point: {
                    events: {
                        click: function () {
                            
                            this.bar_period = this.category;
                            this.bar_value = this.y;
                            alert('Period: ' + this.bar_period + ', Value: ' + this.bar_value);
                           
                        }
                    }
                }
            }
        },
        tooltip: {
            enabled: true,
            crosshairs: true
        },
        series: [],
        exporting: exportChartButton
	};
	
	ngOnInit(): void {
		this.widgetParametersForm = this.formBuilder.group({
			GlobalFilterId: [null],
			Properties: this.formBuilder.group({
				charttype: [null],
				aggregationoption: [null],
				measure: [null],
				parameters: this.formBuilder.array([]),
			}),
			Filters: this.formBuilder.group({
				daterange: [null],
				startDate: [null],
				endDate: [null],
				dateTypes: [null],
				date: [null],
				contractParties: [null],
				contracts: [null],
				kpi: [null],
				incompletePeriod: [false],
				groupReportCheck: [false],
				contractParties1: [null],
				contracts1: [null],
				kpi1: [null],
			}),
			// Note: [null],
		});
		this.widgetParametersForm.get('Filters.contracts').disable();
		this.widgetParametersForm.get('Filters.kpi').disable();
		this.widgetParametersForm.get('Filters.contracts1').disable();
		this.widgetParametersForm.get('Filters.kpi1').disable();
		// Grid options
		this.options = {
			gridType: GridType.Fit,
			displayGrid: DisplayGrid.None,
			pushItems: true,
			swap: false,
			resizable: {
				enabled: false
			},
			draggable: {
				enabled: false,
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
			pushDirections: { north: true, east: true, south: true, west: true },
			itemChangeCallback: this.itemChange.bind(this),
			minCols: 10,
			maxCols: 100,
			minRows: 10,
			maxRows: 100,
			scrollSensitivity: 10,
			scrollSpeed: 20,
		};

		this._route.params.subscribe(params => {
			this.dashboardId = +params["id"];
			this.emitter.loadingStatus(true);
			this.getData(this.dashboardId); /////
		});

		this.widgetParametersForm.get('Filters').get('dateTypes').valueChanges.subscribe((value) => {
			console.log('Date Type Filter', value);
			if (value === '0') {
				this.showDateRangeInFilters = true;
			} else {
				this.showDateRangeInFilters = false;
			}
		});
		this.widgetParametersForm.get('Filters').get('groupReportCheck').valueChanges.subscribe((value) => {
			this.groupReportCheck = value;
		});
		this.closeModalSubscription();
	}

	addParameters(item): void {
      this.parametersArray = this.widgetParametersForm.get('Properties').get('parameters') as FormArray;
      console.log('before',this.parametersArray);
		while (this.parametersArray.length !== 0) {
			this.parametersArray.removeAt(0)
      }
      console.log('after',this.parametersArray);
		this.parametersArray.push(this.formBuilder.group({
			key: item.key,
			value: item.value
		}));
	}

	getData(dashboardId: number) {
		const getAllWidgets = this.dashboardService.getWidgets();
		const getDashboardWidgets = this.dashboardService.getDashboard(dashboardId);
		// const getOrgHierarcy = this.dashboardService.GetOrganizationHierarcy();

		forkJoin([getAllWidgets, getDashboardWidgets]).subscribe(result => {
			if (result) {
				const [allWidgets, dashboardData] = result;
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
		})

	}

	parseJson(dashboardCollection: DashboardModel) {
		// We loop on our dashboardCollection
		dashboardCollection.dashboardwidgets.forEach(widget => {
			// We loop on our componentCollection
			this.componentCollection.forEach(component => {
				// We check if component key in our dashboardCollection
				// is equal to our component name/uiidentifier key in our componentCollection
				// if (widget.component === component.name) {
				if (widget.uiidentifier === component.uiidentifier) {
					// If it is, we replace our serialized key by our component instance
					widget.component = component.componentInstance;
					// this logic needs to be update because in future widget name will be different
					// need to make this match on the basis on uiidentifier
					// let url = this.widgetCollection.find(myWidget => myWidget.name === widget.widgetname).url;
					let url = this.widgetCollection.find(myWidget => myWidget.uiidentifier === widget.uiidentifier).url;
					widget.url = url;
				}
			});
		});
	}

	itemChange() {
		this.dashboardCollection.dashboardwidgets = this.dashboardWidgetsArray;
		window.dispatchEvent(new Event('resize'));
		// let changedDashboardWidgets: DashboardModel = this.dashboardCollection;
		// this.serialize(changedDashboardWidgets.dashboardwidgets);
	}

	saveDashboardState() {
		this.emitter.loadingStatus(true);
		console.log('this.cloneDashboardWidgetsArrayState', this.cloneDashboardWidgetsArrayState);
		let params = this.cloneDashboardWidgetsArrayState.map(widget => {
			if (Object.keys(widget.filters).length > 0) {
				if (widget.filters.hasOwnProperty('startDate') && widget.filters.hasOwnProperty('endDate')) {
					widget.filters.daterange = this.dateTime.getStringDateRange(widget.filters.startDate, widget.filters.endDate);
					delete widget.filters.startDate;
					delete widget.filters.endDate;
				}
				if (widget.filters.hasOwnProperty('groupReportCheck')) {
					widget.filters.groupReportCheck = widget.filters.groupReportCheck.toString(); 
				}
				if (widget.filters.hasOwnProperty('incompletePeriod')) {
					widget.filters.incompletePeriod = widget.filters.incompletePeriod.toString();
				}
			}
			return {
				id: widget.id,
				Filters: widget.filters,
				Properties: widget.properties
			}
		});
		const saveWidgetParams = params.map(param => removeNullKeysFromObject(param));
		this.dashboardService.saveDashboardState(saveWidgetParams).subscribe(result => {
			this.emitter.loadingStatus(false);
			this.toastr.success('Dashboard state saved successfully');
			console.log('saveDashboardState', result);
		}, error => {
			this.emitter.loadingStatus(false);
			this.toastr.error('Error while saving dashboard state');
			console.error('saveDashboardState', error);
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

	organizationTreeNodeCheckEvent($event) {
		console.log('OrganizationTreeNode CheckEvent', $event);
		console.log("this.organizationTree.checkedNodes: ", this.organizationTree.checkedNodes);
		console.log("this.organizationTree.getAllCheckedNodes(): ", this.organizationTree.getAllCheckedNodes());
		console.log("this.allLeafNodesIds: ", this.allLeafNodesIds);
		this.uncheckedNodes = this.allLeafNodesIds.filter(value => {
			const truthy = this.organizationTree.checkedNodes.includes(value.toString());
			return !truthy;
		});
		console.log('this.uncheckedNodes', this.uncheckedNodes)
	}

	organizationTreeNodeSelected(e: NodeSelectEventArgs) {
		console.log('OrganizationTreeNode Selected', e);
		console.log("The selected node's id: ", this.organizationTree.selectedNodes);
	}

	getAllLeafNodesIds(complexJson) {
		if (complexJson) {
			complexJson.forEach((item: any) => {
				if (item.children) {
					this.getAllLeafNodesIds(item.children);
				} else {
					this.allLeafNodesIds.push(item.id);
				}
			});
		}
	}

	onWidgetParametersFormSubmit() {
		this.loadingModalForm = true;
		this.emitter.loadingStatus(true);
		const formValues = this.widgetParametersForm.value;
		let startDate;
		let endDate;
		if (formValues.Filters.dateTypes === '0') {
			let WidgetDateAndTime = this.dateTime.WidgetDateAndTime(formValues.Filters.startDate, formValues.Filters.endDate, formValues.Filters.incompletePeriod, this.isKpiReportTrendComponent);
			startDate = WidgetDateAndTime.startDate;
			endDate = WidgetDateAndTime.endDate;
			//incompletePeriod = WidgetDateAndTime.incompletePeriod;
			/* startDate = this.dateTime.moment(formValues.Filters.startDate).format('MM/YYYY');
			endDate = this.dateTime.moment(formValues.Filters.endDate).format('MM/YYYY'); */
		} else if (!!formValues.Filters.dateTypes) {
			let timePeriodRange = this.dateTime.timePeriodRange(formValues.Filters.dateTypes, formValues.Filters.incompletePeriod, this.isKpiReportTrendComponent);
			startDate = timePeriodRange.startDate;
			endDate = timePeriodRange.endDate;
		}
		if (startDate && endDate) {
			// delete formValues.Filters.dateTypes;
			formValues.Filters.daterange = `${startDate}-${endDate}`;
		} else {
			// formValues.Filters.daterange = null;
			delete formValues.Filters.daterange;
		}
		if (formValues.Filters.date) {
			if (typeof formValues.Filters.date !== 'string') {
				formValues.Filters.date = this.dateTime.moment(formValues.Filters.date).format('MM/YYYY');
			}
			delete formValues.Filters.daterange;
		}

		delete formValues.Filters.startDate;
		delete formValues.Filters.endDate;
		// Organization hierarchy as Customers
		if (this.organizationTree) {
			if(this.organizationTree.checkedNodes && this.organizationTree.checkedNodes.length > 0){
				formValues.Filters.organizations = this.organizationTree.checkedNodes.join(',');
			}
			else{
				delete formValues.Filters.organizations
			}
			
		}
		let copyFormValues = { ...formValues, Filters: formValues.Filters, Properties: formValues.Properties };
		if (formValues.Filters.hasOwnProperty('contractParties')) {
			if (formValues.Filters.hasOwnProperty('kpi')) {
				formValues.Filters.kpi = formValues.Filters.kpi.toString();
			} else {
				delete formValues.Filters.kpi;
			}
		}
		// Danial: TODO There may be issues in copyFormValues while patching with form
		if (formValues.Properties.hasOwnProperty('parameters')) {
			if (formValues.Properties.parameters.length > 0) {
				// formValues.Properties.measure = this.barChartWidgetParameters.getReportQueryDetailByID.id
				formValues.Properties.parameters = JSON.stringify(formValues.Properties.parameters);
			} else {
				delete formValues.Properties.parameters;
			}
		}
		let submitFormValues = removeNullKeysFromObject(formValues);
		this.updateDashboardWidgetsArray(this.barChartWidgetParameters.id, submitFormValues);
		const { url } = this.barChartWidgetParameters;

		if (this.isKpiReportTrendComponent) {
			this.onKpiReportGroupFromSubmit(url, submitFormValues, copyFormValues);
			return true;
		}
		console.log('submitFormValues', JSON.stringify(submitFormValues));
		this.dashboardService.getWidgetIndex(url, submitFormValues).subscribe(result => {
			// sending data to bar chart component only.
			
			if(this.isFreeFormReportComponent) {
				this.emitter.sendNext({
					type: 'freeFormReportWidgetTable',
					data: {
						result,
						freeFormReportWidgetParameters: this.barChartWidgetParameters,
						freeFormReportWidgetParameterValues: copyFormValues
					}
				});
				this.isFreeFormReportComponent = false;
			}
			if (this.isBarChartComponent) {
				this.emitter.sendNext({
					type: 'barChart',
					data: {
						result,
						barChartWidgetParameters: this.barChartWidgetParameters,
						barChartWidgetParameterValues: copyFormValues
					}
				});
				this.isBarChartComponent = false;
			}
			if (this.isKpiCountSummaryComponent) {
				this.emitter.sendNext({
					type: 'kpiCountSummaryChart',
					data: {
						result,
						kpiCountSummaryWidgetParameters: this.barChartWidgetParameters,
						kpiCountSummaryWidgetParameterValues: copyFormValues
					}
				});
				this.isKpiCountSummaryComponent = false;
			}
			if (this.isverificaDoughnutComponent) {
				this.emitter.sendNext({
					type: 'verificaDoughnutChart',
					data: {
						result,
						verificaDoughnutWidgetParameters: this.barChartWidgetParameters,
						verificaDoughnutWidgetParameterValues: copyFormValues
					}
				});
				this.isverificaDoughnutComponent = false;
			}
			if (this.isCatalogPendingComponent) {
				this.emitter.sendNext({
					type: 'catalogPendingChart',
					data: {
						result,
						catalogPendingWidgetParameters: this.barChartWidgetParameters,
						catalogPendingWidgetParameterValues: copyFormValues
					}
				});
				this.isCatalogPendingComponent = false;
			}
			if (this.isNotificationTrendComponent) {
				this.emitter.sendNext({
					type: 'notificationTrendChart',
					data: {
						result,
						notificationTrendWidgetParameters: this.barChartWidgetParameters,
						notificationTrendWidgetParameterValues: copyFormValues
					}
				});
				this.isNotificationTrendComponent = false;
			}
			if (this.isKpiReportTrendComponent) {
				this.emitter.sendNext({
					type: 'kpiReportTrendChart',
					data: {
						result,
						kpiReportTrendWidgetParameters: this.barChartWidgetParameters,
						kpiReportTrendWidgetParameterValues: copyFormValues
					}
				});
				this.isKpiReportTrendComponent = false;
			}
			if (this.isKpiCountOrgComponent) {
				this.emitter.sendNext({
					type: 'kpiCountByOrgChart',
					data: {
						result,
						kpiCountOrgWidgetParameters: this.barChartWidgetParameters,
						kpiCountOrgWidgetParameterValues: copyFormValues
					}
				});
				this.isKpiCountOrgComponent = false;
			}
			if (this.isDistributionByUserComponent) {
				this.emitter.sendNext({
					type: 'distributionByUserChart',
					data: {
						result,
						distributionByUserWidgetParameters: this.barChartWidgetParameters,
						distributionByUserWidgetParameterValues: copyFormValues
					}
				});
				this.isDistributionByUserComponent = false;
			}
			if (this.isKpiStatusSummaryComponent) {
				this.emitter.sendNext({
					type: 'kpiStatusSummaryTable',
					data: {
						result,
						kpiStatusSummaryWidgetParameters: this.barChartWidgetParameters,
						kpiStatusSummaryWidgetParameterValues: copyFormValues
					}
				});
				this.isKpiStatusSummaryComponent = false;
			}
			this.loadingModalForm = false;
			this.emitter.loadingStatus(false);
		}, error => {
			this.toastr.error('Unable to fetch widget data.', 'Error');
			console.log('onWidgetParametersFormSubmit', error);
			this.emitter.loadingStatus(false);
			this.loadingModalForm = false;
		})
	}
	customDateTypes(event) {
		//console.log('customDateTypes', event);
	}

	contractPartiesDropDown(event) {
		this.loadingFiltersDropDown = true;
		this.dashboardService.getContract(0, +event.target.value).subscribe(result => {
			this.widgetParametersForm.get('Filters.contracts').enable();
			this.barChartWidgetParameters.allContracts = result;
			this.widgetParametersForm.patchValue({
				Filters: {
					contracts: ''
				}
			});
			this.loadingFiltersDropDown = false;
			this.filterContracts = [{ key: '', value: 'Select Contracts' }, ...result];
		}, error => {
			this.loadingFiltersDropDown = false;
			console.error('contractPartiesDropDown', error);
			this.toastr.error('Error', 'Unable to get Contracts');
		});
	}

	contractsDropDown(event) {
		this.loadingFiltersDropDown = true;
		this.dashboardService.getKPIs(0, +event.target.value).subscribe(result => {
			this.widgetParametersForm.get('Filters.kpi').enable();
			this.barChartWidgetParameters.allKpis = result;
			this.filterKpis = [{ key: '', value: 'Select KPI' }, ...result];
			this.widgetParametersForm.patchValue({
				Filters: {
					kpi: ''
				}
			});
			this.loadingFiltersDropDown = false;
		}, error => {
			this.loadingFiltersDropDown = false;
			console.error('contractsDropDown', error);
			this.toastr.error('Error', 'Unable to get KPIs');
		});
	}
	contractPartiesDropDown1(event) {
		this.loadingFiltersDropDown = true;
		this.dashboardService.getContract(0, +event.target.value).subscribe(result => {
			this.widgetParametersForm.get('Filters.contracts1').enable();
			this.barChartWidgetParameters.allContracts1 = result;
			//debugger
			this.widgetParametersForm.patchValue({
				Filters: {
					contracts1: ''
				}
			});
			this.loadingFiltersDropDown = false;
			this.filterContracts1 = [{ key: '', value: 'Select Contracts' }, ...result];
		}, error => {
			this.loadingFiltersDropDown = false;
			console.error('contractPartiesDropDown1', error);
			this.toastr.error('Error', 'Unable to get Contracts');
		});
	}

	contractsDropDown1(event) {
		this.loadingFiltersDropDown = true;
		this.dashboardService.getKPIs(0, +event.target.value).subscribe(result => {
			this.widgetParametersForm.get('Filters.kpi1').enable();
			this.barChartWidgetParameters.allKpis1 = result;
			//debugger
			this.filterKpis1 = [{ key: '', value: 'Select KPI' }, ...result];
			this.widgetParametersForm.patchValue({
				Filters: {
					kpi1: ''
				}
			});
			this.loadingFiltersDropDown = false;
		}, error => {
			this.loadingFiltersDropDown = false;
			console.error('contractsDropDown', error);
			this.toastr.error('Error', 'Unable to get KPIs');
		});
	}

	addLoaderToTrees(add = true) {
		let load = false;
		if (add === false) {
			load = true;
		}
		// this.treesArray.forEach((itm: any) => {
		// 	itm.loaded = load;
		// });
	}

	updateDashboardWidgetsArray(widgetId, widgetFormValues) {
		let updatedDashboardArray = this.dashboardWidgetsArray.map(widget => {
			if (widget.id === widgetId) {
				let a = {
					...widget,
					filters: widgetFormValues.Filters,
					properties: widgetFormValues.Properties,
				}
				return a;
			} else {
				return widget;
			}
		});
		// this.dashboardWidgetsArray = updatedDashboardArray;
		this.cloneDashboardWidgetsArrayState = updatedDashboardArray;
		// need to preserve dashbaordCollection state in abother variable to aviod re-rendering
		// this.dashboardCollection.dashboardwidgets = updatedDashboardArray;
	}

	closeModalSubscription() {
		this.emitter.getData().subscribe(data => {
			if (data.type === 'closeModal') {
				this.widgetParametersModal.hide();
				this.isBarChartComponent = false;
				this.isKpiCountSummaryComponent = false;
				this.isverificaDoughnutComponent = false;
				this.isCatalogPendingComponent = false;
				this.isNotificationTrendComponent = false;
				this.isKpiReportTrendComponent = false;
				this.isKpiCountOrgComponent = false;
				this.isDistributionByUserComponent = false;
				this.isKpiStatusSummaryComponent = false;
			}
		});
	}

	onKpiReportGroupFromSubmit(url, submitFormValues, copyFormValues) {
		let chartParams = JSON.parse(JSON.stringify(submitFormValues));
		delete chartParams.Filters.contractParties1;
		delete chartParams.Filters.contracts1;
		delete chartParams.Filters.kpi1;
		console.log('this.barChartWidgetParameters', this.barChartWidgetParameters)
		//debugger
		this.dashboardService.getWidgetIndex(url, chartParams).subscribe(result => {
			this.emitter.sendNext({
				type: 'kpiReportTrendChart',
				data: {
					result,
					kpiReportTrendWidgetParameters: this.barChartWidgetParameters,
					kpiReportTrendWidgetParameterValues: copyFormValues
				}
			});
			this.isKpiReportTrendComponent = false;
			this.loadingModalForm = false;
			this.emitter.loadingStatus(false);
		}, error => {
			this.toastr.error('Unable to fetch widget data.', 'Error');
			console.log('onWidgetParametersFormSubmit', error);
			this.emitter.loadingStatus(false);
			this.loadingModalForm = false;
		});

		
		if (submitFormValues.Filters.groupReportCheck) {
			let chartParams1 = JSON.parse(JSON.stringify(submitFormValues));
			chartParams1.Filters.contractParties = chartParams1.Filters.contractParties1;
			chartParams1.Filters.contracts = chartParams1.Filters.contracts1;
			chartParams1.Filters.kpi = chartParams1.Filters.kpi1;
			delete chartParams1.Filters.contractParties1;
			delete chartParams1.Filters.contracts1;
			delete chartParams1.Filters.kpi1;
			console.log('this.barChartWidgetParameters', this.barChartWidgetParameters)
			//debugger
			this.dashboardService.getWidgetIndex(url, chartParams1).subscribe(result => {
				this.emitter.sendNext({
					type: 'kpiReportTrendChart1',
					data: {
						result,
						kpiReportTrendWidgetParameters: this.barChartWidgetParameters,
						kpiReportTrendWidgetParameterValues: copyFormValues
					}
				});
				this.isKpiReportTrendComponent = false;

				this.loadingModalForm = false;
				this.emitter.loadingStatus(false);
			}, error => {
				this.toastr.error('Unable to fetch widget data.', 'Error');
				console.log('onWidgetParametersFormSubmit', error);
				this.emitter.loadingStatus(false);
				this.loadingModalForm = false;
			});

		}
	}

	reportParametersDropDown(event) {
		if(this.isFreeFormReportComponent) {
			this.loadingFiltersDropDown = true;
			const reportId = event.target.value.split(':')[1].trim();
			this._freeFormReportService.getReportQueryDetailByID(+reportId).subscribe(params => {
        this.loadingFiltersDropDown = false;
              if (params.parameters.length == 0) {
                this.parametersArray = this.widgetParametersForm.get('Properties').get('parameters') as FormArray;
                while (this.parametersArray.length !== 0) {
                  this.parametersArray.removeAt(0)
                }
              }
				params.parameters.map(p => this.addParameters(p));
			}, error => {
				this.toastr.error('Unable to fetch report parameters', 'Error!')
				console.error('reportParametersDropDown', error);
				this.loadingFiltersDropDown = false;
			});	
		}
	}

	getdati1(kpiId:number,toMonth:number,toYear:number) {
        this.periodFilter = 1;
        let month;
		let year;
		if(this.isSelectedPeriod==1){
			month = toMonth;
		}else{
			if(toMonth<10){
				month = '0' + toMonth;
			}else{
				month = toMonth;
			}
		}
        year = toYear;
        this.loadingModalDati = true;
        this.isLoadedDati=1;

        console.log('getdati1 -> ',kpiId,month,year);

        this.apiService.getKpiRawData(kpiId, month, year).subscribe((dati: any) => {
            this.fitroDataById = dati;
            //console.log(dati);
            Object.keys(this.fitroDataById).forEach(key => {
                this.fitroDataById[key].data = JSON.parse(this.fitroDataById[key].data);
                switch (this.fitroDataById[key].event_state_id) {
                    case 1:
                        this.fitroDataById[key].event_state_id = "Originale";
                        break;
                    case 2:
                        this.fitroDataById[key].event_state_id = "Sovrascritto";
                        break;
                    case 3:
                        this.fitroDataById[key].event_state_id = "Eliminato";
                        break;
                    case 4:
                        this.fitroDataById[key].event_state_id = "Correzione";
                        break;
                    case 5:
                        this.fitroDataById[key].event_state_id = "Correzione eliminata";
                        break;
                    case 6:
                        this.fitroDataById[key].event_state_id = "Business";
                        break;
                    default:
                        this.fitroDataById[key].event_state_id = this.fitroDataById[key].event_state_id;
                        break;
                }
                this.fitroDataById[key].event_type_id = this.eventTypes[this.fitroDataById[key].event_type_id] ? this.eventTypes[this.fitroDataById[key].event_type_id] : this.fitroDataById[key].event_type_id;
                this.fitroDataById[key].resource_id = this.resources[this.fitroDataById[key].resource_id] ? this.resources[this.fitroDataById[key].resource_id] : this.fitroDataById[key].resource_id;
                this.fitroDataById[key].modify_date = moment(this.fitroDataById[key].modify_date).format('DD/MM/YYYY HH:mm:ss');
                this.fitroDataById[key].create_date = moment(this.fitroDataById[key].create_date).format('DD/MM/YYYY HH:mm:ss');
                this.fitroDataById[key].time_stamp = moment(this.fitroDataById[key].time_stamp).format('DD/MM/YYYY HH:mm:ss');
            })
            this.getCountCampiData();

            let max = this.countCampiData.length;

            Object.keys(this.fitroDataById).forEach(key => {
                let temp = Object.keys(this.fitroDataById[key].data).length;
                if (temp < max) {
                    for (let i = 0; i < (max - temp); i++) {
                        this.fitroDataById[key].data['empty#' + i] = '##empty##';
                    }
                }
            })
            console.log('dati', dati);
            this.loadingModalDati = false;
        },
        error => {
            this.loadingModalDati = false;
        });
	}
	
	getCountCampiData() {
        let maxLength = 0;
        this.fitroDataById.forEach(f => {
            //let data = JSON.parse(f.data);
            if (Object.keys(f.data).length > maxLength) {
                maxLength = Object.keys(f.data).length;
            }
        });
        this.countCampiData = [];
        for (let i = 1; i <= maxLength; i++) {
            this.countCampiData.push(i);
        }
	}
	
    selectedMonth(e){
		this.isSelectedPeriod=1;
        let stringToSplit = this.monthVar;
        let split = stringToSplit.split("/");
        let month = split[0];
        let year = split[1];

        console.log('KPI ID -> ',this.kpiId,' - Selected Month -> ',month,' - Selected Year -> ',year);
    
        this.selectedmonth = month;
        this.selectedyear = year;

        this.getdati1(this.kpiId,month,year);
	}
	
	selectedPeriod(){
        let stringToSplit = this.dayDrillPeriod;
        let split = stringToSplit.split("/");
        let month = split[0];
        let year = split[1];

        console.log('KPI ID -> ',this.kpiId,' - Selected Month -> ',month,' - Selected Year -> ',year);
    
        this.getDayLevelData(this.kpiId,month,year);
	}
	
	getDayLevelData(globalRuleId, month, year){
        this.apiService.GetDayLevelKPIData(4895,month,year).subscribe((data) => {
            console.log('GetDayLevelKPIData -> ',data);

            if(data.length==0){
                this.toastr.success('Nessun dato per il periodo '+month+'/'+year);
                this.isDayDrill=0;
            }else{    
                this.isDayDrill=1;

                const chartArray = data;

                let targetData = chartArray.filter(data => (data.zvalue === 'Target' || data.zvalue === 'Previsione' ));
                let providedData = chartArray.filter(data => (data.zvalue === 'Provided'));
                
                let allChartLabels = getDistinctArray(chartArray.map(label => label.xvalue).sort());
                
                let allTargetData = targetData.map(data => data.yvalue);
                let allProvidedData = providedData.map(data => data.yvalue);

                this.dayChartOptions.xAxis = {
                    type: 'datetime',
                    categories: allChartLabels,
                }
                this.dayChartOptions.yAxis.title = {
                    text: 'Percent'
                }
                
                this.dayChartOptions.series[0] = {
                    type: 'scatter',
                    name: 'Target',
                    data: allTargetData,
                    marker: {
                        fillColor: '#1985ac'
                    },
                    dataLabels: {
                        color: '#1985ac',
                        // color: '#ffc107',
                    },
                };

                this.dayChartOptions.series[1] = {
                    type: 'scatter',
                    name: 'Provided',
                    data: allProvidedData,
                    marker: {
                        fillColor: '#379457'
                    },
                    dataLabels: {
                        color: '#379457',
                    },
                };
                
                this.dayChartUpdateFlag = true;
            }
        });
	}
	
	hideModal(){
        this.bsiChartModal.hide();
    }
	
}

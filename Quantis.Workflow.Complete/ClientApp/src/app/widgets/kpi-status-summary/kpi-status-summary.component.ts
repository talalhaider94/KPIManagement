import { Component, OnInit, Input, Output, EventEmitter, ViewChild, OnDestroy } from '@angular/core';
import { DashboardService, EmitterService, FreeFormReportService } from '../../_services';
import { DateTimeService, WidgetHelpersService } from '../../_helpers';
import { mergeMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { DataTableDirective } from 'angular-datatables';
import { Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
@Component({
    selector: 'app-kpi-status-summary',
    templateUrl: './kpi-status-summary.component.html',
    styleUrls: ['./kpi-status-summary.component.scss']
})
export class KpiStatusSummaryComponent implements OnInit, OnDestroy {
    @Input() widgetname: string;
    @Input() url: string;
    @Input() filters: Array<any>;
    @Input() properties: Array<any>;
    @Input() widgetid: number;
    @Input() dashboardid: number;
    @Input() id: number;
    loading: boolean = false;
    kpiStatusSummaryWidgetParameters: any;
    setWidgetFormValues: any;
    isDashboardModeEdit: boolean = true;
    @Output()
    kpiStatusSummaryParent = new EventEmitter<any>();
    kpiStatusSummaryData: any = [];
   @ViewChild(DataTableDirective)
    datatableElement: DataTableDirective;
    dtOptions: any = {};
    dtTrigger = new Subject();
    editQueryData = {
      id: 0,
      QueryName: '',
      QueryText: '',
      Parameters: [{
        key: '',
        value: ''
      }]
    }
    executeQueryData = {
      QueryText: '',
      Parameters: [{
        key: '',
        value: ''
      }]
    }
  constructor(
        private _freeFormReport: FreeFormReportService,
        private dashboardService: DashboardService,
        private emitter: EmitterService,
        private dateTime: DateTimeService,
    private router: Router,
    private toastr: ToastrService,
        private widgetHelper: WidgetHelpersService
    ) { }

    ngOnInit() {
        this.dtOptions = {
            pagingType: 'full_numbers',
            pageLength: 10,
            destroy: false,
            dom: 'lBfrtip',
            search: {
                caseInsensitive: true
            },
            buttons: [
                {
                    extend: 'csv',
                    text: '<i class="fa fa-file"></i> CSV',
                    titleAttr: 'CSV',
                    className: 'btn btn-primary'
                },
                {
                    extend: 'pdf',
                    text: '<i class="fa fa-file"></i> PDF',
                    titleAttr: 'PDF',
                    className: 'btn btn-primary',
                    orientation: 'landscape',
                },
            ],
            language: {
                processing: "Elaborazione...",
                search: "Cerca:",
                lengthMenu: "Visualizza _MENU_ elementi",
                info: "Vista da _START_ a _END_ di _TOTAL_ elementi",
                infoEmpty: "Vista da 0 a 0 di 0 elementi",
                infoFiltered: "(filtrati da _MAX_ elementi totali)",
                infoPostFix: "",
                loadingRecords: "Caricamento...",
                zeroRecords: "La ricerca non ha portato alcun risultato.",
                emptyTable: "Nessun dato presente nella tabella.",
                paginate: {
                    first: "Primo",
                    previous: "Precedente",
                    next: "Seguente",
                    last: "Ultimo"
                },
                aria: {
                    sortAscending: ": attiva per ordinare la colonna in ordine crescente",
                    sortDescending: ":attiva per ordinare la colonna in ordine decrescente"
                }
            }
        };

        if (this.router.url.includes('dashboard/public')) {
            this.isDashboardModeEdit = false;
            if (this.url) {
                this.emitter.loadingStatus(true);
                this.getChartParametersAndData();
            }
        }
    }

    ngOnDestroy() {
        this.dtTrigger.unsubscribe();
    }
    ngAfterViewInit() {
        this.dtTrigger.next();
    }

    rerender(): void {
        this.datatableElement.dtInstance.then((dtInstance: DataTables.Api) => {
            dtInstance.destroy();
            this.dtTrigger.next();
        });
    }
    // invokes on component initialization
    /*getChartParametersAndData(url) {
        let myWidgetParameters = null;
        this.loading = true;
        this.dashboardService.getWidgetParameters(url).pipe(
            mergeMap((getWidgetParameters: any) => {
                myWidgetParameters = getWidgetParameters;
                // Map Params for widget index when widgets initializes for first time
                let newParams = this.widgetHelper.initWidgetParameters(getWidgetParameters, this.filters, this.properties);
                newParams.Filters.organizations = this.preSelectedNodes.join(',');
                return this.dashboardService.getWidgetIndex(url, newParams);
            })
        ).subscribe(getWidgetIndex => {
            // populate modal with widget parameters
            myWidgetParameters;
            let kpiStatusSummaryParams;
            if (myWidgetParameters) {
                kpiStatusSummaryParams = {
                    type: 'kpiStatusSummaryTableParams',
                    data: {
                        ...myWidgetParameters,
                        widgetname: this.widgetname,
                        url: this.url,
                        filters: this.filters,
                        properties: this.properties,
                        widgetid: this.widgetid,
                        dashboardid: this.dashboardid,
                        id: this.id
                    }
                }
                this.kpiStatusSummaryWidgetParameters = kpiStatusSummaryParams.data;
                // setting initial Paramter form widget values
                this.setWidgetFormValues = this.widgetHelper.setWidgetParameters(myWidgetParameters, this.filters, this.properties);
                console.log('KpiStatusSummary Table this.setWidgetFormValues', this.setWidgetFormValues);
            }
            // popular chart data
            if (getWidgetIndex) {
                const chartIndexData = getWidgetIndex.body;
                // third params is current widgets settings current only used when
                // widgets loads first time. may update later for more use cases
                this.updateChart(chartIndexData, null, kpiStatusSummaryParams.data);
            }
            this.loading = false;
            this.emitter.loadingStatus(false);
        }, error => {
            console.error('KpiStatusSummary Table', error, this.setWidgetFormValues);
            this.loading = false;
            this.emitter.loadingStatus(false);
        });
    }*/

    getChartParametersAndData() {
        this.loading = true;
        this.clearData();

        this.executeQueryData.QueryText = '$kpiCalculationStatus';
        this.executeQueryData.Parameters = [];
        this._freeFormReport.ExecuteReportQuery(this.executeQueryData).subscribe(data => {
          this.kpiStatusSummaryData = data;
          this.rerender();
          this.loading = false;
        }, error => {
          this.toastr.error('Errore in query execution', 'Error');
        });
    }
    clearData() {
      this.kpiStatusSummaryData = [];
    }
    // dashboardComponentData is result of data coming from
    updateChart(chartIndexData, dashboardComponentData, currentWidgetComponentData) {
        this.kpiStatusSummaryData = chartIndexData;
        this.rerender();
        // this.dtTrigger.next();
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

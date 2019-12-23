import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { DataTableDirective } from 'angular-datatables';
import { ApiService } from '../../../_services/api.service';
import { FreeFormReportService } from '../../../_services';
import { Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ModalDirective } from 'ngx-bootstrap/modal';

declare var $;
var $this;

@Component({
    templateUrl: './import-form-report.component.html'
})

export class ImportFormReportComponent implements OnInit {
    @ViewChild('addConfigModal') public addConfigModal: ModalDirective;
    @ViewChild('configModal') public configModal: ModalDirective;
    @ViewChild('ConfigurationTable') block: ElementRef;
    // @ViewChild('searchCol1') searchCol1: ElementRef;
    @ViewChild(DataTableDirective) private datatableElement: DataTableDirective;
    category_id: number = 0;
    queryData = {
        id: 0,
        QueryName: '',
        QueryText: '',
        OwnerId: '',
        Parameters: []
    }
    withParameters: [{
        key: '',
        value: ''
      }]

    dtOptions: DataTables.Settings = {
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

    loading: boolean = true;
    dtTrigger: Subject<any> = new Subject();
    ConfigTableBodyData: any = [];

    constructor(
        private apiService: ApiService,
        private _freeFormReport: FreeFormReportService,
        private toastr: ToastrService,
    ) {
        $this = this;
    }

    ngOnInit() {
    }

    // tslint:disable-next-line:use-life-cycle-interface
    ngAfterViewInit() {
        this.dtTrigger.next();
        this.getCOnfigurations();
    }

    ngOnDestroy(): void {
        // Do not forget to unsubscribe the event
        this.dtTrigger.unsubscribe();
    }

    rerender(): void {
        this.datatableElement.dtInstance.then((dtInstance: DataTables.Api) => {
            // Destroy the table first
            dtInstance.destroy();
            // Call the dtTrigger to rerender again
            this.dtTrigger.next();
            this.loading = false;
        });
    }

    strip_tags(html) {
        var tmp = document.createElement("div");
        tmp.innerHTML = html;
        return tmp.textContent || tmp.innerText;
    }

    getCOnfigurations() {
        this.loading = true;
        debugger
        this.apiService.getFreeFormReports().subscribe((data) =>{
          this.ConfigTableBodyData = data;
          console.log('import form data -> ', data);
          this.rerender();
        });
    }

    populateData(row){
        this.queryData.id = 0;
        this.queryData.QueryName = row.reportname;
        this.queryData.QueryText = row.query;
        this.queryData.OwnerId = row.ownerid;
        console.log('parameters length -> ',row.parameters.length);
        if(row.parameters.length==0){ 
        }else{
            this.queryData.Parameters = row.parameters;
        }
        console.log('queryData -> ', this.queryData)
        debugger
        this._freeFormReport.addEditReportQuery(this.queryData).subscribe(data => {
            this.toastr.success('Query created successfully');
        }, error => {
            this.toastr.error('Errore esecuzione report');
        });
    }

    onCancel(dismissMethod: string): void {
        console.log('Cancel ', dismissMethod);
    }

    showConfigModal() {
        this.configModal.show();
    }

    hideConfigModal() {
        this.configModal.hide();
    }
}

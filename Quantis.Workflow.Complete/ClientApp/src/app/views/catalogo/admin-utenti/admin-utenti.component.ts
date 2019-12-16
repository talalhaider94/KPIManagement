import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { saveAs } from 'file-saver';
import { DataTableDirective } from 'angular-datatables';
import { ApiService } from '../../../_services/api.service';
import { Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ModalDirective } from 'ngx-bootstrap/modal';

declare var $;
var $this;

@Component({
    selector: 'app-admin-utenti',
    templateUrl: './admin-utenti.component.html',
    styleUrls: ['./admin-utenti.component.scss']
})
export class AdminUtentiComponent implements OnInit {
    @ViewChild('configModal') public configModal: ModalDirective;
    @ViewChild('UserTable') UserTable: ElementRef;
    @ViewChild('btnExporta') btnExporta: ElementRef;
    @ViewChild(DataTableDirective) private datatableElement: DataTableDirective;

    dtOptions: DataTables.Settings = {
        // 'dom': 'rtip',
        // 'pagingType': 'full_numbers'
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

    modalData = {
        id: 0,
        ca_bsi_account: '',
        ca_bsi_user_id: 0,
        name: '',
        surname: '',
        organization: '',
        mail: '',
        userid: '',
        manager: '',
        password: '9e9f3e692e0a7da4ed9bae3c77f084022dd4dd3f376d9cfd711603f6992e665e',
    };

    dtTrigger: Subject<any> = new Subject();
    UtentiTableBodyData: any = [
        {
            id: 0,
            ca_bsi_account: 'BSI ACCOUNT',
            name: 'NOME',
            surname: 'COGNOME',
            organization: 'STRUTTURA',
            mail: 'MAIL',
            userid: 'USERID',
            manager: 'RESPONSABILE'
        }
    ]

    constructor(
        private apiService: ApiService,
        private toastr: ToastrService,
    ) {
        $this = this;
    }

    ngOnInit() {
    }

    populateModalData(data) {
        this.modalData.id = 0;
        this.modalData.ca_bsi_account = data.user_name;
        this.modalData.ca_bsi_user_id = data.user_id;
        this.modalData.name = '';
        this.modalData.surname = '';
        this.modalData.password = '9e9f3e692e0a7da4ed9bae3c77f084022dd4dd3f376d9cfd711603f6992e665e';
        this.modalData.organization = data.user_organization_name;
        this.modalData.mail = data.user_email;
        this.modalData.userid = "RETE\\local";
        this.modalData.manager = '';

        this.showConfigModal();
    }

  updateUtenti() {
    let formCompleted = false;
    let errore = '';
    if (this.modalData.mail && this.modalData.mail.length > 0) {
      let searchEmail = this.modalData.mail.search('@');
      if (searchEmail >= 0) {
        if (this.modalData.userid && this.modalData.userid.length > 0) {
          console.log(this.modalData.userid)
          this.modalData.userid = this.modalData.userid.replace(/\//g, "\\");
          console.log(this.modalData.userid)
          let search = this.modalData.userid.toLowerCase().search(/rete\\/);
          if (search >= 0) {
            this.modalData.userid = this.modalData.userid.toUpperCase();
            formCompleted = true;
            
          } else {
            errore = 'Formato Userid non valido. (es. RETE\\user)';
          }
        } else {
          errore = 'Userid obbligatorio';
        }
      } else {
        errore = 'Formato Mail non valido';
      }
    } else {
      errore = 'Mail obbligatoria';
    }
    if (formCompleted) {
      this.toastr.info('Valore in aggiornamento..', 'Info');
      this.apiService.updateCatalogUtenti(this.modalData).subscribe(data => {
        this.saveAssignedRoles(this.modalData.ca_bsi_user_id);
        this.getUsers(); // this should refresh the main table on page
        this.toastr.success('Valore Aggiornato', 'Successo');
        this.hideConfigModal();
        //$('#utentiModal').modal('toggle').hide();
      }, error => {
        this.toastr.error('Errore durante update.', 'Errore');
        this.hideConfigModal();
        //$('#utentiModal').modal('toggle').hide();
      });
    } else {
      this.toastr.error(errore, 'Errore');
    }
    }

    // tslint:disable-next-line:use-life-cycle-interface
    ngAfterViewInit() {
        this.dtTrigger.next();

        this.setUpDataTableDependencies();

        //this.getUsers1(); can't get the meaning of this
        this.getUsers();

        /*this.apiService.getCatalogoUsers().subscribe((data:any)=>{
          this.UtentiTableBodyData = data;
          this.rerender();
        });*/
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
            this.setUpDataTableDependencies();
        });
    }

    // getKpiTableRef(datatableElement: DataTableDirective): any {
    //   return datatableElement.dtInstance;
    // }

    setUpDataTableDependencies(){
      $(this.btnExporta.nativeElement).off('click');
      $(this.btnExporta.nativeElement).on('click', function (event) {
          event.preventDefault();
          event.stopPropagation();
          $this.datatableElement.dtInstance.then((datatable_Ref: DataTables.Api) => {
              $this.table2csv(datatable_Ref, 'full', '.UserTable');
          });
      });
    }

    table2csv(oTable, exportmode, tableElm) {
      var csv = '|';
      var headers = [];
      var rows = [];
      // Get header names
      $(tableElm + ' thead tr th:not(.notExportCsv)').each(function () {
          var text = $(this).text();
          var header = '"' + text + '"';
          headers.push(text); // original code
      });
      csv += '"' + headers.join('"|"') + '"|\r\n';
      // get table data
      if (exportmode == "full") { // total data
        var totalRows = oTable.data().length;
        for (let i = 0; i < totalRows; i++) {
            rows.push('|"' + oTable.cells(oTable.row(i).nodes(), ':not(.notExportCsv)').data().join('"|"') + '"');
        }
      }
      csv += rows.join("|\r\n");
      console.log(csv)
      var blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
      saveAs(blob, "ExportConsolidareTable.csv");
    }

    strip_tags(html) {
        var tmp = document.createElement("div");
        tmp.innerHTML = html;
        return tmp.textContent || tmp.innerText;
    }

    getUsers1() {
        this.apiService.getTUsers().subscribe((data: any) => {
        });
    }

    getUsers() {
        this.apiService.getTUsers().subscribe((data) => {
            this.UtentiTableBodyData = data;
            console.log('Configs ', data);
            this.rerender();
        });
    }
    saveAssignedRoles(userid) {
        if (userid) {
            let dataToPost = { Id: userid, Ids: [] };
            dataToPost.Ids.push(1); // id role user
            this.apiService.assignRolesToUser(dataToPost).subscribe(data => {
                this.toastr.success('Assegnato ruolo USER', 'Success');
            }, error => {
                this.toastr.error('Errore durante assegnazione ruolo', 'Error');
            });
        }
    }

    showConfigModal() {
        this.configModal.show();
    }

    hideConfigModal() {
        this.configModal.hide();
    }
}

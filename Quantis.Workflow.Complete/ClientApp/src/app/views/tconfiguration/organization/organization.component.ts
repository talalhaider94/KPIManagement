import { Component, OnInit, ViewChild, ViewChildren, ElementRef, QueryList } from '@angular/core';
import { DataTableDirective } from 'angular-datatables';
import { ApiService } from '../../../_services/api.service';
import { Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ModalDirective } from 'ngx-bootstrap/modal';

declare var $;
var $this;

@Component({
    templateUrl: './organization.component.html',
    styleUrls: ['./organization.component.scss']
})

export class OrganizationComponent implements OnInit {
    @ViewChild('addConfigModal') public addConfigModal: ModalDirective;
    @ViewChild('specialModal') public specialModal: ModalDirective;
    @ViewChild('ConfigurationTable') block: ElementRef;
    @ViewChildren(DataTableDirective)
    datatableElements: QueryList<DataTableDirective>;
    dtOptions: DataTables.Settings = {};
    dtTrigger = new Subject();
    dtOptions2: DataTables.Settings = {};
    dtTrigger2 = new Subject();


    modalData = {
        key: 0,
        value: ''
    };

    addOrganizationData = {
        Key: 0,
        Value: ''
    };

    modalSpecialData = {
        key: 0,
        value: ''
    };

    addSpecialData = {
        Key: 0,
        Value: '',
        Note: '',
    };

    modalTitle='';
    buttonText='';

    organizationsData: any = []
    specialReportsData: any = []

    constructor(
        private apiService: ApiService,
        private toastr: ToastrService,
    ) {
        $this = this;
    }
    value;
    specialKey;
    specialValue;
    specialNote;
    isEdit=0;
    isSpecialEdit=0;

    ngOnInit() {
        this.dtOptions = {
            pagingType: 'full_numbers',
            pageLength: 10,
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
            },
            destroy:true
        };
        this.dtOptions2 = {
            pagingType: 'full_numbers',
            pageLength: 10,
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
            },
            destroy:true
        };
    }

    ngAfterViewInit() {
        this.dtTrigger.next();
        this.dtTrigger2.next();
        
        this.GetAllOrganizationUnits();
        this.GetAllReportSpecialValues();
    }

    populateModalData(data) {
        this.isEdit=1;
        this.modalData.key = data.key;
        this.value = data.value;
        this.modalTitle='Modifica Unità Organizaztiva';
        this.buttonText='Aggiorna';
        this.showAddConfigModal();
    }
    
    populateSpecialModalData(data) {
        this.isSpecialEdit=1;
        this.specialKey = data.key;
        this.specialValue = data.value;
        this.specialNote = data.note;
        this.modalTitle='Aggiorna Valore Speciale';
        this.buttonText='Aggiorna';
        this.showSpecialModal();
    }

    addOrganization() {
        if(this.isEdit==1){
            this.addOrganizationData.Key = this.modalData.key;
        }else{
            this.addOrganizationData.Key = 0;
        }
        this.addOrganizationData.Value = this.value;

        this.toastr.info('Valore in aggiornamento..', 'Info');
        this.apiService.AddUpdateOrganizationUnit(this.addOrganizationData).subscribe(data => {
            this.GetAllOrganizationUnits();
            this.toastr.success('Valore aggiornato', 'Success');
            this.hideAddConfigModal();
        }, error => {
            this.toastr.error('Errore durante l\'aggiornamento.', 'Error');
            this.hideAddConfigModal();
        });
    }

    addSpecialValue() {
        this.addSpecialData.Key = this.specialKey;
        this.addSpecialData.Value = this.specialValue;
        this.addSpecialData.Note = this.specialNote;

        this.toastr.info('Valore in aggiornamento..', 'Info');
        this.apiService.AddUpdateReportSpecialValue(this.addSpecialData).subscribe(data => {
            this.GetAllReportSpecialValues();
            this.toastr.success('Valore aggiornato', 'Success');
            this.hideSpecialModal();
        }, error => {
            this.toastr.error('Errore durante l\'aggiornamento.', 'Error');
            this.hideSpecialModal();
        });
    }

    deleteOrganization(data) {
        this.toastr.info('Valore in aggiornamento..', 'Info');
        this.apiService.DeleteOrganizationUnit(data.key).subscribe(data => {
            console.log(data);
            if(data==false){
                this.toastr.error('Non è possibile eliminare un\'unità organizzativa associata ad un KPI', 'Error');
            }else{
                this.GetAllOrganizationUnits(); 
                this.toastr.success('Valore Aggiornato', 'Success');
            }
        }, error => {
            this.toastr.error('Errore durante l\'aggiornamento.', 'Error');
        });
    }
    
    deleteSpecialValue(data) {
        this.toastr.info('Valore in aggiornamento..', 'Info');
        this.apiService.DeleteReportSpecialValue(data.key).subscribe(data => {
            this.GetAllReportSpecialValues(); 
            this.toastr.success('Valore Aggiornato', 'Success');
        }, error => {
            this.toastr.error('Errore durante l\'aggiornamento.', 'Error');
        });
    }

    ngOnDestroy(): void {
        this.dtTrigger.unsubscribe();
        this.dtTrigger2.unsubscribe();
    }

    rerender(): void {
        this.datatableElements.forEach((dtElement: DataTableDirective) => {
            dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
              dtInstance.destroy();
              this.dtTrigger.next();
              this.dtTrigger2.next();
            });
        });
        this.setUpDataTableDependencies();
    }

    setUpDataTableDependencies() {
    }

    strip_tags(html) {
        var tmp = document.createElement("div");
        tmp.innerHTML = html;
        return tmp.textContent || tmp.innerText;
    }

    GetAllOrganizationUnits() {
        this.apiService.GetAllOrganizationUnits().subscribe((data) => {
            this.organizationsData = data;
            console.log('Organizations Data -> ', data);
            this.rerender();
        });
    }

    GetAllReportSpecialValues() {
        this.apiService.GetAllReportSpecialValues().subscribe((data) => {
            this.specialReportsData = data;
            console.log('Special Reports Data -> ', data);
            this.rerender();
        });
    }

    onCancel(dismissMethod: string): void {
        console.log('Cancel ', dismissMethod);
    }

    addOrganizationModal(){
        this.isEdit=0;
        this.value='';
        this.modalTitle='Aggiungi Unità Organizaztiva';
        this.buttonText='Aggiungi';
        this.showAddConfigModal();
    }

    addSpecialModal(){
        this.isSpecialEdit=0;
        this.specialKey='';
        this.specialValue='';
        this.specialNote='';
        this.modalTitle='Nuovo Valore Speciale';
        this.buttonText='Aggiungi';
        this.showSpecialModal();
    }

    showSpecialModal() {
        this.specialModal.show();
    }

    hideSpecialModal() {
        this.specialModal.hide();
    }

    showAddConfigModal() {
        this.addConfigModal.show();
    }

    hideAddConfigModal() {
        this.addConfigModal.hide();
    }
}

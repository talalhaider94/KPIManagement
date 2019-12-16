import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { DataTableDirective } from 'angular-datatables';
import { ApiService } from '../../_services/api.service';
import { Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ModalDirective } from 'ngx-bootstrap/modal';
// import { ContextMenuComponent } from 'ngx-contextmenu';

declare var $;
var $this;

@Component({
    templateUrl: './sdmgroup.component.html'
})

export class SdmGroupComponent implements OnInit {
    @ViewChild('addConfigModal') public addConfigModal: ModalDirective;
    @ViewChild('configModal') public configModal: ModalDirective;
    @ViewChild('ConfigurationTable') block: ElementRef;
    // @ViewChild('searchCol1') searchCol1: ElementRef;
    @ViewChild(DataTableDirective) private datatableElement: DataTableDirective;

    // public items = [
    //     { name: 'John', otherProperty: 'Foo' },
    //     { name: 'Joe', otherProperty: 'Bar' }
    // ];
    // @ViewChild(ContextMenuComponent) public basicMenu: ContextMenuComponent;
  
    // showMessage(message: any) {
    //   console.log(message);
    // }

    category_id: number = 0;
    // handle: any = '';
    // name: any =  '';
    // step: any = '';

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

    modalData = {
        id: '',
        handle: '',
        name: '',
        step: '',
        category_id: 0
    };

    addData = {
        handle: '',
        name: '',
        step: '',
        category_id: 0
    };

    dtTrigger: Subject<any> = new Subject();
    ConfigTableBodyData: any = [
        {
            handle: 'handle',
            name: 'name',
            step: 1,
            category: 'category'
        }
    ]

    customersKP: any = [
        {
            key: '',
            value: ''
        }
    ]

    constructor(
        private apiService: ApiService,
        private toastr: ToastrService,
    ) {
        $this = this;
    }
    public handle: any;
    public name: any;
    public step: any;
    public category: any;

    ngOnInit() {
    }

    populateModalData(data) {
        this.modalData.id = data.id;
        this.modalData.handle = data.handle;
        this.modalData.name = data.name;
        this.modalData.step = data.step;
        this.modalData.category_id = data.category_id;
        this.showConfigModal();
    }

    add() {
        this.addData.handle = this.handle;
        this.addData.name = this.name;
        this.addData.step = this.step;
        this.addData.category_id = this.category_id;

        this.toastr.info('Valore in aggiornamento..', 'Info');
        this.apiService.addSDMGroup(this.addData).subscribe(data => {
            this.getCOnfigurations(); // this should refresh the main table on page
            this.toastr.success('Valore Aggiornato', 'Success');
            this.hideAddConfigModal();
            //$('#addConfigModal').modal('toggle').hide();
        }, error => {
            this.toastr.error('Errore durante update.', 'Error');
            //$('#addConfigModal').modal('toggle').hide();
            this.hideAddConfigModal();
        });
    }

    updateConfig() {
        //this.modalData.category_id = this.category_id;
        this.toastr.info('Valore in aggiornamento..', 'Info');
        this.apiService.updateSDMGroupConfig(this.modalData).subscribe(data => {
            this.getCOnfigurations(); // this should refresh the main table on page
            this.toastr.success('Valore Aggiornato', 'Success');
            this.hideConfigModal();
            //$('#configModal').modal('toggle').hide();
        }, error => {
            this.toastr.error('Errore durante update.', 'Error');
            //$('#configModal').modal('toggle').hide();
            this.hideConfigModal();
        });
    }

    deleteSDMRow(data) {
        this.toastr.info('Valore in aggiornamento..', 'Confirm');
        this.apiService.deleteSDMGroupConfiguration(data.id).subscribe(data => {
            this.getCOnfigurations(); // this should refresh the main table on page
            this.toastr.success('Valore Aggiornato', 'Success');
            // $('#configModal').modal('toggle').hide();
        }, error => {
            this.toastr.error('Errore durante update.', 'Error');
            // $('#configModal').modal('toggle').hide();
        });
    }

    // tslint:disable-next-line:use-life-cycle-interface
    ngAfterViewInit() {
        this.dtTrigger.next();

        this.setUpDataTableDependencies();
        this.getCOnfigurations();
        this.getCustomersKP();

        /*this.apiService.getConfigurations().subscribe((data:any)=>{
          this.ConfigTableBodyData = data;
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

    // getConfigTableRef(datatableElement: DataTableDirective): any {
    //   return datatableElement.dtInstance;
    //   // .then((dtInstance: DataTables.Api) => {
    //   //     console.log(dtInstance);
    //   // });
    // }

    setUpDataTableDependencies() {
        // $(this.searchCol1.nativeElement).on( 'keyup', function () {
        //   $this.datatableElement.dtInstance.then((datatable_Ref: DataTables.Api) => {
        //   datatable_Ref
        //     .columns( 0 )
        //     .search( this.value )
        //     .draw();
        // });
        // });
    }

    strip_tags(html) {
        var tmp = document.createElement("div");
        tmp.innerHTML = html;
        return tmp.textContent || tmp.innerText;
    }

    getCOnfigurations() {
        this.apiService.getSDMGroupConfigurations().subscribe((data) => {
            this.ConfigTableBodyData = data;
            console.log('Configs ', data);
            this.rerender();
        });
    }

    getCustomersKP() {
        this.apiService.getCustomersKP().subscribe((data) => {
            this.customersKP = data;
            console.log('CustomersKP ', data);
            this.rerender();
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

    showAddConfigModal() {
        this.addConfigModal.show();
    }

    hideAddConfigModal() {
        this.addConfigModal.hide();
    }
}

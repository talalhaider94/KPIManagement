import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { DataTableDirective } from 'angular-datatables';
import { ApiService } from '../../_services/api.service';
import { Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ModalDirective } from 'ngx-bootstrap/modal';
//import { FilterUsersPipe } from './../../_pipes/filterUsers.pipe';
declare var $;
var $this;

@Component({
    templateUrl: './adminroles.component.html'
})

export class AdminRolesComponent implements OnInit {
    @ViewChild('addConfigModal') public addConfigModal: ModalDirective;
    @ViewChild('configModal') public configModal: ModalDirective;
    @ViewChild('usersModal') public usersModal: ModalDirective;
    @ViewChild('ConfigurationTable') block: ElementRef;
    // @ViewChild('searchCol1') searchCol1: ElementRef;
    @ViewChild(DataTableDirective) private datatableElement: DataTableDirective;
    name: any = '';
    code: any = '';

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
        name: '',
        code: ''
    };

    addData = {
        name: '',
        code: ''
    };

    filters = {
        searchUsersText: ''
    }

    dtTrigger: Subject<any> = new Subject();
    ConfigTableBodyData: any = [
        {
            key: 'key',
            value: 'value',
            owner: 'owner',
            isenable: true,
            description: 'description',
        }
    ]

    UsersTableBodyData: any = [
        {
            name: ''
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
        this.modalData.id = data.id;
        this.modalData.name = data.name;
        this.modalData.code = data.code;
        this.showConfigModal();
    }

    populateUsersData(data) {
        this.modalData.id = data.id;
        this.modalData.name = data.name;
        this.getUsersList(this.modalData.id);
        this.showUsersModal();
    }

    addRole() {
        this.addData.name = this.name;
        this.addData.code = this.code;
        this.toastr.info('Valore in aggiornamento..', 'Info');
        this.apiService.addRole(this.addData).subscribe(data => {
            this.getCOnfigurations(); // this should refresh the main table on page
            this.toastr.success('Valore Aggiornato', 'Success');
            this.hideAddConfigModal();
            //$('#addConfigModal').modal('toggle').hide();
        }, error => {
            this.toastr.error('Errore durante update.', 'Error');
            this.hideAddConfigModal();
            //$('#addConfigModal').modal('toggle').hide();
        });
    }

    deleteAdminRole(data) {
        this.toastr.info('Valore in aggiornamento..', 'Confirm');
        this.apiService.deleteRole(data.roleId).subscribe(data => {
            this.getCOnfigurations(); // this should refresh the main table on page
            this.toastr.success('Record Deleted', 'Success');
        }, error => {
            this.toastr.error('Errore in record deletion.', 'Error');
        });
    }

    updateConfig() {
        this.toastr.info('Valore in aggiornamento..', 'Info');
        this.apiService.updateRole(this.modalData).subscribe(data => {
            this.getCOnfigurations(); // this should refresh the main table on page
            this.toastr.success('Valore Aggiornato', 'Success');
            this.hideConfigModal();
            //$('#configModal').modal('toggle').hide();
        }, error => {
            this.toastr.error('Errore durante update.', 'Error');
            this.hideConfigModal();
            //$('#configModal').modal('toggle').hide();
        });
    }

    // tslint:disable-next-line:use-life-cycle-interface
    ngAfterViewInit() {
        this.dtTrigger.next();

        this.setUpDataTableDependencies();
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
            this.setUpDataTableDependencies();
        });
    }

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
        this.apiService.getAllRoles().subscribe((data) => {
            this.ConfigTableBodyData = data;
            console.log('Configs ', data);
            this.rerender();
        });
    }

    getUsersList(roleId) {
        this.apiService.getUsersByRole(roleId).subscribe((data) => {
            this.UsersTableBodyData = data;
            console.log('Users ', data);
            //this.rerender();
        });
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

    showUsersModal() {
        this.usersModal.show();
    }

    hideUsersModal() {
        this.usersModal.hide();
    }
}

import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ApiService } from '../../../_services/api.service';
import { ToastrService } from 'ngx-toastr';
//import { FilterUsersPipe } from './../../../_pipes/filterUsers.pipe';

declare var $;
var $this;

@Component({
    templateUrl: './userRolePermissions.component.html',
    styleUrls: ['./userRolePermissions.component.scss']
})

export class UserRolePermissionsComponent implements OnInit {
    format = {
        add: 'Aggiungi', remove: 'Rimuovi', all: 'Tutti', none: 'Nessuno',
        draggable: true, locale: 'it'
    };
    gatheredData = {
        usersList: [],
        rolesList: [],
        assignedRoles: []
    }
    selectedData = {
        userid: null,
        name: ''
    }
    filters = {
        searchUsersText: '',
        searchRolesText: ''
    }
    loading = {
        users: false,
        roles: false
    }
    selectedRoles = [];

    constructor(
        private apiService: ApiService,
        private toastr: ToastrService,
    ) {
        $this = this;
    }

    ngOnInit() {
        this.loading.users = true;
        this.loading.roles = true;
        this.apiService.getCatalogoUsers().subscribe((res) => {
            this.gatheredData.usersList = res;
            //console.log(res);
            this.loading.users = false;
        });
        this.apiService.getAllRoles().subscribe((res) => {
            this.gatheredData.rolesList = res;
            this.loading.roles = false;
        });
    }

    // tslint:disable-next-line:use-life-cycle-interface
    ngAfterViewInit() {
    }

    ngOnDestroy(): void {
        // Do not forget to unsubscribe the event
    }

    onRoleSelectDeselect($event) {
        console.log($event, this.selectedRoles);
    }

    selectUserItem(user, $event) {
        //console.log(user, $event);
        $('.role-permissions-lists ul.users-list li').removeClass('highlited-user');
        $($event.target).addClass('highlited-user');
        this.selectedData.userid = user.ca_bsi_user_id;//user.ca_bsi_user_id;
        this.selectedData.name = user.userid + ' - ' + user.name + ' ' + user.surname + '[' + user.ca_bsi_account + ']';//user.ca_bsi_user_id;
        if (this.selectedData.userid) {
            this.apiService.getRolesByUserId(this.selectedData.userid).subscribe((res) => {
                this.selectedRoles = res;
                //console.log(res);
            });
        }
    }

    saveAssignedRoles() {
        if (this.selectedData.userid) {
            let dataToPost = { Id: this.selectedData.userid, Ids: [] };
            this.selectedRoles.forEach((value, idx) => {
                dataToPost.Ids.push(value.id)
            });
            this.apiService.assignRolesToUser(dataToPost).subscribe(data => {
                this.toastr.success('Ruolo salvato', 'Success');
            }, error => {
                this.toastr.error('Errore durante il salvataggio', 'Error');
            });
        }
    }
}

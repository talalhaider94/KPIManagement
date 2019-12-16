import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../../_services/api.service';
import { ToastrService } from 'ngx-toastr';

@Component({
    templateUrl: './rolepermissions.component.html'
})

export class RolePermissionsComponent implements OnInit {
    urlId = 0;
    gatheredData = {
        roleId: 0,
        roleName: '',
        permissionsList: [],
        assignedPermissions: []
    };
    public roleName = '';
    constructor(
        private apiService: ApiService,
        private toastr: ToastrService,
        private route: ActivatedRoute
    ) {
    }
    format = {
        add: 'Aggiungi', remove: 'Rimuovi', all: 'Tutti', none: 'Nessuno',
        draggable: true, locale: 'it'
    };
    ngOnInit() {
        //this.gatheredData.roleId = +this.route.snapshot.paramMap.get('id');
        this.route.params.subscribe((params) => {
            this.gatheredData.roleId = parseInt(params['id']) || 0;
            this.gatheredData.roleName = params['name'] || '';
            //console.log(this.gatheredData.roleId);
            //console.log(this.gatheredData.roleName)
            if (this.gatheredData.roleId) {
                this.getAllPermissions();
            } else {
                this.toastr.warning('Id del ruolo non valido', 'Warning');
            }
        });
        this.roleName = this.gatheredData.roleName;
    }

    getAllPermissions() {
        this.apiService.getAllPermisisons().subscribe(data => {
            this.gatheredData.permissionsList = data;
            console.log(data);
            this.getPermissionsByRoldId();
        });
    }
    getPermissionsByRoldId() {
        this.apiService.getPermissionsByRoldId(this.gatheredData.roleId).subscribe(data => {
            this.gatheredData.assignedPermissions = data;
        });
    }

    onPermissionSelectDeselect($event) {
        //console.log($event, this.gatheredData.assignedPermissions);
    }

    saveAssignedPermissions() {
        if (this.gatheredData.roleId) {
            let dataToPost = { Id: this.gatheredData.roleId, Ids: [] };
            this.gatheredData.assignedPermissions.forEach((value, idx) => {
                dataToPost.Ids.push(value.id);
            });
            console.log(dataToPost);
            this.apiService.assignPermissionsToRoles(dataToPost).subscribe(data => {
                this.toastr.success('Permessi Assegnati', 'Success');
            }, error => {
                this.toastr.error('Errore durante l\'assegnazione dei permessi', 'Error');
            });
        }
    }

    ngOnDestroy(): void {
    }
}

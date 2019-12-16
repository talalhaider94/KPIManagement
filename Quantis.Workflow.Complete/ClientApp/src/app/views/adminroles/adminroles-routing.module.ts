import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AdminRolesComponent } from './adminroles.component';
import { RolePermissionsComponent } from './rolePermissions/rolepermissions.component';

const routes: Routes = [
    {
        path: '',
        component: AdminRolesComponent,
        data: {
            title: 'Gestione Ruoli'
        }
    },
    {
        path: 'adminRolePermissions/:id/:name',
        component: RolePermissionsComponent,
        data: {
            title: 'Admin Roles & Permissions'
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AdminRolesRoutingModule { }

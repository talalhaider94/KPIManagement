import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UserProfilingComponent } from './userprofiling.component';
import { UserRolePermissionsComponent } from './UserRolePermissions/userRolePermissions.component';

const routes: Routes = [
    {
        path: 'userpermissions',
        component: UserProfilingComponent,
        data: {
            title: 'Profilazione Utente'
        }
    },
    {
        path: 'rolepermissions',
        component: UserRolePermissionsComponent,
        data: {
            title: 'User Role Permissions'
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class UserProfilingRoutingModule { }

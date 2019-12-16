import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
//import { AppModule } from './../../app.module';
import { AdminRolesComponent } from './adminroles.component';
import { RolePermissionsComponent } from './rolePermissions/rolepermissions.component';
import { AdminRolesRoutingModule } from './adminroles-routing.module';
import { DataTablesModule } from 'angular-datatables';
import { AngularDualListBoxModule } from 'angular-dual-listbox';
import { FormsModule } from '@angular/forms';
import { FilterUsersPipe } from './../../_pipes/filterRoleUsers.pipe';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ModalModule } from 'ngx-bootstrap/modal';

@NgModule({
    declarations: [AdminRolesComponent, RolePermissionsComponent, FilterUsersPipe],
    imports: [
        //AppModule,
        CommonModule,
        AdminRolesRoutingModule,
        AngularDualListBoxModule,
        DataTablesModule,
        FormsModule,
        PerfectScrollbarModule,
        ModalModule.forRoot(),
    ]
})
export class AdminRolesModule { }

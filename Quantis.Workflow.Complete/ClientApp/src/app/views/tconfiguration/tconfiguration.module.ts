import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TConfigurationComponent } from './tconfiguration.component';
import { TConfigurationAdvancedComponent } from './advanced/advanced.component';
import { OrganizationComponent } from './organization/organization.component';
import { WorkflowComponent } from './workflow/workflow.component';
import { TConfigurationRoutingModule } from './tconfiguration-routing.module';
import { DataTablesModule } from 'angular-datatables';
import { FormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';

@NgModule({
    declarations: [TConfigurationComponent, TConfigurationAdvancedComponent, WorkflowComponent, OrganizationComponent],
    imports: [
        CommonModule,
        TConfigurationRoutingModule,
        DataTablesModule,
        FormsModule,
        SweetAlert2Module,
        ModalModule.forRoot(),
    ]
})
export class TConfigurationModule { }

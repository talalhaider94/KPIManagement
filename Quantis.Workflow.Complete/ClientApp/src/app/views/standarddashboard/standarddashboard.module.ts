import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StandardDashboardComponent } from './standarddashboard.component';
import { StandardDashboardRoutingModule } from './standarddashboard-routing.module';
import { DataTablesModule } from 'angular-datatables';
import { FormsModule } from '@angular/forms';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { ModalModule } from 'ngx-bootstrap/modal';

@NgModule({
    declarations: [StandardDashboardComponent],
    imports: [
        CommonModule,
        StandardDashboardRoutingModule,
        DataTablesModule,
        SweetAlert2Module,
        FormsModule,
        ModalModule.forRoot(),
    ]
})
export class StandardDashboardModule { }

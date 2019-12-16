import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SdmStatusComponent } from './sdmstatus.component';
import { SdmStatusRoutingModule } from './sdmstatus-routing.module';
import { DataTablesModule } from 'angular-datatables';
import { FormsModule } from '@angular/forms';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { ModalModule } from 'ngx-bootstrap/modal';

@NgModule({
    declarations: [SdmStatusComponent],
    imports: [
        CommonModule,
        SdmStatusRoutingModule,
        DataTablesModule,
        SweetAlert2Module,
        FormsModule,
        ModalModule.forRoot(),
    ]
})
export class SdmStatusModule { }

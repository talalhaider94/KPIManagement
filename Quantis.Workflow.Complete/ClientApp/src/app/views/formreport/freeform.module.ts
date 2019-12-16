import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FreeFormComponent } from './freeform.component';
import { FreeFormRoutingModule } from './freeform-routing.module';
import { DataTablesModule } from 'angular-datatables';
import { FormsModule } from '@angular/forms';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { ModalModule } from 'ngx-bootstrap/modal';

@NgModule({
    declarations: [FreeFormComponent],
    imports: [
        CommonModule,
        FreeFormRoutingModule,
        DataTablesModule,
        SweetAlert2Module,
        FormsModule,
        ModalModule.forRoot(),
    ]
})
export class FreeFormModule { }

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GeneralBookletComponent } from './general-booklet.component';
import { GeneralBookletRoutingModule } from './general-booklet-routing.module';
import { DataTablesModule } from 'angular-datatables';
import { FormsModule } from '@angular/forms';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { ModalModule } from 'ngx-bootstrap/modal';

@NgModule({
    declarations: [GeneralBookletComponent],
    imports: [
        CommonModule,
        GeneralBookletRoutingModule,
        DataTablesModule,
        SweetAlert2Module,
        FormsModule,
        ModalModule.forRoot(),
    ]
})
export class GeneralBookletModule { }

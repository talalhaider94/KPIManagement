import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EmailComponent } from './email.component';
import { EmailRoutingModule } from './email-routing.module';
import { DataTablesModule } from 'angular-datatables';
import { FormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';

@NgModule({
    declarations: [EmailComponent],
    imports: [
        CommonModule,
        EmailRoutingModule,
        DataTablesModule,
        FormsModule,
        ModalModule.forRoot(),
    ]
})
export class EmailModule { }

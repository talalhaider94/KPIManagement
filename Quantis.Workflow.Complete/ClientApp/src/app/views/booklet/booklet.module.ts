import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartsModule } from 'ng2-charts/ng2-charts';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FormsModule } from '@angular/forms';
import { DataTablesModule } from 'angular-datatables';

import { BookletComponent } from './booklet.component';
import { ModalModule } from "ngx-bootstrap";
import { BookletRoutingModule } from './booklet-routing.module';

@NgModule({
    imports: [
        FormsModule,
        CommonModule,
        BookletRoutingModule,
        ChartsModule,
        BsDropdownModule,
        ButtonsModule.forRoot(),
        DataTablesModule,
        ModalModule.forRoot()
    ],
    declarations: [BookletComponent]
})
export class BookletModule { }

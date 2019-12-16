import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DatiGrezziComponent } from './datigrezzi.component';
import { DatiRoutingModule } from './datigrezzi-routing.module';
import { DataTablesModule } from 'angular-datatables';
import { FormsModule } from '@angular/forms';
import { KeysPipePipe } from './keys-pipe.pipe';
import { Ng2SearchPipeModule } from 'ng2-search-filter'
import { NgxPaginationModule } from 'ngx-pagination';
import { ModalModule } from 'ngx-bootstrap/modal';

@NgModule({
    declarations: [DatiGrezziComponent, KeysPipePipe],
    imports: [
        CommonModule,
        DatiRoutingModule,
        DataTablesModule,
        FormsModule,
        Ng2SearchPipeModule,
        NgxPaginationModule,
        ModalModule.forRoot(),
    ]
})
export class DatiModule { }

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

//import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { ArchivedKpiComponent } from './archivedkpi.component';
import { ArchivedKpiRoutingModule } from './archivedkpi-routing.module';
import { DataTablesModule } from 'angular-datatables';
import { FormsModule } from '@angular/forms';
import { MomentModule } from 'ngx-moment';
import { DatePipe } from '@angular/common';
import { KeysPipe } from './keys.pipe'
import { NgxPaginationModule } from 'ngx-pagination';
import { MatCardModule, MatButtonModule } from '@angular/material'
import { Ng2SearchPipeModule } from 'ng2-search-filter'
//import { ArchivedKpiPipe } from '../archived-kpi.pipe';
import { OrderModule } from 'ngx-order-pipe';
import { ModalModule } from 'ngx-bootstrap/modal';

@NgModule({
    declarations: [ArchivedKpiComponent, KeysPipe],
    imports: [
        NgxPaginationModule,
        CommonModule,
        ArchivedKpiRoutingModule,
        DataTablesModule,
        FormsModule,
        MomentModule.forRoot(),
        Ng2SearchPipeModule,
        MatCardModule,
        MatCardModule,
        OrderModule,
        ModalModule.forRoot(),
    ],
})
export class ArchivedKpiModule { }

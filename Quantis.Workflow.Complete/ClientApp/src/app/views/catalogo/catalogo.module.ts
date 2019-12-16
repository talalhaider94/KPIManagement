import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CatalogoKpiComponent } from './catalogo-kpi/catalogo-kpi.component';
import { AdminKpiComponent } from './admin-kpi/admin-kpi.component';
import { CatalogoUtentiComponent } from './catalogo-utenti/catalogo-utenti.component';
import { AdminUtentiComponent } from './admin-utenti/admin-utenti.component';
import { CatalogoRoutingModule } from './catalogo-routing.module';
import { DataTablesModule } from 'angular-datatables';
import { FormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ButtonsModule } from 'ngx-bootstrap/buttons';

@NgModule({
    declarations: [CatalogoKpiComponent, CatalogoUtentiComponent, AdminUtentiComponent, AdminKpiComponent],
    imports: [
        CommonModule,
        CatalogoRoutingModule,
        DataTablesModule,
        FormsModule,
        ModalModule.forRoot(),
        ButtonsModule.forRoot()
    ]
})
export class CatalogoModule { }

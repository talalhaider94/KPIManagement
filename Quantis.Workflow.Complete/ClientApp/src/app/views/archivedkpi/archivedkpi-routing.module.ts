import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ArchivedKpiComponent } from './archivedkpi.component';

const routes: Routes = [
    {
        path: '',
        component: ArchivedKpiComponent,
        data: {
            title: 'KPI'
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ArchivedKpiRoutingModule { }

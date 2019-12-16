import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { DatiGrezziComponent } from './datigrezzi.component';

const routes: Routes = [
    {
        path: '',
        component: DatiGrezziComponent,
        data: {
            title: 'Dati Grezzi'
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class DatiRoutingModule { }

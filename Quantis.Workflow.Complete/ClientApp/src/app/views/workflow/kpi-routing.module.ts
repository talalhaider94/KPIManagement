import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RicercaComponent } from './ricerca/ricerca.component';
import { StatoComponent } from './stato/stato.component';
import { StatoDayComponent } from './statoDay/statoDay.component';
import { KPIComponent } from './kpi.component';
import { AmministrazioneComponent } from './amministrazione/amministrazione.component';

const routes: Routes = [
    {
        path: '',
        data: {
            title: 'Work Flow'
        },
        children: [
            {
                path: '',
                redirectTo: 'ricerca'
            },
            {
                path: 'ricerca',
                component: RicercaComponent,
                data: {
                    title: 'Workflow Ricerca'
                }
            },
            {
                path: 'verifica',
                component: KPIComponent,
                data: {
                    title: 'Workflow verifica'
                }
            },
            {
                path: 'amministrazione',
                component: AmministrazioneComponent,
                data: {
                    title: 'Amministrazione'
                }
            }, 
            {
                path: 'stato',
                component: StatoComponent,
                data: {
                    title: 'Stato'
                }
            }, 
            {
                path: 'statoday',
                component: StatoDayComponent,
                data: {
                    title: 'Stato Day'
                }
            }
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class KPIRoutingModule { }

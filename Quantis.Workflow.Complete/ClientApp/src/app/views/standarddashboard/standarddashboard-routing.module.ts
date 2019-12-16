import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { StandardDashboardComponent } from './standarddashboard.component';

const routes: Routes = [
    {
        path: '',
        component: StandardDashboardComponent,
        data: {
            title: 'View Standard Dashboard'
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class StandardDashboardRoutingModule { }

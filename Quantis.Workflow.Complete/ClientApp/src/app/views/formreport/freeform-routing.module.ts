import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { FreeFormComponent } from './freeform.component';

const routes: Routes = [
    {
        path: '',
        component: FreeFormComponent,
        data: {
            title: 'Free Form Report'
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class FreeFormRoutingModule { }

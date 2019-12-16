import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { GeneralBookletComponent } from './general-booklet.component';

const routes: Routes = [
    {
        path: '',
        component: GeneralBookletComponent,
        data: {
            title: 'Booklet'
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class GeneralBookletRoutingModule { }

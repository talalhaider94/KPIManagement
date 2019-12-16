import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommingsoonComponent } from './commingsoon.component';

const routes: Routes = [
    {
        path: '',
        component: CommingsoonComponent,
        data: {
            title: 'Welcome'
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ComingSoonRoutingModule { }

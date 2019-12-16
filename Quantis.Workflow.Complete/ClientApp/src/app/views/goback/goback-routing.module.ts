import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GobackComponent } from './goback.component';

const routes: Routes = [
    {
        path: '',
        component: GobackComponent,
        data: {
            title: 'Welcome'
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class GobackRoutingModule { }

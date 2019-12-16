import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UserProfilingComponent } from './userprofiling.component';

const routes: Routes = [
    {
        path: 'workflowunit',
        component: UserProfilingComponent,
        data: {
            title: 'Profilazione Utente'
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class UserProfilingRoutingModule { }

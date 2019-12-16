import { NgModule } from '@angular/core';
import { Routes, RouterModule, RouterLinkActive } from '@angular/router';
import { BookletComponent } from './booklet.component';
const routes: Routes = [
    {
        path: '',
        component: BookletComponent,
        data: {
            title: 'Booklet da Template'
        }
    }

];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class BookletRoutingModule { }

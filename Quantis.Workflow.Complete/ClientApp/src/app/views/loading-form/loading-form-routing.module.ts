import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoadingFormComponent } from './loading-form.component';
import { LoadingFormDetailComponent } from './loading-form-detail/loading-form-detail.component';
import { LoadingFormUserComponent } from './loading-form-user/loading-form-user.component';
import { LoadingFormCsvComponent } from './loading-form-csv/loading-form-csv.component';
import { LoadingFormUserNotTrackingComponent } from './loading-form-user-nottracking/loading-form-user-nottracking.component';
import { LoadingFormSecurityUserComponent } from './loading-form-securityuser/loading-form-securityuser.component';
import { ProveVarieComponent } from './prove-varie/prove-varie.component';
import { ProveVarieComponentNoTracking } from './prove-varie-notracking/prove-varie-notracking.component';
import { ProveVarieComponentSecurity } from './prove-varie-securityuser/prove-varie.component';
import { LoadingFormAdminComponent } from './loading-form-admin/loading-form-admin.component';

const routes: Routes = [
    {
        path: '',
        data: {
            title: 'Caricamento Dati'
        },
        children: [
            {
                path: '',
                redirectTo: 'admin'
            },
            {
                path: 'admin',
                component: LoadingFormComponent,
                data: {
                    title: 'Admin'
                }
            },
            {
                path: 'utente',
                component: LoadingFormUserComponent,
                data: {
                    title: 'Utente'
                }
            },
            {
              path: 'utente-notracking',
              component: LoadingFormUserNotTrackingComponent,
              data: {
                title: 'LF diverso periodo'
              }
            },
            {
              path: 'utente-csv',
              component: LoadingFormCsvComponent,
              data: {
                title: 'File CSV'
              }
            },
            {
                path: 'securityuser',
                component: LoadingFormSecurityUserComponent,
                data: {
                    title: 'S-Utente'
                }
            },
            {
                path: 'admin/:formId/:formName',
                component: LoadingFormAdminComponent,
                data: {
                    title: 'Admin'
                }
            },
            {
                path: 'utente/:formId/:formName',
                component: ProveVarieComponent,
                data: {
                    title: 'Utente'
                }
            },
            {
              path: 'utente-notracking/:formId/:formName',
              component: ProveVarieComponentNoTracking,
              data: {
                title: 'Utente'
              }
            },
             {
               path: 'securityuser/:formId/:formName',
              component: ProveVarieComponentSecurity,
              data: {
                title: 'S-Utente'
              }
          },
        ],
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LoadingFormRoutingModule { }

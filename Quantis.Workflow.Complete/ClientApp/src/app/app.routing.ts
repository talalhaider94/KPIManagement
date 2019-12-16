import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

// Import Containers
import { DefaultLayoutComponent } from './containers';

import { P404Component } from './views/error/404.component';
import { P500Component } from './views/error/500.component';
import { LoginComponent } from './views/login/login.component';
import { RegisterComponent } from './views/register/register.component';
import { ForgetComponent } from './views/forget/forget.component';
import { KPIComponent } from './views/workflow/kpi.component';

export const routes: Routes = [
    {
        path: 'login',
        component: LoginComponent,
        data: {
            title: 'Login Page'
        }
    },
    {
        path: 'forget',
        component: ForgetComponent,
        data: {
            title: 'Forget Page'
        }
    },
    {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full',
    },
    {
        path: '404',
        component: P404Component,
        data: {
            title: 'Page 404'
        }
    },
    {
        path: '500',
        component: P500Component,
        data: {
            title: 'Page 500'
        }
    },
    {
        path: 'register',
        component: RegisterComponent,
        data: {
            title: 'Register Page'
        }
    },
    {
        path: '',
        component: DefaultLayoutComponent,
        data: {
            title: 'Home'
        },
        children: [
            {
                path: 'base',
                loadChildren: './views/base/base.module#BaseModule'
            },
            {
                path: 'dashboard',
                loadChildren: './views/dashboard/dashboard.module#DashboardModule'
            },
            {
                path: 'coming-soon',
                loadChildren: './views/comingsoon/comingsoon.module#ComingSoonModule'
            },
            {
                path: 'go-back',
                loadChildren: './views/goback/goback.module#GobackModule'
            },
            {
                path: 'workflow',
                loadChildren: './views/workflow/kpi.module#KPIModule'
            },
            {
                path: 'loading-form',
                loadChildren: './views/loading-form/loading-form.module#LoadingFormModule'
            },
            {
                path: 'catalogo',
                loadChildren: './views/catalogo/catalogo.module#CatalogoModule'
            },
            {
                path: 'archivedkpi',
                loadChildren: './views/archivedkpi/archivedkpi.module#ArchivedKpiModule'
            },
            {
                path: 'tconfiguration',
                loadChildren: './views/tconfiguration/tconfiguration.module#TConfigurationModule'
            },
            {
                path: 'sdmgroup',
                loadChildren: './views/sdmgroup/sdmgroup.module#SdmGroupModule'
            },
            {
                path: 'sdmstatus',
                loadChildren: './views/sdmstatus/sdmstatus.module#SdmStatusModule'
            },
            {
                path: 'userprofiling',
                loadChildren: './views/userprofiling/userprofiling.module#UserProfilingModule'
            },
            {
              path: 'workfloworganizationunit',
              loadChildren: './views/workfloworganizationunit/userprofiling.module#UserProfilingModule'
            },
            {
                path: 'adminroles',
                loadChildren: './views/adminroles/adminroles.module#AdminRolesModule'
            },
            {
                path: 'specialreporting',
                loadChildren: './views/email/email.module#EmailModule'
            },
            {
                path: 'datigrezzi',
                loadChildren: './views/datigrezzi/datigrezzi.module#DatiModule'
            },
            {
                path: 'booklet',
                loadChildren: './views/booklet/booklet.module#BookletModule'
            },
            {
                path: 'general-booklet',
                loadChildren: './views/general-booklet/general-booklet.module#GeneralBookletModule'
            },
            {
                path: 'standarddashboard',
                loadChildren: './views/standarddashboard/standarddashboard.module#StandardDashboardModule'
            },
            {
                path: 'freeformreport',
                loadChildren: './views/formreport/freeform.module#FreeFormModule'
            },
        ]
    },
    { path: '**', component: P404Component }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }

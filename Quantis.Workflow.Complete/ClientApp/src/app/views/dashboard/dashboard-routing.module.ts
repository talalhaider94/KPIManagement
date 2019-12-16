import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PublicComponent } from './public/public.component';
import { DashboardComponent } from './dashboard.component';
import { DashboardListsComponent } from './dashboard-lists/dashboard-lists.component';
import { LandingPageComponent } from './landingpage/landingpage.component';
import { ChartDataComponent } from './chartdata/chartdata.component';
import { FreeFormReportComponent } from './free-form-report/free-form-report.component';
import { FormReportQueryComponent } from './form-report-query/form-report-query.component';
import { ImportFormReportComponent } from './import-form-report/import-form-report.component';
import { BSIReportComponent } from './bsi-report/bsi-report.component';
import { PersonalReportComponent } from './personal-report/personal-report.component';
import { LandingPageDetailsComponent } from './landing-page-details/landing-page-details.component';
import { BlankPageComponent } from './blankpage/blankpage.component';
import { AboutComponent } from './about/about.component';

const routes: Routes = [
    // {
    //   path: '',
    //   component: DashboardComponent,
    //   data: {
    //     title: 'Dashboard'
    //   }
    // },
    {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard/1'
    },
    {
        path: 'dashboard/:id',
        component: DashboardComponent,
        data: {
            title: 'Dashboard'
        }
    },
    {
        path: 'public/:id',
        component: PublicComponent,
        data: {
            title: 'Public Dashboard'
        }
    },
    {
        path: 'list',
        component: DashboardListsComponent,
        data: {
            title: 'Dashboards'
        }
    },
    {
        path: 'chartdata',
        component: ChartDataComponent,
        data: {
            title: 'Chart Data'
        }
    },
    {
        path: 'landingpage',
        component: LandingPageComponent,
        data: {
            title: 'Landing Page'
        }
    },
    {
        path: 'free-form-report',
        component: FreeFormReportComponent,
        data: {
            title: 'Free Form Report'
        }
    },
    {
        path: 'form-report-query',
        component: FormReportQueryComponent,
        data: {
            title: 'Form Report Query'
        }
    },
    {
        path: 'import-form-report',
        component: ImportFormReportComponent,
        data: {
            title: 'Import Form Report'
        }
    },
    {
        path: 'bsi-report',
        component: BSIReportComponent,
        data: {
            title: 'Report da BSI'
        }
    },
    {
        path: 'personal-report',
        component: PersonalReportComponent,
        data: {
            title: 'Report personali'
        }
    },
    {
        path: 'landing-page-details',
        component: LandingPageDetailsComponent,
        data: {
            title: 'Landing Page Details'
        }
    },
    {
        path: 'nodashboard',
        component: BlankPageComponent,
        data: {
            title: ''
        }
    },
    {
        path: 'about',
        component: AboutComponent,
        data: {
            title: ''
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class DashboardRoutingModule { }

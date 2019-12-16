import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoadingFormComponent } from './loading-form.component';
import { LoadingFormRoutingModule } from './loading-form-routing.module';
import { LoadingFormDetailComponent } from './loading-form-detail/loading-form-detail.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ProveVarieComponent } from './prove-varie/prove-varie.component';
import { ProveVarieComponentSecurity } from './prove-varie-securityuser/prove-varie.component';
import { ProveVarieComponentNoTracking } from './prove-varie-notracking/prove-varie-notracking.component';
import { DataTablesModule } from 'angular-datatables';
import { Ng2SearchPipeModule } from 'ng2-search-filter';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { ModalModule } from 'ngx-bootstrap/modal';

import {
    MatSidenavModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatRippleModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatMenuModule,
    MatIconModule,
    MatToolbarModule,
    MatSelectModule,
    MatTableModule,
    MatSortModule,
    MatDatepickerModule,
    MAT_DATE_LOCALE,
    MatAutocompleteModule,
    MatButtonToggleModule,
    MatCheckboxModule,
    MatChipsModule,
    MatDialogModule,
    MatDividerModule,
    MatExpansionModule,
    MatGridListModule,
    MatListModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatRadioModule,
    MatSliderModule,
    MatSlideToggleModule,
    MatSnackBarModule,
    MatStepperModule,
    MatTooltipModule
} from '@angular/material';
import { FileUploadModule } from 'ng2-file-upload';
import { LoadingFormUserComponent } from './loading-form-user/loading-form-user.component';
import { LoadingFormUserNotTrackingComponent } from './loading-form-user-nottracking/loading-form-user-nottracking.component';
import { LoadingFormSecurityUserComponent } from './loading-form-securityuser/loading-form-securityuser.component';
import { LoadingFormAdminComponent } from './loading-form-admin/loading-form-admin.component';
import { LoadingFormCsvComponent } from './loading-form-csv/loading-form-csv.component';

@NgModule({
    declarations: [
        LoadingFormComponent,
        LoadingFormDetailComponent,
        ProveVarieComponent,
        ProveVarieComponentSecurity,
        ProveVarieComponentNoTracking,
        LoadingFormUserComponent,
        LoadingFormSecurityUserComponent,
        LoadingFormUserNotTrackingComponent,
        LoadingFormCsvComponent,
        LoadingFormAdminComponent],
    imports: [
        CommonModule,
        LoadingFormRoutingModule,
        FileUploadModule,
        ReactiveFormsModule,
        FormsModule,
        MatSidenavModule,
        MatButtonModule,
        MatFormFieldModule,
        MatInputModule,
        MatRippleModule,
        MatCardModule,
        MatProgressSpinnerModule,
        MatMenuModule,
        MatIconModule,
        MatToolbarModule,
        MatSelectModule,
        MatTableModule,
        MatSortModule,
        MatDatepickerModule,
        // MAT_DATE_LOCALE,
        MatAutocompleteModule,
        MatButtonToggleModule,
        MatCheckboxModule,
        MatChipsModule,
        MatDialogModule,
        MatDividerModule,
        MatExpansionModule,
        MatGridListModule,
        MatListModule,
        MatNativeDateModule,
        MatPaginatorModule,
        MatProgressBarModule,
        MatRadioModule,
        MatSliderModule,
        MatSlideToggleModule,
        MatSnackBarModule,
        MatStepperModule,
        MatTooltipModule,
        DataTablesModule,
        Ng2SearchPipeModule,
        CollapseModule.forRoot(),
        ModalModule.forRoot(),
    ],
    providers: [
        { provide: MAT_DATE_LOCALE, useValue: 'it-IT' },
    ],
})
export class LoadingFormModule { }

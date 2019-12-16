import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { DataTableDirective } from 'angular-datatables';
import { DashboardService, AuthService } from '../../../_services';
import { DateTimeService } from '../../../_helpers'

@Component({
    selector: 'app-dashboard-lists',
    templateUrl: './dashboard-lists.component.html',
    styleUrls: ['./dashboard-lists.component.scss']
})
export class DashboardListsComponent implements OnInit {
    loading: boolean = false;
    formLoading: boolean = false;
    submitted: boolean = false;
    dashboards: Array<any> = [];
    createDashboardForm: FormGroup;

    @ViewChild('DashboardTable') block: ElementRef;
    @ViewChild(DataTableDirective) private datatableElement: DataTableDirective;

    dtOptions: DataTables.Settings = {
        language: {
            processing: "Elaborazione...",
            search: "Cerca:",
            lengthMenu: "Visualizza _MENU_ elementi",
            info: "Vista da _START_ a _END_ di _TOTAL_ elementi",
            infoEmpty: "Vista da 0 a 0 di 0 elementi",
            infoFiltered: "(filtrati da _MAX_ elementi totali)",
            infoPostFix: "",
            loadingRecords: "Caricamento...",
            zeroRecords: "La ricerca non ha portato alcun risultato.",
            emptyTable: "Nessun dato presente nella tabella.",
            paginate: {
                first: "Primo",
                previous: "Precedente",
                next: "Seguente",
                last: "Ultimo"
            },
            aria: {
                sortAscending: ": attiva per ordinare la colonna in ordine crescente",
                sortDescending: ":attiva per ordinare la colonna in ordine decrescente"
            }
        }
    };

    dtTrigger: Subject<any> = new Subject();

    @ViewChild('createDashboardModal') public createDashboardModal: ModalDirective;
    constructor(
        private dashboardService: DashboardService,
        private toastr: ToastrService,
        private authService: AuthService,
        private formBuilder: FormBuilder,
        private dateTime: DateTimeService
    ) { }

    get f() { return this.createDashboardForm.controls; }

    ngOnInit() {
        this.createDashboardForm = this.formBuilder.group({
            id: ['', Validators.required],
            name: ['', Validators.required],
            owner: ['', Validators.required],
            globalfilterId: ['', Validators.required],
            createdon: ['', Validators.required],
            dashboardwidgets: [''],
        });
        this.getUserDashboards();
    }

    ngAfterViewInit() {
        this.dtTrigger.next();
        this.setUpDataTableDependencies();
        this.getUserDashboards();
    }

    ngOnDestroy(): void {
        // Do not forget to unsubscribe the event
        this.dtTrigger.unsubscribe();
    }

    rerender(): void {
        this.datatableElement.dtInstance.then((dtInstance: DataTables.Api) => {
            // Destroy the table first
            dtInstance.destroy();
            // Call the dtTrigger to rerender again
            this.dtTrigger.next();
            this.setUpDataTableDependencies();
        });
    }

    setUpDataTableDependencies() {
    }

    getUserDashboards() {
        this.loading = true;
        this.dashboardService.getDashboards().subscribe(dashboards => {
            this.dashboards = dashboards;
            //debugger
            this.loading = false;
            this.rerender();
        }, error => {
            console.error('getDashboards', error);
            this.toastr.error('Error while loading dashboards');
            this.loading = false;
        });
    }

    getFormatDate(date){
        return this.dateTime.convertUtcToDateTime(date)
    }

    createDashboard() {
        let loggedInUser = this.authService.currentUserValue;
        this.createDashboardForm.patchValue({
            id: 0,
            owner: loggedInUser.username,
            globalfilterId: 0,
            createdon: new Date(),
            modifiedon: new Date(),
            dashboardwidgets: []
        })
        this.createDashboardModal.show();
    }

    onDashboardFormSubmit() {
        this.submitted = true;
        if (this.createDashboardForm.invalid) {
            this.createDashboardModal.show();
        } else {
            this.createDashboardModal.show();
            this.formLoading = true;
            this.dashboardService.updateDashboard(this.createDashboardForm.value).subscribe(dashboardCreated => {
                this.createDashboardModal.hide();
                this.getUserDashboards();
                this.formLoading = false;
                this.toastr.success('Dashboard created successfully');
            }, error => {
                this.createDashboardModal.hide();
                this.formLoading = false;
                this.toastr.error('Error while creating dashboard');
            });
        }
    }

    dashboardStatus(dashboardId, status) {
        this.loading = true;
        if (status) {
            this.dashboardService.deactivateDashboard(dashboardId).subscribe(result => {
                this.loading = false;
                this.getUserDashboards();
            }, error => {
                this.loading = false;
                this.toastr.error('Error while deactivating dashboard');
            })
        } else {
            this.dashboardService.activateDashboard(dashboardId).subscribe(result => {
                this.loading = false;
                this.getUserDashboards();
            }, error => {
                this.loading = false;
                this.toastr.error('Error while activating dashboard');
            })
        }
    }

    deleteDashboard(id){
        this.loading = true;
        this.dashboardService.DeleteDashboard(id).subscribe(result => {
            this.toastr.success('Dashboard deleted successfully');
            this.loading = false;
            this.getUserDashboards();
        }, error => {
            this.loading = false;
            this.toastr.error('Error in deleting dashboard');
        })
    }
    
    onCancel(dismissMethod: string): void {
        console.log('Cancel ', dismissMethod);
    }
}

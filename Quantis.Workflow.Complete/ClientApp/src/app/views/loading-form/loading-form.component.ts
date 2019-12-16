import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { LoadingFormService, AuthService } from '../../_services';
import { first } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { DataTableDirective } from 'angular-datatables';

@Component({
    // selector: 'app-loading-form',
    templateUrl: './loading-form.component.html',
    styleUrls: ['./loading-form.component.scss']
})
export class LoadingFormComponent implements OnInit, OnDestroy {
    loadingForms: any = [];
    loading: boolean = true;
    @ViewChild(DataTableDirective)
    datatableElement: DataTableDirective;
    dtOptions: DataTables.Settings = {};
    dtTrigger = new Subject();
    isAdmin: boolean = false;
    constructor(
        private router: Router,
        private loadingFormService: LoadingFormService,
        private authService: AuthService
    ) { }

    ngOnInit() {
        // Danial TODO: Some role permission logic is needed here.
        // Admin and super admin can access this
        this.isAdmin = this.authService.getUser().isadmin;
        this.dtOptions = {
            pagingType: 'full_numbers',
            pageLength: 10,
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
        this.loadingFormService.getLoadingForms().pipe(first()).subscribe(data => {
            this.loadingForms = data;
            this.dtTrigger.next();
            this.loading = false;
        }, error => {
            console.error('LoadingFormComponent', error)
            this.loading = false;
        })
    }

    // clickrow(data) {
    //   this.router.navigate(['/loading-form/form', '2', '2']);
    // }
    ngOnDestroy(): void {
        // Do not forget to unsubscribe the event
        this.dtTrigger.unsubscribe();
    }
}

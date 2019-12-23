import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { LoadingFormService, AuthService } from '../../../_services';
import { first } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { DataTableDirective } from 'angular-datatables';
import * as moment from 'moment';

@Component({
    selector: 'app-loading-form-securityuser',
    templateUrl: './loading-form-securityuser.component.html',
    styleUrls: ['./loading-form-securityuser.component.scss']
})
export class LoadingFormSecurityUserComponent implements OnInit, OnDestroy {
    loadingForms: any = [];
    loading: boolean = true;
    @ViewChild(DataTableDirective)
    datatableElement: DataTableDirective;
    dtOptions: DataTables.Settings = {};
    dtTrigger = new Subject();

    constructor(
        private router: Router,
        private loadingFormService: LoadingFormService,
        private authService: AuthService
    ) { }

    ngOnInit() {
        // Danial TODO: Some role permission logic is needed here.
        // Admin and super admin can access this
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
                    sortAscending: ":attiva per ordinare la colonna in ordine crescente",
                    sortDescending: ":attiva per ordinare la colonna in ordine decrescente"
                }
            }
        };
        const currentUser = this.authService.getUser();
        // getLoadingForms()
        this.loadingFormService.getFormsByUserId(0, 'Admin').pipe(first()).subscribe(data => { //userid = 0 to get all forms like a superuser
            console.log('getFormsByUserId', data);
            this.loadingForms = data;
            this.dtTrigger.next();
            this.loading = false;
        }, error => {
            console.error('getFormsByUserId', error);
            this.loading = false;
        })
    }

    ngOnDestroy() {
        this.dtTrigger.unsubscribe();
    }

    cutOffRow(row) {
        if (row.cutoff) {
            let currentDate = moment().format();
            let isDateBefore = moment(row.modify_date).isBefore(currentDate);
            if (isDateBefore) {
                return true;
            } else {
                return false;
            }
        } else {
            return false
        }
    }

    formatInputDate(date) {
        if (date) {
            if (moment(date).isSame(moment('0001/01/01, 00:00:00'))) {
                return 'Nessun caricamento';
            } else {
              return moment(date).format('YYYY/MM/DD, HH:mm:ss');
            }
        } else {
            return 'N/A';
        }
    }
  formatInputDateIT(date) {
    if (date) {
      if (moment(date).isSame(moment('0001-01-01T00:00:00'))) {
        return 'Nessun caricamento';
      } else {
        return moment(date).format('DD/MM/YYYY HH:mm:ss');
      }
    } else {
      return 'N/A';
    }
  }

}

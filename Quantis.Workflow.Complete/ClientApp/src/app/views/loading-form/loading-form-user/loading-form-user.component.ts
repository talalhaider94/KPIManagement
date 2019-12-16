import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { LoadingFormService, AuthService } from '../../../_services';
import { first } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { DataTableDirective } from 'angular-datatables';
import * as moment from 'moment';

@Component({
    selector: 'app-loading-form-user',
    templateUrl: './loading-form-user.component.html',
    styleUrls: ['./loading-form-user.component.scss']
})
export class LoadingFormUserComponent implements OnInit, OnDestroy {
    loadingForms: any = [];
    detailsForms: any = {};
    loading: boolean = true;
    @ViewChild(DataTableDirective)
    @ViewChild('showOnReady') showOnReady: ElementRef;
    datatableElement: DataTableDirective;
    dtOptions: DataTables.Settings = {};
    dtTrigger = new Subject();

    constructor(
        private router: Router,
        private loadingFormService: LoadingFormService,
        private authService: AuthService
    ) { }

    ngOnInit() {
        $('#showOnReady').hide();
        // Danial TODO: Some role permission logic is needed here.
        // Admin and super admin can access this
        this.dtOptions = {
            pagingType: 'full_numbers',
            pageLength: 10,
            columnDefs: [
                { "visible": false, "targets": 0 }
            ],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;
                api.column(0, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        $(rows).eq(i).before(
                            '<tr style="background-color:#eedc00" class="group"><th colspan="6">' + group + '</th></tr>'
                        );
                        last = group;
                    }
                });
            },
            initComplete: function () {
                $('#showOnReady').show();
                $('#loadingDiv').hide();
                console.log(this)
            },
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
        this.loadingFormService.getFormsByUserId(currentUser.userid, 'tracking').pipe(first()).subscribe(data => {
            console.log('getFormsByUserId', data);
            this.loadingForms = data;
            this.dtTrigger.next();

            //
            var groupBy = function (xs, key) {
                return xs.reduce(function (rv, x) {
                    //(rv[x[key]] = rv[x[key]] || []).push(x);
                    //(rv[x[key]] = rv[x[key]] || [])[x.form_id] = { form_name: x.form_name };
                    var index = (rv[x[key]] = rv[x[key]] || []).findIndex(e => e.form_id == x.form_id);
                    if (index === -1) {
                        (rv[x[key]] = rv[x[key]] || []).push(x);
                    }
                    return rv;
                }, {});
            };
            this.detailsForms = groupBy(data, 'global_rule_id')
            console.log(this.detailsForms)
            //console.log(this.detailsForms['37077'])
            //this.loadingForms = groupBy(data, 'global_rule_id')
            //console.log(groubedByTeam);
            //
        }, error => {
            console.error('getFormsByUserId', error);
            this.loading = false;
        })
    }

    ngOnDestroy() {
        this.dtTrigger.unsubscribe();
    }
    loadingCompleted() {
        this.loading = false;
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
            if (moment(date).isSame(moment('0001-01-01T00:00:00'))) {
                return 'Nessun caricamento';
            } else {
                return moment(date).format('YYYY/MM/DD HH:mm:ss');
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

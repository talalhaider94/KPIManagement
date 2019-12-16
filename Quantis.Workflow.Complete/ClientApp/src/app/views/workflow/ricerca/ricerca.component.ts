import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { WorkFlowService } from '../../../_services';
import { first } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { DataTableDirective } from 'angular-datatables';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import * as moment from 'moment';
import WorkFlowHelper from '../../../_helpers/workflow';
import { FileHelpersService } from '../../../_helpers';
declare var $;
let $this;

@Component({
    selector: 'app-ricerca',
    templateUrl: './ricerca.component.html',
    styleUrls: ['./ricerca.component.scss']
})
export class RicercaComponent implements OnInit, OnDestroy {
    @ViewChild('successModal') public successModal: ModalDirective;
    @ViewChild('infoModal') public infoModal: ModalDirective;

    @ViewChild('ricercatable') block: ElementRef;
    @ViewChild('searchCol2') searchCol2: ElementRef;
    @ViewChild('searchCol3') searchCol3: ElementRef;
    @ViewChild('searchCol4') searchCol4: ElementRef;
    @ViewChild('searchCol5') searchCol5: ElementRef;

    @ViewChild('monthSelect') monthSelect: ElementRef;
    @ViewChild('yearSelect') yearSelect: ElementRef;
    allTickets: any = [];
    getTicketHistories: any = [];
    getTicketAttachments: any = [];
    loading: boolean = true;
    @ViewChild(DataTableDirective)
    datatableElement: DataTableDirective;
    // dtOptions: DataTables.Settings = {};
    dtOptions: any = {};
    dtTrigger = new Subject();
    bsValue = new Date();
    isCollapsed = true;
    daterangepickerModel: Date[];

    monthOption;
    yearOption;

    constructor(
        private workFlowService: WorkFlowService,
        private toastr: ToastrService,
        private fileHelper: FileHelpersService
    ) {
        $this = this;
    }

    ngOnInit() {
        this.monthOption = moment().subtract(1, 'months').format('MM');
        this.yearOption = moment().format('YY');

        this.dtOptions = {
            pagingType: 'full_numbers',
            pageLength: 10,
            destroy: false,
            dom: 'lBfrtip',
            search: {
                caseInsensitive: true
            },
            buttons: [
                {
                    extend: 'csv',
                    text: '<i class="fa fa-file"></i> Esporta CSV',
                    titleAttr: 'Esporta CSV',
                    className: 'btn btn-primary mb-3'
                },
            ],
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
                emptyTable: "Nessun Ticket Trovato.",
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
    }

    getRicercaTickets() {
        let period;
        if (this.monthOption === 'all' && this.yearOption === 'all') {
            period = 'all/all';
        } else {
            period = `${this.monthOption}/${this.yearOption}`;
        }
        this.workFlowService.getTicketsSearchByUserRecerca(period).pipe(first()).subscribe(data => {
            console.log('getTicketsSearchByUserRecerca', data);
            if (!!data && data.length > 0) {
                this.allTickets = data;
            } else {
                this.allTickets = null;
            }
            this.rerender();
            this.loading = false;
        }, error => {
            console.error('getTicketsSearchByUserRecerca', error);
            this.allTickets = null;
            this.loading = false;
        })
    }

    ticketActions(ticket) {
        this.loading = true;
        this.workFlowService.getTicketHistory(ticket.id).pipe(first()).subscribe(data => {
            // this.getTicketHistories = data.filter(ticketHistory => ticketHistory.id === ticket.id);
            if (!!data) {
                this.getTicketHistories = data;
                console.log('ticketActions', data);
            }
            this.successModal.show();
            this.loading = false;
        }, error => {
            this.loading = false;
        });
    }

    ticketAttachments(ticket) {
        this.loading = true;
        this.workFlowService.getAttachmentsByTicket(ticket.id).pipe(first()).subscribe(data => {
            if (!!data) {
                this.getTicketAttachments = data;
                console.log('ticketAttachments', data);
            }
            this.infoModal.show();
            this.loading = false;
        }, error => {
            this.loading = false;
        });
    }

    downloadFile(fileName, fileHandler) {
        this.workFlowService.downloadAttachment(fileHandler).pipe(first()).subscribe(base64Data => {
            this.fileHelper.downloadFile(base64Data, fileName);
        }, error => {
            this.toastr.error('Error while downloading from Server.')
            console.error('downloadFile ==>', error)
        })
    }

    ngOnDestroy() {
        this.dtTrigger.unsubscribe();
    }

    // search start
    onDataChange() {
        this.loading = true;
        this.getRicercaTickets();
    }

    ngAfterViewInit() {
        this.dtTrigger.next();
        this.setUpDataTableDependencies();
        this.getRicercaTickets();
    }
    rerender(): void {
        this.datatableElement.dtInstance.then((dtInstance: DataTables.Api) => {
            dtInstance.destroy();
            this.dtTrigger.next();
            this.setUpDataTableDependencies();
        });
    }

    setUpDataTableDependencies() {
        // $this.datatableElement.dtInstance.then((datatable_Ref: DataTables.Api) => {
        //   datatable_Ref.columns(10).every(function () {
        //     const that = this;
        //     that.search(moment().subtract(1, 'months').format('MM/YY')).draw();
        //     $($this.monthSelect.nativeElement).on('change', function () {
        //       that.search(`${$(this).val()}/${$this.yearSelect.nativeElement.value}`).draw();
        //     });
        //   });
        // });

        // $this.datatableElement.dtInstance.then((datatable_Ref: DataTables.Api) => {
        //   datatable_Ref.columns(10).every(function () {
        //     const that = this;
        //     that.search(moment().subtract(1, 'months').format('MM/YY')).draw();
        //     $($this.yearSelect.nativeElement).on('change', function () {
        //       that.search(`${$this.monthSelect.nativeElement.value}/${$(this).val()}`).draw();
        //     });
        //   });
        // });

        $(this.searchCol2.nativeElement).on('keyup', function () {
            $this.datatableElement.dtInstance.then((datatable_Ref: DataTables.Api) => {
                datatable_Ref
                    .columns(1)
                    .search(this.value)
                    .draw();
            });
        });

        $(this.searchCol3.nativeElement).on('keyup', function () {
            $this.datatableElement.dtInstance.then((datatable_Ref: DataTables.Api) => {
                datatable_Ref
                    .columns(2)
                    .search(this.value)
                    .draw();
            });
        });

        $this.datatableElement.dtInstance.then((datatable_Ref: DataTables.Api) => {
            datatable_Ref.columns(7).every(function () {
                const that = this;
                // Create the select list and search operation
                const select = $($this.searchCol4.nativeElement)
                    .on('change', function () {
                        that
                            .search($(this).val())
                            .draw();
                    });
                // Get the search data for the first column and add to the select list
                select.empty();
                select.append($('<option value="">Stato</option>'));
                this
                    .cache('search')
                    .sort()
                    .unique()
                    .each(function (d) {
                        select.append($('<option value="' + d + '">' + d + '</option>'));
                    });
            });
        });
        $(this.searchCol5.nativeElement).on('keyup', function () {
            $this.datatableElement.dtInstance.then((datatable_Ref: DataTables.Api) => {
                datatable_Ref
                    .columns(5)
                    .search(this.value)
                    .draw();
            });
        });
    }
    //search end
    formatDescriptionColumn(description) {
        return WorkFlowHelper.formatDescription(description);
    }

    formatSummaryColumn(summary) {
        return WorkFlowHelper.formatSummary(summary);
    }
}

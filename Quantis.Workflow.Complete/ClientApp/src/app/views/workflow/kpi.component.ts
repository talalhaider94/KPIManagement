import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { WorkFlowService } from '../../_services';
import { FileHelpersService } from '../../_helpers';
import { first, delay, mergeMap, retryWhen, concatMap, map } from 'rxjs/operators';
import { Subject, Observable, of, throwError } from 'rxjs';
import { DataTableDirective } from 'angular-datatables';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { FileUploader } from 'ng2-file-upload';
import * as moment from 'moment';
import WorkFlowHelper from '../../_helpers/workflow';
const URL = 'https://evening-anchorage-3159.herokuapp.com/api/';

declare var $;
let $this;

@Component({
    templateUrl: './kpi.component.html',
    styleUrls: ['./kpi.component.scss']
})
export class KPIComponent implements OnInit, OnDestroy {
    @ViewChild('successModal') public successModal: ModalDirective;
    @ViewChild('infoModal') public infoModal: ModalDirective;
    @ViewChild('approveModal') public approveModal: ModalDirective;
    @ViewChild('rejectModal') public rejectModal: ModalDirective;
    @ViewChild('ticketStatusModal') public ticketStatusModal: ModalDirective;

    @ViewChild('monthSelect') monthSelect: ElementRef;
    @ViewChild('yearSelect') yearSelect: ElementRef;
    @ViewChild('statoKPISelect') statoKPISelect: ElementRef;

    submitted = false;
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
    setActiveTicketId: Number;
    approveForm: FormGroup;
    rejectForm: FormGroup;
    selectedTickets: any = [];
    verificaCheckBoxForm: FormGroup;
    fileUploading = false;
    public uploader: FileUploader = new FileUploader({ url: URL });
    selectedAll: any;
    monthOption;
    yearOption;
    statoKPIOption;
    dataFromWidgetsPage;

    ticketsStatus: any = [];
    constructor(
        private workFlowService: WorkFlowService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder,
        private route: ActivatedRoute,
        private location: Location,
        private fileHelper: FileHelpersService
    ) {
        $this = this;
    }

    get approveValues() { return this.approveForm.controls; }
    get rejectValues() { return this.rejectForm.controls; }

    ngOnInit() {
        //this.dataFromWidgetsPage = window.history.state;
        this.dataFromWidgetsPage = this.route.snapshot.queryParamMap['params'];
        this.location.replaceState('/workflow/verifica'); // remove query params from url after getting its value
        console.log('dataFromWidgetsPage: ', this.dataFromWidgetsPage);

        this.monthOption = this.dataFromWidgetsPage.m || moment().subtract(1, 'months').format('MM');
        this.yearOption = this.dataFromWidgetsPage.y || moment().format('YY');
        this.statoKPIOption = '';
        this.verificaCheckBoxForm = this.formBuilder.group({
            selectTicket: [''],
            selectAllTickets: ['']
        });
        this.approveForm = this.formBuilder.group({
            description: ['']
        });

        this.rejectForm = this.formBuilder.group({
            description: ['', [Validators.required]]
        });

        this.dtOptions = {
            pagingType: 'full_numbers',
            pageLength: 10,
            destroy: false, // check here.
            dom: 'lBfrtip',
            search: {
                caseInsensitive: true
            },
           "columnDefs": [{
                "targets": 0,
                "orderable": false,
                "visible": true,
                "searchable": false
            },
            ],   
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
    }

    _getAllTickets() {
        this.workFlowService.getTicketsVerificationByUserVerifica(`${this.monthOption}/${this.yearOption}`).pipe(first()).subscribe(data => {
            console.log('getTicketsVerificationByUserVerifica', data);
            const appendSelectFalse = data.map(ticket => ({ ...ticket, selected: false }));
            this.allTickets = appendSelectFalse;
            //this.dtTrigger.next();
            this.rerender();
            this.loading = false;
        }, error => {
            console.error('getTicketsVerificationByUserVerifica', error);
            this.loading = false;
        });
    }

    onDataChange() {
        this.loading = true;
        this._getAllTickets();
    }

    ngAfterViewInit() {
        this.dtTrigger.next();
        this.setUpDataTableDependencies();
        this._getAllTickets();
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
    ticketActions(ticket) {
        this.loading = true;
        this.setActiveTicketId = +ticket.id;
        this.workFlowService.getTicketHistory(ticket.id).pipe(first()).subscribe(data => {
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
        this.setActiveTicketId = +ticket.id;
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

    rejectTicket() {
        const selectedTickets = this.allTickets.filter(ticket => ticket.selected);
        this.selectedTickets = selectedTickets;
        if (this.selectedTickets.length > 0) {
            if (this.selectedTickets.length === 1) {
                this.rejectModal.show();
            } else {
                this.toastr.info('L’operazione di Rifiuto è consentita su un solo ticket');
            }
        } else {
            this.toastr.info('Selezionare almeno un ticket');
        }
    }

    approveTicket() {
        const selectedTickets = this.allTickets.filter(ticket => ticket.selected);
        this.selectedTickets = selectedTickets;
        if (this.selectedTickets.length > 0) {
            this.approveModal.show();
        } else {
            this.toastr.info('Selezionare almeno un ticket');
        }
    }

    approveFormSubmit() {
        this.submitted = true;
        const { description } = this.approveValues;
        this.loading = true;
        this.ticketsStatus = [];
        this.ticketStatusModal.show();
        this.approveModal.hide();
        const myObserver = {
            next: status => {
                this.ticketsStatus.push(status);
            },
            error: err => {
                this.approveModal.hide();
                console.error('approveFormSubmit', err);
                this.toastr.error('Error while approving form', 'Error');
                this.loading = false;
            },
            complete: () => {
                this._getAllTickets();
                this.loading = false;
            },
        };

        of(...this.selectedTickets)
            .pipe(concatMap((ticket) => {
                return this.workFlowService.escalateTicketbyID(ticket.id, ticket.status, description.value || null).pipe(map(result => {
                    // Danial: here just for testing
                    // result = {
                    //   isbsistatuschanged: true,
                    //   issdmstatuschanged: false,
                    //   showarchivemsg: true,
                    //   isarchive: false
                    //  }
                    return { ticket, result };
                }));
            })).subscribe(myObserver);
    }

    rejectFormSubmit() {
        this.submitted = true;
        if (this.rejectForm.invalid) {
            return;
        } else {
            const { description } = this.rejectValues;
            this.loading = true;
            this.ticketsStatus = [];
            const myObserver = {
                next: status => {
                    this.ticketsStatus.push(status);
                    this.ticketStatusModal.show();
                    this.rejectModal.hide();
                },
                error: err => {
                    this.rejectModal.hide();
                    console.error('rejectFormSubmit', err);
                    this.toastr.error('Error while rejecting form', 'Error');
                    this.loading = false;
                },
                complete: () => {
                    this._getAllTickets();
                    this.loading = false;
                },
            };
            of(...this.selectedTickets)
                .pipe(concatMap((ticket) => {
                    return this.workFlowService.transferTicketByID(ticket.id, ticket.status, description.value || null).pipe(map(result => {
                        return { ticket, result };
                    }));
                })).subscribe(myObserver);
        }
    }

    ngOnDestroy() {
        this.dtTrigger.unsubscribe();
    }

    fileUploadUI() {
        if (this.uploader.queue.length > 0) {
            console.log('this.uploader', this.uploader);
            this.uploader.queue.forEach((element, index) => {
                let file = element._file;
                this._getUploadedFile(file);
            });
        } else {
            this.toastr.info('Nessun documento da caricare');
        }
    }

    _getUploadedFile(file) {
        this.fileUploading = true;
        const reader: FileReader = new FileReader();
        reader.onloadend = (function (theFile, self) {
            let fileName = theFile.name;
            return function (readerEvent) {
                let binaryString = readerEvent.target.result;
                let base64Data = btoa(binaryString);
                self.fileUploading = false;
                self.workFlowService.uploadAttachmentToTicket(self.setActiveTicketId, fileName, base64Data).pipe(self.delayedRetries(10000, 3)).subscribe(data => {
                    console.log('uploadAttachmentToTicket ==>', data);
                    self.fileUploading = false;
                    self.removeFileFromQueue(fileName);
                    // self.uploader.queue.pop();
                    self.toastr.success(`${fileName} uploaded successfully.`);
                    if (data.status === 200 || data.status === 204) {
                        self.workFlowService.getAttachmentsByTicket(self.setActiveTicketId).pipe(first()).subscribe(data => {
                            if (!!data) {
                                self.getTicketAttachments = data;
                                console.log('ticketAttachments', data);
                            }
                            self.loading = false;
                        }, error => {
                            self.loading = false;
                        });
                    }
                }, error => {
                    console.error('uploadAttachmentToTicket ==>', error);
                    self.fileUploading = false;
                    self.toastr.error('Some error occurred while uploading file');
                });
            };
        })(file, this);
        reader.readAsBinaryString(file); // return only base64 string
    }

    selectAll() {
        for (var i = 0; i < this.allTickets.length; i++) {
            if (!this.allTickets[i].isclosed) {
                this.allTickets[i].selected = this.selectedAll;
            }
        }
    }

    checkIfAllSelected() {
        const notClosedTickets = this.allTickets.filter(ticket => !ticket.isclosed);

        this.selectedAll = notClosedTickets.every(function (ticket: any) {
            return ticket.selected == true;
        })
    }

    // search start

    setUpDataTableDependencies() {
        $this.datatableElement.dtInstance.then((datatable_Ref: DataTables.Api) => {
            datatable_Ref.columns(10).every(function () {
                const that = this;
                $($this.statoKPISelect.nativeElement).on('change', function () {
                    that.search($(this).val()).draw();
                });
            });
        });
    }
    //search end

    formatDescriptionColumn(description) {
        let formatDesc =  WorkFlowHelper.formatDescription(description);
        return formatDesc;
    }

    formatSummaryColumn(summary) {
        return WorkFlowHelper.formatSummary(summary);
    }
    removeFileFromQueue(fileName: string) {
        for (let i = 0; i < this.uploader.queue.length; i++) {
            if (this.uploader.queue[i].file.name === fileName) {
                this.uploader.queue[i].remove();
                return;
            }
        }
    }

    delayedRetries(delayMs: number, maxRetry: number) {
        let retries = maxRetry;
        return (src: Observable<any>) => src.pipe(retryWhen((errors: Observable<any>) => errors.pipe(
            delay(delayMs),
            mergeMap(error => retries-- > 0 ? of(error) : throwError(`Tried to upload ${maxRetry} times. without success.`))
        )
        ))
    }

    formatCheckColumn() {
        let a = WorkFlowHelper.formatCheckBoxColumn();
        return a;
    }
}

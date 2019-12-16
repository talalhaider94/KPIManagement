import { Component, OnInit, ViewChild } from '@angular/core';
import { DataTableDirective } from 'angular-datatables';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { first } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { DateTimeService } from '../../../_helpers';
import { WorkFlowService } from '../../../_services';
import WorkFlowHelper from '../../../_helpers/workflow';

@Component({
    selector: 'app-amministrazione',
    templateUrl: './amministrazione.component.html',
    styleUrls: ['./amministrazione.component.scss']
})
export class AmministrazioneComponent implements OnInit {
    monthOption;
    yearOption;
    isCollapsed = true;
    @ViewChild(DataTableDirective)
    datatableElement: DataTableDirective;
    dtOptions: any = {};
    dtTrigger = new Subject();
    @ViewChild('editTicketModal') public editTicketModal: ModalDirective;
    editTicketForm: FormGroup;
    loading: boolean = true;
    submitted: boolean = false;
    searchedTickets: any = [];

    editTicketId: String = '';
    editTicketRef: String = '';
    editTicketTarget: String = '';

    constructor(
        private dateTimeHelper: DateTimeService,
        private workFlowService: WorkFlowService,
        private formBuilder: FormBuilder,
        private toastr: ToastrService,
    ) { }

    get editTicketValues() { return this.editTicketForm.controls; }

    ngOnInit() {
        const { month, year } = this.dateTimeHelper.getMonthYear();
        this.monthOption = month;
        this.yearOption = year;

        this.editTicketForm = this.formBuilder.group({
            Value: ['', [Validators.required, Validators.min(0)]],
            Sign: ['', [Validators.required, Validators.minLength(1)]],
            Type: ['', [Validators.required]],
            Note: ['', [Validators.required]],
        });

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

    getTicketsForVilore() {
        const period = this.dateTimeHelper.getApiPeriod(this.monthOption, this.yearOption);
        this.workFlowService.getViloreByUser(period).pipe(first()).subscribe(data => {
            console.log('getViloreByUser', data);
            if (!!data && data.length > 0) {
                this.searchedTickets = data;
            } else {
                this.searchedTickets = null;
            }
            this.rerender();
            this.loading = false;
        }, error => {
            console.error('getViloreByUser', error);
            this.searchedTickets = null;
            this.loading = false;
        })
    }

    ngOnDestroy() {
        this.dtTrigger.unsubscribe();
    }

    // search start
    onDataChange() {
        this.loading = true;
        this.getTicketsForVilore();
    }

    ngAfterViewInit() {
        this.dtTrigger.next();
        this.getTicketsForVilore();
    }

    rerender(): void {
        this.datatableElement.dtInstance.then((dtInstance: DataTables.Api) => {
            dtInstance.destroy();
            this.dtTrigger.next();
        });
    }

    openModal(row) {
        this.editTicketId = row.id;
        this.editTicketRef = row.ref_num;
        this.editTicketTarget = this.parseTargetValue(row.description);
        this.editTicketModal.show();
    }

    onEditTicketFormSubmit() {
        this.submitted = true;
        if (this.editTicketForm.invalid) {
            this.toastr.error('Inserisci i campi in maniera corretta.', 'Errore');
            return;
        } else {
            this.loading = true;
            const { Value, Note, Sign, Type } = this.editTicketValues;

            let editTicketObj = {
                TicketId: this.editTicketId,
                Value: Value.value,
                Note: Note.value,
                Sign: Sign.value,
                Type: Type.value
            }
            this.workFlowService.UpdateTicketValue(editTicketObj).pipe(first()).subscribe(data => {
                this.toastr.success('Ticket edited successfully.');
                this.loading = false;
                this.editTicketModal.hide();
                this.getTicketsForVilore();
            }, error => {
                console.log('onEditTicketFormSubmit: error', error);
                this.toastr.error(error.error, error.description);
                this.editTicketModal.hide();
                this.loading = false;
            })
        }
    }

    parseTargetValue(description) {
        let target = WorkFlowHelper.getDescriptionField(description, 'TARGET:');
        return (target) ? target.value : target;
    }

    parseValoreValue(description) {
        let target = WorkFlowHelper.getDescriptionField(description, 'VALORE:');
        return (target) ? target.value : target;
    }
}

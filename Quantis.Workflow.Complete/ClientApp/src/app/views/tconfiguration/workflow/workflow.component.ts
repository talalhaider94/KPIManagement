import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { DataTableDirective } from 'angular-datatables';
import { ApiService } from '../../../_services/api.service';
import { Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import * as moment from 'moment';
import { ModalDirective } from 'ngx-bootstrap/modal';
//import { clearTimeout } from 'timers';

declare var $;
var $this;

@Component({
    templateUrl: './workflow.component.html'
})

export class WorkflowComponent implements OnInit {
    @ViewChild('addConfigModal') public addConfigModal: ModalDirective;
    @ViewChild('configModal') public configModal: ModalDirective;
    @ViewChild('ConfigurationTable') block: ElementRef;
    // @ViewChild('searchCol1') searchCol1: ElementRef;
    @ViewChild(DataTableDirective) private datatableElement: DataTableDirective;
    key: any = '';
    value: any = '';
    owner: any = '';
    isenable: boolean = false;
    iseditable: boolean = true;
    description: any = '';

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
    valuesCheck = {
        day_cutoff_value: null,
        day_notify_value: null,
        day_workflow_value: null,
        tempModal: null,
    }

    modalData = {
        contractId: 0,
        contractparty: '',
        contract: '',
        cutoff: '',
        workflowday: '',
        value: ''
    };

    dtTrigger: Subject<any> = new Subject();
    ConfigTableBodyData: any = [];

    constructor(
        private apiService: ApiService,
        private toastr: ToastrService,
    ) {
        $this = this;
    }

    ngOnInit() {
        this.apiService.getSeconds().subscribe((data: any) => {
            var secondsValue = data + '000';
            var seconds = parseInt(secondsValue);
            console.log("Auto Refresh Seconds: ", seconds);

            setInterval(() => {
                this.getCOnfigurations();
            }, seconds);
        });
    }

    populateModalData(data) {
        this.modalData.contractId = data.contractid;
        this.modalData.contractparty = data.contractpartyname;
        this.modalData.contract = data.contractname;
        this.modalData.cutoff = data.daycuttoff;
        this.modalData.workflowday = data.dayworkflow;
        this.showConfigModal();
    }
    timer = null;

    changeModal(value) {
        this.modalData.value = value;
        clearTimeout(this.timer);
        this.timer = setTimeout(() => {
            if (value < 0 || value > 28) {
                this.toastr.error('Il valore deve essere compreso tra 0 e 28', 'Error');
            }
        }, 500) //time to wait in ms before do the check
    }

    updateConfig(row) {
        if ((row.cutoff < 0 || row.cutoff > 28) || (row.workflowday < 0 || row.workflowday > 28)) {
            this.toastr.error('Il valore deve essere compreso tra 0 e 28', 'Error');
        } else {
            console.log(row.contractId,row.cutoff,row.workflowday);
            this.toastr.info('Valore in aggiornamento..', 'Info');
            this.apiService.AssignCuttoffWorkflowDayByContractId(row.contractId,row.cutoff,row.workflowday).subscribe(data => {
                this.getCOnfigurations(); 
                this.toastr.success('Valore Aggiornato', 'Success');
                this.hideConfigModal();
            }, error => {
                this.toastr.error('Errore durante update.', 'Error');
                this.hideConfigModal();
            });
        }
    }

    // tslint:disable-next-line:use-life-cycle-interface
    ngAfterViewInit() {
        this.dtTrigger.next();

        this.setUpDataTableDependencies();
        this.getCOnfigurations();
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

    // getConfigTableRef(datatableElement: DataTableDirective): any {
    //   return datatableElement.dtInstance;
    //   // .then((dtInstance: DataTables.Api) => {
    //   //     console.log(dtInstance);
    //   // });
    // }

    setUpDataTableDependencies() {
    }

    strip_tags(html) {
        var tmp = document.createElement("div");
        tmp.innerHTML = html;
        return tmp.textContent || tmp.innerText;
    }

    getCOnfigurations() {
        this.apiService.GetAllContractPartiesContracts().subscribe((data: any) => {
            this.ConfigTableBodyData = data;
            console.log("GetAllContractPartiesContracts -> ", data);
            this.rerender();
        }); 
        //     this.ConfigTableBodyData = data;
        //     let valuesCheck = { day_cutoff_value: null, day_notify_value: null, day_workflow_value: null, tempModal: null };
        //     data.forEach(function (config) {
        //         if (config.key == "day_cutoff") {
        //             valuesCheck.day_cutoff_value = config.value;
        //         }
        //         if (config.key == "day_notify") {
        //             valuesCheck.day_notify_value = config.value;
        //         }
        //         if (config.key == "day_workflow") {
        //             valuesCheck.day_workflow_value = config.value;
        //         }
        //     }
        //     );
        //     this.valuesCheck = valuesCheck;
        //     console.log('GetAllContractPartiesContracts ', data);
        //     this.rerender();
        // });
    }

    showConfigModal() {
        this.configModal.show();
    }

    hideConfigModal() {
        this.configModal.hide();
    }

    showAddConfigModal() {
        this.addConfigModal.show();
    }

    hideAddConfigModal() {
        this.addConfigModal.hide();
    }
}

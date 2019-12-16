import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { DataTableDirective } from 'angular-datatables';
import { ApiService } from '../../_services/api.service';
import { Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import * as moment from 'moment';
import { ModalDirective } from 'ngx-bootstrap/modal';
//import { clearTimeout } from 'timers';

declare var $;
var $this;

@Component({
    templateUrl: './tconfiguration.component.html'
})

export class TConfigurationComponent implements OnInit {
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
        key: '',
        value: '',
        owner: '',
        isenable: true,
        iseditable: true,
        description: '',
    };

    addData = {
        key: '',
        value: '',
        owner: '',
        isenable: false,
        iseditable: true,
        description: ''
    };

    dtTrigger: Subject<any> = new Subject();
    ConfigTableBodyData: any = [
        {
            key: 'key',
            value: 'value',
            owner: 'owner',
            isenable: true,
            iseditable: true,
            description: 'description',
        }
    ]

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
        this.modalData.key = data.key;
        this.modalData.owner = data.owner;
        this.modalData.value = data.value;
        this.modalData.isenable = data.isenable;
        this.modalData.iseditable = data.iseditable;
        this.modalData.description = data.description;
        this.valuesCheck.tempModal = data.key;
        this.showConfigModal();
    }
    timer = null;

    changeModal(value) {
        this.modalData.value = value;
        clearTimeout(this.timer);
        this.timer = setTimeout(() => {
            console.log(value, this.valuesCheck.tempModal, this.valuesCheck.day_cutoff_value);
            if (this.valuesCheck.tempModal == "day_workflow") {
                if (parseInt(value) < parseInt(this.valuesCheck.day_cutoff_value)) { this.toastr.warning('La data del Workflow è minore del Cutoff', 'Attenzione'); }
                let today = parseInt(moment().format('DD'));
                console.log(today)
                if (value > today && parseInt(this.valuesCheck.day_workflow_value) < today) { this.toastr.error('Il Workflow ripartirà nel mese corrente per eventuali ticket non aperti', 'Attenzione'); }
            }
            if (this.valuesCheck.tempModal == "day_cutoff") {
                if (parseInt(value) > parseInt(this.valuesCheck.day_workflow_value)) { this.toastr.warning('La data del Cutoff è maggiore del Workflow', 'Attenzione'); }
            }
        }, 500) //time to wait in ms before do the check
    }

    addConfig() {
        this.addData.key = this.key;
        this.addData.owner = this.owner;
        this.addData.value = this.value;
        this.addData.isenable = this.isenable;
        this.addData.iseditable = this.iseditable;
        this.addData.description = this.description;

        this.toastr.info('Valore in aggiornamento..', 'Info');
        this.apiService.addConfig(this.addData).subscribe(data => {
            this.getCOnfigurations(); // this should refresh the main table on page
            this.toastr.success('Valore Aggiornato', 'Success');
            this.hideAddConfigModal();
            //$('#addConfigModal').modal('toggle').hide();
        }, error => {
            this.toastr.error('Errore durante add.', 'Error');
            this.hideAddConfigModal();
            //$('#addConfigModal').modal('toggle').hide();
        });
    }

    updateConfig() {
        var value = +this.modalData.value;
        console.log("value ->", value);
        if ((this.modalData.key == 'day_cutoff' || this.modalData.key == 'day_workflow') && (value < 0 || value > 31)) {
            this.toastr.error('Il valore deve essere compreso tra 0 e 31', 'Error');
        } else {
            this.toastr.info('Valore in aggiornamento..', 'Info');
            this.apiService.updateConfig(this.modalData).subscribe(data => {
                this.getCOnfigurations(); // this should refresh the main table on page
                this.toastr.success('Valore Aggiornato', 'Success');
                this.hideConfigModal();
                //$('#configModal').modal('toggle').hide();
            }, error => {
                this.toastr.error('Errore durante update.', 'Error');
                this.hideConfigModal();
                //$('#configModal').modal('toggle').hide();
            });
        }
    }

    // tslint:disable-next-line:use-life-cycle-interface
    ngAfterViewInit() {
        this.dtTrigger.next();

        this.setUpDataTableDependencies();
        this.getCOnfigurations();

        /*this.apiService.getConfigurations().subscribe((data:any)=>{
          this.ConfigTableBodyData = data;
          this.rerender();
        });*/
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
        // $(this.searchCol1.nativeElement).on( 'keyup', function () {
        //   $this.datatableElement.dtInstance.then((datatable_Ref: DataTables.Api) => {
        //   datatable_Ref
        //     .columns( 0 )
        //     .search( this.value )
        //     .draw();
        // });
        // });
    }

    strip_tags(html) {
        var tmp = document.createElement("div");
        tmp.innerHTML = html;
        return tmp.textContent || tmp.innerText;
    }

    getCOnfigurations() {
        this.apiService.getConfigurations().subscribe((data) => {
            this.ConfigTableBodyData = data;
            let valuesCheck = { day_cutoff_value: null, day_notify_value: null, day_workflow_value: null, tempModal: null };
            data.forEach(function (config) {
                if (config.key == "day_cutoff") {
                    valuesCheck.day_cutoff_value = config.value;
                }
                if (config.key == "day_notify") {
                    valuesCheck.day_notify_value = config.value;
                }
                if (config.key == "day_workflow") {
                    valuesCheck.day_workflow_value = config.value;
                }
            }
            );
            this.valuesCheck = valuesCheck;
            console.log('Configs ', data);
            this.rerender();
        });
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

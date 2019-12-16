import { Component, OnInit, ViewChild, ViewChildren, ElementRef, QueryList } from '@angular/core';
import { DataTableDirective } from 'angular-datatables';
import { ApiService } from '../../../_services/api.service';
import { Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ModalDirective } from 'ngx-bootstrap/modal';
import * as moment from 'moment';

declare var $;
var $this;

@Component({
    templateUrl: './statoDay.component.html',
    styleUrls: ['./statoDay.component.scss']
})

export class StatoDayComponent implements OnInit {
    @ViewChild('addConfigModal') public addConfigModal: ModalDirective;
    @ViewChild('specialModal') public specialModal: ModalDirective;
    @ViewChild('ConfigurationTable') block: ElementRef;
    @ViewChildren(DataTableDirective)
    datatableElements: QueryList<DataTableDirective>;
    dtOptions: DataTables.Settings = {};
    dtTrigger = new Subject();
    dtOptions2: DataTables.Settings = {};
    dtTrigger2 = new Subject();


    loading: boolean = true;
    title='';
    isticketstobeopenedtoday=false;
    isticketsopenedtoday=false;

    ticketstobeopenedtoday: any = [];
    ticketsopenedtoday: any = [];
    
    modalData = {
        key: 0,
        value: ''
    };

    addOrganizationData = {
        Key: 0,
        Value: ''
    };

    modalSpecialData = {
        key: 0,
        value: ''
    };

    addSpecialData = {
        Key: 0,
        Value: '',
        Note: '',
    };

    modalTitle='';
    buttonText='';
    anni = [];

    organizationsData: any = []
    specialReportsData: any = []

    constructor(
        private apiService: ApiService,
        private toastr: ToastrService,
    ) {
        $this = this;
    }
    value;
    specialKey;
    specialValue;
    specialNote;
    isEdit=0;
    isSpecialEdit=0;
    month;
    year;

    ngOnInit() {
        // this.getAnno();
        // this.month = moment().subtract(1, 'month').format('MM');
        // this.year = moment().subtract(1, 'month').format('YYYY');

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
            },
            destroy:true
        };
        this.dtOptions2 = {
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
            },
            destroy:true
        };
    }

    ngAfterViewInit() {
        this.dtTrigger.next();
        this.dtTrigger2.next();
        
        this.GetAllOrganizationUnits();
    }

    ngOnDestroy(): void {
        this.dtTrigger.unsubscribe();
        this.dtTrigger2.unsubscribe();
    }

    rerender(): void {
        this.datatableElements.forEach((dtElement: DataTableDirective) => {
            dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
              dtInstance.destroy();
              this.dtTrigger.next();
              this.dtTrigger2.next();
            });
        });
        this.setUpDataTableDependencies();
    }

    setUpDataTableDependencies() {
    }

    // getAnno() {
    //     for (var i = 2016; i <= +(moment().add(7,'months').format('YYYY')); i++) {
    //         this.anni.push(i);
    //     }
    //     return this.anni;
    //     console.log("aaaa", this.anni);
    // }

    strip_tags(html) {
        var tmp = document.createElement("div");
        tmp.innerHTML = html;
        return tmp.textContent || tmp.innerText;
    }

    // populateDateFilter(){
    //     this.loading=true;
    //     this.GetAllOrganizationUnits();
    // }

    GetAllOrganizationUnits() {
        this.apiService.GetMonitoringDay(this.month,this.year).subscribe((data) => {
            this.organizationsData = data;
            console.log('Day Monitoring Data -> ',data);
            this.rerender();
            this.loading=false;
        });
    }

    onCancel(dismissMethod: string): void {
        console.log('Cancel ', dismissMethod);
    }

    noofticketstobeopenedtoday(data){
        this.isticketstobeopenedtoday=true;
        this.isticketsopenedtoday=false;
        this.title='Tickets to be opened today';
        this.ticketstobeopenedtoday = data;
        this.rerender();
    }

    noofticketsopenedtoday(data){
        this.isticketsopenedtoday=true;
        this.isticketstobeopenedtoday=false;
        this.title='Tickets opened today';
        this.ticketsopenedtoday = data;
        this.rerender();
    }

    showSpecialModal() {
        this.specialModal.show();
    }

    hideSpecialModal() {
        this.specialModal.hide();
    }
}

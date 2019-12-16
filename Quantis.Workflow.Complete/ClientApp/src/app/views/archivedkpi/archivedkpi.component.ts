import { Component, OnInit, ViewChild, ElementRef, Pipe } from '@angular/core';
import { DataTableDirective } from 'angular-datatables';
import { ApiService } from '../../_services/api.service';
import { Subject, from, BehaviorSubject, interval } from 'rxjs';
import { DatePipe, formatDate, getLocaleDateFormat } from '@angular/common'
import { conditionallyCreateMapObjectLiteral } from '@angular/compiler/src/render3/view/util';
import * as moment from 'moment';
import { first } from 'rxjs/operators';
import { Key } from 'protractor';
import { MatSort } from '@angular/material';
import { OrderPipe } from 'ngx-order-pipe'
import * as XLSX from 'xlsx';
import * as FileSaver from 'file-saver';
import { Variable } from '@angular/compiler/src/render3/r3_ast';
import { ToastrService } from 'ngx-toastr';
import { forEach } from '@angular/router/src/utils/collection';
import { ModalDirective } from 'ngx-bootstrap/modal';

declare var $;
var $this;

var nome = '';
const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const EXCEL_EXTENSION = '.csv';
@Component({
    templateUrl: './archivedkpi.component.html'
})
export class ArchivedKpiComponent implements OnInit {
    @ViewChild('archivedKpiModal') public archivedKpiModal: ModalDirective;
    @ViewChild('datiModal') public datiModal: ModalDirective;
  @ViewChild('datiTempoModal') public datiTempoModal: ModalDirective;
  @ViewChild('multiCollapseExample1') public multiCollapseExample1: ModalDirective;
  @ViewChild('multiCollapseExample2') public multiCollapseExample2: ModalDirective;
    @ViewChild('ArchivedkpiTable') block: ElementRef;
    @ViewChild('searchCol1') searchCol1: ElementRef;
    @ViewChild(DataTableDirective) private datatableElement: DataTableDirective;
    @ViewChild('table') table: ElementRef;

    public filter: string;
    public comparator: any;
    public p: any;

    loading: boolean = true;
    loadingModal1: boolean = false;
    loadingModalDati: boolean = false;

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

    modalData = {
        id_kpi: '',
        name_kpi: '',
        interval_kpi: null,
        value_kpi: '',
        ticket_id: '',
        close_timestamp_ticket: '',
        archived: '',
        contract_name: ''
    };

    kpisData = [];
    EventResourceNames = [];
    eventTypes: any = {};
    resources: any = {};
    datiGrezzi = [];
    countCampiData = [];
    id_kpi_temp = '';
    intervalloPeriodo = '';

    fitroDataById: any = [
        {
            event_type_id: '   ',
            resource_id: '',
            time_stamp: ' ',
            raw_data_id: '',
            create_date: ' ',
            data: this.datiGrezzi,
            modify_date: '',
            reader_id: '',
            event_source_type_id: ' ',
            event_state_id: ' ',
            partner_raw_data_id: ' ',
        }
    ]

    dtTrigger: Subject<any> = new Subject();

    ArchivedKpiBodyData: any = [
        {
            customer_name: '',
            contract_name: ' ',
            id_kpi: '',
            name_kpi: '',
            interval_kpi: '',
            value_kpi: '',
            ticket_id: '',
            close_timestamp_ticket: '',
            archived: '',
            global_rule_id: '',
            progressive: false
        }
    ]

    annoIntervallo: any;
    monthVar: any;
    yearVar: any;
    meseInput: any;
    meseSelezionato: any;
    annoSelezionato: any;

    id: any;
    sortedCollection: any[];
    constructor(private apiService: ApiService, private orderPipe: OrderPipe) {
        $this = this;
        this.sortedCollection = orderPipe.transform(this.fitroDataById, 'info.name');
        console.log(this.sortedCollection);
    }

    createLoop(length = this.countCampiData) {
        console.log(length)
        return new Array(length);
    }
    ngOnInit() {
        this.getAnno();
        this.meseSelezionato = moment().subtract(1, 'month').format('MM');
        this.monthVar = moment().subtract(1, 'month').format('MM');
        this.yearVar = moment().subtract(1, 'month').format('YYYY');
        //this.populateDateFilter();
        this.getEventResourceNames();
    }

    getEventResourceNames() {
        this.apiService.getEventResourceNames().subscribe((dati: any) => {
            //this.EventResourceNames = dati;
            var eventTypes = {};
            var resources = {};
            dati.forEach(function (item) {
                if (item.type === 'EVENT_TYPE') {
                    eventTypes[item.id] = item.name + ' [' + item.id + ']';
                } else {
                    resources[item.id] = item.name + ' [' + item.id + ']';
                }
            });
            this.eventTypes = eventTypes;
            this.resources = resources;
        })
    }

    populateModalData(data) {
        this.reset();
        this.loadingModal1 = true;

        this.apiService.getArchivedKpiById(data.global_rule_id).subscribe((kpis: any) => {
            this.kpisData = kpis;
            this.loadingModal1 = false;

            console.log('pop', this.kpisData);
        });
        this.showArchivedKpiModal();
    }

    populateDateFilter() {
        this.loading = true;
        this.apiService.getArchivedKpis(this.monthVar, this.yearVar).subscribe((data: any) => {
            this.ArchivedKpiBodyData = data;
            //console.log("kpi1",data);
            // this.getNumeroContratti();

            this.rerender();
            this.numeroContratti();
            this.addChildren();
            this.loading = false;
        });
        //  console.log("prova", this.ArchivedKpiBodyData)
    }

    ngAfterViewInit() {
        this.dtTrigger.next();
        //this.getKpis1();

        this.apiService.getDataKpis(this.monthVar, this.yearVar).subscribe((data: any) => {
            this.ArchivedKpiBodyData = data;

            this.rerender();
            this.numeroContratti();
            this.addChildren();
            this.loading = false;
        });
    }

    //ngOnDestroy(): void {
    //Do not forget to unsubscribe the event
    // this.dtTrigger.unsubscribe();
    //}

    rerender(): void {
        this.datatableElement.dtInstance.then((dtInstance: DataTables.Api) => {
            // Destroy the table first
            dtInstance.destroy();
            // Call the dtTrigger to rerender again
            this.dtTrigger.next();
            // this.setUpDataTableDependencies();
        });
    }

    // getKpiTableRef(datatableElement: DataTableDirective): any {
    //   return datatableElement.dtInstance;
    // }

    strip_tags(html) {
        var tmp = document.createElement("div");
        tmp.innerHTML = html;
        return tmp.textContent || tmp.innerText;
    }

    /* getKpis() {
       this.apiService.getArchivedKpis(this.monthVar, this.yearVar).subscribe((data) =>{
         this.ArchivedKpiBodyData = data;
         console.log("kpi",data);
         Object.keys(this.fitroDataById).forEach( key => {
           this.fitroDataById[key].data = JSON.parse(this.fitroDataById[key].data);
         })
       });
     }*/

    eventTypeArray = [];

    getdati(id_kpi, tracking_period = '', interval_kpi = '', progressive = false, month = this.monthVar, year = this.yearVar) {
        this.clear();
        this.meseSelezionato = month;
        this.id_kpi_temp = id_kpi;
        if (tracking_period.length > 0 && interval_kpi.length > 0) {
            this.arrayTempo(tracking_period, interval_kpi, progressive);
            month = moment(interval_kpi).format('MM');
            year = moment(interval_kpi).format('YYYY');
            this.meseSelezionato = month;
        }
        this.loadingModalDati = true;
        this.apiService.getKpiArchivedRawData(id_kpi, month, year).subscribe((dati: any) => {
            this.fitroDataById = dati;
            console.log(dati);
            Object.keys(this.fitroDataById).forEach(key => {
                this.fitroDataById[key].data = JSON.parse(this.fitroDataById[key].data);
                switch (this.fitroDataById[key].event_state_id) {
                    case 1:
                        this.fitroDataById[key].event_state_id = "Originale";
                        break;
                    case 2:
                        this.fitroDataById[key].event_state_id = "Sovrascritto";
                        break;
                    case 3:
                        this.fitroDataById[key].event_state_id = "Eliminato";
                        break;
                    case 4:
                        this.fitroDataById[key].event_state_id = "Correzione";
                        break;
                    case 5:
                        this.fitroDataById[key].event_state_id = "Correzione eliminata";
                        break;
                    case 6:
                        this.fitroDataById[key].event_state_id = "Business";
                        break;
                    default:
                        this.fitroDataById[key].event_state_id = this.fitroDataById[key].event_state_id;
                        break;
                }
                this.fitroDataById[key].event_type_id = this.eventTypes[this.fitroDataById[key].event_type_id] ? this.eventTypes[this.fitroDataById[key].event_type_id] : this.fitroDataById[key].event_type_id;
                this.fitroDataById[key].resource_id = this.resources[this.fitroDataById[key].resource_id] ? this.resources[this.fitroDataById[key].resource_id] : this.fitroDataById[key].resource_id;
                this.fitroDataById[key].modify_date = moment(this.fitroDataById[key].modify_date).format('DD/MM/YYYY HH:mm:ss');
                this.fitroDataById[key].create_date = moment(this.fitroDataById[key].create_date).format('DD/MM/YYYY HH:mm:ss');
                this.fitroDataById[key].time_stamp = moment(this.fitroDataById[key].time_stamp).format('DD/MM/YYYY HH:mm:ss');
            })
            this.getCountCampiData();
            this.numeroEventi();
            let max = this.countCampiData.length;

            Object.keys(this.fitroDataById).forEach(key => {
                let temp = Object.keys(this.fitroDataById[key].data).length;
                if (temp < max) {
                    for (let i = 0; i < (max - temp); i++) {
                        this.fitroDataById[key].data['empty#' + i] = '##empty##';
                    }
                }
            })

            //****console.log("array tempo",this.arrayPeriodo);
            console.log('dati', dati);
            //****console.log('key', this.fitroDataById);
            /**** this.getCountCampiData();
             this.numeroEventi();****/

            //****console.log(this.eventTypeArray);

            /*Object.keys(this.eventTypeArray).forEach( e=> {
              console.log(e + '#' + this.eventTypeArray[e]);
            })*/
            this.loadingModalDati = false;
        },
            error => {
                this.loadingModalDati = false;
            });

        this.showDatiModal();
    }

    getdati1(id_kpi, month = this.meseSelezionato, year = this.yearVar) {
        this.clear();

        this.id_kpi_temp = id_kpi;
        this.loadingModalDati = true;

        this.apiService.getKpiArchivedRawData(id_kpi, month, year).subscribe((dati: any) => {
            this.fitroDataById = dati;
            console.log(dati);
            Object.keys(this.fitroDataById).forEach(key => {
                this.fitroDataById[key].data = JSON.parse(this.fitroDataById[key].data);
                switch (this.fitroDataById[key].event_state_id) {
                    case 1:
                        this.fitroDataById[key].event_state_id = "Originale";
                        break;
                    case 2:
                        this.fitroDataById[key].event_state_id = "Sovrascritto";
                        break;
                    case 3:
                        this.fitroDataById[key].event_state_id = "Eliminato";
                        break;
                    case 4:
                        this.fitroDataById[key].event_state_id = "Correzione";
                        break;
                    case 5:
                        this.fitroDataById[key].event_state_id = "Correzione eliminata";
                        break;
                    case 6:
                        this.fitroDataById[key].event_state_id = "Business";
                        break;
                    default:
                        this.fitroDataById[key].event_state_id = this.fitroDataById[key].event_state_id;
                        break;
                }
                this.fitroDataById[key].modify_date = moment(this.fitroDataById[key].modify_date).format('DD/MM/YYYY HH:mm:ss');
                this.fitroDataById[key].create_date = moment(this.fitroDataById[key].create_date).format('DD/MM/YYYY HH:mm:ss');
                this.fitroDataById[key].time_stamp = moment(this.fitroDataById[key].time_stamp).format('DD/MM/YYYY HH:mm:ss');
            })
            this.getCountCampiData();
            this.numeroEventi();
            let max = this.countCampiData.length;

            Object.keys(this.fitroDataById).forEach(key => {
                let temp = Object.keys(this.fitroDataById[key].data).length;
                if (temp < max) {
                    for (let i = 0; i < (max - temp); i++) {
                        this.fitroDataById[key].data['empty#' + i] = '##empty##';
                    }
                }
            },
                error => {
                    this.loadingModalDati = false;
                }
            )

            //****console.log("array tempo",this.arrayPeriodo);
            console.log('dati', dati);
            //****console.log('key', this.fitroDataById);
            /**** this.getCountCampiData();
             this.numeroEventi();****/

            //****console.log(this.eventTypeArray);

            /*Object.keys(this.eventTypeArray).forEach( e=> {
              console.log(e + '#' + this.eventTypeArray[e]);
            })*/
            this.loadingModalDati = false;
        });
    }

    arrayContratti = [];

    numeroContratti() {
        this.arrayContratti = [];
        this.ArchivedKpiBodyData.forEach(e => {
            var count = this.ArchivedKpiBodyData.reduce((acc, cur) => cur.contract_name === e.contract_name ? ++acc : acc, 0);
            // console.log('type:' + e.contract_name + ' # count:' +count);
            if (e.contract_name != null) this.arrayContratti[e.contract_name] = count;
            // console.log('prova',this.arrayContratti);
        })
    }

    numeroContrattoKpi = [];
    arraykpi = [];
    res: any = {};

    addChildren() {
        this.res = {};
        this.ArchivedKpiBodyData.forEach((y: any) => {
            if (this.res[y.contract_name]) {
                this.res[y.contract_name].push(y.name_kpi)
            }
            else {
                this.res[y.contract_name] = [];
                this.res[y.contract_name].push(y.name_kpi)
            }
        });

        console.log('contrattoKPI', this.res);
    }

    numeroEventi() {
        this.eventTypeArray = [];
        this.fitroDataById.forEach(e => {
            var count = this.fitroDataById.reduce((acc, cur) => cur.event_type_id === e.event_type_id ? ++acc : acc, 0);
            //console.log('type:' + e.event_type_id + ' # count:' +count);
            if (e.event_type_id > 0) this.eventTypeArray[e.event_type_id] = count;
            // console.log('prova',this.eventTypeArray);
        })
    }

    /*spit(){
      const [first, second] = "10AM-4PM".split('-');
      console.log(first);
      console.log(second);
    }*/

    getDatiSecondPop(id_kpi, interval, tracking_period) {
        this.id_kpi_temp = id_kpi;
        this.meseSelezionato = moment(interval).format('MM');
        this.annoSelezionato = moment(interval).format('YYYY');
        // this.getdati(id_kpi,mese,anno,tracking_period);
        this.getdati(id_kpi, tracking_period, interval, this.meseSelezionato, this.annoSelezionato);

        this.showDatiTempoModal();

        /*let resources = data["ArchivedKpiBodyData"];
        let resource = resources["interval_kpi"];
        console.log('stampa',resource);*/
    }

    getDatiSecondPop2(id_kpi, meseA, anniA) {
        this.id_kpi_temp = id_kpi;
        this.meseSelezionato = meseA;
        this.annoSelezionato = anniA;
        // this.getdati(id_kpi,mese,anno,tracking_period);
        this.getdati1(id_kpi, this.meseSelezionato, this.annoSelezionato);

        /*let resources = data["ArchivedKpiBodyData"];
        let resource = resources["interval_kpi"];
        console.log('stampa',resource);*/
    }

    getCountCampiData() {
        let maxLength = 0;
        this.fitroDataById.forEach(f => {
            //let data = JSON.parse(f.data);
            if (Object.keys(f.data).length > maxLength) {
                maxLength = Object.keys(f.data).length;
            }
        });
        this.countCampiData = [];
        for (let i = 1; i <= maxLength; i++) {
            this.countCampiData.push(i);
        }
    }

    /*getdati(id_kpi,monthVar,yearVar){
      this.apiService.getDateKpisById(id_kpi,monthVar,yearVar).subscribe(data => {
        let res = data[0]["fitroDataById"];
        let datiGrezzi = res['data'];
        this.datiGrezzi=.datiGrezzi
        // this.dataChange.next(data)
         console.log(this.datiGrezzi);
        });
    }*/

    anni = [];
    //+(moment().add('months', 6).format('YYYY'))
    getAnno() {
        for (var i = 2016; i <= +(moment().add('months', 7).format('YYYY')); i++) {
            this.anni.push(i);
        }
        return this.anni;
        console.log("aaaa", this.anni);
    }
    order: string = 'info.name';
    reverse: boolean = false;

    setOrder(value: string) {
        if (this.order === value) {
            this.reverse = !this.reverse;
        }

        this.order = value;
    }

    getNumeroKPI() {
        return this.ArchivedKpiBodyData.length;
    }

    /* getNumeroContratti(){
       var Length = 0;
       this.ArchivedKpiBodyData.forEach( f => {
         //let data = JSON.parse(f.data);
         if(Object.keys(f.contract_name).length){
           Length = Object.keys(f.contract_name).length;
           return Length;
         }
       });
     }
   */

    /*public exportAsExcelFile(json: any[], excelFileName: string): void {
      const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(json);
      console.log('worksheet',worksheet);
      const workbook: XLSX.WorkBook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
      const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
      //const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'buffer' });
      this.saveAsExcelFile(excelBuffer, excelFileName);
    }

    private saveAsExcelFile(buffer: any, fileName: string): void {
      const data: Blob = new Blob([buffer], {
        type: EXCEL_TYPE
      });
      FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
    }

    exportAsXLSX():void {
      this.exportAsExcelFile(this.fitroDataById, 'sample');
    }*/

    /*fireEvent()
    {
    /*const ws: XLSX.WorkSheet=XLSX.utils.table_to_sheet(this.table.nativeElement);
      const wb: XLSX.WorkBook = XLSX.utils.book_new();
     XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');
        XLSX.writeFile(wb, 'Export.csv');*/

    //   const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.fitroDataById);

    /* generate workbook and add the worksheet */
    //   const wb: XLSX.WorkBook = XLSX.utils.book_new();
    //    XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');

    /* save to file */
    //    XLSX.writeFile(wb, 'SheetJS.csv');

    //}
    fireEvent() {
        const ws: XLSX.WorkSheet = XLSX.utils.table_to_sheet(this.table.nativeElement);
        const wb: XLSX.WorkBook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');
        XLSX.writeFile(wb, 'Export.csv');
    }

    public exportAsExcelFile(json: any[], excelFileName: string): void {
        const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(json);
        console.log('worksheet', worksheet);
        const workbook: XLSX.WorkBook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
        const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
        //const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'buffer' });
        this.saveAsExcelFile(excelBuffer, excelFileName);
    }

    private saveAsExcelFile(buffer: any, fileName: string): void {
        const data: Blob = new Blob([buffer], {
            type: EXCEL_TYPE
        });
        FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
    }

    exportAsXLSX(): void {
        this.exportAsExcelFile(this.fitroDataById, 'sample');
    }

    /*exportExcel(data: any[]){
      const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.fitroDataById);
      const workbook: XLSX.WorkBook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
      XLSX.writeFile(workbook, 'my_file.csv', { bookType: 'csv', type: 'buffer' });
    }*/

    clear() {
        this.filter = '';
        this.fitroDataById = [];
        this.p = 1;

        //this.hideDatiModal();
    }

    clear1() {
        this.filter = '';
        this.fitroDataById = [];
        this.p = 1;

       // this.hideDatiTempoModal();
    }
    clearAndClose() {
      this.filter = '';
      this.fitroDataById = [];
      this.p = 1;
      this.hideDatiModal();
    }

    clear1AndClose() {
      this.filter = '';
      this.fitroDataById = [];
      this.p = 1;
      this.hideDatiTempoModal();
    }
    reset() {
        this.kpisData = [];

        this.hideArchivedKpiModal();
    }

    arrayPeriodo = [];
    arrayTempo(tracking_period, interval_kpi, progressive = true) {
        //debugger;
        console.log(tracking_period);
        this.arrayPeriodo = [];
        if (tracking_period === 'TRIMESTRALE') {
            var mese1 = moment(interval_kpi).format('MM');
            var mese2 = moment(interval_kpi).subtract(1, 'month').format('MM');
            var mese3 = moment(interval_kpi).subtract(2, 'month').format('MM');
            this.arrayPeriodo.push(mese1, mese2, mese3);

            console.log("trimestrale", this.arrayPeriodo);
        }
        else if (tracking_period === 'MENSILE') {
            var mese4 = moment(interval_kpi).format('MM');
            this.arrayPeriodo.push(mese4);
            console.log("mensile", this.arrayPeriodo);
        }
        else if (tracking_period === 'SEMESTRALE') {
            let mese1 = moment(interval_kpi).format('MM');
            let mese2 = moment(interval_kpi).subtract(1, 'month').format('MM');
            let mese3 = moment(interval_kpi).subtract(2, 'month').format('MM');
            let mese4 = moment(interval_kpi).subtract(3, 'month').format('MM');
            let mese5 = moment(interval_kpi).subtract(4, 'month').format('MM');
            let mese6 = moment(interval_kpi).subtract(5, 'month').format('MM');

            this.arrayPeriodo.push(mese1, mese2, mese3, mese4, mese5, mese6);
            console.log("semestrale", this.arrayPeriodo);
        }
        else if (tracking_period === 'ANNUALE') {
            let mese1 = moment(interval_kpi).format('MM');
            let mese2 = moment(interval_kpi).subtract(1, 'month').format('MM');
            let mese3 = moment(interval_kpi).subtract(2, 'month').format('MM');
            let mese4 = moment(interval_kpi).subtract(3, 'month').format('MM');
            let mese5 = moment(interval_kpi).subtract(4, 'month').format('MM');
            let mese6 = moment(interval_kpi).subtract(5, 'month').format('MM');
            let mese7 = moment(interval_kpi).subtract(6, 'month').format('MM');
            let mese8 = moment(interval_kpi).subtract(7, 'month').format('MM');
            let mese9 = moment(interval_kpi).subtract(8, 'month').format('MM');
            let mese10 = moment(interval_kpi).subtract(9, 'month').format('MM');
            let mese11 = moment(interval_kpi).subtract(10, 'month').format('MM');
            let mese12 = moment(interval_kpi).subtract(11, 'month').format('MM');

            this.arrayPeriodo.push(mese1, mese2, mese3, mese4, mese5, mese6, mese7, mese8, mese9, mese10, mese11, mese12);
            console.log("semestrale", this.arrayPeriodo);
      }
      if (progressive) {
        let mese1 = moment(interval_kpi).format('MM');
        //this.arrayPeriodo.push(mese1);
        for (let i = 1; i < parseInt(mese1); i++) {
          let mese = moment(interval_kpi).subtract(i, 'month').format('MM');
          this.arrayPeriodo.push(mese);
        }
        //this.arrayPeriodo.push(mese1, mese2, mese3, mese4, mese5, mese6, mese7, mese8, mese9, mese10, mese11, mese12);
        
      }
    }

    showArchivedKpiModal() {
        this.archivedKpiModal.show();
    }

    hideArchivedKpiModal() {
        this.archivedKpiModal.hide();
    }

    showDatiModal() {
        this.datiModal.show();
    }

    hideDatiModal() {
        this.datiModal.hide();
    }

    showDatiTempoModal() {
        this.datiTempoModal.show();
    }

    hideDatiTempoModal() {
        this.datiTempoModal.hide();
    }

  showCollapse() {
    this.multiCollapseExample1.show();
    this.multiCollapseExample2.show();
  }
  hideCollapse() {

  }
}

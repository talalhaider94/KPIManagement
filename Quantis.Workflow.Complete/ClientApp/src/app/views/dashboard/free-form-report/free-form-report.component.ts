import { Component, OnInit, ViewChild, ViewChildren, OnDestroy, QueryList, ElementRef } from '@angular/core';
import { FreeFormReportService } from '../../../_services';
import { forkJoin } from 'rxjs';
import { Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { DataTableDirective } from 'angular-datatables';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import * as XLSX from 'xlsx';
import { fireEvent } from 'highcharts';
// import {ExcelService} from '../../../_services/excel.service';

let $this;
@Component({
  selector: 'app-free-form-report',
  templateUrl: './free-form-report.component.html',
  styleUrls: ['./free-form-report.component.scss']
})

export class FreeFormReportComponent implements OnInit {
  @ViewChild('configModal') public configModal: ModalDirective;
  @ViewChild('viewAssignedModal') public viewAssignedModal: ModalDirective;
  @ViewChild('executeModal') public executeModal: ModalDirective;
  @ViewChild('parametersModal') public parametersModal: ModalDirective;
  @ViewChild('btnExporta') btnExporta: ElementRef;
  @ViewChild('queryTable') queryTable: ElementRef;
  assignedReportQueries: any = [];
  assignedQueriesBodyData: any = [];
  viewAssignedData: any = [];
  ownedReportQueries: any = [];
  reportQueryDetail: any = [];
  ownername;
  emptyMessage=null;
  isSpecialReport = 0
  client;
  contract;

  loading: boolean = true;
  loadingResult: boolean;
  formLoading: boolean = false;
  submitted: boolean = false;
  queryId=0;
  count=0;
  isSubmit=0;
  isReadonly=0;
  isDebug=0;
  debugResult;
  debugResultArray = [];
  debugCount=0;
  hideData=0;
  parameterCount=0;
  disableId=0;
  assignedUsers = [];
  isDisabled=0;
  isEnabled=0;
  params = {
    id: 0,
    ids: []
  }
  hideExport: boolean = true;
  debugQueryData: any = [];
  debugQueryValue: any = [];
  editQueryData = {
    id: 0,
    QueryName: '',
    QueryText: '',
    Parameters: [{
      key: '',
      value: ''
    }]
  }
  executeQueryData = {
    QueryText: '',
    Parameters: [{
      key: '',
      value: ''
    }]
  }

  modalTitle: string = 'Utenti assegnati';
  assigendModalTitle: string = 'Free Form Report Assegnati';
  ownedModalTitle: string = 'Propri Free Form Report';
  executeModalTitle: string = '';

  // @ViewChildren(DataTableDirective)
  // datatableElement: DataTableDirective;

  // @ViewChild(DataTableDirective)
  // datatableElement2: DataTableDirective;

  @ViewChildren(DataTableDirective)
  datatableElements: QueryList<DataTableDirective>;
  @ViewChild('addEditQueryReportModal')
  addEditQueryReportModal: ModalDirective;

  dtOptions: DataTables.Settings = {};
  dtTrigger = new Subject();
  dtOptions2: DataTables.Settings = {};
  dtTrigger2 = new Subject();
  dtOptions3: any = {};
  dtTrigger3 = new Subject();
  filters: any = {
    searchUsersText: ''
  }

  addEditQueryForm: FormGroup;
  Parameters: FormArray;

  constructor(
    private _freeFormReport: FreeFormReportService,
    private toastr: ToastrService,
    private formBuilder: FormBuilder,
    // private excelService:ExcelService
  ) {
    $this = this;
  }

  get f() { return this.addEditQueryForm.controls; }

  ngOnInit() {
    $(this.btnExporta.nativeElement).hide();
    this.getReportQueryDetailByID();
    this.isDisabled=0;
    this.isEnabled=0;
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
    this.dtOptions3 = {
      pagingType: 'full_numbers',
      pageLength: 10,
      dom: 'lBfrtip',
      buttons: [
          {
              extend: 'pdf',
              text: '<i class="fa fa-file"></i> Esporta PDF',
              titleAttr: 'Esporta PDF',
              className: 'btn btn-primary mb-3',
              orientation: 'landscape',
          }
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
      },
      destroy:true
    };
    this.addEditQueryForm = this.formBuilder.group({
      id: [0, Validators.required],
      QueryName: ['', Validators.required],
      QueryText: ['', Validators.required],
      Parameters: this.formBuilder.array([])
    });

    this.getReportsData();

    this.debugQueryData = [];
    this.debugQueryValue = [];
  }

  createParameters(): FormGroup {
    return this.formBuilder.group({
      key: '',
      value: ''
    });
  }

  addParameters(): void {
    this.Parameters = this.addEditQueryForm.get('Parameters') as FormArray;
    this.Parameters.push(this.createParameters());
    this.parameterCount=1;
  }

  deleteParameters(id: number) {
    if(this.parameterCount==0){
      this.Parameters = this.addEditQueryForm.get('Parameters') as FormArray;
    }
    console.log('remove index: ',id);
    this.Parameters.removeAt(id);
  }

  onKeydown(event) {
    if (event.keyCode === 32 ) {
      return false;
    }
  }

  omit_special_char(event){
    var k;
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 63 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57));
  }

  ngAfterViewInit() {
    this.dtTrigger.next();
    this.dtTrigger2.next();
    this.dtTrigger3.next();
  }

  // downloadPDF(){
  //   const doc = new jsPDF();
  //   doc.text('Some text here', 10,10);

  //   /*doc.autoTable({
  //     html: '#executedQueryResult',
  //     minCellWidth:10
  //   });*/

  //   doc.save('Test.pdf');
  // }

  getReportsData() {
    const $assignedReportQueries = this._freeFormReport.getAssignedReportQueries();
    const $ownedReportQueries = this._freeFormReport.getOwnedReportQueries();
    console.log($assignedReportQueries, $ownedReportQueries);
    
    forkJoin([$assignedReportQueries, $ownedReportQueries]).subscribe(result => {
      if (result) {
        const [assignedReportQueries, ownedReportQueries] = result;
        this.assignedReportQueries = assignedReportQueries;
        this.ownedReportQueries = ownedReportQueries;
        console.log('Queries -> ',this.assignedReportQueries,this.ownedReportQueries);
        // this.dtTrigger.next();
        // this.dtTrigger2.next();
        this.loading = false;
        this.rerender();
      }
    }, error => {
      this.toastr.error('Unable to get Free Form Reports Data.', 'Error');
      console.error('getReportsData', error);
      this.loading = false;
    });

  }

  addQueryModalOpen() {
    this.addEditQueryForm.patchValue({
      id: 0,
      Parameters: {}
    })
    this.addEditQueryReportModal.show();
  }

  onQueryReportFormSubmit(event) {
    console.log('submit form -> ',this.addEditQueryForm.value);
    let s = this.addEditQueryForm.value.QueryText.toLowerCase();
    if(s.includes('delete') || s.includes('truncate') || s.includes('drop') || s.includes('update') || s.includes('alter')){
      this.toastr.error('Statement non permesso nella query');
    }else{
      if(event=='debug'){
        this.debug();
      }
      else if(event=='test'){
        this.test();
      }else{
        //console.log('Selected Query name: ',this.addEditQueryForm.value.QueryName);
        if(this.addEditQueryForm.value.QueryName == 'kpiCalculationStatus'){
          this.toastr.error('Nome KpiCalculationStatus non consentito. Parola riservata.');
        }else{
          this.submitted = true;
          if (this.addEditQueryForm.invalid) {
          } else {
            this.formLoading = true;
            this._freeFormReport.addEditReportQuery(this.addEditQueryForm.value).subscribe(dashboardCreated => {
              //this.getReportsData();
              this.formLoading = false;
              this.submitted = false;
              //this.addEditQueryForm.reset();
              this.getOwnedQueries();
              this.getAssignedQueries();
              this.toastr.success('Query created successfully');
            }, error => {
              this.formLoading = false;
              this.toastr.error('Error while creating Query');
            });
          }
          this.isSubmit=1;
        }
      }
    }
  }

  ngOnDestroy(): void {
    this.dtTrigger.unsubscribe();
    this.dtTrigger2.unsubscribe();
    this.dtTrigger3.unsubscribe();
  }

  rerender(): void {
    this.datatableElements.forEach((dtElement: DataTableDirective) => {
      dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
        dtInstance.destroy();
        this.dtTrigger.next();
        this.dtTrigger2.next();
        this.dtTrigger3.next();
      });
    });
    this.setUpDataTableDependencies();
  }

  setUpDataTableDependencies() {
      // export only what is visible right now (filters & paginationapplied)
      $(this.btnExporta.nativeElement).off('click');
      $(this.btnExporta.nativeElement).on('click', function (event) {
          event.preventDefault();
          event.stopPropagation();
          //$this.datatableElement.dtInstance.then((datatable_Ref: DataTables.Api) => {
        //$this.table2csv('full', '.executedQueryResult');
        $this.fireEvent();
              /*if ($this.viewModel.filters.idKpi || $this.viewModel.filters.titoloBreve || $this.viewModel.filters.referenti || $this.viewModel.filters.tuttiContratti || $this.viewModel.filters.tutteLeFrequenze) {
                  $this.table2csv(datatable_Ref, 'visible', '.kpiTable');
              } else {
                  $this.table2csv(datatable_Ref, 'full', '.kpiTable');
              }*/
              //$this.table2csv(datatable_Ref, 'full', '.kpiTable');
          //});
      });

      //setTimeout(() => { this.applyScrollOnTopOfTable(); }, 100);
  }

  populateAssignedUsers(data,event){
    if (event.target.checked == true) {
      this.assignedUsers.push(data.ca_bsi_user_id);
      console.log(this.assignedUsers)
      //this.count++;
    } else {
      this.assignedUsers.splice(this.assignedUsers.indexOf(data.ca_bsi_user_id),1);
    }
  }

  save(){
    this.params.id = this.queryId;
    this.params.ids = this.assignedUsers;

    //console.log('save -> ',this.params.id,this.params.ids);

    this.toastr.info('Valore in aggiornamento..', 'Confirm');
    this._freeFormReport.setUserPermission(this.params).subscribe(data => {
      this.toastr.success('Valore Aggiornato', 'Success');
      this.hideConfigModal();
      this.getAssignedQueries();
      this.rerender();
    }, error => {
      this.toastr.error('Errore durante update.', 'Error');
      this.hideConfigModal();
    });
  }

  getReportQueryDetailByID(){
    this._freeFormReport.getReportQueryDetailByID(1).subscribe(data => {
      this.reportQueryDetail = data;
      console.log('reportQueryDetail -> ',this.reportQueryDetail);
    });
  }

  getOwnedQueries(){
    this._freeFormReport.getOwnedReportQueries().subscribe(data => {
      this.ownedReportQueries = data;
      this.rerender();
    });
  }

  getAssignedQueries(){
    this._freeFormReport.getAssignedReportQueries().subscribe(data => {
      this.assignedReportQueries = data;
    });
  }

  deleteQuery(data){
    this.toastr.info('Valore in aggiornamento..', 'Confirm');
    this._freeFormReport.DeleteReportQuery(data.id).subscribe(data => {
      this.getOwnedQueries();
      this.getAssignedQueries();
      this.toastr.success('Valore Aggiornato', 'Success');
    }, error => {
      this.toastr.error('Errore durante update.', 'Error');
    });
  }

  onCancel(dismissMethod: string): void {
    console.log('Cancel ', dismissMethod);
  }

  showConfigModal(row) {
    this.assignedUsers = [];
    //this.count = 0;
    this.queryId = row.id;
    this._freeFormReport.GetAllUsersAssignedQueries(row.id).subscribe(data => {
      //console.log('GetAllUsersAssignedQueries -> ',data);
      this.assignedQueriesBodyData = data;
      data.forEach(e => {
        if (e.isassigned === true) {
          console.log(this.assignedUsers)
          this.assignedUsers.push(e.ca_bsi_user_id);
          //this.count++;
        }
      });
      this.configModal.show();
     });
  }

  hideConfigModal() {
    this.configModal.hide();
  }

  showViewModal() {
    this.viewAssignedModal.show();
  }

  hideViewModal() {
    this.viewAssignedModal.hide();
  }

  showExecuteModal() {
    this.executeModal.show();
  }

  hideExecuteModal() {
    this.executeModal.hide();
  }

  showParametersModal() {
    this.parametersModal.show();
  }

  hideParametersModal() {
    this.parametersModal.hide();
  }

  viewAssigned(data){
    this.ownername = data.ownername;
    this._freeFormReport.getReportQueryDetailByID(data.id).subscribe(data => {
      this.viewAssignedData = data;
      console.log('viewAssignedData -> ',this.viewAssignedData)
      this.showViewModal();
    });
  }

  // exportAsXLSX(){
  //   let tableElm = '.executedQueryResult';
  //   var csv = '';
  //   var rows = [];
  //   var headers = [];
  //   $(tableElm + ' thead').each(function () {
  //     var $th = $(this);
  //     var text = $th.text();
  //     var header = '"' + text + '"';
  //     if (text != "") headers.push(header); 
  //   });
  //   csv+=headers;

  //   let totalRows =  $(tableElm + ' tbody tr').length;
  //   for (let i = 0; i < totalRows; i++) {
  //       var row = [];
  //       $($(tableElm).DataTable().row(i).node()).each((i, e) => {
  //           var $td = $(e);
  //           var text = $td.text();
  //           var cell = '"' + text + '"';
  //           row.push(cell);
  //       })
  //       rows.push(row);    
  //   }
  //   csv += rows;

  //   this.excelService.exportAsExcelFile(rows, 'sample');
  // }

  table2csv(exportmode, tableElm) {
      var csv = '';
      var headers = [];
      var rows = [];

      // Get header names
      $(tableElm + ' thead').find('th:not(.notExportCsv)').each(function () {
          var $th = $(this);
          var text = $th.text();
          var header = '"' + text + '"';
          // headers.push(header); // original code
          if (text != "") headers.push(header); // actually datatables seems to copy my original headers so there ist an amount of TH cells which are empty
      });
      csv += headers.join('|') + "\r\n";

      // get table data
      if (exportmode == "full") { // total data
          let totalRows =  $(tableElm + ' tbody tr').length;
          for (let i = 0; i < totalRows; i++) {
              var row = [];
              $($(tableElm).DataTable().row(i).node()).find('td:not(.notExportCsv)').each((i, e) => {
                  var $td = $(e);
                  var text = $td.text();
                  var cell = '"' + text + '"';
                  row.push(cell);
              })
              rows.push(row.join('|'));
              // SOL:1
              // let row = oTable.row(i).data();
              // row = $this.strip_tags(row);
              // rows.push(row);
              // SOL:2
              //rows.push(oTable.cells( oTable.row(i).nodes(), ':not(.notExportCsv)' ).data().join(','));
          }
      } else { // visible rows only
          $(tableElm + ' tbody tr:visible').each(function (index) {
              var row = [];
              $(this).find('td:not(.notExportCsv)').each(function () {
                  var $td = $(this);
                  var text = $td.text();
                  var cell = '"' + text + '"';
                  row.push(cell);
              });
              rows.push(row);
          })
      }
      csv += rows.join("\r\n");
      //var blob = new Blob([csv], { type: "application/vnd.ms-excel" });
      var blob = new Blob([csv], { type: "text/plain;charset=utf-8" });
      saveAs(blob, "ExportKPITable.csv");
  }
  fireEvent() {
    const ws: XLSX.WorkSheet = XLSX.utils.table_to_sheet(this.queryTable.nativeElement);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');
    XLSX.writeFile(wb, 'ReportExport.xlsx');
  }
  clearData(){
    this.debugQueryData = [];
    this.debugQueryValue = [];
  }

  valueCount = 0;
  // executeAssigned(data){
  //   this.valueCount = 0;
  //   this.clearData();

  //   this._freeFormReport.getReportQueryDetailByID(data.id).subscribe(data => {
  //     this.executeQueryData.QueryText = data.querytext;
  //     this.executeQueryData.Parameters = data.parameters;
  //     console.log('Debug -> ',this.executeQueryData);

  //     this._freeFormReport.ExecuteReportQuery(this.executeQueryData).subscribe(data => {
  //       this.debugQueryData = Object.keys(data[0]);
  //       Object.keys(data[0]).forEach(key => {
  //         this.debugQueryValue[this.valueCount] = data[0][key];
  //         this.valueCount++;
  //       });
  //       console.log('Debug Result -> ',this.debugQueryData);
  //     });
  //   });
  // }

  editQuery(data,table){
    if(table=='owned'){
      this.isReadonly=0;
      this.executeModalTitle = this.ownedModalTitle;
    }else if(table=='assigned'){
      this.isReadonly=1;
      this.executeModalTitle = this.assigendModalTitle;
    }

    this._freeFormReport.getReportQueryDetailByID(data.id).subscribe(data => {
      this.editQueryData.id = data.id;
      this.editQueryData.QueryName = data.queryname;
      this.editQueryData.QueryText = data.querytext;

      if(this.editQueryData.QueryName == 'kpiCalculationStatus'){
        this.isSpecialReport = 1;
      }else{
        this.isSpecialReport = 0;
      }

      //console.log('Parameters Length -> ',data.parameters.length);
      if(data.parameters.length==0){
      }else{
        this.editQueryData.Parameters = data.parameters;
        console.log('Edit Parameters -> ',data.parameters);
        for(let i=0;i<data.parameters.length;i++){
          let a = this.addEditQueryForm.get('Parameters') as FormArray;
          a.push(this.createParameters());
        }
      }
      this.addEditQueryForm.patchValue(this.editQueryData);
      console.log('Edit Query -> ',this.addEditQueryForm);
      this.showParametersModal();
    });
    (this.addEditQueryForm.get("Parameters") as FormArray)['controls'].splice(0);
  }

  debug(){
    this.hideData=0;
    this.valueCount = 0;
    this.loadingResult = true;
    this.clearData();

    this.executeQueryData.QueryText = this.addEditQueryForm.value.QueryText;
    this.executeQueryData.Parameters = this.addEditQueryForm.value.Parameters;
    //console.log('Debug -> ',this.executeQueryData);
    this._freeFormReport.ExecuteReportQuery(this.executeQueryData).subscribe(data => {
      this.debugResult = data;
      
      console.log('Debug Result -> ',this.debugResult);
      if(this.debugResult.length==0){
        //this.toastr.error('Errore in query execution', 'Error');
        this.debugResult = [{Error: 'No data found'}]
        this.debugQueryData = Object.keys(this.debugResult[0]);
      }else{

        ////////////// Setting Value ///////////////
        /*
        for (let i = 0; i < this.debugResult.length; i++) {
          //this.valueCount = 0;
          Object.keys(data[i]).forEach(key => {
            this.debugQueryValue[this.valueCount] = data[i];//[key];
            this.valueCount++;
          });
        }
        console.log('debugQueryValue -> ',this.debugQueryValue);
        */
        this.isDebug=1;
        if(data[0]=='O'){
          this.toastr.error('Errore esecuzione Free Form Report. ' +this.debugResult, 'Error');
          this.debugResult = [{Error: 'No data found'}]
          this.debugQueryData = Object.keys(this.debugResult[0]);
          this.hideData=1;
        }else{
          ////////////// Setting Key ///////////////
          $('#btnExporta').show();
          this.debugQueryData = Object.keys(data[0]);
          setTimeout(() => {
          this.dtTrigger3.next();
           this.rerender();
           this.loadingResult = false;
          }, 2000);
        }
      }
    },error => {
      this.toastr.error('Errore in query execution', 'Error');
    });
    this.hideParametersModal();
  }

  test() {
    
    this.hideData=0;
    this.valueCount = 0;
    this.loadingResult = true;
    this.clearData();

    this.executeQueryData.QueryText = this.addEditQueryForm.value.QueryText;
    this.executeQueryData.Parameters = this.addEditQueryForm.value.Parameters;
    this._freeFormReport.ExecuteReportQuery(this.executeQueryData).subscribe(data => {
      this.debugResult = data;
      //this.hideExport = false;
      if(this.debugResult.length==0){
        this.debugResult = [{Error: 'No data found'}]
        this.debugQueryData = Object.keys(this.debugResult[0]);
      }else{
        ////////////// Setting Key ///////////////
        //this.debugQueryData = Object.keys(data[0]);

        this.isDebug = 1;
        if(data[0]=='O'){
          this.toastr.error('Errore esecuzione Free Form Report. ' +this.debugResult, 'Error');
          this.debugResult = [{Error: 'No data found'}]
          this.debugQueryData = Object.keys(this.debugResult[0]);
          this.hideData=1;
        }else{
          if(this.debugResult.length > 10){
            this.debugResult = this.debugResult.splice(0,10);
          }
          ////////////// Setting Key ///////////////
          this.hideExport = false;
          this.debugQueryData = Object.keys(data[0]);

          // for(let value of this.debugResult){
          //   for(let key of this.debugQueryData[0]){
          //     console.log(value[key]);
          //   }
          // }

          console.log(this.debugQueryData[0]);
          $('#btnExporta').show();
          //debugger;
          setTimeout(() => {
          this.dtTrigger3.next();
           this.rerender();
            this.loadingResult = false;
            this.hideExport = false;
          }, 2000);
        }
      }
    },error => {
      this.toastr.error('Errore in query execution', 'Error');
    });
    this.hideParametersModal();
  }

  disable(row){
    this._freeFormReport.disable(row.id).subscribe(data => {
      this.getOwnedQueries();
      this.toastr.success('Query disabled');
    }, error => {
        this.toastr.error('Error in disabling query');
    });
  }

  enable(row){
    this._freeFormReport.enable(row.id).subscribe(data => {
      this.getOwnedQueries();
      this.toastr.success('Query enabled');
    }, error => {
        this.toastr.error('Error in enabling query');
    });
  }


}

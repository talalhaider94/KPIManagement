import { Component, Directive, Input, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { FreeFormReportService } from '../../../_services';
import { forkJoin } from 'rxjs';
import { Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { DataTableDirective } from 'angular-datatables';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';


@Component({
  selector: 'app-free-form-report',
  templateUrl: './form-report-query.component.html',
//   styleUrls: ['./free-form-report.component.scss']
})

export class FormReportQueryComponent implements OnInit { 

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

  assignedReportQueries: any = [];
  ownedReportQueries: any = [];
  debugQueryData: any = [];
  debugQueryValue: any = [];
  valueCount = 0;
  debugResult;
  hideData=0;
  isloaded=0;
  executeQueryData = {
    QueryText: '',
    Parameters: [{
      key: '',
      value: ''
    }]
  }
  debugCount = 0;
  QueryName;
  QueryText;
  parametersData = {
    key: '',
    value: ''
  };
  form: FormGroup;
  
  loading: boolean = false;
  formLoading: boolean = false;
  submitted: boolean = false;
  isSubmit=0;
  parameterCount=0;

  modalTitle: string = 'Add Query Report';
  @ViewChild(DataTableDirective)
  datatableElement: DataTableDirective;

  @ViewChild('addEditQueryReportModal')
  addEditQueryReportModal: ModalDirective;

  dtOptions: DataTables.Settings = {};
  dtTrigger = new Subject();

  addEditQueryForm: FormGroup;
  Parameters: FormArray;

  constructor(
    private _freeFormReport: FreeFormReportService,
    private toastr: ToastrService,
    private formBuilder: FormBuilder,
    private fb: FormBuilder
  ) {
      // this.form = this.fb.group({
      //   published: true,
      //   credentials: this.fb.array([]),
      // });
   }

  get f() { return this.addEditQueryForm.controls; }
  
  ngOnInit() {

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
      }
    };
    
    this.addEditQueryForm = this.formBuilder.group({
      id: [0, Validators.required],
      QueryName: ['', Validators.required],
      QueryText: ['', Validators.required],
      Parameters: this.formBuilder.array([]),
      //Parameters: this.formBuilder.array([ this.createParameters() ]),
    });
  }

  createParameters(): FormGroup {
    return this.formBuilder.group({
      key: '',
      value: '',
    });
  }

  clearData(){ 
    this.debugQueryData = [];
    this.debugQueryValue = [];
  }

  addParameters(): void {
    this.Parameters = this.addEditQueryForm.get('Parameters') as FormArray;
    this.Parameters.push(this.createParameters());
    this.parameterCount=1;
  }

  deleteParameters(id: number) {
    this.Parameters.removeAt(id);
    
  }

  onQueryReportFormSubmit(event) {
    // const creds = this.form.controls.credentials as FormArray;
    console.log('submit form -> ',this.addEditQueryForm.value);
    let s = this.addEditQueryForm.value.QueryText.toLowerCase();
    if(s.includes('delete') || s.includes('truncate') || s.includes('drop') || s.includes('update') || s.includes('alter')){
      this.toastr.error('Statement non permesso nella query');
    }else{
      if(event=='debug'){
        this.loading=true;
        this.debug();
      }else{
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
              this.addEditQueryForm.reset();
              this.toastr.success('Query created successfully');
            }, error => {
              this.loading = false;
              this.formLoading = false;
              this.toastr.error('Errore esecuzione report');
            });
          }
          this.isSubmit=1;
        }
      }
    }
  }
  
  debug(){
    this.valueCount = 0;
    this.clearData();

    this.executeQueryData.QueryText = this.addEditQueryForm.value.QueryText;
    this.executeQueryData.Parameters = this.addEditQueryForm.value.Parameters;
    //console.log('Debug -> ',this.executeQueryData);
    this._freeFormReport.ExecuteReportQuery(this.executeQueryData).subscribe(data => {
      this.loading = false;
      this.debugResult = data;
      console.log('Debug Result -> ',data);
      if(this.debugResult.length==0){
        //this.toastr.error('Errore in query execution', 'Error');
        this.debugResult = [{Error: 'No data found'}]
        this.debugQueryData = Object.keys(this.debugResult[0]);
      }else{
        ////////////// Setting Key ///////////////
        //this.debugQueryData = Object.keys(data[0]);
        ////////////// Setting Value ///////////////
        // Object.keys(data[0]).forEach(key => {
        //   this.debugQueryValue[this.valueCount] = data[0][key];  
        //   this.valueCount++; 
        // });
        //console.log('debugQueryValue -> ',this.debugQueryValue); 
        if(data[0]=='O'){
          this.toastr.error('Errore esecuzione Free Form Report. ' +this.debugResult, 'Error');
          this.hideData=1;
        }else{ 
          if(this.debugResult.length > 10){
            this.debugResult = this.debugResult.splice(0,10);
          }
          ////////////// Setting Key ///////////////
          this.debugQueryData = Object.keys(data[0]);
        }
        this.isloaded=1;
      }
    });
  }

//   ngOnDestroy(): void {
//     this.dtTrigger.unsubscribe();
//   }

//   rerender(): void {
//     this.datatableElement.dtInstance.then((dtInstance: DataTables.Api) => {
//       dtInstance.destroy();
//       this.dtTrigger.next();
//     });
//   }
  


}

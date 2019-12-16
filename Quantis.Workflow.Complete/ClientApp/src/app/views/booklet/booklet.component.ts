import { Component, OnInit, ViewChild, TemplateRef,QueryList, ElementRef } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { FormControl, FormGroup, FormControlName } from '@angular/forms';
import { DataTableDirective } from 'angular-datatables';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ApiService } from '../../_services/api.service';
import { filter } from 'rxjs/operators';
import { ReactiveFormsModule } from '@angular/forms';
import { forEach } from '@angular/router/src/utils/collection';
import { deepStrictEqual } from 'assert';
import { DateAdapter } from '@angular/material';
import { ModalDirective } from 'ngx-bootstrap/modal';
import * as moment from 'moment';
import { ToastrService } from 'ngx-toastr';

declare var $;
var $this;

@Component({
    selector: 'app-booklet',
    templateUrl: './booklet.component.html',
    styleUrls: ['./booklet.component.scss']
})
export class BookletComponent implements OnInit {
    datacorrente: any;
    dataprecedente: any;
    documenti = [];
    array = [];
    contrat = [];
    itemArray = [];
    utente:any = JSON.parse(localStorage.getItem('currentUser'));
    @ViewChild('ArchivedkpiTable') block: ElementRef;
    @ViewChild('thresholdModal') public thresholdModal: ModalDirective;
    @ViewChild('searchCol1') searchCol1: ElementRef;
    @ViewChild('searchCol2') searchCol2: ElementRef;
    @ViewChild('btnExportCSV') btnExportCSV: ElementRef;
    @ViewChild(DataTableDirective)  datatableElement: DataTableDirective;
    validEmail;

    loading: boolean = false;
    documentId=0;
    item=0;
    bookletStatus=0;

    contrattiDef: any = [{
        checked: false,
        clienti: '',
        contratto: ''
    }];

  documentiDef: any = [{
    id: 'documentid',
    nome: 'documentname'
  }];
  viewModel = {
    filters: {
      tuttiClienti: ''
    }
  };
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
    dtTrigger: Subject<any> = new Subject();
    // dtTrigger: Subject<any> = new Subject();

    // @ViewChild(DataTableDirective) dtElement: DataTableDirective;
    constructor(private apiService: ApiService,private toastr: ToastrService) {
        this.documenti = [
            { id: 1, nome: 'contratto1', cognome: 'giacomo', status: 'attivo', success: 50, danger: 10, warning: 12 },
            { id: 2, nome: 'contratto2', cognome: 'rino', status: 'attesa', success: 200, danger: 0, warning: 0 },
            { id: 3, nome: 'contratto3', cognome: 'carmine', status: 'null', success: 100, danger: 30, warning: 12 },
            { id: 4, nome: 'contratto4', cognome: 'mary', status: 'rifiutato', success: 50, danger: 10, warning: 12 },
            { id: 5, nome: 'contratto5', cognome: 'fausto', status: 'bel', success: 50, danger: 10, warning: 32 }

        ]
    }

    ngOnInit() {
         
    }

    ngAfterViewInit() {
        this.dtTrigger.next();
        this.getDocumenti();
        this.getContratti();
        this.getcontract();
        this.getCheckedItemList();
        this.dataprecedente = moment().subtract(1, 'month').format('MM/YYYY');
        this.datacorrente = moment().format('MM/YYYY');  
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
            // dtInstance.columns(1).every(function () {
            //   const that = this;

            //   const select = $('#searchCol1')
            //     .on('change', function () {
            //       that
            //         .search('^' + $(this).val().replace(')', '\\)').replace('(', '\\(') + '$', true, false, false)
            //         .draw();
            //     });

            //   this
            //     .cache('search')
            //     .sort()
            //     .unique()
            //     .each(function (d) {
            //       select.append($('<option value="' + d + '">' + d + '</option>'));
            //     });
            // });

        });
        /*$this.datatableElement.dtInstance.then((datatable_Ref: DataTables.Api) => {

        });*/
    }



    getContratti() {
        this.apiService.getAllKpiHierarchy(this.utente.userid).subscribe((data: any) => {
            this.contrattiDef = data;
            console.log('contrattiDef', this.contrattiDef);
            //this.recuperoChildren();
        });
    }
    getcontract(){
        this.apiService.getContractWithContractParties().subscribe((data:any)=>{
            this.contrat = data;
            console.log('getContractsWithContractParties',data);
            this.rerender();
        })
    }
    getDocumenti() {
        this.apiService.getBooklet().subscribe((data: any) => {
            this.documentiDef = data;
            console.log('documentiDef', this.documentiDef);
            this.rerender();
        });
    }

    /*recuperoChildren(){
      this.contrattiDef.forEach((x:any) => {
       // console.log('cliente',x.name);
        x.children.forEach((y:any) => {
          //cliente:x.name;
         // contratto:y.name
          console.log('contratti',x.name,'-',y.name);
          this.array.push({cliente:x.name,contratto:y.name,checked:false});
        });
      });
      console.log('array',this.array)
    }*/
    /*addItem(){
      this.getBooklet();
       this.documentiDef.map(item => {
      return {
          customer: item.name,
      }
    }).forEach(item => this.array.push(item));
    console.log(JSON.stringify(this.array))
    }*/

    /*getUsers() {
      this.apiService.getCatalogoUsers().subscribe((data: any)=>{
        this.UtentiTableBodyData=data;
        console.log('user ', this.UtentiTableBodyData);
      });
  }*/

    checkedList = [];
    consoleSelect(event) {
        console.log(event);
        this.documentId = event.target.selectedOptions[0].value;
        console.log('documentId -> ',this.documentId);
    }

    getId(obj){
        // console.log(obj,'selected obj');
        this.item = 1;
        var index = this.itemArray.indexOf(obj.contractid);
        if(index === -1){
          // val not found, pushing onto array
          this.itemArray.push(obj.contractid);
        }else{
          // val is found, removing from array
          this.itemArray.splice(index,1);
        }
        if(this.itemArray.length == 0){
        this.item = 0;
        }
        console.log(this.itemArray,'contractid')

    }
    getCheckedItemList() {
        this.checkedList = [];
        for (var i = 0; i < this.array.length; i++) {
            if (this.array[i].checked)
                this.checkedList.push(this.array[i]);
        }
        console.log('data ', this.checkedList);
    }
    createbooklet(){
        this.hideThresholdModal();
        let count = this.itemArray.length;
        if(this.validEmail != null || this.validEmail != ''){
            let data = {
                ContractIds:this.itemArray,
                BookletDocumentId:this.documentiDef.documentid,
                RecipientEmail:this.validEmail
            }
            console.log(data,'payload');
            this.apiService.CreateBooklet(data).subscribe((data: any) => {
                console.log(data,' called createbooklet');
                this.bookletStatus=1;
                if(data==0 || data==null){
                    this.toastr.error('Error', 'Error in adding booklet');
                    this.loading = false;
                }
                else{
                    this.toastr.success('Success', 'Al termine della elaborazione i booklet ('+count+') verranno inviati al seguente indirizzo : '+this.validEmail);
                    this.loading = false;
                }
            },error=>{
                this.bookletStatus=0;
            });
            //console.log('bookletStatus: ',this.bookletStatus) 
        }
    }
    addBooklet(){
        let isValid ;
        this.loading = true;
         this.apiService.getCatalogEmailByUser().subscribe((data: any) => {
            console.log('Email: ',data)
            isValid = data;
            if(data == null || data==''){
                this.showThresholdModal();
            }else{
                this.validEmail = data;
                this.createbooklet();
                
            }
            // if(isValid == this.utente.useremail) {
            //   this.validEmail = data;
            //   this.createbooklet();
            // }else{
            //   this.showThresholdModal();
            // }

        });

        /*if(isValid == this.utente.useremail){
            let data = {
                ListContract:this.itemArray,
                BookletDocumentId:this.documentiDef.documentid,
                RecipientEmail: isValid
            }
              this.apiService.CreateBooklet(data).subscribe((data: any) => {
                  console.log(data)
            });
        }else{
              await this.showThresholdModal();
              this.createbooklet();

        }*/

    }

    showThresholdModal() {
        this.thresholdModal.show();
    }

    hideThresholdModal() {
        this.thresholdModal.hide();
    }
}

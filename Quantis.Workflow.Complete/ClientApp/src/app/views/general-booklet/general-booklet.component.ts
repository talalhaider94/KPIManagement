import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { DataTableDirective } from 'angular-datatables';
import { ApiService } from '../../_services/api.service';
import { Subject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ModalDirective } from 'ngx-bootstrap/modal';

declare var $;
var $this;

@Component({
  templateUrl: './general-booklet.component.html'
})

export class GeneralBookletComponent implements OnInit {
  @ViewChild('thresholdModal') public thresholdModal: ModalDirective;
  @ViewChild('ConfigurationTable') block: ElementRef;
  @ViewChild(DataTableDirective) private datatableElement: DataTableDirective;

  contractArray = [];
  bookletArray = [];
  documentId = 0;
  contractItem = 0;
  bookletItem = 0;
  validEmail;
  category_id: number = 0;

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
  ConfigTableBodyData: any = [];
  loading: boolean = false;
  bookletStatus = 0;
  BookletContracts = [];
  BookletContractsMain;
  bookletContractsObj = {
    ContractId: 0,
    BookletDocumentId: 0
  }

  constructor(
    private apiService: ApiService,
    private toastr: ToastrService,
  ) {
    $this = this;
  }

  ngOnInit() {
  }

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
  setUpDataTableDependencies() {
  }

  strip_tags(html) {
    var tmp = document.createElement("div");
    tmp.innerHTML = html;
    return tmp.textContent || tmp.innerText;
  }

  getCOnfigurations() {
    this.apiService.GetBookletsLV().subscribe((data) => {
      this.ConfigTableBodyData = data;
      console.log('Booklets ', data);
      this.rerender();
      //this.contractArray.length=0;
    });
  }

  onCancel(dismissMethod: string): void {
    console.log('Cancel ', dismissMethod);
  }

  showThresholdModal() {
    this.thresholdModal.show();
  }

  hideThresholdModal() {
    this.thresholdModal.hide();
  }


  getId(obj) {
    this.bookletContractsObj.ContractId = obj.contractcontext;
    this.bookletContractsObj.BookletDocumentId = obj.templateid;

    this.BookletContractsMain = this.bookletContractsObj;
    let index = this.contractArray.findIndex(d => d.ContractId === obj.contractcontext);
    if (index != -1)
      this.contractArray.splice(index, 1);
    if (index == -1)
      this.contractArray.push({ ContractId: obj.contractcontext, BookletDocumentId: obj.templateid });
  }

  addBooklet() {
    let isValid;
    this.loading = true;
    this.apiService.getCatalogEmailByUser().subscribe((data: any) => {
      console.log('Email: ', data)
      isValid = data;
      if (data == null || data == '') {
        this.showThresholdModal();
      } else {
        this.validEmail = data;
        this.createbooklet();
      }
    });
  }

  createbooklet() {
    this.hideThresholdModal();
    let count = this.contractArray.length;
    if (this.validEmail != null || this.validEmail != '') {
      let data = {
        RecipientEmail: this.validEmail,
        BookletContracts: this.contractArray

      }
      console.log(data);
      this.apiService.CreateBooklet(data).subscribe((data: any) => {
        console.log(data, ' called createbooklet');
        // for (var i = 0; i < this.contractArray.length; i++) {
        //   let index1 = this.ConfigTableBodyData.findIndex(d => d.contractcontext === this.contractArray[i].ContractId);
        //   if (index1 != -1)
        //     this.ConfigTableBodyData.splice(index1, 1);
        //     this.rerender();
        // }
        this.bookletStatus = 1;
        if (data == 0 || data == null) {
          this.toastr.error('Error', 'Error in adding booklet');
          this.loading = false;
        }
        else {
          this.toastr.success('Success', 'Al termine della elaborazione i booklet (' + count + ') verranno inviati al seguente indirizzo : ' + this.validEmail);
          this.loading = false;
        }
      }, error => {
        this.bookletStatus = 0;
      });
      
    }
    this.getCOnfigurations();    
  }
}

import { Component, OnInit, ViewChild, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { DataTableDirective } from 'angular-datatables';
import { ApiService } from '../../_services/api.service';
import { ToastrService } from 'ngx-toastr';
import { TreeViewComponent } from '@syncfusion/ej2-angular-navigations';
import { ITreeOptions, TreeComponent } from 'angular-tree-component';
import { TreeviewItem, TreeviewConfig } from 'ngx-treeview';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { Subject } from 'rxjs';
import { saveAs } from 'file-saver';
import { forEach } from '@angular/router/src/utils/collection';

declare var $;
let $this;

@Component({
    templateUrl: './userprofiling.component.html',
    styleUrls: ['./userprofiling.component.scss']
})

export class UserProfilingComponent implements OnInit {
    //@ViewChild('permissionsTree') permissionsTree: TreeViewComponent;
    @ViewChild('contractsModal') public contractsModal: ModalDirective;
    @ViewChild('kpisModal') public kpisModal: ModalDirective;
    @ViewChildren('permissionsTree') allTreesNodes !: QueryList<TreeViewComponent>;
    @ViewChild('csvTable') block: ElementRef;
    @ViewChild('btnExporta') btnExporta: ElementRef;
    @ViewChild(DataTableDirective)
    datatableElement: DataTableDirective;
    dtTrigger: Subject<any> = new Subject();
    dtOptions: any = {};
    loadingSpinner : boolean = false;
    isTreeLoaded = false;
    treesArray = [];
    allCurrentChildIds = [];
    assign = true;
    unassign = false;
  csvTableData = [];
  organizationLoading = false;

    public treeFields: any = {
        dataSource: [],
        id: 'id',
        text: 'name',
        child: 'children',
        title: 'name'
    };

  innerMostChildrenNodeIds = [];
  organizationUnitsPresence = false;
  saveOrganization = [];
  saveWFContract = [];
    gatheredData = {
        usersList: [],
        rolesList: [],
        assignedPermissions: []
    }
    permissionsData: any;
    contractsData: any;
    workflowContractData: any;
    kpisData: any;
    kpisId = [];
    storedIds = [];
    saveKpisData = {
        userId: 0,
        contractId: 0,
        kpiIds: []
    }
    selectedData = {
      userid: null,
      contractID: null,
        permid: null,
        name: '',
        contractParty: '',
        contractName: '',
        checked: null,
      selected: null,
      numOrganization: null
    }

    filters: any = {
        searchUsersText: '',
        searchPermissionsText: ''
    }

    loading = {
        users: false,
        roles: false
    }
    selectedUserObj = {};
    selectedContractsObj = {};
    selectedKpisObj = {};
    hideExport: boolean = true;
    constructor(
        private apiService: ApiService,
        private toastr: ToastrService,
    ) {
        $this = this;

        this.dtOptions = {
            dom: 'Bfrtip',
            buttons: [
                {
                    extend: 'csv',
                    text: '<i class="fa fa-file"></i> Esporta CSV',
                    titleAttr: 'Esporta CSV',
                    className: 'btn btn-primary mb-3'
                },
            ],
            //'dom': 'rtip',
            "columnDefs": [{
                "targets": [11],
                "visible": false,
                "searchable": true
            }],
            deferRender: true,
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

    ngOnInit() {
        this.dtOptions = {
            //dom: 'Bfrtip',
            "columnDefs": [{
                "targets": [12],
                "visible": false,
                "searchable": true
            }],
            deferRender: true,
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

       /* this.apiService.getUserProfilingCSV().subscribe((data) => {
            console.log('CSV Data => ', data);
            this.csvTableData = data;
            this.hideExport = false;
            this.rerender();
        });*/

      this.loading.roles = true;
      this.apiService.GetContractParties().subscribe((res) => {
        console.log('GetContractParties ==> ', res);
            this.gatheredData.usersList = res;
            this.loading.roles = false;
            //this.selectedData.userid = res.ca_bsi_user_id;
        }, err => { this.loading.roles = true; this.toastr.warning('Connection error', 'Info') });

        // this.apiService.getAllKpiHierarchy().subscribe(data=>{
        //   console.log('getAllKpiHierarchy ==> ', data);
        //   //this.treeFields.dataSource = data;
        //   this.createTrees(data);
        // }, err => {this.isTreeLoaded = true; this.toastr.warning('Connection error', 'Info')});
    }

    // tslint:disable-next-line:use-life-cycle-interface
    ngAfterViewInit() {
        this.dtTrigger.next();
        this.setUpDataTableDependencies();
    }

    ngOnDestroy(): void {
        this.dtTrigger.unsubscribe();
    }

    rerender(): void {
        this.datatableElement.dtInstance.then((dtInstance: DataTables.Api) => {
            dtInstance.destroy();
            this.dtTrigger.next();
            this.setUpDataTableDependencies();
        });
    }

    populatePermission(data) {
        this.selectedData.permid = data.id;
        console.log('Contract Party ID ==> ', this.selectedData.permid);
    }

    selectRoleItem(contractparty, $event) {
      this.selectedUserObj = contractparty;
        if ($event) {
            $('.role-permissions-lists ul.users-list li').removeClass('highlited-user');
            $($event.target).addClass('highlited-user');
        }
      this.selectedData.userid = contractparty.id;
      this.selectedData.name = contractparty.name;

      if (this.selectedData.userid) {
        this.apiService.GetContractsByContractParty(this.selectedData.userid).subscribe(data => {
                this.permissionsData = data;
                console.log('getFirstLevelHierarchy ==> ', data);
            });
        } else {
            this.permissionsData = [];
            //this.uncheckAllTrees();
        }
    }

  getContracts(data) {
    console.log(data)
    this.organizationLoading = true;
    this.organizationUnitsPresence = false;
    this.contractsData = [];
    this.workflowContractData = [];
    this.saveOrganization = [];
    this.saveWFContract = [];
        this.selectedContractsObj = data;
        this.selectedData.permid = data.id;
    this.selectedData.contractParty = data.name;
    this.selectedData.contractName = data.contractname;
    this.selectedData.contractID = data.contractid;
    console.log('getContracts ==> ', this.selectedData.userid, this.selectedData.permid);
    this.apiService.GetWorkflowByContract(data.contractid).subscribe(data => {
      this.workflowContractData = data;
      //console.log('wf', data[0].workflow_day)
      if (data.length > 0) {
        if (data[0].workflow_day >= 0) {
          this.saveWFContract[data[0].sla_id] = data[0].workflow_day;
        } else {
          this.saveWFContract[data[0].sla_id] = null;
        }
        
      }
    })
        this.apiService.GetOrganizationUnitsByContract(data.contractid).subscribe(data => {
          this.contractsData = data;
          for (let i = 0; i < data.length; i++) {
            if (data[i].workflow_day != -1) {
              this.saveOrganization[data[i].id] = data[i].workflow_day;
            }
          }
          if (data.length > 0) { this.organizationUnitsPresence = true; }
          this.selectedData.numOrganization = this.contractsData.length;
          console.log((this.contractsData))
          this.organizationLoading = false;
        });
        this.showContractsModal();
    }

    getContractParties(data) {
      this.selectedContractsObj = data;
      this.selectedData.permid = data.id;
      this.selectedData.contractParty = data.name;
      console.log('getContracts ==> ', this.selectedData.userid, this.selectedData.permid);
      this.apiService.getContracts(this.selectedData.userid, this.selectedData.permid).subscribe(data => {
        this.contractsData = data;
      });
      this.showContractsModal();
    }

    getKpis(data) {
        this.selectedKpisObj = data;
        this.selectedData.permid = data.id;
        this.selectedData.contractName = data.name;
        console.log('getKpis ==> ', this.selectedData.userid, this.selectedData.permid);
        this.apiService.getKpis(this.selectedData.userid, this.selectedData.permid).subscribe(data => {
            this.kpisData = data;
            // initially add ids of checked kpis
            this.kpisId = [];
            data.forEach((item) => {
                item.code == '1' ? this.kpisId.push(item.id) : null;
            });
        });
        this.showKpisModal();
    }

    assignedPermissions(data) {
        this.selectedData.permid = data.id;
        console.log('assignedPermissions ==> ', this.selectedData.userid, this.selectedData.permid, this.assign);
        this.apiService.assignContractParty(this.selectedData.userid, this.selectedData.permid).subscribe(data => {
            this.toastr.success('Saved', 'Success');
            this.selectRoleItem(this.selectedUserObj, null);
        }, error => {
            this.toastr.error('Not Saved', 'Error');
        });
    }

    unAssignedPermissions(data) {
        this.selectedData.permid = data.id;
        console.log('unAssignedPermissions ==> ', this.selectedData.userid, this.selectedData.permid, this.unassign);
        this.apiService.unassignContractParty(this.selectedData.userid, this.selectedData.permid).subscribe(data => {
            this.toastr.success('Saved', 'Success');
            this.selectRoleItem(this.selectedUserObj, null);
        }, error => {
            this.toastr.error('Not Saved', 'Error');
        });
    }

    assignedContracts(data) {
        this.selectedData.permid = data.id;
        console.log('assignedContracts ==> ', this.selectedData.userid, this.selectedData.permid, this.assign);
        this.apiService.assignContracts(this.selectedData.userid, this.selectedData.permid).subscribe(data => {
            this.toastr.success('Saved', 'Success');
            this.getContracts(this.selectedContractsObj);
            this.selectRoleItem(this.selectedUserObj, null);
        }, error => {
            this.toastr.error('Not Saved', 'Error');
        });
    }

    unAssignedContracts(data) {
        this.selectedData.permid = data.id;
        console.log('unAssignedContracts ==> ', this.selectedData.userid, this.selectedData.permid, this.unassign);
        this.apiService.unassignContracts(this.selectedData.userid, this.selectedData.permid).subscribe(data => {
            this.toastr.success('Saved', 'Success');
            this.getContracts(this.selectedContractsObj);
            this.selectRoleItem(this.selectedUserObj, null);
        }, error => {
            this.toastr.error('Not Saved', 'Error');
        });
    }

    storeKpis(data, event) {
        // event.target.checked
        if (!event.target.checked) {
            let idx = this.kpisId.indexOf(data.id);
            if (idx > -1) {
                this.kpisId.splice(idx, 1);
            }
        } else {
            if (!this.kpisId.includes(data.id)) {
                this.kpisId.push(data.id);
            }
        }
        console.log('storedKpis ==> ', this.kpisId);
    }

    assignKpis() {
        this.saveKpisData.userId = this.selectedData.userid;
        this.saveKpisData.contractId = this.selectedData.permid;
        this.saveKpisData.kpiIds = this.kpisId;

        this.apiService.assignKpistoUser(this.saveKpisData).subscribe(data => {
            this.toastr.success('Saved', 'Success');
            this.selectRoleItem(this.selectedUserObj, null);
            this.hideKpisModal();
            this.hideContractsModal();
        }, error => {
            this.toastr.error('Not Saved', 'Error');
        });
        console.log('assignKpis ==> ', this.saveKpisData);
    }

    addLoaderToTrees(add = true) {
        let load = false;
        if (add === false) {
            load = true;
        }
        this.treesArray.forEach((itm: any) => {
            itm.loaded = load;
        });
    }
  
    setUpDataTableDependencies() {
        // export only what is visible right now (filters & paginationapplied)
        /*$(this.btnExporta.nativeElement).off('click');
        $(this.btnExporta.nativeElement).on('click', function (event) {
            event.preventDefault();
            event.stopPropagation();
            $this.loadingSpinner = true;
            $this.datatableElement.dtInstance.then((datatable_Ref: DataTables.Api) => {
                $this.table2csv(datatable_Ref, 'full', '.csvTable');
            });
        });*/
    }

  /*saveOrganizationsWorkflow() {
    console.log('save', this.selectedData);
    console.log(this.saveOrganization);
    let organizationCompiled = 0;
    let allFieldsFilled = false;
    let valueError = false;
    let keys = Object.keys(this.saveOrganization);
    for (let i = 0; i < keys.length; i++) {
      if (this.saveOrganization[keys[i]] != null) {
        if (this.saveOrganization[keys[i]] < 0 || this.saveOrganization[keys[i]] > 28) {
          valueError = true;
        } else {
          allFieldsFilled = true
        }
        organizationCompiled++;
      }
    }
    if (this.saveWFContract[this.selectedData.contractID] != null) {
      if (this.saveWFContract[this.selectedData.contractID] < 0 || this.saveWFContract[this.selectedData.contractID] > 28) {
        valueError = true;
      }
    } else {
      allFieldsFilled = false;
    }
    if (organizationCompiled == this.selectedData.numOrganization) {
      if (allFieldsFilled == true && valueError == false) {
          this.apiService.AssignCuttoffWorkflowDayByContractIdAndOrganization(this.selectedData.contractID, '-1', -1, this.saveWFContract[this.selectedData.contractID]).subscribe(data => {
            this.toastr.success('Successo', 'Configurazione Salvata');
            this.hideContractsModal();
          }, error => {
            this.toastr.error('Errore', 'Non tutte le configurazioni sono state salvate');
          });
    
        for (let i = 0; i < keys.length; i++) {
          console.log('sla: ' + this.selectedData.contractID, 'organization: ' + keys[i], 'workflowDay: ' + this.saveOrganization[keys[i]])
          this.apiService.AssignCuttoffWorkflowDayByContractIdAndOrganization(this.selectedData.contractID, keys[i], -1, this.saveOrganization[keys[i]]).subscribe(data => {
            this.toastr.success('Successo', 'Configurazione Salvata');
            this.hideContractsModal();
          }, error => {
            this.toastr.error('Errore', 'Non tutte le configurazioni sono state salvate');
          });
        }
      } else {
        console.log(5,valueError, allFieldsFilled)
        if (valueError == true) {
          this.toastr.error('Il valore deve essere compreso tra 0 e 28', 'Errore');
        } else {
          this.toastr.error('Tutti i campi deve essere compilati', 'Errore');
        }
      }

    } else {
      console.log(organizationCompiled, this.selectedData.numOrganization)
      this.toastr.error('Tutti i campi deve essere compilati', 'Errore');
    }
    console.log(valueError, allFieldsFilled)
  }*/
  saveOrganizationsWorkflow() {
    console.log('save', this.selectedData);
    console.log(this.saveOrganization);
    /*let organizationCompiled = 0;
    let allFieldsFilled = false;
    let valueError = false;*/
    let keys = Object.keys(this.saveOrganization);
  
    if (this.saveWFContract[this.selectedData.contractID] != null) {
      if (this.saveWFContract[this.selectedData.contractID] >= 0 && this.saveWFContract[this.selectedData.contractID] <= 28) {
        this.apiService.AssignCuttoffWorkflowDayByContractIdAndOrganization(this.selectedData.contractID, '-1', -1, this.saveWFContract[this.selectedData.contractID]).subscribe(data => {
          this.toastr.success('Successo', 'Configurazione Salvata');
          this.hideContractsModal();
        }, error => {
          this.toastr.error('Errore', 'Non tutte le configurazioni sono state salvate, riprovare.');
        });
      } else {
        this.toastr.error('Il valore deve essere compreso tra 0 e 28', 'Errore');
      }
    }

    for (let i = 0; i < keys.length; i++) {
      if (this.saveOrganization[keys[i]] != null) {
        if (this.saveOrganization[keys[i]] >= 0 && this.saveOrganization[keys[i]] <= 28) {
          console.log('sla: ' + this.selectedData.contractID, 'organization: ' + keys[i], 'workflowDay: ' + this.saveOrganization[keys[i]])
          this.apiService.AssignCuttoffWorkflowDayByContractIdAndOrganization(this.selectedData.contractID, keys[i], -1, this.saveOrganization[keys[i]]).subscribe(data => {
            this.toastr.success('Successo', 'Configurazione Salvata');
            this.hideContractsModal();
          }, error => {
            this.toastr.error('Errore', 'Non tutte le configurazioni sono state salvate');
          });
        } else {
          this.toastr.error('Il valore deve essere compreso tra 0 e 28', 'Errore');
        }
      }
    }
    /*  } else {
        console.log(5, valueError, allFieldsFilled)
        if (valueError == true) {
          this.toastr.error('Il valore deve essere compreso tra 0 e 28', 'Errore');
        } else {
          this.toastr.error('Tutti i campi deve essere compilati', 'Errore');
        }
      }*/
  }
    showContractsModal() {
        this.contractsModal.show();
    }

    hideContractsModal() {
        this.contractsModal.hide();
    }

    showKpisModal() {
        this.kpisModal.show();
    }

    hideKpisModal() {
        this.kpisModal.hide();
    }
  checkLimit(event, type, id) {
    if (event < 0 || event > 28) {
        this.toastr.error('Il valore deve essere compreso tra 0 e 28', 'Errore');
    }
    /*console.log(event,type,id)
    if (event > 28) {
      if (type == 'contract') {
        this.saveWFContract[id] = 28;
      }
      if (type == 'organization') {
        this.saveOrganization[id] = 28;
      }
    }
    if (event < 0) {
      if (type == 'contract') {
        this.saveWFContract[id] = 0;
      }
      if (type == 'organization') {
        this.saveOrganization[id] = 0;
      }
    }*/
  }
}

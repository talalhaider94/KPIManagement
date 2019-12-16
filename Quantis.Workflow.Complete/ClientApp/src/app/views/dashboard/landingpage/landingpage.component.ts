import { Component, OnInit, ComponentRef, ViewChildren,ViewChild, ElementRef,QueryList } from '@angular/core';
import { GridsterConfig, GridsterItem, GridType, CompactType, DisplayGrid } from 'angular-gridster2';
import { DashboardService, EmitterService } from '../../../_services';
import { DateTimeService } from '../../../_helpers';
import { ActivatedRoute } from '@angular/router';
import { DashboardModel, DashboardContentModel, WidgetModel, ComponentCollection } from '../../../_models';
import { Subscription, forkJoin, interval } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../_services/api.service';
import { UUID } from 'angular2-uuid';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { DataTableDirective } from 'angular-datatables';
import { GlobalVarsService } from '../../../_services/global-vars.service';
import { forEach } from '@angular/router/src/utils/collection';

@Component({
    templateUrl: 'landingpage.component.html',
    styleUrls: ['landingpage.component.scss'],
    providers:[GlobalVarsService]
})
export class LandingPageComponent implements OnInit {
    @ViewChild('thresholdModal') public thresholdModal: ModalDirective;
    @ViewChild('saveConfig') public saveConfig: ModalDirective;
    @ViewChild('compliantModal') public compliantModal: ModalDirective;
    @ViewChild('nonCompliantModal') public nonCompliantModal: ModalDirective;

    @ViewChildren(DataTableDirective)
    datatableElements: QueryList<DataTableDirective>;
    @ViewChild('CompliantTable') block: ElementRef;
    @ViewChild('NonCompliantTable') block1: ElementRef;
    dtOptions: DataTables.Settings = {};
    dtOptions2: DataTables.Settings ={};
    loading: boolean;
    dtTrigger: Subject<any> = new Subject();
    dtTrigger2: Subject<any> = new Subject();
    public period = '02/2019';
    gridsData: any = [];
    contName: any = [];
    contrctName:any=[];
    limitedData: any = [];
    bestContracts: any = [];
    KpiCompliants: any = [];
    KpiNonCompliants: any = [];
    monthVar: any;
    month: any;
    yearVar: any;
    contractName: any;
    count = 0;
    setViewAll = 0;
    gridLength = 0;
    thresholdkey = '@thresholdKey';
    thresholdkey1 = '@standardDashbroadContractparties';
    thresholdvalue = 0;
    thresholdvalue1;
    thresholdLength=0;
    showMultiSelect : boolean = false;
    orignalArray: any = [];
    myStyle = {
        'width': '40%',
        'position': 'absolute',
        'right': '13%',
        'top': '37px',
        'z-index': '9',
        height: 'auto'
    };
    selectedMonth = localStorage.getItem('month');
    constructor(
        private dashboardService: DashboardService,
        private apiService: ApiService,
        private _route: ActivatedRoute,
        private emitter: EmitterService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder,
        private dateTime: DateTimeService,
        public globalvar :GlobalVarsService
    ) { }
    ngOnInit(): void {

        this.thresholdvalue = 0;
        this.setViewAll=0;
        this.gridLength = 0;
        this.thresholdLength=0;

        this.apiService.getThresholdDetails(this.thresholdkey).subscribe((data: any) => {
            this.thresholdvalue = data;
        });

        this.thresholdvalue = 0;
        this.month = moment().format('MMMM');
        this.selectedMonth?this.monthVar = this.selectedMonth:this.monthVar = moment().format('MM');
        this.yearVar = moment().format('YYYY');
        this.getAnno();
        console.log(this.globalvar.getSelectedmonth(),'global selected month')
        this.loading = true;
        this.apiService.getLandingPage(this.monthVar, this.yearVar).subscribe((data: any) => {

            this.gridsData = data;

            if(this.gridsData == null){
                this.toastr.error("Nessun contraente assegnato all'utente");
                this.loading = false;
            }else{
                this.setViewAll=0;
                this.gridLength = this.gridsData.length;
                if(this.gridsData.length>6){
                    this.limitedData = this.gridsData.splice(0,6);
                  this.contName = this.limitedData;
                  this.myStyle.height = ((this.contName.length + 2) * 20) + 'px';
                    this.orignalArray = [...this.limitedData, ...this.gridsData]
                }else{
                    this.limitedData = this.gridsData;
                    this.orignalArray = this.gridsData;
                  this.contName = this.limitedData;
                  this.myStyle.height = ((this.contName.length + 2) * 20) + 'px';
                }
            }
            console.log("orignalArray -->", this.orignalArray);

            console.log("gridsData -> ", this.gridsData, this.limitedData);
            this.loading = false;
        },error =>{
            this.toastr.error("Nessun contraente assegnato all'utente");
        });

        this.dtOptions = {
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

        this.apiService.getThresholdDetails(this.thresholdkey1).subscribe((data: any) => {
            this.thresholdvalue1 = data;
            let split = this.thresholdvalue1.split(",");
            //this.limitedData = split;
            //console.log('limitedData -> ',this.limitedData);
            //this.customFilter1(this.thresholdvalue1);
            //console.log('thresholdvalue1 -> ',this.thresholdvalue1);
            
        });
    }

    ngAfterViewInit() {

            this.dtTrigger.next();
            this.dtTrigger2.next();
        //this.getCOnfigurations();
    }

    ngOnDestroy(): void {
        this.dtTrigger.unsubscribe();
        this.dtTrigger2.unsubscribe();
        localStorage.removeItem('month');
    }

    rerender(): void {
        /*this.datatableElement.dtInstance.then((dtInstance: DataTables.Api) => {
            // Destroy the table first
            dtInstance.destroy();
            // Call the dtTrigger to rerender again
                this.dtTrigger.next();
                setTimeout(() => {
                this.dtTri.next();

                }, 1000);
        });*/
        this.datatableElements.forEach((dtElement: DataTableDirective) => {
          dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
            dtInstance.destroy();
            this.dtTrigger.next();
            this.dtTrigger2.next();
          });
        });
    }

    populateDateFilter() {
        if (this.monthVar == null || this.yearVar == null) {
        } else {
            this.globalvar.setmonth(this.monthVar);
            this.setViewAll=0;
            this.loading = true;
            this.apiService.getLandingPage(this.monthVar, this.yearVar).subscribe((data: any) => {
                this.gridsData = data;
                this.gridLength = this.gridsData.length;
              this.contName = this.gridsData;
              this.myStyle.height = ((this.contName.length + 2) * 20) + 'px';
                if(this.gridsData.length==0){
                    this.toastr.error("Nessun contraente assegnato all'utente");
                    this.loading = false;
                }else{
                    this.gridLength = this.gridsData.length;
                    if(this.gridsData.length>6){
                      this.limitedData = this.gridsData.splice(0,6);
                      this.contName = this.limitedData;
                      this.myStyle.height = ((this.contName.length + 2) * 20) + 'px';
                      this.orignalArray = [...this.limitedData, ...this.gridsData]
                    }else{
                      this.limitedData = this.gridsData;
                      this.orignalArray = this.gridsData;
                      this.contName = this.limitedData;
                      this.myStyle.height = ((this.contName.length + 2) * 20) + 'px';
                    }
                }
                console.log("gridsData -> ", this.gridsData, this.limitedData);
                this.loading = false;
            });
        }
    }
    multiSelect(){
      this.showMultiSelect = (this.showMultiSelect) ? false : true;
    }

    async customFilter(){
        let value:any = this.contractName;
        this.thresholdvalue1 = this.contractName;
        this.thresholdLength=this.thresholdvalue1.length;

        console.log(this.contractName);
        if(value == 'ALL'){
            this.loading = true;
            if(this.setViewAll == 0){
                this.limitedData = this.contName
            }else{
                this.orignalArray = this.contName;
            }
            //this.limitedData = this.contName;
            //this.gridsData = this.contName;
            this.loading = false;
        }else{
            this.loading = true;
        var temp:any = this.contName
        var temp2:any = [];
        await value.forEach(async element => {
            await temp.forEach(ele =>  {
                let e = element.item_text?element.item_text:element
                if(ele.contractpartyname == e){
                temp2.push(ele);
                }else{}});
        });
        await temp2.forEach((val, i) => temp2[i] =  {

            bestcontracts: temp2[i].bestcontracts?temp2[i].bestcontracts:'',
            complaintcontracts: temp2[i].complaintcontracts,
            complaintkpis: temp2[i].complaintkpis,
            contractpartyid: temp2[i].contractpartyid,
            contractpartyname: temp2[i].contractpartyname,
            noncomplaintcontracts: temp2[i].noncomplaintcontracts,
            noncomplaintkpis: temp2[i].noncomplaintkpis,
            totalcontracts: temp2[i].totalcontracts,
            totalkpis: temp2[i].totalkpis,
            worstcontracts: temp2[i].worstcontracts
        })
        if(this.setViewAll == 0){
            this.limitedData = temp2;
        }else{
            this.orignalArray = temp2;
        }


        this.loading = false;
        }

     }

     async customFilter1(data){

        let value:any = this.contractName;
        if(value == 'ALL'){
            this.loading = true;
            if(this.setViewAll == 0){
              this.limitedData = data
            }else{
              this.orignalArray = data;
            }
            this.loading = false;
        }else{
            this.loading = true;
            var temp:any = data
            var temp2:any = [];
            await value.forEach(async element => {
                await temp.forEach(ele =>  {
                    let e = element.item_text?element.item_text:element
                    if(ele.contractname == e){
                    temp2.push(ele);
                    }else{}});
            });
            await temp2.forEach((val, i) => temp2[i] =  {

                bestcontracts: temp2[i].bestcontracts?temp2[i].bestcontracts:'',
                complaintcontracts: temp2[i].complaintcontracts,
                complaintkpis: temp2[i].complaintkpis,
                contractpartyid: temp2[i].contractpartyid,
                contractname: temp2[i].contractname,
                noncomplaintcontracts: temp2[i].noncomplaintcontracts,
                noncomplaintkpis: temp2[i].noncomplaintkpis,
                totalcontracts: temp2[i].totalcontracts,
                totalkpis: temp2[i].totalkpis,
                worstcontracts: temp2[i].worstcontracts
            })
            if(this.setViewAll == 0){
              this.limitedData = temp2;
            }else{
              this.orignalArray = temp2;
            }

            this.loading = false;
        }
     }

    anni = [];
    //+(moment().add('months', 6).format('YYYY'))
    getAnno() {
        for (var i = 2016; i <= +(moment().add(7,'months').format('YYYY')); i++) {
            this.anni.push(i);
        }
        return this.anni;
    }

    viewAll(){
        this.setViewAll=1;
      this.contName = this.orignalArray;
      this.myStyle.height = ((this.contName.length + 2) * 20) + 'px';
        this.showMultiSelect = false;
    }

    details(contractpartyid,contractpartyname) {
        let params = { contractpartyid: contractpartyid, contractpartyname: contractpartyname, month:this.monthVar, year:this.yearVar };
        window.open(`/#/dashboard/landing-page-details/?contractpartyid=${params.contractpartyid}&contractpartyname=${params.contractpartyname}&month=${params.month}&year=${params.year}`,"_self");
    }

    setThreshold() {
        this.apiService.AddUpdateUserSettings(this.thresholdkey, this.thresholdvalue).subscribe((data: any) => {
            this.toastr.success('Configurazione salvata con successo');
        }, error => {
          this.toastr.error('Error while updating threshold value');
        });
        this.hideThresholdModal();
    }
    
    setThreshold1() {
        this.apiService.AddUpdateUserSettings(this.thresholdkey1, this.thresholdvalue1).subscribe((data: any) => {
            this.toastr.success('Configurazione salvata con successo');
        }, error => {
          this.toastr.error('Error while updating threshold value');
        });
    }

    showThresholdModal() {
        this.thresholdModal.show();
    }

    hideThresholdModal() {
        this.thresholdModal.hide();
    }
    
    // showThresholdModal1() {
    //     this.saveConfig.show();
    // }

    // hideThresholdModal1() {
    //     this.saveConfig.hide();
    // }

    showCompliantModal(contractPartyId) {
        this.apiService.GetLandingPageKPIDetails(contractPartyId,this.monthVar,this.yearVar).subscribe((data: any) => {
            this.KpiCompliants = data.filter(a => a.result == 'compliant')
            this.rerender();
        });
        this.compliantModal.show();

    }

    hideCompliantModal() {
        this.compliantModal.hide();
    }

    showNonCompliantModal(contractPartyId) {
        this.apiService.GetLandingPageKPIDetails(contractPartyId,this.monthVar,this.yearVar).subscribe((data: any) => {
            this.KpiNonCompliants = data.filter(a => a.result == 'non compliant')
            console.log(this.KpiNonCompliants,'kpinonCompiant')
            // setTimeout(() => {
                this.rerender();
            // }, 1000);

        });
        this.nonCompliantModal.show();
    }

    hideNonCompliantModal() {
        this.nonCompliantModal.hide();
    }
}

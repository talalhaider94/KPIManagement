import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { LoadingFormService, AuthService } from '../../../_services';
import { first } from 'rxjs/operators';
import { FileUploader } from 'ng2-file-upload';
import { FileHelpersService } from '../../../_helpers';
import { Subject } from 'rxjs';
import { DataTableDirective } from 'angular-datatables';
import * as moment from 'moment';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { Observable, of, throwError } from 'rxjs';
import { delay, mergeMap, retryWhen } from 'rxjs/operators';
import { FormControl } from '@angular/forms';

@Component({
    selector: 'app-loading-form-csv',
    templateUrl: './loading-form-csv.component.html',
    styleUrls: ['./loading-form-csv.component.scss']
})
export class LoadingFormCsvComponent implements OnInit, OnDestroy {
  formId: string = null;
    loadingForms: any = [];
  detailsForms: any = {};
  loadingPattern: string = '';
  global_rule_id: 0;
    loading: boolean = true;
    @ViewChild(DataTableDirective)
    @ViewChild('showOnReady') showOnReady: ElementRef;
    @ViewChild('uploadCSV') public uploadCSV: ModalDirective;
    datatableElement: DataTableDirective;
    dtOptions: DataTables.Settings = {};
  dtTrigger = new Subject();
  fileUploadMonth: string[] = [];
  fileUploadYear: string[] = [];
  loadingAttachments: boolean = false;
  fileUploading: boolean = false;
  formAttachmentsArray: any = [];
  formAttachmentsArrayFiltered: any = [];
    public uploader: FileUploader = new FileUploader({ url: 'test' });
    constructor(
      private router: Router,
      private toastr: ToastrService,
        private loadingFormService: LoadingFormService,
        private authService: AuthService
    ) { }
  monthOption;
  yearOption;
  ngOnInit() {
    this.getAnno();
    const currentUser = this.authService.getUser();
    this.monthOption = moment().subtract(1, 'month').format('MM');
    this.yearOption = moment().format('YYYY');
    if (this.monthOption === '12') {
      this.yearOption = moment().subtract(1, 'year').format('YYYY');
    }
        $('#showOnReady').hide();
        // Danial TODO: Some role permission logic is needed here.
        // Admin and super admin can access this
        this.dtOptions = {
            pagingType: 'full_numbers',
            pageLength: 10,
            columnDefs: [
                { "visible": false, "targets": 0 }
            ],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;
                api.column(0, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        $(rows).eq(i).before(
                            '<tr style="background-color:#eedc00" class="group"><th colspan="6">' + group + '</th></tr>'
                        );
                        last = group;
                    }
                });
            },
            initComplete: function () {
                $('#showOnReady').show();
                $('#loadingDiv').hide();
                console.log(this)
            },
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
                    sortAscending: ":attiva per ordinare la colonna in ordine crescente",
                    sortDescending: ":attiva per ordinare la colonna in ordine decrescente"
                }
            }
        };
        
        // getLoadingForms()
        this.loadingFormService.getFormsByUserId(currentUser.userid, 'csvtracking').pipe(first()).subscribe(data => {
            console.log('getFormsByUserId', data);
            this.loadingForms = data;
            this.dtTrigger.next();

            //
            var groupBy = function (xs, key) {
                return xs.reduce(function (rv, x) {
                    //(rv[x[key]] = rv[x[key]] || []).push(x);
                    //(rv[x[key]] = rv[x[key]] || [])[x.form_id] = { form_name: x.form_name };
                    var index = (rv[x[key]] = rv[x[key]] || []).findIndex(e => e.form_id == x.form_id);
                    if (index === -1) {
                        (rv[x[key]] = rv[x[key]] || []).push(x);
                    }
                    return rv;
                }, {});
            };
            this.detailsForms = groupBy(data, 'global_rule_id')
            console.log(this.detailsForms)
        }, error => {
            console.error('getFormsByUserId', error);
            this.loading = false;
        })
    }

    ngOnDestroy() {
        this.dtTrigger.unsubscribe();
    }
    loadingCompleted() {
        this.loading = false;
    }
  populateCSVModal(row) {
    this.uploadCSV.show();
    console.log(row)
   // $('#uploadCSV').show();
  }
  hideuploadCSV() {
    this.uploadCSV.hide();
   // $('#uploadCSV').hide();
  }
    cutOffRow(row) {
        if (row.cutoff) {
            let currentDate = moment().format();
            let isDateBefore = moment(row.modify_date).isBefore(currentDate);
            if (isDateBefore) {
                return true;
            } else {
                return false;
            }
        } else {
            return false
        }
    }
    formatInputDate(date) {
        if (date) {
            if (moment(date).isSame(moment('0001-01-01T00:00:00'))) {
                return 'Nessun caricamento';
            } else {
                return moment(date).format('YYYY/MM/DD HH:mm:ss');
            }
        } else {
            return 'N/A';
        }
    }
  onFileSelected(event) {
    let period = moment().subtract(1, 'month').format('MM');
    let year = moment().format('YYYY');
    console.log(event);
    this.uploader.queue.forEach((file, index) => {
      this.fileUploadMonth.push(period);
      this.fileUploadYear.push(year);
    });
  }
  
  fileUploadUI() {
    if (this.uploader.queue.length > 0) {
      this.uploader.queue.forEach((element, index) => {
        let file = element._file;
        let uploadName = file.name;
        if (this.loadingPattern != null && this.loadingPattern.length > 5) {
          let patternArray = this.loadingPattern.split('_');
          let patternExtArray = patternArray[2].split('.');
          let patternExt = patternExtArray[1];

          let uploadNameArray = uploadName.split('_');
          let uploadExtArray = uploadNameArray[2] ? uploadNameArray[2].split('.') : null;
          let uploadExt = uploadExtArray != null ? uploadExtArray[1] : null;

          if (patternArray[0] == uploadNameArray[0]) {
            if (uploadNameArray[1].length == 6) {
              if (patternExt == uploadExt) {
                this._getUploadedFile(file, this.fileUploadMonth[index], this.fileUploadYear[index]);
              } else {
                //errore
                this.toastr.error('Nome file errato. Pattern: ' + this.loadingPattern)
              }
            } else {
              //errore
              this.toastr.error('Nome file errato. Pattern: ' + this.loadingPattern)
            }
          } else {
            //errore
            this.toastr.error('Nome file errato. Pattern: ' + this.loadingPattern)
          }
        } else {
          this.toastr.error('Pattern non impostato. Contattare un amministratore.')
        }
      });
    } else {
      this.toastr.info('Nessun documento da caricare');
    }
  }
  

  _getUploadedFile(file, month, year) {
    console.log(this.global_rule_id)
    let global_rule_id = this.global_rule_id;
    this.fileUploading = true;
    const reader: FileReader = new FileReader();
    reader.onloadend = (function (theFile, self) {
      let fileName = theFile.name;
      return function (readerEvent) {
        let formAttachments = { content: '', form_attachment_id: global_rule_id, form_id: global_rule_id, period: '', year: 0, name: '', checksum: ''};
        let binaryString = readerEvent.target.result;
        let base64Data = btoa(binaryString);
        let dateObj = self._getPeriodYear();
        formAttachments.content = base64Data;
        formAttachments.period = month + '/' + year;
        formAttachments.year = year;
        formAttachments.name = fileName;
        formAttachments.checksum = 'checksum';
        self.fileUploading = true;
        self.loadingFormService.submitCSV(formAttachments).pipe(self.delayedRetries(10000, 3)).subscribe(data => {
          console.log('submitAttachment ==>', data);
          self.fileUploading = false;
          self.removeFileFromQueue(fileName);
          //self.uploader.queue.pop();
          self.toastr.success(`${fileName} caricato correttamente.`);
          /*if (data) {
            self._getAttachmentsByFormIdEndPoint(+self.formId, false); per controllare il carimento corretto
          }*/
        }, error => {
          console.error('submitAttachment ==>', error);
          self.fileUploading = false;
          self.toastr.error('Errore durante il caricamento dell\'allegato');
        });
      };
    })(file, this);
    // reader.readAsDataURL(file); // returns file with base64 type prefix
    reader.readAsBinaryString(file); // return only base64 string
  }
  // move to helper later
  _getPeriodYear() {
    let currentDate = new Date();
    let period = moment(currentDate).subtract(1, 'month').format('MM');
    let year = Number(moment(currentDate).format('YYYY'));
    if (period === '12') {
      year = Number(moment(currentDate).subtract(1, 'year').format('YYYY'));
    }
    return { period, year };
  }

  onDataChange() {
    this.formAttachmentsArrayFiltered = this.formAttachmentsArray.filter(attachment => attachment.year == this.yearOption);
    this.formAttachmentsArrayFiltered = this.formAttachmentsArrayFiltered.filter(attachment => attachment.period == this.monthOption);
  }
  removeFileFromQueue(fileName: string) {
    for (let i = 0; i < this.uploader.queue.length; i++) {
      if (this.uploader.queue[i].file.name === fileName) {
        this.uploader.queue[i].remove();
        return;
      }
    }
  }
  _getAttachmentsByFormIdEndPoint(formId: number, shouldTrigger: boolean) {
    this.loadingAttachments = true;
    this.loadingFormService.getAttachmentsByFormId(formId).pipe().subscribe(data => {
      this.loadingAttachments = false;
      console.log('_getAttachmentsByFormIdEndPoint ==>', data);
      if (data) {
        this.formAttachmentsArray = data;
        this.onDataChange();
      }
    }, error => {
      this.loadingAttachments = false;
      console.error('_getAttachmentsByFormIdEndPoint ==>', error);
      this.toastr.error('Errore durante la lettura degli allegati.');
    })
  }
  anni = [];
  getAnno() {
    for (let i = 2016; i <= +(moment().add('months', 7).format('YYYY')); i++) {
      this.anni.push(i);
    }
    return this.anni;
  }
  delayedRetries(delayMs: number, maxRetry: number) {
    let retries = maxRetry;
    return (src: Observable<any>) => src.pipe(retryWhen((errors: Observable<any>) => errors.pipe(
      delay(delayMs),
      mergeMap(error => retries-- > 0 ? of(error) : throwError(`Tried to upload ${maxRetry} times. without success.`))
    )
    ))
  }
  formatInputDateIT(date) {
    if (date) {
      if (moment(date).isSame(moment('0001-01-01T00:00:00'))) {
        return 'Nessun caricamento';
      } else {
        return moment(date).format('DD/MM/YYYY HH:mm:ss');
      }
    } else {
      return 'N/A';
    }
  }
}

import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { FileUploader } from 'ng2-file-upload';
import { FormAttachments, FormField, UserSubmitLoadingForm, Form, FileUploadModel } from '../../../_models';
import * as moment from 'moment';
import { LoadingFormService, AuthService } from '../../../_services';
import { FileHelpersService } from '../../../_helpers';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from "@angular/router";
import Swal from 'sweetalert2';
import { Observable, of, throwError } from 'rxjs';
import { delay, mergeMap, retryWhen } from 'rxjs/operators';

export class FormClass {
    form_id: number;
    // form_body: json;
    form_body: any;
}
// means field control
export class ControlloCampo {
    min: any;
    max: any;
}
// means comparison check
export class ControlloConfronto {
    campo1: any = '';
    segno: string = '';
    campo2: any = '';
}

const URL = 'https://evening-anchorage-3159.herokuapp.com/api/';

@Component({
    selector: 'app-prove-varie',
    templateUrl: './prove-varie.component.html',
    styleUrls: ['./prove-varie.component.scss']
})
export class ProveVarieComponent implements OnInit {
    formId: string = null;
    formName: string = null;
    formRulesBody: [] = [];
    comparisonRulesBody: [] = [];
    loading: boolean = false;
    formElementsLoading: boolean = false;
    displayUserFormCheckBox: boolean = false;
    formAttachmentsArray: any = [];
    formAttachmentsArrayFiltered: any = [];
    userLoadingFormErrors: string[] = [];
    fileUploading: boolean = false;
    cutOff: boolean = false;
    day_cutoff: number;
    modifyDate: Date;
    readOnlyUserForm: boolean = true;
    public uploader: FileUploader = new FileUploader({ url: URL });
    public listaKpiPerForm = [];
    defaultFont = [];
    public myInputForm: FormGroup;
    private files: Array<FileUploadModel> = [];
    // for filters use of arrays to be able to declare e
    // save several module variables, in this way
    // I index and make possible the dynamism of the filter fields
    // I use it for both i numbers and strings
    numeroMax: number[] = [];
    numeroMin: number[] = [];
    // filto per le date
    maxDate: any[] = [];
    minDate: any[] = [];
    actionOnSubmit: string = '';
    dt: any[] = [];
    numero: number[] = [];
    stringa: string[] = [];
    daje: boolean = false;

    isAdmin: boolean = false;

    erroriArray: string[] = [];
    arraySecondo = new Array;
    confronti: string[] = ['<', '>', '=', '!=', '>=', '<='];
    arrayFormElements: any = [];

    jsonForm: any = [];
    defaultTimestamp;
    numeroForm: number;
    title: string = '';
    checked: boolean;
    Is_Dato_Mancante: boolean = false;
    displayComparisonRules: string[] = [];
    isCollapsed = true;
    fileUploadMonth: string[] = [];
    fileUploadYear: string[] = [];
    loadingAttachments: boolean = false;
    constructor(
        private fb: FormBuilder,
        private loadingFormService: LoadingFormService,
        private toastr: ToastrService,
        private activatedRoute: ActivatedRoute,
        private authService: AuthService,
        private router: Router,
        private fileHelper: FileHelpersService
    ) { }
    monthOption;
    yearOption;
    ngOnInit() {
        this.getAnno();
        const currentUser = this.authService.getUser();
        this.monthOption = moment().subtract(1, 'month').format('MM');
        this.defaultTimestamp = moment().subtract(1, 'month').endOf('month').toDate();
        this.yearOption = moment().format('YYYY');
        if (this.monthOption === '12') {
            this.yearOption = moment().subtract(1, 'year').format('YYYY');
        }

        this.isAdmin = currentUser.isadmin;
        this.activatedRoute.paramMap.subscribe(params => {
            this.formId = params.get("formId");
            this.formName = params.get("formName");
            this._init(+this.formId, this.formName);
            this._getAttachmentsByFormIdEndPoint(+this.formId, true);
        });
    }
    onCheck() {
        console.log(this.Is_Dato_Mancante);
    }
    initInputForm() {
        return this.fb.group({
            valoreUtente: [''], // user value
            valoreMin: [''], // min value
            valoreMax: [''] // max value
        })
    }

    addInputForm() {
        const control = <FormArray>this.myInputForm.controls['valories'];
        control.push(this.initInputForm());
    }

    initComparisonForm(array) {
        return this.fb.group({
            campo1: [{ value: array.campo1, disabled: false }], // means field
            segno: [{ value: array.segno, disabled: false }],
            campo2: [{ value: array.campo2, disabled: false }]
        })
    }

    _mapFormValuesWithFields(formFields, formValues) {
        return formValues.map((value, index) => {
            return {
                name: formFields[index].name,
                type: formFields[index].type,
                value: value.valoreUtente || ''
            }
        });
    }
    after_submit(action) {
        this.actionOnSubmit = action;
    }
    saveUser(model: any) {
        let utenteFormData;
        if (!!model.value.termsCheck) {
            utenteFormData = this._mapFormValuesWithFields(this.arrayFormElements, model.value.valories)
                .find(field => field.name == 'Note' && !!field.value);

            if (!utenteFormData) {
                // The Notes field is mandatory because Missing Data has been selected
                this.toastr.info('Il campo Note è obbligatorio perchè è stato selezionato "Dato Mancante"');
                return false;
            }
            //  else {
            //   // passing NOTE input field value.
            //   this._noteTextFileUpload(utenteFormData.value);
            // }
        }

        var formFields: FormField;
        var userSubmit: UserSubmitLoadingForm = new UserSubmitLoadingForm();
        var dataAttuale = new Date();
        dataAttuale.setMonth((dataAttuale.getMonth()) - 1);
        if (dataAttuale.getMonth() == 12) {
            dataAttuale.setFullYear(dataAttuale.getFullYear() - 1);
        }
        var periodRaw = moment(dataAttuale).format('MM/YY');
        const errorArray = this._customFormRulesValidation(this.arrayFormElements, model.value.valories, this.formRulesBody);
        // errorArray empty means Either all rules passed
        if (errorArray.length > 0) {
            this.userLoadingFormErrors = errorArray;
            this.toastr.error('Form non valido');
            return false;
        } else {
            const errorArray = this._comparisonRulesValidation(this.arrayFormElements, model.value.valories, this.comparisonRulesBody);
            let newArray = errorArray.filter(value => value !== 'remove');
            if (newArray.length > 0) {
                this.userLoadingFormErrors = newArray;
                this.toastr.info('Errore durante la compilazione del form');
                return false;
            } else {
                this.userLoadingFormErrors = [];
            }
        }

        //part where I fill in field values
        this.arrayFormElements.forEach((element, index) => {
            formFields = new FormField;
            if (element.type != 'Label') {
                formFields.FieldName = element.name;
                formFields.FieldType = element.type;
                switch (element.type) {
                    case 'time':
                        formFields.FieldValue = this.dt[index] || '';
                        break;
                    case 'string':
                        if (element.name == 'Is_Dato_Mancante' || element.name == 'Dato_Mancante') {
                            if (this.Is_Dato_Mancante === true) {
                                formFields.FieldValue = '1';
                            } else {
                                formFields.FieldValue = '0';
                            }
                        } else {
                            formFields.FieldValue = this.stringa[index] || '';
                        }
                        break;
                    default:
                        // for real and integer
                        if (element.name == 'Is_Dato_Mancante' || element.name == 'Dato_Mancante') {
                            if (this.Is_Dato_Mancante === true) {
                                formFields.FieldValue = '1';
                            } else {
                                formFields.FieldValue = '0';
                            }
                        } else {
                            formFields.FieldValue = String(this.numero[index] || '');
                        }
                        break;
                }
                userSubmit.inputs.push(formFields);
            }
        });
        if (utenteFormData) {
            this._noteTextFileUpload(utenteFormData.value);
        }
        this._userLoadingFormSubmit(periodRaw, dataAttuale, userSubmit);
    }

    _userLoadingFormSubmit(periodRaw: any, dataAttuale, userSubmit) {
        this.loading = true;
        const currentUser = this.authService.getUser();
        userSubmit.locale_id = currentUser.localeid;
        userSubmit.form_id = +this.numeroForm;
        userSubmit.user_id = currentUser.userid;
        userSubmit.empty_form = false;
        userSubmit.period = String(periodRaw);
        userSubmit.year = Number(moment(dataAttuale).format('YY'));
        this.loadingFormService.submitForm(userSubmit).subscribe(data => {
            this.loading = false;
            console.log('USER FORM SUBMIT SUCCESS', data);
            if (!data) {
                this.toastr.error('Errore durante l\'invio del form', 'Error');
                return;
            } else {
                this.userLoadingFormErrors = [];
                this.toastr.success('Form inviato correttamente.', 'Success');
                if (this.actionOnSubmit == 'saveExit') {
                    this.router.navigate(['/loading-form/utente']);
                }
            }
        }, error => {
            this.loading = false;
            this.toastr.error(error.statusText, 'Error');
        });
    }

    // generates form fields dynamically
    _init(numero: number, nome: string) {
        this.loading = true;
        this.formElementsLoading = true;
        // since I put the filters first by comparison,
        // when the max and min filters go I go to
        // subtract the number of past comparisons from the index
        // camp means field and segno means sign
        let array = { 'campo1': '', 'segno': '', 'campo2': '' };
        this.title = nome;

        this.myInputForm = this.fb.group({
            valories: this.fb.array([
                this.initInputForm()
            ]),
            termsCheck: '',
        });

        this.loadingFormService.getKpiByFormId(numero).subscribe(data => {
            console.log('getKpiByFormId', data);
            if (!!data && typeof data === "object" && !Array.isArray(data)) {
                this.listaKpiPerForm.push(data);
            } else if (!!data && Array.isArray(data)) {
                data.forEach(element => {
                    this.listaKpiPerForm.push(element);
                });
            } else {
                this.toastr.info('Nessun KPI Assegnato al Loading Form');
            }
            this.loading = false;
        }, error => {
            this.loading = false;
            this.toastr.error(error.error.error.message, 'KPI Form Error');
            console.log('getKpiByFormId', error)
        });

        this.loadingFormService.getFormRuleByFormId(numero).subscribe(data => {
            this.loading = false;
            console.log('getFormRuleByFormId', data);
            if (data) {
                let formBody = JSON.parse(data.form_body);
                let formRules = formBody.formRules;
                let comparisonRulesBody = formBody.comparisonRules;
                this.comparisonRulesBody = comparisonRulesBody;
                this.formRulesBody = formRules;
                if (comparisonRulesBody) {
                    this.displayComparisonRules = comparisonRulesBody.map(compRule => {
                        return `${compRule.campo1.name} ${compRule.segno} ${compRule.campo2.name}`
                    })
                }
                if (formRules) {
                    formRules.forEach((rule, index) => {
                        if (rule.type == 'time') {
                            this.maxDate[index] = rule.rule.max;
                            this.minDate[index] = rule.rule.min;
                        } else {
                            this.numeroMax[index] = rule.rule.max;
                            this.numeroMin[index] = rule.rule.min;
                        }
                    });
                }
            }
        }, error => {
            this.loading = false;
            this.toastr.error(error.error.error.message, 'Form Rule Error');
            console.log('getFormRuleByFormId', error)
        });
        // if there are no filters for this form I create an empty field
        if (array.campo1 == "" && array.segno == "" && array.campo2 == "") {
            this.initComparisonForm(array);
            console.log('IF FIELDS ARE EMPTY:', array);
        } else {
            console.log(array);
        }

        this.loadingFormService.getFormById(numero).subscribe(data => {
            this.loading = false;
            this.formElementsLoading = false;
            // mutiple forms values are coming for single form Id so picking first one.
            this.jsonForm = data[0];
            console.log('DYNAMIC FORM FIELDS : jsonForm', this.jsonForm);
            this.cutOff = data[0].cutoff;
            this.day_cutoff = data[0].day_cutoff;
            this.modifyDate = data[0].modify_date;
            const lastSubmittedDate = data[0].latest_input_date;
            if (moment(lastSubmittedDate).isSame(moment(), 'month')) {
                Swal.fire({
                    type: 'info',
                    html: `E' già stato inserito un valore nel periodo di rilevazione corrente. Sei sicuro di voler proseguire?`,
                    showCloseButton: true,
                    focusConfirm: false,
                    confirmButtonText:
                        'Chiudi',
                })
            }
            if (data[0].cutoff) {
                let currentDate = moment().format();
                let isDateBefore = moment(data[0].day_cutoff).isBefore(currentDate);
                if (isDateBefore) {
                    this.readOnlyUserForm = false;
                }
            }
            this.readOnlyUserForm = data[0].cutoff ? false : true; // readOnly is mentally inverted
            this.arrayFormElements = data[0].reader_configuration.inputformatfield;
            console.log('this.arrayFormElements', this.arrayFormElements);
            for (let i = 0; i < this.arrayFormElements.length - 1; i++) {
                this.addInputForm();
            }
            var indexOfTimestamp = this.arrayFormElements.findIndex(field => (field.name === 'Timestamp') && (field.type === 'time'));
            if (indexOfTimestamp) {
                this.dt[indexOfTimestamp] = this.defaultTimestamp;
            }
            this.numeroForm = numero;
            // CHECK BOX DISPLAY CONDITION START
            const checkboxFieldExists = this.arrayFormElements.find(field => (field.name === 'Dato_Mancante' || field.name === 'Is_Dato_Mancante') && (field.type === 'integer' || field.type === 'string'));
            if (checkboxFieldExists) {
                this.displayUserFormCheckBox = true;
            }
            // CHECK BOX DISPLAY CONDITION END
        }, error => {
            this.loading = false;
            this.formElementsLoading = false;
            this.toastr.error(error.error.message, 'Error')
            console.log('getFormById', error)
        });
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

    _comparisonRulesValidation(formElements, formValues, comparisonRules) {
        if (!comparisonRules.length) {
            return comparisonRules;
        }
        const mapFormValues = this._mapFormValuesWithFields(formElements, formValues);
        const invalidRules = comparisonRules.map((compare, index) => {
            let type = compare.campo1.type;
            let field1 = compare.campo1;
            let field2 = compare.campo2;
            let sign = compare.segno;
            let currentFormValue1 = mapFormValues.find(value => value.name == field1.name).value;
            let currentFormValue2 = mapFormValues.find(value => value.name == field2.name).value;
            let errorString = 'remove';
            if (type == 'string') {
                // string comparison start
                if (sign === '=') {
                    if (currentFormValue1 !== currentFormValue2) {
                        errorString = `${field1.name} deve essere uguale a ${field2.name}`;
                    }
                } else if (sign === '!=') {
                    if (currentFormValue1 === currentFormValue2) {
                        errorString = `${field1.name} deve essere diverso da ${field2.name}`;
                    }
                } else {
                    errorString = 'Unknown string comparison';
                    console.log('Unknown sign type in string comparison.')
                }
                // string comparison end
            } else if (type == 'time') {
                // time comparison start
                if (sign === '>') {
                    let result = moment(new Date(currentFormValue1)).isAfter(new Date(currentFormValue2));
                    if (!result) {
                        errorString = `${field1.name} deve essere maggiore di ${field2.name}`;
                    }
                }
                else if (sign === '<') {
                    let result = moment(new Date(currentFormValue1)).isBefore(new Date(currentFormValue2));
                    if (!result) {
                        errorString = `${field1.name} deve essere minore di ${field2.name}`;
                    }
                }
                else if (sign === '>=') {
                    let result = moment(new Date(currentFormValue1)).isSameOrAfter(new Date(currentFormValue2));
                    if (!result) {
                        errorString = `${field1.name} deve essere maggiore o uguale a ${field2.name}`;
                    }
                }
                else if (sign === '<=') {
                    let result = moment(new Date(currentFormValue1)).isSameOrBefore(new Date(currentFormValue2));
                    if (!result) {
                        errorString = `${field1.name} deve essere minore o uguale a ${field2.name}`;
                    }
                } else {
                    errorString = 'Unknown time comparison';
                }
                // time comparison end
            } else if (type == 'real' || type == 'integer') {
                // real & integer comparison start
                if (sign === '=') {
                    if (currentFormValue1 !== currentFormValue2) {
                        errorString = `${field1.name} deve essere uguale a ${field2.name}`;
                    }
                } else if (sign === '!=') {
                    if (currentFormValue1 === currentFormValue2) {
                        errorString = `${field1.name} deve essere diverso da ${field2.name}`;
                    }
                }
                else if (sign === '>') {
                    if (currentFormValue1 < currentFormValue2) {
                        errorString = `${field1.name} deve essere maggiroe di ${field2.name}`;
                    }
                }
                else if (sign === '<') {
                    if (currentFormValue1 > currentFormValue2) {
                        errorString = `${field1.name} deve essere minore di ${field2.name}`;
                    }
                }
                else if (sign === '>=') {
                    if (currentFormValue1 <= currentFormValue2) {
                        errorString = `${field1.name} deve essere maggiore o uguale a ${field2.name}`;
                    }
                }
                else if (sign === '<=') {
                    if (currentFormValue1 >= currentFormValue2) {
                        errorString = `${field1.name} deve essere minore o uguale a ${field2.name}`;
                    }
                } else {
                    errorString = 'Unknown real/integer operator';
                }
                // real & integer comparison send
            } else {
                errorString = 'Unknown comparison';
            }
            return errorString || 'remove';
        });
        return invalidRules;
    }

    _customFormRulesValidation(formElements, formValues, formRules) {
        // return if formRules are empty/not set
        if (!formRules.length) {
            return formRules;
        }
        let rulesNotNull = formRules.filter(obj => ((!!obj.rule.min && !!obj.rule.max)));
        if (!rulesNotNull.length) {
            // if form rules are empty then then there should be atleat one form field filled
            let atLeastOneField = formValues.filter(value => !!value.valoreUtente).length;
            if (!!atLeastOneField) {
                return [];
            } else {
                return ['Nessun campo compilato nel Loading Form'];
            }
        }
        const inValidRulesArray = formRules.filter((value, index) => {
            if (value.type === 'string') {
                let rule = value.rule;
                let ruleMin = rule.min;
                let ruleMax = rule.max;
                if (!ruleMin && !ruleMax) { return false }; // if string rules are empty dont return error
                const formStringValue = formValues[index];
                if ((formStringValue.valoreUtente && formStringValue.valoreUtente.length >= ruleMin) && (formStringValue.valoreUtente && formStringValue.valoreUtente.length <= ruleMax)) {
                    return false;
                } else {
                    return true;
                }
            } else if (value.type === 'time') {
                let rule = value.rule;
                let ruleMin = rule.min;
                let ruleMax = rule.max;
                if (!ruleMin && !ruleMax) { return false };
                if (!rule.min) {
                    ruleMin = moment(new Date('1970-01-01'), 'YYYY-MM-DD');
                } else {
                    ruleMin = moment(new Date(rule.min), 'YYYY-MM-DD');
                }
                if (!rule.max) {
                    ruleMax = moment(new Date('2970-01-01'), 'YYYY-MM-DD');
                } else {
                    ruleMax = moment(new Date(rule.max), 'YYYY-MM-DD');
                }
                const formDateValue = formValues[index];
                let isBetween = moment(formDateValue.valoreUtente, 'YYYY-MM-DD').isBetween(ruleMin, ruleMax);
                return isBetween ? false : true;
            } else if (value.type === 'real' || value.type === 'integer') {
                //type real or integer
                let rule = value.rule;
                let ruleMin = rule.min;
                let ruleMax = rule.max;
                if (!ruleMin && !ruleMax) { return false };
                const formRealValue = formValues[index];
                if ((formRealValue.valoreUtente >= ruleMin) && formRealValue.valoreUtente <= ruleMax) {
                    return false;
                } else {
                    return true;
                }
            } else {
                console.log('VALIDATION NOT HANDLES FOR VALUE', value);
            }
        });
        return inValidRulesArray.map(invalidField => {
            return `${invalidField.name} Input non valido`;
        })
    }

    downloadFile(base64Data, fileName) {
        this.fileHelper.downloadFile(base64Data, fileName);
    }

    onFileSelected(event) {
        let period = moment().subtract(1, 'month').format('MM');
        let year = moment().format('YYYY');
        this.uploader.queue.forEach((file, index) => {
            this.fileUploadMonth.push(period);
            this.fileUploadYear.push(year);
        });
    }

    fileUploadUI() {
        if (this.uploader.queue.length > 0) {
            this.uploader.queue.forEach((element, index) => {
                let file = element._file;
                this._getUploadedFile(file, this.fileUploadMonth[index], this.fileUploadYear[index]);
            });
        } else {
            this.toastr.info('Nessun documento da caricare');
        }
    }

    _getUploadedFile(file, month, year) {
        this.fileUploading = true;
        const reader: FileReader = new FileReader();
        reader.onloadend = (function (theFile, self) {
            let fileName = theFile.name;
            return function (readerEvent) {
                let formAttachments: FormAttachments = new FormAttachments();
                let binaryString = readerEvent.target.result;
                let base64Data = btoa(binaryString);
                let dateObj = self._getPeriodYear();
                formAttachments.content = base64Data;
                formAttachments.form_attachment_id = 0;
                formAttachments.form_id = +self.formId;
                formAttachments.period = month;
                formAttachments.year = year;
                formAttachments.doc_name = fileName;
                formAttachments.checksum = 'checksum';
                self.fileUploading = true;
                self.loadingFormService.submitAttachment(formAttachments).pipe(self.delayedRetries(10000, 3)).subscribe(data => {
                    console.log('submitAttachment ==>', data);
                    self.fileUploading = false;
                    self.removeFileFromQueue(fileName);
                    //self.uploader.queue.pop();
                    self.toastr.success(`${fileName} caricato correttamente.`);
                    if (data) {
                        self._getAttachmentsByFormIdEndPoint(+self.formId, false);
                    }
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

    anni = [];
    getAnno() {
        for (let i = 2016; i <= +(moment().add('months', 7).format('YYYY')); i++) {
            this.anni.push(i);
        }
        return this.anni;
    }

    _noteTextFileUpload(textString) {
        const blob = new Blob([textString], { type: "text/plain;charset=utf-8" });
        const reader = new FileReader();
        reader.readAsDataURL(blob);
        reader.onloadend = (function (self) {
            let fileName = `Nota-Dato-Mancante-${moment().format('DD-MM-YYYY,h:mm:ss')}.txt`;
            return function (readerEvent) {
                let formAttachments: FormAttachments = new FormAttachments();
                let binaryString = readerEvent.target.result;
                let base64Data = binaryString.substring(37);
                let dateObj = self._getPeriodYear();
                formAttachments.content = base64Data;
                formAttachments.form_attachment_id = 0;
                formAttachments.form_id = +self.formId;
                formAttachments.period = dateObj.period;
                formAttachments.year = dateObj.year;
                formAttachments.doc_name = fileName;
                formAttachments.checksum = 'checksum';
                self.loadingFormService.submitAttachment(formAttachments).pipe().subscribe(data => {
                    console.log('submitAttachment ==>', data);
                    self.fileUploading = false;
                    self.uploader.queue.pop();
                    self.toastr.success(`${fileName} caricato correttamente.`);
                    if (data) {
                        self._getAttachmentsByFormIdEndPoint(+self.formId, false);
                    }
                }, error => {
                    console.error('submitAttachment ==>', error);
                    self.fileUploading = false;
                    self.toastr.error('Errore durante il caricamento del file');
                });
            };
        })(this);
    }

    removeFileFromQueue(fileName: string) {
        for (let i = 0; i < this.uploader.queue.length; i++) {
            if (this.uploader.queue[i].file.name === fileName) {
                this.uploader.queue[i].remove();
                return;
            }
        }
    }

    delayedRetries(delayMs: number, maxRetry: number) {
        let retries = maxRetry;
        return (src: Observable<any>) => src.pipe(retryWhen((errors: Observable<any>) => errors.pipe(
            delay(delayMs),
            mergeMap(error => retries-- > 0 ? of(error) : throwError(`Tried to upload ${maxRetry} times. without success.`))
        )
        ))
    }
}

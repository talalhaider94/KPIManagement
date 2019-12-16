import { Component, OnInit, ViewChild, ElementRef, Output, Input, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { Form } from '../../../_models';
import { LoadingFormService, AuthService } from '../../../_services';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from "@angular/router";

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
    campo1: any;
    segno: string = '';
    campo2: any;
}

@Component({
    selector: 'app-loading-form-admin',
    templateUrl: './loading-form-admin.component.html',
    styleUrls: ['./loading-form-admin.component.scss']
})
export class LoadingFormAdminComponent implements OnInit {
    formId: string = null;
    formName: string = null;
    id: number = 0;
    loading: boolean = false;
    public listaKpiPerForm = [];
    defaultFont = [];
    public myInputForm: FormGroup;
    // private files: Array<FileUploadModel> = [];
    // for filters use of arrays to be able to declare e
    // save several module variables, in this way
    // I index and make possible the dynamism of the filter fields
    // I use it for both i numbers and strings
    numeroMax: number[] = [];
    numeroMin: number[] = [];
    // filto per le date
    maxDate: any[] = [];
    minDate: any[] = [];

    dt: any[] = [];
    numero: number[] = [];
    stringa: string[] = [];
    daje: boolean = false;

    isAdmin: boolean = false;

    erroriArray: string[] = [];
    arraySecondo = new Array;
    confronti = new Array;

    stringTypeRules: string[] = ['=', '!='];
    integerTypeRules: string[] = ['<', '>', '=', '!=', '>=', '<='];
    dateTypeRules: string[] = ['<', '>', '>=', '<='];
    arrayFormElements: any = [];
    jsonForm: any = [];
    numeroForm: number;
    title: string = '';
    checked: boolean;

    comparisonField1: any = [];
    comparisonField2: any = [];
    comparisonField3: any = [];

    constructor(
        private fb: FormBuilder,
        private loadingFormService: LoadingFormService,
        private toastr: ToastrService,
        private activatedRoute: ActivatedRoute,
        private authService: AuthService,
        private router: Router,
    ) { }

    ngOnInit() {
        const currentUser = this.authService.getUser();
        this.isAdmin = currentUser.isadmin;
        //this.tornaIndietro(); // means come back
        this.activatedRoute.paramMap.subscribe(params => {
            this.formId = params.get("formId");
            this.formName = params.get("formName");
            this._init(+this.formId, this.formName);
        });
    }
    initInputForm() {
        return this.fb.group({
            valoreUtente: [''], // user value
            valoreMin: [''], // min value
            valoreMax: [''] // max value
        })
    }

    addInputForm() {
        console.log('addInputForm this.myInputForm 1', this.myInputForm);
        const control = <FormArray>this.myInputForm.get('valories')['controls'];
        control.push(this.initInputForm());
    }

    initComparisonForm(array) {
        return this.fb.group({
            campo1: [{ value: array.campo1, disabled: false }],
            segno: [{ value: array.segno, disabled: false }],
            campo2: [{ value: array.campo2, disabled: false }]
        });
    }

    addComparisonForm(array) {
        if (array == '') {
            array = new ControlloConfronto;
        }
        const control = <FormArray>this.myInputForm.get('campiConfronto'); // means comparison fields
        control.push(this.initComparisonForm(array));
    }

    removeComparisonForm(i: number) {
        const control = <FormArray>this.myInputForm.get('campiConfronto');
        // check remove later :/
        control.removeAt(i);
    }

    filtraElementi(formField, index: number) {
        this.arraySecondo[index] = this.arrayFormElements.filter(form => (form.name !== formField.name && form.type === formField.type));
        if (formField.type === 'string') {
            this.confronti[index] = this.stringTypeRules;
        } else if (formField.type === 'integer' || formField.type === 'real') {
            this.confronti[index] = this.integerTypeRules;
        } else if (formField.type === 'time') {
            this.confronti[index] = this.dateTypeRules;
        } else {
            console.log('Danial: Unknown/New CONDITION MISSING IN filtraElementi method')
        }
    }

    save(model: any) {
        this.loading = true;
        console.log('ADMIN FORM SAVE MODEL', model.value);
        //elementi da mettere nel json
        // items to put in the json
        // let jsonDaPassare:json;
        let jsonDaPassare: any; // means json to pass
        let arrayControlli = []; // controls array
        let comparisonFieldsArray = [];
        // confrontoAppoggio = means comparison support || ControlloConfronto = means comparison check
        let confrontoAppoggio = new ControlloConfronto;
        let controlloCampo = new ControlloCampo; //means fields control

        this.erroriArray = [];
        // datiModelConfronto means  DataModel Comparison
        // campiConfronto means comparison fields
        let datiModelConfronto = model.value.campiConfronto;
        datiModelConfronto.forEach((element, index) => {
            if (!!element.campo1 && !!element.campo1.name && !!element.segno && !!element.campo2 && !!element.campo2.name) {
                confrontoAppoggio = new ControlloConfronto;
                confrontoAppoggio.campo1 = element.campo1;
                confrontoAppoggio.segno = element.segno;
                confrontoAppoggio.campo2 = element.campo2;
                comparisonFieldsArray.push(confrontoAppoggio);
            }
        });
        if (this.erroriArray.length > 0) {
            this.toastr.error('Please fill the form correctly.', 'Error');
            return;
        } else {
            console.log('NO ERRORS IN FORMS');
            this.arrayFormElements.forEach((element, index) => {
                console.log('Elements :', element);
                controlloCampo = new ControlloCampo;
                if (element.type == 'time') {
                    controlloCampo.max = this.maxDate[index] || '';
                    controlloCampo.min = this.minDate[index] || '';
                } else {
                    controlloCampo.max = this.numeroMax[index] || '';
                    controlloCampo.min = this.numeroMin[index] || '';
                }
                arrayControlli.push({ ...element, rule: controlloCampo });
            });
        }
        console.log('arrayControlli :', arrayControlli);
        let formToSend = new Form;
        formToSend.form_id = this.numeroForm;
        let fullFormData = { formRules: arrayControlli, comparisonRules: comparisonFieldsArray };
        formToSend.form_body = JSON.stringify(fullFormData);
        console.log('FINAL SENDING FORM', formToSend);
        this.loadingFormService.createForm(formToSend).subscribe(data => {
            this.toastr.success('Form has been submitted', 'Success');
            console.log('FINAL CREATE FORM SUCCESS', data);
            this.loading = false;
            this.router.navigate(['/loading-form/admin']);
        }, error => {
            this.toastr.error(error.message, 'Error');
            console.error('FINAL CREATE FORM ERROR', error);
            this.loading = false;
        });
    }
    // means check comparison
    // segno means sign
    checkConfronto(val1, val2, segno, elemento1, elemento2) {
        console.log('checkConfronto val1', val1);
        console.log('checkConfronto val2', val2);
        console.log('checkConfronto segno', segno);
        console.log('checkConfronto elemento1', elemento1);
        console.log('checkConfronto elemento2', elemento2);
        switch (segno) {
            case '=':
                if (val1 == val2) {
                } else {
                    this.erroriArray.push(elemento1.name + " deve essere uguale a " + elemento2.name);
                }
                break;
            case '!=':
                if (val1 != val2) {
                } else {
                    this.erroriArray.push(elemento1.name + " deve essere diverso di " + elemento2.name);
                }
                break;
            case '<':
                if (val1 < val2) {
                } else {
                    this.erroriArray.push(elemento1.name + " deve essere minore di " + elemento2.name);
                }
                break;
            case '>':
                if (val1 > val2) {
                } else {
                    this.erroriArray.push(elemento1.name + " deve essere maggiore di " + elemento2.name);
                }
                break;
            case '>=':
                if (val1 >= val2) {
                } else {
                    this.erroriArray.push(elemento1.name + " deve essere maggiore o uguale a " + elemento2.name);
                }
                break;
            case '<=':
                if (val1 <= val2) {
                } else {
                    this.erroriArray.push(elemento1.name + " deve essere minore o uguale a " + elemento2.name);
                }
                break;
        }
    }

    _init(numero: number, nome: string) {
        this.loading = true;
        this.title = nome;

        let array = { 'campo1': '', 'segno': '', 'campo2': '' };
        this.myInputForm = this.fb.group({
            // means values
            valories: this.fb.array([
                this.initInputForm()
            ]),
            // means comparison fields
            campiConfronto: this.fb.array([
                // this.initComparisonForm(array)
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
            }
            this.loading = false;
        }, error => {
            this.loading = false;
            setTimeout(() => this.toastr.error(error.error.error.message, 'KPI Form Error'), 2000);
            console.log('getKpiByFormId', error)
        });

        this.loadingFormService.getFormRuleByFormId(numero).subscribe(data => {
            this.loading = false;
            console.log('getFormRuleByFormId', data);
            if (data) {
                let formbody = JSON.parse(data.form_body);
                let formRules = formbody.formRules;
                let comparisonRules = formbody.comparisonRules;
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
                if (comparisonRules) {
                    for (let i = 0; i < comparisonRules.length; i++) {
                        this.comparisonField1[i] = comparisonRules[i].campo1;
                        this.comparisonField2[i] = comparisonRules[i].segno;
                        this.comparisonField3[i] = comparisonRules[i].campo2;
                        console.log('comparisonRules[i]', comparisonRules[i])
                        this.addComparisonForm(comparisonRules[i]);
                    }
                } else {
                    this.addComparisonForm(array);
                }
            }
        }, error => {
            this.loading = false;
            setTimeout(() => this.toastr.error(error.error.error.message, 'Form Rule Error'), 0);
            console.log('getFormRuleByFormId', error)
        });

        this.loadingFormService.getFormById(numero).subscribe(data => {
            this.loading = false;
            this.jsonForm = data;
            // remove checkbox
            const filterFormFields = this.jsonForm[0].reader_configuration.inputformatfield.filter(fields => {
                if (fields.name === 'Checkbox_8') {
                    return false;
                } else if (fields.name === 'Is_Dato_Mancante') {
                    return false;
                } else {
                    return true;
                }
            });

            this.arrayFormElements = filterFormFields;
            console.log('this.arrayFormElements', this.arrayFormElements);
            for (let i = 0; i < this.arrayFormElements.length - 1; i++) {
                this.addInputForm();
            }
            this.numeroForm = numero;
        }, error => {
            this.loading = false;
            this.toastr.error(error.error.message, 'Error')
            console.log('getFormById', error)
        });
    }
}

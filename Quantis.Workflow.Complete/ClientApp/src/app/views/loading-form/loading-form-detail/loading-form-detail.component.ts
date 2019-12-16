import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { FormGroup, FormBuilder, Validators, FormControl, FormArray } from '@angular/forms';
import { LoadingFormService } from '../../../_services';
import { ToastrService } from 'ngx-toastr';
import { Observable, forkJoin } from 'rxjs';

@Component({
    selector: 'app-loading-form-detail',
    templateUrl: './loading-form-detail.component.html',
    styleUrls: ['./loading-form-detail.component.scss']
})
export class LoadingFormDetailComponent implements OnInit {
    loading: boolean = false;
    formId: string = null;
    formName: string = null;
    // addLoadingForm: FormGroup;
    apiFormData: [] = [];
    apiFormRules: [] = [];
    homeworld: Observable<{}>;

    dynamicForm: FormGroup;
    items: FormArray;

    constructor(
        private activatedRoute: ActivatedRoute,
        private loadigFormService: LoadingFormService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder,
    ) { }

    ngOnInit() {
        this.dynamicForm = this.formBuilder.group({
            items: this.formBuilder.array([this.createItem()])
        });
        this.activatedRoute.paramMap.subscribe(params => {
            this.formId = params.get("formId");
            this.formName = params.get("formName");
            this._getFormById(+this.formId);
            this._getFormRuleByFormId(+this.formId);
            this._getKpiByFormId(+this.formId);
            // this._getFormFieldsAndRulesData(+this.formId);
        })
    }

    createItem(): FormGroup {
        return this.formBuilder.group({
            min: '',
            max: '',
        });
    }

    addItem(): void {
        this.items = this.dynamicForm.get('items') as FormArray;
        this.items.push(this.createItem());
    }

    _getFormFieldsAndRulesData(formId: number) {
        this.loading = true;
        const getFormById = this.loadigFormService.getFormById(formId);
        const getFormRuleByFormId = this.loadigFormService.getFormRuleByFormId(formId);

        forkJoin([getFormById, getFormRuleByFormId]).subscribe(results => {
            // const { getForm, getFormRule} = results;
            // console.log('getForm ==>', getForm);
            // console.log('getFormRule ==>', getFormRule);

            this.loading = false;
        }, error => {
            console.error('_getFormFieldsAndRulesData ==> ', error);
            this.toastr.error('Unable to get form fields data', 'Error');
            this.loading = false;
        });
    }
    _getFormById(formId: number) {
        this.loading = true;
        this.loadigFormService.getFormById(formId).pipe().subscribe(data => {
            console.log('_getFormById ==>', data);
            let inputformatfield;
            if (!!data) {
                inputformatfield = data[0].reader_configuration.inputformatfield;
                this.apiFormData = inputformatfield;
                this.items = inputformatfield.map(field => {
                    this.addItem();
                });
                console.log('_getFormById this.items ==>', this.items);
            } else {
                this.toastr.error('Unable to get form fields data', 'Error');
            }
            this.loading = false;
        }, error => {
            this.toastr.error('Unable to get form fields data', 'Error');
            console.error('_getFormById ==>', error);
            this.loading = false;
        });
    }

    _getFormRuleByFormId(formId: number) {
        this.loading = true;
        this.loadigFormService.getFormRuleByFormId(formId).pipe().subscribe(data => {
            this.loading = false;
            console.log('_getFormRuleByFormId ==>', data)
        }, error => {
            this.toastr.error(error.error.error, 'Error');
            console.error('_getFormRuleByFormId ==>', error);
            this.loading = false;
        });
    }

    _getKpiByFormId(formId: number) {
        this.loading = true;
        this.loadigFormService.getKpiByFormId(formId).subscribe(data => {
            console.log('_getKpiByFormId ==>', data);
            this.loading = false;
        }, error => {
            this.toastr.error(error.error.error, 'Error');
            console.error('_getKpiByFormId ==>', error);
            this.loading = false;
        });
    }

    AddLoadingFormSubmit(form) {
        debugger;
    }
}

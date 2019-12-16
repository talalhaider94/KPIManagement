import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import Headers from '../_helpers/headers';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { UserSubmitLoadingForm } from '../_models';

@Injectable({
    providedIn: 'root'
})
export class LoadingFormService {
    constructor(private http: HttpClient) { }

    getLoadingForms(): Observable<any> {
        const loadingFormEndPoint = `${environment.API_URL}/data/GetAllForms`;
        return this.http.get(loadingFormEndPoint);
    }

    getKpiByFormId(form_id: number): Observable<any> {
        const getKpiByFormIdEndPoint = `${environment.API_URL}/Data/GetKpiByFormId/${form_id}`;
        return this.http.get(getKpiByFormIdEndPoint);
    }

    getFormFilterById(form_id: number): Observable<any> {
        const getFormFilterByIdEndPoint = `${environment.API_URL}/Data/GetFormById/${form_id}`
        return this.http.get(getFormFilterByIdEndPoint);
    }
    // form submitted by User from Loading form
    submitForm(formFields: UserSubmitLoadingForm) {
        const submitFormEndPoint = `${environment.API_URL}/Data/SubmitForm`;
        return this.http.post(submitFormEndPoint, JSON.stringify(formFields));
    }

    getFormById(form_id: number): Observable<any> {
        const getFormByIdEndPoint = `${environment.API_URL}/Data/GetFormById/${form_id}`
        return this.http.get(getFormByIdEndPoint);
    }

    getFormsByUserId(user_id: number, type: string): Observable<any> {
        const formsByUserIdEndPoint = `${environment.API_URL}/Data/GetFormsByUser/${user_id}/${type}`;
        return this.http.get(formsByUserIdEndPoint);
    }
    // form submitted by Admin from Loading form
    createForm(form): Observable<any> {
        const createFormEndPoint = `${environment.API_URL}/Data/AddUpdateFormRule`;
        return this.http.post(createFormEndPoint, form);
    }

    getFormRuleByFormId(formId): Observable<any> {
        const getFormRuleByFormIdEndPoint = `${environment.API_URL}/Data/GetFormRuleByFormId/${formId}`;
        return this.http.get(getFormRuleByFormIdEndPoint);
    }
    getAttachmentsByFormId(formId: number): Observable<any> {
        const getAttachmentsByFormIdEndPoint = `${environment.API_URL}/Data/GetAttachmentsByFormId/`;
        const params = new HttpParams().set('formId', formId.toString());
        return this.http.get(getAttachmentsByFormIdEndPoint, { params });
    }

    submitAttachment(attachment): Observable<any> {
        const submitAttachmentEndPoint = `${environment.API_URL}/Data/SubmitAttachment`;
        let a = [];
        a.push(attachment);
        return this.http.post(submitAttachmentEndPoint, a);
    }
    submitCSV(attachment): Observable<any> {
      const submitAttachmentEndPoint = `${environment.API_URL}/information/UploadFileToSFTPServer`;
      let a = [];
      a.push(attachment);
      return this.http.post(submitAttachmentEndPoint, attachment);
    }
}

import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class FreeFormReportService {
    constructor(
        private http: HttpClient
    ) { }

    getOwnedReportQueries() {
        return this.http.get(`${environment.API_URL}/data/GetOwnedReportQueries`);
    }

    getAssignedReportQueries() {
        return this.http.get(`${environment.API_URL}/data/GetAssignedReportQueries`);
    }

    addEditReportQuery(params) {
        return this.http.post(`${environment.API_URL}/data/AddEditReportQuery`, params);
    }

    getReportQueryDetailByID(id: number = 1): Observable<any> {
        const params = new HttpParams().set('id', id.toString());
        return this.http.get<any>(`${environment.API_URL}/data/GetReportQueryDetailByID`, { params });
    }
    getKpis(userId, contractId): Observable<any> {
        const getKpisEndPoint = `${environment.API_URL}/information/GetAllKpisByUserId?userId=${userId}&contractId=${contractId}`;
        return this.http.get(getKpisEndPoint);
    }

    GetAllUsersAssignedQueries(id): Observable<any> {
        const params = `${environment.API_URL}/data/GetAllUsersAssignedQueries?queryid=${id}`;
        return this.http.get(params);
    }

    deleteReportQuery(id: number = 1) {
        const params = new HttpParams().set('id', id.toString());
        return this.http.get<any>(`${environment.API_URL}/data/DeleteReportQuery`, { params });
    }

    setUserPermission(params) {
        return this.http.post(`${environment.API_URL}/data/AssignReportQuery`, params);
    }

    ExecuteReportQuery(params) {
        return this.http.post(`${environment.API_URL}/data/ExecuteReportQuery`, params);
    }

    DeleteReportQuery(id): Observable<any> {
        const params = `${environment.API_URL}/data/DeleteReportQuery?id=${id}`;
        return this.http.get(params);
    }
    disable(id): Observable<any> {
        const disableEndPoint = `${environment.API_URL}/data/EnableDisableReportQuery?id=${id}&isenable=false`;
        return this.http.get(disableEndPoint);
    }
    enable(id): Observable<any> {
        const enableEndPoint = `${environment.API_URL}/data/EnableDisableReportQuery?id=${id}&isenable=true`;
        return this.http.get(enableEndPoint);
    }
}

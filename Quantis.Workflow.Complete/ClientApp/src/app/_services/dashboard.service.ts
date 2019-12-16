import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { WidgetModel, DashboardModel, DashboardContentModel } from '../_models';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})

export class DashboardService {
    constructor(private http: HttpClient) { }
    // Return Array of WidgetModel
    getWidgets(): Observable<Array<WidgetModel>> {
        // return this.http.get<Array<WidgetModel>>(`http://localhost:3000/widgets`);
        return this.http.get<Array<WidgetModel>>(`${environment.API_URL}/dashboard/GetAllWidgets`);
    }

    // Return Array of DashboardModel
    getDashboards(): Observable<Array<DashboardModel>> {
        // return this.http.get<Array<DashboardModel>>('http://localhost:3000/dashboards');
        return this.http.get<Array<DashboardModel>>(`${environment.API_URL}/dashboard/GetDashboardsHomePage`);
    }

    // Return an object
    getDashboard(id: number): Observable<any> {
        // return this.http.get(`http://localhost:3000/dashboards/${id}`);
        const params = new HttpParams().set('id', id.toString());
        return this.http.get<any>(
            `${environment.API_URL}/dashboard/GetDashboardWigetsByDashboardId`,
            { params }).pipe(map(result => {
                let dashboardwidgets = [];
                if (result.dashboardwidgets.length > 0) {
                    dashboardwidgets = result.dashboardwidgets.map(widget => {
                        return {
                            cols: widget.sizey,
                            rows: widget.sizex,
                            x: widget.locationx,
                            y: widget.locationy,
                            component: widget.widgetname,
                            widgetname: widget.widgetname,
                            filters: widget.filters,
                            properties: widget.properties,
                            widgetid: widget.widgetid,
                            dashboardid: widget.dashboardid,
                            uiidentifier: widget.uiidentifier,
                            id: widget.id
                        }
                    });
                }
                let createDashboardObj = {
                    id: result.id,
                    name: result.name,
                    createdon: result.createdon,
                    dashboardwidgets,
                    globalfilterid: result.globalfilterid || 0
                }
                return createDashboardObj;
            }));
    }

    updateDashboard(params): Observable<DashboardModel> {
        // return this.http.put<DashboardModel>(`http://localhost:3000/dashboards/${id}`, params);
        let dashboardwidgets = [];
        if (params.dashboardwidgets.length > 0) {
            dashboardwidgets = params.dashboardwidgets.map(widget => {
                return {
                    sizex: widget.rows,
                    sizey: widget.cols,
                    locationx: widget.x,
                    locationy: widget.y,
                    widgetname: widget.widgetname,
                    filters: widget.filters,
                    properties: widget.properties,
                    widgetid: widget.widgetid,
                    dashboardid: widget.dashboardid,
                    uiidentifier: widget.uiidentifier,
                    id: (typeof widget.id === "number") ? widget.id : 0  // for new / cloned widgets id will be zero
                }
            });
        }
        let newParams = {
            id: params.id,
            name: params.name,
            // createdon: params.createdon,
            globalfilterid: params.globalfilterid,
            dashboardwidgets
        }
        console.log('AddUpdateDasboard ==>', JSON.stringify(newParams))
        return this.http.post<DashboardModel>(`${environment.API_URL}/dashboard/AddUpdateDasboard`, newParams);
    }

    getWidgetParameters(url: string): Observable<any> {
        return this.http.get(`${environment.API_URL}/${url}/GetWidgetParameters`);
    }

    getWidgetIndex(url: string, formValues: any): Observable<any> {
        console.log('getWidgetIndex formValues', formValues);
        const widgetIndexEndPoint = `${environment.API_URL}/${url}/Index`;
        return this.http.post(widgetIndexEndPoint, formValues, { observe: 'response' });
    }

    GetOrganizationHierarcy(): Observable<any> {
        return this.http.get(`${environment.API_URL}/globalfilter/GetOrganizationHierarcy?globalFilterId=0`);
    }

    saveDashboardState(dashboardWidgetsState): Observable<any> {
        return this.http.post(`${environment.API_URL}/dashboard/SaveDashboardState`, dashboardWidgetsState);
    }

    activateDashboard(id: number) {
        const params = new HttpParams().set('id', id.toString());
        return this.http.get<any>(`${environment.API_URL}/dashboard/ActivateDashboard`, { params });
    }

    deactivateDashboard(id: number) {
        const params = new HttpParams().set('id', id.toString());
        return this.http.get<any>(`${environment.API_URL}/dashboard/DeactivateDashboard`, { params });
    }

    setDefaultDashboard(id: number) {
        const params = new HttpParams().set('id', id.toString());
        return this.http.get<any>(`${environment.API_URL}/dashboard/SetDefaultDashboard`, { params });
    }
    GetDefaultDashboardId() {
        return this.http.get<any>(`${environment.API_URL}/dashboard/GetDefaultDashboardId`);
    }

    getContractParties(globalFilterId: number = 0) {
        const params = new HttpParams().set('globalFilterId', globalFilterId.toString());
        return this.http.get<any>(`${environment.API_URL}/globalfilter/GetContractParties`, { params });
    }

    getContract(globalFilterId: number = 0, contractpartyId: number = 1) {
        const params = new HttpParams().set('globalFilterId', globalFilterId.toString()).set('contractpartyId', contractpartyId.toString());
        return this.http.get<any>(`${environment.API_URL}/globalfilter/GetContracts`, { params });
    }

    getKPIs(globalFilterId: number = 0, contractId: number = 1) {
        const params = new HttpParams().set('globalFilterId', globalFilterId.toString()).set('contractId', contractId.toString());
        return this.http.get<any>(`${environment.API_URL}/globalfilter/GetKPIs`, { params });
    }

    getLandingPage(period: string = '02/2019') {
        const params = new HttpParams().set('period', period.toString());
        return this.http.get<any>(`${environment.API_URL}/oracle/GetLandingPageByUser`, { params });
    }
    getLandingPageInfo(): Observable<any> {
        const getLandingPageEndPoint = `${environment.API_URL}/data/GetLandingPageInformation`;
        return this.http.get(getLandingPageEndPoint);
    }
    selectLandingPage(): Observable<any> {
        const selectLandingPageEndPoint = `${environment.API_URL}/data/SelectLandingPage`;
        return this.http.get(selectLandingPageEndPoint);
    }
    DeleteDashboard(id): Observable<any> {
        const deleteDashboardEndPoint = `${environment.API_URL}/dashboard/DeleteDashboard?Id=${id}`;
        return this.http.get(deleteDashboardEndPoint);
    }
}

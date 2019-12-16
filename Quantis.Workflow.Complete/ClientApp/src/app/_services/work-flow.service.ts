import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import Headers from '../_helpers/headers';
import {environment} from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WorkFlowService {

  constructor(
    private http: HttpClient
  ) { }

  getAllTickets(): Observable<any>{
    const allTicketsEndPoint = `${environment.API_URL}/sdm/GetAllTickets`;
    //return this.http.get('assets/tempData/getalltickets.json');
    return this.http.get(allTicketsEndPoint);
  }

  getTicketByUser(username): Observable<any> {
    //Danial TODO: need to send Login Token in HEADERS
    const userTicketsEndPoint = `${environment.API_URL}/sdm/GetTicketsByUser`;
    const  params = new  HttpParams().set('username', username);
    return this.http.get(userTicketsEndPoint, { params });
  }

  getAttachmentsByTicket(ticketId): Observable<any>{
    const ticketAttachmentEndPoint = `${environment.API_URL}/sdm/GetAttachmentsByTicket`;
    const  params = new  HttpParams().set('ticketId', ticketId.toString());
    // return this.http.get('assets/tempData/GetAttachmentsByTicket.json');
    return this.http.get(ticketAttachmentEndPoint, { params });
  }

  getTicketHistory(ticketId): Observable<any>{
    const ticketHistoryEndPoint = `${environment.API_URL}/sdm/GetTicketHistory`;
    const  params = new  HttpParams().set('ticketId', ticketId.toString());
    // return this.http.get('assets/tempData/GetTicketHistory.json');
    return this.http.get(ticketHistoryEndPoint, { params });
  }

  downloadAttachment(attachmentHandler): Observable<any>{
    const downloadAttachmentEndPoint = `${environment.API_URL}/sdm/DownloadAttachment`;
    const  params = new  HttpParams().set('attachmentHandle', attachmentHandler);
    return this.http.get(downloadAttachmentEndPoint, { params });
    // return this.http.get('assets/tempData/DownloadAttachment.json');
  }

  getTicketDescriptionByUser(username): Observable<any>{
    // no end point found in controller
    const getFormRuleByFormIdEndPoint = `${environment.API_URL}/sdm/GetTicketDescriptionByUser/${username}`;
    return this.http.get(getFormRuleByFormIdEndPoint);
  }

  transferTicketByID(id, status, description) {
    const transferTicketEndPoint = `${environment.API_URL}/sdm/TransferTicketByID`;
    const  params = new  HttpParams().set('id', id).set('status', status).set('description', description);
    return this.http.get(transferTicketEndPoint, { params });
  }

  escalateTicketbyID(id, status, description) {
    const escalateTicketEndPoint = `${environment.API_URL}/sdm/EscalateTicketbyID`;
    const  params = new  HttpParams().set('id', id).set('status', status).set('description', description);
    return this.http.get(escalateTicketEndPoint, { params });
  }

  getTicketsVerificationByUserVerifica (period: string): Observable<any>{
    const ticketVerificationEndPoint = `${environment.API_URL}/sdm/GetTicketsVerificationByUser`;
    // return this.http.get('https://api.myjson.com/bins/ktkyz');
    const  params = new  HttpParams().set('period', period);
    return this.http.get(ticketVerificationEndPoint,{ params });
  }

  getTicketsSearchByUserRecerca (period: string): Observable<any>{
    const ticketSearchEndPoint = `${environment.API_URL}/sdm/GetTicketsSearchByUser`;
    // return this.http.get('https://api.myjson.com/bins/ktkyz');
    const  params = new  HttpParams().set('period', period);
    return this.http.get(ticketSearchEndPoint, { params });
  }

  uploadAttachmentToTicket(ticketId, docName, docContent): Observable<any> {
    const submitAttachmentEndPoint = `${environment.API_URL}/sdm/UploadAttachmentToTicket`;
    const  params = { TicketId: ticketId, AttachmentName: docName, AttachmentContent: docContent };
    return this.http.post(submitAttachmentEndPoint, params, {observe: 'response'});
  }

  getViloreByUser(period: string): Observable<any> {
    // return this.http.get('https://api.myjson.com/bins/ktkyz');
    const viloreEndPoint = `${environment.API_URL}/sdm/GetTicketsSearchForViloreByUser`;
    const  params = new  HttpParams().set('period', period);
    return this.http.get(viloreEndPoint, { params });
  }

  UpdateTicketValue(args): Observable<any> {
    const updateTicketEndPoint = `${environment.API_URL}/sdm/UpdateTicketValue`;
    return this.http.post(updateTicketEndPoint, args, {observe: 'response'});
  }

}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FileSaverService } from 'ngx-filesaver';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class FileHelpersService {
  constructor(
    private httpClient: HttpClient,
    private fileSaver: FileSaverService,
    private toastr: ToastrService
  ) { }

  downloadFile(base64Data, fileName) {
    let extension = fileName.split('.').pop();
    let prefix = '';
    if (extension === 'pdf') {
      prefix = `data:application/pdf;base64,${base64Data}`;
    } else if (extension === 'png') {
      prefix = `data:image/png;base64,${base64Data}`;
    } else if (extension === 'jpg') {
      prefix = `data:image/jpg;base64,${base64Data}`;
    } else if (extension === 'csv') {
      prefix = `data:application/octet-stream;base64,${base64Data}`;
    } else if (extension === 'xlsx' || extension === 'xls') {
      prefix = `data:application/vnd.ms-excel;base64,${base64Data}`;
    } else if (extension === 'txt') {
      prefix = `data:text/plain;base64,${base64Data}`;
    } else if (extension === 'docx' || extension == 'doc') {
      prefix = `data:application/vnd.ms-word;base64,${base64Data}`;
    } else if (extension === 'ppt' || extension == 'pps') {
      prefix = `data:application/vnd.ms-powerpoint;base64,${base64Data}`;
    } else if (extension === 'msg') {
      prefix = `data:application/vnd.ms-outlook;base64,${base64Data}`;
    } else if (extension === 'gif') {
      prefix = `data:image/gif;base64,${base64Data}`;
    } else if (extension === 'jpeg') {
      prefix = `data:image/jpeg;base64,${base64Data}`;
    } else if (extension === 'eml') {
      prefix = `data:message/rfc822;base64,${base64Data}`;
    } else {
      this.toastr.error('Error', 'Unknown file extension');
      return false;
    }
    
    // fetch(prefix).then(res => res.blob()).then(blob => {
    //   this._FileSaverService.save(blob, fileName);
    // });
    this.httpClient.get(prefix,
      {
        observe: 'response',
        responseType: 'blob'
      }).subscribe(data => {
        this.fileSaver.save(data.body, fileName);
      });
  }
}

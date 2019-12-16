import { Injectable , Directive } from '@angular/core';

@Injectable()
export class GlobalVarsService {
  private month:number;
  constructor() { }
  setmonth(val){
    console.log(val,'value selected');
    this.month = val;
  }
  getSelectedmonth(){
    return this.month;
  }
}

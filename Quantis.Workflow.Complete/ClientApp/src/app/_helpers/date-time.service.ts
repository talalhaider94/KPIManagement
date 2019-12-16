import { Injectable } from '@angular/core';
import * as moment from 'moment';
import { PublicComponent } from '../views/dashboard/public/public.component'

@Injectable({
  providedIn: 'root'
})
export class DateTimeService {
  public moment: any = moment;
  public incompletePeriod:PublicComponent;
  constructor() { }

  getDateTime() {
    return moment().format();
  }

  subtractMonth(no: number) {
    return moment().subtract(no, 'month').format();
  }

  getMonthYear() {
    let month = moment().subtract(1, 'months').format('MM');
    let year = moment().format('YY');
    return { month, year };
  }

  getApiPeriod(month, year) {
    let period;
    if (month === 'all' && year === 'all') {
      period = 'all/all';
    } else {
      period = `${month}/${year}`;
    }
    return period;
  }

  convertUtcToDateTime(date){
    if(date) {
        return moment(date).format('MM/DD/YYYY, hh:mm a')
    } else {
      return 'N/A';
    }
  }

  sortDate(data){
    if(data && data.length === 0){
      return [];
    }
    return data.sort((a,b) => this.moment(a.xvalue).format('DDMMYYYY') - this.moment(b.xvalue).format('DDMMYYYY'))
  }

  buildRangeDate(dateRange) {
    if(Array.isArray(dateRange)) {
      console.log('buildRangeDate if', dateRange);
      return dateRange;
    } else {
      let [startDate, endDate] = dateRange.split('-');
      let [startMonth, startYear] = startDate.split('/');
      let [endMonth, endYear] = endDate.split('/');
      let dateRangeArray = [new Date(`${startMonth}/01/${startYear}`), new Date(`${endMonth}/01/${endYear}`)];
      console.log('buildRangeDate else', dateRangeArray);
      return dateRangeArray;
    }
  }

  timePeriodRange(rangeType, incompletePeriod, isKpiReportTrend: boolean = false) {
    let startDate;
    let endDate;
    endDate = moment().format('MM/YYYY');
    //WE DON'T WANT TO DEDUCT FROM CURRENT MONTH BECAUSE OF TICKET #598
    /* if(!incompletePeriod && isKpiReportTrend){
      endDate = moment().subtract(1, 'months').format('MM/YYYY');
    } */
    if(rangeType === '2') {
      startDate = moment().subtract(2, 'months').format('MM/YYYY');
    } else if(rangeType === '3') {
      startDate = moment().subtract(3, 'months').format('MM/YYYY');
    } else if(rangeType === '4') {
      startDate = moment().subtract(6, 'months').format('MM/YYYY');
    } else if(rangeType === '1') {
      startDate = moment().subtract(1, 'months').format('MM/YYYY');
    } else {
      alert('Please notify developer.');
    }
    return { startDate, endDate }
  }

  WidgetDateAndTime(startDate, endDate, incompletePeriod, isKpiReportTrend: boolean = false){
    let startDate1 = moment(startDate).format('MM/YYYY');
    let endDate1 = moment(endDate).format('MM/YYYY');
    if(!incompletePeriod && isKpiReportTrend){
      endDate1 = moment(endDate).subtract(1, 'months').format('MM/YYYY');
    }
    return { startDate:startDate1, endDate:endDate1}
  } 

  getStringDateRange(startDate, endDate) {
    let formatStartDate = moment(startDate).format('MM/YYYY');
    let formatEndDate = moment(endDate).format('MM/YYYY');
    let stringDateRange = `${formatStartDate}-${formatEndDate}`;
    return stringDateRange;
  }
  // using in BSI chart
  periodWithFalseIncompletePeriod(fromDate, toDate, incompletePeriod){
    let fromCheck = moment(fromDate, 'DD/MM/YYYY');
    let toCheck = moment(toDate, 'DD/MM/YYYY');
    let months = [];
    if(!incompletePeriod){ 
        toCheck = moment(toDate, 'DD/MM/YYYY').subtract(1, 'months');
    }
    let fromMonth = fromCheck.format('M');
    let fromYear  = fromCheck.format('YYYY');
    let toMonth = toCheck.format('M');
    let toYear  = toCheck.format('YYYY');

    while(toCheck > fromCheck || fromCheck.format('M') === toCheck.format('M')){
        let monthyear = fromCheck.format('M') + '/' + fromCheck.format('YYYY');
        months.push(monthyear);
        fromCheck.add(1,'month');
    }
    return {months, fromMonth, toMonth, fromYear, toYear};
  }
}

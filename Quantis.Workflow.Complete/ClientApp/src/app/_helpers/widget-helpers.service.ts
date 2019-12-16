import { Injectable } from '@angular/core';
import { DateTimeService} from './date-time.service';
import { filter } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class WidgetHelpersService {
  allLeafNodesIds: Array<number> = [];
  constructor(
    private dateTimeService: DateTimeService
  ) { }
  // filters and properties are coming from saved widget state
  initWidgetParameters(apiParams, filters, properties) {
    // making it {} gives error temp giving it any type
    try {
      let buildParams: any = {};
      // dirty way
      buildParams.Properties = {};
      buildParams.Filters = {};

      buildParams.GlobalFilterId = 0;
      // buildParams.Note = '';
      // PROPERTIES
      if (apiParams.showmeasure) {
        let index = (Object.keys(properties).length > 0 && !!properties.measure) ? properties.measure : Object.keys(apiParams.measures)[0];
        // let value = Object.keys(apiParams.measures)[index];
        buildParams.Properties.measure = index;
      }
      if (apiParams.showcharttype) {
        let index = (!!properties.charttype) ? properties.charttype : Object.keys(apiParams.charttypes)[0];
        buildParams.Properties.charttype = index;
      }
      if (apiParams.showaggregationoption) {
        let index = (!!properties.aggregationoption) ? properties.aggregationoption : Object.keys(apiParams.aggregationoptions)[0];
        buildParams.Properties.aggregationoption = index;
      }
      // FILTERS
      if (apiParams.showdatetype) {
        // need to change it base on key error might be in filters.dateTypes
        let dateType = (Object.keys(filters).length > 0 && !!filters.showdatetype) ? filters.dateTypes : '0';
        buildParams.Filters.dateTypes = dateType;
      }
      if (apiParams.showdatefilter) {
        let date;
        if (!!filters.date) {
          date = filters.date;
         } else {
          date = '01/2019';
        }
        buildParams.Filters.date = date;
      }
      if (apiParams.showdaterangefilter) {
        // dateTypes custom condition may be needed
        // if defaultdaterange is null need to write custom method for it.
        let dateRangeValue;
        if (!!filters.daterange) {
          dateRangeValue = filters.daterange;
        } else if (!!apiParams.defaultdaterange) {
          dateRangeValue = apiParams.defaultdaterange;
        } else {
          dateRangeValue = '01/2019-12/2019';
        }
        buildParams.Filters.daterange = dateRangeValue;
      }
      if(filters.kpi) {
        buildParams.Filters.kpi = filters.kpi;
      }
      if(filters.kpi1) {
        buildParams.Filters.kpi1 = filters.kpi1;
      }
      if(!apiParams.showincompleteperiodcheck) {
        buildParams.Filters.incompletePeriod = (filters.incompletePeriod === "true");
      }

      if(filters.groupReportCheck) {
        buildParams.Filters.groupReportCheck = (filters.groupReportCheck === "true"); 
      }

      if(apiParams.getOrgHierarcy) {
        if(filters.organizations) {
          buildParams.Filters.organizations = filters.organizations;
        } else {
          // const organizations = this.getAllLeafNodesIds(apiParams.getOrgHierarcy);
          // buildParams.Filters.organizations = organizations.join(',');
        }
      }

      console.log('initWidgetParameters buildParams', buildParams);
      return buildParams;
    } catch (error) {
      console.error('initWidgetParameters', error);
    }
  }

  setWidgetParameters(apiParams, filters, properties) {
    // making it {} gives error temp giving it any type
    try {
      let buildParams: any = {};
      // dirty way
      buildParams.Properties = {};
      buildParams.Filters = {};

      buildParams.GlobalFilterId = 0;
      // buildParams.Note = '';
      // PROPERTIES
      if (apiParams.showmeasure) {
        let index = (Object.keys(properties).length > 0 && !!properties.measure) ? properties.measure : Object.keys(apiParams.measures)[0];
        buildParams.Properties.measure = index.toString();
      }
      if (apiParams.showcharttype) {
        let index = (!!properties.charttype) ? properties.charttype : Object.keys(apiParams.charttypes)[0];
        buildParams.Properties.charttype = index;
      }
      if (apiParams.showaggregationoption) {
        let index = (!!properties.aggregationoption) ? properties.aggregationoption : Object.keys(apiParams.aggregationoptions)[0];
        buildParams.Properties.aggregationoption = index;
      }
      // FILTERS
      if (apiParams.showdatetype) {
        // need to change it base on key error might be in filters.dateTypes
        let dateType = (Object.keys(filters).length > 0 && !!filters.showdatetype) ? filters.dateTypes : '0';
        buildParams.Filters.dateTypes = dateType;
      }
      if (apiParams.showdatefilter) {
        let date;
        if (!!filters.date) {
          date = filters.date;
         } else {
          date = '01/2019';
        }
        buildParams.Filters.date = date;
      }
      if (apiParams.showdaterangefilter) {
        // dateTypes custom condition may be needed
        // if defaultdaterange is null need to write custom method for it.
        let dateRangeValue;
        if (!!filters.daterange) {
          dateRangeValue = this.dateTimeService.buildRangeDate(filters.daterange);
        } else if (!!apiParams.defaultdaterange) {
          dateRangeValue = this.dateTimeService.buildRangeDate(apiParams.defaultdaterange);
        } else {
          dateRangeValue = this.dateTimeService.buildRangeDate('01/2019-12/2019');
        }
        buildParams.Filters.startDate = dateRangeValue[0];
        buildParams.Filters.endDate = dateRangeValue[1];
      }
      if(apiParams.allContractParties) {
        // buildParams.Filters.contractParties = filters.contractParties || apiParams.allContractParties[0].key;
        buildParams.Filters.contractParties = filters.contractParties || '';
      }
      if(apiParams.allContracts) {
        // buildParams.Filters.contracts = filters.contracts || apiParams.allContracts[0].key;
        buildParams.Filters.contracts = filters.contracts || '';
      }
      if(apiParams.allKpis) {
        // buildParams.Filters.kpi = filters.kpi || apiParams.allKpis[0].key;
        buildParams.Filters.kpi = filters.kpi || '';
      }

      if(apiParams.allContractParties1) {
        // buildParams.Filters.contractParties1 = filters.contractParties1 || apiParams.allContractParties1[0].key;
        buildParams.Filters.contractParties1 = filters.contractParties1 || '';
      }
      if(apiParams.allContracts1) {
        // buildParams.Filters.contracts1 = filters.contracts1 || apiParams.allContracts1[0].key;
        buildParams.Filters.contracts1 = filters.contracts1 || '';
      }
      if(apiParams.allKpis1) {
        // buildParams.Filters.kpi1 = filters.kpi1 || apiParams.allKpis1[0].key;
        buildParams.Filters.kpi1 = filters.kpi1 || '';
      }

      if(apiParams.showincompleteperiodcheck) {
        buildParams.Filters.incompletePeriod = Boolean(filters.incompletePeriod) || false;
      }
      if(filters.groupReportCheck) {
        buildParams.Filters.groupReportCheck = (filters.groupReportCheck === "true"); 
      }
      if(apiParams.getOrgHierarcy) {
        if(filters.organizations) {
          buildParams.Filters.organizations = filters.organizations;
        } else {
          // buildParams.Filters.organizations = this.getAllLeafNodesIds(apiParams.getOrgHierarcy);
        }
      }
      console.log('setWidgetParameters buildParams', buildParams);
      return buildParams;
    } catch (error) {
      console.error('setWidgetParameters', error);
    }
  }
  
  getAllLeafNodesIds(complexJson) {
    try {
        if (complexJson) {
            complexJson.forEach((item: any) => {
                if (item.children) {
                    this.getAllLeafNodesIds(item.children);
                } else {
                    this.allLeafNodesIds.push(item.id);
                }
            });
            return this.allLeafNodesIds;
        }
    } catch(error) {
        console.error('getAllLeafNodesIds', error);
    }
}
}

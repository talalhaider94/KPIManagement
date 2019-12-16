export class WidgetsHelper {
    // filters and properties are coming from saved widget state
    static initWidgetParameters(apiParams, filters, properties) {
        // making it {} gives error temp giving it any type
        try {
            let buildParams: any = {};
            // dirty way
            buildParams.Properties = {};
            buildParams.Filters = {};

            buildParams.GlobalFilterId = 0;
            // buildParams.Note = '';
            // PROPERTIES
            if(apiParams.showmeasure) {
                let index = (Object.keys(properties).length > 0 && !!properties.measure) ? properties.measure : '0';
                // let value = Object.keys(apiParams.measures)[index];
                buildParams.Properties.measure = index;
            }
            if(apiParams.showcharttype) {
                let index = (!!properties.charttype) ? properties.charttype : Object.keys(apiParams.charttypes)[0];
                buildParams.Properties.charttype = index;
            }
            if(apiParams.showaggregationoption) {
                let index = (!!properties.aggregationoption) ? properties.aggregationoption : Object.keys(apiParams.aggregationoptions)[0];
                buildParams.Properties.aggregationoption = index;
            }
            // FILTERS
            if(apiParams.showdaterangefilter) {
                if(apiParams.showdatetype) {
                    // need to change it base on key error might be in filters.dateTypes
                    let dateType = (Object.keys(filters).length > 0 && !!filters.showdatetype) ? filters.dateTypes : '0';
                    buildParams.Filters.dateTypes = dateType;
                }
                // dateTypes custom condition may be needed
                // if defaultdaterange is null need to write custom method for it.
                let dateRangeValue;
                if(!!filters.daterange) {
                    dateRangeValue = filters.daterange;
                } else if(!!apiParams.defaultdaterange) {
                    dateRangeValue = apiParams.defaultdaterange;
                } else {
                    dateRangeValue = '01/2019-12/2019';
                }
                //  (!!filters.daterange) ? filters.daterange : (!!apiParams.defaultdaterange) ? apiParams.defaultdaterange : '01/2019-12/2019';
                buildParams.Filters.daterange = dateRangeValue;
            }
            return buildParams;
        } catch(error) {
            console.error('initWidgetParameters', error);
        }
    }
}

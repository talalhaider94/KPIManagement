export const formatDataLabelForNegativeValues = (dataLabel) => {
    if (dataLabel === -999) {
        return 'NE';
    } else if (dataLabel === -888) {
        return 'NF';
    } else if (dataLabel === -777) {
        return 'NT';
    } else if (dataLabel === 0 || dataLabel === null ){
        return '0';
    } else {
        return dataLabel;
    }
}

export const updateChartLabelStyle = (color: string = '#000000', shadow: string = '#000000', outline: string = '2px contrast') =>{
    let dataLabelsObj = {
        color: color,
        style: {
            textShadow: shadow, 
            textOutline: outline, 
        }
    }
    return dataLabelsObj;
}

export * from './error-interceptor.service';
export * from './token-interceptor.service';
export * from './workflow';
export * from './date-time.service';
export * from './widgetsHelper';
export * from './widget-helpers.service';
export * from './file-helpers.service';
export * from './chartHelpers';

export function removeNullKeysFromObject(obj) {
  const newObj = {};
  Object.keys(obj).forEach(key => {
    if (obj[key] !== 'date' && obj[key] && typeof obj[key] === "object") {
      newObj[key] = removeNullKeysFromObject(obj[key]); // recurse
    } else if (obj[key] !== null && obj[key] !== undefined) {
      newObj[key] = obj[key].toString(); // copy value
    }
  });
  return newObj;
}

export const chartExportTranslations = {
  downloadJPEG: 'Download JPEG',
  downloadPDF: 'Download PDF',
  downloadPNG: 'Download PNG',
  downloadSVG: 'Download SVG',
  viewFullscreen: 'Vedi in full screen',
  printChart: 'Stampa'
}

export const exportChartButton = {
  enabled: true,
  buttons: {
    contextButton: {
      menuItems: ['printChart', 'separator', 'downloadPNG', 'downloadJPEG', 'downloadPDF', 'downloadSVG']
    }
  }
}

export function getDistinctArray(arr) {
  let dups = {};
  return arr.filter(function(el) {
      var hash = el.valueOf();
      var isDup = dups[hash];
      dups[hash] = true;
      return !isDup;
  });
}

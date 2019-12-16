import { Pipe, PipeTransform } from '@angular/core';
@Pipe({
  name: 'filter'
})
export class FilterUsersPipe implements PipeTransform {
  transform(items: any[], searchText: string): any[] {
    if(!items) return [];
    if (!searchText) return items;
    searchText = searchText.toLowerCase();
    return items.filter(it => {
      return JSON.stringify(it).toLowerCase().includes(searchText);
          /*return (it.userid ? it.userid.toLowerCase() : ''
                + ' ' + it.name ? it.name.toLowerCase() : ''
                + ' ' + it.surname ? it.surname.toLowerCase() : ''
                + ' ' + it.ca_bsi_account ? it.ca_bsi_account.toLowerCase() : '').includes(searchText);*/
        });
   }
}

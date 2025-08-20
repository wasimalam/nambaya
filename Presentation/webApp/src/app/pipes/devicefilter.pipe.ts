import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'devicefilter' })
export class AutocompleteDeviceStartsWith implements PipeTransform {
  public transform(collection: any[], term = '') {
    if (term) {
      return collection.filter(item =>
        item.serialNumber
          .toString()
          .toLowerCase()
          .startsWith(term.toString().toLowerCase())
      );
    } else {
      return collection;
    }
  }
}

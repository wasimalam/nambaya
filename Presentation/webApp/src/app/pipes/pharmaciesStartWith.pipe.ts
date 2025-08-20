import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'pharmaciesStartsWith' })
export class PharmaciesStartsWithPipe implements PipeTransform {
  public transform(collection: any[], term = '') {
    if (term) {
      return collection.filter(item =>
        item.name
          .toString()
          .toLowerCase()
          .startsWith(term.toString().toLowerCase())
      );
    } else {
      return collection;
    }
  }
}

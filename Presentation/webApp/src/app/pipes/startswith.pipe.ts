import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'startsWith' })
export class AutocompletePipeStartsWith implements PipeTransform {
  // tslint:disable-next-line:typedef
  public transform(collection: any[], term = '') {
    if (term) {
      return collection.filter(item =>
        item.description
          .toString()
          .toLowerCase()
          .startsWith(term.toString().toLowerCase())
      );
    } else {
      return collection;
    }
  }
}

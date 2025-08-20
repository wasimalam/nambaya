import { Pipe, PipeTransform } from '@angular/core';
import { orderBy } from 'lodash';

@Pipe({ name: 'sortFilter' })
export class SortPipe implements PipeTransform {
  transform(value: any[], order = '', column: string = ''): any[] {
    if (!value || order === '' || !order) {
      return value;
    } // no array

    return orderBy(value, [column], [order]);
  }
}

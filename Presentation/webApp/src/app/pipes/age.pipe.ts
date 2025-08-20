import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';
@Pipe({
  name: 'age'
})
export class AgePipe implements PipeTransform {
  transform(value: Date): string {
    if (value) {
      const today = moment();
      const birthdate = moment(value);
      const dateFormat = localStorage.getItem('dateFormat');
      const timeZone = localStorage.getItem('timeZone');
      const formatedDate = moment.utc(value).tz(timeZone).format(dateFormat);
      const years = today.diff(birthdate, 'years');
      const html: string = formatedDate.toString() + ' (' + years + 'y)';
      return html;
    } else {
      return '';
    }
  }
}

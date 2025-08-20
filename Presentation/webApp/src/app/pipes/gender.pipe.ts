import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'genderFilter' })
export class GenderPipe implements PipeTransform {
  public transform(genderId: number) {
    if (!genderId) {
      return genderId;
    } else if (genderId === 401) {
      return 'Male';
    } else if (genderId === 402) {
      return 'Female';
    } else if (genderId === 403) {
      return 'Unknown';
    } else if (genderId === 404) {
      return 'Diverse';
    } else {
      return genderId;
    }
  }
}

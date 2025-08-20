import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'emailSMSType' })
export class EmailSMSTypePipe implements PipeTransform {
  public transform(typeID: number) {
    if (!typeID) {
      return typeID;
    } else if (typeID === 601) {
      return 'Email';
    } else if (typeID === 602) {
      return 'SMS';
    } else {
      return typeID;
    }
  }
}

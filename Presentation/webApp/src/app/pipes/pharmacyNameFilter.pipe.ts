import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'pharmacyName' })
export class PHarmacyNamePipe implements PipeTransform {
  public transform(id: number, collection: any[], type?: string) {
    if (type === 'pharmacy') {
      let pharmacyName: string = '';
      if (id && collection) {
        let pharmacy: any = collection.filter(item => item.id === id)[0];
        pharmacyName = pharmacy.name;
      }
      return pharmacyName;
    } else {
      return id;
    }
  }
}

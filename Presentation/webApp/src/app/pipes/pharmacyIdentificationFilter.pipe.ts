import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'pharmacyIdentification' })
export class PharmacyIdentificationPipe implements PipeTransform {
  public transform(id: number, collection: any[], type?: string) {
      let pharmacyIdent = '';
      if (id && collection) {
        const pharmacy: any = collection.filter(item => item.id === id)[0];
        pharmacyIdent = pharmacy.identification;
      }
      return pharmacyIdent;
  }
}

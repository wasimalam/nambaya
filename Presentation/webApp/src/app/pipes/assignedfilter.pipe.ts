import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'assignedFilter' })
export class DeviceAssignedCheck implements PipeTransform {
  public transform(collection: any[], id: number) {
    if (id === 452) {
      return collection;
    } else {
      for (let index = 0; index < collection.length; index++) {
        const element = collection[index];
        if (collection[index].id === 452) {
          collection.splice(index, 1);
          break;
        }
      }
      return collection;
    }
  }
}

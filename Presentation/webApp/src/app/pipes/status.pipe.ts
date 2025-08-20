import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'statusFilter' })
export class StatusPipe implements PipeTransform {
  public transform(statusNumber: number) {
    if (!statusNumber) {
      return statusNumber;
    } else if (statusNumber === 451) {
      return 'device_statuses.Available';
    } else if (statusNumber === 452) {
      return 'device_statuses.Assigned';
    } else if (statusNumber === 453) {
      return 'device_statuses.Inactive';
    } else {
      return statusNumber;
    }
  }
}

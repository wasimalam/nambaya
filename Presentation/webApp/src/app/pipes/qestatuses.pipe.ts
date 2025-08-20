import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'qeStatusFilter' })
export class QeStatusesPipe implements PipeTransform {
  public transform(qeStatusId: number) {
    if (!qeStatusId) {
      return qeStatusId;
    } else if (qeStatusId === 511) {
      return 'Green';
    } else if (qeStatusId === 512) {
      return 'Yellow';
    } else if (qeStatusId === 513) {
      return 'Orange';
    } else if (qeStatusId === 514) {
      return 'Red';
    } else if (qeStatusId === 515) {
      return 'Red Red';
    } else {
      return qeStatusId;
    }
  }
}

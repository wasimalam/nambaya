import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'evaluationStatus' })
export class EvaluationStatusPipe implements PipeTransform {
  public transform(statusId: any) {
    if (!statusId) {
      return statusId;
    } else if (statusId === 651 || statusId === '651') {
      return 'cardio_case_statuses.Case Started';
    } else if (statusId === 652 || statusId === '652') {
      return 'cardio_case_statuses.Device Allocated';
    } else if (statusId === 653 || statusId === '653') {
      return 'cardio_case_statuses.Device Returned';
    } else if (statusId === 654 || statusId === '654') {
      return 'cardio_case_statuses.Quick Eval In Queue';
    } else if (statusId === 655 || statusId === '655') {
      return 'cardio_case_statuses.Quick Eval Completed';
    } else if (statusId === 656 || statusId === '656') {
      return 'cardio_case_statuses.Detailed Eval Locked';
    } else if (statusId === 657 || statusId === '657') {
      return 'cardio_case_statuses.E-Sign Pending';
    } else if (statusId === 658 || statusId === '658') {
      return 'cardio_case_statuses.Detailed Eval Completed';
    } else if (statusId === 659 || statusId === '659') {
      return 'cardio_case_statuses.Report Dispatch Failed';
    }  else if (statusId === 660 || statusId === '660') {
      return 'cardio_case_statuses.Report Dispatching';
    }  else if (statusId === 661 || statusId === '661') {
      return 'cardio_case_statuses.Report Dispatched';
    } else {
      return statusId;
    }
  }
}

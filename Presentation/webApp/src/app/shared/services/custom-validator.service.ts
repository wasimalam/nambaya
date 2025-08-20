import { Injectable } from '@angular/core';
import { ValidatorFn, AbstractControl } from '@angular/forms';
import { FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class CustomValidatorService {
  // function to set error messages
  static getValidatorErrorMessage(validatorName: string, validatorValue?: any) {
    const config = {
      alphaNumericAllowed: 'Only apha numeric input is allowed',
      numericAllowed: 'Only numeric values are allowed'
    };

    return config[validatorName];
  }

  alpaNumValidator(control: AbstractControl) {
    if (control.value.match(/^[a-zA-Z0-9]*$/)) {
      return null;
    } else {
      return { alphaNumericAllowed: true };
    }
  }

  decimalValidation(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      if (!control.value) {
        return null;
      }

      if (String(control.value).match(/^\d*\.?\d{0,2}$/g)) {
        return null;
      } else {
        return { twoDecimalAllowed: true };
      }
    };
  }

  decimalCommaValidation(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      if (!control.value) {
        return null;
      }

      if (String(control.value).match(/^\d*\,?\d{0,2}$/g)) {
        return null;
      } else {
        return { twoDecimalAllowed: true };
      }
    };
  }

  numberValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      if (!control.value) {
        return null;
      }
      if (control.value.length === 0 || String(control.value).match(/^[0-9]*$/)) {
        return null;
      } else {
        return { invalidNumber: true };
      }
    };
  }

  maxLengthValidator(maxLength: number): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      if (!control.value) {
        return null;
      }

      if (control.value.length > maxLength) {
        return { invalidMaxLength: true };
      } else {
        return null;
      }
    };
  }

  pharmacyIdMaxLengthValidator(maxLength: number, pharmacyPrefix: string): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      if (!control.value) {
        return null;
      }

      if (Number(control.value.length) + Number(1) + Number(pharmacyPrefix.length) > maxLength) {
        return { invalidMaxLength: true };
      } else {
        return null;
      }
    };
  }

  phoneValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      if (!control.value) {
        return null;
      }
      if (control.value.length === 0 || control.value.match(/\(?\+\(?49\)?[ ()]?([- ()]?\d[- ()]?){10}/g)) {
        return null;
      } else {
        return { invalidPhone: true };
      }
    };
  }

  patternValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      if (!control.value) {
        return null;
      }
      const regex = new RegExp('^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).{15,}$');
      const valid = regex.test(control.value);
      return valid ? null : { invalidPassword: true };
    };
  }

  MatchPassword(password: string, confirmPassword: string) {
    return (formGroup: FormGroup) => {
      const passwordControl = formGroup.controls[password];
      const confirmPasswordControl = formGroup.controls[confirmPassword];

      if (!passwordControl || !confirmPasswordControl) {
        return null;
      }

      if (confirmPasswordControl.errors && !confirmPasswordControl.errors.passwordMismatch) {
        return null;
      }

      if (passwordControl.value !== confirmPasswordControl.value) {
        if (passwordControl.value == null) {
          passwordControl.setValue('');
          return;
        }
        confirmPasswordControl.setErrors({ passwordMismatch: true });
      } else {
        confirmPasswordControl.setErrors(null);
      }
    };
  }
}

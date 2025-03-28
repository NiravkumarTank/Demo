import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

// Custom validator for password strength
export function passwordStrengthValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;

    // Check password strength using regex
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/;
    
    // If password doesn't meet criteria, return error
    if (value && !passwordRegex.test(value)) {
      return { weakPassword: true };
    }

    return null; // If valid, return null
  };
}

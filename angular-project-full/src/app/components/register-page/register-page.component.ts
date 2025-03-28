
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpserviceService } from '../../httpservice.service';
import { passwordStrengthValidator } from '../../Interface/Password-validation';

@Component({
  selector: 'app-register-page',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, HttpClientModule],
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.css']
})
// export class RegisterPageComponent {
//   registrationForm: FormGroup;
//   otpVerificationForm: FormGroup;
//   otpSent: boolean = false;
//   otp: string = ''; // Store OTP temporarily
//   otpExpirationTime: Date | null = null; // Store OTP expiration time

//   http1 = inject(HttpserviceService);

//   constructor(
//     private fb: FormBuilder,
//     private router: Router
//   ) {
//     this.registrationForm = this.fb.group({
//       FirstName: ['', Validators.required],
//       LastName: ['', Validators.required],
//       Email: ['', [Validators.required, Validators.email]],
//       Password: ['', [Validators.required, passwordStrengthValidator()]],
//       cPassword: ['', Validators.required],
//       PhoneNumber: ['', [Validators.required, Validators.pattern('^[0-9]+$')]],
//       Gender: ['', Validators.required]
//     });

//     this.otpVerificationForm = this.fb.group({
//       Otp: ['', [Validators.required, Validators.pattern('^[0-9]{6}$')]] // OTP should be a 6-digit number
//     });
//   }

//   ngOnInit(): void {
//     // Clear any stored registration data on page load
//     sessionStorage.removeItem('registrationData');
//     sessionStorage.removeItem('otp');

//     // Check if there is any stored registration data in sessionStorage (for pre-filling form)
//     const storedData = sessionStorage.getItem('registrationData');
//     if (storedData) {
//       const parsedData = JSON.parse(storedData);
//       this.registrationForm.patchValue(parsedData);
//     }

//     // Check if OTP and expiration time are available in sessionStorage
//     const otpData = sessionStorage.getItem('otp');
//     if (otpData) {
//       const { otp, expirationTime } = JSON.parse(otpData);
//       this.otp = otp;
//       this.otpExpirationTime = new Date(expirationTime);
//     }
//   }

//   onSubmit() {
//     this.registrationForm.markAllAsTouched();

//     if (this.registrationForm.invalid) {
//       return;
//     }

//     const { FirstName, LastName, Email, Password, PhoneNumber, Gender } = this.registrationForm.value;

//     if (Password !== this.registrationForm.get('cPassword')?.value) {
//       alert('Passwords do not match!');
//       return;
//     }

//     const formData = new FormData();
//     formData.append('FirstName', this.registrationForm.value.FirstName);
//     formData.append('LastName', this.registrationForm.value.LastName);
//     formData.append('Email', this.registrationForm.value.Email);
//     formData.append('Password', this.registrationForm.value.Password);
//     formData.append('PhoneNumber', this.registrationForm.value.PhoneNumber);
//     formData.append('Gender', this.registrationForm.value.Gender);

//     // Save form data in sessionStorage
//     sessionStorage.setItem('registrationData', JSON.stringify(this.registrationForm.value));

//     // Step 1: Register the user and send OTP
//     this.http1.register(formData).subscribe(
//       (response: any) => {
//         console.log(response);

//         if (response.message === 'OTP sent to email. Please verify to complete registration.') {
//           // OTP is sent successfully
//           this.otpSent = true;
//           this.otp = response.otp; // Store OTP temporarily
//           this.otpExpirationTime = new Date(response.otpExpirationTime); // Store OTP expiration time

//           // Save OTP and expiration time in sessionStorage
//           sessionStorage.setItem('otp', JSON.stringify({ otp: this.otp, expirationTime: this.otpExpirationTime }));

//           alert('OTP sent to your email. Please verify.');
//         } else {
//           alert(response.message);
//         }
//       },
//       (error) => {
//         console.error('Error:', error);
//         alert('An error occurred during registration.');
//       }
//     );
//   }

//   onVerifyOtp() {
//     const otp = this.otpVerificationForm.get('Otp')?.value;
//     console.log(otp);
//     console.log(this.otp);

//     if (otp !== this.otp) {
//       alert('Invalid OTP');
//       return;
//     }

//     if (this.otpExpirationTime && new Date() > this.otpExpirationTime) {
//       alert('OTP has expired. Please request a new OTP.');
//       return;
//     }

//     // Step 2: Verify OTP
//     const verificationData = new FormData();
//     verificationData.append('Email', this.registrationForm.value.Email);
//     verificationData.append('Otp', otp);

//     this.http1.verifyOtp(verificationData).subscribe(
//       (response: any) => {
//         if (response.message === 'Registration successful') {
//           alert('Registration successful');
//           // Clear sessionStorage after successful registration
//           sessionStorage.removeItem('registrationData');
//           sessionStorage.removeItem('otp');
          
//           // Reset the form
//           this.registrationForm.reset();
//           this.otpVerificationForm.reset();
//           this.otpSent = false; // Hide OTP verification form

//           // Navigate to login page
//           this.router.navigate(['/login']);
//         } else {
//           alert(response.message);
//         }
//       },
//       (error) => {
//         console.error('Error:', error);
//         alert('An error occurred during OTP verification.');
//       }
//     );
//   }
// }

export class RegisterPageComponent {
  registrationForm: FormGroup;
  otpVerificationForm: FormGroup;
  otpSent: boolean = false;
  otp: string = '';
  otpExpirationTime: Date | null = null;
  http1 = inject(HttpserviceService);

  isSubmitting: boolean = false;  // This will control the visibility of the loading GIF

  constructor(
    private fb: FormBuilder,
    private router: Router
  ) {
    this.registrationForm = this.fb.group({
      FirstName: ['', Validators.required],
      LastName: ['', Validators.required],
      Email: ['', [Validators.required, Validators.email]],
      Password: ['', [Validators.required, passwordStrengthValidator()]],
      cPassword: ['', Validators.required],
      PhoneNumber: ['', [Validators.required, Validators.pattern('^[0-9]+$')]],
      Gender: ['', Validators.required]
    });

    this.otpVerificationForm = this.fb.group({
      Otp: ['', [Validators.required, Validators.pattern('^[0-9]{6}$')]]
    });
  }

  onSubmit() {
    this.registrationForm.markAllAsTouched();

    if (this.registrationForm.invalid) {
      return;
    }

    const { FirstName, LastName, Email, Password, PhoneNumber, Gender } = this.registrationForm.value;

    if (Password !== this.registrationForm.get('cPassword')?.value) {
      alert('Passwords do not match!');
      return;
    }

    const formData = new FormData();
    formData.append('FirstName', this.registrationForm.value.FirstName);
    formData.append('LastName', this.registrationForm.value.LastName);
    formData.append('Email', this.registrationForm.value.Email);
    formData.append('Password', this.registrationForm.value.Password);
    formData.append('PhoneNumber', this.registrationForm.value.PhoneNumber);
    formData.append('Gender', this.registrationForm.value.Gender);

    sessionStorage.setItem('registrationData', JSON.stringify(this.registrationForm.value));

    // Show the loading GIF
    this.isSubmitting = true;

    // Step 1: Register the user and send OTP
    this.http1.register(formData).subscribe(
      (response: any) => {
        console.log(response);

        if (response.message === 'OTP sent to email. Please verify to complete registration.') {
          this.otpSent = true;
          this.otp = response.otp;
          this.otpExpirationTime = new Date(response.otpExpirationTime);
          sessionStorage.setItem('otp', JSON.stringify({ otp: this.otp, expirationTime: this.otpExpirationTime }));
          // alert('OTP sent to your email. Please verify.');
        } else {
          alert(response.message);
        }

        // Hide the loading GIF after response
        this.isSubmitting = false;
      },
      (error) => {
        console.error('Error:', error);
        alert('An error occurred during registration.');
        // Hide the loading GIF after error
        this.isSubmitting = false;
      }
    );
  }

  onVerifyOtp() {
    const otp = this.otpVerificationForm.get('Otp')?.value;
    console.log(otp);
    console.log(this.otp);

    if (otp !== this.otp) {
      alert('Invalid OTP');
      return;
    }

    if (this.otpExpirationTime && new Date() > this.otpExpirationTime) {
      alert('OTP has expired. Please request a new OTP.');
      return;
    }

    const verificationData = new FormData();
    verificationData.append('Email', this.registrationForm.value.Email);
    verificationData.append('Otp', otp);

    this.http1.verifyOtp(verificationData).subscribe(
      (response: any) => {
        if (response.message === 'Registration successful') {
          alert('Registration successful');
          sessionStorage.removeItem('registrationData');
          sessionStorage.removeItem('otp');
          this.registrationForm.reset();
          this.otpVerificationForm.reset();
          this.otpSent = false;
          this.router.navigate(['/login']);
        } else {
          alert(response.message);
        }

        // Hide the loading GIF after response
        this.isSubmitting = false;
      },
      (error) => {
        console.error('Error:', error);
        alert('An error occurred during OTP verification.');
        // Hide the loading GIF after error
        this.isSubmitting = false;
      }
    );
  }
}

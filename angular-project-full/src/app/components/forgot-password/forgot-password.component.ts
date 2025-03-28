import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-forgot-password',
  imports: [CommonModule, FormsModule,RouterLink],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {
  email: string = '';
  otp: number | any = 0;
  newPassword: string = '';
  userId: number | any = 0; // This will be fetched after OTP verification
  isOtpSent: boolean = false;
  isOtpVerified: boolean = false;
  isPasswordReset: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(private http: HttpClient) {}

  // // Step 1: Request OTP
   requestOtp() {
    console.log('Attempting to request OTP...');
    
    // Validate email before sending OTP
    if (!this.email || !this.validateEmail(this.email)) {
      console.log('Invalid email');
      this.errorMessage = 'Please enter a valid email address.';
      return;
    }
    
    // Prepare form data for the API request
    const formData = new FormData();
    formData.append("email", this.email);

    this.http.post('https://localhost:7262/api/ForgotPassword/Email', formData).subscribe(
      (response: any) => {
        // console.log(response);

        // Check if userId exists before assigning
        if (response && response.userId) {
          this.userId = response.userId;
          this.isOtpSent = true;
          this.successMessage = 'OTP has been sent to your email.';
          this.errorMessage = ''; // Clear error message on success
          
          // Store userId in sessionStorage
          sessionStorage.setItem('userId', response.userId.toString());

          // Remove userId from sessionStorage after 5 minutes
          setTimeout(() => {
            sessionStorage.removeItem('userId');
            // console.log('userId removed from sessionStorage after 5 minutes');
          }, 300000); // 5 minutes = 300,000 milliseconds
        } else {
          this.errorMessage = 'No such email registered. Please register first.';
          this.successMessage = ''; // Clear success message in case of error
        }
      },
      (error) => {
        console.error('Error: ', error);
        
        // Check if the error contains a message or status code
        this.errorMessage = error?.message || `An error occurred (status code: ${error?.status}). Please try again.`;
        this.successMessage = ''; // Clear success message on error
      }
    );
}

  // Step 2: Verify OTP
   verifyOtp() {
    if (!this.otp) {
      this.errorMessage = 'Please enter the OTP.';
      return;
    }
  
    // Prepare form data for the OTP verification request
    const formData = new FormData();
    formData.append("userId", this.userId.toString());
    formData.append("enteredOtp", this.otp.toString());
  
    this.http.post('https://localhost:7262/api/ForgotPassword/Otp', formData).subscribe(
      (response: any) => {
        // console.log(response);
  
        if (response?.message === "OTP verified successfully. Please proceed with resetting your password.") {
          // OTP is successfully verified
          this.isOtpVerified = true;  // Set OTP verified flag to true
          this.successMessage = 'OTP verified successfully!';
          this.errorMessage = '';
          
          // Proceed to step 3 (new password entry form)
          this.isPasswordReset = false;  // Ensure password reset form is displayed
        } else if (response?.message === "OTP expired or invalid, a new OTP has been sent") {
          // Invalid OTP or OTP expired
          this.errorMessage = 'OTP expired or invalid. A new OTP has been sent.';
          this.successMessage = ''; // Clear success message
          
          // Allow user to enter a new OTP
          this.isOtpVerified = false;  // OTP is not verified yet
          this.isPasswordReset = true; // Allow them to enter a new OTP
          this.otp = '';  // Reset the OTP field to allow new input
        } else {
          // General error message if something unexpected occurs
          this.errorMessage = 'Invalid OTP. Please try again.';
          this.successMessage = ''; // Clear success message
        }
      },
      (error) => {
        this.errorMessage = 'An error occurred. Please try again.';
        this.successMessage = ''; // Clear success message
      }
    );
  }
  
  
  // Step 3: Reset Password
  resetPassword() {
    if (!this.newPassword) {
      this.errorMessage = 'Please enter a new password.';
      return;
    }

    // Prepare form data for the password reset request
    const formData = new FormData();
    formData.append("userId", this.userId);
    formData.append("newPassword", this.newPassword);
    
    this.http.post('https://localhost:7262/api/ForgotPassword/Password', formData).subscribe(
      (response: any) => {
        // console.log(response);
        
        if (response?.message) {

          this.isPasswordReset = true; // Set password reset flag to true
          this.successMessage = 'Your password has been reset successfully!';
          this.errorMessage = ''; // Clear error message
          setTimeout(() => {
            this.successMessage = ''; 
            sessionStorage.removeItem('userId');
            window.location.href = '/login';
          }, 2000);
        } else {
          this.errorMessage = response?.message || 'Failed to reset password. Please try again.';
          this.successMessage = ''; // Clear success message
        }
      },
      (error) => {
        this.errorMessage = 'An error occurred. Please try again.';
        this.successMessage = ''; // Clear success message
      }
    );
  }

  // Email validation function
  validateEmail(email: string): boolean {
    const regex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return regex.test(email);
  }
}

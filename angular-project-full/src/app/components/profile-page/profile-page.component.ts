// import { CommonModule } from '@angular/common';
// import { HttpClient } from '@angular/common/http';
// import { Component } from '@angular/core';
// import { FormsModule } from '@angular/forms';

// @Component({
//   selector: 'app-profile-page',
//   imports: [CommonModule,FormsModule],

//   templateUrl: './profile-page.component.html',
//   styleUrl: './profile-page.component.css'
// })
// export class ProfilePageComponent {

//   user: any = null; // To store the user data
//   isEditing: boolean = false; // To toggle editing mode

//   constructor(private http: HttpClient) {}

//   ngOnInit() {
//     // Fetch user data on component initialization
//     this.http.get('https://localhost:7262/api/User/GetByIdUser/2').subscribe(
//       (data) => {
//         console.log(data);
        
//         this.user = data;
//       },
//       (error) => {
//         console.error('Error fetching user data', error);
//       }
//     );
//   }

//   toggleEdit() {
//     this.isEditing = !this.isEditing;
//   }

//   saveProfile() {
//     // Send the updated user data to the API (add POST/PUT request if needed)
//     console.log('Saving profile', this.user);
//     this.isEditing = false; // Exit edit mode after saving
//   }
// }

import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-profile-page',
  imports: [CommonModule, FormsModule],
  templateUrl: './profile-page.component.html',
  styleUrls: ['./profile-page.component.css']
})
export class ProfilePageComponent {
  user: any = null; 
  isEditing: boolean = false; 

  private userId: string|null = '';  

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.userId = sessionStorage.getItem('UserId');
    // Fetch user data on component initialization
    this.http.get('https://localhost:7262/api/User/GetByIdUser/'+this.userId).subscribe(
      (data) => {
        // console.log(data);
        this.user = data;
      },
      (error) => {
        console.error('Error fetching user data', error);
      }
    );
  }

  toggleEdit() {
    this.isEditing = !this.isEditing;
  }

  saveProfile() {
    // Send the updated user data to the API using PUT request
    this.http.put(`https://localhost:7262/api/User/UpdateUser/`+this.userId, this.user).subscribe(
      (response) => {
        alert('Profile updated successfully');
        this.isEditing = false; 
      },
      (error) => {
        console.error('Error saving profile', error);
      }
    );
  }
}

import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-show-reviews',
  imports: [CommonModule, FormsModule],
  templateUrl: './show-reviews.component.html',
  styleUrl: './show-reviews.component.css'
})
export class ShowReviewsComponent  implements OnInit{
users: any[] = [];
  filteredusers: any[] = [];
  nameFilter: string = '';
  categoryFilter: string = '';
  subcategoryFilter: string = '';

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    this.getusers();
  }

  getusers(): void {
    this.http.get<any[]>('https://localhost:7262/api/Review/getAllReviews')
      .subscribe(
        data => {
          // console.log(data);
          
          this.users = data;
          this.filteredusers = data;  // Initially, display all users
        },
        error => {
          console.error('Error fetching users:', error);
        }
      );
  }


}

import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { filter } from 'rxjs';

@Component({
  selector: 'app-admin-home-page',
  imports: [RouterModule, CommonModule],
  templateUrl: './admin-home-page.component.html',
  styleUrls: ['./admin-home-page.component.css']
})
export class AdminHomePageComponent implements OnInit {

  // Public properties to store the current URL and login status
  public currentUrl: string = '';
  public isAdmin: boolean = false; // Check if the user is an admin

  constructor(private router: Router) {}

  ngOnInit(): void {
    // Listen for route changes to update the current URL
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.currentUrl = this.router.url;
    });

    // Check if the user has an "admin" role stored in sessionStorage
    this.checkUserRole();
  }

  // Function to check if the user has an "admin" role in sessionStorage
  private checkUserRole(): void {
    const userRole = sessionStorage.getItem('Role'); 
    if (userRole === 'Admin') {
      this.isAdmin = true; 
    } else {
      this.isAdmin = false;
    }
  }
}

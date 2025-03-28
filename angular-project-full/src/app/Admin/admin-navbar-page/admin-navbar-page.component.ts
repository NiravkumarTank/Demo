import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink, RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-navbar-page',
  imports: [RouterLink, CommonModule,RouterModule],
  templateUrl: './admin-navbar-page.component.html',
  styleUrl: './admin-navbar-page.component.css'
})
export class AdminNavbarPageComponent {
 isLoggedIn: boolean = false;

 
constructor(private router: Router) {}



  ngOnInit(): void {
    this.isLoggedIn = !!sessionStorage.getItem('UserId'); 
  }

  logout() {
    sessionStorage.removeItem('UserId'); 
    sessionStorage.removeItem('Role'); 
    this.isLoggedIn = false;
      this.router.navigateByUrl('home')
    window.location.href = 'home';
  }


}

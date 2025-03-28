// import { CommonModule } from '@angular/common';
// import { Component, OnInit } from '@angular/core';
// import { Router, RouterLink } from '@angular/router';

// @Component({
//   selector: 'app-navbar-page',
//   imports: [RouterLink, CommonModule],
//   templateUrl: './navbar-page.component.html',
//   styleUrls: ['./navbar-page.component.css']
// })
// export class NavbarPageComponent implements OnInit {
//   isLoggedIn: boolean = false;

//   ngOnInit(): void {
//     this.isLoggedIn = !!sessionStorage.getItem('UserId'); 
//   }

//   logout() {
//     sessionStorage.removeItem('UserId'); 
//     sessionStorage.removeItem('Role'); 

//     this.isLoggedIn = false;
//     this.router.navigateByUrl("home");

//   }


// constructor(private router: Router) {}



// }

import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Subscription, interval } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { HttpserviceService } from '../../httpservice.service';
import { CommonModule } from '@angular/common';
import { CartResponse } from '../../Interface/interface-all';

@Component({
  selector: 'app-navbar-page',
  imports: [RouterLink, CommonModule],
  templateUrl: './navbar-page.component.html',
  styleUrls: ['./navbar-page.component.css']
})
export class NavbarPageComponent implements OnInit {

  isLoggedIn: boolean = false;
  cartItemCount: number = 0;  // Variable to store the cart item count
  private userId: string = '';  // Store UserId
  
  constructor(private router: Router, private cartService: HttpserviceService, private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.isLoggedIn = !!sessionStorage.getItem('UserId');
    if (this.isLoggedIn) {
      this.userId = sessionStorage.getItem('UserId')!;
      this.fetchCartItems();
    }
  }
  
  fetchCartItems() {
    this.cartService.totalCartNumber(this.userId).subscribe((cartItems: CartResponse) => {
      // console.log(cartItems); 
      this.cartItemCount = cartItems.total_cart_items; 
    });
  }
  
  
  logout() {
    sessionStorage.removeItem('UserId');
    sessionStorage.removeItem('Role');
    this.isLoggedIn = false;
    this.router.navigateByUrl("home");
  }
}

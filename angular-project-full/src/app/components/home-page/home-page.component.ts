// import { CommonModule } from '@angular/common';
// import { Component, OnInit } from '@angular/core';
// import { RouterModule } from '@angular/router';
// import { HttpserviceService } from '../../httpservice.service';
// import { Product } from '../../Interface/interface-all';

// @Component({
//   selector: 'app-home-page',
//   imports: [CommonModule, RouterModule],
//   templateUrl: './home-page.component.html',
//   styleUrls: ['./home-page.component.css']
// })
// export class HomePageComponent implements OnInit {

//   products: Product[] = [];
//   loading: boolean = true;
//   errorMessage: string = '';
//   showModal: boolean = false; // To manage modal visibility

//   constructor(private http: HttpserviceService) { }

//   ngOnInit(): void {
//     this.http.getProducts().subscribe(
//       (data) => {
//         this.products = data;
//         this.loading = false;
//       },
//       (error) => {
//         this.errorMessage = 'Failed to load products.';
//         this.loading = false;
//       }
//     );
//   }



//   addToCart(product: Product): void {
//     if (this.isLoggedIn()) {
//       const userId = sessionStorage.getItem('UserId') as string;
//       this.http.addToCart(product.productId, userId).subscribe(
//         (response) => {
//           // console.log('Product added to cart successfully:', response);
//           alert("Product added to cart successfully");
//           window.location.reload();
//         },
//         (error) => {
//           console.error('Error adding to cart:', error);
//         }
//       );
//     } else {
//       this.showModal = true;
//     }
//   }

//   isLoggedIn(): boolean {
//     const userId = sessionStorage.getItem('UserId');
//     return userId !== null;
//   }

//   closeModal(): void {
//     this.showModal = false;
//   }
// }
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterModule } from '@angular/router';
import { HttpserviceService } from '../../httpservice.service';
import { Product } from '../../Interface/interface-all';
import { HomeImageCarouselComponent } from '../home-image-carousel/home-image-carousel.component';
import { AboutUsComponent } from '../about-us/about-us.component';
import { FooterPageComponent } from '../footer-page/footer-page.component';
import { ShoesBrandLogosComponent } from '../shoes-brand-logos/shoes-brand-logos.component';

@Component({
  selector: 'app-home-page',
  imports: [CommonModule, RouterModule,HomeImageCarouselComponent,RouterLink,AboutUsComponent,FooterPageComponent,ShoesBrandLogosComponent],
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css']
})
export class HomePageComponent implements OnInit {

  products: Product[] = [];
  loading: boolean = true;
  errorMessage: string = '';
  showModal: boolean = false; 
  successMessage: string = '';

  constructor(private http: HttpserviceService) { }

  ngOnInit(): void {
    window.scrollTo(0, 0); 

    // this.http.getProducts().subscribe(
    //   (data) => {
    //     this.products = data;
    //     this.loading = false;
    //   },
    //   (error) => {
    //     this.errorMessage = 'Failed to load products.';
    //     this.loading = false;
    //   }
    // );
  }

  // addToCart(product: Product): void {
  //   if (this.isLoggedIn()) {
  //     const userId = sessionStorage.getItem('UserId') as string;
  //     this.http.addToCart(product.productId, userId).subscribe(
  //       (response) => {
  //         this.successMessage = "Product added to cart successfully!";
  //         setTimeout(() => {
  //           this.successMessage = ''; 
  //           window.location.reload();
  //         }, 500); 
  //       },
  //       (error) => {
  //         console.error('Error adding to cart:', error);
  //       }
  //     );
  //   } else {
  //     this.showModal = true;
  //   }
  // }

  isLoggedIn(): boolean {
    const userId = sessionStorage.getItem('UserId');
    return userId !== null;
  }

  closeModal(): void {
    this.showModal = false;
  }
}

// import { Component } from '@angular/core';
// import { Product } from '../../Interface/interface-all';
// import { HttpserviceService } from '../../httpservice.service';
// import { CommonModule } from '@angular/common';
// import { FormsModule } from '@angular/forms';
// import { HttpClient } from '@angular/common/http';

// @Component({
//   selector: 'app-man-category',
//   imports: [CommonModule, FormsModule],
//   templateUrl: './man-category.component.html',
//   styleUrls: ['./man-category.component.css']
// })
// export class ManCategoryComponent {
//   products: Product[] = [];
//   filteredProducts: Product[] = [];
//   loading: boolean = true;
//   errorMessage: string = '';
//   successMessage: string = '';
//   showModal: boolean = false;
//   selectedSubcategory: string = 'Man';
//   searchQuery: string = '';
//   availableSizes: string[] = Array.from({ length: 6 }, (_, i) => (i + 7).toString());
//   selectedSizes: { [productId: number]: string } = {};

//   selectedProduct: Product | null = null;

//   constructor(private http: HttpserviceService, private http1: HttpClient) { }

//   ngOnInit(): void {
//     window.scrollTo(0, 0);
//     this.getBySubcategory(this.selectedSubcategory);


//   }

//   getBySubcategory(subcategory: string): void {
//     this.loading = true;
//     this.http.getBySubcategory(subcategory).subscribe(
//       (products) => {
//         if (products && products.length > 0) {
//           this.products = products;
//           this.filteredProducts = products;
//           this.errorMessage = '';
//           this.products.forEach((product) => {
//             this.selectedSizes[product.productId] = '8';
//           });
//         } else {
//           this.products = [];
//           this.filteredProducts = [];
//           this.errorMessage = 'No products found for this Man category.';
//         }
//         this.loading = false;
//       },
//       (error) => {
//         this.errorMessage = 'Error fetching products. Please try again later.';
//         this.loading = false;
//       }
//     );
//   }


  


//   showProductDetails(product: Product): void {
//     this.loading = true;
//     this.http.getProductById(product.productId).subscribe(
//       (response) => {
//         this.selectedProduct = response;
//       },
//       (error) => {
//         console.error('Error fetching product details:', error);
//       },
//       () => {
//         this.loading = false;
//       }
//     );
//   }


//   addToCart(product: Product): void {
//     if (this.isLoggedIn()) {
//       const userId = sessionStorage.getItem('UserId') as string;
//       const selectedSize = this.selectedSizes[product.productId];
  
//       if (selectedSize) {
//         this.http.addToCart(product.productId, userId, selectedSize).subscribe(
//           (response) => {
//             this.successMessage = 'Product added to cart successfully!';
//             setTimeout(() => {
//               this.successMessage = '';
//               window.location.reload();
//             }, 500);
//           },
//           (error) => {
//             console.error('Error adding to cart:', error);
//           }
//         );
//       } else {
//         this.errorMessage = 'Please select a size before adding the product to the cart.';
//       }
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
//     this.selectedProduct = null;
//   }

//   filterProducts(): void {
//     if (this.searchQuery.trim() === '') {
//       this.filteredProducts = [...this.products];
//     } else {
//       this.filteredProducts = this.products.filter((product) =>
//       (product.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
//         product.subcategory.toLowerCase().includes(this.searchQuery.toLowerCase()))
//       );
//     }
//   }

//   sortProducts(event: any): void {
//     const sortOrder = event.target.value;

//     if (sortOrder === 'lowToHigh') {
//       this.filteredProducts = [...this.filteredProducts].sort((a, b) => a.price - b.price);
//     } else if (sortOrder === 'highToLow') {
//       this.filteredProducts = [...this.filteredProducts].sort((a, b) => b.price - a.price);
//     } else {
//       // When "Select Price filter" is selected (empty value)
//       this.filteredProducts = [...this.products]; // Show all products, no sorting
//     }
//   }


//   filterBySubcategory(subcategory: string): void {
//     this.getBySubcategory(subcategory);
//   }
// }


import { Component } from '@angular/core';
import { Product } from '../../Interface/interface-all';
import { HttpserviceService } from '../../httpservice.service';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-man-category',
  imports: [CommonModule, FormsModule],
  templateUrl: './man-category.component.html',
  styleUrls: ['./man-category.component.css']
})
export class ManCategoryComponent {
  products: Product[] = [];
  filteredProducts: Product[] = [];
  loading: boolean = true;
  errorMessage: string = '';
  successMessage: string = '';
  showModal: boolean = false;
  selectedSubcategory: string = 'Man';
  searchQuery: string = '';
  availableSizes: string[] = Array.from({ length: 6 }, (_, i) => (i + 7).toString());
  selectedSizes: { [productId: number]: string } = {};

  selectedProduct: Product | null = null;
  reviews: any[] = [];  // Holds reviews for the selected product

  constructor(private http: HttpserviceService, private http1: HttpClient) { }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    this.getBySubcategory(this.selectedSubcategory);
  }

  getBySubcategory(subcategory: string): void {
    this.loading = true;
    this.http.getBySubcategory(subcategory).subscribe(
      (products) => {
        if (products && products.length > 0) {
          this.products = products;
          this.filteredProducts = products;
          this.errorMessage = '';
          this.products.forEach((product) => {
            this.selectedSizes[product.productId] = '8';
          });
        } else {
          this.products = [];
          this.filteredProducts = [];
          this.errorMessage = 'No products found for this Man category.';
        }
        this.loading = false;
      },
      (error) => {
        this.errorMessage = 'Error fetching products. Please try again later.';
        this.loading = false;
      }
    );
  }

  // Show product details and fetch reviews
  showProductDetails(product: Product): void {
    this.loading = true;
    this.http.getProductById(product.productId).subscribe(
      (response) => {
        this.selectedProduct = response;
        this.fetchProductReviews(response.productId);  // Fetch reviews after product details
      },
      (error) => {
        console.error('Error fetching product details:', error);
      },
      () => {
        this.loading = false;
      }
    );
  }
  fetchProductReviews(productId: number): void {
    this.http1.get<string>(`https://localhost:7262/api/Review/GetProductIdReview/${productId}`, { responseType: 'text' as 'json' }).subscribe(
      (response) => {
        try {
          // Check if the response is plain text message or a valid JSON
          if (response === "No reviews found for this Product.") {
            console.log('No reviews found for this Product');
            this.reviews = []; 
          } else {
            const parsedResponse = JSON.parse(response);
            this.reviews = parsedResponse || []; // Ensure we handle the case of empty or invalid response
          }
        } catch (e) {
          console.error('Error parsing reviews:', e);
          this.reviews = [];
          this.errorMessage = 'An error occurred while fetching reviews.'; // Set the error message
        }
      },
      (error) => {
        // HTTP error handling
        console.error('Error fetching reviews:', error);
        this.reviews = [];
        this.errorMessage = 'An error occurred while fetching reviews.'; // Set a user-friendly error message
      }
    );
  }
  
  

  addToCart(product: Product): void {
    if (this.isLoggedIn()) {
      const userId = sessionStorage.getItem('UserId') as string;
      const selectedSize = this.selectedSizes[product.productId];
  
      if (selectedSize) {
        this.http.addToCart(product.productId, userId, selectedSize).subscribe(
          (response) => {
            this.successMessage = 'Product added to cart successfully!';
            setTimeout(() => {
              this.successMessage = '';
              window.location.reload();
            }, 500);
          },
          (error) => {
            console.error('Error adding to cart:', error);
          }
        );
      } else {
        this.errorMessage = 'Please select a size before adding the product to the cart.';
      }
    } else {
      this.showModal = true;
    }
  }

  isLoggedIn(): boolean {
    const userId = sessionStorage.getItem('UserId');
    return userId !== null;
  }

  closeModal(): void {
    this.showModal = false;
    this.selectedProduct = null;
  }

  filterProducts(): void {
    if (this.searchQuery.trim() === '') {
      this.filteredProducts = [...this.products];
    } else {
      this.filteredProducts = this.products.filter((product) =>
        (product.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
          product.subcategory.toLowerCase().includes(this.searchQuery.toLowerCase()))
      );
    }
  }

  sortProducts(event: any): void {
    const sortOrder = event.target.value;

    if (sortOrder === 'lowToHigh') {
      this.filteredProducts = [...this.filteredProducts].sort((a, b) => a.price - b.price);
    } else if (sortOrder === 'highToLow') {
      this.filteredProducts = [...this.filteredProducts].sort((a, b) => b.price - a.price);
    } else {
      // When "Select Price filter" is selected (empty value)
      this.filteredProducts = [...this.products]; // Show all products, no sorting
    }
  }

  filterBySubcategory(subcategory: string): void {
    this.getBySubcategory(subcategory);
  }
}

// import { Component } from '@angular/core';
// import { Product } from '../../Interface/interface-all';
// import { HttpserviceService } from '../../httpservice.service';
// import { CommonModule } from '@angular/common';
// import { FormsModule } from '@angular/forms';

// @Component({
//   selector: 'app-woman-category',
//   imports: [CommonModule,FormsModule],
//   templateUrl: './woman-category.component.html',
//   styleUrl: './woman-category.component.css'
// })
// export class WomanCategoryComponent {
//  products: Product[] = []; 
//   filteredProducts: Product[] = [];  
//   loading: boolean = true;  
//   errorMessage: string = ''; 
//   showModal: boolean = false; 
//   successMessage: string = '';


//   selectedSubcategory: string = 'woman';  
//   searchQuery: string = ''; 


//   availableSizes: string[] = Array.from({ length: 7 }, (_, i) => (i + 4).toString()); 
//   selectedSizes: { [productId: number]: string } = {};

//   selectedProduct: Product | null = null;

//   constructor(private http: HttpserviceService) {}

//   ngOnInit(): void {
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
//             this.selectedSizes[product.productId] = '6';
//           });
//         } else {
       
//           this.products = [];  
//           this.filteredProducts = [];  
//           this.errorMessage = 'No products found for this woman categoty.';  
//         }
  
//         this.loading = false;
//       },
//       (error) => {
//         this.errorMessage = 'Error fetching products. Please try again later.';
//         this.loading = false;
//       }
//     );
//   }




//    // Function to show product details in the modal
//    showProductDetails(product: Product): void {
//     this.loading = true;
//     this.http.getProductById(product.productId).subscribe(
//       (response) => {
//         console.log('Product Details:', response);
//         this.selectedProduct = response; // Set the selected product details
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
//       const selectedSize = this.selectedSizes[product.productId]; // Add size support here if needed
//       this.http.addToCart(product.productId, userId, selectedSize).subscribe(
//         (response) => {
//           this.successMessage = "Product added to cart successfully!";
//           setTimeout(() => {
//             this.successMessage = ''; 
//           }, 500); 
//         },
//         (error) => {
//           console.error('Error adding to cart:', error);
//         }
//       );
//     } else {
//       this.showModal = true;
//     }
//   }
 

//     filterProducts(): void {
//       if (this.searchQuery.trim() === '') {
//         this.filteredProducts = [...this.products];  
//       } else {
//         this.filteredProducts = this.products.filter((product) =>
//          ( product.name.toLowerCase().includes(this.searchQuery.toLowerCase())|| product.subcategory.toLowerCase().includes(this.searchQuery.toLowerCase()))
//         );
//       }
//     }
  
//     isLoggedIn(): boolean {
//       const userId = sessionStorage.getItem('UserId');
//       return userId !== null;
//     }
  
//     closeModal(): void {
//       this.showModal = false;
//       this.selectedProduct = null; 
//     }

//   filterBySubcategory(subcategory: string): void {
//     this.getBySubcategory(subcategory); 
//   }
// }


import { Component } from '@angular/core';
import { Product } from '../../Interface/interface-all';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpserviceService } from '../../httpservice.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-woman-category',
  imports: [CommonModule, FormsModule],
  templateUrl: './woman-category.component.html',
  styleUrls: ['./woman-category.component.css']
})
export class WomanCategoryComponent {
  products: Product[] = [];
  filteredProducts: Product[] = [];
  loading: boolean = true;
  errorMessage: string = '';
  successMessage: string = '';
  showModal: boolean = false;
  selectedProduct: Product | null = null;
  searchQuery: string = '';
  availableSizes: string[] = Array.from({ length: 7 }, (_, i) => (i + 4).toString());
  selectedSizes: { [productId: number]: string } = {};

  reviews: any[] = [];  // Holds reviews for the selected product


  constructor(private http: HttpserviceService , private http1: HttpClient) {}

  ngOnInit(): void {
    window.scrollTo(0, 0); 
    this.getBySubcategory('woman');
  }

  // Fetch products based on the subcategory
  getBySubcategory(subcategory: string): void {
    this.loading = true;
    this.http.getBySubcategory(subcategory).subscribe(
      (products) => {
        if (products && products.length > 0) {
          this.products = products;
          this.filteredProducts = products;
          this.errorMessage = '';
          this.products.forEach((product) => {
            this.selectedSizes[product.productId] = '6'; // Default size for women
          });
        } else {
          this.products = [];
          this.filteredProducts = [];
          this.errorMessage = 'No products found for this Woman category.';
        }
        this.loading = false;
      },
      (error) => {
        this.errorMessage = 'Error fetching products. Please try again later.';
        this.loading = false;
      }
    );
  }

  // Show product details in the modal
  // showProductDetails(product: Product): void {
  //   this.loading = true;
  //   this.http.getProductById(product.productId).subscribe(
  //     (response) => {
  //       // console.log('Product Details:', response);
  //       this.selectedProduct = response;
  //     },
  //     (error) => {
  //       console.error('Error fetching product details:', error);
  //     },
  //     () => {
  //       this.loading = false;
  //     }
  //   );
  // }

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
  
    // fetchProductReviews(productId: number): void {
    //   this.http1.get<any[]>(`https://localhost:7262/api/Review/GetProductIdReview/${productId}`).subscribe(
    //     (reviews) => {
    //       this.reviews = reviews;
    //       // No need to set hasReviews, just check reviews length in the template
    //     },
    //     (error) => {
    //       console.error('Error fetching reviews:', error);
    //       this.reviews = [];  // In case of error, set reviews to an empty array
    //     }
    //   );
    // }
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
              this.reviews = parsedResponse || []; 
            }
          } catch (e) {
            console.error('Error parsing reviews:', e);
            this.reviews = [];
            this.errorMessage = 'An error occurred while fetching reviews.';
          }
        },
        (error) => {
          // HTTP error handling
          console.error('Error fetching reviews:', error);
          this.reviews = [];
          this.errorMessage = 'An error occurred while fetching reviews.'; 
        }
      );
    }

  // Add product to the cart
  addToCart(product: Product): void {
    if (this.isLoggedIn()) {
      const userId = sessionStorage.getItem('UserId') as string;
      const selectedSize = this.selectedSizes[product.productId];

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
      this.showModal = true;
    }
  }

  // Check if the user is logged in
  isLoggedIn(): boolean {
    const userId = sessionStorage.getItem('UserId');
    return userId !== null;
  }

  // Close modal
  closeModal(): void {
    this.showModal = false;
    this.selectedProduct = null;
  }

  // Filter products by search query
  filterProducts(): void {
    if (this.searchQuery.trim() === '') {
      this.filteredProducts = [...this.products];
    } else {
      this.filteredProducts = this.products.filter((product) =>
        product.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        product.subcategory.toLowerCase().includes(this.searchQuery.toLowerCase())
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
  

  // Filter products by subcategory
  filterBySubcategory(subcategory: string): void {
    this.getBySubcategory(subcategory);
  }
}

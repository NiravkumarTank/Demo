import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-show-product',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './show-product.component.html',
  styleUrl: './show-product.component.css',
})
export class ShowProductComponent implements OnInit {
  products: any[] = [];
  filteredProducts: any[] = [];
  nameFilter: string = '';
  categoryFilter: string = '';
  subcategoryFilter: string = '';

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    this.getProducts();
  }

  // Fetch products from the API
  getProducts(): void {
    this.http.get<any[]>('https://localhost:7262/api/Admin/ShowAllProduct')
      .subscribe(
        data => {
          this.products = data;
          this.filteredProducts = data;  // Initially, display all products
        },
        error => {
          console.error('Error fetching products:', error);
        }
      );
  }

  // Handle delete product
  deleteProduct(productId: number): void {
    const confirmation = confirm('Are you sure you want to delete this product?');
    if (confirmation) {
      this.http.delete(`https://localhost:7262/api/Admin/DeleteById/${productId}`)
        .subscribe(
          () => {
            alert('Product deleted successfully');
            this.getProducts();  // Refresh the product list
          },
          error => {
            console.error('Error deleting product:', error);
          }
        );
    }
  }

  // Navigate to the update page
  updateProduct(id: number): void {
    this.router.navigate(['admin/edit-product', id]);
  }

  // Filter products based on category and subcategory
  filterProducts(): void {
    this.filteredProducts = this.products.filter(product => {
      const matchesName = this.nameFilter
        ? product.name.toLowerCase().includes(this.nameFilter.toLowerCase())
        : true;

      const matchesCategory = this.categoryFilter
        ? product.category.toLowerCase().includes(this.categoryFilter.toLowerCase())
        : true;

      const matchesSubcategory = this.subcategoryFilter
        ? product.subcategory.toLowerCase().includes(this.subcategoryFilter.toLowerCase())
        : true;

      return matchesName && matchesCategory && matchesSubcategory;
    });
  }
}

import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-product',
  imports: [CommonModule,FormsModule],
  templateUrl: './create-product.component.html',
  styleUrl: './create-product.component.css'
})
export class CreateProductComponent {
  product = {
    name: '',
    description: '',
    category: '',
    subcategory: '',
    price: 0,
    stock: 0,
    image: ''
  };

  categories: any[] = [];
  subcategories: any[] = [];
  
  nameError = '';
  descriptionError = '';
  categoryError = '';
  subcategoryError = '';
  priceError = '';
  stockError = '';
  imageError = '';

  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit(): void {
    this.getCategories();
  }

  getCategories() {
    this.http.get<any[]>('https://localhost:7262/api/Category/CategoryShow').subscribe(
      response => {
        this.categories = response;
      },
      error => {
        console.error('Error fetching categories:', error);
      }
    );
  }

  onCategoryChange() {
    if (this.product.category) {
      this.http.get<any[]>(`https://localhost:7262/api/Admin/GetBySubCategory/${this.product.category}`).subscribe(
        response => {
          this.subcategories = response;
        },
        error => {
          console.error('Error fetching subcategories:', error);
        }
      );
    } else {
      this.subcategories = [];
    }
  }

  onFileSelect(event: any) {
    this.product.image = event.target.files[0];
  }

  validateFields() {
    let isValid = true;
  
    // Validate each field
    this.nameError = this.product.name ? '' : 'Product Name is required';
    this.descriptionError = this.product.description ? '' : 'Description is required';
    this.categoryError = this.product.category ? '' : 'Category is required';
    this.subcategoryError = this.product.subcategory ? '' : 'Subcategory is required';
    this.priceError = this.product.price ? '' : 'Price is required';
    this.stockError = this.product.stock >= 0 ? '' : 'Stock is required';
    
    // Image validation
    this.imageError = this.product.image ? '' : 'Image is required';
  
    // If any error exists, the form is not valid
    isValid = !(this.nameError || this.descriptionError || this.categoryError || this.subcategoryError || this.priceError || this.stockError || this.imageError);
  
    return isValid;
  }
  
  onSubmit() {
    if (this.validateFields()) {
      const formData = new FormData();
      formData.append('Name', this.product.name);
      formData.append('Description', this.product.description);
      formData.append('c_id', this.product.category);
      formData.append('sc_id', this.product.subcategory);
      formData.append('Price', this.product.price.toString());
      formData.append('Stock', this.product.stock.toString());
      formData.append('Image', this.product.image);

      this.http.post('https://localhost:7262/api/Admin/AddProduct', formData).subscribe(
        response => {
          alert('Product added successfully!');
          this.router.navigate(['/admin/show-product']);
        },
        error => {
          console.error('Error adding product:', error);
        }
      );
    }
  }

}

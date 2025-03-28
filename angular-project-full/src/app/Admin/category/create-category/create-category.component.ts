import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpserviceService } from '../../../httpservice.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-create-category',
  imports: [FormsModule, CommonModule],
  templateUrl: './create-category.component.html',
  styleUrls: ['./create-category.component.css']
})
export class CreateCategoryComponent implements OnInit {

  categories: any = [];
  selectedCategory: string = '';
  subcategoryName: string = '';

  constructor(private categoryService: HttpserviceService, private router: Router) { }

  ngOnInit(): void {
    this.getCategories();
  }

  // Fetch categories from API
  getCategories() {
    this.categoryService.getCategories().subscribe({
      next: (response) => {
        if (response) {
          this.categories = response;
        } else {
          console.log('No categories found.');
        }
      },
      error: (err) => {
        console.error('Error fetching categories:', err);
      }
    });
  }

  onSubmit(categoryForm: any) {
    // Mark all fields as touched to trigger validation
    categoryForm.controls['category'].markAsTouched();
    categoryForm.controls['sc_Name'].markAsTouched();

    // Validate the form, if invalid, do not proceed
    if (categoryForm.invalid) {
      return;
    }

    // If the form is valid, submit the data
    let FromData = new FormData();
    FromData.append("sc_Name", this.subcategoryName);
    FromData.append("c_id", this.selectedCategory);
    
    this.categoryService.addSubCategory(FromData).subscribe({
      next: () => {
        alert('Category added successfully!');
        this.router.navigate(['/admin/show-category']);
      },
      error: (err) => {
        console.error('Error adding subcategory:', err);
      }
    });
  }
}

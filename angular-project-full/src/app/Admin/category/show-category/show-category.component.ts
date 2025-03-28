// import { Component } from '@angular/core';
// import { HttpserviceService } from '../../../httpservice.service';
// import { CommonModule } from '@angular/common';
// import { RouterLink } from '@angular/router';

// @Component({
//   selector: 'app-show-category',
//   imports: [CommonModule,RouterLink],
//   templateUrl: './show-category.component.html',
//   styleUrl: './show-category.component.css'
// })
// export class ShowCategoryComponent {
//   subcategories: any[] = [];

//   constructor(private categoryService: HttpserviceService) { }

//   ngOnInit(): void {
//     this.categoryService.getSubCategories().subscribe((data: any[]) => {
//       this.subcategories = data;
//       // console.log(data);
      
//     }, error => {
//       console.error('Error fetching subcategories', error);
//     });
//   }

//   getCategoryName(c_id: number): string {
//     switch(c_id) {
//       case 1:
//         return 'Men';
//       case 2:
//         return 'Women';
//       case 3:
//         return 'Kids';
//       default:
//         return 'Unknown';
//     }
//   }
// }

import { Component } from '@angular/core';
import { HttpserviceService } from '../../../httpservice.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';  // Add FormsModule for ngModel

@Component({
  selector: 'app-show-category',
  imports: [CommonModule, RouterLink, FormsModule],  // Import FormsModule
  templateUrl: './show-category.component.html',
  styleUrl: './show-category.component.css'
})
export class ShowCategoryComponent {
  subcategories: any[] = [];
  filteredSubcategories: any[] = [];  // To hold filtered results
  searchText: string = '';  // Binding variable for search box

  constructor(private categoryService: HttpserviceService) { }

  ngOnInit(): void {
    this.categoryService.getSubCategories().subscribe((data: any[]) => {
      this.subcategories = data;
      this.filteredSubcategories = data;  // Initialize filteredSubcategories with all data
    }, error => {
      console.error('Error fetching subcategories', error);
    });
  }

  getCategoryName(c_id: number): string {
    switch (c_id) {
      case 1:
        return 'Man';
      case 2:
        return 'Woman';
      case 3:
        return 'Kids';
      default:
        return 'Unknown';
    }
  }

  // Filter the subcategories based on the search text
  filterSubcategories(): void {
    if (this.searchText) {
      this.filteredSubcategories = this.subcategories.filter(subcategory => 
        subcategory.sc_Name.toLowerCase().includes(this.searchText.toLowerCase()) ||
        this.getCategoryName(subcategory.c_id).toLowerCase().includes(this.searchText.toLowerCase())
      );
    } else {
      this.filteredSubcategories = this.subcategories;  // Show all if no search text
    }
  }
}

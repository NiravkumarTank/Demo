import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FooterPageComponent } from '../../components/footer-page/footer-page.component';

@Component({
  selector: 'app-category',
  imports: [ CommonModule,RouterModule,FooterPageComponent ],
  templateUrl: './category.component.html',
  styleUrl: './category.component.css'
})
export class CategoryComponent {

}

import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-shoes-brand-logos',
  imports: [CommonModule],
  templateUrl: './shoes-brand-logos.component.html',
  styleUrl: './shoes-brand-logos.component.css'
})
export class ShoesBrandLogosComponent {

  images = [
    'Logo/adidas.jpg',
    'Logo/asics.jpg',
    'Logo/bata1.jpg',
    'Logo/puma3.jpg',
    'Logo/Nike.jpg',
    'Logo/rebook.jpg',
    'Logo/redtep.jpg',
    'Logo/skechers.jpg',
  ];
  
  currentIndex = 0;
  visibleItems = 6;  
  slideWidth = 300;
  autoSlideInterval: any;

  constructor() { }

  ngOnInit(): void {
    this.startAutoSlide();
  }

  ngOnDestroy(): void {
    if (this.autoSlideInterval) {
      clearInterval(this.autoSlideInterval);
    }
  }

  get sliderPosition() {
    return `translateX(-${this.currentIndex * this.slideWidth}px)`;
  }

  startAutoSlide() {
    this.autoSlideInterval = setInterval(() => {
      this.nextSlide();
    }, 2000); 
  }

  // Move to the next slide
  nextSlide() {
    if (this.currentIndex < this.images.length - this.visibleItems) {
      this.currentIndex++;
    } else {
      this.currentIndex = 0; 
    }
  }

  // Move to the previous slide
  prevSlide() {
    if (this.currentIndex > 0) {
      this.currentIndex--;
    } else {
      this.currentIndex = this.images.length - this.visibleItems; 
    }
  }
}

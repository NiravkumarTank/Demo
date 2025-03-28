import { Component, OnInit, OnDestroy } from '@angular/core';

@Component({
  selector: 'app-home-image-carousel',
  templateUrl: './home-image-carousel.component.html',
  styleUrls: ['./home-image-carousel.component.css']
})
export class HomeImageCarouselComponent implements OnInit, OnDestroy {

  currentIndex: number = 0;
  slides = [
    { image: 'Image/shoe2.avif' }, 
    { image: 'Image/shoe10.webp' },
    { image: 'Image/male-with-fashion-brown-leather-boots.jpg' },
    { image: 'Image/view-shoe-rack-stacking-pair-footwear.jpg' }
  ];
  interval: any; 

  constructor() { }

  ngOnInit(): void {
    this.startAutoSlide();
  }

  ngOnDestroy(): void {
    if (this.interval) {
      clearInterval(this.interval);
    }
  }

  prevSlide() {
    this.currentIndex = (this.currentIndex - 1 + this.slides.length) % this.slides.length;
  }

  nextSlide() {
    this.currentIndex = (this.currentIndex + 1) % this.slides.length;
  }

  startAutoSlide() {
    this.interval = setInterval(() => {
      this.nextSlide();
    }, 3000);
  }
}

import { Component, OnInit } from '@angular/core';
import { FooterPageComponent } from '../footer-page/footer-page.component';

@Component({
  selector: 'app-about-us-full',
  imports: [FooterPageComponent],
  templateUrl: './about-us-full.component.html',
  styleUrl: './about-us-full.component.css'
})
export class AboutUsFullComponent implements OnInit {
  ngOnInit(): void {
    window.scrollTo(0, 0); 
  }

}

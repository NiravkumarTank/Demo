import { Component } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { NavbarPageComponent } from './components/navbar-page/navbar-page.component';
import { filter } from 'rxjs';
import { CommonModule } from '@angular/common';
import { HttpserviceService } from './httpservice.service';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,NavbarPageComponent,CommonModule,HttpClientModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  isLoginPage = false;
  
  constructor(private router: Router,private httpService: HttpserviceService) {}

  ngOnInit(): void {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.isLoginPage = this.router.url === '/login' ||  this.router.url === '/register' ;
    });
  }
}

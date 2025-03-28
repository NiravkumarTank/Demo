import { RouterModule, Routes } from '@angular/router';
import { LoginPageComponent } from './components/login-page/login-page.component';
import { HomePageComponent } from './components/home-page/home-page.component';
import { RegisterPageComponent } from './components/register-page/register-page.component';
import { CategoryComponent } from './category/category/category.component';
import { ManCategoryComponent } from './category/man-category/man-category.component';
import { WomanCategoryComponent } from './category/woman-category/woman-category.component';
import { ChildrenCategoryComponent } from './category/children-category/children-category.component';
import { CartPageComponent } from './components/cart-page/cart-page.component';
import { OrderPageComponent } from './components/order-page/order-page.component';
import { AboutUsComponent } from './components/about-us/about-us.component';
import { FooterPageComponent } from './components/footer-page/footer-page.component';
import { AdminNavbarPageComponent } from './Admin/admin-navbar-page/admin-navbar-page.component';
import { ShowCategoryComponent } from './Admin/category/show-category/show-category.component';
import { CreateCategoryComponent } from './Admin/category/create-category/create-category.component';
import { AdminHomePageComponent } from './Admin/admin-home-page/admin-home-page.component';
import { ShowProductComponent } from './Admin/product/show-product/show-product.component';
import { CreateProductComponent } from './Admin/product/create-product/create-product.component';
import { PaidOrdersComponent } from './components/paid-orders/paid-orders.component';
import { ProductEditComponent } from './Admin/product/product-edit/product-edit.component';
import { AdminHomeComponent } from './Admin/admin-home/admin-home.component';
import { NgModule } from '@angular/core';
import { AboutUsFullComponent } from './components/about-us-full/about-us-full.component';
import { ShowUserComponent } from './Admin/show-user/show-user.component';
import { ShowReviewsComponent } from './Admin/show-reviews/show-reviews.component';
import { ProfilePageComponent } from './components/profile-page/profile-page.component';
import { PagenotfoundComponent } from './components/pagenotfound/pagenotfound.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';

export const routes: Routes = [
  // { path: '', component: HomePageComponent },
  {path:'',redirectTo:'home',pathMatch:'full'},
  { path: 'home', component: HomePageComponent },
  { path: 'about-us', component: AboutUsComponent },
  { path: 'login', component: LoginPageComponent },
  { path: 'register', component: RegisterPageComponent },
  { path: 'cart', component: CartPageComponent },
  { path: 'order', component: OrderPageComponent },
  { path: 'about', component: AboutUsFullComponent },

  {path:"paid-order",component:PaidOrdersComponent},
  {path:"profile",component:ProfilePageComponent},
  {path:"forgot-password",component:ForgotPasswordComponent},


  // { path: 'shose-logos', component: ShoesBrandLogosComponent },



  {
    path: '', component: AdminNavbarPageComponent,
    children: [
      { path: '', redirectTo: 'admin', pathMatch: 'full' },  
      { path: 'admin', component: AdminHomePageComponent, children: [
        { path: 'home', component: AdminHomeComponent }  ,
        { path: 'show-category', component: ShowCategoryComponent }  ,
        { path: 'create-category', component: CreateCategoryComponent }  ,
        { path: 'show-product', component: ShowProductComponent }  ,
        { path: 'create-product', component: CreateProductComponent }  ,
        { path: 'edit-product/:id', component: ProductEditComponent },
        { path: 'show-user', component: ShowUserComponent },
        { path: 'show-reviews', component: ShowReviewsComponent },
      ]},
   
    ]
  },
  

  { path: 'footer', component: FooterPageComponent },


  {
    path: 'category', component: CategoryComponent,
    children: [
      { path: '', redirectTo: 'man-category', pathMatch: 'full' },
      { path: 'man-category', component: ManCategoryComponent },
      { path: 'woman-category', component: WomanCategoryComponent },
      { path: 'children-category', component: ChildrenCategoryComponent },
    ]
  },

  // page not found 
  {path:"**",component:PagenotfoundComponent},

];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' })  
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
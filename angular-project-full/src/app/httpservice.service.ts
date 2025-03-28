import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CartResponse, Category, Product } from './Interface/interface-all';

@Injectable({
  providedIn: 'root'
})
export class HttpserviceService {

  constructor(private http: HttpClient) { }
  private apiUrl = 'https://localhost:7262/api'; 
 



  login(data:any){
    return  this.http.post(this.apiUrl+"/User/Login",data);
  }

  // register(data:any){
  //   return this.http.post(this.apiUrl+"/User/Registration",data);
  // }


  // register(formData: FormData) {
  //   return this.http.post<any>(this.apiUrl+"/User/Registration", formData);
  // }

  // // Method to verify OTP
  // verifyOtp(formData: FormData) {
  //   return this.http.post<any>(this.apiUrl+"/User/VerifyOtp", formData);
  // }

  register(formData: FormData): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/User/Registration`, formData, {
      withCredentials: true // Ensure cookies are sent with the request
    });
  }

  // Example for OTP Verification
  verifyOtp(verificationData: FormData): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/User/VerifyOtp`, verificationData, {
      withCredentials: true // Ensure cookies are sent with the request
    });
  }
  
  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl+"/Admin/ShowAllProduct");
  }

  


  getBySubcategory(subcategory: string): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/Admin/SubCategoryName/${subcategory}`);
  }
  



  addToCart(productId: number, userId: string, size: string): Observable<any> {
    const formData = new FormData();
    formData.append('ProductId', productId.toString());
    formData.append('UserId', userId);
    formData.append('ShoesNumber', size); // Add the size to the form data
  
    return this.http.post(this.apiUrl + "/Cart/AddToCart", formData);
  }
  

  userId:any = sessionStorage.getItem('UserId');

  getCartItems(userId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl+"/Cart"}/${userId}`);
  }




  updateQuantity(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Cart/AddToCart`, data);
  }
  
  DecreaseQuantity(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Cart/DecreaseStock`, data);
  }

  removeItem(userId: string, productId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/Cart/DeleteCart?uid=${userId}&pid=${productId}`);
  }



  updateSizeInCart(data:any): Observable<any>{
    return this.http.post(`${this.apiUrl}/Cart/update-size`, data);
  }



  totalCartNumber(userId: string): Observable<CartResponse>{
    return this.http.get<CartResponse>(`${this.apiUrl+"/Cart/total-cart-items"}/${userId}`);
  }





  getPincodeInfo(pincode: string): Observable<any> {
    return this.http.get(`https://api.postalpincode.in/pincode/${pincode}`);
  }

  placeOrder(userId: string, shippingAddress: string): Observable<any> {
    const orderData = {
      UserId: userId,
      ShippingAddress: shippingAddress,
      Status: 'pending',
    };
    return this.http.post(`${this.apiUrl}/Order/PlaceOrder`, orderData);
  }




  getSubCategories(): Observable<any> {
    return this.http.get<any>(this.apiUrl+"/Category/SubCategoryShow");
  }



  getCategories(): Observable<Category> {
    return this.http.get<any>(`${this.apiUrl}/Category/CategoryShow`);
  }

  addSubCategory(data: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/Category/SubCategoryAdd`, data);
  }


  updateProduct(formData: FormData): Observable<any> {
    const url = `https://localhost:7262/api/Admin/updateProduct`;  // Replace with your API URL
    return this.http.put(url, formData);
  }


  getProductById(productId: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/Admin/GetById/${productId}`);
  }
  


  private apiUrl1 = 'https://localhost:7262/api/Order';

  
  getPendingOrders(userId: string): Observable<any> {
    return this.http.get(`${this.apiUrl1}/GetPendingOrders?userId=${userId}`);
  }

  getPaidOrders(userId: string): Observable<any> {
    return this.http.get(`${this.apiUrl1}/GetPaidOrders?userId=${userId}`);
  }

  removeOrder(orderId: string, productId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl1}/RemoveOrder?orderId=${orderId}&productId=${productId}`);
  }

  createPayment(userId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/Payment/createpayment/${userId}`, {});
  }

  generatePdf(userId: string) {
    const url = `https://localhost:7262/api/Cart/PdfGenerate/${userId}`;
    return this.http.get(url, { responseType: 'blob' });
  }





 // Step 1: Request OTP
 requestOtp(email: string): Observable<any> {
  return this.http.post(`${this.apiUrl}/ForgotPassword`, { email });
}

// Step 2: Verify OTP
verifyOtpForgotpassword(userId: number, otp: number): Observable<any> {
  return this.http.post(`${this.apiUrl}/VerifyOtp`, { userId, otp });
}

// Step 3: Reset Password
resetPassword(userId: number, newPassword: string): Observable<any> {
  return this.http.post(`${this.apiUrl}/ResetPassword`, { userId, newPassword });
}
}

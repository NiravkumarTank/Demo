import { Component, inject } from '@angular/core';
import { HttpserviceService } from '../../httpservice.service';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cart-page',
  imports: [CommonModule, FormsModule],
  templateUrl: './cart-page.component.html',
  styleUrls: ['./cart-page.component.css']
})
export class CartPageComponent {
  cartItems: any[] = [];
  totalPrice: number = 0;
  userId: any = sessionStorage.getItem('UserId');
  apiMessage: string = '';
  apiMessageType: string = '';
  showAddressPopup: boolean = false;
  blockNumber: string = '';
  houseAddress: string = '';
  area: string = '';
  areas: string[] = [];
  state: string = '';
  country: string = '';
  pinCode: string = '';
  city: string = '';
  availableSizes: number[] = [];
  loading: boolean = false;  // Loading flag

  constructor(private http: HttpserviceService) {}

  ngOnInit() {
    this.loadCartItems();
  }

  loadCartItems() {
    const userId = sessionStorage.getItem('UserId');
    if (!userId) {
      window.location.href = '/login';
      return;
    }

    this.http.getCartItems(userId).subscribe((data: any) => {
      if (data.message) {
        this.apiMessage = data.message;
        this.apiMessageType = 'error';
      }

      if (Array.isArray(data)) {
        this.cartItems = data;
        // console.log(data);

        this.cartItems.forEach(item => {
          // Set the default selected size
          item.selectedSize = item.shoesNumber;

          // Set available sizes based on the category
          if (item.categoryName === 'Man') {
            item.availableSizes = [7, 8, 9, 10, 11, 12];
          } else if (item.categoryName === 'Woman') {
            item.availableSizes = [4, 5, 6, 7, 8, 9];
          } else if (item.categoryName === 'Children') {
            item.availableSizes = [1, 2, 3, 4, 5, 6];
          }


          // Recalculate the total price
          this.calculateTotalPrice();
        });
      }
    });
  }

  calculateTotalPrice() {
    this.totalPrice = this.cartItems.reduce((total, item) => total + (item.price * item.stock), 0);
  }

  isSubmitDisabled(): boolean {
    return !this.blockNumber || !this.pinCode || !this.houseAddress;
  }

  onSizeChange(item: any) {
    const data = new FormData();
    data.append('pid', item.productId);
    data.append('uid', this.userId);
    data.append('newSize', item.selectedSize.toString());

    this.http.updateSizeInCart(data).subscribe(
      (response: any) => {
        this.apiMessage = response.message;
        this.apiMessageType = 'success';
        setTimeout(() => {
          this.apiMessage = '';
          this.apiMessageType = '';
        }, 3000);
      },
      (error) => {
        this.apiMessage = 'An error occurred while updating size';
        this.apiMessageType = 'error';
      }
    );
  }

  closeAddressPopup() {
    this.showAddressPopup = false;
  }

  onPinCodeChange() {
    if (this.pinCode && this.pinCode.length === 6) {
      this.fetchLocationDetails();
    }
  }

  fetchLocationDetails() {
    this.http.getPincodeInfo(this.pinCode).subscribe(
      (data: any) => {
        if (data && data[0] && data[0].PostOffice && data[0].PostOffice.length > 0) {
          const postOffices = data[0].PostOffice;
          this.areas = postOffices.map((postOffice: any) => postOffice.Name);
          this.city = postOffices[0].District;
          this.state = postOffices[0].State;
          this.country = postOffices[0].Country;
          
          if (this.areas.length === 1) {
            this.area = this.areas[0];
          }
        }
      },
      (error) => {
        console.error('Error fetching location details:', error);
        alert('Failed to fetch location details. Please try again.');
      }
    );
  }

  onOrderClick(): void {
    const userId = sessionStorage.getItem('UserId');
    if (!userId) {
      alert('User is not logged in.');
      return;
    }

    this.openAddressPopup();
  }

  openAddressPopup() {
    this.showAddressPopup = true;
  }

  placeOrder(address: string) {
    const userId = sessionStorage.getItem('UserId');
    if (!userId) {
      alert('User is not logged in.');
      return;
    }

    this.http.getCartItems(userId).subscribe((cartData) => {
      if (!cartData || cartData.length === 0) {
        alert('Your cart is empty. Add items to your cart before placing an order.');
        return;
      }

      this.http.placeOrder(userId, address).subscribe(
        (response) => {
          if (response.orderId) {
            // alert('Order placed successfully!');
            // window.location.reload();
          } else {
            alert('Failed to place order: ' + response.message);
          }
        },
        (error) => {
          console.error('Error placing the order:', error);
          alert('Error placing the order. See console for details.');
        }
      );
    });
  }

  onSubmit(form: NgForm) {
    if (!form.valid) {
      alert('Please fill in all the required fields.');
      return;
    }

    this.loading = true;
    const fullAddress = `${this.blockNumber}, ${this.houseAddress}, ${this.area}, ${this.city}, ${this.state}, ${this.country}`;
    
    // window.scrollTo(0, 0);
    this.placeOrder(fullAddress);
  
    setTimeout(() => {
      this.loading = false;
      window.location.href = '/order'; 
    }, 3000);
  
    this.closeAddressPopup();
  }

  updateQuantity(productId: string, size: number) {
    const data = new FormData();
    data.append('ProductId', productId);
    data.append('UserId', this.userId);
    data.append('Size', size.toString());

    this.http.updateQuantity(data).subscribe(
      (response: any) => {
        this.apiMessage = response.message;
        this.apiMessageType = 'success';
        setTimeout(() => {
          this.apiMessage = '';
          this.apiMessageType = '';
        }, 1000);
        this.loadCartItems();
      },
      (error) => {
        this.apiMessage = 'An error occurred';
        this.apiMessageType = 'error';
      }
    );
  }

  decreaseQuantity(productId: string) {
    const data = new FormData();
    data.append('ProductId', productId);
    data.append('UserId', this.userId);

    this.http.DecreaseQuantity(data).subscribe(
      (response: any) => {
        this.apiMessage = response.message;
        this.apiMessageType = 'success';
        setTimeout(() => {
          window.location.reload();
        }, 1000);
      },
      (error) => {
        this.apiMessage = 'An error occurred';
        this.apiMessageType = 'error';
      }
    );
  }

  removeCart(productId: number) {
    this.http.removeItem(this.userId, productId).subscribe(
      (response: any) => {
        this.apiMessage = response.message;
        this.apiMessageType = 'success';
        setTimeout(() => {
          window.location.reload();
        }, 1000);
      },
      (error) => {
        this.apiMessage = 'An error occurred';
        this.apiMessageType = 'error';
      }
    );
  }
}

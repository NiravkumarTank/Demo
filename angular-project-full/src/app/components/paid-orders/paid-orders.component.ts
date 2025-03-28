import { Component, OnInit } from '@angular/core';
import { HttpserviceService } from '../../httpservice.service';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-paid-orders',
  imports: [CommonModule, FormsModule], // Add these modules here
  templateUrl: './paid-orders.component.html',
  styleUrls: ['./paid-orders.component.css']
})
export class PaidOrdersComponent implements OnInit {
  paidOrders: { [key: string]: { orders: any[] } } = {};
  loadingPaid: boolean = true;
  errorMessage: string = '';
  successMessage: string = ''; // Added for success feedback
  userId: string = sessionStorage.getItem('UserId') || '';
  isReviewOverlayVisible: boolean = false;
  reviewComment: string = '';
  orderToReview: any = null;

  constructor(private orderService: HttpserviceService, private router: Router, private http: HttpClient) { }

  ngOnInit(): void {
    if (this.userId) {
      this.fetchPaidOrders();
    } else {
      this.errorMessage = 'User ID not found. Please log in.';
      this.loadingPaid = false;
    }
  }

  openReviewOverlay(order: any): void {
    this.isReviewOverlayVisible = true;
    this.orderToReview = order;
    this.reviewComment = '';
  }

  closeReviewOverlay(): void {
    this.isReviewOverlayVisible = false;
    this.reviewComment = '';
    this.successMessage = ''; // Reset success message when overlay is closed
  }

  // Function to handle the review submission
  // submitReview(order: any): void {
  //   if (!this.reviewComment.trim()) {
  //     this.errorMessage = 'Please enter a comment before submitting.';
  //     return;
  //   }

  //   const reviewData = {
  //     UserId: this.userId,
  //     ProductId: order.productId,
  //     Comment: this.reviewComment,
  //   };

  //   this.http.post('https://localhost:7262/api/Review/addReview', reviewData)
  //     .subscribe(
  //       (response) => {
  //         console.log('Review submitted successfully', response);
  //         this.successMessage = 'Review submitted successfully!'; // Set success message
  //         this.closeReviewOverlay(); // Close overlay after submitting
  //       },
  //       (error) => {
  //         this.errorMessage = 'Failed to submit review. Please try again.';
  //         console.error('Error submitting review:', error);
  //       }
  //     );
  // }

  submitReview(order: any): void {
    if (!this.reviewComment.trim()) {
      this.errorMessage = 'Please enter a comment before submitting.';
      return;
    }
  
    const reviewData = {
      UserId: this.userId,
      ProductId: order.productId,
      Comment: this.reviewComment,
    };
  
    this.http.post('https://localhost:7262/api/Review/addReview', reviewData)
      .subscribe(
        (response) => {
          console.log('Review submitted successfully', response);
          
          // Set success message
          this.successMessage = 'Review submitted successfully!';
          
          // Hide success message after 3 seconds and reset form
          setTimeout(() => {
            this.successMessage = ''; // Clear the success message
            this.closeReviewOverlay(); // Close the review overlay
            this.reviewComment = ''; // Clear the review comment
          }, 1000); // 3 seconds delay
          
        },
        (error) => {
          this.errorMessage = 'Failed to submit review. Please try again.';
          console.error('Error submitting review:', error);
        }
      );
  }
  
  fetchPaidOrders(): void {
    this.orderService.getPaidOrders(this.userId).subscribe(
      (response) => {
        this.groupOrdersByDate(response);
        this.loadingPaid = false;
      },
      (error) => {
        console.error('Error fetching paid orders:', error);
        this.errorMessage = 'Error fetching paid orders. Please try again later.';
        this.loadingPaid = false;
      }
    );
  }

  groupOrdersByDate(orders: any[]): void {
    const groupedOrders: { [key: string]: { orders: any[] } } = {};

    orders.forEach(orderGroup => {
      const date = orderGroup.orderDate;
      if (!groupedOrders[date]) {
        groupedOrders[date] = { orders: [] };
      }
      groupedOrders[date].orders = [...orderGroup.orders];
    });

    this.paidOrders = groupedOrders;
  }

  navigateToOrderPaid(): void {
    this.router.navigateByUrl('/order');
  }

  getPaidOrderKeys(): string[] {
    return Object.keys(this.paidOrders);
  }
}

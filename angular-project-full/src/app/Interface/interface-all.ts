export interface Product {
    category: string;
    description: string;
    imageUrl: string;
    name: string;
    price: number;
    productId: number;
    stock: number;
    subcategory: string;

  }
  



export  interface Category {
    categories: boolean;
    c_Id: string;
    c_Name: string;
  }

  
    
  export   interface Subcategory {
    subcategories:boolean;
      sc_id: string;
      sc_Name: string;
      c_Id:number
    }
    


    export  interface CartResponse {
        total_cart_items: number;
      }
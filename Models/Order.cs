using System;
using System.Collections.Generic;

namespace Models
{
    public class Order
    {
    public int Id { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerAddress { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string? Status { get; set; } // Pending, Paid, Delivered
    public DateTime CreatedAt { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    public decimal TotalPrice { get; set; } // <-- Add this property
    }

    public class OrderItem
    {
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    }
}

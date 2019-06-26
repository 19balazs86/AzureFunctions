﻿using System;

namespace AzureFunctions.Models
{
  public class Order
  {
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
  }
}
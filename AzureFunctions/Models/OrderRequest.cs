﻿using System.ComponentModel.DataAnnotations;

namespace AzureFunctions.Models
{
  public class OrderRequest
  {
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

    [StringLength(100, MinimumLength = 1)]
    public string ProductName { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
  }
}
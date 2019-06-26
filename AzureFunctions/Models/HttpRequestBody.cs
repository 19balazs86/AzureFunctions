﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AzureFunctions.Models
{
  public class HttpRequestBody<T> where T : class
  {
    public bool IsValid { get; private set; }
    public T Value { get; private set; }

    public IEnumerable<ValidationResult> ValidationResults { get; private set; }

    public HttpRequestBody(bool isValid, T value, IEnumerable<ValidationResult> validationResults = null)
    {
      IsValid           = isValid;
      Value             = value;
      ValidationResults = validationResults ?? Enumerable.Empty<ValidationResult>();
    }

    public HttpRequestBody(bool isValid, T value, ValidationResult validationResult)
      : this(isValid, value, new[] { validationResult })
    {
      
    }

    public string ValidationString() => string.Join(", ", ValidationResults.Select(v => v.ErrorMessage));
  }
}
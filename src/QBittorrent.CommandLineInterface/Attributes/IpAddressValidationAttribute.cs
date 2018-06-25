using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;

namespace QBittorrent.CommandLineInterface.Attributes
{
    public class IpAddressValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            if (value is string str && IPAddress.TryParse(str, out _))
                return ValidationResult.Success;

            return new ValidationResult($"The value {value} is not a correct IP address.");
        }
    }
}

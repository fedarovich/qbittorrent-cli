using System.ComponentModel.DataAnnotations;
using System.Net;

namespace QBittorrent.CommandLineInterface.Attributes
{
    public class IpNetworkValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            if (value is string str && IPNetwork.TryParse(str, out _))
                return ValidationResult.Success;

            return new ValidationResult($"The value {value} is not a correct IP network.");
        }
    }
}

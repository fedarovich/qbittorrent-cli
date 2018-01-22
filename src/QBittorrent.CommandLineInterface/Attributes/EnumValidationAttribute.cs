using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace QBittorrent.CommandLineInterface.Attributes
{
    public class EnumValidationAttribute : ValidationAttribute
    {
        public EnumValidationAttribute(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum)
                throw new ArgumentException("The type must be an enumeration.", nameof(enumType));

            EnumType = enumType;
        }

        public Type EnumType { get; }

        public bool CaseSensitive { get; set; }

        public bool AllowEmpty { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null && AllowEmpty)
                return ValidationResult.Success;

            if (value is string str && 
                (EnumHelper.TryParse(EnumType, str, !CaseSensitive, out _) || (AllowEmpty && string.IsNullOrEmpty(str)))
            )
                return ValidationResult.Success;

            var values = string.Join(", ", Enum.GetValues(EnumType).Cast<object>());
            if (!CaseSensitive)
            {
                values = values.ToLowerInvariant();
            }
            return new ValidationResult($"The values for {validationContext.DisplayName} must be one of the following: {values}.");
        }
    }
}

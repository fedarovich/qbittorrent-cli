using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;

namespace QBittorrent.CommandLineInterface.Attributes
{
    public class IpEndpointValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            if (value is string str && TryParse(str.AsSpan(), out _))
                return ValidationResult.Success;

            return new ValidationResult($"The value {value} is not a correct IP endpoint.");
        }

        private static bool TryParse(ReadOnlySpan<char> s, out IPEndPoint result)
        {
            int addressLength = s.Length;  // If there's no port then send the entire string to the address parser
            int lastColonPos = s.LastIndexOf(':');

            // Look to see if this is an IPv6 address with a port.
            if (lastColonPos > 0)
            {
                if (s[lastColonPos - 1] == ']')
                {
                    addressLength = lastColonPos;
                }
                // Look to see if this is IPv4 with a port (IPv6 will have another colon)
                else if (s.Slice(0, lastColonPos).LastIndexOf(':') == -1)
                {
                    addressLength = lastColonPos;
                }
            }

#if NETFRAMEWORK
            if (IPAddress.TryParse(GetString(s.Slice(0, addressLength)), out var address))
            {
                uint port = 0;
                if (addressLength == s.Length ||
                    (uint.TryParse(GetString(s.Slice(addressLength + 1)), NumberStyles.None, CultureInfo.InvariantCulture, out port) && port <= IPEndPoint.MaxPort))

                {
                    result = new IPEndPoint(address, (int)port);
                    return true;
                }
            }

            unsafe static string GetString(in ReadOnlySpan<char> span)
            {
                fixed (char* p = span)
                {
                    return new string(p, 0, span.Length);
                }
            }
#else
            if (IPAddress.TryParse(s.Slice(0, addressLength), out var address))
            {
                uint port = 0;
                if (addressLength == s.Length ||
                    (uint.TryParse(s.Slice(addressLength + 1), NumberStyles.None, CultureInfo.InvariantCulture, out port) && port <= IPEndPoint.MaxPort))

                {
                    result = new IPEndPoint(address, (int)port);
                    return true;
                }
            }
#endif

            result = null;
            return false;
        }
    }
}

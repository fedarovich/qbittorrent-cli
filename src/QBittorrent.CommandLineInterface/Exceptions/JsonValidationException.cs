using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NJsonSchema.Validation;

namespace QBittorrent.CommandLineInterface.Exceptions
{
    [Serializable]
    public class JsonValidationException : Exception
    {
        public JsonValidationException(string message, ICollection<ValidationError> errors) : base(message)
        {
            Errors = errors.ToArray();
        }

        public IReadOnlyCollection<ValidationError> Errors { get; }

        protected JsonValidationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}

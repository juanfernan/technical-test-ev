using System;
using System.Collections.Generic;
using System.Text;

namespace TestEv.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public string Code { get; }

        public DomainException(string message, string code = "DOMAIN_ERROR") : base(message)
        {
            Code = code;
        }
    }

    public class EntityNotFoundException : DomainException
    {
        public string EntityType { get; }
        public string EntityId { get; }

        public EntityNotFoundException(string entityType, string entityId)
            : base($"{entityType} with ID '{entityId}' not found", "ENTITY_NOT_FOUND")
        {
            EntityType = entityType;
            EntityId = entityId;
        }
    }

    public class ValidationException : DomainException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("Validation error occurred", "VALIDATION_ERROR")
        {
            Errors = errors;
        }

        public ValidationException(string field, string error)
            : base($"Validation error on field '{field}': {error}", "VALIDATION_ERROR")
        {
            Errors = new Dictionary<string, string[]> { { field, new[] { error } } };
        }
    }

    public class AuthenticationException : DomainException
    {
        public AuthenticationException(string message = "Authentication failed")
            : base(message, "AUTHENTICATION_ERROR")
        {
        }
    }

    public class AuthorizationException : DomainException
    {
        public AuthorizationException(string message = "Not authorized to perform this action")
            : base(message, "AUTHORIZATION_ERROR")
        {
        }
    }

}

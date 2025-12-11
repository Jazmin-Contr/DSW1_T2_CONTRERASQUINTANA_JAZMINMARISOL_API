namespace library_api.Domain.Exceptions
{
    public class DuplicateEntityException : DomainException
    {
        public DuplicateEntityException(string entityName, string field, string value)
            : base($"{entityName} with {field} '{value}' already exists.")
        {
        }
    }
}
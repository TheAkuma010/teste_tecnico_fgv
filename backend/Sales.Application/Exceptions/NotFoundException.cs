namespace Sales.Application.Exceptions;

public sealed class NotFoundException : ApplicationException
{
    public NotFoundException(string entityName, object id)
        : base($"{entityName} com identificador '{id}' não foi encontrado.")
    {
    }
}
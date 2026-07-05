namespace Library.Application.Common;

/// <summary>Base type for expected, mappable application errors.</summary>
public abstract class AppException(string message) : Exception(message)
{
    public abstract ErrorType ErrorType { get; }
}

public class NotFoundException : AppException
{
    public override ErrorType ErrorType => ErrorType.NotFound;
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string entity, Guid id) : base($"{entity} with id '{id}' was not found.") { }
}

public class ValidationException(string message) : AppException(message)
{
    public override ErrorType ErrorType => ErrorType.Validation;
}

public class ConflictException(string message) : AppException(message)
{
    public override ErrorType ErrorType => ErrorType.Conflict;
}

public class NotCreatedException(string message) : AppException(message)
{
    public override ErrorType ErrorType => ErrorType.NotCreated;
}

public class UnauthorizedException(string message) : AppException(message)
{
    public override ErrorType ErrorType => ErrorType.Unauthorized;
}

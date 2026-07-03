namespace Library.Application.Common;

/// <summary>Base type for expected, mappable application errors.</summary>
public abstract class AppException : Exception
{
    public abstract ErrorType ErrorType { get; }

    protected AppException(string message) : base(message) { }
}

public class NotFoundException : AppException
{
    public override ErrorType ErrorType => ErrorType.NotFound;
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string entity, Guid id) : base($"{entity} with id '{id}' was not found.") { }
}

public class ValidationException : AppException
{
    public override ErrorType ErrorType => ErrorType.Validation;
    public ValidationException(string message) : base(message) { }
}

public class ConflictException : AppException
{
    public override ErrorType ErrorType => ErrorType.Conflict;
    public ConflictException(string message) : base(message) { }
}

public class NotCreatedException : AppException
{
    public override ErrorType ErrorType => ErrorType.NotCreated;
    public NotCreatedException(string message) : base(message) { }
}

public class UnauthorizedException : AppException
{
    public override ErrorType ErrorType => ErrorType.Unauthorized;
    public UnauthorizedException(string message) : base(message) { }
}

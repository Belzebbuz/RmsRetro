using Grpc.Core;

namespace RmsRetro.Abstractions.Exceptions;

[GenerateSerializer]
public class DomainException : Exception
{
	[Id(0)]
	public StatusCode Status {get;}

	private DomainException(StatusCode status)
	{
		Status = status;
	}
	private DomainException(StatusCode status, string message) : base(message)
	{
		Status = status;
	}

	public static DomainException NotFound(string message)
		=> new(StatusCode.NotFound, $"Не найдено: {message}");

	public static DomainException Unauthenticated()
		=> new (StatusCode.Unauthenticated, "Пользователь не существует");

	public static DomainException Unauthorized()
		=> new (StatusCode.PermissionDenied, "Пользователь не имеет прав");

	public static DomainException Internal()
		=> new (StatusCode.Internal, "Внутренняя ошибка");
}
namespace TaskFlow.Api.Contracts.Tasks;

public record TaskResponse(Guid Id, string Title, string Status);
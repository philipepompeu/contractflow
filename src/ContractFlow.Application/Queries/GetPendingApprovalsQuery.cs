using MediatR;

namespace ContractFlow.Application.Queries;

public sealed record GetPendingApprovalsQuery(int Page = 1, int Size = 50): IRequest<PageResult<PendingApprovalDto>>;

public sealed record PendingApprovalDto(Guid Id, Guid ContractId, DateTimeOffset CreatedAt);

public sealed record PageResult<T>(IReadOnlyList<T> Items, int Page, int Size, int Total, int TotalPages);

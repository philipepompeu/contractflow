using MediatR;

namespace ContractFlow.Application.Commands;

public sealed record ApproveDocumentCommand(Guid DocumentId): IRequest<ApproveDocumentResult>;

public sealed record ApproveDocumentResult(Guid DocumentId, Guid ContractId, DateTime ApprovedAt);
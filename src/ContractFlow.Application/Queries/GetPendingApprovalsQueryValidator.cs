using FluentValidation;

namespace ContractFlow.Application.Queries;

public sealed class GetPendingApprovalsQueryValidator: AbstractValidator<GetPendingApprovalsQuery>
{
    public GetPendingApprovalsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Size).InclusiveBetween(1, 100);
    }
}
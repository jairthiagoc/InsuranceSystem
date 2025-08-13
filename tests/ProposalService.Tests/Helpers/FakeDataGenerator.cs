using Bogus;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;
using ProposalService.Ports.Inbound;
using ProposalService.Ports.Inbound.Shared;

namespace ProposalService.Tests.Helpers;

public static class FakeDataGenerator
{
    private static readonly Faker _faker = new Faker("pt_BR");

    public static Faker<Proposal> ProposalFaker => new Faker<Proposal>("pt_BR")
        .CustomInstantiator(f => new Proposal(
            f.Person.FullName,
            f.Person.Email,
            f.PickRandom("Auto Insurance", "Home Insurance", "Life Insurance", "Health Insurance", "Travel Insurance"),
            f.Random.Decimal(1000, 100000),
            f.Random.Decimal(100, 10000)
        ));

    public static Faker<CreateProposalRequest> CreateProposalRequestFaker => new Faker<CreateProposalRequest>("pt_BR")
        .CustomInstantiator(f => new CreateProposalRequest(
            f.Person.FullName,
            f.Person.Email,
            f.PickRandom("Auto Insurance", "Home Insurance", "Life Insurance", "Health Insurance", "Travel Insurance"),
            f.Random.Decimal(1000, 100000),
            f.Random.Decimal(100, 10000)
        ));

    public static Faker<UpdateProposalStatusRequest> UpdateProposalStatusRequestFaker => new Faker<UpdateProposalStatusRequest>("pt_BR")
        .CustomInstantiator(f => 
        {
            var status = f.PickRandom<ProposalStatus>();
            var rejectionReason = status == ProposalStatus.Rejected ? f.Lorem.Sentence() : null;
            return new UpdateProposalStatusRequest(f.Random.Guid(), status, rejectionReason);
        });

    // Métodos utilitários
    public static Proposal GenerateProposal(Guid? proposalId = null) => ProposalFaker.Generate();
    public static IEnumerable<Proposal> GenerateProposals(int count = 5) => ProposalFaker.Generate(count);
    
    public static CreateProposalRequest GenerateCreateProposalRequest() => CreateProposalRequestFaker.Generate();
    public static UpdateProposalStatusRequest GenerateUpdateProposalStatusRequest() => UpdateProposalStatusRequestFaker.Generate();

    // Geradores específicos para cenários de teste
    public static Proposal GenerateApprovedProposal()
    {
        var proposal = GenerateProposal();
        proposal.Approve();
        return proposal;
    }

    public static Proposal GenerateRejectedProposal(string reason = null)
    {
        var proposal = GenerateProposal();
        proposal.Reject(reason ?? _faker.Lorem.Sentence());
        return proposal;
    }

    public static Proposal GenerateUnderReviewProposal()
    {
        var proposal = GenerateProposal();
        return proposal; // Propostas já começam em UnderReview
    }

    public static UpdateProposalStatusRequest GenerateApprovalRequest() => new UpdateProposalStatusRequest(Guid.NewGuid(), ProposalStatus.Approved, null);
    public static UpdateProposalStatusRequest GenerateRejectionRequest() => new UpdateProposalStatusRequest(Guid.NewGuid(), ProposalStatus.Rejected, _faker.Lorem.Sentence());
} 
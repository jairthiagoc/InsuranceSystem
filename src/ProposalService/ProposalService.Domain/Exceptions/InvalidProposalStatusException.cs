namespace ProposalService.Domain.Exceptions;

public class InvalidProposalStatusException : Exception
{
    public InvalidProposalStatusException(string message) : base(message)
    {
    }

    public InvalidProposalStatusException(string message, Exception innerException) : base(message, innerException)
    {
    }
} 
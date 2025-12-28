using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChertSdk;

/// <summary>
/// Privacy manager for stealth addresses and private transactions.
/// </summary>
public class PrivacyManager
{
    private readonly ChertClient _client;

    internal PrivacyManager(ChertClient client)
    {
        _client = client;
    }

    public async Task<StealthKeys> GenerateStealthKeysAsync()
    {
        return await _client.RpcCallAsync<StealthKeys>("privacy_generateStealthKeys");
    }

    public async Task<StealthAccount> CreateStealthAccountAsync(string viewKey, string spendPublicKey, StealthKeys? keys = null)
    {
        var address = $"stealth_{viewKey[..20]}{spendPublicKey[..20]}".Replace(" ", "").ToLower();
        return new StealthAccount
        {
            Address = address,
            ViewKey = viewKey,
            SpendPublicKey = spendPublicKey,
            Keys = keys
        };
    }

    public async Task<string> SendPrivateTransactionAsync(PrivateTransactionRequest request, string recipientViewKey, string recipientSpendKey)
    {
        var result = await _client.RpcCallAsync<object>("sendPrivateTransaction", new object[] { request, recipientViewKey, recipientSpendKey });
        return result?.ToString() ?? throw new PrivacyException("Invalid private transaction response");
    }
}

/// <summary>
/// Staking manager for staking and delegation operations.
/// </summary>
public class StakingManager
{
    private readonly ChertClient _client;

    internal StakingManager(ChertClient client)
    {
        _client = client;
    }

    public async Task<List<Validator>> GetValidatorsAsync()
    {
        return await _client.RpcCallAsync<List<Validator>>("getValidators");
    }

    public async Task<Validator> GetValidatorAsync(string address)
    {
        return await _client.RpcCallAsync<Validator>("getValidator", new object[] { address });
    }

    public async Task<string> DelegateAsync(string delegatorAddress, string validatorAddress, string amount, string fee)
    {
        var result = await _client.RpcCallAsync<object>("staking_delegate", new object[] { delegatorAddress, validatorAddress, amount, fee });
        return result?.ToString() ?? throw new StakingException("Invalid delegation response");
    }

    public async Task<List<Delegation>> GetDelegationsAsync(string delegatorAddress)
    {
        return await _client.RpcCallAsync<List<Delegation>>("getDelegations", new object[] { delegatorAddress });
    }

    public async Task<StakingRewards> GetStakingRewardsAsync(string delegatorAddress)
    {
        return await _client.RpcCallAsync<StakingRewards>("getStakingRewards", new object[] { delegatorAddress });
    }
}

/// <summary>
/// Governance manager for governance operations and proposals.
/// </summary>
public class GovernanceManager
{
    private readonly ChertClient _client;

    internal GovernanceManager(ChertClient client)
    {
        _client = client;
    }

    public async Task<List<Proposal>> GetProposalsAsync(int limit = 10)
    {
        return await _client.RpcCallAsync<List<Proposal>>("governance_getProposals", new object[] { new { limit } });
    }

    public async Task<Proposal> GetProposalAsync(string proposalId)
    {
        return await _client.RpcCallAsync<Proposal>("governance_getProposal", new object[] { proposalId });
    }

    public async Task<string> CreateProposalAsync(string title, string description, string proposerAddress, string fee)
    {
        var result = await _client.RpcCallAsync<object>("governance_createProposal", new object[] { title, description, proposerAddress, fee });
        return result?.ToString() ?? throw new GovernanceException("Invalid proposal creation response");
    }

    public async Task<string> VoteAsync(string proposalId, string voterAddress, VoteOption option, string fee)
    {
        var result = await _client.RpcCallAsync<object>("governance_vote", new object[] { proposalId, voterAddress, option, fee });
        return result?.ToString() ?? throw new GovernanceException("Invalid vote response");
    }

    public async Task<VoteTally> GetProposalVotesAsync(string proposalId)
    {
        return await _client.RpcCallAsync<VoteTally>("governance_getProposalVotes", new object[] { proposalId });
    }
}

// Additional types needed for the managers
public class PrivateTransactionRequest
{
    public StealthKeys SenderKeys { get; set; } = new();
    public string RecipientViewKey { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Fee { get; set; } = string.Empty;
    public PrivacyLevel PrivacyLevel { get; set; } = PrivacyLevel.Stealth;
    public ulong Nonce { get; set; }
    public string? Memo { get; set; }
}
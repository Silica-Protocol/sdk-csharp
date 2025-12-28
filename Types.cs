using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ChertSdk;

/// <summary>
/// Account information.
/// </summary>
public class Account
{
    /// <summary>
    /// Account address.
    /// </summary>
    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Public key.
    /// </summary>
    [JsonProperty("public_key")]
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Private key (only available for owned accounts).
    /// </summary>
    [JsonProperty("private_key")]
    public string? PrivateKey { get; set; }
}

/// <summary>
/// Account balance information.
/// </summary>
public class Balance
{
    /// <summary>
    /// Available balance.
    /// </summary>
    [JsonProperty("available")]
    public string Available { get; set; } = string.Empty;

    /// <summary>
    /// Pending balance.
    /// </summary>
    [JsonProperty("pending")]
    public string Pending { get; set; } = string.Empty;

    /// <summary>
    /// Total balance.
    /// </summary>
    [JsonProperty("total")]
    public string Total { get; set; } = string.Empty;
}

/// <summary>
/// Transaction request.
/// </summary>
public class TransactionRequest
{
    /// <summary>
    /// Recipient address.
    /// </summary>
    [JsonProperty("to")]
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Amount to send.
    /// </summary>
    [JsonProperty("amount")]
    public string Amount { get; set; } = string.Empty;

    /// <summary>
    /// Transaction fee.
    /// </summary>
    [JsonProperty("fee")]
    public string Fee { get; set; } = string.Empty;

    /// <summary>
    /// Optional memo.
    /// </summary>
    [JsonProperty("memo")]
    public string? Memo { get; set; }

    /// <summary>
    /// Nonce (auto-calculated if not provided).
    /// </summary>
    [JsonProperty("nonce")]
    public ulong? Nonce { get; set; }
}

/// <summary>
/// Transaction status.
/// </summary>
public enum TransactionStatus
{
    /// <summary>
    /// Transaction is pending.
    /// </summary>
    Pending,

    /// <summary>
    /// Transaction is confirmed.
    /// </summary>
    Confirmed,

    /// <summary>
    /// Transaction failed.
    /// </summary>
    Failed,

    /// <summary>
    /// Transaction was rejected.
    /// </summary>
    Rejected
}

/// <summary>
/// Transaction data.
/// </summary>
public class Transaction
{
    /// <summary>
    /// Transaction hash.
    /// </summary>
    [JsonProperty("hash")]
    public string Hash { get; set; } = string.Empty;

    /// <summary>
    /// Sender address.
    /// </summary>
    [JsonProperty("from")]
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// Recipient address.
    /// </summary>
    [JsonProperty("to")]
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Amount.
    /// </summary>
    [JsonProperty("amount")]
    public string Amount { get; set; } = string.Empty;

    /// <summary>
    /// Fee.
    /// </summary>
    [JsonProperty("fee")]
    public string Fee { get; set; } = string.Empty;

    /// <summary>
    /// Transaction memo.
    /// </summary>
    [JsonProperty("memo")]
    public string? Memo { get; set; }

    /// <summary>
    /// Block height.
    /// </summary>
    [JsonProperty("block_height")]
    public ulong? BlockHeight { get; set; }

    /// <summary>
    /// Transaction status.
    /// </summary>
    [JsonProperty("status")]
    public TransactionStatus Status { get; set; }

    /// <summary>
    /// Timestamp.
    /// </summary>
    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Nonce.
    /// </summary>
    [JsonProperty("nonce")]
    public ulong Nonce { get; set; }
}

/// <summary>
/// Privacy types.
/// </summary>
public class KeyPair
{
    [JsonProperty("public")]
    public string Public { get; set; } = string.Empty;

    [JsonProperty("secret")]
    public string Secret { get; set; } = string.Empty;
}

public class StealthKeys
{
    [JsonProperty("view_keypair")]
    public KeyPair ViewKeypair { get; set; } = new();

    [JsonProperty("spend_keypair")]
    public KeyPair SpendKeypair { get; set; } = new();
}

public enum PrivacyLevel
{
    [JsonProperty("stealth")]
    Stealth,

    [JsonProperty("encrypted")]
    Encrypted
}

public class StealthAccount
{
    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("view_key")]
    public string ViewKey { get; set; } = string.Empty;

    [JsonProperty("spend_public_key")]
    public string SpendPublicKey { get; set; } = string.Empty;

    [JsonProperty("keys")]
    public StealthKeys? Keys { get; set; }
}

/// <summary>
/// Staking types.
/// </summary>
public enum ValidatorStatus
{
    [JsonProperty("active")]
    Active,

    [JsonProperty("inactive")]
    Inactive,

    [JsonProperty("jailed")]
    Jailed
}

public class Validator
{
    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("voting_power")]
    public string VotingPower { get; set; } = string.Empty;

    [JsonProperty("commission")]
    public string Commission { get; set; } = string.Empty;

    [JsonProperty("status")]
    public ValidatorStatus Status { get; set; }

    [JsonProperty("total_delegated")]
    public string TotalDelegated { get; set; } = string.Empty;

    [JsonProperty("delegator_count")]
    public int DelegatorCount { get; set; }
}

public class Delegation
{
    [JsonProperty("validator_address")]
    public string ValidatorAddress { get; set; } = string.Empty;

    [JsonProperty("amount")]
    public string Amount { get; set; } = string.Empty;

    [JsonProperty("rewards")]
    public string Rewards { get; set; } = string.Empty;

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }
}

public class StakingRewards
{
    [JsonProperty("total")]
    public string Total { get; set; } = string.Empty;

    [JsonProperty("available")]
    public string Available { get; set; } = string.Empty;

    [JsonProperty("pending")]
    public string Pending { get; set; } = string.Empty;

    [JsonProperty("last_claim")]
    public DateTime? LastClaim { get; set; }
}

/// <summary>
/// Governance types.
/// </summary>
public enum ProposalStatus
{
    [JsonProperty("voting")]
    Voting,

    [JsonProperty("passed")]
    Passed,

    [JsonProperty("rejected")]
    Rejected,

    [JsonProperty("executed")]
    Executed,

    [JsonProperty("failed")]
    Failed
}

public enum VoteOption
{
    [JsonProperty("yes")]
    Yes,

    [JsonProperty("no")]
    No,

    [JsonProperty("abstain")]
    Abstain,

    [JsonProperty("no_with_veto")]
    NoWithVeto
}

public class VoteTally
{
    [JsonProperty("yes")]
    public string Yes { get; set; } = string.Empty;

    [JsonProperty("no")]
    public string No { get; set; } = string.Empty;

    [JsonProperty("abstain")]
    public string Abstain { get; set; } = string.Empty;

    [JsonProperty("no_with_veto")]
    public string NoWithVeto { get; set; } = string.Empty;
}

public class Proposal
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("proposer")]
    public string Proposer { get; set; } = string.Empty;

    [JsonProperty("status")]
    public ProposalStatus Status { get; set; }

    [JsonProperty("voting_start_time")]
    public DateTime VotingStartTime { get; set; }

    [JsonProperty("voting_end_time")]
    public DateTime VotingEndTime { get; set; }

    [JsonProperty("tally")]
    public VoteTally Tally { get; set; } = new();
}

/// <summary>
/// Network types.
/// </summary>
public class NetworkStatus
{
    [JsonProperty("block_height")]
    public ulong BlockHeight { get; set; }

    [JsonProperty("network_id")]
    public string NetworkId { get; set; } = string.Empty;

    [JsonProperty("consensus_version")]
    public string ConsensusVersion { get; set; } = string.Empty;

    [JsonProperty("peer_count")]
    public ulong PeerCount { get; set; }

    [JsonProperty("syncing")]
    public bool Syncing { get; set; }

    [JsonProperty("latest_block_time")]
    public DateTime LatestBlockTime { get; set; }
}

public class Block
{
    [JsonProperty("height")]
    public ulong Height { get; set; }

    [JsonProperty("hash")]
    public string Hash { get; set; } = string.Empty;

    [JsonProperty("previous_hash")]
    public string PreviousHash { get; set; } = string.Empty;

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonProperty("transaction_count")]
    public ulong TransactionCount { get; set; }

    [JsonProperty("proposer")]
    public string Proposer { get; set; } = string.Empty;

    [JsonProperty("transactions")]
    public List<Transaction>? Transactions { get; set; }
}

/// <summary>
/// Fee estimation.
/// </summary>
public class Fee
{
    [JsonProperty("amount")]
    public string Amount { get; set; } = string.Empty;

    [JsonProperty("gas_limit")]
    public ulong? GasLimit { get; set; }

    [JsonProperty("gas_price")]
    public string? GasPrice { get; set; }
}
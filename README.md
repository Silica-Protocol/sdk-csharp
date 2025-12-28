# Chert SDK for C#

[![NuGet](https://img.shields.io/nuget/v/Chert.Sdk.svg)](https://www.nuget.org/packages/Chert.Sdk/)
[![.NET](https://img.shields.io/badge/.NET-6.0+-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

Official C# SDK for the Chert/Silica blockchain network.

## Features

- **Async/Await Support**: Modern C# async programming with `Task` and `ValueTask`
- **Strong Typing**: Full type safety with C#'s type system and nullable reference types
- **LINQ Support**: Query blockchain data using LINQ
- **Dependency Injection**: Built for dependency injection containers
- **Wallet Management**: Create and manage accounts, send transactions
- **Privacy Features**: Stealth addresses and private transactions
- **Staking**: Delegate tokens to validators and manage stakes
- **Governance**: Participate in network governance and voting
- **Network Operations**: Query blockchain state and network information

## Installation

### NuGet Package Manager
```bash
Install-Package Chert.Sdk
```

### .NET CLI
```bash
dotnet add package Chert.Sdk
```

### Package Reference
```xml
<PackageReference Include="Chert.Sdk" Version="0.1.0" />
```

## Quick Start

```csharp
using ChertSdk;

// Create a client
using var client = new ChertClient();

// Create a new account
var account = await client.Wallet.CreateAccountAsync();
Console.WriteLine($"Created account: {account.Address}");

// Get network status
var status = await client.GetNetworkStatusAsync();
Console.WriteLine($"Block height: {status.BlockHeight}");

// Send a transaction
var txRequest = new TransactionRequest
{
    To = "recipient_address",
    Amount = "100.0",
    Fee = "0.1",
    Memo = "Hello Chert!"
};

var txHash = await client.Wallet.SendTransactionAsync(txRequest, account);
Console.WriteLine($"Transaction sent: {txHash}");
```

## Usage Examples

### Wallet Operations

```csharp
using var client = new ChertClient();

// Create account
var account = await client.Wallet.CreateAccountAsync();
Console.WriteLine($"Address: {account.Address}");
Console.WriteLine($"Public Key: {account.PublicKey}");

// Import account from private key
var imported = await client.Wallet.ImportAccountAsync("your_private_key_hex");
Console.WriteLine($"Imported: {imported.Address}");

// Get balance
var balance = await client.Wallet.GetBalanceAsync(account.Address);
Console.WriteLine($"Balance: {balance.Available} available");

// Send transaction
var txHash = await client.Wallet.SendTransactionAsync(new TransactionRequest
{
    To = imported.Address,
    Amount = "50.0",
    Fee = "0.05",
    Memo = "Test transaction"
}, account);

// Wait for confirmation
var confirmedTx = await client.Wallet.WaitForTransactionAsync(txHash);
if (confirmedTx != null)
{
    Console.WriteLine($"Transaction confirmed: {confirmedTx.Status}");
}
```

### Privacy Features

```csharp
using var client = new ChertClient();

// Generate stealth keys
var stealthKeys = await client.Privacy.GenerateStealthKeysAsync();
Console.WriteLine($"View key: {stealthKeys.ViewKeypair.Public}");

// Create stealth account
var stealthAccount = await client.Privacy.CreateStealthAccountAsync(
    stealthKeys.ViewKeypair.Secret,
    stealthKeys.SpendKeypair.Public,
    stealthKeys);
Console.WriteLine($"Stealth address: {stealthAccount.Address}");

// Send private transaction
var privateTx = await client.Privacy.SendPrivateTransactionAsync(new PrivateTransactionRequest
{
    SenderKeys = stealthKeys,
    RecipientViewKey = "recipient_view_key",
    Amount = "25.0",
    Fee = "0.02",
    PrivacyLevel = PrivacyLevel.Stealth,
    Memo = "Private message",
    Nonce = 1
}, "recipient_view_key", "recipient_spend_key");

Console.WriteLine($"Private transaction: {privateTx}");
```

### Staking Operations

```csharp
using var client = new ChertClient();

// Get validators
var validators = await client.Staking.GetValidatorsAsync();
foreach (var validator in validators.Take(3))
{
    Console.WriteLine($"Validator: {validator.Name} ({validator.Status})");
}

// Delegate tokens
using var account = await client.Wallet.CreateAccountAsync();
var delegationTx = await client.Staking.DelegateAsync(
    account.Address,
    validators[0].Address,
    "1000.0",
    "0.1");
Console.WriteLine($"Delegation transaction: {delegationTx}");

// Get staking rewards
var rewards = await client.Staking.GetStakingRewardsAsync(account.Address);
Console.WriteLine($"Available rewards: {rewards.Available}");
```

### Governance Operations

```csharp
using var client = new ChertClient();

// Get proposals
var proposals = await client.Governance.GetProposalsAsync(limit: 5);
foreach (var proposal in proposals)
{
    Console.WriteLine($"Proposal: {proposal.Title} ({proposal.Status})");
}

// Create a proposal
using var account = await client.Wallet.CreateAccountAsync();
var proposalId = await client.Governance.CreateProposalAsync(
    "Network Upgrade",
    "Proposal to upgrade the network to version 2.0",
    account.Address,
    "1.0");
Console.WriteLine($"Created proposal: {proposalId}");

// Vote on proposal
var voteTx = await client.Governance.VoteAsync(
    proposalId,
    account.Address,
    VoteOption.Yes,
    "0.1");
Console.WriteLine($"Vote cast: {voteTx}");
```

## Configuration

```csharp
// Custom configuration
var config = new ClientConfig
{
    Endpoint = "https://api.chert.com",
    Network = Network.Testnet,
    TimeoutSeconds = 45,  // 45 seconds
    ApiKey = "your_api_key",
    Headers = new Dictionary<string, string>
    {
        ["X-Custom-Header"] = "value"
    }
};

using var client = new ChertClient(config);
```

## Dependency Injection

```csharp
// Register with DI container
services.AddSingleton<ClientConfig>(new ClientConfig
{
    Endpoint = "https://api.chert.com"
});
services.AddTransient<ChertClient>();
```

## Error Handling

```csharp
try
{
    var balance = await client.Wallet.GetBalanceAsync("invalid_address");
}
catch (ValidationException ex)
{
    Console.WriteLine($"Validation error: {ex.Message}");
}
catch (NetworkException ex)
{
    Console.WriteLine($"Network error: {ex.Message}");
}
catch (ApiException ex)
{
    Console.WriteLine($"API error {ex.StatusCode}: {ex.Message}");
}
catch (ChertException ex)
{
    Console.WriteLine($"Chert error {ex.Code}: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

## LINQ Integration

```csharp
// Query transactions with LINQ
var recentBlocks = await client.GetLatestBlockAsync();
var largeTransactions = recentBlocks.Transactions?
    .Where(tx => decimal.Parse(tx.Amount) > 1000)
    .OrderByDescending(tx => tx.Amount)
    .ToList();

foreach (var tx in largeTransactions ?? new List<Transaction>())
{
    Console.WriteLine($"Large transaction: {tx.Amount} from {tx.From}");
}
```

## API Reference

For complete API documentation, see the [API Reference](api_reference.md).

## Testing

```bash
# Run tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Contributing

Contributions are welcome! Please see our [contributing guidelines](CONTRIBUTING.md).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Security

This SDK handles cryptographic operations and private keys. Always:

- Use strong, randomly generated private keys
- Never log or expose private keys
- Use HTTPS endpoints in production
- Keep dependencies updated
- Audit your code for security vulnerabilities

## Requirements

- .NET 6.0 or later
- Newtonsoft.Json for JSON serialization
- System.Net.Http.Json for HTTP client functionality
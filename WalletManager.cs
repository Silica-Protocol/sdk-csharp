using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChertSdk;

/// <summary>
/// Wallet manager for account operations and transaction management.
/// </summary>
public class WalletManager
{
    private readonly ChertClient _client;

    internal WalletManager(ChertClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Create a new account with a randomly generated keypair.
    /// </summary>
    /// <returns>New account with generated keys.</returns>
    public async Task<Account> CreateAccountAsync()
    {
        try
        {
            var (privateKey, publicKey) = GenerateKeyPair();
            var address = GenerateAddress(publicKey);

            return new Account
            {
                Address = address,
                PublicKey = publicKey,
                PrivateKey = privateKey
            };
        }
        catch (Exception ex)
        {
            throw new WalletException($"Failed to create account: {ex.Message}");
        }
    }

    /// <summary>
    /// Import an account from a private key.
    /// </summary>
    /// <param name="privateKey">Hex-encoded private key.</param>
    /// <returns>Account derived from the private key.</returns>
    public async Task<Account> ImportAccountAsync(string privateKey)
    {
        if (string.IsNullOrEmpty(privateKey))
            throw new ValidationException("privateKey", "Private key cannot be empty");

        try
        {
            // Validate hex format
            Convert.FromHexString(privateKey);

            var publicKey = DerivePublicKey(privateKey);
            var address = GenerateAddress(publicKey);

            return new Account
            {
                Address = address,
                PublicKey = publicKey,
                PrivateKey = privateKey
            };
        }
        catch (FormatException)
        {
            throw new ValidationException("privateKey", "Invalid hex format");
        }
        catch (Exception ex)
        {
            throw new WalletException($"Failed to import account: {ex.Message}");
        }
    }

    /// <summary>
    /// Create a watch-only account from a public key.
    /// </summary>
    /// <param name="publicKey">Hex-encoded public key.</param>
    /// <returns>Watch-only account.</returns>
    public async Task<Account> CreateWatchOnlyAccountAsync(string publicKey)
    {
        if (string.IsNullOrEmpty(publicKey))
            throw new ValidationException("publicKey", "Public key cannot be empty");

        try
        {
            // Validate hex format
            Convert.FromHexString(publicKey);

            var address = GenerateAddress(publicKey);

            return new Account
            {
                Address = address,
                PublicKey = publicKey
            };
        }
        catch (FormatException)
        {
            throw new ValidationException("publicKey", "Invalid hex format");
        }
    }

    /// <summary>
    /// Get the balance for an account.
    /// </summary>
    /// <param name="address">Account address.</param>
    /// <returns>Account balance information.</returns>
    public async Task<Balance> GetBalanceAsync(string address)
    {
        if (string.IsNullOrEmpty(address))
            throw new ValidationException("address", "Address cannot be empty");

        return await _client.RpcCallAsync<Balance>("getBalance", new object[] { address });
    }

    /// <summary>
    /// Send a transaction to the network.
    /// </summary>
    /// <param name="request">Transaction request details.</param>
    /// <param name="account">Account to send from.</param>
    /// <returns>Transaction hash.</returns>
    public async Task<string> SendTransactionAsync(TransactionRequest request, Account account)
    {
        if (string.IsNullOrEmpty(account.PrivateKey))
            throw new WalletException("Account does not have a private key");

        ValidateTransactionRequest(request);

        var signature = SignTransaction(request, account.PrivateKey);

        var txData = new
        {
            sender = account.Address,
            recipient = request.To,
            amount = request.Amount,
            fee = request.Fee,
            nonce = request.Nonce ?? 0,
            signature,
            memo = request.Memo
        };

        var result = await _client.RpcCallAsync<object>("sendTransaction", new object[] { txData });

        if (result is Newtonsoft.Json.Linq.JObject obj && obj.ContainsKey("hash"))
        {
            return obj["hash"]?.ToString() ?? throw new TransactionException("Invalid transaction response");
        }

        throw new TransactionException("Invalid transaction response");
    }

    /// <summary>
    /// Estimate the fee for a transaction.
    /// </summary>
    /// <param name="request">Transaction request details.</param>
    /// <returns>Fee estimation data.</returns>
    public async Task<Fee> EstimateFeeAsync(TransactionRequest request)
    {
        ValidateTransactionRequest(request);
        return await _client.RpcCallAsync<Fee>("estimateFee", new object[] { request });
    }

    /// <summary>
    /// Wait for a transaction to be confirmed.
    /// </summary>
    /// <param name="txHash">Transaction hash to wait for.</param>
    /// <param name="timeoutMs">Maximum time to wait in milliseconds.</param>
    /// <param name="intervalMs">Polling interval in milliseconds.</param>
    /// <returns>Transaction data if confirmed, null if timeout.</returns>
    public async Task<Transaction?> WaitForTransactionAsync(
        string txHash,
        int timeoutMs = 60000,
        int intervalMs = 2000)
    {
        if (string.IsNullOrEmpty(txHash))
            throw new ValidationException("txHash", "Transaction hash cannot be empty");

        var startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        while (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime < timeoutMs)
        {
            try
            {
                var tx = await _client.GetTransactionAsync(txHash);

                if (tx.Status == TransactionStatus.Confirmed)
                    return tx;

                if (tx.Status == TransactionStatus.Failed || tx.Status == TransactionStatus.Rejected)
                    throw new TransactionException($"Transaction {tx.Status}", txHash);

                await Task.Delay(intervalMs);
            }
            catch (Exception)
            {
                // Continue polling if transaction not found yet
                await Task.Delay(intervalMs);
            }
        }

        return null;
    }

    private static (string privateKey, string publicKey) GenerateKeyPair()
    {
        // Generate 32 bytes of random data for private key
        var privateKeyBytes = new byte[32];
        RandomNumberGenerator.Fill(privateKeyBytes);
        var privateKey = Convert.ToHexString(privateKeyBytes).ToLower();

        // Derive public key (simplified for demo)
        var publicKeyBytes = SHA256.HashData(privateKeyBytes);
        var publicKey = Convert.ToHexString(publicKeyBytes).ToLower();

        return (privateKey, publicKey);
    }

    private static string DerivePublicKey(string privateKey)
    {
        var privateKeyBytes = Convert.FromHexString(privateKey);
        var publicKeyBytes = SHA256.HashData(privateKeyBytes);
        return Convert.ToHexString(publicKeyBytes).ToLower();
    }

    private static string GenerateAddress(string publicKey)
    {
        var publicKeyBytes = Convert.FromHexString(publicKey);
        var hash = SHA256.HashData(publicKeyBytes);
        var address = "chert_" + Convert.ToHexString(hash)[..40].ToLower();
        return address;
    }

    private static void ValidateTransactionRequest(TransactionRequest request)
    {
        if (string.IsNullOrEmpty(request.To))
            throw new ValidationException("to", "Recipient address cannot be empty");

        if (string.IsNullOrEmpty(request.Amount))
            throw new ValidationException("amount", "Amount cannot be empty");

        if (string.IsNullOrEmpty(request.Fee))
            throw new ValidationException("fee", "Fee cannot be empty");

        // Basic validation - could be enhanced
        if (!decimal.TryParse(request.Amount, out _))
            throw new ValidationException("amount", "Invalid amount format");

        if (!decimal.TryParse(request.Fee, out _))
            throw new ValidationException("fee", "Invalid fee format");
    }

    private static string SignTransaction(TransactionRequest request, string privateKey)
    {
        // Create transaction hash for signing
        var txData = $"{request.To}{request.Amount}{request.Fee}{request.Nonce ?? 0}";
        if (!string.IsNullOrEmpty(request.Memo))
            txData += request.Memo;

        var txBytes = Encoding.UTF8.GetBytes(txData);
        var signatureBytes = SHA256.HashData(txBytes);
        var signature = Convert.ToHexString(signatureBytes).ToLower();

        return signature;
    }
}
using System;

namespace ChertSdk;

/// <summary>
/// Base exception class for Chert SDK errors.
/// </summary>
public class ChertException : Exception
{
    /// <summary>
    /// Error code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Additional error data.
    /// </summary>
    public object? Data { get; }

    /// <summary>
    /// Initialize a new Chert exception.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="code">Error code.</param>
    /// <param name="data">Additional error data.</param>
    public ChertException(string message, string code = "UNKNOWN_ERROR", object? data = null)
        : base(message)
    {
        Code = code;
        Data = data;
    }
}

/// <summary>
/// Network-related errors.
/// </summary>
public class NetworkException : ChertException
{
    public NetworkException(string message, Exception? innerException = null)
        : base(message, "NETWORK_ERROR")
    {
        if (innerException != null)
            HResult = innerException.HResult;
    }
}

/// <summary>
/// API response errors.
/// </summary>
public class ApiException : ChertException
{
    /// <summary>
    /// HTTP status code.
    /// </summary>
    public int? StatusCode { get; }

    public ApiException(string message, int? statusCode = null, object? data = null)
        : base(message, "API_ERROR", data)
    {
        StatusCode = statusCode;
    }

    public ApiException(string message, int code, object? data = null)
        : base(message, code.ToString(), data)
    {
        StatusCode = code;
    }
}

/// <summary>
/// Input validation errors.
/// </summary>
public class ValidationException : ChertException
{
    /// <summary>
    /// Field that failed validation.
    /// </summary>
    public string Field { get; }

    public ValidationException(string field, string message)
        : base($"Validation error in {field}: {message}", "VALIDATION_ERROR", new { field })
    {
        Field = field;
    }
}

/// <summary>
/// Transaction-related errors.
/// </summary>
public class TransactionException : ChertException
{
    /// <summary>
    /// Transaction hash if available.
    /// </summary>
    public string? TransactionHash { get; }

    public TransactionException(string message, string? txHash = null)
        : base(message, "TRANSACTION_ERROR", txHash != null ? new { transactionHash = txHash } : null)
    {
        TransactionHash = txHash;
    }
}

/// <summary>
/// Wallet and account errors.
/// </summary>
public class WalletException : ChertException
{
    public WalletException(string message)
        : base(message, "WALLET_ERROR")
    {
    }
}

/// <summary>
/// Privacy feature errors.
/// </summary>
public class PrivacyException : ChertException
{
    public PrivacyException(string message)
        : base(message, "PRIVACY_ERROR")
    {
    }
}

/// <summary>
/// Staking-related errors.
/// </summary>
public class StakingException : ChertException
{
    public StakingException(string message)
        : base(message, "STAKING_ERROR")
    {
    }
}

/// <summary>
/// Governance-related errors.
/// </summary>
public class GovernanceException : ChertException
{
    public GovernanceException(string message)
        : base(message, "GOVERNANCE_ERROR")
    {
    }
}

/// <summary>
/// Configuration errors.
/// </summary>
public class ConfigurationException : ChertException
{
    public ConfigurationException(string message)
        : base(message, "CONFIG_ERROR")
    {
    }
}

/// <summary>
/// Cryptographic operation errors.
/// </summary>
public class CryptoException : ChertException
{
    public CryptoException(string message)
        : base(message, "CRYPTO_ERROR")
    {
    }
}

/// <summary>
/// Timeout errors.
/// </summary>
public class TimeoutException : ChertException
{
    /// <summary>
    /// Operation that timed out.
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Timeout duration in seconds.
    /// </summary>
    public double TimeoutSeconds { get; }

    public TimeoutException(string operation, double timeoutSeconds)
        : base($"Operation '{operation}' timed out after {timeoutSeconds}s", "TIMEOUT_ERROR",
               new { operation, timeoutSeconds })
    {
        Operation = operation;
        TimeoutSeconds = timeoutSeconds;
    }
}
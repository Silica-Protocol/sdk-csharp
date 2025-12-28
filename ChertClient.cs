using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChertSdk;

/// <summary>
/// Main client for interacting with the Chert blockchain.
/// </summary>
public class ChertClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ClientConfig _config;

    /// <summary>
    /// Wallet operations manager.
    /// </summary>
    public WalletManager Wallet { get; }

    /// <summary>
    /// Privacy features manager.
    /// </summary>
    public PrivacyManager Privacy { get; }

    /// <summary>
    /// Staking operations manager.
    /// </summary>
    public StakingManager Staking { get; }

    /// <summary>
    /// Governance operations manager.
    /// </summary>
    public GovernanceManager Governance { get; }

    /// <summary>
    /// Initialize a new Chert client with default configuration.
    /// </summary>
    public ChertClient() : this(new ClientConfig()) { }

    /// <summary>
    /// Initialize a new Chert client with custom configuration.
    /// </summary>
    /// <param name="config">Client configuration.</param>
    public ChertClient(ClientConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds)
        };

        // Set default headers
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        // Add API key if provided
        if (!string.IsNullOrEmpty(config.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.ApiKey}");
        }

        // Add custom headers
        if (config.Headers != null)
        {
            foreach (var header in config.Headers)
            {
                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        // Initialize managers
        Wallet = new WalletManager(this);
        Privacy = new PrivacyManager(this);
        Staking = new StakingManager(this);
        Governance = new GovernanceManager(this);
    }

    /// <summary>
    /// Get current network status.
    /// </summary>
    /// <returns>Network status information.</returns>
    public async Task<NetworkStatus> GetNetworkStatusAsync()
    {
        return await RpcCallAsync<NetworkStatus>("getNetworkStatus");
    }

    /// <summary>
    /// Get the latest block information.
    /// </summary>
    /// <returns>Latest block data.</returns>
    public async Task<Block> GetLatestBlockAsync()
    {
        return await RpcCallAsync<Block>("getLatestBlock");
    }

    /// <summary>
    /// Get block information by height.
    /// </summary>
    /// <param name="height">Block height to retrieve.</param>
    /// <returns>Block data for the specified height.</returns>
    public async Task<Block> GetBlockAsync(ulong height)
    {
        return await RpcCallAsync<Block>("getBlock", new object[] { height });
    }

    /// <summary>
    /// Get transaction information by hash.
    /// </summary>
    /// <param name="hash">Transaction hash to retrieve.</param>
    /// <returns>Transaction data.</returns>
    public async Task<Transaction> GetTransactionAsync(string hash)
    {
        if (string.IsNullOrEmpty(hash))
            throw new ArgumentException("Transaction hash cannot be empty", nameof(hash));

        return await RpcCallAsync<Transaction>("getTransaction", new object[] { hash });
    }

    /// <summary>
    /// Check if the client is connected to the network.
    /// </summary>
    /// <returns>True if connected and responsive, false otherwise.</returns>
    public async Task<bool> IsConnectedAsync()
    {
        try
        {
            await GetNetworkStatusAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get the client configuration.
    /// </summary>
    /// <returns>Client configuration.</returns>
    public ClientConfig GetConfig() => _config;

    /// <summary>
    /// Make an RPC call to the blockchain.
    /// </summary>
    /// <typeparam name="T">Expected return type.</typeparam>
    /// <param name="method">RPC method name.</param>
    /// <param name="parameters">RPC parameters.</param>
    /// <returns>RPC result.</returns>
    internal async Task<T> RpcCallAsync<T>(string method, object[]? parameters = null)
    {
        var request = new JsonRpcRequest
        {
            Jsonrpc = "2.0",
            Method = method,
            Params = parameters ?? Array.Empty<object>(),
            Id = 1
        };

        var response = await _httpClient.PostAsJsonAsync(_config.Endpoint, request);
        response.EnsureSuccessStatusCode();

        var rpcResponse = await response.Content.ReadFromJsonAsync<JsonRpcResponse<T>>();
        if (rpcResponse == null)
            throw new ChertException("Invalid RPC response");

        if (rpcResponse.Error != null)
            throw new ApiException(rpcResponse.Error.Message, rpcResponse.Error.Code);

        return rpcResponse.Result!;
    }

    /// <summary>
    /// Dispose of the client resources.
    /// </summary>
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

/// <summary>
/// Client configuration.
/// </summary>
public class ClientConfig
{
    /// <summary>
    /// API endpoint URL.
    /// </summary>
    public string Endpoint { get; set; } = "https://api.chert.com";

    /// <summary>
    /// Blockchain network.
    /// </summary>
    public Network Network { get; set; } = Network.Mainnet;

    /// <summary>
    /// Request timeout in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// API key for authenticated requests.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Custom HTTP headers.
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }
}

/// <summary>
/// Blockchain network types.
/// </summary>
public enum Network
{
    /// <summary>
    /// Main production network.
    /// </summary>
    Mainnet,

    /// <summary>
    /// Test network for development.
    /// </summary>
    Testnet,

    /// <summary>
    /// Local development network.
    /// </summary>
    Devnet
}

/// <summary>
/// JSON-RPC request.
/// </summary>
internal class JsonRpcRequest
{
    [JsonProperty("jsonrpc")]
    public string Jsonrpc { get; set; } = "2.0";

    [JsonProperty("method")]
    public string Method { get; set; } = string.Empty;

    [JsonProperty("params")]
    public object[] Params { get; set; } = Array.Empty<object>();

    [JsonProperty("id")]
    public object Id { get; set; } = 1;
}

/// <summary>
/// JSON-RPC response.
/// </summary>
internal class JsonRpcResponse<T>
{
    [JsonProperty("jsonrpc")]
    public string Jsonrpc { get; set; } = string.Empty;

    [JsonProperty("result")]
    public T? Result { get; set; }

    [JsonProperty("error")]
    public JsonRpcError? Error { get; set; }

    [JsonProperty("id")]
    public object Id { get; set; } = 1;
}

/// <summary>
/// JSON-RPC error.
/// </summary>
internal class JsonRpcError
{
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("data")]
    public object? Data { get; set; }
}
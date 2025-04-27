using CSharpAIClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
// Assuming your model classes (ApiRequest, ApiResponse, Content, Part, GenerationConfig etc.) are accessible
// 假设您的模型类（ApiRequest, ApiResponse, Content, Part, GenerationConfig 等）是可访问的

/// <summary>
/// A client class to interact with the Google Gemini API for chat completions, supporting multi-turn conversations.
/// 一个用于与 Google Gemini API 交互以进行聊天补全的客户端类，支持多轮对话。
/// </summary>
public class GeminiApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelName;
    /// <summary>
    /// 允许修改基础url,防止链接过时或大模型开源源.
    /// </summary>
    public string _baseUrl = "https://generativelanguage.googleapis.com/v1beta/models/";
    private List<GeminiContent> _conversationHistory; // Stores the history of the conversation // 存储对话历史记录
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private bool _leaveHttpClientOpen; // Flag to indicate if HttpClient lifetime is managed externally // 标记 HttpClient 生命周期是否由外部管理

    /// <summary>
    /// Gets or sets the configuration for content generation.
    /// 获取或设置内容生成的配置。
    /// </summary>
    public GeminiGenerationConfig GenerationConfig { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeminiApiClient"/> class using a new HttpClient instance.
    /// 使用新的 HttpClient 实例初始化 <see cref="GeminiApiClient"/> 类的新实例。
    /// </summary>
    /// <param name="apiKey">Your Google AI API Key. / 您的 Google AI API 密钥。</param>
    /// <param name="modelName">The name of the model to use (e.g., "gemini-2.5-pro-exp-03-25"). / 要使用的模型名称（例如 "gemini-2.5-pro-exp-03-25"）。</param>
    public GeminiApiClient(string apiKey, string modelName = "gemini-2.5-pro-exp-03-25")
        : this(apiKey, modelName, new HttpClient(), false) // Creates HttpClient, so we own its disposal
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeminiApiClient"/> class using a provided HttpClient instance.
    /// 使用提供的 HttpClient 实例初始化 <see cref="GeminiApiClient"/> 类的新实例。
    /// </summary>
    /// <param name="apiKey">Your Google AI API Key. / 您的 Google AI API 密钥。</param>
    /// <param name="modelName">The name of the model to use (e.g., "gemini-1.5-flash-latest"). / 要使用的模型名称（例如 "gemini-1.5-flash-latest"）。</param>
    /// <param name="httpClient">The HttpClient instance to use for requests. / 用于请求的 HttpClient 实例。</param>
    /// <param name="leaveOpen">True to leave the HttpClient open after the GeminiApiClient is disposed; false to dispose it. Set to true if HttpClient is managed externally (e.g., by IHttpClientFactory). / 如果在 GeminiApiClient 处置后保持 HttpClient 打开，则为 true；否则为 false 以处置它。如果 HttpClient 由外部管理（例如，由 IHttpClientFactory 管理），则设置为 true。</param>
    public GeminiApiClient(string apiKey, string modelName, HttpClient httpClient, bool leaveOpen = true)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentNullException(nameof(apiKey), "API key cannot be null or empty. / API 密钥不能为空。");
        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentNullException(nameof(modelName), "Model name cannot be null or empty. / 模型名称不能为空。");
        if (httpClient == null)
            throw new ArgumentNullException(nameof(httpClient), "HttpClient cannot be null. / HttpClient 不能为空。");


        _apiKey = apiKey;
        _modelName = modelName;
        _httpClient = httpClient;
        _leaveHttpClientOpen = leaveOpen; // Store whether we should dispose the HttpClient
        _conversationHistory = new List<GeminiContent>();

        // Default Generation Config
        // 默认生成配置
        GenerationConfig = new GeminiGenerationConfig
        {
            Temperature = 0.35,
            TopK = 1,
            TopP = 1,
            MaxOutputTokens = 65536
        };

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            // Configure serialization options as needed
            // 根据需要配置序列化选项
            // PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // If needed
            // DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // If needed
        };
    }

    // --- 将此方法添加到 GeminiApiClient 类内部 ---

    /// <summary>
    /// 从 Google Generative Language API 异步获取可用模型的列表。
    /// Asynchronously retrieves the list of available models from the Google Generative Language API.
    /// </summary>
    /// <param name="cancellationToken">用于取消异步操作的取消令牌。/ A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>包含模型信息的列表。如果发生错误或未找到模型，则可能返回空列表或抛出异常。/ A list containing model information. May return an empty list or throw an exception if an error occurs or no models are found.</returns>
    /// <exception cref="HttpRequestException">如果 HTTP 请求失败，则抛出。/ Thrown if the HTTP request fails.</exception>
    /// <exception cref="JsonException">如果 JSON 反序列化失败，则抛出。/ Thrown if JSON deserialization fails.</exception>
    /// <exception cref="ApiException">如果 API 返回成功状态但响应内容无效，则抛出。/ Thrown if the API returns a success status but the response content is invalid.</exception>
    public async Task<List<GeminiModelInfo>> GetModelsListAsync(CancellationToken cancellationToken = default)
    {
        // **新增：在使用 API 密钥之前进行检查**
        // **Added: Check the API key before using it**
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            throw new InvalidOperationException("API 密钥无效或未设置，无法获取模型列表。/ API key is invalid or not set, cannot list models.");
        }

        // 构建请求 URL / Construct the request URL
        // 注意：基础 URL 可能与 generateContent 不同，这里使用 /v1beta/models
        // Note: The base URL might differ from generateContent, using /v1beta/models here
        var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models?key={_apiKey}";

        HttpResponseMessage response = null;
        try
        {
            // 发送 GET 请求 / Send the GET request
            response = await _httpClient.GetAsync(requestUrl, cancellationToken);

            // 在异步调用后检查取消 / Check for cancellation after the async call
            cancellationToken.ThrowIfCancellationRequested();

            // 处理潜在的 HTTP 错误 / Handle potential HTTP errors
            if (!response.IsSuccessStatusCode)
            {
                // string errorBody = await response.Content.ReadAsStringAsync(); // 可选：读取错误体
                response.EnsureSuccessStatusCode(); // 抛出 HttpRequestException
            }

            // 读取并反序列化响应 / Read and deserialize the response
            var jsonResponse = await response.Content.ReadAsStringAsync();

            // 在反序列化前再次检查取消 / Check for cancellation again before deserialization
            cancellationToken.ThrowIfCancellationRequested();

            GeminiModelsListResponse modelsResponse;
            try
            {
                // 使用之前定义的 ModelsListResponse 类进行反序列化
                // Deserialize using the previously defined ModelsListResponse class
                modelsResponse = JsonSerializer.Deserialize<GeminiModelsListResponse>(jsonResponse, _jsonSerializerOptions);
            }
            catch (JsonException ex)
            {
                throw new JsonException($"反序列化模型列表响应时出错: {ex.Message}. 响应体: {jsonResponse}", ex);
                // Error deserializing model list response: {ex.Message}. Response body: {jsonResponse}
            }

            // 返回模型列表，如果响应或列表本身为 null，则返回一个空列表以避免 NullReferenceException
            // Return the list of models, or an empty list if the response or the list itself is null to avoid NullReferenceException
            return modelsResponse?.Models ?? new List<GeminiModelInfo>();

        }
        // 不需要单独捕获 HttpRequestException 或 OperationCanceledException，让它们自然抛出
        // No need to catch HttpRequestException or OperationCanceledException separately, let them propagate
        finally
        {
            // 处置响应消息对象 / Dispose the response message object
            response?.Dispose();
        }
    }

    /// <summary>
    /// Sends a user message to the Gemini API as part of the ongoing conversation and returns the model's response.
    /// 将用户消息作为正在进行的对话的一部分发送到 Gemini API，并返回模型的响应。
    /// </summary>
    /// <param name="userMessage">The message from the user. / 来自用户的消息。</param>
    /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation. / 用于取消异步操作的取消令牌。</param>
    /// <returns>The text content of the model's response. / 模型响应的文本内容。</returns>
    /// <exception cref="ArgumentException">Thrown if userMessage is null or whitespace. / 如果 userMessage 为 null 或空白，则抛出。</exception>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails. / 如果 HTTP 请求失败，则抛出。</exception>
    /// <exception cref="JsonException">Thrown if JSON serialization or deserialization fails. / 如果 JSON 序列化或反序列化失败，则抛出。</exception>
    /// <exception cref="ApiException">Thrown for API specific errors (e.g., bad request, empty response). / 针对 API 特定错误（例如，错误请求、空响应）抛出。</exception>
    public async Task<string> SendChatMessageAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userMessage))
        {
            throw new ArgumentException("User message cannot be null or whitespace.用户消息不能为null或者只有空格", nameof(userMessage));
        }

        // 1. Add user message to history
        // 1. 将用户消息添加到历史记录
        var userContent = new GeminiContent
        {
            Role = "user",
            Parts = new List<GeminiPart> { new GeminiPart { Text = userMessage } }
        };
        _conversationHistory.Add(userContent);

        // 2. Prepare the request payload
        // 2. 准备请求负载
        var requestUrl = $"{_baseUrl}{_modelName}:generateContent?key={_apiKey}";
        var requestBody = new GeminiApiRequest
        {
            Contents = new List<GeminiContent>(_conversationHistory), // Send the entire history // 发送整个历史记录
            GenerationConfig = this.GenerationConfig
            // Add safetySettings if needed // 如果需要，添加 safetySettings
        };

        string jsonRequest;
        try
        {
            jsonRequest = JsonSerializer.Serialize(requestBody, _jsonSerializerOptions);
        }
        catch (JsonException ex)
        {
            // Remove the user message we optimistically added, as the request failed before sending
            // 删除我们乐观添加的用户消息，因为请求在发送前失败了
            _conversationHistory.Remove(userContent);
            // Wrap the JsonException for more context if desired, or just rethrow
            // 如果需要，包装 JsonException 以提供更多上下文，或者直接重新抛出
            throw new JsonException($"Error serializing request body: {ex.Message}", ex);
        }

        HttpResponseMessage response = null;
        try
        {
            using (var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json"))
            {
                // 3. Send the POST request
                // 3. 发送 POST 请求
                response = await _httpClient.PostAsync(requestUrl, httpContent, cancellationToken);
            } // using disposes httpContent here

            // Check for cancellation after the async call
            // 在异步调用后检查取消
            cancellationToken.ThrowIfCancellationRequested();

            // 4. Handle potential HTTP errors
            // 4. 处理潜在的 HTTP 错误
            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                // Remove the user message we optimistically added, as the request failed.
                // 删除我们乐观添加的用户消息，因为请求失败了。
                _conversationHistory.Remove(userContent);
                // Throw a detailed HttpRequestException
                // 抛出详细的 HttpRequestException
                response.EnsureSuccessStatusCode(); // This will throw the appropriate HttpRequestException
            }

            // 5. Read and deserialize the response
            // 5. 读取并反序列化响应
            var jsonResponse = await response.Content.ReadAsStringAsync();

            // Check for cancellation again before potentially long deserialization
            // 在可能冗长的反序列化之前再次检查取消
            cancellationToken.ThrowIfCancellationRequested();

            GeminiApiResponse apiResponse;
            try
            {
                apiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(jsonResponse, _jsonSerializerOptions);
            }
            catch (JsonException ex)
            {
                // Keep the user message in history, as the request was sent, but response processing failed.
                // User might retry or inspect the state.
                // 将用户消息保留在历史记录中，因为请求已发送，但响应处理失败。用户可能会重试或检查状态。
                throw new JsonException($"Error deserializing API response: {ex.Message}. Response body: {jsonResponse}", ex);
            }

            // 6. Process the response and update history
            // 6. 处理响应并更新历史记录
            if (apiResponse?.Candidates != null && apiResponse.Candidates.Count > 0)
            {
                var firstCandidate = apiResponse.Candidates[0];
                // Check finish reason if needed (e.g., handle MAX_TOKENS, SAFETY etc.)
                // 如果需要，检查完成原因（例如，处理 MAX_TOKENS、SAFETY 等）
                // if (firstCandidate.FinishReason != "STOP" && firstCandidate.FinishReason != "MAX_TOKENS") { ... }

                var modelContent = firstCandidate.Content;
                if (modelContent?.Parts != null && modelContent.Parts.Count > 0)
                {
                    // Ensure the role is correctly set (it should be "model" from the API)
                    // 确保角色设置正确（应该来自 API 的 "model"）
                    if (string.IsNullOrEmpty(modelContent.Role))
                    {
                        modelContent.Role = "model"; // Assign default if missing
                    }

                    // Add model response to history for the next turn
                    // 将模型响应添加到历史记录以供下一轮使用
                    _conversationHistory.Add(modelContent);

                    // Return the text from the first part (assuming text modality)
                    // 返回第一部分的文本（假设是文本模态）
                    return modelContent.Parts[0].Text;
                }
                else
                {
                    // Throw a specific exception if the expected content is missing
                    // 如果缺少预期的内容，则抛出特定异常
                    throw new ApiException("API response candidate content or parts were null or empty.", apiResponse);
                }
            }
            else
            {
                // Throw a specific exception if no candidates are received
                // 如果未收到候选者，则抛出特定异常
                throw new ApiException("API response did not contain any candidates.", apiResponse);
            }
        }
        catch (HttpRequestException ex)
        {
            // If we haven't removed the user message yet (e.g., EnsureSuccessStatusCode threw), remove it now.
            // 如果我们还没有删除用户消息（例如，EnsureSuccessStatusCode 抛出异常），现在删除它。
            if (_conversationHistory.Count > 0 && _conversationHistory[_conversationHistory.Count - 1] == userContent)
            {
                _conversationHistory.Remove(userContent);
            }
            // Re-throw the original exception, potentially adding more context if needed
            // 重新抛出原始异常，如果需要可以添加更多上下文
            throw;
        }
        catch (OperationCanceledException) // Catch cancellation specifically
        {
            // Remove the user message if the operation was cancelled during/after send but before success processing
            // 如果操作在发送期间/之后但在成功处理之前被取消，则删除用户消息
            if (_conversationHistory.Count > 0 && _conversationHistory[_conversationHistory.Count - 1] == userContent)
            {
                _conversationHistory.Remove(userContent);
            }
            throw; // Re-throw cancellation exception
        }
        finally
        {
            // Dispose the response message object
            // 处置响应消息对象
            response?.Dispose();
        }
    }

    /// <summary>
    /// **设置**或**替换**当前的对话历史记录。用于导入或加载修改后的历史记录。
    /// **Sets** or **replaces** the current conversation history. Use this for importing or loading a modified history.
    /// </summary>
    /// <param name="history">要设置的对话历史记录。应该是 Content 对象的集合。/ The conversation history to set. Should be a collection of Content objects.</param>
    /// <exception cref="ArgumentNullException">如果提供的 history 为 null，则抛出。/ Thrown if the provided history is null.</exception>
    public void SetConversationHistory(IEnumerable<GeminiContent> history)
    {
        if (history == null)
        {
            throw new ArgumentNullException(nameof(history), "提供的历史记录不能为 null。/ Provided history cannot be null.");
        }
        // 创建一个新的列表副本以替换内部列表，确保传入的 IEnumerable 被完全迭代且内部状态独立
        // Create a new list copy to replace the internal list, ensuring the passed IEnumerable is fully iterated and internal state is independent
        _conversationHistory = new List<GeminiContent>(history);
    }

    /// <summary>
    /// Clears the current conversation history.
    /// 清除当前的对话历史记录。
    /// </summary>
    public void ClearConversationHistory()
    {
        _conversationHistory.Clear();
    }

    /// <summary>
    /// Gets a read-only copy of the current conversation history.
    /// 获取当前对话历史记录的只读副本。
    /// </summary>
    /// <returns>A read-only list of Content objects. / Content 对象的只读列表。</returns>
    public IReadOnlyList<GeminiContent> GetConversationHistory()
    {
        // Return a copy to prevent external modification of the internal list
        // 返回一个副本以防止外部修改内部列表
        return new List<GeminiContent>(_conversationHistory).AsReadOnly();
    }


    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="HttpClient"/> and optionally releases the managed resources.
    /// 释放 <see cref="HttpClient"/> 使用的非托管资源，并可选择释放托管资源。
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="GeminiApiClient"/> and optionally releases the managed resources.
    /// 释放 <see cref="GeminiApiClient"/> 使用的非托管资源，并可选择释放托管资源。
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. / true 释放托管和非托管资源；false 仅释放非托管资源。</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose managed resources only if we own the HttpClient instance
            // 仅当我们拥有 HttpClient 实例时才释放托管资源
            if (!_leaveHttpClientOpen)
            {
                _httpClient?.Dispose();
            }
        }
        // Dispose unmanaged resources here if any
        // 如果有任何非托管资源，在此处释放
    }

    /// <summary>
    /// Custom exception for API specific errors not covered by HttpRequestException or JsonException.
    /// 针对 HttpRequestException 或 JsonException 未涵盖的 API 特定错误的自定义异常。
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// The full API response object, if available, when the error occurred.
        /// 发生错误时完整的 API 响应对象（如果可用）。
        /// </summary>
        public GeminiApiResponse ApiResponse { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class.
        /// 初始化 <see cref="ApiException"/> 类的新实例。
        /// </summary>
        /// <param name="message">The error message. / 错误消息。</param>
        /// <param name="response">The API response object associated with the error. / 与错误关联的 API 响应对象。</param>
        public ApiException(string message, GeminiApiResponse response = null) : base(message)
        {
            ApiResponse = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class.
        /// 初始化 <see cref="ApiException"/> 类的新实例。
        /// </summary>
        /// <param name="message">The error message. / 错误消息。</param>
        /// <param name="innerException">The inner exception. / 内部异常。</param>
        /// <param name="response">The API response object associated with the error. / 与错误关联的 API 响应对象。</param>
        public ApiException(string message, Exception innerException, GeminiApiResponse response = null) : base(message, innerException)
        {
            ApiResponse = response;
        }
    }
}
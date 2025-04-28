using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpAIClient
{
    /// <summary>
    /// 与 Anthropic Claude API 交互的客户端类，支持聊天补全和多轮对话。
    /// A client class to interact with the Anthropic Claude API for chat completions, supporting multi-turn conversations.
    /// </summary>
    public class ClaudeApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _modelName;
        public  string _baseUrl = "https://api.anthropic.com/v1/";
        private string _apiVersion = "2023-06-01";
        private List<ClaudeMessage> _conversationHistory;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private bool _leaveHttpClientOpen;
        private string _systemMessage;

        /// <summary>
        /// 获取或设置温度参数，控制响应的随机性。
        /// Gets or sets the temperature parameter which controls the randomness of the response.
        /// </summary>
        public double Temperature { get; set; } = 0.7;

        /// <summary>
        /// 获取或设置 top_p 参数，控制令牌选择的累积概率阈值。
        /// Gets or sets the top_p parameter which controls the cumulative probability threshold for token selection.
        /// </summary>
        public double TopP { get; set; } = 1.0;

        /// <summary>
        /// 获取或设置 top_k 参数，控制采样时考虑的最大令牌数。
        /// Gets or sets the top_k parameter which controls the maximum number of tokens to consider when sampling.
        /// </summary>
        public int? TopK { get; set; } = null;

        /// <summary>
        /// 获取或设置模型可以生成的最大令牌数。
        /// Gets or sets the maximum number of tokens the model can generate.
        /// </summary>
        public int MaxTokens { get; set; } = 1024;

        /// <summary>
        /// 获取或设置与Anthropic API通信时使用的API版本。
        /// Gets or sets the API version used when communicating with the Anthropic API.
        /// </summary>
        public string ApiVersion
        {
            get { return _apiVersion; }
            set
            {
                _apiVersion = value;
                // 更新HTTP请求头中的版本
                UpdateApiVersionHeader();
            }
        }

        private void UpdateApiVersionHeader()
        {
            // 移除现有版本头（如果存在）
            if (_httpClient.DefaultRequestHeaders.Contains("anthropic-version"))
            {
                _httpClient.DefaultRequestHeaders.Remove("anthropic-version");
            }
            // 添加新版本头
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", _apiVersion);
        }

        /// <summary>
        /// 使用新的 HttpClient 实例初始化 <see cref="ClaudeApiClient"/> 类的新实例。
        /// Initializes a new instance of the <see cref="ClaudeApiClient"/> class using a new HttpClient instance.
        /// </summary>
        /// <param name="apiKey">您的 Anthropic API 密钥。/ Your Anthropic API Key.</param>
        /// <param name="modelName">要使用的模型名称（例如 "claude-3-opus-20240229"）。/ The name of the model to use (e.g., "claude-3-opus-20240229").</param>
        public ClaudeApiClient(string apiKey, string modelName = "claude-3-opus-20240229")
            : this(apiKey, modelName, new HttpClient(), false)
        {
        }

        /// <summary>
        /// 使用提供的 HttpClient 实例初始化 <see cref="ClaudeApiClient"/> 类的新实例。
        /// Initializes a new instance of the <see cref="ClaudeApiClient"/> class using a provided HttpClient instance.
        /// </summary>
        /// <param name="apiKey">您的 Anthropic API 密钥。/ Your Anthropic API Key.</param>
        /// <param name="modelName">要使用的模型名称（例如 "claude-3-opus-20240229"）。/ The name of the model to use (e.g., "claude-3-opus-20240229").</param>
        /// <param name="httpClient">用于请求的 HttpClient 实例。/ The HttpClient instance to use for requests.</param>
        /// <param name="leaveOpen">如果在 AnthropicApiClient 处置后保持 HttpClient 打开，则为 true；否则为 false。/ True to leave the HttpClient open after the AnthropicApiClient is disposed; false to dispose it.</param>
        public ClaudeApiClient(string apiKey, string modelName, HttpClient httpClient, bool leaveOpen = true)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException(nameof(apiKey), "API 密钥不能为空。/ API key cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(modelName))
                throw new ArgumentNullException(nameof(modelName), "模型名称不能为空。/ Model name cannot be null or empty.");
            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient), "HttpClient 不能为空。/ HttpClient cannot be null.");

            _apiKey = apiKey;
            _modelName = modelName;
            _httpClient = httpClient;
            _leaveHttpClientOpen = leaveOpen;
            _conversationHistory = new List<ClaudeMessage>();
            _systemMessage = string.Empty;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            // 设置默认的 HTTP 头部
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", _apiVersion);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        }

        /// <summary>
        /// 异步获取可用模型的列表。
        /// Asynchronously retrieves the list of available models.
        /// </summary>
        /// <param name="cancellationToken">用于取消异步操作的取消令牌。/ A cancellation token to cancel the asynchronous operation.</param>
        /// <returns>可用模型的列表。/ A list of available models.</returns>
        public async Task<List<ClaudeModelInfo>> GetModelsListAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new InvalidOperationException("API 密钥无效或未设置，无法获取模型列表。/ API key is invalid or not set, cannot list models.");
            }

            var requestUrl = $"{_baseUrl}models";

            HttpResponseMessage response = null;
            try
            {
                response = await _httpClient.GetAsync(requestUrl, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (!response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                cancellationToken.ThrowIfCancellationRequested();

                ClaudeModelsListResponse modelsResponse;
                try
                {
                    modelsResponse = JsonSerializer.Deserialize<ClaudeModelsListResponse>(jsonResponse, _jsonSerializerOptions);
                }
                catch (JsonException ex)
                {
                    throw new JsonException($"反序列化模型列表响应时出错: {ex.Message}. 响应体: {jsonResponse}", ex);
                }

                return modelsResponse?.Models ?? new List<ClaudeModelInfo>();
            }
            finally
            {
                response?.Dispose();
            }
        }

        /// <summary>
        /// 设置系统消息，用于指导模型的行为。
        /// Sets the system message to guide the model's behavior.
        /// </summary>
        /// <param name="systemMessage">系统消息内容。/ The system message content.</param>
        public void SetSystemMessage(string systemMessage)
        {
            _systemMessage = systemMessage ?? string.Empty;
        }

        /// <summary>
        /// 将用户消息作为正在进行的对话的一部分发送到 Anthropic API，并返回模型的响应。
        /// Sends a user message to the Anthropic API as part of the ongoing conversation and returns the model's response.
        /// </summary>
        /// <param name="userMessage">来自用户的消息。/ The message from the user.</param>
        /// <param name="cancellationToken">用于取消异步操作的取消令牌。/ A cancellation token to cancel the asynchronous operation.</param>
        /// <returns>模型响应的文本内容。/ The text content of the model's response.</returns>
        public async Task<string> SendChatMessageAsync(string userMessage, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                throw new ArgumentException("用户消息不能为空。/ User message cannot be null or whitespace.", nameof(userMessage));
            }

            // 添加用户消息到历史记录
            var userContentPart = new ClaudeContentPart
            {
                Type = "text",
                Text = userMessage
            };

            var userMessage_obj = new ClaudeMessage
            {
                Role = "user",
                Content = new List<ClaudeContentPart> { userContentPart }
            };

            _conversationHistory.Add(userMessage_obj);

            // 准备请求负载
            var requestUrl = $"{_baseUrl}messages";
            var requestBody = new ClaudeApiRequest
            {
                Model = _modelName,
                Messages = new List<ClaudeMessage>(_conversationHistory),
                System = !string.IsNullOrEmpty(_systemMessage) ? _systemMessage : null,
                MaxTokens = MaxTokens,
                Temperature = Temperature,
                TopP = TopP,
                TopK = TopK,
                Stream = false
            };

            string jsonRequest;
            try
            {
                jsonRequest = JsonSerializer.Serialize(requestBody, _jsonSerializerOptions);
            }
            catch (JsonException ex)
            {
                _conversationHistory.Remove(userMessage_obj);
                throw new JsonException($"序列化请求体时出错: {ex.Message}", ex);
            }

            HttpResponseMessage response = null;
            try
            {
                using (var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json"))
                {
                    response = await _httpClient.PostAsync(requestUrl, httpContent, cancellationToken);
                }

                cancellationToken.ThrowIfCancellationRequested();

                if (!response.IsSuccessStatusCode)
                {
                    string errorBody = await response.Content.ReadAsStringAsync();
                    _conversationHistory.Remove(userMessage_obj);
                    response.EnsureSuccessStatusCode();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                cancellationToken.ThrowIfCancellationRequested();

                ClaudeApiResponse apiResponse;
                try
                {
                    apiResponse = JsonSerializer.Deserialize<ClaudeApiResponse>(jsonResponse, _jsonSerializerOptions);
                }
                catch (JsonException ex)
                {
                    throw new JsonException($"反序列化 API 响应时出错: {ex.Message}. 响应体: {jsonResponse}", ex);
                }

                if (apiResponse?.Content != null && apiResponse.Content.Count > 0)
                {
                    // 创建助手消息并添加到历史记录
                    var assistantMessage = new ClaudeMessage
                    {
                        Role = "assistant",
                        Content = apiResponse.Content
                    };

                    _conversationHistory.Add(assistantMessage);

                    // 假设第一个内容部分是文本
                    return string.Join("\n", apiResponse.Content.ConvertAll(part => part.Text));
                }
                else
                {
                    throw new ApiException("API 响应没有包含预期的内容。", apiResponse);
                }
            }
            catch (HttpRequestException ex)
            {
                if (_conversationHistory.Count > 0 && _conversationHistory[_conversationHistory.Count - 1] == userMessage_obj)
                {
                    _conversationHistory.Remove(userMessage_obj);
                }
                throw;
            }
            catch (OperationCanceledException)
            {
                if (_conversationHistory.Count > 0 && _conversationHistory[_conversationHistory.Count - 1] == userMessage_obj)
                {
                    _conversationHistory.Remove(userMessage_obj);
                }
                throw;
            }
            finally
            {
                response?.Dispose();
            }
        }

        /// <summary>
        /// 设置或替换当前的对话历史记录。
        /// Sets or replaces the current conversation history.
        /// </summary>
        /// <param name="history">要设置的对话历史记录。/ The conversation history to set.</param>
        public void SetConversationHistory(IEnumerable<ClaudeMessage> history)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history), "提供的历史记录不能为 null。/ Provided history cannot be null.");
            }
            _conversationHistory = new List<ClaudeMessage>(history);
        }

        /// <summary>
        /// 清除当前的对话历史记录。
        /// Clears the current conversation history.
        /// </summary>
        public void ClearConversationHistory()
        {
            _conversationHistory.Clear();
        }

        /// <summary>
        /// 获取当前对话历史记录的只读副本。
        /// Gets a read-only copy of the current conversation history.
        /// </summary>
        /// <returns>消息对象的只读列表。/ A read-only list of message objects.</returns>
        public IReadOnlyList<ClaudeMessage> GetConversationHistory()
        {
            return new List<ClaudeMessage>(_conversationHistory).AsReadOnly();
        }

        /// <summary>
        /// 释放资源。
        /// Releases resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源。
        /// Releases resources.
        /// </summary>
        /// <param name="disposing">true 释放托管和非托管资源；false 仅释放非托管资源。/ true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_leaveHttpClientOpen)
            {
                _httpClient?.Dispose();
            }
        }

        /// <summary>
        /// API 特定错误的自定义异常。
        /// Custom exception for API specific errors.
        /// </summary>
        public class ApiException : Exception
        {
            /// <summary>
            /// 发生错误时的 API 响应对象。
            /// The API response object when the error occurred.
            /// </summary>
            public ClaudeApiResponse ApiResponse { get; }

            /// <summary>
            /// 初始化 <see cref="ApiException"/> 类的新实例。
            /// Initializes a new instance of the <see cref="ApiException"/> class.
            /// </summary>
            /// <param name="message">错误消息。/ The error message.</param>
            /// <param name="response">与错误关联的 API 响应对象。/ The API response object associated with the error.</param>
            public ApiException(string message, ClaudeApiResponse response = null) : base(message)
            {
                ApiResponse = response;
            }

            /// <summary>
            /// 初始化 <see cref="ApiException"/> 类的新实例。
            /// Initializes a new instance of the <see cref="ApiException"/> class.
            /// </summary>
            /// <param name="message">错误消息。/ The error message.</param>
            /// <param name="innerException">内部异常。/ The inner exception.</param>
            /// <param name="response">与错误关联的 API 响应对象。/ The API response object associated with the error.</param>
            public ApiException(string message, Exception innerException, ClaudeApiResponse response = null) : base(message, innerException)
            {
                ApiResponse = response;
            }
        }
    }
}
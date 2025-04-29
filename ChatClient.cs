using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LlmClientLibrary
{
    /// <summary>
    /// 该类主要用于私有部署的LM Studio客户端.
    /// This class is mainly used for LM Studio client requests for private deployment
    /// </summary>
    public class ChatClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly List<ChatMessage> _messageHistory;
        private readonly string _model;

        /// <summary>
        /// 随机性参数，介于0到2之间。较高的值会使输出更加随机，较低的值会使其更加集中和确定性。
        /// Randomness parameter, between 0 and 2. Higher values make output more random, lower values make it more focused and deterministic.
        /// </summary>
        public float Temperature { get; set; } = 0.5f;

        /// <summary>
        /// 生成的最大token数量。设置为-1表示无限制。
        /// Maximum number of tokens to generate. Set to -1 for no limit.
        /// </summary>
        public int MaxTokens { get; set; } = -1;

        /// <summary>
        /// 是否使用流式响应作为默认设置
        /// Whether to use streaming response as the default setting
        /// </summary>
        public bool UseStream { get; set; } = false;

        /// <summary>
        /// 创建一个新的聊天客户端实例
        /// Creates a new chat client instance
        /// </summary>
        /// <param name="baseUrl">API基础URL，例如：http://localhost:12340 | API base URL, e.g., http://localhost:12340</param>
        /// <param name="model">默认模型名称 | Default model name</param>
        /// <param name="apiKey">API密钥（如果需要） | API key (if needed)</param>
        /// <param name="temperature">默认温度参数 | Default temperature parameter</param>
        /// <param name="maxTokens">默认最大token数 | Default maximum tokens</param>
        public ChatClient(string baseUrl, string model, string apiKey = null, float temperature = 0.7f, int maxTokens = -1)
        {
            _httpClient = new HttpClient();
            _baseUrl = baseUrl.TrimEnd('/');
            _messageHistory = new List<ChatMessage>();
            _model = model;
            Temperature = temperature;
            MaxTokens = maxTokens;

            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }
        }

        /// <summary>
        /// 设置系统提示消息
        /// Sets the system prompt message
        /// </summary>
        /// <param name="systemPrompt">系统提示内容 | System prompt content</param>
        public void SetSystemPrompt(string systemPrompt)
        {
            // 移除任何现有的系统消息
            // Remove any existing system messages
            _messageHistory.RemoveAll(m => m.Role == "system");

            // 添加新的系统消息作为第一条消息
            // Add new system message as the first message
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                _messageHistory.Insert(0, ChatMessage.CreateSystemMessage(systemPrompt));
            }
        }

        /// <summary>
        /// 发送消息并获取响应，使用客户端默认参数设置
        /// Send a message and get a response using client default parameter settings
        /// </summary>
        /// <param name="message">用户消息内容 | User message content</param>
        /// <returns>助手的回复 | Assistant's reply</returns>
        public async Task<string> SendMessageAsync(string message)
        {
            return await SendMessageAsync(message, Temperature, MaxTokens, UseStream);
        }

        /// <summary>
        /// 发送消息并获取响应，使用自定义参数设置
        /// Send a message and get a response using custom parameter settings
        /// </summary>
        /// <param name="message">用户消息内容 | User message content</param>
        /// <param name="temperature">生成温度参数 | Generation temperature parameter</param>
        /// <param name="maxTokens">生成的最大token数，-1表示无限制 | Maximum tokens to generate, -1 means no limit</param>
        /// <param name="stream">是否使用流式响应 | Whether to use streaming response</param>
        /// <returns>助手的回复 | Assistant's reply</returns>
        public async Task<string> SendMessageAsync(string message, float? temperature = null, int? maxTokens = null, bool? stream = null)
        {
            // 添加用户消息到历史
            // Add user message to history
            _messageHistory.Add(ChatMessage.CreateUserMessage(message));

            // 构建请求，使用传入的参数或默认参数
            // Build request using provided parameters or default parameters
            var request = new ChatCompletionRequest(_model, new List<ChatMessage>(_messageHistory))
            {
                Temperature = temperature ?? Temperature,
                MaxTokens = maxTokens ?? MaxTokens,
                Stream = stream ?? UseStream
            };

            // 发送请求
            // Send request
            var response = await SendRequestAsync(request);

            // 获取助手的回复
            // Get assistant's reply
            var assistantMessage = response.Choices[0].Message;

            // 将助手回复添加到历史
            // Add assistant reply to history
            _messageHistory.Add(assistantMessage);

            return assistantMessage.Content;
        }

        /// <summary>
        /// 发送流式请求并处理响应，使用客户端默认参数设置
        /// Send a streaming request and process the response using client default parameter settings
        /// </summary>
        /// <param name="message">用户消息内容 | User message content</param>
        /// <param name="onChunkReceived">当接收到响应块时调用的回调 | Callback called when a response chunk is received</param>
        /// <returns>完整的助手回复 | Complete assistant reply</returns>
        public async Task<string> SendStreamingMessageAsync(string message, Action<string> onChunkReceived)
        {
            return await SendStreamingMessageAsync(message, onChunkReceived, Temperature, MaxTokens);
        }

        /// <summary>
        /// 发送流式请求并处理响应，使用自定义参数设置
        /// Send a streaming request and process the response using custom parameter settings
        /// </summary>
        /// <param name="message">用户消息内容 | User message content</param>
        /// <param name="onChunkReceived">当接收到响应块时调用的回调 | Callback called when a response chunk is received</param>
        /// <param name="temperature">生成温度参数 | Generation temperature parameter</param>
        /// <param name="maxTokens">生成的最大token数，-1表示无限制 | Maximum tokens to generate, -1 means no limit</param>
        /// <returns>完整的助手回复 | Complete assistant reply</returns>
        public async Task<string> SendStreamingMessageAsync(string message, Action<string> onChunkReceived, float? temperature = null, int? maxTokens = null)
        {
            // 添加用户消息到历史
            // Add user message to history
            _messageHistory.Add(ChatMessage.CreateUserMessage(message));

            // 构建请求
            // Build request
            var request = new ChatCompletionRequest(_model, new List<ChatMessage>(_messageHistory))
            {
                Temperature = temperature ?? Temperature,
                MaxTokens = maxTokens ?? MaxTokens,
                Stream = true  // 流式请求必须为true
            };

            string endpoint = $"{_baseUrl}/v1/chat/completions";

            var jsonContent = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var httpResponse = await _httpClient.PostAsync(endpoint, httpContent);
            httpResponse.EnsureSuccessStatusCode();

            var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            var reader = new System.IO.StreamReader(responseStream);

            StringBuilder fullContent = new StringBuilder();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(line) || !line.StartsWith("data: "))
                    continue;

                var jsonData = line.Substring("data: ".Length);
                if (jsonData == "[DONE]")
                    break;

                try
                {
                    var chunkResponse = JsonSerializer.Deserialize<ChatCompletionResponse>(jsonData);
                    if (chunkResponse.Choices?.Count > 0 && chunkResponse.Choices[0].Message != null)
                    {
                        var chunkContent = chunkResponse.Choices[0].Message.Content;
                        if (!string.IsNullOrEmpty(chunkContent))
                        {
                            fullContent.Append(chunkContent);
                            onChunkReceived(chunkContent);
                        }
                    }
                }
                catch (JsonException)
                {
                    // 忽略无效的JSON块
                    // Ignore invalid JSON chunks
                }
            }

            var finalContent = fullContent.ToString();

            // 将助手回复添加到历史
            // Add assistant reply to history
            _messageHistory.Add(ChatMessage.CreateAssistantMessage(finalContent));

            return finalContent;
        }

        /// <summary>
        /// 清除对话历史
        /// Clear conversation history
        /// </summary>
        /// <param name="keepSystemPrompt">是否保留系统提示 | Whether to keep system prompt</param>
        public void ClearHistory(bool keepSystemPrompt = true)
        {
            if (keepSystemPrompt)
            {
                var systemMessages = _messageHistory.FindAll(m => m.Role == "system");
                _messageHistory.Clear();
                _messageHistory.AddRange(systemMessages);
            }
            else
            {
                _messageHistory.Clear();
            }
        }

        /// <summary>
        /// 获取当前对话历史
        /// Get current conversation history
        /// </summary>
        /// <returns>消息历史列表的副本 | A copy of the message history list</returns>
        public List<ChatMessage> GetMessageHistory()
        {
            return new List<ChatMessage>(_messageHistory);
        }

        /// <summary>
        /// 向API发送请求并获取响应
        /// Send a request to the API and get a response
        /// </summary>
        /// <param name="request">聊天完成请求 | Chat completion request</param>
        /// <returns>聊天完成响应 | Chat completion response</returns>
        private async Task<ChatCompletionResponse> SendRequestAsync(ChatCompletionRequest request)
        {
            string endpoint = $"{_baseUrl}/v1/chat/completions";

            var jsonContent = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var httpResponse = await _httpClient.PostAsync(endpoint, httpContent);
            httpResponse.EnsureSuccessStatusCode();

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ChatCompletionResponse>(responseContent);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LlmClientLibrary
{
    /// <summary>
    /// 表示来自大语言模型API的聊天完成响应
    /// Represents a chat completion response from a large language model API
    /// </summary>
    public class ChatCompletionResponse
    {
        /// <summary>
        /// 响应的唯一标识符
        /// Unique identifier for the response
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// 响应的对象类型
        /// Object type of the response
        /// </summary>
        [JsonPropertyName("object")]
        public string Object { get; set; }

        /// <summary>
        /// 响应创建的时间戳
        /// Timestamp when the response was created
        /// </summary>
        [JsonPropertyName("created")]
        public long Created { get; set; }

        /// <summary>
        /// 使用的模型名称
        /// Model name used
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// 生成的选择列表
        /// List of generated choices
        /// </summary>
        [JsonPropertyName("choices")]
        public List<ChatCompletionChoice> Choices { get; set; }

        /// <summary>
        /// Token使用统计
        /// Token usage statistics
        /// </summary>
        [JsonPropertyName("usage")]
        public ChatCompletionUsage Usage { get; set; }

        /// <summary>
        /// 系统指纹
        /// System fingerprint
        /// </summary>
        [JsonPropertyName("system_fingerprint")]
        public string SystemFingerprint { get; set; }
    }

    /// <summary>
    /// 表示聊天完成的单个选择
    /// Represents a single choice in a chat completion
    /// </summary>
    public class ChatCompletionChoice
    {
        /// <summary>
        /// 选择的索引
        /// Index of the choice
        /// </summary>
        [JsonPropertyName("index")]
        public int Index { get; set; }

        /// <summary>
        /// 概率日志，通常为null
        /// Log probabilities, usually null
        /// </summary>
        [JsonPropertyName("logprobs")]
        public object LogProbs { get; set; }

        /// <summary>
        /// 完成原因（例如：stop, length, content_filter等）
        /// Reason for completion (e.g., stop, length, content_filter, etc.)
        /// </summary>
        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }

        /// <summary>
        /// 生成的消息
        /// Generated message
        /// </summary>
        [JsonPropertyName("message")]
        public ChatMessage Message { get; set; }
    }

    /// <summary>
    /// 表示Token使用统计
    /// Represents token usage statistics
    /// </summary>
    public class ChatCompletionUsage
    {
        /// <summary>
        /// 提示中使用的token数量
        /// Number of tokens used in the prompt
        /// </summary>
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        /// <summary>
        /// 完成中使用的token数量
        /// Number of tokens used in the completion
        /// </summary>
        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        /// <summary>
        /// 总使用的token数量
        /// Total number of tokens used
        /// </summary>
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
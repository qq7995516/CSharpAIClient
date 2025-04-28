using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CSharpAIClient
{
    /// <summary>
    /// 表示来自 Anthropic API 的响应结构。
    /// Represents the response structure from the Anthropic API.
    /// </summary>
    public class AnthropicApiResponse
    {
        /// <summary>
        /// 响应的唯一标识符。
        /// A unique identifier for the response.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// 响应的类型，通常为 "message"。
        /// The type of the response, typically "message".
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// 响应生成时使用的模型。
        /// The model used to generate the response.
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// 模型的回复消息。
        /// The message from the model.
        /// </summary>
        [JsonPropertyName("content")]
        public List<AnthropicContentPart> Content { get; set; }

        /// <summary>
        /// 生成停止的原因（如果适用）。
        /// The reason the generation stopped (if applicable).
        /// </summary>
        [JsonPropertyName("stop_reason")]
        public string StopReason { get; set; }

        /// <summary>
        /// 生成停止的序列（如果适用）。
        /// The sequence that caused the generation to stop (if applicable).
        /// </summary>
        [JsonPropertyName("stop_sequence")]
        public string StopSequence { get; set; }

        /// <summary>
        /// 使用令牌的详细信息。
        /// Details about the usage of tokens.
        /// </summary>
        [JsonPropertyName("usage")]
        public AnthropicTokenUsage Usage { get; set; }

        /// <summary>
        /// 响应创建的时间戳。
        /// The timestamp when the response was created.
        /// </summary>
        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }
    }

    /// <summary>
    /// 表示令牌使用情况的详细信息。
    /// Represents details about token usage.
    /// </summary>
    public class AnthropicTokenUsage
    {
        /// <summary>
        /// 输入令牌数量。
        /// The number of input tokens.
        /// </summary>
        [JsonPropertyName("input_tokens")]
        public int InputTokens { get; set; }

        /// <summary>
        /// 输出令牌数量。
        /// The number of output tokens.
        /// </summary>
        [JsonPropertyName("output_tokens")]
        public int OutputTokens { get; set; }
    }
}
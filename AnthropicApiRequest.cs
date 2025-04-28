using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CSharpAIClient
{
    /// <summary>
    /// 表示发送到 Anthropic API 的请求结构。
    /// Represents the request structure sent to the Anthropic API.
    /// </summary>
    public class AnthropicApiRequest
    {
        /// <summary>
        /// 要使用的模型 ID（例如 "claude-3-opus-20240229"）。
        /// The ID of the model to use (e.g., "claude-3-opus-20240229").
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// 对话消息列表，按时间顺序排列。
        /// The list of conversation messages in chronological order.
        /// </summary>
        [JsonPropertyName("messages")]
        public List<AnthropicMessage> Messages { get; set; }

        /// <summary>
        /// 系统指令，用于指导模型的响应。
        /// System instructions to guide the model's responses.
        /// </summary>
        [JsonPropertyName("system")]
        public string System { get; set; }

        /// <summary>
        /// 模型可生成的最大令牌数。
        /// The maximum number of tokens to generate.
        /// </summary>
        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }

        /// <summary>
        /// 控制随机性的参数，范围为 [0.0, 1.0]。
        /// Controls randomness. Range: [0.0, 1.0].
        /// </summary>
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        /// <summary>
        /// 核采样参数，范围为 [0.0, 1.0]。
        /// Nucleus sampling parameter. Range: [0.0, 1.0].
        /// </summary>
        [JsonPropertyName("top_p")]
        public double TopP { get; set; }

        /// <summary>
        /// 采样时要考虑的最大令牌数。
        /// The maximum number of tokens to consider when sampling.
        /// </summary>
        [JsonPropertyName("top_k")]
        public int? TopK { get; set; }

        /// <summary>
        /// 指示 API 是否应流式传输部分结果。
        /// Indicates if the API should stream partial results.
        /// </summary>
        [JsonPropertyName("stream")]
        public bool Stream { get; set; }
    }
}
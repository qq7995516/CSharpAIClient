using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CSharpAIClient
{
    /// <summary>
    /// 表示从 API 获取的模型列表的响应结构。
    /// Represents the response structure for the list models request from the API.
    /// </summary>
    public class AnthropicModelsListResponse
    {
        /// <summary>
        /// 可用模型的列表。
        /// A list of available models.
        /// </summary>
        [JsonPropertyName("models")]
        public List<AnthropicModelInfo> Models { get; set; }
    }

    /// <summary>
    /// 包含有关单个模型的详细信息。
    /// Contains detailed information about a single model.
    /// </summary>
    public class AnthropicModelInfo
    {
        /// <summary>
        /// 模型的唯一名称（例如 "claude-3-opus-20240229"）。
        /// The unique name of the model (e.g., "claude-3-opus-20240229").
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 模型的描述。
        /// The description of the model.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// 模型的最大上下文窗口大小（以令牌为单位）。
        /// The maximum context window size in tokens.
        /// </summary>
        [JsonPropertyName("context_window")]
        public int ContextWindow { get; set; }

        /// <summary>
        /// 此模型支持的最大输入令牌数。
        /// The maximum number of input tokens supported by this model.
        /// </summary>
        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }

        /// <summary>
        /// 创建模型的日期时间。
        /// The date and time the model was created.
        /// </summary>
        [JsonPropertyName("created")]
        public long Created { get; set; }

        /// <summary>
        /// 该模型是否支持工具使用功能。
        /// Whether the model supports tool use.
        /// </summary>
        [JsonPropertyName("supports_tool_use")]
        public bool SupportsToolUse { get; set; }
    }
}
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CSharpAIClient
{
    // --- 在你的模型类文件或者 GeminiApiClient 文件顶部添加 using ---

    /// <summary>
    /// 表示从 API 获取的模型列表的响应结构。
    /// Represents the response structure for the list models request from the API.
    /// </summary>
    public class GeminiModelsListResponse
    {
        /// <summary>
        /// 可用模型的列表。
        /// A list of available models.
        /// </summary>
        [JsonPropertyName("models")]
        public List<GeminiModelInfo> Models { get; set; }
    }

    /// <summary>
    /// 包含有关单个模型的详细信息。
    /// Contains detailed information about a single model.
    /// </summary>
    public class GeminiModelInfo
    {
        /// <summary>
        /// 模型的唯一名称（例如 "models/gemini-1.5-flash-latest"）。
        /// The unique name of the model (e.g., "models/gemini-1.5-flash-latest").
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 模型的版本。
        /// The version of the model.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// 模型的显示名称。
        /// The display name of the model.
        /// </summary>
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 模型的描述。
        /// The description of the model.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// 模型支持的最大输入令牌数。
        /// The maximum number of input tokens supported by the model.
        /// </summary>
        [JsonPropertyName("inputTokenLimit")]
        public int InputTokenLimit { get; set; }

        /// <summary>
        /// 模型支持的最大输出令牌数。
        /// The maximum number of output tokens supported by the model.
        /// </summary>
        [JsonPropertyName("outputTokenLimit")]
        public int OutputTokenLimit { get; set; }

        /// <summary>
        /// 模型支持的生成方法列表（例如 "generateContent", "countTokens"）。
        /// A list of generation methods supported by the model (e.g., "generateContent", "countTokens").
        /// </summary>
        [JsonPropertyName("supportedGenerationMethods")]
        public List<string> SupportedGenerationMethods { get; set; }

        /// <summary>
        /// 模型的默认或推荐温度值（可能为空）。
        /// The default or recommended temperature value for the model (can be null).
        /// </summary>
        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; } // 使用可空类型，因为 JSON 中可能不存在 / Use nullable as it might not be present in JSON

        /// <summary>
        /// 模型的默认或推荐 TopP 值（可能为空）。
        /// The default or recommended TopP value for the model (can be null).
        /// </summary>
        [JsonPropertyName("topP")]
        public double? TopP { get; set; } // 使用可空类型 / Use nullable

        /// <summary>
        /// 模型的默认或推荐 TopK 值（可能为空）。
        /// The default or recommended TopK value for the model (can be null).
        /// </summary>
        [JsonPropertyName("topK")]
        public int? TopK { get; set; } // 使用可空类型 / Use nullable

        // 注意：根据实际 API 响应，可能还有其他字段。
        // Note: There might be other fields depending on the actual API response.
    }
}

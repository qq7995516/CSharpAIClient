using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CSharpAIClient
{

    /// <summary>
    /// Configuration settings for content generation.
    /// 内容生成的配置设置。
    /// </summary>
    public class GeminiGenerationConfig
    {
        /// <summary>
        /// Controls randomness. Lower values make the output more deterministic. Range: [0.0, 1.0].
        /// 控制随机性。较低的值使输出更具确定性。范围：[0.0, 1.0]。
        /// </summary>
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        /// <summary>
        /// The maximum number of tokens to consider when sampling.
        /// 采样时要考虑的最大令牌数。
        /// </summary>
        [JsonPropertyName("topK")]
        public int TopK { get; set; }

        /// <summary>
        /// The cumulative probability cutoff for token selection.
        /// 用于令牌选择的累积概率阈值。
        /// </summary>
        [JsonPropertyName("topP")]
        public int TopP { get; set; } // Note: JSON shows 1, often this is a float [0.0, 1.0], but matching JSON type int.

        /// <summary>
        /// The maximum number of tokens to generate in the response.
        /// 响应中要生成的最大令牌数。
        /// </summary>
        [JsonPropertyName("maxOutputTokens")]
        public int MaxOutputTokens { get; set; }
    }

    /// <summary>
    /// Represents the overall request structure sent to the API.
    /// 表示发送到 API 的整体请求结构。
    /// </summary>
    public class GeminiApiRequest
    {
        /// <summary>
        /// A list of content blocks representing the conversation history or input.
        /// 代表对话历史或输入的内容块列表。
        /// </summary>
        [JsonPropertyName("contents")]
        public List<GeminiContent> Contents { get; set; }

        /// <summary>
        /// Configuration settings for the generation process.
        /// 生成过程的配置设置。
        /// </summary>
        [JsonPropertyName("generationConfig")]
        public GeminiGenerationConfig GenerationConfig { get; set; }

        // Note: safetySettings could be added here if needed.
        // 注意：如果需要，可以在此处添加 safetySettings。
        // [JsonPropertyName("safetySettings")]
        // public List<SafetySetting> SafetySettings { get; set; }
    }
}

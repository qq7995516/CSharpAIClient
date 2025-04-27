using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpAIClient
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a part of the content, usually containing text.
    /// 表示内容的一部分，通常包含文本。
    /// </summary>
    public class GeminiPart
    {
        /// <summary>
        /// The actual text content of the part.
        /// 部分的实际文本内容。
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    /// <summary>
    /// Represents the content block, containing role and parts. Used in both request and response.
    /// 表示内容块，包含角色和组成部分。在请求和响应中均有使用。
    /// </summary>
    public class GeminiContent
    {
        /// <summary>
        /// The role associated with the content (e.g., "user", "model").
        /// 与内容相关的角色（例如，“user”、“model”）。
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; }

        /// <summary>
        /// A list of parts that make up the content.
        /// 构成内容的各个部分的列表。
        /// </summary>
        [JsonPropertyName("parts")]
        public List<GeminiPart> Parts { get; set; }
    }
}

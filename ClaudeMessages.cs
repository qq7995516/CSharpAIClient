using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CSharpAIClient
{
    /// <summary>
    /// 表示消息的内容，包括文本内容或其他类型。
    /// Represents the content of a message, including text or other types.
    /// </summary>
    public class ClaudeContentPart
    {
        /// <summary>
        /// 内容部分的类型（例如 "text"）。
        /// The type of the content part (e.g., "text").
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// 文本内容（当类型为 "text" 时）。
        /// The text content (when type is "text").
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    /// <summary>
    /// 表示对话中的一条消息。
    /// Represents a message in a conversation.
    /// </summary>
    public class ClaudeMessage
    {
        /// <summary>
        /// 消息的角色（例如 "user" 或 "assistant"）。
        /// The role of the message (e.g., "user" or "assistant").
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; }

        /// <summary>
        /// 消息的内容部分。
        /// The content parts of the message.
        /// </summary>
        [JsonPropertyName("content")]
        public List<ClaudeContentPart> Content { get; set; }
    }

    /// <summary>
    /// 系统消息，用于指导模型的行为。
    /// A system message to guide the model's behavior.
    /// </summary>
    public class ClaudeSystemMessage
    {
        /// <summary>
        /// 系统消息的内容（指导指令）。
        /// The content of the system message (guiding instructions).
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
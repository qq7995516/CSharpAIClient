using System;
using System.Text.Json.Serialization;

namespace LlmClientLibrary
{
    /// <summary>
    /// 表示聊天对话中的单条消息
    /// Represents a single message in a chat conversation
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// 消息角色，可以是"system"、"user"或"assistant"
        /// Message role, can be "system", "user", or "assistant"
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; }

        /// <summary>
        /// 消息内容
        /// Message content
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }

        /// <summary>
        /// 创建一个新的聊天消息
        /// Creates a new chat message
        /// </summary>
        /// <param name="role">消息角色 | Message role</param>
        /// <param name="content">消息内容 | Message content</param>
        public ChatMessage(string role, string content)
        {
            Role = role;
            Content = content;
        }

        /// <summary>
        /// 创建一个系统消息
        /// Creates a system message
        /// </summary>
        /// <param name="content">系统消息内容 | System message content</param>
        /// <returns>系统消息实例 | System message instance</returns>
        public static ChatMessage CreateSystemMessage(string content)
        {
            return new ChatMessage("system", content);
        }

        /// <summary>
        /// 创建一个用户消息
        /// Creates a user message
        /// </summary>
        /// <param name="content">用户消息内容 | User message content</param>
        /// <returns>用户消息实例 | User message instance</returns>
        public static ChatMessage CreateUserMessage(string content)
        {
            return new ChatMessage("user", content);
        }

        /// <summary>
        /// 创建一个助手消息
        /// Creates an assistant message
        /// </summary>
        /// <param name="content">助手消息内容 | Assistant message content</param>
        /// <returns>助手消息实例 | Assistant message instance</returns>
        public static ChatMessage CreateAssistantMessage(string content)
        {
            return new ChatMessage("assistant", content);
        }
    }
}
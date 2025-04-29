using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LlmClientLibrary
{
    /// <summary>
    /// 表示向大语言模型API发送的聊天完成请求
    /// Represents a chat completion request sent to a large language model API
    /// </summary>
    public class ChatCompletionRequest
    {
        /// <summary>
        /// 要使用的模型名称
        /// The model name to use
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// 对话消息列表
        /// List of conversation messages
        /// </summary>
        [JsonPropertyName("messages")]
        public List<ChatMessage> Messages { get; set; }

        /// <summary>
        /// 随机性参数，介于0到2之间。较高的值会使输出更加随机，较低的值会使其更加集中和确定性。
        /// Randomness parameter, between 0 and 2. Higher values make output more random, lower values make it more focused and deterministic.
        /// </summary>
        [JsonPropertyName("temperature")]
        public float Temperature { get; set; } = 0.7f;

        /// <summary>
        /// 生成的最大token数量。设置为-1表示无限制。
        /// Maximum number of tokens to generate. Set to -1 for no limit.
        /// </summary>
        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = -1;

        /// <summary>
        /// 是否启用流式响应
        /// Whether to enable streaming response
        /// </summary>
        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;

        /// <summary>
        /// 创建一个新的聊天完成请求
        /// Creates a new chat completion request
        /// </summary>
        /// <param name="model">模型名称 | Model name</param>
        /// <param name="messages">消息列表 | Message list</param>
        public ChatCompletionRequest(string model, List<ChatMessage> messages)
        {
            Model = model;
            Messages = messages ?? new List<ChatMessage>();
        }

        /// <summary>
        /// 向请求中添加一条新消息
        /// Adds a new message to the request
        /// </summary>
        /// <param name="message">要添加的消息 | Message to add</param>
        public void AddMessage(ChatMessage message)
        {
            Messages.Add(message);
        }
    }
}
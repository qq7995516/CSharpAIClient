/*using System;
using System.Net.Http;

namespace CSharpAIClient
{
    public class AIClient
    {
        /// <summary>
        /// You API Key
        /// 你的API密钥
        /// </summary>
        public string APIKey { get; set; } = string.Empty;
        /// <summary>
        /// The type of AI model to use
        /// 使用的AI模型的类型
        /// </summary>
        public string ModelName { get; set; } = string.Empty;
        public enum AIType
        {
            OpenAI,
            DeepSeek,
            Claude,
            Gemini,
        }

        public AIClient()
        {

        }
        public AIClient(AIType aIType,string modelName,string apikey)
        {
            APIKey = apikey;
            ModelName = modelName;
        }
    }
}
*/
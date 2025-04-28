# CSharpAIClient

## 项目介绍

CSharpAIClient 是一个轻量级 C# 客户端库，用于与多种AI大语言模型API进行交互。目前支持Google Gemini和Anthropic Claude等主流大语言模型，提供简单易用的接口进行聊天对话、文本生成和模型查询等操作。

该库设计用于.NET Standard 2.0环境，可以在多种.NET平台上使用，包括.NET Core、.NET Framework和Xamarin等。

## 主要功能

- **多模型支持**：支持Google Gemini和Anthropic Claude等多种LLM模型
- **聊天对话**：支持多轮对话，轻松管理对话历史记录
- **模型查询**：获取可用模型列表及其详细能力信息
- **参数控制**：支持温度、TopP、TopK等参数的精细调节
- **可扩展架构**：易于添加新的模型API支持
- **双语注释**：提供中英文双语注释，方便多语言开发者使用

## 安装

### 通过NuGet安装


```
dotnet add package CSharpAIClient

```

## 快速开始

### Google Gemini API示例


```csharp
using CSharpAIClient;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // 初始化Gemini客户端
        var geminiClient = new GeminiApiClient("YOUR_GEMINI_API_KEY", "gemini-2.5-pro-exp-03-25");
        
        // 发送聊天消息
        string response = await geminiClient.SendChatMessageAsync("请介绍一下自己");
        Console.WriteLine("Gemini: " + response);
        
        // 查询可用模型
        var models = await geminiClient.GetModelsListAsync();
        Console.WriteLine($"可用模型数量: {models.Count}");
        
        // 清除对话历史
        geminiClient.ClearConversationHistory();
    }
}

```

### Anthropic Claude API示例


```csharp
using CSharpAIClient;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // 初始化Claude客户端
        var claudeClient = new AnthropicApiClient("YOUR_ANTHROPIC_API_KEY", "claude-3-opus-20240229");
        
        // 设置系统消息
        claudeClient.SetSystemMessage("你是一个专业的AI助手，专长于回答技术问题。");
        
        // 发送聊天消息
        string response = await claudeClient.SendChatMessageAsync("请介绍一下量子计算");
        Console.WriteLine("Claude: " + response);
        
        // 调整参数
        claudeClient.Temperature = 0.5;
        claudeClient.MaxTokens = 2000;
    }
}

```

## API文档

### GeminiApiClient

Google Gemini大语言模型的API客户端类。

- **构造函数**
  - `GeminiApiClient(string apiKey, string modelName = "gemini-2.5-pro-exp-03-25")`
  - `GeminiApiClient(string apiKey, string modelName, HttpClient httpClient, bool leaveOpen = true)`

- **主要方法**
  - `SendChatMessageAsync(string userMessage, CancellationToken cancellationToken = default)` - 发送用户消息并获取模型回复
  - `GetModelsListAsync(CancellationToken cancellationToken = default)` - 获取可用模型列表
  - `SetConversationHistory(IEnumerable<GeminiContent> history)` - 设置对话历史
  - `ClearConversationHistory()` - 清除对话历史
  - `GetConversationHistory()` - 获取当前对话历史的只读副本

### AnthropicApiClient

Anthropic Claude大语言模型的API客户端类。

- **构造函数**
  - `AnthropicApiClient(string apiKey, string modelName = "claude-3-opus-20240229")`
  - `AnthropicApiClient(string apiKey, string modelName, HttpClient httpClient, bool leaveOpen = true)`

- **主要方法**
  - `SendChatMessageAsync(string userMessage, CancellationToken cancellationToken = default)` - 发送用户消息并获取模型回复
  - `GetModelsListAsync(CancellationToken cancellationToken = default)` - 获取可用模型列表
  - `SetSystemMessage(string systemMessage)` - 设置系统消息以指导模型行为
  - `SetConversationHistory(IEnumerable<AnthropicMessage> history)` - 设置对话历史
  - `ClearConversationHistory()` - 清除对话历史
  - `GetConversationHistory()` - 获取当前对话历史的只读副本

## 高级用法

### 自定义HTTP客户端


```csharp
// 使用自定义的HttpClient，例如配置代理或超时
var httpClient = new HttpClient(new HttpClientHandler 
{ 
    Proxy = new WebProxy("http://your-proxy:port"),
    UseProxy = true
});
httpClient.Timeout = TimeSpan.FromSeconds(30);

// 将自定义的HttpClient传递给API客户端
var geminiClient = new GeminiApiClient(apiKey, modelName, httpClient, true);

```

### 异常处理


```csharp
try 
{
    var response = await geminiClient.SendChatMessageAsync("你好");
}
catch (HttpRequestException ex) 
{
    Console.WriteLine($"网络请求错误: {ex.Message}");
}
catch (JsonException ex) 
{
    Console.WriteLine($"JSON解析错误: {ex.Message}");
}
catch (GeminiApiClient.ApiException ex) 
{
    Console.WriteLine($"API错误: {ex.Message}");
}

```

## 系统要求

- .NET Standard 2.0 或更高版本
- 有效的Google Gemini API密钥或Anthropic Claude API密钥

## 许可证

此项目遵循MIT许可证

## 贡献指南

欢迎贡献代码、报告问题或提出功能请求。请通过GitHub Issues或Pull Requests参与项目开发。

---

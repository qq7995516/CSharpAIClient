
# CSharpAIClient

## 项目简介

CSharpAIClient是一个C#编写的跨平台AI客户端库，旨在简化与多种大型语言模型API的交互。该库支持多种主流AI服务，包括Claude、Gemini等，提供统一的接口进行AI对话和内容生成。

## 特性

- **多模型支持**：
  - Anthropic Claude（claude-3-opus、claude-3-sonnet等）
  - Google Gemini（gemini-2.5-pro、gemini-1.5-flash等）
  - 扩展设计支持更多模型（OpenAI、DeepSeek等）

- **统一API**：通过一致的接口与不同AI服务交互，简化代码

- **对话管理**：
  - 完整的对话历史管理
  - 支持多轮对话
  - 系统提示设置（System Prompt）

- **高级配置选项**：
  - 温度控制（Temperature）
  - Top-K和Top-P参数设置
  - 最大输出令牌控制

- **.NET标准 2.0兼容**：可用于各种.NET项目，包括：
  - .NET Core/.NET 5+
  - .NET Framework 4.6.1+
  - Xamarin应用
  - Unity游戏开发

## 安装


```shell
# 通过NuGet安装（命令行）
dotnet add package CSharpAIClient

# 或使用Package Manager Console
Install-Package CSharpAIClient

```

## 快速入门

### Claude示例


```csharp
using CSharpAIClient;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // 创建Claude客户端
        var claudeClient = new ClaudeApiClient(
            apiKey: "your-anthropic-api-key",
            modelName: "claude-3-opus-20240229"
        );
        
        // 设置系统消息
        claudeClient.SetSystemMessage("你是一位专业的助手。");
        
        // 发送消息并获取回复
        string response = await claudeClient.SendChatMessageAsync("你能告诉我关于人工智能的最新进展吗？");
        
        Console.WriteLine(response);
    }
}

```

### Gemini示例


```csharp
using CSharpAIClient;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // 创建Gemini客户端
        var geminiClient = new GeminiApiClient(
            apiKey: "your-google-ai-api-key",
            modelName: "gemini-2.5-pro-exp-03-25"
        );
        
        // 配置生成参数
        geminiClient.GenerationConfig.Temperature = 0.7;
        geminiClient.GenerationConfig.MaxOutputTokens = 1024;
        
        // 发送消息并获取回复
        string response = await geminiClient.SendChatMessageAsync("请介绍量子计算的基本概念");
        
        Console.WriteLine(response);
    }
}

```

## 高级用法

### 对话历史管理


```csharp
// 获取对话历史
var history = claudeClient.GetConversationHistory();

// 清除对话历史
claudeClient.ClearConversationHistory();

// 导入对话历史（恢复上下文）
claudeClient.SetConversationHistory(savedHistory);

```

### 使用自定义HttpClient


```csharp
// 使用自定义HttpClient（带代理配置）
var handler = new HttpClientHandler
{
    Proxy = new WebProxy("http://your-proxy-address:port"),
    UseProxy = true
};

var httpClient = new HttpClient(handler);
var claudeClient = new ClaudeApiClient(
    apiKey: "your-api-key",
    modelName: "claude-3-opus-20240229",
    httpClient: httpClient,
    leaveOpen: true // 允许外部管理HttpClient生命周期
);

```

### 查询可用模型


```csharp
// 获取可用的Claude模型
var claudeModels = await claudeClient.GetModelsListAsync();
foreach (var model in claudeModels)
{
    Console.WriteLine($"模型名称: {model.Name}, 描述: {model.Description}");
}

// 获取可用的Gemini模型
var geminiModels = await geminiClient.GetModelsListAsync();
foreach (var model in geminiModels)
{
    Console.WriteLine($"模型名称: {model.Name}, 版本: {model.Version}");
}

```

## 系统要求

- .NET Standard 2.0或更高版本
- 支持的运行时环境:
  - .NET Core 2.0+
  - .NET Framework 4.6.1+
  - .NET 5.0+
  - Xamarin.iOS 10.14+
  - Xamarin.Android 8.0+
  - Unity 2018.1+

## 许可证

MIT许可证 - 详见LICENSE文件

---

如需更多信息或报告问题，请访问我们的GitHub仓库。

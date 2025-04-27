# CSharpAIClient

CSharpAIClient 是一个简洁易用的 C# 客户端库，目前支持与 AI 模型进行交互，主要功能包括：

- 发送消息并接收响应
- 查看聊天记录
- 支持自定义基础 URL（适用于本地模型）
- 支持清空聊天记录

## 快速开始

以下是如何使用 CSharpAIClient 的基本示例：

### 1. 创建客户端
```csharp
var gemini = new GeminiApiClient(_apiKey);
```
### 2. 发送消息并等待响应
```csharp
var ret = await gemini.SendChatMessageAsync("Message");
```
### 3. 查看聊天记录
```csharp
var history = gemini.GetConversationHistory();
```
### 4. 自定义基础 URL（适用于本地模型）
```csharp
gemini._baseUrl = "http://your-custom-url.com";
```
### 5. 清空聊天记录
```csharp
gemini.ClearConversationHistory();
```
### 支持的.NET版本
该库支持所有符合 .NET Standard 2.0 的项目。
### 未来计划
目前支持基本功能，计划未来支持更多主流模型：
>OpenAI：建议直接使用官方 C# SDK。
>下一个计划支持的模型商：Claude。

#### 欢迎对本项目提出建议和改进！

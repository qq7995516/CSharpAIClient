//目前暂时只支持文字.

//其实也支持gemma

//创建客户端

var gemini = new GeminiApiClient(_apiKey);

//发送消息,等待响应

var ret = await gemini.SendChatMessageAsync("Message");

//查看聊天记录

var History = gemini.GetConversationHistory();

//如果是本地模型,支持修改基础url

gemini._baseUrl = "http....";

//清空聊天记录

gemini.ClearConversationHistory();

//支持所有符合.Net Standard 2.0的项目

本来打算支持主流大模型的,但是发现OpenAI在C#有自己的SDK,DeepSeek可以直接用OpenAI的,所以下一个支持的模型商是Claude

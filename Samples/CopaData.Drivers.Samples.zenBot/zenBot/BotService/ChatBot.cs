using System;
using System.Collections.Generic;
using System.Text;
using zenBot.Definitions;
using zenBot.Messages;

namespace zenBot.BotService
{
  public class ChatBot
  {
    public ZenBotMessageQueue ZenBotMessageQueue { get; set; }

    public ChatBot(ILoopBreaker loopBreaker, ChatBotConnection chatBotConnection)
    {
      ZenBotMessageQueue = new ZenBotMessageQueue(loopBreaker, chatBotConnection);
    }

    public ZenBotMessage GetNewAnswer(ZenBotMessage message)
    {
      ZenBotMessageQueue.SendMessage(message);
      return ZenBotMessageQueue.GetLastMessage();
    }

    public ZenBotMessage GetLastAnwer()
    {
      return ZenBotMessageQueue.GetLastMessage();
    }
  }
}

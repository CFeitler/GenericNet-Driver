using System;
using System.Collections.Generic;
using System.Text;
using zenBot.Definitions;
using zenBot.Messages;

namespace zenBot.BotService
{
  public class ChatBot
  {
    public zenBotMessageQueue ZenBotMessageQueue { get; set; }

    public ChatBot(ILoopBreaker loopBreaker)
    {
      ZenBotMessageQueue = new zenBotMessageQueue(loopBreaker);
    }

    public zenBotMessage GetAnswer(zenBotMessage message)
    {
      ZenBotMessageQueue.SendMessage(message);
      return ZenBotMessageQueue.GetLastMessage();
    }
  }
}

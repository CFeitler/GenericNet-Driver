using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zenBot.BotService;
using zenBot.Definitions;

namespace zenBot.Messages
{
  public class ZenBotMessageQueue
  {
    public int QueueSize { get; set; }
    public ILoopBreaker LoopBreaker { get; set; }
    public List<ZenBotMessage> SentMessages { get; set; } =new List<ZenBotMessage>();
    public List<ZenBotMessage> ReceivedAnswers { get; set; } =new List<ZenBotMessage>();
    public ChatBotConnection Connection { get; set; }
    public ZenBotMessageQueue(ILoopBreaker loopBreaker, ChatBotConnection connection, int queueSize = 4)
    {
      QueueSize = queueSize;
      LoopBreaker = loopBreaker;
      Connection = connection;
      for (int i = 0; i < queueSize; i++)
      {
        SentMessages.Add(new ZenBotMessage(){Message = ""});
        ReceivedAnswers.Add(new ZenBotMessage(){Message = ""});
      }
    }

    public ZenBotMessage GetLastMessage()
    {
      return ReceivedAnswers.Last();
    }

    public void SendMessage(ZenBotMessage message)
    {
      if (SentMessages.Contains(message))
      {
        message.Message = LoopBreaker.GetRandomMessage();
      }
      SentMessages.Add(message);
      var answer = Connection.SendMessage(message.Message).Result;
      
      ReceivedAnswers.Add(new ZenBotMessage(){Message = answer});
      if (ReceivedAnswers.Count > QueueSize)
      {
        ReceivedAnswers.RemoveAt(0);
      }

      if (SentMessages.Count > QueueSize)
      {
        SentMessages.RemoveAt(0);
      }
    }

    
  }
}

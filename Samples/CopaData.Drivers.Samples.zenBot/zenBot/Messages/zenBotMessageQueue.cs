using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zenBot.Definitions;

namespace zenBot.Messages
{
  public class zenBotMessageQueue
  {
    private int QueueSize { get; set; }
    public ILoopBreaker LoopBreaker { get; set; }
    public List<zenBotMessage> SentMessages { get; set; }
    public List<zenBotMessage> ReceivedAnswers { get; set; }
    public zenBotMessageQueue(ILoopBreaker loopBreaker, int queueSize = 5)
    {
      QueueSize = queueSize;
      LoopBreaker = loopBreaker;
    }

    public zenBotMessage GetLastMessage()
    {
      return ReceivedAnswers.Last();
    }

    public void SendMessage(zenBotMessage message)
    {
      if (SentMessages.Contains(message))
      {
        message.Message = LoopBreaker.GetRandomMessage();
      }
      //Todo send to bot
      var answer = "result";
      ReceivedAnswers.Add(new zenBotMessage(){Message = answer});
      if (ReceivedAnswers.Count > QueueSize)
      {
        ReceivedAnswers.RemoveAt(0);
      }


    }

    
  }
}

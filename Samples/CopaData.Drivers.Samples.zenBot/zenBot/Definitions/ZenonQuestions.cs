using System;
using System.Collections.Generic;
using System.Text;

namespace zenBot.Definitions
{
  public class ZenonQuestions : ILoopBreaker
  {
    private static string[] _questions =
    {
      "What is the performance of zenon?",
      "Can you archive data in zenon?",
      "Can zenon send a text message to my phone?",
      "What do you like most on zenon?",
      "How many customers are using zenon?",
      "Did you ever create a project with zenon?",
      "What core industries is zenon helping?",
      "Can zenon record events on my PLC?",
      "How is zenon licensed?"
    };

    public string GetRandomMessage()
    {
      return _questions[new Random().Next(_questions.Length)];
    }
  }
}

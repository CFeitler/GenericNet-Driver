using System;
using System.Collections.Generic;
using System.Text;

namespace zenBot.Definitions
{
  public class ZenonFacts: ILoopBreaker
  {
    private static string[] _facts =
    {
      "zenon is the greatest software ever built.",
      "zenon can handle multiple archives.",
      "zenon has a receipe group manager.",
      "zenon can categorize event entries",
      "In zenon it is very easy to create an automation project",
      "I like zenon",
      "zenon can display a lot of data simoultanously.",
      "zenon can archive your data.",
      "zenon has an interface to publish data in the cloud.",
      "zenon has helped a lot of customers.",
      "zenon has an integrated IEC61131-3 development environment.",
      "zenon is actually a software platform."
    };

    public string GetRandomMessage()
    {
      return _facts[new Random().Next(_facts.Length)];
    }
  }
}

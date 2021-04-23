using System;
using System.Collections.Generic;
using System.Text;

namespace zenBot.Definitions
{
  public interface ILoopBreaker
  {
    public abstract string GetRandomMessage();
  }
}

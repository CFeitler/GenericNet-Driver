//#define USE_ZEN_BOTS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CopaData.Drivers.Contracts;
using zenBot.BotService;
using zenBot.Definitions;
using zenBot.Messages;

namespace zenBot
{
  public class zenBot : IDriverExtension
  {
#if USE_ZEN_BOTS
    ChatBotConnection JulieVirtualAssistantConnection = new ChatBotConnection("38171648");
    ChatBotConnection BrainBotConnection = new ChatBotConnection("38172074");
#else
    ChatBotConnection JulieVirtualAssistantConnection = new ChatBotConnection("14867019");
    ChatBotConnection BrainBotConnection = new ChatBotConnection("165");
#endif
    private ILogger _logger;
    private IValueCallback _valueCallback;
    private ChatBot _askBot;
    private ChatBot _tellBot;

    public Task InitializeAsync(ILogger logger, IValueCallback valueCallback, string configFilePath)
    {
      _logger = logger;
      _valueCallback = valueCallback;
      _logger.Info("zenBot driver extension started.");
      _askBot = new ChatBot(new ZenonQuestions(), JulieVirtualAssistantConnection);
      _tellBot = new ChatBot(new ZenonFacts(), BrainBotConnection);
      //Initialize Conversation
      var askBotAnswer =_askBot.GetNewAnswer(new ZenBotMessage() { Message = "Hello from zenon!" });
      _tellBot.GetNewAnswer(askBotAnswer);

      return Task.CompletedTask;
    }

    private Dictionary<string, object> _subscriptions = new Dictionary<string, object>();

    public Task ShutdownAsync()
    {
      _subscriptions.Clear();
      _logger.DeepDebug("All variables have been removed from subscription");
      return Task.CompletedTask;
    }

    public Task<bool> SubscribeAsync(string symbolicAddress)
    {
      if (symbolicAddress == "tellBotLast")
      {
        _subscriptions.Add(symbolicAddress, _isTellBotLast ? 1:0);
      }
      else
      {
        _subscriptions.Add(symbolicAddress, null);
      }
      _logger.DeepDebug($"Variable '{symbolicAddress}' advised.");
      return Task.FromResult(true);
    }

    public Task UnsubscribeAsync(string symbolicAddress)
    {
      _subscriptions.Remove(symbolicAddress);
      _logger.DeepDebug($"Variable '{symbolicAddress}' unadvised.");
      return Task.CompletedTask;
    }


    private bool _isTellBotLast = true;

    public Task ReadAllAsync()
    {
      if (!IsConversationStarted())
      {
        return Task.CompletedTask;
      }

      if(_subscriptions.ContainsKey("tellBotLast")) _valueCallback.SetValue("tellBotLast",_isTellBotLast?1:0);

      //only if there are texts requested
      if (_subscriptions.Keys
        .Select(k => k.StartsWith("zenBot"))
        .Any())
      {      
        
        //do chit-chat
        if (_isTellBotLast)
        {
          _askBot.GetNewAnswer(_tellBot.GetLastAnwer());
        }
        else
        {
          _tellBot.GetNewAnswer(_askBot.GetLastAnwer());
        }

      }

      //map results to subscriptions
      var keys = _subscriptions.Keys.ToList();
      foreach (var key in keys)
      {
        if (key == "zenBot_ask_4" && _subscriptions.ContainsKey("zenBot_ask_4")) _subscriptions["zenBot_ask_4"] = _askBot.ZenBotMessageQueue.ReceivedAnswers[_askBot.ZenBotMessageQueue.QueueSize-1]?.Message ?? "";
        if (key == "zenBot_ask_3" && _subscriptions.ContainsKey("zenBot_ask_3")) _subscriptions["zenBot_ask_3"] = _askBot.ZenBotMessageQueue.ReceivedAnswers[_askBot.ZenBotMessageQueue.QueueSize-2]?.Message ?? "";
        if (key == "zenBot_ask_2" && _subscriptions.ContainsKey("zenBot_ask_2")) _subscriptions["zenBot_ask_2"] = _askBot.ZenBotMessageQueue.ReceivedAnswers[_askBot.ZenBotMessageQueue.QueueSize-3]?.Message ?? ""; 
        if (key == "zenBot_ask_1" && _subscriptions.ContainsKey("zenBot_ask_1")) _subscriptions["zenBot_ask_1"] = _askBot.ZenBotMessageQueue.ReceivedAnswers[_askBot.ZenBotMessageQueue.QueueSize-4]?.Message ?? "";
        
        if (key == "zenBot_tell_4" && _subscriptions.ContainsKey("zenBot_tell_4")) _subscriptions["zenBot_tell_4"] = _tellBot.ZenBotMessageQueue.ReceivedAnswers[_tellBot.ZenBotMessageQueue.QueueSize-1]?.Message ?? "";
        if (key == "zenBot_tell_3" && _subscriptions.ContainsKey("zenBot_tell_3")) _subscriptions["zenBot_tell_3"] = _tellBot.ZenBotMessageQueue.ReceivedAnswers[_tellBot.ZenBotMessageQueue.QueueSize-2]?.Message ?? "";
        if (key == "zenBot_tell_2" && _subscriptions.ContainsKey("zenBot_tell_2")) _subscriptions["zenBot_tell_2"] = _tellBot.ZenBotMessageQueue.ReceivedAnswers[_tellBot.ZenBotMessageQueue.QueueSize-3]?.Message ?? ""; 
        if (key == "zenBot_tell_1" && _subscriptions.ContainsKey("zenBot_tell_1")) _subscriptions["zenBot_tell_1"] = _tellBot.ZenBotMessageQueue.ReceivedAnswers[_tellBot.ZenBotMessageQueue.QueueSize-4]?.Message ?? "";
      }

      foreach (var subscription in _subscriptions)
      {
        if (_isTellBotLast)
        {
          if (_subscriptions.ContainsKey("zenBot_ask_4")) _valueCallback.SetValue("zenBot_ask_4", (string)_subscriptions["zenBot_ask_4"] ?? "", DateTime.Now);
          if (_subscriptions.ContainsKey("zenBot_ask_3")) _valueCallback.SetValue("zenBot_ask_3", (string)_subscriptions["zenBot_ask_3"] ?? "", DateTime.Now);
          if (_subscriptions.ContainsKey("zenBot_ask_2")) _valueCallback.SetValue("zenBot_ask_2", (string)_subscriptions["zenBot_ask_2"] ?? "", DateTime.Now);
          if (_subscriptions.ContainsKey("zenBot_ask_1")) _valueCallback.SetValue("zenBot_ask_1", (string)_subscriptions["zenBot_ask_1"] ?? "", DateTime.Now);
        }
        else
        {
          if (_subscriptions.ContainsKey("zenBot_tell_4")) _valueCallback.SetValue("zenBot_tell_4", (string)_subscriptions["zenBot_tell_4"] ?? "", DateTime.Now);
          if (_subscriptions.ContainsKey("zenBot_tell_3")) _valueCallback.SetValue("zenBot_tell_3", (string)_subscriptions["zenBot_tell_3"] ?? "", DateTime.Now);
          if (_subscriptions.ContainsKey("zenBot_tell_2")) _valueCallback.SetValue("zenBot_tell_2", (string)_subscriptions["zenBot_tell_2"] ?? "", DateTime.Now);
          if (_subscriptions.ContainsKey("zenBot_tell_1")) _valueCallback.SetValue("zenBot_tell_1", (string)_subscriptions["zenBot_tell_1"] ?? "", DateTime.Now);
        }
      }

      _isTellBotLast = !_isTellBotLast;

      return Task.CompletedTask;
    }

    private bool IsConversationStarted()
    {
      const string startVariableSymbolicAddress = "Start";
      const double conversationStartedValue = 1.0;

      if (!_subscriptions.ContainsKey(startVariableSymbolicAddress))
      {
        return false;
      }
      if (null == _subscriptions[startVariableSymbolicAddress])
      {
        return false;
      }
      if (((double)_subscriptions[startVariableSymbolicAddress]) < conversationStartedValue)
      {
        return false;
      }
      return true;
    }

    public Task<bool> WriteStringAsync(string symbolicAddress, string value, DateTime dateTime, StatusBits statusBits)
    {
      if (_subscriptions.ContainsKey(symbolicAddress))
      {
        _subscriptions[symbolicAddress] = value;
      }
      return Task.FromResult(true);
    }

    public Task<bool> WriteNumericAsync(string symbolicAddress, double value, DateTime dateTime, StatusBits statusBits)
    {
      if (_subscriptions.ContainsKey(symbolicAddress))
      {
        _subscriptions[symbolicAddress] = value;
      }
      return Task.FromResult(true);
    }
  }
}

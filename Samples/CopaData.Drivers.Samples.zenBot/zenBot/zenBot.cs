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
      var askBotAnswer = _askBot.GetNewAnswer(new ZenBotMessage() { Message = "Hello from zenon!" });
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
        _subscriptions.Add(symbolicAddress, _isTellBotLast ? 1 : 0);
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
    private bool IsTellBotLast => _isTellBotLast;
    private bool IsAskBotLast => !_isTellBotLast;

    public Task ReadAllAsync()
    {
      if (!IsConversationStarted())
      {
        return Task.CompletedTask;
      }



      //only if there are texts requested
      if (_subscriptions.Keys
        .Select(k => k.StartsWith("zenBot"))
        .Any())
      {

        //do chit-chat
        if (_isTellBotLast)
        {
          var messageToSend = _tellBot.GetLastAnwer();
          if (new Random().Next(100) > 80) 
            messageToSend = new ZenBotMessage() {Message = new ZenonFacts().GetRandomMessage()};
          _askBot.GetNewAnswer(messageToSend);
        }
        else
        {
          var messageToSend = _askBot.GetLastAnwer();
          if (new Random().Next(100) > 80) messageToSend = new ZenBotMessage() {Message = new ZenonQuestions().GetRandomMessage()};
          _tellBot.GetNewAnswer(_askBot.GetLastAnwer());
        }

      }

      //map results to subscriptions
      var keys = _subscriptions.Keys.ToList();
      foreach (var key in keys)
      {

        if (key == "zenBot_ask_4" && _subscriptions.ContainsKey(key)) _subscriptions[key] = _tellBot.ZenBotMessageQueue.SentMessages[^1]?.Message ?? "";
        if (key == "zenBot_ask_3" && _subscriptions.ContainsKey(key)) _subscriptions[key] = _tellBot.ZenBotMessageQueue.SentMessages[^2]?.Message ?? "";
        if (key == "zenBot_ask_2" && _subscriptions.ContainsKey(key)) _subscriptions[key] = _tellBot.ZenBotMessageQueue.SentMessages[^3]?.Message ?? "";
        if (key == "zenBot_ask_1" && _subscriptions.ContainsKey(key)) _subscriptions[key] = _tellBot.ZenBotMessageQueue.SentMessages[^4]?.Message ?? "";

        if (key == "zenBot_tell_4" && _subscriptions.ContainsKey(key)) _subscriptions[key] = _askBot.ZenBotMessageQueue.SentMessages[^1]?.Message ?? "";
        if (key == "zenBot_tell_3" && _subscriptions.ContainsKey(key)) _subscriptions[key] = _askBot.ZenBotMessageQueue.SentMessages[^2]?.Message ?? "";
        if (key == "zenBot_tell_2" && _subscriptions.ContainsKey(key)) _subscriptions[key] = _askBot.ZenBotMessageQueue.SentMessages[^3]?.Message ?? "";
        if (key == "zenBot_tell_1" && _subscriptions.ContainsKey(key)) _subscriptions[key] = _askBot.ZenBotMessageQueue.SentMessages[^4]?.Message ?? "";

        if (key == "tellBotLast" && _subscriptions.ContainsKey(key)) _subscriptions[key] = _isTellBotLast ? 1 : 0;
        if (key == "askBotLast" && _subscriptions.ContainsKey(key)) _subscriptions[key] = _isTellBotLast ? 0 : 1;
      }

      //set values to zenon
      foreach (var key in keys)
      {
        if (key.StartsWith("zenBot"))
        {
          _valueCallback.SetValue(key, (string)_subscriptions[key]);
        }
        else
        {
          if (key.Equals("tellBotLast")) _valueCallback.SetValue(key, _isTellBotLast ? 1 : 0);
          if (key.Equals("askBotLast")) _valueCallback.SetValue(key, _isTellBotLast ? 0 : 1);
          if (key.Equals("Start")) _valueCallback.SetValue(key, (double)_subscriptions[key]);
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

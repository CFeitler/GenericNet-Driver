﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CopaData.Drivers.Contracts;
using LibreHardwareMonitor.Hardware;

namespace SystemMonitor
{
  public class SystemMonitor : IDriverExtension
  {
    private ILogger _logger;
    private IValueCallback _valueCallback;

    public Task InitializeAsync(ILogger logger, IValueCallback valueCallback, string configFilePath)
    {
      _logger = logger;
      _valueCallback = valueCallback;
      return Task.CompletedTask;
    }

    public Dictionary<string, object> subscriptions { get; set; } = new Dictionary<string, object>();

    public Task ShutdownAsync()
    {
      subscriptions.Clear();
      return Task.CompletedTask;
    }


    public Task<bool> SubscribeAsync(string symbolicAddress)
    {
      if (!subscriptions.ContainsKey(symbolicAddress))
      {
        subscriptions.Add(symbolicAddress, 10.0);
      }

      return Task.FromResult(true);
    }

    public Task UnsubscribeAsync(string symbolicAddress)
    {
      if (subscriptions.ContainsKey(symbolicAddress))
      {
        subscriptions.Remove(symbolicAddress);
      }
      return Task.CompletedTask;
    }

    public Task ReadAllAsync()
    {
      _logger.Info("Read system information...");
      SystemInfo systemInfo = ReadSystemInfoAsync();
      var regexCpuNumber = new Regex(@"(?<VariableName>[A-Za-z ]+)(?<CpuNumber>\d+)",RegexOptions.IgnoreCase); //this regex is for identifying the variables. it should match the symbolic address of the variable

      var keys = new List<string>(subscriptions.Keys);

      foreach (var key in keys)
      {
        if (key.ToLower().StartsWith("cpu core")) //Symbolic address of temperature variable name must start with 'CPU Core' followed by a number to get the temperature for that core. Numbers start with 1. 
        {
          var currentCpuNumber = regexCpuNumber.Match(key)?.Groups["CpuNumber"].Value;
          if (String.IsNullOrEmpty(currentCpuNumber)) {
            _logger.Error($"CPU number of variable with symbolic address '{key}' not found. E.g. use 'CPU Core 1' as symbolic address for temperature value.");
            continue; 
          }
          var coreInfo = systemInfo.CoreInfos.FirstOrDefault(p => p.Name.Equals("CPU Core #" + currentCpuNumber)); //This is the name we get from systemInfo. it is called: 'CPU Core #1'
          if (coreInfo != null)
          {
            subscriptions[key] = coreInfo.Temp;
          }
        }
      }

      foreach (var subscription in subscriptions)
      {
        if (subscription.Value != null)
        {
          _valueCallback.SetValue(subscription.Key, (double)subscription.Value, DateTime.Now);
        }
      }

      foreach (SystemInfo.CoreInfo cInfo in systemInfo.CoreInfos)
      {
        _logger.Info($"Name: {cInfo.Name} - {cInfo.Load} % - {cInfo.Temp} °C");
      }
      return Task.CompletedTask;
    }

    public Task<bool> WriteStringAsync(string symbolicAddress, string value, DateTime dateTime, StatusBits statusBits)
    {
      throw new NotImplementedException();
    }

    public Task<bool> WriteNumericAsync(string symbolicAddress, double value, DateTime dateTime, StatusBits statusBits)
    {
      throw new NotImplementedException();
    }

    public static SystemInfo ReadSystemInfoAsync()
    {

      SystemInfo systemInfo = new SystemInfo();

      SystemVisitor updateVisitor = new SystemVisitor();
      Computer computer = new Computer()
      {
        IsCpuEnabled = true,
        IsMotherboardEnabled = true,
      };

      try
      {
        computer.Open();

        computer.Accept(updateVisitor);

        foreach (IHardware hw in computer.Hardware
                  .Where(hw => hw.HardwareType == HardwareType.Cpu))
        {
          foreach (ISensor sensor in hw.Sensors)
          {
            switch (sensor.SensorType)
            {
              case SensorType.Load:
                systemInfo.AddOrUpdateCoreLoad(
                      sensor.Name, sensor.Value.GetValueOrDefault(0));

                break;
              case SensorType.Temperature:
                systemInfo.AddOrUpdateCoreTemp(
                      sensor.Name, sensor.Value.GetValueOrDefault(0));

                break;
            }
          }
        }
      }
      finally
      {
        computer.Close();
      }

      return systemInfo;

    }
  }

  public class SystemInfo
  {
    public class CoreInfo
    {
      public string Name { get; set; }
      public double Load { get; set; }
      public double Temp { get; set; }
    }

    public List<CoreInfo> CoreInfos = new List<CoreInfo>();

    private CoreInfo GetCoreInfo(string name)
    {
      CoreInfo coreInfo = CoreInfos.SingleOrDefault(c => c.Name == name);
      if (coreInfo is null)
      {
        coreInfo = new CoreInfo { Name = name };
        CoreInfos.Add(coreInfo);
      }

      return coreInfo;
    }

    public void AddOrUpdateCoreTemp(string name, double temp)
    {
      CoreInfo coreInfo = GetCoreInfo(name);
      coreInfo.Temp = temp;
    }

    public void AddOrUpdateCoreLoad(string name, double load)
    {
      CoreInfo coreInfo = GetCoreInfo(name);
      coreInfo.Load = load;
    }
  }

  public class SystemVisitor : IVisitor
  {
    public void VisitComputer(IComputer computer) { computer.Traverse(this); }

    public void VisitHardware(IHardware hardware)
    {
      hardware.Update();
      foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
    }

    public void VisitSensor(ISensor sensor) { }
    public void VisitParameter(IParameter parameter) { }
  }
}


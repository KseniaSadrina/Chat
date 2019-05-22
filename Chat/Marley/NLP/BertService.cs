using Chat.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Marley
{
  public class BertService : INlpService
  {

    private string _scriptPath;
    private string _questionContextPath;
    private readonly ILogger<BertService> _logger;

    public BertService(IOptions<NlpConfiguration> configuration, ILogger<BertService> logger)
    {
      _scriptPath = configuration.Value.NlpPyScriptPath;
      _logger = logger;
    }

    public string AskQuestionAboutContext(QAModelInput input)
    {
      SaveInputToJson(input);
      return AskQuestionAboutContext();
    }

    private void SaveInputToJson(QAModelInput input)
    {
      using (StreamWriter file = File.CreateText(_questionContextPath))
      {
        JsonSerializer serializer = new JsonSerializer();
        serializer.Serialize(file, input);
      }
    }

    private string AskQuestionAboutContext()
    {
      try
      {
        var startInfo = new ProcessStartInfo(_scriptPath)
        {
          RedirectStandardOutput = true,
          UseShellExecute = false,
          CreateNoWindow = true,
          Arguments = "" // string.Format("{0} {1}", cmd, args) TODO: Add arguments
        };
        using (Process process = Process.Start(startInfo))
        {
          using (StreamReader reader = process.StandardOutput)
          {
            string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
            string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
            return result;
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError("Something went wrong during the execution of the nlp python script: ", ex);
        return String.Empty;
      }
    }
  }
}

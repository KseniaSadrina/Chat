using Chat.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Models.JsonEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Chat.Services
{
  public class MnemonicReaderService : INlpService
  {
    private readonly ILogger<MnemonicReaderService> _logger;
    private readonly NlpConfiguration _config;
    private readonly string _pythonPath = "python.exe";

    public MnemonicReaderService(IOptions<NlpConfiguration> configuration, ILogger<MnemonicReaderService> logger)
    {
      _config = configuration.Value;
      _logger = logger;
    }

    public string AskQuestionAboutContext(QAModelInput input)
    {
      SaveInputToJson(input);
      var execResult = AskQuestionAboutContext();
      return execResult ? ProcessResult(input.Id) : string.Empty;
    }

    private void SaveInputToJson(QAModelInput input)
    {
      var finalInput = new List<QAModelInput>() { input };
      using (StreamWriter file = File.CreateText(_config.QuestionPath))
      {
        JsonSerializer serializer = new JsonSerializer();
        serializer.Serialize(file, finalInput);
      }
    }

    private bool AskQuestionAboutContext()
    {
      try
      {
        var startInfo = new ProcessStartInfo()
        {
          FileName = _pythonPath,
          RedirectStandardOutput = true,
          RedirectStandardError = true,
          UseShellExecute = false,
          CreateNoWindow = true,
          Arguments = $"{_config.NlpPyScriptPath} --model {_config.ModelPath} --data-file {_config.QuestionPath} --output-file {_config.OutputPath}"
        };
        using (Process process = Process.Start(startInfo))
        {
          using (StreamReader reader = process.StandardOutput)
          {
            string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
            string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError("Something went wrong during the execution of the nlp python script: ", ex);
        return false;
      }
    }

    private string ProcessResult(int messageId)
    {
      var result = string.Empty;
      try
      {
        using (StreamReader reader = new StreamReader(_config.OutputPath))
        {
          _logger.LogInformation("Parsing output to the users question after nlp proccessing.");
          string json = reader.ReadToEnd();
          var output = JsonConvert.DeserializeObject<Dictionary<int, IList<QAModelOutput>>>(json);
          _logger.LogInformation($"Parsed output:{output}");
          if (output.TryGetValue(messageId, out var predictions))
            result = predictions.ToList().OrderByDescending(item => item.Score)?.FirstOrDefault()?.Answer;
        }
      }
      catch (Exception ex)
      {
        _logger.LogError("Failed parsing output of the nlp script: ", ex);
      }

      return result;

    }
  }
}

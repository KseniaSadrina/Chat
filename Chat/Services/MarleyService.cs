using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.DAL;
using Chat.Helpers;
using Chat.Services;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.Enums;

namespace Chat.Services
{
  public class MarleyService : IBotService
  {
    #region Private Fields

    private readonly ISessionsService _sessions;
    private readonly ITrainingDALService _trainings;
    private readonly INlpService _nlpService;
    private readonly IGuideService _guideService;
    private readonly IGoalsService _goalService;
    private readonly UserManager<User> _userManager;

    private readonly IList<string> _greetingWords = new List<string>()
    {
      "hi ", "hello ", "hey ", "how do you do", "goodmorning", "goodafternoon", "goodevening"
    };
    private readonly IList<string> _networkWords = new List<string>()
    {
      "#network", "#net"
    };
    private readonly IList<string> _abstractWords = new List<string>()
    {
      "#abstract", "#general"
    };
    private readonly Dictionary<BotMessageHandler, string> _botDefaultResponses = new Dictionary<BotMessageHandler, string>
    {
      { BotMessageHandler.Greeting, "Hi there!" },
      { BotMessageHandler.Question, "Sorry, I can't help you with that." },
    };

    #endregion

    #region Init

    public MarleyService(ISessionsService sessionsService,
      UserManager<User> userManager,
      INlpService nlpService,
      ITrainingDALService trainingService,
      IGuideService guideService,
      IGoalsService goalService)
    {
      _sessions = sessionsService;
      _userManager = userManager;
      _nlpService = nlpService;
      _trainings = trainingService;
      _guideService = guideService;
      _goalService = goalService;

      Init();
    }

    private async void Init()
    {
      CurrentBot = await _userManager.FindByNameAsync(Consts.MARLEYNAME);
    }

    #endregion

    #region Properties

    public User CurrentBot { get; set; }

    #endregion

    #region Public Methods

    public async Task<Message> HandleMessage(Message message, BotMessageHandler handlerType)
    {
      if (message == null) return null;
      switch (handlerType)
      {
        case BotMessageHandler.Greeting:
          return SayHello(message.ChatSessionId, message.SessionName);
        case BotMessageHandler.Question:
        case BotMessageHandler.Abstract:
          return await AnswerQuestion(message, handlerType);
        case BotMessageHandler.None:
        default:
          return null;
      }

    }

    public (bool, BotMessageHandler) WillHandleMessage(Message message)
    {
      var messageText = message.Text.ToLower().Replace(" ", ""); // sort of tokenize this message
      var lowerBotName = CurrentBot.UserName.ToLower();

      if (CurrentBot != null && messageText.Contains(String.Join("", "@", lowerBotName)))
      {
        if (_abstractWords.Any(word => messageText.Contains(word)))
          return (true, BotMessageHandler.Abstract);
        if (_greetingWords.Any(word => messageText.Contains(word)))
          return (true, BotMessageHandler.Greeting);
        return (true, BotMessageHandler.Question);
      }

      return (false, BotMessageHandler.None);
    }

    #endregion

    #region Handle Message (private)

    private Message SayHello(int sessionId, string sessionName) => GenerateMessage(sessionId, sessionName, _botDefaultResponses[BotMessageHandler.Greeting]);

    private async Task<Message> AnswerQuestion(Message message, BotMessageHandler handler)
    {
      if (message == null) return null ;
      var modelInput = await GenerateModelInput(message, handler);

      var res = string.Empty;
      if (modelInput != null && modelInput.Contexts != null && modelInput.Contexts.Count > 0)
        res = _nlpService.AskQuestionAboutContext(modelInput);

      if (string.IsNullOrEmpty(res))
        res = _botDefaultResponses[BotMessageHandler.Question];

      return GenerateMessage(message.ChatSessionId, message.SessionName, res);
    }

    private Message GenerateMessage(int sessionId, string sessionName, string message)
    {
      return new Message
      {
        Sender = CurrentBot.UserName,
        Text = message,
        ChatSessionId = sessionId,
        SessionName = sessionName,
        TimeStamp = DateTime.Now,
        SenderType = UserType.Bot
      };

    }

    private async Task<QAModelInput> GenerateModelInput(Message message, BotMessageHandler handler)
    {
      var currentGoal = (await _goalService.GetCurrentGoalBySessionid(message.ChatSessionId))?.Goal;
      if (currentGoal == null) return null;
      var currentGoalGuide = _guideService.GetGoalGuide(currentGoal);
      var contexts = new List<string>();
      if (handler == BotMessageHandler.Abstract) {
        var currentScenarioGuide = _guideService.GetScenarioGuide(currentGoal.ScenarioId);
        contexts.Add(currentScenarioGuide.Abstract);
      } else
        contexts.Add(currentGoalGuide.Description);

      var modelInput = new QAModelInput() { Id = message.Id , Contexts = contexts, Question = message.Text };
      return modelInput;
    }

    #endregion
  }
}

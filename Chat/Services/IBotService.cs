using Models;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Services
{
  public interface IBotService
  {
    /// <summary>
    /// Returns whether the bot will handle this message
    /// If it will, the chat is supposed to display the bot as typing
    /// else, just ignore
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    (bool, BotMessageHandler) WillHandleMessage(Message message);

    /// <summary>
    /// Handles the message and returns response message
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task<Message> HandleMessage(Message message, BotMessageHandler handlerType);

    /// <summary>
    /// Returns the informatioin regarding the active bot (happens when the message hub is initialized
    /// </summary>
    /// <returns></returns>
    User CurrentBot { get; set; }
    

    
  }
}

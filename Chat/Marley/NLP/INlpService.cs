using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Marley
{
  public interface INlpService
  {
    /// <summary>
    /// Asks question about the given context and returns the answer.
    /// </summary>
    /// <param name="context"> </param>
    /// <param name="question"></param>
    /// <returns></returns>
    string AskQuestionAboutContext(QAModelInput modelInput);

  }
}

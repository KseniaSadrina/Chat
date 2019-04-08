using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Helpers
{
  public static class DBStatusToWebAPI
  {
    public static ActionResult<T> ConvertToWebAPI<T>(this DbExecutionStatus result, ActionResult<T> success)
    {
      switch (result)
      {
        case DbExecutionStatus.Failed:
          return new StatusCodeResult(500);
        case DbExecutionStatus.NotFound:
          return new NotFoundResult();
        case DbExecutionStatus.Succeeded:
          return success;
        default:
          return new StatusCodeResult(500);
      }

    }

    public static IActionResult ConvertToWebAPI(this DbExecutionStatus result, IActionResult success)
    {
      switch (result)
      {
        case DbExecutionStatus.Failed:
          return new StatusCodeResult(500);
        case DbExecutionStatus.NotFound:
          return new NotFoundResult();
        case DbExecutionStatus.Succeeded:
          return success;
        default:
          return new StatusCodeResult(500);
      }

    }
  }
}

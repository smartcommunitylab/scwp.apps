using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.Helpers
{
  public class ExceptionLoggerHelper
  {
    FileStorageHelper fsh;
    private readonly string UnhandledExceptionFilePath = "loggedUnhandledExceptions.log";
    private readonly string HandledExceptionFilePath = "loggedHandledExceptions.log";
    private readonly TimeSpan requiredAge = new TimeSpan(0, 0, 0, 30);


    public ExceptionLoggerHelper()
    {
      fsh = new FileStorageHelper();
    }

    public void LogNewException(Exception e, ExceptionType exType)
    {
      if(exType == ExceptionType.Unhandled)
        fsh.WriteFile(UnhandledExceptionFilePath, JsonConvert.SerializeObject(e, Formatting.Indented), true);
      else
        fsh.AppendFile(HandledExceptionFilePath, JsonConvert.SerializeObject(e, Formatting.Indented));

    }

    public string RetrieveLoggedException(ExceptionType exType)
    {
      if (IsALogPending(exType))
      {
        if (exType == ExceptionType.Unhandled)
          return fsh.ReadFile(UnhandledExceptionFilePath);
        else
          return fsh.ReadFile(HandledExceptionFilePath);
      }
      return null;
    }

    public bool DeleteLoggedException(ExceptionType exType)
    {
      if (exType == ExceptionType.Unhandled)
        return fsh.DeleteFile(UnhandledExceptionFilePath);
      else
        return fsh.DeleteFile(HandledExceptionFilePath);
    }

    public bool IsALogPending(ExceptionType exType)
    {
      if (exType == ExceptionType.Unhandled)
        return fsh.FileExist(UnhandledExceptionFilePath);
      else
        return fsh.IsFileOlderThan(HandledExceptionFilePath, requiredAge);
    }
  }

  public enum ExceptionType
  {
    Handled, Unhandled
  }
}

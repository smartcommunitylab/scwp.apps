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
    private readonly string ExceptionFilePath = "loggedExceptions.log";

    public ExceptionLoggerHelper()
    {
      fsh = new FileStorageHelper();
    }

    public void LogNewException(Exception e)
    {
      fsh.WriteFile(ExceptionFilePath, JsonConvert.SerializeObject(e), true);
    }

    public string RetrieveLoggedException()
    {
      if (IsALogPending())
      {
        return fsh.ReadFile(ExceptionFilePath);
      }
      return null;
    }

    public bool DeleteLoggedException()
    {
      return fsh.DeleteFile(ExceptionFilePath);
    }

    public bool IsALogPending()
    {
      return fsh.FileExist(ExceptionFilePath);
    }
  }
}

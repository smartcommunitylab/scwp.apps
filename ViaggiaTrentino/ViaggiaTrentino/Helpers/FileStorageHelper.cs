using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.Helpers
{
  public class FileStorageHelper
  {
    IsolatedStorageFile isf;

    public FileStorageHelper()
    {
      isf = IsolatedStorageFile.GetUserStoreForApplication();
    }

    public bool WriteFile(string name, string content, bool overwrite = false)
    {
      if (isf.FileExists(name) && !overwrite)
        return false;

      bool fileIORes;

      try
      {
        using (StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(name, FileMode.Create, isf)))
        {
          sw.Write(content);
        }
      }
      catch
      {
        fileIORes = false;
      }
      fileIORes = true;

      return fileIORes;
    }

    public string ReadFile(string name)
    {
      if (!isf.FileExists(name))
        return null;

      string result;
      try
      {
        using (StreamReader rw = new StreamReader(new IsolatedStorageFileStream(name, FileMode.Open, isf)))
        {
          result = rw.ReadToEnd();
        }
      }
      catch
      {
        result = null;
      }

      return result;
    }

  }
}


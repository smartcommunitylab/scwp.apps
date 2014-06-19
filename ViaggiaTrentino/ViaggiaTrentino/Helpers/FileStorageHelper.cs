using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.Helpers
{
  /// <summary>
  /// Helper class that wraps the IsolatedStorageFile object to easily read/write on file
  /// </summary>
  public class FileStorageHelper
  {
    IsolatedStorageFile isf;

    public FileStorageHelper()
    {
      isf = IsolatedStorageFile.GetUserStoreForApplication();
    }

    /// <summary>
    /// Writes a string on filesystem. Unless otherwise specified, if a file with the same name already exists, this function will not write any data
    /// </summary>
    /// <param name="name">name of the destination file</param>
    /// <param name="content">the string that should be wrote in the file</param>
    /// <param name="overwrite">boolean value indicating wheather the file should be overwritten if it already exists. Default value is false.</param>
    /// <returns>a boolean value indicating the success of the operation</returns>
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

    /// <summary>
    /// Reads a specified file as a whole string. If the file does not exist, null is returned
    /// </summary>
    /// <param name="name">the name of the file to read</param>
    /// <returns>the content of the file, in string form</returns>
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


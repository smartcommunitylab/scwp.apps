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

    /// <summary>
    /// Deletes a specified file from the filesystem
    /// </summary>
    /// <param name="name">name of the condemned file</param>
    /// <returns>a boolean value indicating whether the file has been deleted or not</returns>
    public bool DeleteFile(string name)
    {
      bool resVal;
      if (!isf.FileExists(name))
        resVal = false;
      else
      {
        try
        {
          isf.DeleteFile(name);
          resVal = true;
        }
        catch
        {
          resVal = false;
        }
      }
      return resVal;
    }
    
    /// <summary>
    /// Checks whether a specific file exists or not
    /// </summary>
    /// <param name="name">name of the destination file</param>
    /// <returns>a boolean value indicating the existance of the file</returns>
    public bool FileExist(string name)
    {
      return isf.FileExists(name);
    }

    /// <summary>
    /// Writes a string on filesystem. if the file already exists, the content will be appended to the existing file
    /// </summary>
    /// <param name="name">name of the destination file</param>
    /// <param name="content">the string that should be wrote in the file</param>
    /// <returns>a boolean value indicating the success of the operation</returns>
    public bool AppendFile(string name, string content)
    {
      bool fileIORes;

      try
      {
        using (StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(name, FileMode.Append, isf)))
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
    /// Checks if a specified file is older than a certain age
    /// </summary>
    /// <param name="name">name of the requested file</param>
    /// <param name="age">required minimum age for the file</param>
    /// <returns>a boolean value indicating whether the requested file is older than the desired age</returns>
    public bool IsFileOlderThan(string name, TimeSpan age)
    {
      if (!isf.FileExists(name))
        return false;

      var fileCreationDate = isf.GetCreationTime(name);

      if ((DateTime.Now - fileCreationDate) > age)
        return true;
      return false;
    }

  }
}


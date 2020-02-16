using System;

namespace Hungabor01Website.Utilities.Classes
{
  /// <summary>
  /// Extension methods
  /// </summary>
  public static class ExtensionMethods
  {
    /// <summary>
    /// Null checking for method parameters of any type
    /// Note, that this uses the == operator, so be careful, when overloading it
    /// </summary>
    /// <param name="obj">The object, which is under checking</param>
    /// <param name="parameterName">The name of the parameter</param>
    /// <exception cref="ArgumentNullException">If the object is null</exception>
    public static void ThrowExceptionIfNull(this object obj, string parameterName)
    {
      if (obj == null)
      {
        throw new ArgumentNullException(parameterName);
      }
    }

    /// <summary>
    /// Null and white space checking for method parameters of string
    /// </summary>
    /// <param name="obj">The string, which is under checking</param>
    /// <param name="parameterName">The name of the parameter</param>
    /// <exception cref="ArgumentException">If the object is null or white space</exception>
    public static void ThrowExceptionIfNullOrWhiteSpace(this string str, string parameterName)
    {
      if (string.IsNullOrWhiteSpace(str))
      {
        throw new ArgumentException(parameterName);
      }
    }
  }
}

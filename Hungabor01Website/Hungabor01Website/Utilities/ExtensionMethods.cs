using System;

namespace Hungabor01Website.Utilities
{
  /// <summary>
  /// Extension methods
  /// </summary>
  public static class ExtensionMethods
  {
    /// <summary>
    /// Null checking for method parameters
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
  }
}

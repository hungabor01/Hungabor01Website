namespace Hungabor01Website.Utilities.Interfaces
{
  /// <summary>
  /// Interface to validate, that a given email address has valid format or not
  /// </summary>
  public interface IEmailValidator
  {
    /// <summary>
    /// Checks, that the email address is in valid format or not
    /// Based on the official email pattern
    /// </summary>
    /// <param name="email">The email address to check</param>
    /// <returns>Valid or not</returns>
    public bool IsValidEmail(string email);
  }
}
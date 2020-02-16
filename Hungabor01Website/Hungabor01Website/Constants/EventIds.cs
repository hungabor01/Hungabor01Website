namespace Hungabor01Website.Constants
{
  /// <summary>
  /// Stores the event id-s for logging
  /// </summary>
  public static class EventIds
  {
    //Errors from Error views
    public static readonly int ExceptionCodeHandlerError = 3000;
    public static readonly int UnhandledErrorError = 3001;

    //Confirmation email
    public static readonly int ConfirmationEmailSent = 3010;
    public static readonly int ConfirmationEmailSentError = 3011;

    //Account related events    
    public static readonly int ConfirmEmailUserIdNullError = 3020;
    public static readonly int ConfirmEmailCannotConfirmError = 3021;
    public static readonly int LoginInvalidUsername = 3022;
    public static readonly int LoginError = 3023;
    public static readonly int ExternalLoginCallbackError = 3024;
    public static readonly int ExternalLoginCallbackEmailError = 3025;
    public static readonly int RegisterCreateUserError = 3026;
    public static readonly int RegisterExternalCreateUserError = 3027;
    public static readonly int ResetPasswordError = 3028;
  }
}

namespace Hungabor01Website.Constants
{
  /// <summary>
  /// Stores the event id-s for logging
  /// </summary>
  public static class EventIds
  {
    //Confirmation email
    public static readonly int ConfirmationEmailSent = 3000;
    public static readonly int ConfirmationEmailSentError = 3001;

    //Account related events
    public static readonly int RegisterCreateUserError = 3010;
    public static readonly int ConfirmEmailUserIdNullError = 3011;
    public static readonly int ConfirmEmailCannotConfirmError = 3012;
    public static readonly int LoginInvalidUsername = 3013;
    public static readonly int LoginError = 3014;
    public static readonly int ExternalLoginCallbackError = 3015;
    public static readonly int ExternalLoginCallbackEmailError = 3016;
    public static readonly int ResetPasswordError = 3017;
    public static readonly int RegisterExternalCreateUserError = 3018;

    //Errors from Error views
    public static readonly int ExceptionCodeHandlerError = 3030;
    public static readonly int UnhandledErrorError = 3031;
  }
}

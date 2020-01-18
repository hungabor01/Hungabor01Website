namespace Hungabor01Website.Constants
{
  /// <summary>
  /// Stores the event id-s for logging
  /// </summary>
  public static class EventIds
  {
    //Confirmation email
    public const int ConfirmationEmailSent = 3000;
    public const int ConfirmationEmailSentError = 3001;

    //Account related events
    public const int RegisterCreateUserError = 3010;
    public const int ConfirmEmailUserIdNullError = 3011;
    public const int ConfirmEmailCannotConfirmError = 3012;
    public const int LoginInvalidUsername = 3013;
    public const int LoginError = 3014;
    public const int ExternalLoginCallbackError = 3015;
    public const int ExternalLoginCallbackEmailError = 3016;
    public const int ResetPasswordError = 3017;
    public const int RegisterExternalCreateUserError = 3018;

    //Errors from Error views
    public const int ExceptionCodeHandlerError = 3030;
    public const int UnhandledErrorError = 3031;
  }
}

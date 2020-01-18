namespace Hungabor01Website.Constants
{
  /// <summary>
  /// Stores the constant strings
  /// </summary>
  public static class Strings
  {
    //Email sending
    public const string NotifyUserConfirmationEmailSent = "Confirmation email is sent. Please click on the link in the email.";
    public const string NotifyUserConfirmationEmailSentError = "Could not send the confirmation email. Please try again signing up.";

    //Confirmation email
    public const string ConfirmationEmailSubject = "Email confirmation request from {0}.";       
    public const string ConfirmationEmailBody =
      "Hey,\n" + 
      "Thank you for your registration. Please click to the link below to finish your registration.\n" +
      "{0}\n" +
      "For further assistance, please contact me on hungabor01@gmail.com";

    //Password reset email
    public const string PasswordResetEmailSubject = "Password reset link from {0}.";
    public const string PasswordResetEmailBody =
      "Hey,\n" +
      "Please click to the link below and enter your new password.\n" +
      "{0}\n" +
      "For further assistance, please contact me on hungabor01@gmail.com";

    //Errors from Error views
    public const string Error404 = "The requested resource could not be found.";
    public const string UnexpectedError = "An unexpected error has occured.";

    //Account related strings
    public const string AccountError = "Error {0} happened at user {1}.";
    public const string InvalidLogin = "Invalid login attempt.";
    public const string EmailNotConfirmed = "Email is not confirmed yet.";
    
    public const string EmailConfirmationError = "Email cannot be confirmed.";    

    public const string ExternalLoginCallbackEmailError = "Email is not received from {0} external provider.";
    public const string ExternalLoginError = "Error loading external login information.";
    public const string ExternalProviderError = "Error from external provider: {0}.";
    public const string EmailClaimError = "Email claim not received from: {0}.";

    public const string EmailIsTaken = "Email {0} is already in use.";
    public const string UsernameIsTaken = "Username {0} is already in use.";

    public const string ForgotPasswordSent = "Password reset email has been sent to {0}.";
    public const string ResetPasswordNotification = "Password has been changed for {0}.\nYou can sign in with your new password.";
    public const string ResetPasswordError = "Could not change password for {0} with toke {1}.";

    //Attribute strings
    public const string FileExtensionIsNotValid = "This file extension is not allowed!";
    public const string MaxFileSizeError = "Maximum allowed file size is {0} bytes.";
  }
}

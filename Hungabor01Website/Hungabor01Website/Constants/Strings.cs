namespace Hungabor01Website.Constants
{
  /// <summary>
  /// Stores the static readonlyant strings
  /// </summary>
  public static class Strings
  {
    //Email sending
    public static readonly string NotifyUserConfirmationEmailSent = "Confirmation email is sent. Please click on the link in the email.";
    public static readonly string NotifyUserConfirmationEmailSentError = "Could not send the confirmation email. Please try again signing up or contact me on hungabor01@gmail.com.";

    public static readonly string EmailRegexPattern =
      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

    //Confirmation email
    public static readonly string ConfirmationEmailSubject = "Email confirmation request from {0}.";       
    public static readonly string ConfirmationEmailBody =
      "Hey,\n" + 
      "Thank you for your registration. Please click on the link below to finish your registration.\n" +
      "{0}\n" +
      "For further assistance, please contact me on hungabor01@gmail.com";

    //Password reset email
    public static readonly string PasswordResetEmailSubject = "Password reset link from {0}.";
    public static readonly string PasswordResetEmailBody =
      "Hey,\n" +
      "Please click on the link below and enter your new password.\n" +
      "{0}\n" +
      "For further assistance, please contact me on hungabor01@gmail.com";

    //Errors from Error views
    public static readonly string Error404 = "The requested resource could not be found.";
    public static readonly string UnexpectedError = "An unexpected error has occured.";

    //Account related strings
    public static readonly string AccountError = "Error {0} happened at user {1}.";
    public static readonly string InvalidLogin = "Invalid login attempt.";
    public static readonly string LoginUsernameError = "Wrong username or email.";
    public static readonly string LoginPasswordError = "Wrong password.";
    public static readonly string EmailNotConfirmed = "Email is not confirmed yet.";
    
    public static readonly string EmailConfirmationError = "Email cannot be confirmed.";    

    public static readonly string ExternalLoginCallbackEmailError = "Email is not received from {0} external provider.";
    public static readonly string ExternalLoginError = "Error loading external login information.";
    public static readonly string ExternalProviderError = "Error from external provider: {0}.";
    public static readonly string EmailClaimError = "Email claim not received from: {0}.";

    public static readonly string EmailIsTaken = "Email {0} is already in use.";
    public static readonly string UsernameIsTaken = "Username {0} is already in use.";

    public static readonly string ForgotPasswordSent = "Password reset email has been sent to {0}.";
    public static readonly string ResetPasswordNotification = "Password has been changed for {0}.\nYou can use your new password from now on.";
    public static readonly string ResetPasswordError = "Could not change password for {0} with token {1}, because {2}.";

    public static readonly string ProfilePictureDeleted = "Your profile picture has been deleted. The default image is used from now on.";
    public static readonly string ProfilePictureDeletedError = "Your profile picture cannot be deleted.";

    //Attribute strings
    public static readonly string FileExtensionIsNotValid = "This file extension is not allowed!";
    public static readonly string MaxFileSizeError = "Maximum allowed file size is {0} bytes.";

    //AccountHistory descriptions
    public static readonly string LoginBuiltIn = "Built-in login.";
    public static readonly string LoginExternal = "{0} login.";
  }
}

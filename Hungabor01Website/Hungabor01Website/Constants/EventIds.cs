namespace Hungabor01Website.Constants
{
    public static class EventIds
    {
        //ErrorController
        public const int ExceptionCodeHandlerError = 3000;
        public const int UnhandledErrorError = 3001;

        //SmtpEmailSender
        public const int InvalidEmail = 3010;
        public const int SendEmailError = 3011;

        //RegisterController
        public const int RegisterCreateUserError = 3020;
        public const int ConfirmEmailUserIdNullError = 3021;
        public const int ConfirmEmailCannotConfirmError = 3022;

        //LoginController
        public const int LoginError = 3030;
        public const int LoginInvalidUsername = 3031;
        public const int ExternalLoginCallbackError = 3032;
        public const int ExternalLoginCallbackEmailError = 3033;
        public const int RegisterExternalCreateUserError = 3034;

        //AccountController
        public const int ForgotPasswordWrongEmail = 3040;
        public const int ResetPasswordError = 3041;
        public const int ResetPasswordWrongUser = 3042;
    }
}

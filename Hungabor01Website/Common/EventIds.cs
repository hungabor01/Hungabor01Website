namespace Common
{
    public static class EventIds
    {
        //ErrorController
        public const int ExceptionCodeHandlerError = 3000;
        public const int UnhandledErrorError = 3001;

        //SmtpEmailSender
        public const int InvalidEmail = 3010;
        public const int SendEmailError = 3011;

        //RegistrationController
        public const int RegistrationCreateUserError = 3020;
        public const int ConfirmEmailUserIdNullError = 3021;
        public const int ConfirmEmailCannotConfirmError = 3022;

        //LoginController
        public const int LoginError = 3030;
        public const int LoginInvalidUsername = 3031;
        public const int ExternalLoginCallbackError = 3032;
        public const int ExternalLoginCallbackEmailError = 3033;
        public const int RegistrationExternalCreateUserError = 3034;

        //ProfileController
        public const int ForgotPasswordWrongEmail = 3040;
        public const int ResetPasswordError = 3041;
        public const int ResetPasswordWrongUser = 3042;
    }
}

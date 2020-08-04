namespace Common.Strings
{
    public static class RegistrationStrings
    {
        public const string NotifyUserConfirmationEmailSent = "Confirmation email is sent. Please click on the link in the email.";
        public const string NotifyUserConfirmationEmailSentError = "Could not send the confirmation email. Please try again signing up or contact me on hungabor01@gmail.com.";

        public const string CannotRegister = "The registration was unsuccessful. Please try again or contact me on hungabor01@gmail.com.";

        public const string ConfirmationEmailSubject = "Email confirmation request from {0}.";
        public const string ConfirmationEmailBody =
            "Hey,\n" +
            "Thank you for your signing up. Please click on the link below to finish your registration.\n" +
            "{0}\n" +
            "For further assistance, please contact me on hungabor01@gmail.com";

        public const string EmailConfirmationError = "Email cannot be confirmed.";

        public const string EmailIsTaken = "Email {0} is already in use.";
        public const string UsernameIsTaken = "Username {0} is already in use.";
    }
}

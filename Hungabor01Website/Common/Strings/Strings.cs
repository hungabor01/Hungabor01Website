namespace Common.Strings
{
    public static class Strings
    {
        public const string LogFileName = "logs.txt";

        //ErrorController
        public const string Error404 = "The requested resource could not be found.";
        public const string UnexpectedError = "An unexpected error has occured.";

        //SmtpemailSender
        public const string InvalidEmail = "The provided email address is not valid: {0}";

        //Attribute strings
        public const string FileExtensionIsNotValid = "This file extension is not allowed!";
        public const string MaxFileSizeError = "Maximum allowed file size is {0} bytes.";

        //Common registration, login, account strings for logging
        public const string AccountError = "{0} error happened at user {1}.";        
    }
}

namespace Common.Strings
{
    public static class ProfileStrings
    {
        public const string PasswordResetEmailSubject = "Password reset link from {0}.";
        public const string PasswordResetEmailBody =
            "Hey,\n" +
            "Please click on the link below and enter your new password.\n" +
            "{0}\n" +
            "For further assistance, please contact me on hungabor01@gmail.com";

        public const string ForgotPasswordSent = "Password reset email has been sent to {0}.";
        public const string ForgotPasswordWrongEmail = "Invalid email in ForgotPassword action: {0}.";

        public const string ResetPasswordNotification = "Password has been changed for {0}.\nYou can use your new password from now on.";
        public const string ResetPasswordError = "Could not change password for {0} with token {1}, because {2}.";
        public const string ResetPasswordWrongUser = "User does not exist.";

        public const string ProfilePictureDeleted = "Your profile picture has been deleted. The default image is used from now on.";
        public const string ProfilePictureDeletedError = "Your profile picture cannot be deleted.";
    }
}

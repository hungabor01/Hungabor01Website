namespace Hungabor01Website.Utilities
{
  /// <summary>
  /// Interface for sending message through different ways
  /// </summary>
  public interface IMessageSender
  {
    /// <summary>
    /// Sends the message
    /// </summary>
    /// <param name="address">The address to send the message</param>
    /// <param name="subject">The subject of the message</param>
    /// <param name="message">The actual message body</param>
    /// <returns>Successfully sent or not</returns>
    public bool SendMessage(string address, string subject, string message);
  }
}

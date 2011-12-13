namespace Common
{
    public enum Message
    {
        None = 0, //No Message
        RequestError = 1, //Generic
        RequestMalformed = 10, //Client sent a malformed request
        DatabaseUnavailable = 254, //The database could not be contacted
        ServerUnavailable = 255 //There is no server available to process this request
    }
}
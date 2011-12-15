using Common;

namespace Router.Classes
{
    internal class Response
    {
        public object Data;
        public Message? Error;
        public string Path = string.Empty;
        public Request Request;
        public double Time;
    }
}
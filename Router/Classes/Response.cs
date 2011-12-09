using Constants;
using Newtonsoft.Json;
using System;

namespace Router.Classes
{
    public class Response
    {
        public string Path = string.Empty;
        public object Data = null;
        public Message? Error = null;
        public double Time = 0;
    }
}

using Common;
using Newtonsoft.Json;
using System;

namespace Router.Classes
{
    class Response
    {
        public Request Request = null;
        public string Path = string.Empty;
        public object Data = null;
        public Message? Error = null;
        public double Time = 0;
    }
}

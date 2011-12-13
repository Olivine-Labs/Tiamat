using System;

namespace Router.Classes
{
    internal class Server
    {
        public Guid Id = Guid.NewGuid();
        public String Path = "";

        public Boolean Forward(Request request)
        {
            //TODO - forward request on to the other server
            return false;
        }
    }
}
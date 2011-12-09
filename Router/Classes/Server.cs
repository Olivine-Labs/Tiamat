using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Router.Classes
{
    class Server
    {
        public  Guid    Id      = Guid.NewGuid();
        public  String  Path    = "";

        public void Forward(Request request)
        {
            //TODO - forward request on to the other server
        }
    }
}

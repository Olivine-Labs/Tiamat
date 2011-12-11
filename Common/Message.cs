using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public enum Message
    {
        NONE                        = 0,        //No Message
        REQUEST_ERROR               = 1,        //Generic
        REQUEST_MALFORMED           = 10,       //Client sent a malformed request
        DATABASE_UNAVAILABLE        = 254,      //The database could not be contacted
        SERVER_UNAVAILABLE          = 255       //There is no server available to process this request
    }
}

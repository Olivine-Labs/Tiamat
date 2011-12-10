using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vocale.Classes.Exceptions
{
    class VocaleException
    {
        public Exception NestedException = null;
        public String Message = String.Empty;
    }
}

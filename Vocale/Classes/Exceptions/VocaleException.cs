using System;

namespace Vocale.Classes.Exceptions
{
    internal class VocaleException
    {
        public String Message = String.Empty;
        public Exception NestedException;
    }
}
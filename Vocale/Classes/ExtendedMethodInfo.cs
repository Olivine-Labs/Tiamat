using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Vocale.Classes
{
    class ExtendedMethodInfo
    {
        public MethodInfo MethodInfo = null;
        public Object Context = null;
        public delegate Object MethodType(params Object[] parameters);
        public MethodType Method = null;
    }
}

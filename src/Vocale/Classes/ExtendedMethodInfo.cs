using System;
using System.Reflection;

namespace Vocale.Classes
{
    internal class ExtendedMethodInfo
    {
        #region Delegates

        public delegate Object MethodType(params Object[] parameters);

        #endregion

        public Object Context;

        public MethodType Method;
        public MethodInfo MethodInfo;
    }
}
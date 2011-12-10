using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Vocale.Classes;
using Vocale.Classes.Exceptions;
using System.Threading;

namespace Vocale
{
    public class Vocale
    {
        private Dictionary<String, ExtendedMethodInfo> _commands = new System.Collections.Generic.Dictionary<string, ExtendedMethodInfo>();

        public void Register(String commandName, Type type)
        {
            ExtendedMethodInfo aMethod = new ExtendedMethodInfo();
            aMethod.Method = type.GetMethod(commandName);
            _commands.Add(commandName, aMethod);
        }

        public void Register(Type type)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo rawMethod in methods)
            {
                ExtendedMethodInfo aMethod = new ExtendedMethodInfo();
                aMethod.Method = rawMethod;
                _commands.Add(rawMethod.Name, aMethod);
            }
        }

        public void Register(String commandName, Object type)
        {
            ExtendedMethodInfo aMethod = new ExtendedMethodInfo();
            aMethod.Method = type.GetType().GetMethod(commandName);
            aMethod.Context = type;
            _commands.Add(commandName, aMethod);
        }

        public void Register(Object type)
        {
            MethodInfo[] methods = type.GetType().GetMethods(BindingFlags.Public);
            foreach (MethodInfo rawMethod in methods)
            {
                ExtendedMethodInfo aMethod = new ExtendedMethodInfo();
                if(!rawMethod.IsStatic)
                    aMethod.Context = type;
                aMethod.Method = rawMethod;
                _commands.Add(rawMethod.Name, aMethod);
            }
        }

        public Boolean Exists(String commandName)
        {
            ExtendedMethodInfo trash;
            return _commands.TryGetValue(commandName, out trash);
        }

        public void Remove(String commandName)
        {
            _commands.Remove(commandName);
        }

        public void RemoveAll()
        {
            _commands.Clear();
        }

        public Object Execute(String commandName, params Object[] parameters)
        {
            Object result = null;
            ExtendedMethodInfo aMethod = null;
            _commands.TryGetValue(commandName, out aMethod);
            if (aMethod != null)
            {
                try
                {
                    result = aMethod.Method.Invoke(aMethod.Context, parameters);
                }
                catch (TargetParameterCountException)
                {
                    VocaleException anException = new VocaleException();
                    anException.Message = "Invalid parameter count. The format of the method call should be " + commandName + "(";
                    foreach (Object parameter in parameters)
                    {
                        anException.Message += parameter.GetType().ToString() + ", ";
                    }
                    result = anException;
                }
                catch (ArgumentException)
                {
                    VocaleException anException = new VocaleException();
                    anException.Message = "Invalid argument type. The format of the method call should be " + commandName + "(";
                    foreach(Object parameter in parameters)
                    {
                        anException.Message += parameter.GetType().ToString() + ", ";
                    }
                    result = anException;
                }
                catch (Exception e)
                {
                    VocaleException anException = new VocaleException();
                    anException.NestedException = e;
                    anException.Message = "An exception has occurred in the called method.";
                    result = anException;
                }
            }

            return result;
        }
    }
}

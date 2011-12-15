using System;
using System.Collections.Generic;
using System.Reflection;
using Vocale.Classes;
using Vocale.Classes.Exceptions;

namespace Vocale
{
    public class Vocale
    {
        private readonly Dictionary<String, ExtendedMethodInfo> _commands = new Dictionary<string, ExtendedMethodInfo>();

        public void Register(String commandName, Type type)
        {
            var aMethod = new ExtendedMethodInfo {MethodInfo = type.GetMethod(commandName)};
            aMethod.Method =
                (ExtendedMethodInfo.MethodType)
                Delegate.CreateDelegate(typeof (ExtendedMethodInfo.MethodType), aMethod.MethodInfo);
            _commands.Add(commandName, aMethod);
        }

        public void Register(Type type)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo rawMethod in methods)
            {
                var aMethod = new ExtendedMethodInfo {MethodInfo = rawMethod};
                aMethod.Method =
                    (ExtendedMethodInfo.MethodType)
                    Delegate.CreateDelegate(typeof (ExtendedMethodInfo.MethodType), aMethod.MethodInfo);
                _commands.Add(rawMethod.Name, aMethod);
            }
        }

        public void Register(String commandName, Object type)
        {
            var aMethod = new ExtendedMethodInfo { MethodInfo = type.GetType().GetMethod(commandName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy), Context = type };
            aMethod.Method =
                (ExtendedMethodInfo.MethodType)
                Delegate.CreateDelegate(typeof (ExtendedMethodInfo.MethodType), aMethod.Context, aMethod.MethodInfo);
            _commands.Add(commandName, aMethod);
        }

        public void Register(Object type)
        {
            MethodInfo[] methods =
                type.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            foreach (MethodInfo rawMethod in methods)
            {
                if (rawMethod.ReturnType == typeof (Object))
                {
                    var aMethod = new ExtendedMethodInfo();
                    if (!rawMethod.IsStatic)
                    {
                        aMethod.Context = type;
                    }
                    aMethod.MethodInfo = rawMethod;
                    aMethod.Method =
                        (ExtendedMethodInfo.MethodType)
                        Delegate.CreateDelegate(typeof (ExtendedMethodInfo.MethodType), aMethod.Context,
                                                aMethod.MethodInfo);
                    _commands.Add(rawMethod.Name, aMethod);
                }
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
            ExtendedMethodInfo aMethod;
            _commands.TryGetValue(commandName, out aMethod);
            if (aMethod != null)
            {
                try
                {
                    result = aMethod.Method(parameters);
                }
                catch (TargetParameterCountException)
                {
                    var anException = new VocaleException
                    {
                        Message =
                            "Invalid parameter count. The format of the method call should be " +
                            commandName + "("
                    };

                    foreach (Object parameter in parameters)
                    {
                        anException.Message += parameter.GetType() + ", ";
                    }
                    result = anException;
                }
                catch (ArgumentException)
                {
                    var anException = new VocaleException
                    {
                        Message =
                            "Invalid argument type. The format of the method call should be " +
                            commandName + "("
                    };

                    foreach (Object parameter in parameters)
                    {
                        anException.Message += parameter.GetType() + ", ";
                    }
                    result = anException;
                }
                catch (Exception e)
                {
                    var anException = new VocaleException
                    {
                        NestedException = e,
                        Message = "An exception has occurred in the called method."
                    };
                    result = anException;
                }
            }

            return result;
        }
    }
}
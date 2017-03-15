using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;

namespace nhn_EventSystem
{
    public class nhn_ObjectMethodData
    {

        public List<MethodInfo> methods = new List<MethodInfo>();

        public nhn_ObjectMethodData(Type t)
        {
            methods = GetMethodsOfType(t);
        }

        private List<MethodInfo> GetMethodsOfType(Type t)
        {

            List<MethodInfo> methods = new List<MethodInfo>();

            foreach (MethodInfo m in t.GetMethods((BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)))
            {
                if (methods.Contains(m))
                    continue;

                foreach (string s in nhn_EventHandler.EventTypes.Select(p => p.EventPrefix))
                {
                    if (m.Name.Contains(s))
                    {
                        methods.Add(m);
                        break;
                    }
                }
            }
            return methods;
        }
    }
}
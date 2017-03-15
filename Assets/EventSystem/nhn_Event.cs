using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace nhn_EventSystem {

    /// <summary>
    /// A base class for an event handled by the nhn_EventHandler, extend it to implement custom
    /// events with specific callbacks and constraints
    /// </summary>
    public abstract class nhn_Event {

        public string EventName;
        protected List<nhn_EventType> EventTypes = new List<nhn_EventType>();
        protected Dictionary<string, nhn_Delegate> delegates = new Dictionary<string, nhn_Delegate>();
        protected nhn_EventHandler handler;

        public nhn_Event(string name, nhn_EventHandler h)
        {
            EventName = name;
            handler = h;
        }

        protected virtual void InitializeCallbacks()
        {
            if (EventTypes == null || EventTypes.Count == 0)
                return;

            foreach (nhn_EventType i in EventTypes)
            {
                delegates.Add(i.EventPrefix, new nhn_Delegate(i.DelegateType));
            }
        }

        public virtual void Register(object target, string methodName, string prefixName)
        {
            nhn_Delegate d = null;
            delegates.TryGetValue(prefixName, out d);

            if (d == null)
                return;

            //todo: Verify methods before adding, although it shouldn't cause any errors as is

            d.AddMethod(target, methodName);
        }

        public virtual void UnRegister(object target)
        {
            foreach (nhn_Delegate d in delegates.Values)
            {
                d.RemoveMethodsFromObject(target);
            }
        }

    }
}
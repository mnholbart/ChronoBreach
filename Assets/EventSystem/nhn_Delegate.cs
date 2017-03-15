using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace nhn_EventSystem
{
    public class nhn_Delegate
    {

        public delegate void EventCallback();
        public delegate bool EventCondition();

        public EventCallback MyEventCallback = null;
        public EventCondition MyEventCondition = null;

        public void Empty()
        {

        }

        public bool NoConditions()
        {
            return true;
        }

        public DelegateType MyDelegateType;

        public nhn_Delegate(DelegateType T = DelegateType.None)
        {
            InitializeDelegateType(T);
        }

        public void InitializeDelegateType(DelegateType T)
        {
            MyDelegateType = T;
            switch (T)
            {
                case DelegateType.None: MyEventCallback = null; MyEventCondition = null; break;
                case DelegateType.EventCallback: MyEventCallback = Empty; break;
                case DelegateType.EventCondition: MyEventCondition = NoConditions; break;
                default: break;
            }
        }

        /// <summary>
        /// Add a method to the current delegate type from a methods name
        /// </summary>
        public void AddMethod(object target, string methodName)
        {
            if (MyDelegateType == DelegateType.EventCallback)
            {
                Delegate d = Delegate.CreateDelegate(MyEventCallback.GetType(), target, methodName, false, false);
                MyEventCallback += (EventCallback)d;
            }
            else if (MyDelegateType == DelegateType.EventCondition)
            {
                Delegate d = Delegate.CreateDelegate(MyEventCondition.GetType(), target, methodName, false, false);
                MyEventCondition += (EventCondition)d;
            }
        }

        public void RemoveMethodsFromObject(object o)
        {
            if (MyDelegateType == DelegateType.EventCallback)
            {
                List<Delegate> dels = new List<Delegate>(MyEventCallback.GetInvocationList());

                if (dels == null || dels.Count == 0)
                    return;

                foreach (Delegate d in dels)
                {
                    if (d.Target == o)
                        MyEventCallback -= (EventCallback)d;
                }
            }
            else if (MyDelegateType == DelegateType.EventCondition)
            {
                List<Delegate> dels = new List<Delegate>(MyEventCondition.GetInvocationList());

                if (dels == null || dels.Count == 0)
                    return;

                foreach (Delegate d in dels)
                {
                    if (d.Target == o)
                        MyEventCondition -= (EventCondition)d;
                }
            }
        }
    }

}

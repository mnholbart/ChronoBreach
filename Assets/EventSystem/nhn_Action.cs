using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nhn_EventSystem
{
    /// <summary>
    /// An action is a one time activated event that implements callbacks for failing/succeeding to start
    /// and conditions for starting
    /// </summary>
    public class nhn_Action : nhn_Event
    {
        public nhn_Action(string name, nhn_EventHandler h) : base(name, h)
        {
            EventTypes.Add(nhn_EventHandler.GetEventTypeFromName("OnStart"));
            EventTypes.Add(nhn_EventHandler.GetEventTypeFromName("OnFailStart"));
            EventTypes.Add(nhn_EventHandler.GetEventTypeFromName("CanStart"));
            InitializeCallbacks();
        }

        public virtual void TryStart()
        {
            foreach (nhn_Delegate.EventCondition d in delegates["CanStart"].MyEventCondition.GetInvocationList())
            {
                if (!d.Invoke())
                {
                    delegates["OnFailStart"].MyEventCallback.Invoke();
                    return;
                }
            }

            delegates["OnStart"].MyEventCallback.Invoke();
        }
    }
}
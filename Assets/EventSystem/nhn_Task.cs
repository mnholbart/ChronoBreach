using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nhn_EventSystem
{
    /// <summary>
    /// A task is an activated event that implements callbacks for failing/succeeding to start
    /// and conditions for starting and continues to run until its timer runs out
    /// </summary>
    public class nhn_Task : nhn_Event
    {
        private bool _active;
        public bool Active
        {
            get { return _active; } 
            protected set { _active = value; }
        }

        public float minTaskTime = 0.01f; //Min time to run before it can be paused
        public float maxTaskTime = 0.01f; //Max time to run before it auto stops

        protected float timeUntilCanStop; //Time when this task can be stopped, set on activation

        public nhn_Task(string name, nhn_EventHandler h) : base(name, h)
        {
            EventTypes.Add(nhn_EventHandler.GetEventTypeFromName("OnStart"));
            EventTypes.Add(nhn_EventHandler.GetEventTypeFromName("OnFailStart"));
            EventTypes.Add(nhn_EventHandler.GetEventTypeFromName("CanStart"));
            EventTypes.Add(nhn_EventHandler.GetEventTypeFromName("CanStop"));
            EventTypes.Add(nhn_EventHandler.GetEventTypeFromName("OnStop"));
            EventTypes.Add(nhn_EventHandler.GetEventTypeFromName("OnFailStop"));
            InitializeCallbacks();
        }

        public virtual bool TryStart()
        {
            if (Active)
                return false;

            foreach (nhn_Delegate.EventCondition d in delegates["CanStart"].MyEventCondition.GetInvocationList())
            {
                if (!d.Invoke())
                {
                    delegates["OnFailStart"].MyEventCallback.Invoke();
                    return false;
                }
            }

            ForceStart();
            return true;
        }

        public virtual bool TryStop()
        {
            if (!Active)
                return false;

            if (Time.time < timeUntilCanStop)
            {
                delegates["OnFailStop"].MyEventCallback.Invoke();
                return false;
            }

            foreach (nhn_Delegate.EventCondition d in delegates["CanStop"].MyEventCondition.GetInvocationList())
            {
                if (!d.Invoke())
                {
                    delegates["OnFailStop"].MyEventCallback.Invoke();
                    return false;
                }
            }

            ForceStop();
            return true;
        }

        /// <summary>
        /// Initiates the task regardless of conditions
        /// </summary>
        protected virtual void ForceStart()
        {
            Active = true;

            timeUntilCanStop = Time.time + minTaskTime;

            handler.ScheduleCallback(this, maxTaskTime, delegate () { TaskFinished(); });
            delegates["OnStart"].MyEventCallback.Invoke();
        }

        /// <summary>
        /// Cancels the task regardless of conditions
        /// </summary>
        protected virtual void ForceStop()
        {
            Active = false;

            handler.CancelCallback(this);
            delegates["OnStop"].MyEventCallback.Invoke();
        }

        protected void TaskFinished()
        {
            ForceStop();
        }
    }
}
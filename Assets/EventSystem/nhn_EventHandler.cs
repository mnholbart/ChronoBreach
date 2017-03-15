using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace nhn_EventSystem
{
    public enum DelegateType
    {
        None, //Non initialized
        EventCallback, //A callback from an event
        EventCondition //A condition before an event can be ran
    }

    /// <summary>
    /// Struct holding method prefixes and their DelegateType
    /// </summary>
    public struct nhn_EventType
    {
        public string EventPrefix;
        public DelegateType DelegateType;

        public nhn_EventType(string name, DelegateType type)
        {
            EventPrefix = name;
            DelegateType = type;
        }
    }

    public abstract class nhn_EventHandler : MonoBehaviour
    {
        //Initialization fields
        private List<object> objectRegisterQueue = new List<object>(); //Objects waiting for events to be created before being registered
        private bool initialized = false;

        //Event data
        private Dictionary<string, nhn_Event> events = new Dictionary<string, nhn_Event>(); //All events this handler is tracking and initializing, created from child nhn_Event fields
        public static List<nhn_EventType> EventTypes = new List<nhn_EventType> { new nhn_EventType("OnStart", DelegateType.EventCallback),
                                                                                new nhn_EventType("OnFailStart", DelegateType.EventCallback),
                                                                                new nhn_EventType("OnStop", DelegateType.EventCallback),
                                                                                new nhn_EventType("OnFailStop", DelegateType.EventCallback),
                                                                                new nhn_EventType("CanStart", DelegateType.EventCondition),
                                                                                new nhn_EventType("CanStop", DelegateType.EventCondition) };

        public static Dictionary<Type, nhn_ObjectMethodData> storedTypes = new Dictionary<Type, nhn_ObjectMethodData>(); //Cache of method info on types loaded

        protected virtual void Start()
        {
            if (!initialized)
            {
                RegisterMyEvents();
                initialized = true; //Has to be before RegisterQeueudObjects or you get register race conditions, and after events are registered
                RegisterQueuedObjects();
            }
        }

        /// <summary>
        /// If other scripts run first this function will register any objects that tried to register themselves before events were created
        /// </summary>
        private void RegisterQueuedObjects()
        {
            if (objectRegisterQueue == null || objectRegisterQueue.Count == 0)
                return;

            foreach (object o in objectRegisterQueue)
            {
                Register(o);
            }
            objectRegisterQueue = null;
        }

        /// <summary>
        /// Register all the events from nhn_Event type field infos
        /// </summary>
        private void RegisterMyEvents()
        {
            List<FieldInfo> fields;
            fields = GetEventFieldInfos();
            object e;

            if (fields == null || fields.Count == 0)
                return;

            foreach (FieldInfo f in fields)
            {
                e = Activator.CreateInstance(f.FieldType, f.Name, this);
                if (e == null)
                    continue;

                f.SetValue(this, e);

                foreach (nhn_EventType et in EventTypes)
                {
                    string n = et.EventPrefix + "_" + f.Name;
                    if (!events.ContainsKey(n))
                        events.Add(n, e as nhn_Event);
                }
            }
        }

        /// <summary>
        /// Register an objects methods to existing events
        /// </summary>
        public void Register(object o)
        {
            if (o == null)
                return;
            if (!initialized)
            {
                objectRegisterQueue.Add(o);
                return;
            }
            nhn_ObjectMethodData d = GetMethodsFromObject(o);

            if (d == null)
                return;

            nhn_Event e;
            foreach (MethodInfo m in d.methods)
            {
                bool foundEvent = events.TryGetValue(m.Name, out e);
                if (!foundEvent)
                    continue;

                foreach (nhn_EventType et in EventTypes)
                {
                    if (m.Name.Contains(et.EventPrefix))
                    {
                        e.Register(o, m.Name, et.EventPrefix);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Remove any methods assigned to events from object
        /// </summary>
        public void Unregister(object o)
        {
            if (o == null)
                return;

            foreach (nhn_Event e in events.Values)
            {
                if (e == null)
                    return;

                e.UnRegister(o);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private nhn_ObjectMethodData GetMethodsFromObject(object o)
        {
            nhn_ObjectMethodData d = null;
            storedTypes.TryGetValue(o.GetType(), out d);
            if (d == null)
            {
                d = new nhn_ObjectMethodData(o.GetType());
                storedTypes.Add(o.GetType(), d);
            }
            return d;
        }

        /// <summary>
        /// Get all the nhn_Event fields on the EventHandler
        /// </summary>
        private List<FieldInfo> GetEventFieldInfos()
        {
            List<FieldInfo> fields = new List<FieldInfo>();

            //Not sure the repercussions of FlattenHierarchy may need to iterate through each class using t.BaseType grabbing each field
            Type t = GetType();
            fields.AddRange(t.GetFields(
                BindingFlags.FlattenHierarchy | 
                BindingFlags.Public | 
                BindingFlags.Instance | 
                BindingFlags.NonPublic)
                .ToList()
                .Where(p => p.FieldType.IsSubclassOf(typeof(nhn_Event)) || p.FieldType == typeof(nhn_Event)));

            if (fields == null || fields.Count == 0)
            {
                Debug.Log("No events found on EventHandler: " + name);
                return null;
            }
            else return fields;
        }

        /// <summary>
        /// Get the corresponding CallbackType from a prefix name
        /// </summary>
        public static nhn_EventType GetEventTypeFromName(string prefixName)
        {
            nhn_EventType type = EventTypes.First(p => p.EventPrefix == prefixName);
            return type;
        }


        //Todo -- all this should probably be its own scheduling class rather than the event handler
        public Dictionary<nhn_Event, IEnumerator> ScheduledCallbacks = new Dictionary<nhn_Event, IEnumerator>();
        public void ScheduleCallback(nhn_Event e, float timer, Callback c)
        {
            if (ScheduledCallbacks.ContainsKey(e))
                return;
            IEnumerator co = CallbackTimer(timer, c, e);
            StartCoroutine(co);
            ScheduledCallbacks.Add(e, co);
        }

        public void CancelCallback(nhn_Event e)
        {
            IEnumerator co;
            if (ScheduledCallbacks.TryGetValue(e, out co))
            {
                StopCoroutine(co);
                ScheduledCallbacks.Remove(e);
            } 
        }

        public delegate void Callback();
        public IEnumerator CallbackTimer(float timer, Callback c, nhn_Event e)
        {
            while (timer > 0)
            {
                yield return null;
                timer -= Time.deltaTime;
            }
            c.Invoke();
            ScheduledCallbacks.Remove(e);
        }
    }
}
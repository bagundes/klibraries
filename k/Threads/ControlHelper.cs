using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace k.Threads
{
    public static class ControlHelper
    {
        private static string LOG => typeof(ControlHelper).FullName;

        internal struct ThreadStruct
        {
            public readonly string name;
            public readonly Action action;
            public readonly string description;
            public readonly int Minutes;
            public DateTime Next { get; internal set; }

            public ThreadStruct(Action action, string description, int minutes) : this()
            {
                this.name = action.Method.DeclaringType.FullName;
                this.action = action;
                this.description = description;
                Minutes = minutes;
                Next = DateTime.Now;
            }

            public void UpdateTime()
            {
                Next = DateTime.Now.AddMinutes(Minutes);
            }

            public bool Run()
            {
                return DateTime.Now >= Next;
            }
        }

        private static Thread thread;

        internal static void End()
        {
            try
            {
                if (thread != null && thread.IsAlive)
                    thread.Abort();
                
                Diagnostic.Debug(LOG, null, $"Control thread finished");
            }
            catch (Exception ex)
            {
                var track = Diagnostic.TrackObject(actions);
                Diagnostic.Error(LOG, track, $"Fatal control thread error trying to terminate thread.");
                Diagnostic.Error(LOG, ex);
            }
        }

        private static List<ThreadStruct> actions;


        internal static void Start()
        {
            try
            {
                if (actions == null) actions = new List<ThreadStruct>();

                thread = new Thread(new ThreadStart(ControlTime));
                thread.Start();
                Diagnostic.Debug(LOG, null, $"Control thread started");
            }
            catch (Exception ex)
            {
                var track = Diagnostic.TrackObject(actions);
                Diagnostic.Error(LOG, track, $"Fatal control thread error.");
                Diagnostic.Error(LOG, ex);
            }
        }

        private static void ControlTime()
        {
            var controls = actions.Where(t => t.Run()).ToList();
            do
            {
                foreach (var action in controls)
                {
                    action.UpdateTime();
                    var date = DateTime.Now;
                    try
                    {
                        action.action();

                        Diagnostic.Debug(LOG, null, $"Performed {action.name} action in {(DateTime.Now - date).TotalSeconds} seconds. next round will be {action.Next}.");
                    }
                    catch(ThreadAbortException ex)
                    {
                        var track = Diagnostic.TrackObject(action);
                        Diagnostic.Error(LOG, track, $"Thread aborted, the {action.name} creates exception.");
                        Diagnostic.Error(LOG, ex);
                    }
                    catch (Exception ex)
                    {
                        var track = Diagnostic.TrackObject(action);
                        Diagnostic.Error(LOG, track, $"Error to execute {action.name} action in the thread.");
                        Diagnostic.Error(LOG, ex);
                    }
                }
                Thread.Sleep(60000); // 1 minute
            } while (true);
        }

        public static void Add(Action staticMethod, string description, int minutes)
        {

            if (actions == null) actions = new List<ThreadStruct>();

            var name = staticMethod.Method.DeclaringType.FullName;
            var thread = new ThreadStruct(staticMethod, description, minutes);
            var track = Diagnostic.TrackObject(thread);

            if (!actions.Where(t => t.name == name).Any())
            {                    
                actions.Add(thread);
                    
                Diagnostic.Debug(LOG, track,  $"Added {thread.name} action in the thread. Details: {thread.description}.");
            }else
            {
                var index = actions.FindIndex(t => t.name == name);
                actions[index] = thread;
                Diagnostic.Debug(LOG, track,  $"Updated {thread.name} action in the thread. Details: {thread.description}.");
            }
        }
        
    }
}

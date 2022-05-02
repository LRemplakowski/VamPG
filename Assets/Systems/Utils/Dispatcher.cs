using System;
using System.Collections.Generic;
using Utils.Singleton;

namespace Utils.Threading
{
    public class Dispatcher : Singleton<Dispatcher>
    {
        private List<Action> pending = new List<Action>();

        public void Invoke(Action fn)
        {
            lock (this.pending)
            {
                this.pending.Add(fn);
            }
        }

        private void InvokePending()
        {
            lock (this.pending)
            {
                foreach (Action action in this.pending)
                {
                    action();
                }

                this.pending.Clear();
            }
        }

        private void Update()
        {
            this.InvokePending();
        }
    }
}

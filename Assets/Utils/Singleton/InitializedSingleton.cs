using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.Singleton
{
    public abstract class InitializedSingleton<T> : Singleton<T>, IInitializable where T : Component
    {
        public abstract void Initialize();
    }
}

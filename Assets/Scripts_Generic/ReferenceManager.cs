using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunsetSystems.Management
{
    public class ReferenceManager : ExposableMonobehaviour
    {
        private static ReferenceManager instance;
        private void Start()
        {
            instance = this;
        }

        public static T GetManager<T>() where T : Manager => instance.GetComponentInChildren<T>(true);
    }

    public abstract class Manager : ExposableMonobehaviour  { }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunsetSystems.Management
{
    public class References : ExposableMonobehaviour
    {
        private static References instance;
        private void Start()
        {
            instance = this;
        }

        public static T Get<T>() where T : Manager => instance.GetComponentInChildren<T>(true);

        public static List<T> GetAll<T>() => instance.GetComponentsInChildren<T>(true).ToList();
    }

    public abstract class Manager : ExposableMonobehaviour  { }
}

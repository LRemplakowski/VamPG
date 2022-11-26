using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Utils.Threading
{
    public static class UnityAwaiters
    {
        public static async Task NextFrame()
        {
            int current = Time.frameCount;
            while (current == Time.frameCount)
            {
                await Task.Yield();
            }
        }
    }
}
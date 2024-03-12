using Sirenix.OdinInspector;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Dialogue
{
    public class NodeEventListener : MonoBehaviour
    {
        [Title("Config")]
        [SerializeField]
        private string _listenForNode = "";

        [Title("Events")]
        public UltEvent OnNodeEventMatch = new();

        public void OnNodeEvent(string nodeName)
        {
            if (_listenForNode.Equals(nodeName))
                OnNodeEventMatch?.InvokeSafe();
        }
    }
}

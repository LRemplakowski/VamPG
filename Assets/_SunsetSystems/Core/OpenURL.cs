using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Core
{
    public class OpenURL : MonoBehaviour
    {
        [SerializeField]
        private string _url;

        public void DoOpenURL()
        {
            if (string.IsNullOrWhiteSpace(_url) is false)
                Application.OpenURL(_url);
        }
    }
}

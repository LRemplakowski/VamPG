using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Economy.UI;
using UnityEngine;

namespace SunsetSystems.Economy
{
    public class TradingManager : SerializedMonoBehaviour
    {
        public static TradingManager Instance { get; private set; }

        [SerializeField]
        private TradingInterface _tradeUI;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void StartTrade(IMerchant merchant)
        {
            _tradeUI.InitializeTradeInterface(merchant);
        }
    }
}

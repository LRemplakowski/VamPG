using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.UI.Utils
{
    public interface IUserInterfaceUpdateReciever<T,U> where U : MonoBehaviour, IUserInterfaceView<T,U> where T : IGameDataProvider<T>
    {
        U ViewPrefab { get; }
        Transform ViewParent { get; }
        List<IUserInterfaceView<T, U>> ViewPool { get; }

        void DisableViews();

        void UpdateViews(IList<IGameDataProvider<T>> data)
        {
            DisableViews();
            if (data == null || data.Count <= 0)
            {
                Debug.LogWarning("UI Update Reciever recieved an empty or null collection!");
                return;
            }
            for (int i = 0; i < data.Count; i++)
            {
                IGameDataProvider<T> dataProvider = data[i];
                if (dataProvider == null)
                {
                    Debug.LogError("Null DataProvider while creating view!");
                    continue;
                }

                IUserInterfaceView<T,U> view;
                if (ViewPool.Count > i)
                {
                    Debug.Log("Getting view from pool!");
                    view = ViewPool[i];
                }
                else
                {
                    Debug.Log("Instantiating new view!");
                    view = Object.Instantiate(ViewPrefab, ViewParent);
                    ViewPool.Add(view);
                }
                view.UpdateView(dataProvider);
                (view as MonoBehaviour).gameObject.SetActive(true);
            }
        }
    }
}

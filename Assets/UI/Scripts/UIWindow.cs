using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class UIWindow : ExposableMonobehaviour
{
    public static Player player;

    private void Awake()
    {
        if(player == null)
        {
            player = FindObjectOfType<Player>();
        }
    }
}
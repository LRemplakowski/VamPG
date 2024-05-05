using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBarUI : SerializedMonoBehaviour
{
    private SelectedBarAction selected;
    private static readonly SelectedBarAction DEFAULT = new(BarAction.MOVE);

    public static ActionBarUI Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        selected = DEFAULT;
    }

    public SelectedBarAction GetSelectedBarAction()
    {
        return selected;
    }

    public void SetBarAction(int n)
    {
        BarAction action = (BarAction)n;
        SelectedBarAction newSelected = action switch
        {
            BarAction.MOVE => new SelectedBarAction(BarAction.MOVE),
            BarAction.ATTACK => new SelectedBarAction(BarAction.ATTACK),
            BarAction.SELECT_TARGET => new SelectedBarAction(BarAction.SELECT_TARGET),
            _ => DEFAULT,
        };
        selected = newSelected;
    }

    public class SelectedBarAction
    {
        public readonly BarAction actionType;

        public SelectedBarAction(BarAction actionType)
        {
            this.actionType = actionType;
        }
    }
}

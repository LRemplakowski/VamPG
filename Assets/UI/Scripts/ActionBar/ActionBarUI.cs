using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBarUI : ExposableMonobehaviour
{
    private SelectedBarAction selected;
    private static readonly SelectedBarAction DEFAULT = new SelectedBarAction(BarAction.MOVE);

    public static ActionBarUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
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

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class ContextAction : MonoBehaviour
{
    [SerializeField]
    protected Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public abstract void OnClick();
}


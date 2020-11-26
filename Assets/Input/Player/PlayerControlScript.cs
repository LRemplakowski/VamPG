using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlScript : MonoBehaviour
{
    private Player player;

    public void OnClick(InputAction.CallbackContext context)
    {
        Debug.Log(player);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            Debug.Log("ray hit\n"+hit.point);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = this.GetComponent<Player>();
    }

}

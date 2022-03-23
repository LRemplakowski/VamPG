
using System;
using SunsetSystems.Management;
using UnityEngine;


public abstract class InputHandler : Manager
{
    //This method should be called in case of any and all events handled by classes inheriting from InputHandler.
    //This method wraps around all methods that have to be called in case of ANY GUI event.
    //Action provided is what is supposed to happen in case of specific event in question.
    protected virtual void ManageInput(Action action)
    {
        Cleanup();
        action();
    }

    //This method should be called in case of any and all events handled by classes inheriting from InputHandler.
    //This method wraps around all methods that have to be called in case of ANY GUI event.
    //Action provided is what is supposed to happen in case of specific event in question.
    protected virtual void ManageInput<T>(Action<T> action, T arg)
    {
        Cleanup();
        action(arg);
    }

    private void Cleanup()
    {
        CustomContextMenu.ClearContextMenu();
    }
}

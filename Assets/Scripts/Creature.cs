using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : MonoBehaviour
{
    public int health;
    public int willpower;

    public int strength, dexterity, stamina;
    public int charisma, manipulation, composure;
    public int intelligence, wits, resolve;

    public abstract void Move();
}

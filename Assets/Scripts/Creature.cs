using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Creature : MonoBehaviour
{
    public NavMeshAgent agent;

    public int health;
    public int willpower;

    public int strength, dexterity, stamina;
    public int charisma, manipulation, composure;
    public int intelligence, wits, resolve;

    public abstract void Move(Vector3 moveTarget);

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
}

using UnityEngine;
using System.Collections;

[System.Serializable]
public class ArticyLocaData
{
    [SerializeField]  string locaID;
    public string LocaID { get { return locaID; } set { locaID = value; } }

    [SerializeField]  string value;
    public string Value { get { return value; } set { this.value = value; } }

    [SerializeField]  string contextPath;
    public string ContextPath { get { return contextPath; } set { contextPath = value; } }

    [SerializeField]  string isFinal;
    public string IsFinal { get { return isFinal; } set { isFinal = value; } }
}
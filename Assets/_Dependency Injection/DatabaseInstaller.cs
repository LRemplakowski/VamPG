using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Database Installer", menuName = "Installers/Database Installer")]
public class DatabaseInstaller : ScriptableObjectInstaller<DatabaseInstaller>
{
    public override void InstallBindings()
    {
    }
}
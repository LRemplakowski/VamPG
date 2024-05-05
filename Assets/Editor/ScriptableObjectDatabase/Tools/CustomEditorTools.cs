using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace MyLib.EditorTools.Tools
{
    public static class CustomEditorTools
    {
        /// <summary>
        /// Opens a new inspector window for given object.
        /// </summary>
        /// <typeparam name="T">The type of object to inspect.</typeparam>
        /// <param name="target">The object to inspect.</param>
        public static void InspectTarget<T>(UnityEngine.Object target)
        where T : UnityEngine.Object
        {
            var inspectorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            var inspectorInstance = ScriptableObject.CreateInstance(inspectorType) as EditorWindow;

            inspectorInstance.Show();

            UnityEngine.Object[] prevSelection = Selection.objects;
            Selection.objects = new UnityEngine.Object[1] { target };

            var isLocked = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public);
            isLocked.GetSetMethod().Invoke(inspectorInstance, new object[] { true });

            Selection.objects = prevSelection;
        }

        /// <summary>
        /// Used to get all derived types within the AppDomain
        /// </summary>
        /// <typeparam name="T">The type to start with.</typeparam>
        /// <param name="aAppDomain">The AppDomain to search.</param>
        /// <returns>A list of derived types of Type T.</returns>
        public static System.Type[] GetAllDerivedObjectTypes<T>(this System.AppDomain aAppDomain)
        where T : UnityEngine.Object
        {
            var result = new List<System.Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if ((type.IsSubclassOf(typeof(T)) || type == typeof(T)) && !type.IsAbstract && type.IsClass)
                        result.Add(type);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Used to get all derived types within the AppDomain
        /// </summary>
        /// <param name="aAppDomain">The AppDomain to search.</param>
        /// <param name="typeInput">The type to start with.</param>
        /// <returns>A list of derived types of Type T.</returns>
        public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type typeInput)
        {
            var result = new List<System.Type>();
            var assemblies = aAppDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if ((type.IsSubclassOf(typeInput) || type == typeInput) && !type.IsAbstract && type.IsClass)
                        result.Add(type);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Gets all the base types of a specified object.
        /// </summary>
        /// <param name="unityObject">The object to get base types for.</param>
        /// <param name="ignoreAbstract">Should we ignore abstract types?</param>
        /// <returns>A list of types that are bases of specified object.</returns>
        public static System.Type[] GetAllBaseTypes(Object unityObject, bool ignoreAbstract)
        {
            System.Type baseType = unityObject.GetType();
            List<System.Type> returnTypes = new List<System.Type>
            {
                baseType
            };

            System.Type nextType;
            int depth = 0;
            do
            {
                nextType = baseType.BaseType;
                if (nextType.Equals(typeof(ScriptableObject)))
                    break;

                if (nextType.IsClass && (!ignoreAbstract || (ignoreAbstract && !nextType.IsAbstract)))
                    returnTypes.Add(nextType);

                baseType = nextType;
                depth++;
            }
            while (!baseType.Equals(typeof(ScriptableObject)) && depth < 100);

            return returnTypes.ToArray();
        }
    }
}
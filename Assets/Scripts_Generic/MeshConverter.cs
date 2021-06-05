using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshConverter : ExposableMonobehaviour
{
    [ContextMenu("Convert to regular mesh")]
    public void ConvertToRegularMesh()
    {
        SkinnedMeshRenderer skinnedMesh = GetComponent<SkinnedMeshRenderer>();
        MeshRenderer regularMesh = gameObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        meshFilter.sharedMesh = skinnedMesh.sharedMesh;
        regularMesh.sharedMaterials = skinnedMesh.sharedMaterials;

        DestroyImmediate(skinnedMesh);
        DestroyImmediate(this);
    }
}

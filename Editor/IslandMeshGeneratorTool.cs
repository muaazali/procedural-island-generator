using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IslandMeshGenerator), true)]
public class IslandMeshGeneratorTool : Editor
{
    private IslandMeshGenerator islandGenerator;

    void Awake()
    {
        islandGenerator = (IslandMeshGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        if (GUILayout.Button("Generate Island"))
        {
            islandGenerator.GenerateIsland();
        }
    }
}

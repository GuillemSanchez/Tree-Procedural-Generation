using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TreeEditor;
using UnityEngine.UIElements;
public class BasicTreeGenerator : EditorWindow {


    // Core Variables
    Tree tree;
    TreeData advancedData;
   


 
    // Root Vars
    int seed = 0;
    float areaSpreed = 1.0f;
    float groundOffset = 0.0f;
    float LODoverall = 0.8f;

    
    [MenuItem("Tree Procedural Generation/BasicTreeGenerator")]
    private static void ShowWindow() {
        var window = GetWindow<BasicTreeGenerator>();
        window.titleContent = new GUIContent("BasicTreeGenerator");
        window.Show();
        // Primero que pongan el arbol despues que se cargan los valors del arbol y salen las opciones
    }

    private void OnGUI() {
        GUILayout.Label("test de arbol", EditorStyles.boldLabel);
        tree = EditorGUILayout.ObjectField("tree object", tree, typeof(Tree), true) as Tree;
        seed = EditorGUILayout.IntSlider("Seed",seed,1,999999);
        areaSpreed = EditorGUILayout.Slider("Area Spread",areaSpreed,0,10);
        groundOffset = EditorGUILayout.Slider("Ground OffSet",groundOffset,0,10);
        LODoverall = EditorGUILayout.Slider("LOD quality Overall", LODoverall, 0, 1);
    }

    void OnInspectorUpdate()
    {
        ChangeRoot();
    }

    private void ChangeRoot() {
        advancedData = tree.data as TreeData;


        
        advancedData.root.seed = seed;
        advancedData.root.rootSpread = areaSpreed;
        advancedData.root.groundOffset = groundOffset;
        advancedData.root.adaptiveLODQuality = LODoverall;
        
        
        
        advancedData.UpdateSeed(seed);
        Material[] materials;
        advancedData.UpdateMesh(tree.transform.worldToLocalMatrix, out materials);




    }
}


﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class ConditionEditor : EditorWindow
{
    private GameObject myTree;
    public TreeConditions myTreeConditions;

    [MenuItem("Tree Procedural Generation/Condition Editor")]
    private static void ShowWindow()
    {
        var window = GetWindow<ConditionEditor>();
        window.titleContent = new GUIContent("Condition Editor");
        window.Show();
    }

    private void OnGUI()
    {
        GetTree();
        if (myTree != null)
        {
            EditConditionTreeVars();
            Corrections();
        }
    }

    private void GetTree()
    {
        bool vChange = GUI.changed;
        myTree = EditorGUILayout.ObjectField(new GUIContent("Tree Object:", "Object with a tree Component."), myTree, typeof(GameObject), true) as GameObject;

        // We Check if there is something in myTree.
        if (myTree != null)
        {
            // We check if this something have the tree component.
            if (myTree.GetComponent<Tree>() == null)
            {
                // If not we transform it once again to null.
                myTree = null;
            }
        }

        // myTree has been changed
        if (vChange != GUI.changed)
        {
            if (myTree.GetComponent<ConditionCore>())
            {
                if (myTree.GetComponent<ConditionCore>().myConditions == null)
                {
                    myTree.GetComponent<ConditionCore>().myConditions = new TreeConditions();
                    AssetDatabase.CreateAsset(myTree.GetComponent<ConditionCore>().myConditions, "Assets/" + myTree.name + "conditions.asset");
                }
            }
            else
            {
                myTree.AddComponent<ConditionCore>();
                myTree.GetComponent<ConditionCore>().myConditions = new TreeConditions();
                AssetDatabase.CreateAsset(myTree.GetComponent<ConditionCore>().myConditions, "Assets/" + myTree.name + "conditions.asset");

            }

            myTreeConditions = myTree.GetComponent<ConditionCore>().myConditions;
        }

    }

    public void EditConditionTreeVars()
    {
        GUILayout.Space(20);
        GUILayout.Label("TREE CONDITIONS:", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("Trunk and branches");
        GUILayout.Space(5);
        myTreeConditions.optimHeight = EditorGUILayout.Slider(new GUIContent("Optimum Height:", "The peak height that the tree gets in the best conditions"), myTreeConditions.optimHeight, 0.001f, 50.0f);
        myTreeConditions.unOptimHeight = EditorGUILayout.Slider(new GUIContent("Apalling Height:", "The worst height that the tree gets in the worst conditions"), myTreeConditions.unOptimHeight, 0.001f, 50.0f);
        GUILayout.Space(5);
        myTreeConditions.optimRadius = EditorGUILayout.Slider(new GUIContent("Optimum Radius:", "The peak radius that the tree gets in the best conditions"), myTreeConditions.optimRadius, 0.001f, 5.0f);
        myTreeConditions.unOptimRadius = EditorGUILayout.Slider(new GUIContent("Apalling Radius:", "The worst radius that the tree gets in the worst conditions"), myTreeConditions.unOptimRadius, 0.001f, 5.0f);
        GUILayout.Space(5);
        myTreeConditions.optimWoodColor = EditorGUILayout.ColorField(new GUIContent("Optimum wood color:", "In the perfect conditions which tonality is the color of the wood?"), myTreeConditions.optimWoodColor);
        myTreeConditions.unOptimWoodColor = EditorGUILayout.ColorField(new GUIContent("Apalling wood color:", "In the worst conditions which tonality is the color of the wood?"), myTreeConditions.unOptimWoodColor);
        GUILayout.Space(10);
        GUILayout.Label("Leafs");
        GUILayout.Space(5);
        myTreeConditions.optimLeafSize = EditorGUILayout.Slider(new GUIContent("Optimum Leaf Size:", "The peak Leaf Size that the tree gets in the best conditions"), myTreeConditions.optimLeafSize, 0.001f, 10.0f);
        myTreeConditions.unOptimLeafSize = EditorGUILayout.Slider(new GUIContent("Apalling Leaf Size:", "The worst Leaf Size that the tree gets in the worst conditions"), myTreeConditions.unOptimLeafSize, 0.001f, 10.0f);
        GUILayout.Space(5);
        // Actual leafs?
        myTreeConditions.optimNumberLeafs = EditorGUILayout.IntField(new GUIContent("Optimum Number of Leafs:", "The peak Number of Leafs that the tree gets in the best conditions"), myTreeConditions.optimNumberLeafs);
        myTreeConditions.unOptimNumberLeafs = EditorGUILayout.IntField(new GUIContent("Apalling Number of Leafs:", "The worst Number of Leafs that the tree gets in the worst conditions"), myTreeConditions.unOptimNumberLeafs);
        GUILayout.Space(5);
        myTreeConditions.optimLeafColor = EditorGUILayout.ColorField(new GUIContent("Optimum leaf color:", "In the perfect conditions which tonality is the color of the leaf?"), myTreeConditions.optimLeafColor);
        myTreeConditions.unOptimLeafColor = EditorGUILayout.ColorField(new GUIContent("Apalling leaf color:", "In the worst conditions which tonality is the color of the leaf?"), myTreeConditions.unOptimLeafColor);

    }

    public void Corrections()
    {
        if (myTreeConditions.optimNumberLeafs < 0)
            myTreeConditions.optimNumberLeafs = 0;

        if (myTreeConditions.unOptimNumberLeafs < 0)
            myTreeConditions.unOptimNumberLeafs = 0;
    }
}


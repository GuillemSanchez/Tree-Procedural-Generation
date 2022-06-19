using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TreeEditor;



public class ConditionEditor : EditorWindow
{
    private GameObject myTree;
    private GameObject planeToSpawn;
    public TreeConditions myTreeConditions;
    public int numberOfOriginalLeafs = 0;
    private int numberOfTrees = 0;


    // Condition Tools ---------------------------------------


    bool showTree = true;
    bool showTemp = false;
    bool showWater = false;
    bool showSoil = false;
    bool showWind = false;

    [MenuItem("Window/Tree Procedural Generation/Condition Editor")]
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
            bool vChange = GUI.changed;
            numberOfOriginalLeafs = myTree.GetComponent<ConditionCore>().getOriginalLeafs();
            EditConditionTreeVars();
            Corrections();
        }
    }
    // TODO resetear el tree al original.

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

            bool needOriginal = false;
            if (myTree.GetComponent<ConditionCore>())
            {
                if (myTree.GetComponent<ConditionCore>().myConditions == null)
                {
                    myTree.GetComponent<ConditionCore>().myConditions = new TreeConditions();
                    AssetDatabase.CreateAsset(myTree.GetComponent<ConditionCore>().myConditions, "Assets/" + myTree.name + "conditions.asset");
                    needOriginal = true;
                }
            }
            else
            {
                myTree.AddComponent<ConditionCore>();
                myTree.GetComponent<ConditionCore>().myConditions = new TreeConditions();
                AssetDatabase.CreateAsset(myTree.GetComponent<ConditionCore>().myConditions, "Assets/" + myTree.name + "conditions.asset");
                needOriginal = true;

            }

            myTreeConditions = myTree.GetComponent<ConditionCore>().myConditions;

            if (myTreeConditions.myTemp == null)
            {
                myTreeConditions.myTemp = new TemperatureTool();
            }
            if (myTreeConditions.myWater == null)
            {
                myTreeConditions.myWater = new WaterTool();
            }
            if (myTreeConditions.myWind == null)
            {
                myTreeConditions.myWind = new WindTool();
            }
            if (myTreeConditions.mySoil == null)
            {
                myTreeConditions.mySoil = new SoilTool();
            }



            myTree.GetComponent<ConditionCore>().GetInfo();

            if (needOriginal)
            {
                myTree.GetComponent<ConditionCore>().GetOriginalData();
            }
        }

    }

    public void EditConditionTreeVars()
    {
        GUILayout.Space(20);
        GUILayout.Label("TREE CONDITIONS:", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();





        if (GUILayout.Button("Tree")) // Se puede cambiar a imagenes.
        {
            showTree = true;
            showTemp = false;
            showWater = false;
            showSoil = false;
            showWind = false;
        }
        if (GUILayout.Button("Temperature"))
        {
            showTree = false;
            showTemp = true;
            showWater = false;
            showSoil = false;
            showWind = false;
        }
        if (GUILayout.Button("Water"))
        {
            showTree = false;
            showTemp = false;
            showWater = true;
            showSoil = false;
            showWind = false;
        }
        if (GUILayout.Button("Soil"))
        {
            showTree = false;
            showTemp = false;
            showWater = false;
            showSoil = true;
            showWind = false;
        }
        if (GUILayout.Button("Wind"))
        {
            showTree = false;
            showTemp = false;
            showWater = false;
            showSoil = false;
            showWind = true;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        if (showTree)
            ShowTreeConditions();

        if (showTemp)
            ShowTemperatureConditions();

        if (showWater)
            ShowWaterConditions();

        if (showSoil)
            ShowSoilConditions();

        if (showWind)
            ShowWindConditions();

        if (GUILayout.Button("Save"))
            myTree.GetComponent<ConditionCore>().FUpdate();

        //TODO mover de sitio en el update de condition core
        myTree.GetComponent<ConditionCore>().GettingReadyToUpdate();
        GUILayout.Space(10);
        planeToSpawn = EditorGUILayout.ObjectField(new GUIContent("Plane:", "Plane where the trees will spawn."), planeToSpawn, typeof(GameObject), true) as GameObject;
        numberOfTrees = EditorGUILayout.IntField(new GUIContent("Number of trees:", "Number of trees to spawn in the plane"), numberOfTrees);

        if (GUILayout.Button("Spawn Trees in Plane"))
        {
            SpawnTrees();
        }
    }
    private void SpawnTrees()
    {
        if (planeToSpawn != null)
        {
            for (int i = 0; i < numberOfTrees; i++)
            {
                SpawnTree();
            }
        }
    }

    private void SpawnTree()
    {
        GameObject holder = Instantiate(myTree, Vector3.one, Quaternion.identity);

        TreeData reference = Instantiate(myTree.GetComponent<Tree>().data as TreeData);

        Mesh referenceMesh = Instantiate(myTree.GetComponent<MeshFilter>().sharedMesh);

        holder.GetComponent<MeshFilter>().sharedMesh = referenceMesh;

        holder.GetComponent<Tree>().data = reference;

        reference.mesh = holder.GetComponent<MeshFilter>().sharedMesh;

        holder.GetComponent<ConditionCore>().GetInfo();

    }
    public void Corrections()
    {
        if (myTreeConditions.optimNumberLeafs < 0)
            myTreeConditions.optimNumberLeafs = 0;

        if (myTreeConditions.unOptimNumberLeafs < 0)
            myTreeConditions.unOptimNumberLeafs = 0;
    }


    private void ShowTreeConditions()
    {
        GUILayout.Label("Trunk and branches", EditorStyles.boldLabel);
        GUILayout.Space(5);
        myTreeConditions.optimHeight = EditorGUILayout.Slider(new GUIContent("Optimum Height:", "The peak height that the tree gets in the best conditions"), myTreeConditions.optimHeight, 0.001f, 50.0f);
        myTreeConditions.unOptimHeight = EditorGUILayout.Slider(new GUIContent("Apalling Height:", "The worst height that the tree gets in the worst conditions"), myTreeConditions.unOptimHeight, 0.001f, 50.0f);
        myTreeConditions.standartHeight = EditorGUILayout.Slider(new GUIContent("Standart Height:", "The standart height of the tree"), myTreeConditions.standartHeight, 0.001f, 50.0f);
        GUILayout.Space(5);
        myTreeConditions.optimRadius = EditorGUILayout.Slider(new GUIContent("Optimum Radius:", "The peak radius that the tree gets in the best conditions"), myTreeConditions.optimRadius, 0.001f, 5.0f);
        myTreeConditions.unOptimRadius = EditorGUILayout.Slider(new GUIContent("Apalling Radius:", "The worst radius that the tree gets in the worst conditions"), myTreeConditions.unOptimRadius, 0.001f, 5.0f);
        myTreeConditions.standartRadius = EditorGUILayout.Slider(new GUIContent("Standart Radius:", "The standart radius of the tree"), myTreeConditions.standartRadius, 0.001f, 5.0f);
        GUILayout.Space(5);
        myTreeConditions.optimWoodColor = EditorGUILayout.ColorField(new GUIContent("Optimum wood color:", "In the perfect conditions which tonality is the color of the wood?"), myTreeConditions.optimWoodColor);
        myTreeConditions.unOptimWoodColor = EditorGUILayout.ColorField(new GUIContent("Apalling wood color:", "In the worst conditions which tonality is the color of the wood?"), myTreeConditions.unOptimWoodColor);
        Color helper = EditorGUILayout.ColorField(new GUIContent("Final Wood Color:", "This is the color that the wood of tree will get if saved, you can't edit it."), myTreeConditions.finalWColor);
        GUILayout.Space(10);
        GUILayout.Label("Leafs", EditorStyles.boldLabel);
        GUILayout.Space(5);
        myTreeConditions.optimLeafSize = EditorGUILayout.Slider(new GUIContent("Optimum Leaf Size (Multiple):", "This Number is a multiplier, the standart leaf size is the original size, and this will multiply the size of the original"), myTreeConditions.optimLeafSize, 0.001f, 2f);
        myTreeConditions.unOptimLeafSize = EditorGUILayout.Slider(new GUIContent("Apalling Leaf Size (Multiple):", "This Number is a multiplier, the standart leaf size is the original size, and this will multiply the size of the original"), myTreeConditions.unOptimLeafSize, 0.001f, 2f);
        //myTreeConditions.standartLeafSize = EditorGUILayout.Slider(new GUIContent("Standart Leaf Size:", "The standart Leaf Size of the tree"), myTreeConditions.standartLeafSize, 0.001f, 2f);
        GUILayout.Space(5);
        GUILayout.Label(new GUIContent("Original Number: " + numberOfOriginalLeafs, "The number of leafs that had the original tree, its recomended to put more leafs than this number."));
        myTreeConditions.optimNumberLeafs = EditorGUILayout.IntField(new GUIContent("Optimum Number of Leafs:", "The peak Number of Leafs that the tree gets in the best conditions"), myTreeConditions.optimNumberLeafs);
        myTreeConditions.unOptimNumberLeafs = EditorGUILayout.IntField(new GUIContent("Apalling Number of Leafs:", "The worst Number of Leafs that the tree gets in the worst conditions"), myTreeConditions.unOptimNumberLeafs);
        GUILayout.Space(5);
        myTreeConditions.optimLeafColor = EditorGUILayout.ColorField(new GUIContent("Optimum leaf color:", "In the perfect conditions which tonality is the color of the leaf?"), myTreeConditions.optimLeafColor);
        myTreeConditions.unOptimLeafColor = EditorGUILayout.ColorField(new GUIContent("Apalling leaf color:", "In the worst conditions which tonality is the color of the leaf?"), myTreeConditions.unOptimLeafColor);
        Color helper1 = EditorGUILayout.ColorField(new GUIContent("Final Leaf Color:", "This is the color that the leafs of tree will get if saved, you can't edit it."), myTreeConditions.finalLColor);
        GUILayout.Space(5);
    }

    private void ShowTemperatureConditions()
    {
        myTreeConditions.myTemp.ShowTool();
    }

    private void ShowWaterConditions()
    {
        myTreeConditions.myWater.ShowTool();
    }

    private void ShowSoilConditions()
    {
        myTreeConditions.mySoil.ShowTool();
    }

    private void ShowWindConditions()
    {
        myTreeConditions.myWind.ShowTool();
    }



}


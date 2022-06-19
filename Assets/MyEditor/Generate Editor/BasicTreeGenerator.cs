using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TreeEditor;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;

public class BasicTreeGenerator : EditorWindow
{
    bool editingTree = false;
    // window vars
    bool treeWindow = true;
    bool rootWindow = true;

    bool rootSelected = false;
    // Core Variables
    Tree tree;
    TreeData advancedData;


    bool changed = false;
    List<BasicMainTrunk> myMainGroups = new List<BasicMainTrunk>();
    List<BasicBranches> myBranchesGroups = new List<BasicBranches>();
    List<BasicLeafs> myLeafsGroups = new List<BasicLeafs>();



    // Root Vars
    int seed = 0;
    float areaSpreed = 1.0f;
    float groundOffset = 0.0f;
    float LODoverall = 0.8f;


    [MenuItem("Window/Tree Procedural Generation/Basic Tree Generator")]
    private static void ShowWindow()
    {
        var window = GetWindow<BasicTreeGenerator>();
        window.titleContent = new GUIContent("Tree Base");
        window.Show();

    }

    private void OnGUI()
    {
        var editing = GUI.changed;
        treeWindow = EditorGUILayout.Foldout(treeWindow, "Select a Base");
        if (treeWindow)
        {
            ShowTreeBasic();
        }
        if (rootSelected)
        {
            rootWindow = EditorGUILayout.Foldout(rootWindow, "Root Basics");

            if (rootWindow)
            {
                ShowRootBasics();
            }

            if (GUILayout.Button("AddTrunk"))
            {
                AddMainTrunk();
            }
             if (GUILayout.Button("Save Mesh"))
            {
                UpdateTree();
            }
        }
        editingTree = true;
        if (editing == GUI.changed)
        {
            editingTree = false;
            if (rootSelected)
                UpdateFromOriginal();
        }

        if (editingTree)
        {
            if (rootSelected && rootWindow && this.hasFocus)
            {
                ChangeRoot();
                if (changed)
                    PreviewTree();
            }
            changed = false;
        }
    }

    private void ChangeRoot()
    {


        // tambien ha de haver una funcion que lo haga a la inversa TODO
        if (seed != advancedData.root.seed || areaSpreed != advancedData.root.rootSpread || advancedData.root.groundOffset != groundOffset || advancedData.root.adaptiveLODQuality != LODoverall)
        {
            advancedData.root.seed = seed;
            advancedData.root.rootSpread = areaSpreed;
            advancedData.root.groundOffset = groundOffset;
            advancedData.root.adaptiveLODQuality = LODoverall;


            changed = true;
            advancedData.UpdateSeed(seed);
        }

    }

    private void ShowTreeBasic()
    {
        var holder = GUI.changed;
        // In this function we want the developer to put first the tree base
        tree = EditorGUILayout.ObjectField("tree object", tree, typeof(Tree), true) as Tree;
        if (tree != null)
        {
            rootSelected = true;

        }
        else
        {
            rootSelected = false;
        }

        if (holder != GUI.changed)
        {
            CreateOurTree();
        }
    }

    private void ShowRootBasics()
    {
        // Basics of the Root of a Tree
        seed = EditorGUILayout.IntSlider("Seed", seed, 1, 999999);
        areaSpreed = EditorGUILayout.Slider("Area Spread", areaSpreed, 0, 10);
        groundOffset = EditorGUILayout.Slider("Ground OffSet", groundOffset, 0, 10);
        LODoverall = EditorGUILayout.Slider("LOD quality Overall", LODoverall, 0, 1);
    }

    private void AddMainTrunk()
    {

        TreeGroup holder = advancedData.AddGroup(advancedData.root, typeof(TreeGroupBranch));
        myMainGroups.Add(new BasicMainTrunk());
        myMainGroups[myMainGroups.Count - 1].ShowWindow();
        myMainGroups[myMainGroups.Count - 1].GenerateData(advancedData, holder as TreeGroupBranch, this);
        UpdateTree();
    }


    private void AssignMaterials(Renderer renderer, Material[] materials)
    {
        if (renderer != null)
        {
            if (materials == null)
                materials = new Material[0];

            renderer.sharedMaterials = materials;
        }
    }


    public void UpdateTree()
    { 
        Material[] materials;
        advancedData.UpdateMesh(tree.transform.worldToLocalMatrix, out materials);
        AssignMaterials(tree.GetComponent<Renderer>(), materials);
    }

    public void PreviewTree()
    {
        Material[] materials;
        advancedData.PreviewMesh(tree.transform.worldToLocalMatrix, out materials);
        AssignMaterials(tree.GetComponent<Renderer>(), materials);
    }


    public void CreateOurTree()
    {
        advancedData = tree.data as TreeData;
        seed = advancedData.root.seed;
        areaSpreed = advancedData.root.rootSpread;
        groundOffset = advancedData.root.groundOffset;
        LODoverall = advancedData.root.adaptiveLODQuality;

        // Creating Trunks and branches
        for (int i = 0; i < advancedData.branchGroups.Length; i++)
        {
            if (advancedData.branchGroups[i].parentGroupID == advancedData.root.uniqueID)
            {
                myMainGroups.Add(new BasicMainTrunk());
                myMainGroups[myMainGroups.Count - 1].ShowWindow();
                myMainGroups[myMainGroups.Count - 1].GenerateData(advancedData, advancedData.branchGroups[i] as TreeGroupBranch, this);
                myMainGroups[myMainGroups.Count - 1].CreateChilds();
            }
        }

        UpdateTree();
    }

    private void UpdateFromOriginal()
    {
        seed = advancedData.root.seed;
        areaSpreed = advancedData.root.rootSpread;
        groundOffset = advancedData.root.groundOffset;
        LODoverall = advancedData.root.adaptiveLODQuality;
    }

    public void CreateBranches(TreeGroup parent)
    {
        TreeGroup holder = advancedData.AddGroup(parent, typeof(TreeGroupBranch));
        myBranchesGroups.Add(new BasicBranches());
        myBranchesGroups[myBranchesGroups.Count - 1].ShowWindow();
        myBranchesGroups[myBranchesGroups.Count - 1].GenerateData(advancedData, holder as TreeGroupBranch, this);
        UpdateTree();
    }

    public void CreateOurBranches(TreeGroupBranch myBranch)
    {
        myBranchesGroups.Add(new BasicBranches());
        myBranchesGroups[myBranchesGroups.Count - 1].ShowWindow();
        myBranchesGroups[myBranchesGroups.Count - 1].GenerateData(advancedData, myBranch as TreeGroupBranch, this);
        myBranchesGroups[myBranchesGroups.Count - 1].CreateChilds();

    }

    public void CreateLeafs(TreeGroup parent)
    {
        TreeGroup holder = advancedData.AddGroup(parent, typeof(TreeGroupLeaf));
        myLeafsGroups.Add(new BasicLeafs());
        myLeafsGroups[myLeafsGroups.Count - 1].ShowWindow();
        myLeafsGroups[myLeafsGroups.Count - 1].GenerateData(advancedData, holder as TreeGroupLeaf, this);
        UpdateTree();
    }

    public void CreateOurLeafs(TreeGroupLeaf myLeaf)
    {
        myLeafsGroups.Add(new BasicLeafs());
        myLeafsGroups[myLeafsGroups.Count - 1].ShowWindow();
        myLeafsGroups[myLeafsGroups.Count - 1].GenerateData(advancedData, myLeaf as TreeGroupLeaf, this);
    }

    public void DeleteGroup(TreeGroup toDelete)
    {
        for (int i = 0; i < advancedData.branchGroups.Length; i++)
        {
            for (int j = 0; j < myMainGroups.Count; j++)
            {
                if (advancedData.branchGroups[i] == myMainGroups[j].myBranch && advancedData.branchGroups[i].parentGroupID == toDelete.uniqueID)
                {
                    myMainGroups[j].DeleteGroup();
                    myMainGroups.Remove(myMainGroups[j]);
                }
            }
            for (int j = 0; j < myBranchesGroups.Count; j++)
            {
                if (advancedData.branchGroups[i] == myBranchesGroups[j].myBranch && advancedData.branchGroups[i].parentGroupID == toDelete.uniqueID)
                {
                    myBranchesGroups[j].DeleteGroup();
                    myBranchesGroups.Remove(myBranchesGroups[j]);
                }
            }

        }
        for (int i = 0; i < advancedData.leafGroups.Length; i++)
        {
            for (int j = 0; j < myLeafsGroups.Count; j++)
            {
                if (advancedData.leafGroups[i] == myLeafsGroups[j].myLeaf && advancedData.leafGroups[i].parentGroupID == toDelete.uniqueID)
                {
                    myLeafsGroups[j].DeleteGroup();
                    myLeafsGroups.Remove(myLeafsGroups[j]);
                }
            }
        }
        advancedData.DeleteGroup(toDelete);
        UpdateTree();
    }

}


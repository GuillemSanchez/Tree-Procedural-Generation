using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TreeEditor;
using UnityEngine.UIElements;

public class BasicTreeGenerator : EditorWindow {

    // window vars
    bool treeWindow = true;
    bool rootWindow = true;

    bool rootSelected = false;
    // Core Variables
    Tree tree;
    TreeData advancedData;
    

    List<BasicMainTrunk> myMainGroups = new List<BasicMainTrunk>();

 
    // Root Vars
    int seed = 0;
    float areaSpreed = 1.0f;
    float groundOffset = 0.0f;
    float LODoverall = 0.8f;

    
    [MenuItem("Tree Procedural Generation/BasicTreeGenerator")]
    private static void ShowWindow() {
        var window = GetWindow<BasicTreeGenerator>();
        window.titleContent = new GUIContent("Tree Base");
        window.Show();

    }

    private void OnGUI() {
        treeWindow = EditorGUILayout.Foldout(treeWindow, "Select a Base");
        if (treeWindow)
        {
            ShowTreeBasic();
        }
        if (rootSelected){
            rootWindow = EditorGUILayout.Foldout(rootWindow, "Root Basics");

            if (rootWindow)
            {
                ShowRootBasics();
            }

            // Button to adding the trunk?
            if (GUILayout.Button("AddTrunk")){
                AddMainTrunk();
            }
        }
    }

    void Update()
    {   

        if (rootSelected && rootWindow){
            ChangeRoot();
            UpdateTree();
        }
            
    }

    private void ChangeRoot() {
        advancedData = tree.data as TreeData;


        
        advancedData.root.seed = seed;
        advancedData.root.rootSpread = areaSpreed;
        advancedData.root.groundOffset = groundOffset;
        advancedData.root.adaptiveLODQuality = LODoverall;
        
        
        
        advancedData.UpdateSeed(seed);
    } 

    private void ShowTreeBasic() {

        // In this function we want the developer to put first the tree base
        tree = EditorGUILayout.ObjectField("tree object", tree, typeof(Tree), true) as Tree;
        if (tree != null) {
            rootSelected = true;
        }
        else {
            rootSelected = false;
        }
    }

    private void ShowRootBasics() {
        // Basics of the Root of a Tree
        seed = EditorGUILayout.IntSlider("Seed",seed,1,999999);
        areaSpreed = EditorGUILayout.Slider("Area Spread",areaSpreed,0,10);
        groundOffset = EditorGUILayout.Slider("Ground OffSet",groundOffset,0,10);
        LODoverall = EditorGUILayout.Slider("LOD quality Overall", LODoverall, 0, 1);
    }

    private void AddMainTrunk() {
        advancedData = tree.data as TreeData;

        TreeGroup holder = advancedData.AddGroup(advancedData.root, typeof(TreeGroupBranch));
        myMainGroups.Add(new BasicMainTrunk());
        myMainGroups[myMainGroups.Count-1].ShowWindow();
        myMainGroups[myMainGroups.Count-1].GenerateData(advancedData, holder as TreeGroupBranch, this);
    }


    private void AssignMaterials(Renderer renderer, Material[] materials){
        if (renderer != null)
        {
            if (materials == null)
                materials = new Material[0];

            renderer.sharedMaterials = materials;
        }
    }


    public void UpdateTree(){
        Material[] materials;
        advancedData.UpdateMesh(tree.transform.worldToLocalMatrix, out materials);
        AssignMaterials(tree.GetComponent<Renderer>(), materials); 
    }
}


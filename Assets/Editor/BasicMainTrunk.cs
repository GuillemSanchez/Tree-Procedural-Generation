using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TreeEditor;
using UnityEngine.UIElements;

public class BasicMainTrunk : EditorWindow
{   
    BasicTreeGenerator myGenerator;

    int seed = 1234;



    TreeData advancedData;
    TreeGroupBranch myBranch;
    public void ShowWindow() {
        var window = GetWindow<BasicMainTrunk>(typeof(BasicTreeGenerator));
        window.titleContent = new GUIContent("Main Trunk");
        window.Show();
    }

    public void GenerateData(TreeData data, TreeGroupBranch branchData, BasicTreeGenerator gene){
        advancedData = data;
        myBranch = branchData;
        myGenerator = gene;
    }


    private void OnGUI() {
        seed = EditorGUILayout.IntSlider("Seed",seed,1,999999);
    }

     void Update()
    {   
        myBranch.seed = seed;
        myBranch.UpdateSeed();
        myGenerator.UpdateTree();
    }



}

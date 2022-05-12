using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TreeEditor;
using UnityEditor.AnimatedValues;




public class BasicLeafs : EditorWindow
{

    // Vars ---------------------------------------------------

    BasicTreeGenerator myGenerator;
    TreeData advancedData;
    public TreeGroupLeaf myLeaf;


    //Optimize vars
    bool editingTree = false;

    // Core Leaf vars --------------------------------------------
    // 0->999999 Slider
    int seed = 1234;
    // 1->20 Slider
    int frequency = 1;


    //Distribution things 
    // 0.001f <-> 0.999f Slider
    float internalRing = 0.001f;
    // 0.002f <-> 1.000f Slider
    float externalRing = 1.000f;
    //limit private

    float internalRing_ = 0.001f;
    float externalRing_ = 1.000f;

    AnimationCurve distributionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.001f, 1, float.PositiveInfinity, float.PositiveInfinity), new Keyframe(1, 0));

    //Grow paras
    float internalGrowth = 1.000f;
    float externalGrowth = 1.000f;

    float internalGrowth_ = 1.000f;
    float externalGrowth_ = 1.000f;
    float growthtotal = 1.000f;
    AnimationCurve growthCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

    // Angle paras
    float angleGrowth = 0.000f;
    float angleGrowth_ = 0.000f;
    float angleMax = 1.000f;
    AnimationCurve growthAngleCurve = new AnimationCurve(new Keyframe(0, 0.000f));
    // Core Leaf vars --------------------------------------------



    // Shape vars -------------------------------------------
    // Size and aligment
    float minLeafSize = 10.0f;
    float maxLeafSize = 15.0f;
    float perpendicularA = 0.0f;
    float horizontalA = 0.0f;

    Material leafMaterial;
    GameObject leafMesh;

    bool isMesh = false;


    // Initialaze the Animation curves
    private readonly Rect m_CurveRangesA = new Rect(0, 0, 1, 1); // Range 0..1
    private readonly Rect m_CurveRangesB = new Rect(0, -1, 1, 2); // Range -1..1

    // For scroll purpose
    Vector2 scrollPos;

    // Vars ---------------------------------------------------


    public void ShowWindow()
    {
        var desiredType = typeof(EditorWindow);
        var window = EditorWindow.GetWindow<BasicLeafs>("Leafs");
        window.titleContent = new GUIContent("Leafs");
        window.Show();
    }

    // Using this function to initialize the data
    public void GenerateData(TreeData data, TreeGroupLeaf leafData, BasicTreeGenerator gene)
    {
        advancedData = data;
        myLeaf = leafData;
        myGenerator = gene;
    }


    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(600));
        bool can = GUI.changed;
        
        GUILayout.Label("Core Leaf Vars:", EditorStyles.boldLabel); // Cambiar para mas relevancia todo
        seed = EditorGUILayout.IntSlider("Seed", seed, 1, 999999);
        frequency = EditorGUILayout.IntSlider("Quantity of Leafs", frequency, 1, 100);
        isMesh = EditorGUILayout.Toggle("The geometry of the leaf is a Mesh?", isMesh);
        if (!isMesh)
            leafMaterial = EditorGUILayout.ObjectField("Leaf Material", leafMaterial, typeof(Material), false) as Material;
        if (isMesh)
            leafMesh = EditorGUILayout.ObjectField("Leaf Material", leafMesh, typeof(GameObject), false) as GameObject;
        // Distribution UI
        GUILayout.Label("Distribution:", EditorStyles.boldLabel);
        internalRing = EditorGUILayout.Slider("Bottom Ring distribution", internalRing, 0.001f, 0.999f);
        externalRing = EditorGUILayout.Slider("Top Ring distribution", externalRing, 0.002f, 1.000f);
        EditorGUILayout.CurveField(distributionCurve, Color.green, m_CurveRangesA);
        // Growth UI
        GUILayout.Label("Growth:", EditorStyles.boldLabel);
        growthtotal = EditorGUILayout.Slider("Growth general", growthtotal, 0.000f, 1.000f);
        internalGrowth = EditorGUILayout.Slider("Bottom Growth", internalGrowth, 0.000f, 1.000f);
        externalGrowth = EditorGUILayout.Slider("Top Growth", externalGrowth, 0.000f, 1.000f);
        EditorGUILayout.CurveField(growthCurve, Color.green, m_CurveRangesA);
        // Angle UI
        GUILayout.Label("Growth Angle:", EditorStyles.boldLabel);
        angleMax = EditorGUILayout.Slider("How much afects the angle", angleMax, 0.000f, 1.000f);
        angleGrowth = EditorGUILayout.Slider("Angle of growth", angleGrowth, -1.000f, 1.000f);
        EditorGUILayout.CurveField(growthAngleCurve, Color.green, m_CurveRangesB);
        // Shape UI
        GUILayout.Label("Shape", EditorStyles.boldLabel); // Cambiar para mas relevancia todo
        // Lenght UI
        GUILayout.Label("Size:", EditorStyles.boldLabel);
        maxLeafSize = EditorGUILayout.Slider("Max leaf size", maxLeafSize, 0.1f, 4.9f);
        minLeafSize = EditorGUILayout.Slider("Min leaf size", minLeafSize, 0.2f, 5f);
        // Aligment UI
        horizontalA = EditorGUILayout.Slider("Horizontal align", horizontalA, 0.1f, 1f);
        perpendicularA = EditorGUILayout.Slider("Perpendicular align", perpendicularA, 0.1f, 1f);

        if (GUILayout.Button("Delete this Group"))
        {
            DeleteGroup();
        }

        editingTree = false;
        if (GUI.changed != can)
        {
            editingTree = true;
        }
        EditorGUILayout.EndScrollView();

        if (editingTree)
        {
            // Updating Everypart of the main trunk 
            UpdateFrequencyandSeed();
            UpdateDistribution();
            UpdateGrowth();
            UpdateAngleofGrowth();
            UpdateSizeAndAlign();
            advancedData.UpdateFrequency(myLeaf.uniqueID);
            myLeaf.UpdateSeed();
            myGenerator.UpdateTree();
            UpdateLatest();
        }
        else
        {
            UpdateFromOriginal();
        }
    }

    void Update()
    {
        //Mirar si la pestaña seleccionada es esta
        
    }

    private void UpdateDistribution()
    {
        // No puede el external ring ser más pequeño que el internal ring 
        // No puede el internal ring ser más grande que el external ring 
        if (externalRing <= internalRing)
        {
            internalRing = externalRing - 0.001f;
        }
        if (internalRing >= externalRing)
        {
            externalRing = internalRing + 0.001f;
        }

        if (externalRing != externalRing_ || internalRing != internalRing_)
            distributionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(internalRing, 1, float.PositiveInfinity, float.PositiveInfinity), new Keyframe(externalRing, 0));

        myLeaf.distributionCurve = distributionCurve;
    }

    private void UpdateGrowth()
    {
        // This to points are the inner and the external circles of the growth
        if (internalGrowth != internalGrowth_ || externalGrowth != externalGrowth_)
            growthCurve = new AnimationCurve(new Keyframe(0, internalGrowth), new Keyframe(1, externalGrowth));

        myLeaf.distributionScaleCurve = growthCurve;
        myLeaf.distributionScale = growthtotal;
    }

    private void UpdateAngleofGrowth()
    {
        if (angleGrowth != angleGrowth_)
            growthAngleCurve = new AnimationCurve(new Keyframe(0.500f, angleGrowth));

        myLeaf.distributionPitch = angleMax;
        myLeaf.distributionPitchCurve = growthAngleCurve;

    }

    private void UpdateFrequencyandSeed()
    {
        myLeaf.seed = seed;
        myLeaf.distributionFrequency = frequency;

        //myLeaf.
        if (!isMesh)
        {
            myLeaf.materialLeaf = leafMaterial;
            myLeaf.geometryMode = 0;
        }
        else
        {
            myLeaf.instanceMesh = leafMesh;
            myLeaf.geometryMode = 4;
        }

    }

    private void UpdateSizeAndAlign()
    {

        if (maxLeafSize <= minLeafSize)
        {
            maxLeafSize = minLeafSize + 0.1f;
        }

        myLeaf.size = new Vector2(minLeafSize, maxLeafSize);

        myLeaf.horizontalAlign = horizontalA;
        myLeaf.perpendicularAlign = perpendicularA;
    }



    private void UpdateFromOriginal()
    {
        distributionCurve = myLeaf.distributionCurve;
        growthCurve = myLeaf.distributionScaleCurve;
        growthtotal = myLeaf.distributionScale;
        angleMax = myLeaf.distributionPitch;
        growthAngleCurve = myLeaf.distributionPitchCurve;
        seed = myLeaf.seed;
        frequency = myLeaf.distributionFrequency;
        minLeafSize = myLeaf.size.x;
        maxLeafSize = myLeaf.size.y;
        horizontalA = myLeaf.horizontalAlign;
        perpendicularA = myLeaf.perpendicularAlign;
        if (myLeaf.geometryMode == 0)
        {
            leafMaterial = myLeaf.materialLeaf;
            isMesh = false;
        }
        if (myLeaf.geometryMode == 4)
        {
            leafMesh = myLeaf.instanceMesh;
            isMesh = true;
        }


    }

    private void UpdateLatest()
    {

        angleGrowth_ = angleGrowth;
        externalRing_ = externalRing;
        internalRing_ = internalRing;
        externalGrowth_ = externalGrowth;
        internalGrowth_ = internalGrowth;
    }

    public void DeleteGroup()
    {
        myGenerator.DeleteGroup(myLeaf);
        this.Close();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TreeEditor;
using UnityEditor.AnimatedValues;


public class BasicMainTrunk : EditorWindow
{
    BasicTreeGenerator myGenerator;
    TreeData advancedData;
    public TreeGroupBranch myBranch;


    //Optimize vars
    bool editingTree = false;

    // Core Trunk vars --------------------------------------------
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
    // Core Trunk vars --------------------------------------------


    // Shape Trunk vars -------------------------------------------
    // Lenght paras
    float minLenght = 10.0f;
    float maxLenght = 15.0f;


    // Radius paras
    // 0.1f -> 5.0f
    bool relativeLength = true;
    float radiusValue = 0.5f;
    float bottomRadius = 1.0f;
    float topRadius = 0.0f;

    float bottomRadius_ = 1.0f;
    float topRadius_ = 0.0f;
    float capSmoothing = 0.0f;

    AnimationCurve radiusCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));

    // Crinkliness and seek
    float crinklinessValue = 0.1f;
    float crinklinessTop = 1.0f;
    float crinklinessBottom = 1.0f;
    float crinklinessTop_ = 1.0f;
    float crinklinessBottom_ = 1.0f;
    float seekValue = 0.1f;
    float seekTop = 0.0f;
    float seekbottom = 0.0f;
    float seekTop_ = 0.0f;
    float seekbottom_ = 0.0f;

    // Flare or Weld

    float FWradius = 0.0f;
    float FWheight = 0.1f;
    float FWnoise = 0.3f;

    Material branchMaterial;
    bool isFrond = false;


    // Fronds Only Vars
    Material frondMaterial;
    // 1 - 10
    int frondCount = 1;
    // 1 - 10
    float frondsWidth = 0.2f;
    AnimationCurve frondsCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
    float startFronds = 0.0f;
    float endFronds = 1.0f;
    float frondsRotation = 0.0f;


    AnimationCurve crinklinessCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
    AnimationCurve seekCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
    // Shape Trunk vars -------------------------------------------

    // Initialaze the Animation curves
    private readonly Rect m_CurveRangesA = new Rect(0, 0, 1, 1); // Range 0..1
    private readonly Rect m_CurveRangesB = new Rect(0, -1, 1, 2); // Range -1..1

    // For scroll purpose
    Vector2 scrollPos;

    public void ShowWindow()
    {
        var desiredType = typeof(EditorWindow);
        var window = EditorWindow.GetWindow<BasicMainTrunk>("Main Trunk", true, desiredType);
        window.titleContent = new GUIContent("Main Trunk");
        window.Show();
    }

    // Using this function to initialize the data
    public void GenerateData(TreeData data, TreeGroupBranch branchData, BasicTreeGenerator gene)
    {
        advancedData = data;
        myBranch = branchData;
        myGenerator = gene;
    }


    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(600));
        bool can = GUI.changed;

        GUILayout.Label("Core Main Trunk:", EditorStyles.boldLabel); // Cambiar para mas relevancia todo
        seed = EditorGUILayout.IntSlider("Trunk seed", seed, 1, 999999);
        frequency = EditorGUILayout.IntSlider("Quantity of trunks", frequency, 1, 100);
        isFrond = EditorGUILayout.Toggle("Is a frond?", isFrond);
        branchMaterial = EditorGUILayout.ObjectField("Branch Material", branchMaterial, typeof(Material), false) as Material;
        if (isFrond)
            frondMaterial = EditorGUILayout.ObjectField("Frond (leafs) Material", frondMaterial, typeof(Material), false) as Material;

        // Distribution UI
        GUILayout.Label("Distribution:", EditorStyles.boldLabel);
        internalRing = EditorGUILayout.Slider("Internal Ring distribution", internalRing, 0.001f, 0.999f);
        externalRing = EditorGUILayout.Slider("External Ring distribution", externalRing, 0.002f, 1.000f);
        EditorGUILayout.CurveField(distributionCurve, Color.green, m_CurveRangesA);
        // Growth UI
        GUILayout.Label("Growth:", EditorStyles.boldLabel);
        growthtotal = EditorGUILayout.Slider("Growth general", growthtotal, 0.000f, 1.000f);
        internalGrowth = EditorGUILayout.Slider("Internal Growth", internalGrowth, 0.000f, 1.000f);
        externalGrowth = EditorGUILayout.Slider("External Growth", externalGrowth, 0.000f, 1.000f);
        EditorGUILayout.CurveField(growthCurve, Color.green, m_CurveRangesA);
        // Angle UI
        GUILayout.Label("Growth Angle:", EditorStyles.boldLabel);
        angleMax = EditorGUILayout.Slider("How much afects the angle", angleMax, 0.000f, 1.000f);
        angleGrowth = EditorGUILayout.Slider("Angle of growth", angleGrowth, -1.000f, 1.000f);
        EditorGUILayout.CurveField(growthAngleCurve, Color.green, m_CurveRangesB);
        // Shape UI
        GUILayout.Label("Shape", EditorStyles.boldLabel); // Cambiar para mas relevancia todo
        // Lenght UI
        GUILayout.Label("Lenght:", EditorStyles.boldLabel);
        maxLenght = EditorGUILayout.Slider("Max lenght", maxLenght, 0.1f, 49.9f);
        minLenght = EditorGUILayout.Slider("Min lenght", minLenght, 0.2f, 50.0f);
        // Radius UI
        GUILayout.Label("Radius:", EditorStyles.boldLabel);
        relativeLength = EditorGUILayout.Toggle("Relative Radius to Length", relativeLength);
        radiusValue = EditorGUILayout.Slider("Radius", radiusValue, 0.1f, 5.000f);
        topRadius = EditorGUILayout.Slider("Top of the Trunk", topRadius, 0.100f, 1.000f);
        bottomRadius = EditorGUILayout.Slider("Bottom of the Trunk", bottomRadius, 0.100f, 1.000f);
        capSmoothing = EditorGUILayout.Slider("Top smoothed", capSmoothing, 0.000f, 1.000f);
        EditorGUILayout.CurveField(radiusCurve, Color.green, m_CurveRangesA);
        // Misc UI
        GUILayout.Label("Misc:", EditorStyles.boldLabel);
        crinklinessValue = EditorGUILayout.Slider("Crinkliness", crinklinessValue, 0.0f, 1.000f);
        crinklinessTop = EditorGUILayout.Slider("Top of the trunk", crinklinessTop, 0.0f, 1.000f);
        crinklinessBottom = EditorGUILayout.Slider("Bottom of the trunk", crinklinessBottom, 0.0f, 1.000f);
        crinklinessCurve = EditorGUILayout.CurveField(crinklinessCurve, Color.green, m_CurveRangesA);
        seekValue = EditorGUILayout.Slider("Seek Sun", seekValue, 0.0f, 1.000f);
        seekTop = EditorGUILayout.Slider("Top of the trunk", seekTop, -1.0f, 1.000f);
        seekbottom = EditorGUILayout.Slider("bottom of the trunk", seekbottom, -1.0f, 1.000f);
        seekCurve = EditorGUILayout.CurveField(seekCurve, Color.green, m_CurveRangesB);
        FWradius = EditorGUILayout.Slider("Root Deformation Radius", FWradius, 0.0f, 5.000f);
        FWheight = EditorGUILayout.Slider("Root Deformation Height", FWheight, 0.0f, 1.000f);
        FWnoise = EditorGUILayout.Slider("Root Deformation Noise", FWnoise, 0.0f, 1.000f);
        if (isFrond)
        {
            GUILayout.Label("Fronds:", EditorStyles.boldLabel);
            frondCount = EditorGUILayout.IntSlider("Frond Count", frondCount, 1, 10);
            frondsWidth = EditorGUILayout.Slider("Frond Width", frondsWidth, 0.1f, 10.0f);
            EditorGUILayout.CurveField(frondsCurve, Color.green, m_CurveRangesA);
            startFronds = EditorGUILayout.Slider("Where do the fronds start", startFronds, 0.0f, 0.999f);
            endFronds = EditorGUILayout.Slider("Where do the fronds end", endFronds, 0.001f, 1f);
            frondsRotation = EditorGUILayout.Slider("Rotation of the fronds", frondsRotation, 0.0f, 1f);
        }

        if (GUILayout.Button("Add Branches"))
        {
            CreateBranches();
        }

        if (GUILayout.Button("Add Leafs"))
        {
            CreateLeafs();
        }

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
            UpdateLenght();
            UpdateRadius();
            UpdateMisc();
            if (isFrond)
                UpdateFronds();
            advancedData.UpdateFrequency(myBranch.uniqueID);
            myBranch.UpdateSeed();
            myGenerator.PreviewTree();
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

        myBranch.distributionCurve = distributionCurve;
    }

    private void UpdateGrowth()
    {
        // This to points are the inner and the external circles of the growth
        if (internalGrowth != internalGrowth_ || externalGrowth != externalGrowth_)
            growthCurve = new AnimationCurve(new Keyframe(0, internalGrowth), new Keyframe(1, externalGrowth));

        myBranch.distributionScaleCurve = growthCurve;
        myBranch.distributionScale = growthtotal;
    }

    private void UpdateAngleofGrowth()
    {
        if (angleGrowth != angleGrowth_)
            growthAngleCurve = new AnimationCurve(new Keyframe(0.500f, angleGrowth));

        myBranch.distributionPitch = angleMax;
        myBranch.distributionPitchCurve = growthAngleCurve;

    }

    private void UpdateFrequencyandSeed()
    {
        myBranch.seed = seed;
        myBranch.distributionFrequency = frequency;
        myBranch.materialBranch = branchMaterial;
        if (isFrond)
        {
            myBranch.geometryMode = TreeGroupBranch.GeometryMode.BranchFrond;
            myBranch.materialFrond = frondMaterial;
        }
        else
        {
            myBranch.geometryMode = TreeGroupBranch.GeometryMode.Branch;
        }
    }

    private void UpdateLenght()
    {

        if (maxLenght <= minLenght)
        {
            maxLenght = minLenght + 0.1f;
        }

        myBranch.height = new Vector2(minLenght, maxLenght);
    }

    private void UpdateRadius()
    {
        myBranch.radius = radiusValue;
        myBranch.capSmoothing = capSmoothing;
        myBranch.radiusMode = relativeLength;

        if (bottomRadius != bottomRadius_ || topRadius != topRadius_)
            radiusCurve = new AnimationCurve(new Keyframe(0, bottomRadius), new Keyframe(1, topRadius));

        myBranch.radiusCurve = radiusCurve;
    }

    private void UpdateMisc()
    {
        // Crinklyness
        myBranch.crinklyness = crinklinessValue;

        if (crinklinessBottom != crinklinessBottom_ || crinklinessTop != crinklinessTop_)
            crinklinessCurve = new AnimationCurve(new Keyframe(0, crinklinessBottom), new Keyframe(1, crinklinessTop));

        myBranch.crinkCurve = crinklinessCurve;

        // Seek
        myBranch.seekBlend = seekValue;
        if (seekbottom != seekbottom_ || seekTop != seekTop_)
            seekCurve = new AnimationCurve(new Keyframe(0, seekbottom), new Keyframe(1, seekTop));

        myBranch.seekCurve = seekCurve;

        // Flare/Weld
        myBranch.flareSize = FWradius;
        myBranch.flareNoise = FWnoise;
        myBranch.flareHeight = FWheight;
    }

    private void UpdateFronds()
    {
        myBranch.frondCount = frondCount;
        myBranch.frondWidth = frondsWidth;
        myBranch.frondCurve = frondsCurve;
        if (startFronds >= endFronds)
        {
            endFronds = startFronds + 0.001f;
        }
        myBranch.frondRange = new Vector2(startFronds, endFronds);
        myBranch.frondRotation = frondsRotation;
    }


    private void UpdateFromOriginal()
    {
        distributionCurve = myBranch.distributionCurve;
        growthCurve = myBranch.distributionScaleCurve;
        growthtotal = myBranch.distributionScale;
        angleMax = myBranch.distributionPitch;
        growthAngleCurve = myBranch.distributionPitchCurve;
        seed = myBranch.seed;
        frequency = myBranch.distributionFrequency;
        minLenght = myBranch.height.x;
        maxLenght = myBranch.height.y;
        radiusValue = myBranch.radius;
        capSmoothing = myBranch.capSmoothing;
        relativeLength = myBranch.radiusMode;
        radiusCurve = myBranch.radiusCurve;
        crinklinessValue = myBranch.crinklyness;
        crinklinessCurve = myBranch.crinkCurve;
        seekValue = myBranch.seekBlend;
        seekCurve = myBranch.seekCurve;
        FWradius = myBranch.flareSize;
        FWnoise = myBranch.flareNoise;
        FWheight = myBranch.flareHeight;
        branchMaterial = myBranch.materialBranch;
        frondMaterial = myBranch.materialFrond;
        if (myBranch.geometryMode == TreeGroupBranch.GeometryMode.Branch)
            isFrond = false;
        if (myBranch.geometryMode == TreeGroupBranch.GeometryMode.BranchFrond)
            isFrond = true;

        if (isFrond)
        {
            frondCount = myBranch.frondCount;
            frondsWidth = myBranch.frondWidth;
            frondsCurve = myBranch.frondCurve;
            startFronds = myBranch.frondRange.x;
            endFronds = myBranch.frondRange.y;
            frondsRotation = myBranch.frondRotation;
        }
    }

    public void CreateChilds()
    {

        for (int i = 0; i < advancedData.branchGroups.Length; i++)
        {
            if (advancedData.branchGroups[i].parentGroupID == myBranch.uniqueID)
            {
                myGenerator.CreateOurBranches(advancedData.branchGroups[i]);
            }
        }

        for (int i = 0; i < advancedData.leafGroups.Length; i++)
        {
            if (advancedData.leafGroups[i].parentGroupID == myBranch.uniqueID)
            {
                myGenerator.CreateOurLeafs(advancedData.leafGroups[i]);
            }
        }

    }



    public void CreateBranches()
    {
        myGenerator.CreateBranches(myBranch);
    }

    public void CreateLeafs()
    {
        myGenerator.CreateLeafs(myBranch);
    }

    private void UpdateLatest()
    {
        seekTop_ = seekTop;
        topRadius_ = topRadius;
        seekbottom_ = seekbottom;
        angleGrowth_ = angleGrowth;
        bottomRadius_ = bottomRadius;
        externalRing_ = externalRing;
        internalRing_ = internalRing;
        crinklinessTop_ = crinklinessTop;
        externalGrowth_ = externalGrowth;
        internalGrowth_ = internalGrowth;
        crinklinessBottom_ = crinklinessBottom;

    }

    public void DeleteGroup()
    {
        myGenerator.DeleteGroup(myBranch);
        this.Close();
    }


}

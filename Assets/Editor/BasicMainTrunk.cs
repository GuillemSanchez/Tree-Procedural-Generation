using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TreeEditor;
using UnityEditor.AnimatedValues;


public class BasicMainTrunk : EditorWindow
{
    BasicTreeGenerator myGenerator;


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
    float lastI = 0.001f;
    float lastE = 1.000f;
    AnimationCurve distributionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.001f, 1, float.PositiveInfinity, float.PositiveInfinity), new Keyframe(1, 0));

    //Grow things
    float internalGrowth = 1.000f;
    float lastIG = 1.000f;
    float externalGrowth = 1.000f;
    float lastEG = 1.000f;
    AnimationCurve growthCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

    // Angle
    float angleGrowth = 0.000f;
    float lastAG = 0.000f;
    float angleMax = 1.000f;
    AnimationCurve growthAngleCurve = new AnimationCurve(new Keyframe(0, 0.500f));
    // Core Trunk vars --------------------------------------------


    private readonly Rect m_CurveRangesA = new Rect(0, 0, 1, 1); // Range 0..1
    private readonly Rect m_CurveRangesB = new Rect(0, -1, 1, 2); // Range -1..1



    TreeData advancedData;
    TreeGroupBranch myBranch;
    public void ShowWindow()
    {
        var desiredType = typeof(EditorWindow);
        var window = EditorWindow.GetWindow<BasicMainTrunk>("Main Trunk", true, desiredType);
        window.titleContent = new GUIContent("Main Trunk");
        window.Show();
    }

    public void GenerateData(TreeData data, TreeGroupBranch branchData, BasicTreeGenerator gene)
    {
        advancedData = data;
        myBranch = branchData;
        myGenerator = gene;
    }


    private void OnGUI()
    {
        GUILayout.Label("Core Main Trunk", EditorStyles.boldLabel);
        seed = EditorGUILayout.IntSlider("Trunk seed", seed, 1, 999999);
        frequency = EditorGUILayout.IntSlider("Quantity of trunks", frequency, 1, 20);
        // Distribution UI
        GUILayout.Label("Distribution:", EditorStyles.boldLabel);
        internalRing = EditorGUILayout.Slider("Internal Ring distribution", internalRing, 0.001f, 0.999f);
        externalRing = EditorGUILayout.Slider("External Ring distribution", externalRing, 0.002f, 1.000f);
        EditorGUILayout.CurveField(distributionCurve, Color.green, m_CurveRangesA);
        // Growth UI
        GUILayout.Label("Growth:", EditorStyles.boldLabel);
        internalGrowth = EditorGUILayout.Slider("Internal Growth", internalGrowth, 0.000f, 1.000f);
        externalGrowth = EditorGUILayout.Slider("External Growth", externalGrowth, 0.000f, 1.000f);
        EditorGUILayout.CurveField(growthCurve, Color.green, m_CurveRangesA);
        // Angle UI
        angleMax = EditorGUILayout.Slider("How much afects the angle", angleMax, 0.000f, 1.000f);
        angleGrowth = EditorGUILayout.Slider("Angle of growth", angleGrowth, -1.000f, 1.000f);
        EditorGUILayout.CurveField(growthAngleCurve, Color.green, m_CurveRangesB);
    }

    void Update()
    {
        myBranch.seed = seed;
        myBranch.distributionFrequency = frequency;
        myBranch.UpdateSeed();
        myGenerator.UpdateTree();

        EditDistribution();
        EditGrowth();
        EditAngleofGrowth();
        advancedData.UpdateFrequency(myBranch.uniqueID);

    }

    private void EditDistribution()
    {

        // Si alguno de los valores cambia pasamos a este modo 

        if (externalRing != lastE || internalRing != lastI)
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

            distributionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(internalRing, 1, float.PositiveInfinity, float.PositiveInfinity), new Keyframe(externalRing, 0));

        }
        lastE = externalRing;
        lastI = internalRing;

        // Tambien ha de haber una funcion que lo haga a la inversa. TODO
        myBranch.distributionCurve = distributionCurve;

    }
    private void EditGrowth()
    {
        // Si alguno de los valores cambia pasamos a este modo
        if (externalGrowth != lastEG || internalGrowth != lastIG)
        {
            growthCurve = new AnimationCurve(new Keyframe(0, internalGrowth), new Keyframe(1, externalGrowth));
        }
        lastIG = internalGrowth;
        lastEG = externalGrowth;
        // Tambien ha de haber una funcion que lo haga a la inversa. TODO
        myBranch.distributionScaleCurve = growthCurve;

    }
    private void EditAngleofGrowth()
    {   
        // Si alguno de los valores cambia pasamos a este modo
        if (angleGrowth != lastAG)
        {
            growthAngleCurve = new AnimationCurve(new Keyframe(0.500f, angleGrowth));
        }
        lastAG = angleGrowth;
        // Tambien ha de haber una funcion que lo haga a la inversa. TODO
        myBranch.distributionPitchCurve = growthAngleCurve;
        myBranch.distributionPitch = angleMax;
    }

}

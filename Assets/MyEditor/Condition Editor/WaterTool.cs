using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WaterTool : ConditionTool
{
    bool advancedBool = false;
    float optimValue_;
    float adaptabilityRange_;
    public void ShowTool(float currentVal)
    {
        NotNull();
        GUILayout.Label("Humidity/Rain:", EditorStyles.boldLabel);
        GUILayout.Label("Current Humidity: " + currentVal);
        GUILayout.Space(5);
        GUILayout.Space(5);
        optimValue = EditorGUILayout.FloatField(new GUIContent("Optimum Humidity/Rain (mm/m²)", "The optimum Humidity/Rain of the tree where it grows the best."), optimValue);
        adaptabilityRange = EditorGUILayout.FloatField(new GUIContent("Adaptability (mm/m²)", "How adaptable is the range of the tree to the environment Humidity/Rain."), adaptabilityRange);
        GUILayout.Space(5);
        GUILayout.Label("Adaptability Curve:", EditorStyles.boldLabel);
        GUILayout.Space(5);
        CalculateCurve();
        GUILayout.Space(10);
        advancedBool = EditorGUILayout.Foldout(advancedBool, "Advanced Options");
        if (advancedBool)
        {
            GUILayout.Space(5);
            heightModifier = EditorGUILayout.Slider(new GUIContent("Height Mod:", "How much does this condition affects the height of the tree."), heightModifier, 0.0f, 1.0f);
            radiusModifier = EditorGUILayout.Slider(new GUIContent("Radius Mod:", "How much does this condition affects the Radius of the tree."), radiusModifier, 0.0f, 1.0f);
            leafSizeModifier = EditorGUILayout.Slider(new GUIContent("Leaf Size Mod:", "How much does this condition affects the Leaf Size of the tree."), leafSizeModifier, 0.0f, 1.0f);
            numberOfLeafsModifier = EditorGUILayout.Slider(new GUIContent("N. of Leafs Mod:", "How much does this condition affects the Number of Leafs of the tree."), numberOfLeafsModifier, 0.0f, 1.0f);
            leafColorModifier = EditorGUILayout.Slider(new GUIContent("Leaf Color Mod:", "How much does this condition affects the Leaf Color of the tree."), leafColorModifier, 0.0f, 1.0f);
            woodColorModifier = EditorGUILayout.Slider(new GUIContent("Wood Color Mod:", "How much does this condition affects the Wood Color of the tree."), woodColorModifier, 0.0f, 1.0f);
            GUILayout.Space(5);
            curveBeggining = EditorGUILayout.FloatField(new GUIContent("Curve Init", "In which point the first key frame is in the animation curve."), curveBeggining);
            curveEnd = EditorGUILayout.FloatField(new GUIContent("Curve Lenght", "How much the curve measures in the respective units."), curveEnd);
        }
        UpdateLatest();

    }
    private void CalculateCurve()
    {
        if (adaptabilityRange_ != adaptabilityRange || optimValue != optimValue_)
        {
            float pointA = optimValue - adaptabilityRange;
            float pointB = optimValue + adaptabilityRange;

            if (pointA < curveBeggining)
                pointA = curveBeggining;

            if (pointB > curveEnd)
                pointB = curveEnd + curveBeggining;

            rangeCurve = new AnimationCurve(new Keyframe(curveBeggining, -1), new Keyframe(pointA, -1), new Keyframe(optimValue, 1), new Keyframe(pointB, -1), new Keyframe(curveEnd + curveBeggining, -1));
        }

        curveRect = new Rect(curveBeggining, -1, curveEnd, 2);
        EditorGUILayout.CurveField(rangeCurve, Color.green, curveRect, GUILayout.Height(70));

    }

    private void UpdateLatest()
    {
        optimValue_ = optimValue;
        adaptabilityRange_ = adaptabilityRange;
    }

    private void NotNull()
    {

        if (heightModifier == -1)
        {
            // Designer Choices -----------------------------
            curveBeggining = -200;
            curveEnd = 1000;

            heightModifier = 0.5f;
            radiusModifier = 0.5f;
            leafSizeModifier = 0.8f;
            numberOfLeafsModifier = 0.5f;
            leafColorModifier = 0.8f;
            woodColorModifier = 0.5f;
            // Designer Choices -----------------------------
        }
    }

}

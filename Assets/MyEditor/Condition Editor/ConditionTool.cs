using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class ConditionTool : EditorWindow
{
    public float optimValue = 0;
    public float adaptabilityRange = 0;
    public AnimationCurve rangeCurve;

    public Rect curveRect;

    // Advanced settings ---------------------------------
    public float heightModifier = -1;
    public float radiusModifier;
    public float leafSizeModifier;
    public float numberOfLeafsModifier;
    public float woodColorModifier;
    public float leafColorModifier;


    public float curveBeggining;
    public float curveEnd;
    // Advanced settings ---------------------------------

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;


[CreateAssetMenu(fileName = "New Conditions", menuName = "Procedural Tree")]
public class TreeConditions : ScriptableObject
{
    public TreeData myOriginalData;

    public float optimHeight = -1f;
    public float unOptimHeight = 0;
    public float standartHeight = 0;

    public float optimRadius = 0;
    public float unOptimRadius = 0;
    public float standartRadius = 0;


    public float optimLeafSize = 0;
    public float unOptimLeafSize = 0;
    public float standartLeafSize = 0;

    public Color optimWoodColor;
    public Color unOptimWoodColor;
    public Color finalWColor;

    public Color optimLeafColor;
    public Color unOptimLeafColor;
    public Color finalLColor;

    public int optimNumberLeafs = 0;
    public int unOptimNumberLeafs = 0;

    public TemperatureTool myTemp;
    public WaterTool myWater;
    public WindTool myWind;
    public SoilTool mySoil;

}

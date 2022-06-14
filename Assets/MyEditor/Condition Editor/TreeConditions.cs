using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Conditions", menuName = "Procedural Tree")]
public class TreeConditions : ScriptableObject
{
    public string treeClass;
    public Tree tree;

    public float optimHeight;
    public float unOptimHeight;
    public float standartHeight;

    public float optimRadius;
    public float unOptimRadius;
    public float standartRadius;


    public float optimLeafSize;
    public float unOptimLeafSize;
    public float standartLeafSize;

    public Color optimWoodColor;
    public Color unOptimWoodColor;
    public Color finalWColor;

    public Color optimLeafColor;
    public Color unOptimLeafColor;
    public Color finalLColor;

    public int optimNumberLeafs;
    public int unOptimNumberLeafs;

    public TemperatureTool myTemp;
    public WaterTool myWater;
    public WindTool myWind;
    public SoilTool mySoil;


    public float temp;
    public float water;
    public float wind;
    public float soil;


}

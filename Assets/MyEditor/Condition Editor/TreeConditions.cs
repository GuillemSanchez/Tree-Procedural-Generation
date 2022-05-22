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

    public float optimRadius;
    public float unOptimRadius;

    public float optimLeafSize;
    public float unOptimLeafSize;

    public Color optimWoodColor;
    public Color unOptimWoodColor;

    public Color optimLeafColor;
    public Color unOptimLeafColor;

    public int optimNumberLeafs;
    public int unOptimNumberLeafs;


    // Does it bend like a curve or like and angle?



}

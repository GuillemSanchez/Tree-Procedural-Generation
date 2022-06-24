using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Conditions", menuName = "Procedural Tree")]
public class MapConditions : ScriptableObject
{
    public Texture2D heatMap;
    public Texture2D waterMap;
    public Texture2D soilMap;
    public Texture2D windMap;


    public float heatMapMax = 45;
    public float heatMapMin = -30;
    public float soilMapMax = 80;
    public float soilMapMin = 0;
    public float waterMapMax = 200;
    public float waterMapMin = 50;
    public float windMapMax = 10;
    public float windMapMin = 1;

}

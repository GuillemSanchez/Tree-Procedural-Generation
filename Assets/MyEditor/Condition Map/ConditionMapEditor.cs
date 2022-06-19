using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConditionMapEditor : EditorWindow
{

    public GameObject myPlane;
    private int option = 0;

    public MapConditions myConditions;

    const int BUTTON_TEMP = 0;
    const int BUTTON_WATER = 1;
    const int BUTTON_SOIL = 2;
    const int BUTTON_WIND = 3;



    [MenuItem("Window/Tree Procedural Generation/Condition Map Editor")]
    private static void ShowWindow()
    {
        var window = GetWindow<ConditionMapEditor>();
        window.titleContent = new GUIContent("Condition Map Editor");
        window.Show();
    }

    private void OnGUI()
    {
        GetPlane();
        if (myPlane != null)
        {
            ConditionButtons();
        }
    }

    private void GetPlane()
    {
        bool vChange = GUI.changed;
        myPlane = EditorGUILayout.ObjectField(new GUIContent("Plane:", "Plane where the environment variables will be saved. It must be a Plane"), myPlane, typeof(GameObject), true) as GameObject;

        if (vChange != GUI.changed)
        {
            if (myPlane.GetComponent<MapCore>())
            {
                if (myPlane.GetComponent<MapCore>().mapConditions == null)
                {
                    myPlane.GetComponent<MapCore>().mapConditions = new MapConditions();
                    AssetDatabase.CreateAsset(myPlane.GetComponent<MapCore>().mapConditions, "Assets/" + myPlane.name + "MapConditions.asset");
                }
            }
            else
            {
                myPlane.AddComponent<MapCore>();
                myPlane.GetComponent<MapCore>().mapConditions = new MapConditions();
                AssetDatabase.CreateAsset(myPlane.GetComponent<MapCore>().mapConditions, "Assets/" + myPlane.name + "MapConditions.asset");

            }
            myConditions = myPlane.GetComponent<MapCore>().mapConditions;
        }
    }
    private void ConditionButtons()
    {
        GUILayout.Space(20);
        GUILayout.Label("MAP EDITOR:", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Temperature"))
        {
            option = BUTTON_TEMP;
        }
        if (GUILayout.Button("Water"))
        {
            option = BUTTON_WATER;
        }
        if (GUILayout.Button("Soil"))
        {
            option = BUTTON_SOIL;
        }
        if (GUILayout.Button("Wind"))
        {
            option = BUTTON_WIND;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        bool guicd = GUI.changed;
        switch (option)
        {
            case BUTTON_TEMP:
                myConditions.heatMap = EditorGUILayout.ObjectField(new GUIContent("Heat Map:", "The temperature map."), myConditions.heatMap, typeof(Texture2D), true) as Texture2D;
                myConditions.heatMapMax = EditorGUILayout.FloatField(new GUIContent("Max temperature (C°):", "The max temperature of the Map"), myConditions.heatMapMax);
                myConditions.heatMapMin = EditorGUILayout.FloatField(new GUIContent("Min temperature (C°):", "The min temperature of the Map"), myConditions.heatMapMin);
                break;
            case BUTTON_WATER:
                myConditions.waterMap = EditorGUILayout.ObjectField(new GUIContent("Humidity/Rain Map:", "The Humidity/Rain map."), myConditions.waterMap, typeof(Texture2D), true) as Texture2D;
                myConditions.waterMapMax = EditorGUILayout.FloatField(new GUIContent("Max Humidity/Rain (mm/m²):", "The max Humidity/Rain of the Map"), myConditions.waterMapMax);
                myConditions.waterMapMin = EditorGUILayout.FloatField(new GUIContent("Min Humidity/Rain (mm/m²):", "The min Humidity/Rain of the Map"), myConditions.waterMapMin);
                break;
            case BUTTON_WIND:
                myConditions.windMap = EditorGUILayout.ObjectField(new GUIContent("Wind Map:", "The Wind map."), myConditions.windMap, typeof(Texture2D), true) as Texture2D;
                myConditions.windMapMax = EditorGUILayout.FloatField(new GUIContent("Max Wind (m/s):", "The max Wind of the Map"), myConditions.windMapMax);
                myConditions.windMapMin = EditorGUILayout.FloatField(new GUIContent("Min Wind (m/s):", "The min Wind of the Map"), myConditions.windMapMin);
                break;
            case BUTTON_SOIL:
                myConditions.soilMap = EditorGUILayout.ObjectField(new GUIContent("Soil Map:", "The Soil map."), myConditions.soilMap, typeof(Texture2D), true) as Texture2D;
                myConditions.soilMapMax = EditorGUILayout.FloatField(new GUIContent("Max Soil quality:", "The max Soil quality of the Map"), myConditions.soilMapMax);
                myConditions.soilMapMin = EditorGUILayout.FloatField(new GUIContent("Min Soil quality:", "The min Soil quality of the Map"), myConditions.soilMapMin);
                break;
        }


    }

}





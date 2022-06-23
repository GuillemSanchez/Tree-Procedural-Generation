using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;
using UnityEditor;

[ExecuteInEditMode]
public class ConditionCore : MonoBehaviour
{
    public TreeConditions myConditions;
    private Tree myTree;
    public TreeData myData;

    public bool firstUpdate = false;
    private float temp;
    private float water;
    private float wind;
    private float soil;

    public TreeData initalData;

    public GameObject myPlane;

    private float RadiusValue;
    private float HeightValue;

    private int TotalLeafs;

    List<float> heightArr = new List<float>();
    List<float> radiusArr = new List<float>();
    List<float> lSizeArr = new List<float>();
    List<float> lFrequencyArr = new List<float>();
    List<float> lColorArr = new List<float>();
    List<float> wColorArr = new List<float>();

    private float lastHe = 0;
    private float lastRa = 0;
    private float lastLS = 0;
    private float lastLF = 0;
    private float lastLC = 0;
    private float lastWC = 0;


    private Vector3 lastTransform;

    private void Update()
    {
        if (lastTransform == null)
            lastTransform = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);



        if (lastTransform != this.gameObject.transform.position)
        {
            GetPlaneUnderUs();
            this.gameObject.GetComponent<ConditionCore>().GettingReadyToUpdate();
        }

        if (firstUpdate)
        {
            GettingReadyToUpdate();
            FUpdate();
            firstUpdate = false;
        }



        lastTransform = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
    }

    private void GetPlaneUnderUs()
    {
        RaycastHit hit;

        if (Physics.Raycast(this.gameObject.transform.position, Vector3.down, out hit, 100f))
        {
            if (hit.collider.gameObject.GetComponent<MapCore>() != null)
            {
                myPlane = hit.collider.gameObject;
                Vector2 ColisionPoint = hit.textureCoord;
                GetPixelInformation(ColisionPoint);
            }
            else
            {
                myPlane = null;
            }
        }

    }

    private void GetPixelInformation(Vector2 textcoord)
    {
        // This is the texture that is coliding with with the vector2 in pixels.
        float finalTemp = 0;
        float finalSoil = 0;
        float finalWater = 0;
        float finalWind = 0;

        MapConditions con = myPlane.GetComponent<MapCore>().mapConditions;


        if (con.heatMap != null)
        {
            finalTemp = con.heatMap.GetPixel((int)(con.heatMap.width * textcoord.x), (int)(con.heatMap.height * textcoord.y)).grayscale;
            temp = FastChange(con.heatMapMin, con.heatMapMax, finalTemp);
        }

        if (con.soilMap != null)
        {
            finalSoil = con.soilMap.GetPixel((int)(con.soilMap.width * textcoord.x), (int)(con.soilMap.height * textcoord.y)).grayscale;
            soil = FastChange(con.soilMapMin, con.soilMapMax, finalSoil);
        }

        if (con.waterMap != null)
        {
            finalWater = con.waterMap.GetPixel((int)(con.waterMap.width * textcoord.x), (int)(con.waterMap.height * textcoord.y)).grayscale;
            water = FastChange(con.waterMapMin, con.waterMapMax, finalWater);
        }
        if (con.windMap != null)
        {
            finalWind = con.windMap.GetPixel((int)(con.windMap.width * textcoord.x), (int)(con.windMap.height * textcoord.y)).grayscale;
            wind = FastChange(con.windMapMin, con.windMapMax, finalWind);
        }


    }


    private float FastChange(float ini, float fin, float val)
    {
        float range = fin - ini;
        float finalval = (val * range) + ini;
        return finalval;
    }

    public void ModifyingHeight(float final)
    {
        float heightValue = 0;
        if (final > 0)
        {
            heightValue = ((myConditions.optimHeight - myConditions.standartHeight) * final) + myConditions.standartHeight;
        }
        if (final == 0)
        {
            heightValue = myConditions.standartHeight;
        }
        if (final < 0)
        {
            heightValue = myConditions.standartHeight - ((myConditions.standartHeight - myConditions.unOptimHeight) * Mathf.Abs(final));
        }

        List<TreeGroupBranch> toEdit = GetMainTrunks();


        for (int i = 0; i < toEdit.Count; i++)
        {
            toEdit[i].height = new Vector2(heightValue * 0.9f, heightValue * 1.1f);
        }

        HeightValue = heightValue;
        Preview();
    }
    public void ModifyingRadius(float final)
    {
        float radiusValue = 0;
        if (final > 0)
        {
            radiusValue = ((myConditions.optimRadius - myConditions.standartRadius) * final) + myConditions.standartRadius;
        }
        if (final == 0)
        {
            radiusValue = myConditions.standartRadius;
        }
        if (final < 0)
        {
            radiusValue = myConditions.standartRadius - ((myConditions.standartRadius - myConditions.unOptimRadius) * Mathf.Abs(final));
        }

        List<TreeGroupBranch> toEdit = GetMainTrunks();

        for (int i = 0; i < toEdit.Count; i++)
        {
            toEdit[i].radius = radiusValue;
        }
        RadiusValue = radiusValue;
        Preview();
    }

    public void ModifyingWoodColor(float final)
    {
        Color colorValue = new Color();

        colorValue.a = ChangeRange(myConditions.unOptimWoodColor.a, myConditions.optimWoodColor.a, final);
        colorValue.r = ChangeRange(myConditions.unOptimWoodColor.r, myConditions.optimWoodColor.r, final);
        colorValue.g = ChangeRange(myConditions.unOptimWoodColor.g, myConditions.optimWoodColor.g, final);
        colorValue.b = ChangeRange(myConditions.unOptimWoodColor.b, myConditions.optimWoodColor.b, final);

        myConditions.finalWColor = colorValue;
        for (int i = 0; i < myData.branchGroups.Length; i++)
        {
            myData.branchGroups[i].materialBranch.color = colorValue;
        }
    }

    public void ModifyingLeafColor(float final)
    {
        Color colorValue = new Color();

        colorValue.a = ChangeRange(myConditions.unOptimLeafColor.a, myConditions.optimLeafColor.a, final);
        colorValue.r = ChangeRange(myConditions.unOptimLeafColor.r, myConditions.optimLeafColor.r, final);
        colorValue.g = ChangeRange(myConditions.unOptimLeafColor.g, myConditions.optimLeafColor.g, final);
        colorValue.b = ChangeRange(myConditions.unOptimLeafColor.b, myConditions.optimLeafColor.b, final);

        myConditions.finalLColor = colorValue;
        for (int i = 0; i < myData.leafGroups.Length; i++)
        {
            myData.leafGroups[i].materialLeaf.color = colorValue;
        }
        for (int i = 0; i < myData.branchGroups.Length; i++)
        {
            if (myData.branchGroups[i].geometryMode == TreeGroupBranch.GeometryMode.BranchFrond)
                myData.branchGroups[i].materialFrond.color = colorValue;
        }
    }

    public void ModifyingFrequencyLeafs(float final)
    {
        int leafsValue = (int)ChangeRange(myConditions.unOptimNumberLeafs, myConditions.optimNumberLeafs, final);

        for (int i = 0; i < myData.leafGroups.Length; i++)
        {
            for (int j = 0; j < initalData.leafGroups.Length; j++)
            {
                if (myData.leafGroups[i].uniqueID == initalData.leafGroups[j].uniqueID)
                {
                    myData.leafGroups[i].distributionFrequency = leafsValue * initalData.leafGroups[j].distributionFrequency / TotalLeafs;
                    myData.UpdateFrequency(myData.leafGroups[i].uniqueID);
                }
            }
        }

        for (int i = 0; i < myData.branchGroups.Length; i++)
        {
            if (myData.branchGroups[i].geometryMode == TreeGroupBranch.GeometryMode.BranchFrond)
            {
                for (int j = 0; j < initalData.branchGroups.Length; j++)
                {
                    if (myData.branchGroups[i].uniqueID == initalData.branchGroups[j].uniqueID)
                    {
                        myData.branchGroups[i].distributionFrequency = leafsValue * initalData.branchGroups[j].distributionFrequency / TotalLeafs;
                        myData.UpdateFrequency(myData.branchGroups[i].uniqueID);
                    }
                }
            }

        }
        Preview();
    }

    public void ModifyingLeafSize(float final)
    {

        float sizeValue = ((myConditions.optimLeafSize - myConditions.unOptimLeafSize) / 2) + (((myConditions.optimLeafSize - myConditions.unOptimLeafSize) / 2) * final) + myConditions.unOptimLeafSize;
        for (int i = 0; i < initalData.leafGroups.Length; i++)
        {
            myData.leafGroups[i].size.x = initalData.leafGroups[i].size.x * sizeValue;
            myData.leafGroups[i].size.y = initalData.leafGroups[i].size.y * sizeValue;
        }

        List<TreeGroupBranch> fronds = GetFronds();

        for (int i = 0; i < fronds.Count; i++)
        {
            for (int j = 0; j < initalData.branchGroups.Length; j++)
            {
                if (fronds[i].uniqueID == initalData.branchGroups[j].uniqueID)
                {
                    fronds[i].frondWidth = initalData.branchGroups[j].frondWidth * sizeValue;
                    fronds[i].height.x = initalData.branchGroups[j].height.x * sizeValue;
                    fronds[i].height.y = initalData.branchGroups[j].height.y * sizeValue;
                }
            }
        }
        Preview();
    }
    public void ModifyingGrowth()
    {
        float heightRatio = ((HeightValue / myConditions.standartHeight) - 1);
        float radiusRatio = ((RadiusValue / myConditions.standartRadius) - 1);

        // Ha de ser un multiplicador
        float weight = (heightRatio + radiusRatio) / 2;

        List<TreeGroupBranch> mainGroups = GetMainTrunks();
        List<TreeGroupBranch> firstChilds = new List<TreeGroupBranch>();


        // We get all the first branches.
        for (int i = 0; i < mainGroups.Count; i++)
        {
            firstChilds.AddRange(GetMyBranches(mainGroups[i].uniqueID));
        }

        for (int i = 0; i < firstChilds.Count; i++)
        {
            for (int j = 0; j < initalData.branchGroups.Length; j++)
            {
                if (firstChilds[i].uniqueID == initalData.branchGroups[j].uniqueID)
                {
                    // Hacer una curva directamente.
                    AnimationCurve gCurve = new AnimationCurve();
                    // List<Keyframe> gFrames = new List<Keyframe>();
                    for (int c = 0; c < initalData.branchGroups[j].distributionScaleCurve.keys.Length; c++)
                    {
                        // Esta cambiando el value 
                        gCurve.AddKey(initalData.branchGroups[j].distributionScaleCurve.keys[c].time, initalData.branchGroups[j].distributionScaleCurve.keys[c].value + (initalData.branchGroups[j].distributionScaleCurve.keys[c].value * weight));
                        //gFrames.Add(new Keyframe(initalData.branchGroups[j].distributionScaleCurve.keys[c].time, initalData.branchGroups[j].distributionScaleCurve.keys[c].value * weight));
                    }
                    firstChilds[i].distributionScaleCurve = gCurve;
                }
            }
        }
    }



    public int getOriginalLeafs()
    {
        int totalLeafs = 0;

        for (int i = 0; i < initalData.leafGroups.Length; i++)
        {
            totalLeafs += initalData.leafGroups[i].nodeIDs.Length;
        }

        for (int i = 0; i < initalData.branchGroups.Length; i++)
        {
            if (initalData.branchGroups[i].geometryMode == TreeGroupBranch.GeometryMode.BranchFrond)
                totalLeafs += initalData.branchGroups[i].nodeIDs.Length;
        }
        TotalLeafs = totalLeafs;
        return totalLeafs;
    }


    public void GetInfo()
    {
        myTree = this.GetComponentInParent<Tree>();
        myData = myTree.data as TreeData;
        getOriginalLeafs();
    }

    public void GetOriginalData()
    {
        initalData = TreeData.Instantiate(myData);
       // AssetDatabase.CreateAsset(initalData, "Assets/" + myTree.name + "OrginalData.asset");
    }

    private List<TreeGroupBranch> GetMainTrunks()
    {
        List<TreeGroupBranch> final = new List<TreeGroupBranch>();

        for (int i = 0; i < myData.branchGroups.Length; i++)
        {
            if (myData.branchGroups[i].parentGroupID == myData.root.uniqueID)
                final.Add(myData.branchGroups[i]);
        }

        return final;
    }

    private List<TreeGroupBranch> GetMyBranches(int myID)
    {
        List<TreeGroupBranch> final = new List<TreeGroupBranch>();
        for (int i = 0; i < myData.branchGroups.Length; i++)
        {
            if (myData.branchGroups[i].parentGroupID == myID)
                final.Add(myData.branchGroups[i]);
        }

        return final;
    }

    private List<TreeGroupBranch> GetFronds()
    {
        List<TreeGroupBranch> final = new List<TreeGroupBranch>();

        for (int i = 0; i < myData.branchGroups.Length; i++)
        {
            if (myData.branchGroups[i].geometryMode == TreeGroupBranch.GeometryMode.BranchFrond)
                final.Add(myData.branchGroups[i]);
        }

        return final;
    }


    private float ChangeRange(float xi, float xf, float v)
    {
        return ((xf - xi) / 2) * (v + 1) + xi;
    }

    private void AssignMaterials(Renderer renderer, Material[] materials)
    {
        if (renderer != null)
        {
            if (materials == null)
                materials = new Material[0];

            renderer.sharedMaterials = materials;
        }
    }

    private void Preview()
    {
        Material[] materials;
        myData.PreviewMesh(myTree.transform.worldToLocalMatrix, out materials);
        AssignMaterials(myTree.GetComponent<Renderer>(), materials);
    }

    public void FUpdate()
    {
        Material[] materials;
        myData.UpdateMesh(myTree.transform.worldToLocalMatrix, out materials);
        AssignMaterials(myTree.GetComponent<Renderer>(), materials);
    }



    public void GettingReadyToUpdate()
    {
        heightArr.Clear();
        radiusArr.Clear();
        lSizeArr.Clear();
        lColorArr.Clear();
        lFrequencyArr.Clear();
        wColorArr.Clear();

        if (myConditions.myTemp != null)
        {
            if (myConditions.myTemp.adaptabilityRange != 0)
            {
                float val = myConditions.myTemp.GetCurveValue(temp);
                heightArr.Add(val);
                heightArr.Add(myConditions.myTemp.heightModifier);

                radiusArr.Add(val);
                radiusArr.Add(myConditions.myTemp.radiusModifier);

                lSizeArr.Add(val);
                lSizeArr.Add(myConditions.myTemp.leafSizeModifier);

                lColorArr.Add(val);
                lColorArr.Add(myConditions.myTemp.leafColorModifier);

                lFrequencyArr.Add(val);
                lFrequencyArr.Add(myConditions.myTemp.numberOfLeafsModifier);

                wColorArr.Add(val);
                wColorArr.Add(myConditions.myTemp.woodColorModifier);

            }
        }
        if (myConditions.myWater != null)
        {
            if (myConditions.myWater.adaptabilityRange != 0)
            {
                float val = myConditions.myWater.GetCurveValue(water);

                heightArr.Add(val);
                heightArr.Add(myConditions.myWater.heightModifier);

                radiusArr.Add(val);
                radiusArr.Add(myConditions.myWater.radiusModifier);

                lSizeArr.Add(val);
                lSizeArr.Add(myConditions.myWater.leafSizeModifier);

                lColorArr.Add(val);
                lColorArr.Add(myConditions.myWater.leafColorModifier);

                lFrequencyArr.Add(val);
                lFrequencyArr.Add(myConditions.myWater.numberOfLeafsModifier);

                wColorArr.Add(val);
                wColorArr.Add(myConditions.myWater.woodColorModifier);

            }
        }
        if (myConditions.mySoil != null)
        {
            if (myConditions.mySoil.adaptabilityRange != 0)
            {
                float val = myConditions.mySoil.GetCurveValue(soil);

                heightArr.Add(val);
                heightArr.Add(myConditions.mySoil.heightModifier);

                radiusArr.Add(val);
                radiusArr.Add(myConditions.mySoil.radiusModifier);

                lSizeArr.Add(val);
                lSizeArr.Add(myConditions.mySoil.leafSizeModifier);

                lColorArr.Add(val);
                lColorArr.Add(myConditions.mySoil.leafColorModifier);

                lFrequencyArr.Add(val);
                lFrequencyArr.Add(myConditions.mySoil.numberOfLeafsModifier);

                wColorArr.Add(val);
                wColorArr.Add(myConditions.mySoil.woodColorModifier);

            }
        }
        if (myConditions.myWind != null)
        {
            if (myConditions.myWind.adaptabilityRange != 0)
            {
                float val = myConditions.myWind.GetCurveValue(wind);

                heightArr.Add(val);
                heightArr.Add(myConditions.myWind.heightModifier);

                radiusArr.Add(val);
                radiusArr.Add(myConditions.myWind.radiusModifier);

                lSizeArr.Add(val);
                lSizeArr.Add(myConditions.myWind.leafSizeModifier);

                lColorArr.Add(val);
                lColorArr.Add(myConditions.myWind.leafColorModifier);

                lFrequencyArr.Add(val);
                lFrequencyArr.Add(myConditions.myWind.numberOfLeafsModifier);

                wColorArr.Add(val);
                wColorArr.Add(myConditions.myWind.woodColorModifier);
            }
        }
        float toHeight = 0;
        float valHeight = 0;
        float toRadius = 0;
        float valRadius = 0;
        float tolSize = 0;
        float vallSize = 0;
        float toFleaf = 0;
        float valFleaf = 0;
        float toLcolor = 0;
        float valLcolor = 0;
        float toWcolor = 0;
        float valWcolor = 0;

        if (heightArr.Count > 0)
        {
            for (int i = 0; i < heightArr.Count; i += 2)
            {
                toHeight += (heightArr[i] * heightArr[i + 1]);
                valHeight += heightArr[i + 1];
            }
            if (valHeight != 0)
                toHeight /= valHeight;

            //Debug.Log("Height: to " + toHeight);
        }


        if (radiusArr.Count > 0)
        {
            for (int i = 0; i < radiusArr.Count; i += 2)
            {
                toRadius += (radiusArr[i] * radiusArr[i + 1]);
                valRadius += radiusArr[i + 1];
            }
            if (valRadius != 0)
                toRadius /= valRadius;

        }

        if (lSizeArr.Count > 0)
        {
            for (int i = 0; i < lSizeArr.Count; i += 2)
            {
                tolSize += (lSizeArr[i] * lSizeArr[i + 1]);
                vallSize += lSizeArr[i + 1];
            }
            if (vallSize != 0)
                tolSize /= vallSize;

        }


        if (lFrequencyArr.Count > 0)
        {
            for (int i = 0; i < lFrequencyArr.Count; i += 2)
            {
                toFleaf += (lFrequencyArr[i] * lFrequencyArr[i + 1]);
                valFleaf += lFrequencyArr[i + 1];
            }
            if (valFleaf != 0)
                toFleaf /= valFleaf;

        }


        if (lColorArr.Count > 0)
        {
            for (int i = 0; i < lColorArr.Count; i += 2)
            {
                toLcolor += (lColorArr[i] * lColorArr[i + 1]);
                valLcolor += lColorArr[i + 1];
            }
            if (valLcolor != 0)
                toLcolor /= valLcolor;
        }


        if (wColorArr.Count > 0)
        {
            for (int i = 0; i < wColorArr.Count; i += 2)
            {
                toWcolor += (wColorArr[i] * wColorArr[i + 1]);
                valWcolor += wColorArr[i + 1];
            }
            if (valWcolor != 0)
                toWcolor /= valWcolor;
        }

        if (toHeight != lastHe)
            ModifyingHeight(toHeight);
        if (toRadius != lastRa)
            ModifyingRadius(toRadius);
        if (toRadius != lastRa || toHeight != lastHe)
            ModifyingGrowth();
        if (tolSize != lastLS)
            ModifyingLeafSize(tolSize);
        if (toFleaf != lastLF)
            ModifyingFrequencyLeafs(toFleaf);
        if (toLcolor != lastLC)
            ModifyingLeafColor(toLcolor);
        if (toWcolor != lastWC)
            ModifyingWoodColor(toWcolor);


        lastHe = toHeight;
        lastRa = toRadius;
        lastLS = tolSize;
        lastLF = toFleaf;
        lastLC = toLcolor;
        lastWC = toWcolor;
    }

    public void ShowOptim()
    {

        ModifyingHeight(1);

        ModifyingRadius(1);

        ModifyingGrowth();

        ModifyingLeafSize(1);

        ModifyingFrequencyLeafs(1);

        ModifyingLeafColor(1);

        ModifyingWoodColor(1);

        FUpdate();
    }


    public void ShowUnOptim()
    {

        ModifyingHeight(-1);

        ModifyingRadius(-1);

        ModifyingGrowth();

        ModifyingLeafSize(-1);

        ModifyingFrequencyLeafs(-1);

        ModifyingLeafColor(-1);

        ModifyingWoodColor(-1);

        FUpdate();
    }

}

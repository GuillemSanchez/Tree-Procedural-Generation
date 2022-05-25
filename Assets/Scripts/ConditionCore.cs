using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;

public class ConditionCore : MonoBehaviour
{
    public TreeConditions myConditions;
    private Tree myTree;
    private TreeData myData;

    private TreeData initalData;

    private float RadiusValue;
    private float HeightValue;


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

    public void ModifyingLeafSize(float final)
    {

    }
    public void ModifyingGrowth()
    {
        float heightRatio = ((HeightValue/myConditions.standartHeight)-1);
        float radiusRatio = ((RadiusValue/myConditions.standartRadius)-1);

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
                        gCurve.AddKey(initalData.branchGroups[j].distributionScaleCurve.keys[c].time, initalData.branchGroups[j].distributionScaleCurve.keys[c].value + (initalData.branchGroups[j].distributionScaleCurve.keys[c].value* weight));
                        //gFrames.Add(new Keyframe(initalData.branchGroups[j].distributionScaleCurve.keys[c].time, initalData.branchGroups[j].distributionScaleCurve.keys[c].value * weight));
                    }
                    firstChilds[i].distributionScaleCurve = gCurve;
                }
            }
        }
    }


    public void GetInfo()
    {
        myTree = this.GetComponentInParent<Tree>();
        myData = myTree.data as TreeData;
        initalData = myTree.data as TreeData;
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

}

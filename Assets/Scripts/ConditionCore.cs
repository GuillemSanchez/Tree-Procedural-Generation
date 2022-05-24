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
            List<TreeGroupBranch> myChilds = GetMyBranches(toEdit[i].uniqueID);
            for (int j = 0; j < myChilds.Count; j++)
            {
                if (heightValue != ((toEdit[i].height.y + toEdit[i].height.x) / 2))
                {
                    myChilds[j].height.x *= ((heightValue / ((toEdit[i].height.y + toEdit[i].height.x) / 2)) - 1);
                    myChilds[j].height.y *= ((heightValue / ((toEdit[i].height.y + toEdit[i].height.x) / 2)) - 1);
                }
            }
            toEdit[i].height = new Vector2(heightValue * 0.9f, heightValue * 1.1f);
        }


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
            List<TreeGroupBranch> myChilds = GetMyBranches(toEdit[i].uniqueID);
            for (int j = 0; j < myChilds.Count; j++)
            {
                if (radiusValue != toEdit[i].radius)
                    myChilds[j].radius *= ((radiusValue / (toEdit[i].radius)) - 1);
            }
            toEdit[i].radius = radiusValue;
        }
        Preview();
    }

    public void ModifyingLeafSize(float final)
    {

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

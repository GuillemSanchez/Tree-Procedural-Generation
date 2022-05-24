using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;

public class ConditionCore : MonoBehaviour
{
    public TreeConditions myConditions;
    private Tree myTree;
    private TreeData myData;


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
        Preview();
    }
    public void ModifyingRadius(float final)
    {

    }
    public void ModifyingLeafSize(float final)
    {

    }

    public void GetInfo()
    {
        myTree = this.GetComponentInParent<Tree>();
        myData = myTree.data as TreeData;
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

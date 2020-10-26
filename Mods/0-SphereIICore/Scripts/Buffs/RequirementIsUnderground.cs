
using UnityEngine;

public class RequirementIsUnderGround : TargetedCompareRequirementBase
{
    public override bool IsValid(MinEventParams _params)
    {

        bool result = false;
        
        Vector3i position = new Vector3i(_params.Self.position);
        if (position.y < GameManager.Instance.World.GetTerrainHeight(position.x, position.z))
            result = true;

        Debug.Log("Result: " + true);
        if (this.invert)
            return !result;
        else
            return result;
    }

}
public class RequirementIsOutdoor : TargetedCompareRequirementBase
{
    public override bool IsValid(MinEventParams _params)
    {

        bool result =false;

        Vector3i position = new Vector3i(_params.Self.position);
        if (GameManager.Instance.World.IsOpenSkyAbove(0, position.x, position.y, position.z))
            result = true;

        if (this.invert)
            return !result;
        else
            return result;
    }

}

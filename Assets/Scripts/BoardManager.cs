using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private MasterController mc;

    // these variables can be used to determine the slice to display
    private string targetFaceName = string.Empty;
    private string downwardFaceName = string.Empty;
    private int depth = 0;

    void Start()
    {
        mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        targetFaceName = mc.sliceTargetFaceName;
        downwardFaceName = mc.sliceDownwardFaceName;
    }

    // find the upper-left cell index and the planar dimension
    void ConvertFaceNamesToSlice()
    {
    }
}

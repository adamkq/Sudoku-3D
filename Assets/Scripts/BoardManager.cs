using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject _cellContainer;

    private CellController[] cells;
    private MasterController mc;

    // these variables can be used to determine the slice to display
    private string targetFaceName = string.Empty;
    private string downwardFaceName = string.Empty;
    private int depth = 0;

    void Start()
    {
        cells = _cellContainer.GetComponentsInChildren<CellController>();

        mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        AssignMCToCells(mc);

        targetFaceName = mc.sliceTargetFaceName;
        downwardFaceName = mc.sliceDownwardFaceName;

        SetIndexesForAllCells();
    }

    private void AssignMCToCells(MasterController mc)
    {
        foreach(var cell in cells)
        {
            cell.mc = this.mc;
        }
    }

    private void GetIndexOfUpperLeftCell()
    {
        // On Board Load, the upper left cell will be one of the 8 possible cube corners
        // there are 24 possible cube orientations to be in (6 faces, 4 rotations for each face)
        // The cell at [0,0,0] is the cell at the minimum x-axis position, maximum y-axis position, and minimum z-axis position

        int[] upperLeftCellIndex = new int[] { 0, 0, 0 };

        switch (targetFaceName)
        {
            case "CubeFaceFront":
                break;
        }
            

    }

    void SetIndexesForAllCells()
    {
        int index = 0;
        foreach(var cell in cells)
        {
            int[] cellIndex = new int[] { index % 8, index / 8, 0 };
            cell.SetCellIndex(cellIndex);
            index += 1;
        }
    }
}

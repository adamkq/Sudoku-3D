using System.Collections.Generic;
using System;
using UnityEngine;

public class Sieve
{
    // update this on each recursion of the solver
    public HashSet<char>[,,] validOptions;
    // initialize this, then on each recursion find the largest SR and recurse on that
    public int[,,] initialSearchReduction;

    public Sieve()
    {
        validOptions = new HashSet<char>[8, 8, 8];
        initialSearchReduction = new int[8, 8, 8];
    }

    internal int[] GetBestSearchReduction(int[,,] boardGivens)
    {
        /* Find the cellIndex that contains the option that, if picked, 
        * reduces the search space more than any other option. Skip over Given cells.
        */
        int runningBestReduction = 0;
        int[] runningBestIndex = new int[] { 8, 8, 8 }; // deliberately out of range

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (boardGivens[i, j, k] == 1) continue;

                    int isr = initialSearchReduction[i, j, k];

                    if (isr > runningBestReduction)
                    {
                        runningBestReduction = isr;
                        runningBestIndex = new int[] { i, j, k };
                    }

                    /* since each cell is in at least one set with 25 other cells (7 rows,
                    * 7 cols, 7 planes, 4 other subcube cells) and each cell has 8 total options, 
                    * the best possible search reduction is 25 * 8 = 200. In this case the function can stop early.
                    */
                    if (runningBestReduction == 200)
                    {
                        return runningBestIndex;
                    }
                }
            }
        }
        return runningBestIndex;
    }

    internal void UpdateValidOptionsForIntersectingCells(Solver solver, int[,,] boardGivens, char[,,] boardState, int[] cellIndex)
    {
        // for each cell that intersects with the chosen cell, update its set of possible options

        for (int i = 0; i < 8; i++)
        {
            // scan vertically thru rows
            if (i != cellIndex[0] && boardGivens[i, cellIndex[1], cellIndex[2]] != 1)
            {
                validOptions[i, cellIndex[1], cellIndex[2]] = solver.GetValidTokensForCell(boardState, cellIndex);
            }

            // scan horizontally thru columns
            if (i != cellIndex[1] && boardGivens[cellIndex[0], i, cellIndex[2]] != 1)
            {
                validOptions[cellIndex[0], i, cellIndex[2]] = solver.GetValidTokensForCell(boardState, cellIndex);
            }

            // scan depth-wise thru planes
            if (i != cellIndex[2] && boardGivens[cellIndex[0], cellIndex[1], i] != 1)
            {
                validOptions[cellIndex[0], cellIndex[1], i] = solver.GetValidTokensForCell(boardState, cellIndex);
            }
        }

        // check cubeset
        // find the corner cell for the cubeset, then iterate over all 8 cells
        int[] cornerCellIndex = new int[]
        {
            cellIndex[0] - cellIndex[0] % 2,
            cellIndex[1] - cellIndex[1] % 2,
            cellIndex[2] - cellIndex[2] % 2
        };

        for (int i = 0; i < 8; i++)
        {
            int rowIndex = cornerCellIndex[0] + i / 4;
            int colIndex = cornerCellIndex[1] + (i / 2) % 2;
            int planeIndex = cornerCellIndex[2] + i % 2;

            // no self-conflict; if the cell being checked is the given cell, skip
            if (rowIndex == cellIndex[0] && colIndex == cellIndex[1] && planeIndex == cellIndex[2]) continue;

            // if two of the three indices are the same as the corner cell index, also skip, since the cell has already been updated
            if (rowIndex == cornerCellIndex[0] && colIndex == cornerCellIndex[1]
                || colIndex == cornerCellIndex[1] && planeIndex == cornerCellIndex[2]
                || planeIndex == cornerCellIndex[2] && rowIndex == cornerCellIndex[0])
            {
                continue;
            }

            validOptions[rowIndex, colIndex, planeIndex] = solver.GetValidTokensForCell(boardState, cellIndex);
        }
    }

    internal bool AllCellsHaveAtLeastOneOption()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (validOptions[i, j, k].Count == 0) return false;
                }
            }
        }
        return true;
    }

    internal int GetInitialSearchReduction(StateManager stateManager, char[,,] boardState, int[] cellIndex)
    {
        int sr = 0;
        HashSet<char> TokenSet = new HashSet<char>(stateManager.TokenSet);

        foreach (char chr in TokenSet)
        {
            for (int i = 0; i < 8; i++)
            {
                // scan vertically thru rows
                if (i != cellIndex[0]
                    && validOptions[i, cellIndex[1], cellIndex[2]].Contains(chr)
                    && stateManager.BoardGivens[i, cellIndex[1], cellIndex[2]] != 1) sr++;

                // scan horizontally thru columns
                if (i != cellIndex[1]
                    && validOptions[cellIndex[0], i, cellIndex[2]].Contains(chr)
                    && stateManager.BoardGivens[cellIndex[0], i, cellIndex[2]] != 1) sr++;

                // scan depth-wise thru planes
                if (i != cellIndex[2]
                    && validOptions[cellIndex[0], cellIndex[1], i].Contains(chr)
                    && stateManager.BoardGivens[cellIndex[0], cellIndex[1], i] != 1) sr++;
            }

            // check cubeset
            // find the corner cell for the cubeset, then iterate over all 8 cells
            int[] cornerCellIndex = new int[]
            {
                cellIndex[0] - cellIndex[0] % 2,
                cellIndex[1] - cellIndex[1] % 2,
                cellIndex[2] - cellIndex[2] % 2
            };

            for (int i = 0; i < 8; i++)
            {
                int rowIndex = cornerCellIndex[0] + i / 4;
                int colIndex = cornerCellIndex[1] + (i / 2) % 2;
                int planeIndex = cornerCellIndex[2] + i % 2;

                // no self-conflict; if the cell being checked is the given cell, skip
                if (rowIndex == cellIndex[0] && colIndex == cellIndex[1] && planeIndex == cellIndex[2]) continue;

                // if two of the three indices are the same as the corner cell index, also skip, since the cell has already been updated
                if (rowIndex == cornerCellIndex[0] && colIndex == cornerCellIndex[1]
                    || colIndex == cornerCellIndex[1] && planeIndex == cornerCellIndex[2]
                    || planeIndex == cornerCellIndex[2] && rowIndex == cornerCellIndex[0])
                {
                    continue;
                }

                if (validOptions[rowIndex, colIndex, planeIndex].Contains(chr)) sr++;
            }
        }
        return sr;
    }
}
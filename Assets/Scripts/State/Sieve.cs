using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Sieve : MonoBehaviour
{
    public HashSet<char>[,,] validOptions;
    public Dictionary<char, int>[,,] searchReduction; // results are cached to improve performance

    public Sieve()
    {
        validOptions = new HashSet<char>[8, 8, 8];
    }

    internal void InitializeSearchReduction()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                for (int k = 0; k < 8; j++)
                {
                    foreach (char chr in validOptions[i, j, k])
                    {
                        searchReduction[i, j, k][chr] = GetSearchReduction();
                    }
                }
            }
        }
    }

    internal int[] GetBestSearchReduction(int[,,] boardGivens)
    {
        /* Find the cellIndex that contains the option that, if picked, 
        * reduces the search space more than any other option. Skip over Given cells.
        */
        int runningBestReduction = 0;
        int[] runningBestIndex = new int[] { 0, 0, 0 };

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                for (int k = 0; k < 8; j++)
                {
                    if (boardGivens[i, j, k] == 1) continue;

                    foreach (char chr in searchReduction[i, j, k].Keys)
                    {
                        int sr = searchReduction[i, j, k][chr];

                        if (sr > runningBestReduction)
                        {
                            runningBestReduction = sr;
                            runningBestIndex = new int[] { i, j, k };
                        }

                        /* since each cell is in at least one set with 25 other cells (7 rows,
                        * 7 cols, 7 planes, 4 other subcube cells), the best possible search
                        * reduction is 25. In this case the function can stop early.
                        */
                        if (runningBestReduction == 25)
                        {
                            return runningBestIndex;
                        }
                    }
                }
            }
        }

        return runningBestIndex;
    }

    private int GetSearchReduction()
    {
        // find out how many options get eliminated for each choice in each cell. Do not count Given cells.
        throw new NotImplementedException();
    }

    internal bool ApplySelection(int[] cellIndex, char selection)
    {
        throw new NotImplementedException();
    }

    internal bool RevertSelection(int[] cellIndex, char selection)
    {
        throw new NotImplementedException();
    }

    internal bool AllCellsHaveAtLeastOneOption()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                for (int k = 0; k < 8; j++)
                {
                    if (validOptions[i, j, k].Count == 0) return false;
                }
            }
        }
        return true;
    }
}

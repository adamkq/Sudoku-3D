using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solver : MonoBehaviour
{
    private MasterController _mc;
    private StateManager _sm;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        _sm = _mc.stateManager;
    }

    // find the tokens that don't conflict with other tokens in the set(s)
    public HashSet<char> GetValidTokensForCell(char[,,] boardState, int[] cellIndex)
    {
        HashSet<char> TokenSet = new HashSet<char>(_sm.TokenSet);
        HashSet<char> ExcludeSet = new HashSet<char>();

        // LINQ is unsuitable because a multidimensional array doesn't implement IEnumerable<T>
        // have to use good old forloops

        // check rowset, columnset, and planeset
        // no self-conflict; if the cell being checked is the given cell, skip
        for (int i = 0; i < 8; i++)
        {
            // scan vertically thru rows
            if (i != cellIndex[0]) ExcludeSet.Add(boardState[i, cellIndex[1], cellIndex[2]]);

            // scan horizontally thru columns
            if (i != cellIndex[1]) ExcludeSet.Add(boardState[cellIndex[0], i, cellIndex[2]]);

            // scan depth-wise thru planes
            if (i != cellIndex[2]) ExcludeSet.Add(boardState[cellIndex[0], cellIndex[1], i]);
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

            ExcludeSet.Add(boardState[rowIndex, colIndex, planeIndex]);
        }

        TokenSet.ExceptWith(ExcludeSet);

        return TokenSet;
    }

    int SolveBacktrack(char[,,] boardState, bool stopOnFirstSolution = false)
    {
        int numberOfSolutionsFound = 0;

        HashSet<char>[,,] sieve = new HashSet<char>[8, 8, 8];

        // get initial choices
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                for (int k = 0; k < 8; j++)
                {
                    sieve[i, j, k] = GetValidTokensForCell(boardState, new int[] { i, j, k });
                }
            }
        }

        // for each choice, take the choice, evaluate the number of options per cell (iteratively) and keep track
        // of the one that reduces the search space the most. Then actually apply the most reductive choice (recursively).
        


        return numberOfSolutionsFound;
    }
}

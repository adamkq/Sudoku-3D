using System;
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

    bool IsSolved(char[,,] boardState)
    {
        // If any cell is empty, return false. This will handle the majority of calls quickly.
        // If any cell conflicts with any other cell, return false.
        return false; // TODO
    }

    int SolveBacktrack(char[,,] boardState, bool stopOnFirstSolution = false)
    {
        Sieve sieve = new Sieve();
        int numberOfSolutionsFound = 0;

        void SolveBacktrackRecursive(char[,,] _boardState)
        {
            // stop early
            if (stopOnFirstSolution && numberOfSolutionsFound > 0) return;

            // increment solution count
            if (IsSolved(boardState))
            {
                numberOfSolutionsFound += 1;
                return;
            }

            // any branches in which not all cells have an option can be pruned.
            if (!sieve.AllCellsHaveAtLeastOneOption()) return;

            /* Approach:
             * 1. Find the cellIndex that contains the option that, if picked, 
             * reduces the search space more than any other option (this is done
             * via the sieve class).
             * 2. Iterate through all options in the cell index.
             * 3. Apply each option, call this inner function recursively, and
             * then revert each option.
             */
            int[] cellIndex = sieve.GetBestSearchReduction(_sm.BoardGivens);

            if (_sm.IsGiven(cellIndex))
            {
                throw new Exception("You messed up");
            }

            foreach(char chr in sieve.validOptions[cellIndex[0],cellIndex[1],cellIndex[2]])
            {
                _boardState[cellIndex[0], cellIndex[1], cellIndex[2]] = chr;

                sieve.ApplySelection(cellIndex, chr);
                SolveBacktrackRecursive(_boardState);

                sieve.RevertSelection(cellIndex, chr);
            }
            
        }

        // get initial choices
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                for (int k = 0; k < 8; j++)
                {
                    sieve.validOptions[i, j, k] = GetValidTokensForCell(boardState, new int[] { i, j, k });
                }
            }
        }
        sieve.InitializeSearchReduction();

        SolveBacktrackRecursive(boardState);

        return numberOfSolutionsFound;
    }
}

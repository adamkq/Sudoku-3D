using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (boardState[i, j, k] == ' ') return false;
                }
            }
        }
        return true;
    }

    public int SolveBacktrack(char[,,] boardState, bool stopOnFirstSolution = false)
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
             * via the Sieve class).
             * 2. Iterate through all options in the cell index.
             * 3. Apply each option and call this inner function recursively
             */
            int[] cellIndex = sieve.GetBestSearchReduction(_sm.BoardGivens);
            
            if (cellIndex.Max() > 7)
            {
                return; // no best isr found, or all cells are givens
            }
            if (_sm.IsGiven(cellIndex))
            {
                throw new Exception("Solver attempted to modify given (constant) cell");
            }

            sieve.initialSearchReduction[cellIndex[0], cellIndex[1], cellIndex[2]] = -1; // marks the cell so the algorithm doesn't recurse on it

            char storeChr = _boardState[cellIndex[0], cellIndex[1], cellIndex[2]];

            foreach (char chr in sieve.validOptions[cellIndex[0],cellIndex[1],cellIndex[2]])
            {
                Debug.LogFormat("{0}, {1}, {2}: {3}", cellIndex[0], cellIndex[1], cellIndex[2], chr);
                _boardState[cellIndex[0], cellIndex[1], cellIndex[2]] = chr;

                sieve.UpdateValidOptionsForIntersectingCells(this, _sm.BoardGivens, boardState, cellIndex);
                SolveBacktrackRecursive(_boardState);
            }

            _boardState[cellIndex[0], cellIndex[1], cellIndex[2]] = storeChr; // leave board unmodified in case of no solution

        }

        // get initial options
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                for (int k = 0; k < 8; k++)
                {
                    int[] cellIndex = new int[] { i, j, k };
                    sieve.validOptions[i, j, k] = GetValidTokensForCell(boardState, cellIndex);
                }
            }
        }

        // get initial search reduction
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                for (int k = 0; k < 8; k++)
                {
                    int[] cellIndex = new int[] { i, j, k };
                    if (_sm.BoardGivens[i, j, k] == 1)
                    {
                        sieve.initialSearchReduction[i, j, k] = -1; // cells with isr value = -1 will not be recursed on
                    }
                    else
                    {
                        sieve.initialSearchReduction[i, j, k] = sieve.GetInitialSearchReduction(_sm, boardState, cellIndex);
                    }
                }
            }
        }

        SolveBacktrackRecursive(boardState);

        return numberOfSolutionsFound;
    }
}

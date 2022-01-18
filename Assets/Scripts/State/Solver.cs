﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Solver : MonoBehaviour
{
    private MasterController _mc;
    private StateManager m_stateManager;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        m_stateManager = _mc.stateManager;
    }

    // find the tokens that don't conflict with other tokens in the set(s)
    public HashSet<char> GetValidTokensForCell(char[,,] boardState, int[] cellIndex)
    {
        HashSet<char> TokenSet = new HashSet<char>(m_stateManager.TokenSet);
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
        int rowIndex, colIndex, planeIndex;
        for (int i = 0; i < 8; i++)
        {
            rowIndex = cellIndex[0] - cellIndex[0] % 2 + i / 4;
            colIndex = cellIndex[1] - cellIndex[1] % 2 + i / 2 % 2;
            planeIndex = cellIndex[2] - cellIndex[2] % 2 + i % 2;

            // no self-conflict; if the cell being checked is the given cell, skip
            if (rowIndex == cellIndex[0] && colIndex == cellIndex[1] && planeIndex == cellIndex[2]) continue;

            ExcludeSet.Add(boardState[rowIndex, colIndex, planeIndex]);
        }

        TokenSet.ExceptWith(ExcludeSet);

        return TokenSet;
    }

    public bool IsSolved(char[,,] boardState)
    {
        HashSet<char> subSet = new HashSet<char>();
        char chr;
        // in each rowset, columnset, planeset, and cubeset, each symbol must appear exactly once
        for (int setID = 0; setID < 64; setID++)
        {
            // rowset; column and page fixed
            subSet.Clear();
            for (int i = 0; i < 8; i++)
            {
                chr = boardState[i, setID / 8, setID % 8];
                if (!m_stateManager.TokenSet.Contains(chr)) return false; // clear/blank cells
                if (subSet.Contains(chr)) return false; // duplicate cells
                subSet.Add(chr);
            }

            // columnset; row and page fixed
            subSet.Clear();
            for (int i = 0; i < 8; i++)
            {
                chr = boardState[setID / 8, i, setID % 8];
                if (!m_stateManager.TokenSet.Contains(chr)) return false; // clear/blank cells
                if (subSet.Contains(chr)) return false; // duplicate cells
                subSet.Add(chr);
            }

            // planeset; column and row fixed
            subSet.Clear();
            for (int i = 0; i < 8; i++)
            {
                chr = boardState[setID / 8, setID % 8, i];
                if (!m_stateManager.TokenSet.Contains(chr)) return false; // clear/blank cells
                if (subSet.Contains(chr)) return false; // duplicate cells
                subSet.Add(chr);
            }

            // cubeset
            subSet.Clear();

            int cornerRowIndex = (setID / 16) * 2;
            int cornerColumnIndex = ((setID / 4) % 4) * 2;
            int cornerPlaneIndex = (setID % 4) * 2;
            for (int i = 0; i < 8; i++)
            {
                int rowIndex = cornerRowIndex + i / 4;
                int colIndex = cornerColumnIndex + i / 2 % 2;
                int planeIndex = cornerPlaneIndex + i % 2;

                chr = boardState[rowIndex, colIndex, planeIndex];
                if (!m_stateManager.TokenSet.Contains(chr)) return false; // clear/blank cells
                if (subSet.Contains(chr)) return false; // duplicate cells
                subSet.Add(chr);
            }
        }

        return true;
    }

    public int SolveBacktrack(char[,,] boardState, int maxNumberOfSolutions = 1)
    {
        int numberOfSolutionsFound = 0;
        int recursiveCalls = 0;

        bool SolveBacktrackRecursive(char[,,] _boardState, int cellIndexSerialized = 0)
        {
            /* Approach:
             * 1. Find the next cell Index.
             * 2. Iterate through all options in the cell.
             * 3. Apply each option and call this inner function recursively
             */

            // blank board takes 1428 calls to solve
            recursiveCalls += 1;
            if (recursiveCalls > Math.Max(1500 * maxNumberOfSolutions, 999999)) return false;

            // stop if enough solutions have been found
            if (numberOfSolutionsFound >= maxNumberOfSolutions) return true;

            // Check solution
            if (IsSolved(_boardState))
            {
                numberOfSolutionsFound += 1;
                return true;
            }

            // find next cell and deserialize it
            int[] cellIndex = DeserializeCell(cellIndexSerialized);
            while (m_stateManager.IsGiven(cellIndex))
            {
                cellIndexSerialized += 1; // skip over Given cells
                cellIndex = DeserializeCell(cellIndexSerialized);
            }

            // safeguards
            if (cellIndex.Max() > 7) throw new Exception("Index out of range error in solver");

            if (m_stateManager.IsGiven(cellIndex)) throw new Exception("Solver tried to modify given cell");

            // cache the current cell value
            char storeChr = _boardState[cellIndex[0], cellIndex[1], cellIndex[2]];

            HashSet<char> validTokens = GetValidTokensForCell(_boardState, cellIndex);

            // reduce search space
            List<char> validTokensWithAtLeastOneOption = LeaveAtLeastOneValidTokenPerEmptyCell(_boardState, cellIndex, validTokens).ToList();

            //var rnd = new System.Random();
            //var randomized = validTokensWithAtLeastOneOption.OrderBy(item => rnd.Next()).ToList();

            foreach (char chr in validTokensWithAtLeastOneOption)
            {
                _boardState[cellIndex[0], cellIndex[1], cellIndex[2]] = chr;

                // recurse on next cell
                if (SolveBacktrackRecursive(_boardState, cellIndexSerialized + 1) && numberOfSolutionsFound >= maxNumberOfSolutions) return true;

                _boardState[cellIndex[0], cellIndex[1], cellIndex[2]] = storeChr; // leave board unmodified in case of no solution
            }   

            return false;
        }

        // first, check for conflicts among given cells. If any, return -1
        for (int ii = 0; ii < 8; ii++)
        {
            for (int jj = 0; jj < 8; jj++)
            {
                for (int kk = 0; kk < 8; kk++)
                {
                    int[] cellIndex = new int[] { ii, jj, kk };
                    HashSet<char> validTokens = GetValidTokensForCell(boardState, cellIndex);
                    if (m_stateManager.IsGiven(cellIndex) && !validTokens.Contains(boardState[ii, jj, kk])) return -1;
                }
            }
        }

        SolveBacktrackRecursive(boardState);

        Debug.Log(recursiveCalls);

        return numberOfSolutionsFound;
    }

    private int[] DeserializeCell(int cellIndexSerialized)
    {
        // cIS = row * 64 + col * 8 + pageIndex
        int pageIndex = cellIndexSerialized % 8;
        int columnIndex = (cellIndexSerialized / 8) % 64 % 8;
        int rowIndex = (cellIndexSerialized - columnIndex * 8 - pageIndex) / 64;

        return new int[] { rowIndex, columnIndex, pageIndex };
    }

    private HashSet<char> LeaveAtLeastOneValidTokenPerEmptyCell(char[,,] boardState, int[] cellIndex, HashSet<char> validTokens)
    {
        // scan all cells that could be affected by the choice, and of those that are empty,
        // check that they will still have valid tokens after the choice is made

        // copy validTokens, then remove ones that don't fit criteria
        HashSet<char> validTokensAtLeastOneOption = new HashSet<char>(validTokens);
        HashSet<char> excludeSet = new HashSet<char>();

        foreach(var chr in validTokens)
        {
            for (int i = 0; i < 8; i++)
            {
                HashSet<char> _validTokens;

                // scan vertically thru rows; no self-check and only check empty cells
                if (i != cellIndex[0] && boardState[i, cellIndex[1], cellIndex[2]] == ' ')
                {
                    _validTokens = GetValidTokensForCell(boardState, new int[] { i, cellIndex[1], cellIndex[2] });
                    if (_validTokens.Count == 1 && _validTokens.Contains(chr)) excludeSet.Add(chr);
                }

                // scan horizontally thru columns
                if (i != cellIndex[1] && boardState[cellIndex[0], i, cellIndex[2]] == ' ')
                {
                    _validTokens = GetValidTokensForCell(boardState, new int[] { cellIndex[0], i, cellIndex[2] });
                    if (_validTokens.Count == 1 && _validTokens.Contains(chr)) excludeSet.Add(chr);
                }

                // scan depth-wise thru planes
                if (i != cellIndex[2] && boardState[cellIndex[0], cellIndex[1], i] == ' ')
                {
                    _validTokens = GetValidTokensForCell(boardState, new int[] { cellIndex[0], cellIndex[1], i });
                    if (_validTokens.Count == 1 && _validTokens.Contains(chr)) excludeSet.Add(chr);
                }

                // stop early
                if (validTokens.SetEquals(excludeSet)) return new HashSet<char>();
            }

            // check cubeset
            // find the corner cell for the cubeset, then iterate over all 8 cells
            int rowIndex, colIndex, planeIndex;
            for (int i = 0; i < 8; i++)
            {
                HashSet<char> _validTokens;

                rowIndex = cellIndex[0] - cellIndex[0] % 2 + i / 4;
                colIndex = cellIndex[1] - cellIndex[1] % 2 + i / 2 % 2;
                planeIndex = cellIndex[2] - cellIndex[2] % 2 + i % 2;

                // no self-conflict; if the cell being checked is the given cell, skip
                if (rowIndex == cellIndex[0] && colIndex == cellIndex[1] && planeIndex == cellIndex[2] || boardState[cellIndex[0], cellIndex[1], i] != ' ') continue;

                _validTokens = GetValidTokensForCell(boardState, new int[] { rowIndex, colIndex, planeIndex });
                if (_validTokens.Count == 1 && _validTokens.Contains(chr)) excludeSet.Add(chr);

                // stop early
                if (validTokens.SetEquals(excludeSet)) return new HashSet<char>();
            }
        }

        validTokensAtLeastOneOption.ExceptWith(excludeSet);
        return validTokensAtLeastOneOption;
    }

}

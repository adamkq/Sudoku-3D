using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Solver : MonoBehaviour
{
    private MasterController _mc;
    private StateManager m_stateManager;

    [SerializeField] private int m_maxNumberOfSolutions = 1;
    [SerializeField] private bool solve;
    [SerializeField] private bool clear;

    public BoardManager BoardManager { get; set; }

    private void OnValidate()
    {
        if (solve)
        {
            solve = false;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            int solutions = SolveBacktrack(m_stateManager.BoardState, m_maxNumberOfSolutions);
            watch.Stop();

            Debug.LogFormat("Found {0} solutions in {1} seconds",solutions, watch.ElapsedMilliseconds / 1000f);
        }

        if (clear)
        {
            clear = false;
            for (int ii = 0; ii < 8; ii++)
            {
                for (int jj = 0; jj < 8; jj++)
                {
                    for (int kk = 0; kk < 2; kk++)
                    {
                        int[] cellIndex = new int[] { ii, jj, kk };
                        if (!m_stateManager.IsGiven(cellIndex)) m_stateManager.BoardState[ii, jj, kk] = ' ';
                    }
                }
            }
        }

        if (BoardManager) BoardManager.UpdateAllCells();
    }

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
            //if (i != cellIndex[2]) ExcludeSet.Add(boardState[cellIndex[0], cellIndex[1], i]);
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
            //subSet.Clear();
            //for (int i = 0; i < 8; i++)
            //{
            //    chr = boardState[setID / 8, setID % 8, i];
            //    if (!m_stateManager.TokenSet.Contains(chr)) return false; // clear/blank cells
            //    if (subSet.Contains(chr)) return false; // duplicate cells
            //    subSet.Add(chr);
            //}

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

            recursiveCalls += 1;
            if (recursiveCalls > Math.Max(1000 * maxNumberOfSolutions, 99999999)) return false;

            // Check solution
            if (cellIndexSerialized > 127)
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
            if (cellIndex[0] > 7 || cellIndex[1] > 7 || cellIndex[2] > 1) throw new Exception("Index out of range error in solver");

            if (m_stateManager.IsGiven(cellIndex)) throw new Exception("Solver tried to modify given cell");

            // cache the current cell value
            char storeChr = _boardState[cellIndex[0], cellIndex[1], cellIndex[2]];

            HashSet<char> validTokens = GetValidTokensForCell(_boardState, cellIndex);

            foreach (char chr in validTokens)
            {
                _boardState[cellIndex[0], cellIndex[1], cellIndex[2]] = chr;

                SolveBacktrackRecursive(_boardState, cellIndexSerialized + 1);
                
                if (maxNumberOfSolutions >= 0 && numberOfSolutionsFound >= maxNumberOfSolutions) return true;
            }
            _boardState[cellIndex[0], cellIndex[1], cellIndex[2]] = storeChr; // leave board unmodified in case of no solution

            return false;
        }

        // first, check for conflicts among given cells. If any, return -1
        for (int ii = 0; ii < 8; ii++)
        {
            for (int jj = 0; jj < 8; jj++)
            {
                for (int kk = 0; kk < 2; kk++)
                {
                    int[] cellIndex = new int[] { ii, jj, kk };
                    HashSet<char> validTokens = GetValidTokensForCell(boardState, cellIndex);
                    if (m_stateManager.IsGiven(cellIndex) && !validTokens.Contains(boardState[ii, jj, kk])) return -1;
                }
            }
        }

        SolveBacktrackRecursive(boardState);

        Debug.Log("Recursive calls: " + recursiveCalls);

        return numberOfSolutionsFound;
    }

    private int[] DeserializeCell(int cellIndexSerialized)
    {
        // cIS = row * 16 + col * 2 + pageIndex (sandwich sudoku)
        int pageIndex = cellIndexSerialized % 2;
        int columnIndex = cellIndexSerialized / 2 % 8;
        int rowIndex = (cellIndexSerialized - columnIndex * 2 - pageIndex) / 16;

        return new int[] { rowIndex, columnIndex, pageIndex };
    }
}

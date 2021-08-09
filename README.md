# Sudoku-3D
Generalize the rules of Sudoku to a 3D board.

The "board" in this case is an 8 x 8 x 8 cube, containing 512 cells up from 81 in standard Sudoku.
To solve the puzzle, the digits 1 - 8 must be entered into each cell, and each subset of cells must have only one of each. In standard Sudoku there are 27 subsets (9 rows, 9 columns, 9 subsquares) but in Sudoku 3D there are 256 (64 rows, 64 columns, 64 aisles, 64 subcubes). 

## UI ##
The user can alternate between 2 views in order to solve the puzzle:
* an "Orbit" view that allows the user to rotate and zoom on the 3D cube
* a "Slice" view that presents a 2D cross section of the cube (8 x 8 cells)

In Slice view, on-screen buttons will allow the user to go to the next or previous slice.

## Solver ##
This app will also contain a Solver which will take the current board state and determine the following:
* Which cells are in conflict, if any
* The possible valid entries (digits 1 thru 8) for each cell
* The possible solutions (fully populated boards)

The intent is for puzzles to only have one possible solution. To make it easier for the user this rule could be relaxed.

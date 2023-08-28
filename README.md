# Sudoku-3D
Generalize the rules of Sudoku to a 3D board.

The "board" in this case is an 8 x 8 x 2 grid, containing 128 cells up from 81 in standard Sudoku.
To solve the puzzle, the digits 1 - 8 must be entered into each cell, and each subset of cells must have only one of each. In standard Sudoku there are 27 subsets (9 rows, 9 columns, 9 subsquares) but in Sudoku 3D there are 48 (16 rows, 16 columns, 16 subcubes). 

## UI ##
The user can view both "slices" at once on a sufficiently large iPhone screen. These operations are available in-game:
* select cell value (1-8) on a popup menu
* clear the value of a cell
* Undo/Redo
* Erase (while selected, click cells to automatically clear them)

As well, a separate menu will be available to do the following:
* Exit without saving
* Save and exit
* Reset Board
* Help Guide
* Options


## Solver ##
This app will also contain a Solver which will take the current board state and determine the following:
* Which cells are in conflict, if any
* The possible valid entries (digits 1 thru 8) for each cell
* The possible solutions (fully populated boards), where the intent is for unsolved puzzles to only have one possible solution.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using System.Diagnostics;



namespace Sudoku
{
    class Sudoku
    {


        /// <summary>
        /// Sudoku grid
        /// </summary>
        public int[,] Grid = new int[9,9];

        /// <summary>
        /// The starting sudoku grid
        /// </summary>
        public int[,] StartGrid = new int[9, 9];





        private SudokuSolver Solver = new SudokuSolver();










        public Sudoku() { }

        public Sudoku(int[,] grid)
        {
            if (grid.GetLength(0) != 9 || grid.GetLength(1) != 9) { Debug.WriteLine($"Sudoku: Sudoku(int[,]) the UI might only be capable of accepting a grid of 9x9"); }

            Grid = grid;
            StartGrid = grid;
        }














        /// <summary>
        /// Length of the first dimension of the sudoku grid (array)
        /// </summary>
        public int M { get { return Grid.GetLength(0); } }
        /// <summary>
        /// Length of the second dimension of the sudoku grid (array)
        /// </summary>
        public int N { get { return Grid.GetLength(1); } }
        /// <summary>
        /// Get the total number of elements in the sudoku grid
        /// </summary>
        public int Length { get { return Grid.Length; } }

        /// <summary>
        /// Get whether the grid completely full and has no empty cells left, or not
        /// </summary>
        public bool IsFull { get { return GetFirstEmptyCell()[0] > 0; } }












        // -----------------------------------------------------------------
        // -------------------      Grid Creation      ---------------------
        // -----------------------------------------------------------------
        #region GRID_CREATION_METHODS

        /// <summary>
        /// Load a new grid
        /// </summary>
        /// <param name="grid"></param>
        public void NewGrid(int[,] grid)
        {
            if (grid.GetLength(0) != 9 || grid.GetLength(1) != 9) { Debug.WriteLine($"Sudoku: NewGrid(int[,]) at the moment the UI is only capable of accepting a grid of 9x9"); }

            Grid = grid;
            StartGrid = grid;
        }

        /// <summary>
        /// Load a new grid
        /// </summary>
        /// <param name="grid"></param>
        public void NewGrid(int[] grid)
        {
            if (!IsSqrtWholeInt(grid.Length)) { Debug.WriteLine($"Sudoku: NewGrid(int[]) the square root of grid.Length does not result in a whole number, this might cause problems!"); }
            if (grid.Length != 81) { Debug.WriteLine($"Sudoku: NewGrid(int[]) at the moment the UI is only capable of accepting a grid of 9x9"); }

            int x = (int)Math.Sqrt(grid.Length);
            for (int m = 0; m < x; m++)
            {
                for (int n = 0; n < x; n++)
                {
                    StartGrid[m, n] = grid[m * x + n];
                }
            }
            Grid = StartGrid;
        }

        /// <summary>
        /// Reset the grid to its starting condition
        /// </summary>
        public void RestartGrid()
        {
            Grid = (int[,])StartGrid.Clone();
        }

        #endregion GRID_CREATION_METHODS








        // -----------------------------------------------------------------
        // --------------------      Grid Gets      ------------------------
        // -----------------------------------------------------------------
        #region GRID_GET_METHODS

        /// <summary>
        /// Get the solution for the currently active sudoku
        /// </summary>
        /// <returns></returns>
        public async Task<int[,]> GetSolution(bool useGuessOnDeadEnd = false)
        {
            // Make sure a grid is available to solve
            if (!GridContainsValues(StartGrid)) { return StartGrid; }
            int[,] solution = await Task.Run(() => Solver.SolveHomeBrew(StartGrid, useGuessOnDeadEnd));
            return solution;
        }

        /// <summary>
        /// Get the length of the specified dimension dim of the current sudoku grid (array)
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        public int GetLength(int dim)
        {
            if (dim < 0 || dim >= Grid.Rank) return -1;
            else return Grid.GetLength(dim);
        }

        /// <summary>
        /// Get the coordinates of the first empty cell found on the grid
        /// </summary>
        /// <returns>int[2], 0=m & 1=n</returns>
        public int[] GetFirstEmptyCell()
        {
            int[] cellCoor = new int[2] { -1, -1 };

            for (int m = 0; m < M; m++)
            {
                for (int n = 0; n < N; n++)
                {
                    if (Grid[m, n] == 0)
                    {
                        cellCoor[0] = m;
                        cellCoor[1] = n;
                        break;
                    }
                }
            }
            return cellCoor;
        }

        /// <summary>
        /// Get the valid numbers for the given cell. Returns 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool[] GetValidNumbersForCell(int m, int n)
        {
            return Solver.GetValidNumbers(m, n, Grid);
        }

        #endregion GRID_GET_METHODS








        // -----------------------------------------------------------------
        // -----------------      Cell Validation      ---------------------
        // -----------------------------------------------------------------
        #region CELL_VALIDATION_METHODS

        /// <summary>
        /// Check whether the number v in valid in the cell at coors m, n
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="v"></param>
        /// <returns>True is no conflicting value(s) is found. False is there is</returns>
        public bool IsCellValid(int m, int n, int v = -1)
        {
            if (v < 0) { v = Grid[m, n]; }

            return IsRowValidCell(m, n, v) && IsColumnValidCell(m, n, v) && IsRegionValidCell(m, n, v);
        }

        /// <summary>
        /// Check whether the number v is present anywhere else the row m
        /// </summary>
        /// <param name="m">Row to search through</param
        /// <param name="n">Column of the original cell with value v, needed to not hit on the original cell and its value. Provide a negative number to ignore and match with any cell</param>
        /// <param name="v">Value to match</param>
        /// <returns>True is no matching value is found. False is there is</returns>
        public bool IsRowValidCell(int m, int n, int v = -1)
        {
            // If no v was specified, get the current value of the Sudoku cell at [m, n]
            if (v < 0) { v = Grid[m, n]; }

            // If v is not 0, ie the cell is not empty
            if (v != 0)
            {
                // Go over the cells in row m
                for (int i = 0; i < Grid.GetLength(1); i++)
                {
                    // If the value v matches with the value in the cell, and the cell is not the original at column n, cell is not valid so return false
                    if (v == Grid[m, i] && i != n) { return false; }
                }
            }

            // Value is 0 or no match was found, return true
            return true;
        }

        /// <summary>
        /// Check whether the number v is present anywhere else in the column n
        /// </summary>
        /// <param name="m">Row of the original cell with value v, needed to not hit on the original cell and its value. Provide a negative number to ignore and match with any cell</param>
        /// <param name="n"></param>
        /// <param name="v"></param>
        /// <returns>True is no matching value is found. False is there is</returns>
        public bool IsColumnValidCell(int m, int n, int v = -1)
        {
            // If no v was specified, get the current value of the Sudoku cell at [m, n]
            if (v < 0) { v = Grid[m, n]; }

            // If v is not 0, ie the cell is not empty
            if (v != 0)
            {
                // Go over the cells in row m
                for (int i = 0; i < Grid.GetLength(0); i++)
                {
                    // If the value v matches with the value in the cell, and the cell is not the original at row m, cell is not valid so return false
                    if (v == Grid[i, n] && i != m) { return false; }
                }
            }

            // Value is 0 or no match was found, return true
            return true;
        }

        /// <summary>
        /// Check whether the number v is present anywhere else in the local region
        /// The dimensions of the region are determined by dividing the Sudoku into segments/squares/rectangles, with their dimensions equal to the square root of the length of the corresponding dimension of the sudoku itself
        /// To ignore the coors m and n, and hit on any cell in the region with specify negative values
        /// </summary>
        /// <param name="m">Row coor of the original cell</param>
        /// <param name="n">Column coor of the original cell</param>
        /// <param name="v">Value to match</param>
        /// <returns>True is no matching value is found. False is there is</returns>
        public bool IsRegionValidCell(int m, int n, int v = -1)
        {
            // If no v was specified, get the current value of the Sudoku cell at [m, n]
            if (v < 0) { v = Grid[m, n]; }

            if (v != 0)
            {
                // Calculate the lengths of a single region, log debug messages if sqrt does not result in a whole integer
                if (!IsSqrtWholeInt(Grid.GetLength(0))) { Debug.WriteLine($"Sudoku: IsCellValidRegion() the square root of the grid dimension M is not a whole number, this might cause problems while determining the length for regions of the sudoku, due to required rounding."); }
                if (!IsSqrtWholeInt(Grid.GetLength(1))) { Debug.WriteLine($"Sudoku: IsCellValidRegion() the square root of the grid dimension N is not a whole number, this might cause problems while determining the length for regions of the sudoku, due to required rounding."); }
                int regLengthM = (int)Math.Sqrt(Grid.GetLength(0));
                int regLengthN = (int)Math.Sqrt(Grid.GetLength(1));

                // Test, TODO remove
                Debug.WriteLine($"Sudoku: IsCellValidRegion(m={m}, n={n}, v={v}) region dimensions are {regLengthM}, {regLengthN}");

                // Go over each cell in the region
                for (int m1 = m / regLengthM * regLengthM; m1 < m / regLengthM * regLengthM + regLengthM; m1++)
                {
                    for (int n1 = n / regLengthN * regLengthN; n1 < n / regLengthN * regLengthN + regLengthN; n1++)
                    {
                        // Test, TODO remove
                        Debug.WriteLine($"Sudoku: IsCellValidRegion(m={m}, n={n}, v={v}) checking cell {m1}, {n1}");

                        if (v == Grid[m1, n1] && m1 != m && n1 != n) { return false; }
                    }
                }
            }

            // Value is 0 or no match was found, return true
            return true;
        }

        #endregion CELL_VALIDATION_METHODS








        // -----------------------------------------------------------------
        // ---------------      Sequence Validation      -------------------
        // -----------------------------------------------------------------
        #region SEQUENCE_VALIDATION_METHODS

        /// <summary>
        /// Check whether the current grid, with all its current values, is valid
        /// </summary>
        /// <returns></returns>
        public bool IsGridValid()
        {
            bool isValid = true;

            // For each row of the grid
            if (isValid)
            {
                for (int m = 0; m < M; m++)
                {
                    if (!IsRowValid(m))
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            // For each column of the grid
            if (isValid)
            {
                for (int n = 0; n < N; n++)
                {
                    if (!IsColumnValid(n))
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            // For each region of the grid
            if (isValid)
            {
                int regDimM = (int)Math.Sqrt(M);
                int regDimN = (int)Math.Sqrt(N);
                for (int regM = 0; regM < M; regM += regDimM)
                {
                    for (int regN = 0; regN < N; regN += regDimN)
                    {
                        if (!IsRegionValid(regM, regN))
                        {
                            isValid = false;
                            break;
                        }
                    }
                }
            }

            return isValid;
        }

        /// <summary>
        /// Check whether all current values in row m are valid, or if there are any conflicts/duplicates
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool IsRowValid(int m)
        {
            if (m < 0 || m >= Grid.GetLength(0)) { return false; }

            bool?[] numbers = new bool?[Grid.GetLength(0)];

            for (int n = 0; n < Grid.GetLength(1); n++)
            {
                // If the number is encountered for the first time, set numbers[n] to true
                // If it is encounter again, numbers[n] now not being null but true, set numbers[n] to false
                if (Grid[m, n] != 0 && numbers[Grid[m, n] - 1] != false) { numbers[n] = numbers[n] == null; }
            }

            bool result = true;
            foreach (bool? num in numbers)
            {
                // If result is not yet false, set it to true if num is either true or null, false if num is false (false means an invalid number was found in the sequence)
                if (result) { result = num != false; }
            }
            return result;
        }

        /// <summary>
        /// Check whether all current values in the column n are valid, or if there are any conflicts/duplicates
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool IsColumnValid(int n)
        {
            if (n < 0 || n >= Grid.GetLength(1)) { return false; }

            bool?[] numbers = new bool?[Grid.GetLength(1)];

            for (int m = 0; m < Grid.GetLength(0); m++)
            {
                // If the number is encountered for the first time, set numbers[n] to true
                // If it is encounter again, numbers[n] now not being null but true, set numbers[n] to false
                if (Grid[m, n] != 0 && numbers[Grid[m, n] - 1] != false) { numbers[n] = numbers[n] == null; }
            }

            bool result = true;
            foreach (bool? num in numbers)
            {
                // If result is not yet false, set it to true if num is either true or null, false if num is false (false means an invalid number was found in the sequence)
                if (result) { result = num != false; }
            }
            return result;
        }

        /// <summary>
        /// Check whether all current values in the region containing the cell [m, n] are valid, or if there are any conflicts/duplicates
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool IsRegionValid(int m, int n)
        {
            if (m < 0 || n < 0 || m >= Grid.GetLength(0) || n > Grid.GetLength(1)) { return false; }

            // Calculate the lengths of a single region, log debug messages if sqrt does not result in a whole integer
            if (!IsSqrtWholeInt(Grid.GetLength(0))) { Debug.WriteLine($"MainPage: IsCellValidRegion() the square root of the grid dimension M is not a whole number, this might cause problems while determining the length for regions of the sudoku, due to required rounding."); }
            if (!IsSqrtWholeInt(Grid.GetLength(1))) { Debug.WriteLine($"MainPage: IsCellValidRegion() the square root of the grid dimension N is not a whole number, this might cause problems while determining the length for regions of the sudoku, due to required rounding."); }
            int x = (int)Math.Sqrt(Grid.GetLength(0));
            int y = (int)Math.Sqrt(Grid.GetLength(1));

            int xOffset = m / x * x;
            int yOffset = n / y * y;

            bool?[] numbers = new bool?[x*y];

            for (int m1 = 0; m1 < x; m1++)
            {
                for (int n1 = 0; n1 < x; n1++)
                {
                    // If the number is encountered for the first time, set numbers[n] to true
                    // If it is encounter again, numbers[n] now not being null but true, set numbers[n] to false
                    if (Grid[m1 + xOffset, n1 + yOffset] != 0 && numbers[Grid[m1 + xOffset, n1 + yOffset] - 1] != false) { numbers[m1 * n1] = numbers[m1 * n1] == null; }

                    // Test, TODO remove
                    Debug.WriteLine($"Sudoku: IsRegionValid(m={m}, n={n}) checking cell {m1}, {n1} with value {Grid[m1 + xOffset, n1 + yOffset]}, is {numbers[Grid[m1 + xOffset, n1 + yOffset]]}");
                }
            }

            bool result = true;
            foreach (bool? num in numbers)
            {
                // If num is false, result is false and the sequence is not valid
                // TODO copy this operation over its equivalents in IsRowValid and IsColumnValid if it works correctly
                if (num == false) { result = false; }
            }
            return result;
        }

        #endregion SEQUENCE_VALIDATION_METHODS








        // -----------------------------------------------------------------
        // -------------------      Helper Methods      --------------------
        // -----------------------------------------------------------------
        #region HELPER_METHODS

        /// <summary>
        /// Is the square root of x a whole number
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private bool IsSqrtWholeInt(int x)
        {
            if (x < 0) { return false; }
            return Math.Sqrt(x) % 1 == 0;
        }

        /// <summary>
        /// Does the grid contain any value larger than 0
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private bool GridContainsValues(int[,] grid)
        {
            for (int m = 0; m < grid.GetLength(0); m++)
            {
                for  (int n = 0; n < grid.GetLength(1); n++)
                {
                    if (grid[m, n] > 0) { return true; }
                }
            }
            return false;
        }

        #endregion HELPER_METHODS

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace Sudoku
{
    class SudokuSolver
    {


        // TODO move homebrew into its own class
        private class HomebrewSolver
        {



            public HomebrewSolver() { }








        }












        /// <summary>
        /// The length of the first dimension of a region
        /// </summary>
        int RegionDim = 0;







        public SudokuSolver() { }








        public int[,] SolveHomeBrew(int[,] grid)
        {
            // Solution grid, copy of given grid
            int[,] solution = (int[,])grid.Clone();

            // If any values of the given grid are invalid, return solution in its initial state, with -1's instead of the 0's off the given grid
            if (grid.GetLength(0) != grid.GetLength(1) || Math.Sqrt(grid.GetLength(0)) % 1 != 0)
            {
                Debug.WriteLine($"SudokuSolver: SolveHomeBrew() requires a grid with 2 equal dimensions. The square root of the length of a dimension must also result in a whole number");
                return solution;
            }

            // Calculate the dimesion(s) of the regions for the grid
            RegionDim = (int)Math.Sqrt(grid.GetLength(0));

            // Bool array representing whether a number is valid for a cell on grid, index of 3rd dimension equals the number itself, minus 1 since going from 1-base of sudoku-numbers to 0-base of array
            bool[,,] validNums = new bool[grid.GetLength(0), grid.GetLength(1), (int)Math.Sqrt(grid.Length)];
            // Set all elements in validNums back to false
            // FillBoolArray(validNums, false);
            // Find all valid numbers for each cell on the grid
            FindValidNumbers(solution, validNums);

            // Bool to check if any cell resulted in a solution this iteration, or at least its valid numbers were altered (reduced)
            bool isAnyCellAltered = false;
            // Iteration count for while loop, to enforce a limit in case no solution is being found
            int iterations = 0;
            int maxIterations = 60;

            // Try to find solutions for the cells on the grid. While the solution grid is not yet full, the previous iteration found at least one solution, and the total iterations has not exceeded the maximum allowed
            do
            {
                // Debug, TODO remove
                PrintSudokuToOutput(solution);

                // Find the cell for which only a single number is valid, enter it into the solution
                isAnyCellAltered = FindSingleValueCells(solution, validNums);

                // If no single values for cells were found
                if (!isAnyCellAltered)
                {
                    Debug.WriteLine($"SudokuSolver: SolveHomeBrew() did not find a cell with a single valid value this iteration, now searching sequences for single value");

                    // Check to see if a number is only valid in a single cell for an entire row, for all rows
                    isAnyCellAltered = FindSingleValuesRows(solution, validNums) ? true : isAnyCellAltered;

                    // Check to see if a number is only valid in a single cell for an entire column, for all columns
                    isAnyCellAltered = FindSingleValuesColumns(solution, validNums) ? true : isAnyCellAltered;

                    // Check to see if a number is only valid in a single cell for an entire region, for all regions
                    isAnyCellAltered = FindSingleValuesRegions(solution, validNums) ? true : isAnyCellAltered;
                }

                // If no single values were found in any row, column or region
                if (!isAnyCellAltered)
                {
                    Debug.WriteLine($"SudokuSolver: SolveHomeBrew() did not find any single values in a row, column or region, now searching for row/column limited numbers");

                    // Find values that are limited to a single row or column within a region, meaning that that number must be entered somewhere in the row or column within the region, and can be entered in the section of the row or column that is outside of the region
                    // Does not alter solution, only removes valid numbers from validNums
                    isAnyCellAltered = FindValuesLimitedToSingleRowOrColumnOfRegions(solution, validNums);
                }

                // If still not a single solution was found
                if (!isAnyCellAltered)
                {
                    Debug.WriteLine($"SudokuSolver: SolveHomeBrew() failed to find any solutions or alterations this iteration. Loop will terminate");
                }

                Debug.WriteLine($"SudokuSolver: SolveHomeBrew() iteration = {iterations}");
                iterations++;

            } while (!IsGridFull(solution) && isAnyCellAltered && iterations < maxIterations);
                        
            return solution;
        }

















        /// <summary>
        /// Get which numbers are valid for the cell grid[m, n]
        /// Returns a bool[], its length equal to a dimension of the grid, and thereby equal to the length of all numbers required
        /// So, element[0] will contain whether the number 1 is allowed or not at the given position [m,n] in grid, [1] contains if 2 is allowed, etc
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public bool[] GetValidNumbers(int m, int n, int[,] grid)
        {
            bool[] validNums = new bool[grid.GetLength(0)];

            if (m > -1 && n > -1 && grid != null)
            {
                bool[] validNumsRow = GetValidNumbersRow(m, grid);
                bool[] validNumsColumn = GetValidNumbersColumn(n, grid);
                bool[] validNumbersRegion = GetValidNumbersRegion(m, n, grid);

                for (int i = 0; i < validNums.Length; i++)
                {
                    validNums[i] = validNumsRow[i] && validNumsColumn[i] && validNumbersRegion[i];
                }
            }

            return validNums;
        }

        /// <summary>
        /// Get which numbers are valid for the row m on grid
        /// Returns a bool[], its length equal to a dimension of the grid, and thereby equal to the length of all numbers required
        /// So, element[0] will contain whether the number 1 is allowed or not at the given position [m,n] in grid, [1] contains if 2 is allowed, etc
        /// </summary>
        /// <param name="m"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        private bool[] GetValidNumbersRow(int m, int[,] grid)
        {
            // Create an bool[] filled with true, starting assumption is all numbers are valid
            bool[] validNums = new bool[grid.GetLength(1)];
            Array.Fill(validNums, true);

            // Debug, TODO remove
            string validNumsString = "";

            // Go over the entire row, setting the value in validNums, at the index equal to the value in the cell [m,n] of the grid, minus one, to false
            for (int n = 0; n < grid.GetLength(1); n++)
            {
                // If the value in the cell is not zero. Zero is always valid since it represents an empty cell, and should be ignored
                if (grid[m, n] > 0)
                {
                    // Set its bool to false since it is already present in the row
                    validNums[grid[m, n] - 1] = false;

                    // Debug, TODO remove
                    validNumsString += grid[m, n] + " ";
                }                
            }

            // Debug, TODO remove
            // Debug.WriteLine($"SudokuSolver: GetValidNumbersRow() found for row {m}, not {validNumsString}");

            return validNums;
        }

        /// <summary>
        /// Get which numbers are valid for the column n on grid
        /// Returns a bool[], its length equal to a dimension of the grid, and thereby equal to the length of all numbers required
        /// So, element[0] will contain whether the number 1 is allowed or not at the given position [m,n] in grid, [1] contains if 2 is allowed, etc
        /// </summary>
        /// <param name="n"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        private bool[] GetValidNumbersColumn(int n, int[,] grid)
        {
            // Create an bool[] filled with true, starting assumption is all numbers are valid
            bool[] validNums = new bool[grid.GetLength(0)];
            Array.Fill(validNums, true);

            // Debug, TODO remove
            string validNumsString = "";

            // Go over the entire column, setting the value in validNums, at the index equal to the value in the cell [m,n] of the grid, minus one, to false
            for (int m = 0; m < grid.GetLength(0); m++)
            {
                // If the value in the cell is not zero. Zero is always valid since it represents an empty cell, and should be ignored
                if (grid[m, n] > 0)
                {
                    // Set its bool to false since it is already present in the column
                    validNums[grid[m, n] - 1] = false;

                    // Debug, TODO remove
                    validNumsString += grid[m, n] + " ";
                }
            }

            // Debug, TODO remove
            // Debug.WriteLine($"SudokuSolver: GetValidNumbersColumn() found for column {n}, not {validNumsString}");

            return validNums;
        }

        /// <summary>
        /// Get which numbers are valid for the region on the grid containing the cell [m, n]
        /// Returns a bool[], its length equal to a dimension of the grid, and thereby equal to the length of all numbers required
        /// So, element[0] will contain whether the number 1 is allowed or not at the given position [m,n] in grid, [1] contains if 2 is allowed, etc
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        private bool[] GetValidNumbersRegion(int m, int n, int[,] grid)
        {
            // Create an bool[] filled with true, starting assumption is all numbers are valid
            bool[] validNums = new bool[(int)Math.Sqrt(grid.Length)];
            Array.Fill(validNums, true);

            // Debug, TODO remove
            string validNumsString = "";

            // Calculate the offset from [0, 0] for the region the given cell [m, n] is in
            int regMOffset = m / RegionDim * RegionDim;
            int regNOffset = n / RegionDim * RegionDim;

            // Go over the entire region, setting the value in validNums, at the index equal to the value in the cell of the grid, minus one, to false
            for (int m1 = regMOffset; m1 < RegionDim + regMOffset; m1++)
            {
                for (int n1 = regNOffset; n1 < RegionDim + regNOffset; n1++)
                {
                    // Debug, TODO remove
                    // Debug.WriteLine($"SudokuSolver: GetValidNumbersRegion() looking at cell [{m1}, {n1}]");

                    // If the value in the cell is not zero. Zero is always valid since it represents an empty cell, and should be ignored
                    if (grid[m1, n1] > 0)
                    {
                        // Set its bool to false since it is already present in the region
                        validNums[grid[m1, n1] - 1] = false;

                        // Debug, TODO remove
                        validNumsString += grid[m1, n1] + " ";
                    }
                }
            }

            // Debug, TODO remove
            // Debug.WriteLine($"SudokuSolver: GetValidNumbersRegion() found for region around [{m},{n}], not {validNumsString}");

            return validNums;
        }









        // TODO remove
        /// <summary>
        /// Find all values that occure only once in a row, meaning that that is the only place they can be entered for the solution
        /// Looks at all rows, since no row has any weight when considering the valid values of another
        /// Returns a collection of int[]'s, one int[] for each found value, with the int[] containing the data {m, n, value}
        /// </summary>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private List<int[]> FindSingleValuesRowsOld(bool[,,] validNums)
        {
            // Collection that will contain all found single values as int[] with {m, n, value}
            List<int[]> singleValues = new List<int[]>();

            // Bool?[] to search for single occurance values
            bool?[] isSingleNum = new bool?[validNums.GetLength(2)];

            // For each row
            for (int mRow = 0; mRow < validNums.GetLength(0); mRow++)
            {
                // For each cell in the row
                for (int nRow = 0; nRow < validNums.GetLength(1); nRow++)
                {
                    // Go over all available numbers in validNums (index = number)
                    for (int num = 0; num < validNums.GetLength(2); num++)
                    {
                        // If the current number is valid
                        if (validNums[mRow, nRow, num])
                        {
                            // Set isSingleNum[num] on the first encounter to true, and on any subsequent encounters to false
                            isSingleNum[num] = isSingleNum[num] == null;
                        }
                    }
                }

                // For the bools in isSingleNum
                for (int sinNum = 0; sinNum < isSingleNum.Length; sinNum++)
                {
                    // If the number occured only once
                    if (isSingleNum[sinNum] == true)
                    {
                        // Find its location in validNums
                        for (int indexRow = 0; indexRow < validNums.GetLength(2); indexRow++)
                        {
                            if (validNums[mRow, indexRow, sinNum])
                            {
                                // Add its info to the singleValues collection
                                singleValues.Add(new int[] { mRow, indexRow, sinNum + 1 });
                                Debug.WriteLine($"SudokuSolver: FindSingleValuesRowsOld() found [{mRow}, {indexRow}] = {sinNum + 1}");
                            }
                        }
                    }
                }
            }

            return singleValues;
        }



        // TODO remove
        /// <summary>
        /// Find all values that occure only once in a column, meaning that that is the only place they can be entered for the solution
        /// Looks at all columns, since no column has any weight when considering the valid values of another
        /// Returns a collection of int[]'s, one int[] for each found value, with the int[] containing the data {m, n, value}
        /// </summary>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private List<int[]> FindSingleValuesColumnsOld(bool[,,] validNums)
        {
            // Collection that will contain all found single values as int[] with {m, n, value}
            List<int[]> singleValues = new List<int[]>();

            // Bool?[] to search for single occurance values
            bool?[] isSingleNum = new bool?[validNums.GetLength(2)];

            // For each column
            for (int nColumn = 0; nColumn < validNums.GetLength(1); nColumn++)
            {
                // For each cell in the column
                for (int mColumn = 0; mColumn < validNums.GetLength(0); mColumn++)
                {
                    // Go over all available numbers in validNums (index = number)
                    for (int num = 0; num < validNums.GetLength(2); num++)
                    {
                        // If the current number is valid
                        if (validNums[mColumn, nColumn, num])
                        {
                            // Set isSingleNum[num] on the first encounter to true, and on any subsequent encounters to false
                            isSingleNum[num] = isSingleNum[num] == null;
                        }
                    }
                }

                // For the bools in isSingleNum
                for (int sinNum = 0; sinNum < isSingleNum.Length; sinNum++)
                {
                    // If the number occured only once
                    if (isSingleNum[sinNum] == true)
                    {
                        // Find its location in validNums
                        for (int indexColumn = 0; indexColumn < validNums.GetLength(2); indexColumn++)
                        {
                            if (validNums[indexColumn, nColumn, sinNum])
                            {
                                // Add its info to the singleValues collection
                                singleValues.Add(new int[] { indexColumn, nColumn, sinNum + 1 });
                                Debug.WriteLine($"SudokuSolver: FindSingleValuesColumnsOld() found [{indexColumn}, {nColumn}] = {sinNum + 1}");
                            }
                        }
                    }
                }
            }

            return singleValues;
        }



        // TODO remove
        /// <summary>
        /// Find all values that occure only once in a region, meaning that that is the only place they can be entered for the solution
        /// Looks at all region, since no region has any weight when considering the valid values of another
        /// Returns a collection of int[]'s, one int[] for each found value, with the int[] containing the data {m, n, value}
        /// </summary>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private List<int[]> FindSingleValuesRegionsOld(bool[,,] validNums)
        {
            // Collection that will contain all found single values as int[] with {m, n, value}
            List<int[]> singleValues = new List<int[]>();

            // Bool?[] to search for single occurance values
            bool?[] isSingleNum = new bool?[validNums.GetLength(2)];

            // For each region
            for (int mReg = 0; mReg < validNums.GetLength(0); mReg += RegionDim)
            {
                for (int nReg = 0; nReg < validNums.GetLength(0); nReg += RegionDim)
                {

                    // For each cell in the region
                    for (int m = mReg; m < mReg + RegionDim; m++)
                    {
                        for (int n = nReg; n < nReg + RegionDim; n++)
                        {
                            // Go over all available numbers in validNums (index = number)
                            for (int num = 0; num < validNums.GetLength(2); num++)
                            {
                                // If the current number is valid
                                if (validNums[m, n, num])
                                {
                                    // Set isSingleNum[num] on the first encounter to true, and on any subsequent encounters to false
                                    isSingleNum[num] = isSingleNum[num] == null;
                                }
                            }
                        }
                    }

                    // For the bools in isSingleNum
                    for (int sinNum = 0; sinNum < isSingleNum.Length; sinNum++)
                    {
                        // If the number occured only once
                        if (isSingleNum[sinNum] == true)
                        {
                            // Find its location in validNums
                            for (int mRegIndex = mReg; mRegIndex < mReg + RegionDim; mRegIndex++)
                            {
                                for (int nRegIndex = nReg; nRegIndex < nReg + RegionDim; nRegIndex++)
                                {
                                    if (validNums[mRegIndex, nRegIndex, sinNum])
                                    {
                                        // Add its info to the singleValues collection
                                        singleValues.Add(new int[] { mRegIndex, nRegIndex, sinNum + 1 });
                                        Debug.WriteLine($"SudokuSolver: FindSingleValuesRegionsOld() found [{mRegIndex}, {nRegIndex}] = {sinNum + 1}");
                                    }
                                }                                
                            }
                        }
                    }
                }
            }

            return singleValues;
        }




















        /// <summary>
        /// Find the valid numbers for each cell on the specified solution grid and write them to validNums
        /// If a cell only has a single valid number, enter it into the cell of solution (and remove it from validNums)
        /// Returns true if such a single valid number was found, else false
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private bool FindValidNumbersAndSingleValueCells(int[,] solution, bool[,,] validNums)
        {
            // Is any solution found by this method
            bool isAnyCellSolved = false;

            // For every row of the sudoku
            for (int m = 0; m < solution.GetLength(0); m++)
            {
                // For every column, on every row. ie every cell
                for (int n = 0; n < solution.GetLength(1); n++)
                {
                    // If no single valid number has yet been determined for this cell, look for said number
                    if (solution[m, n] == 0)
                    {
                        // Debug statement, TODO remove
                        Debug.WriteLine($"SudokuSolver: FindValidNumbers() looking at cell [{m}, {n}]");

                        // Get all valid numbers for the current cell
                        bool[] validNumsCell = GetValidNumbers(m, n, solution);

                        // Go over the valid numbers
                        bool? isSingle = null;
                        for (int i = 0; i < validNumsCell.Length; i++)
                        {
                            // If a number is valid
                            if (validNumsCell[i])
                            {
                                // If this number is encountered for the first time, isSingle will be null, and as a result isSingle be changed to true
                                // If the number has been encountered already, isSingle will no longer be null, but true, and isSingle will be set to false
                                isSingle = isSingle == null;
                            }

                            // Also, store the valid number(s) for this cell in the grid-wide array, for later reference
                            validNums[m, n, i] = validNumsCell[i];
                        }

                        // If not a single valid number was found for the cell, set its value to -1 to signify this failure (should not happen, unless the grid does not have a solution, or something else went wrong)
                        if (isSingle == null)
                        {
                            Debug.WriteLine($"SudokuSolver: FindValidNumbers() could not find a solution for cell [{m}, {n}] !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            solution[m, n] = -1;
                        }
                        // If a single value was found for the cell
                        else if (isSingle == true)
                        {
                            // For each number for the current cell in validNums, see if it is the valid number
                            for (int num = 0; num < validNums.GetLength(2); num++)
                            {
                                // If it is the valid number
                                if (validNums[m, n, num])
                                {
                                    Debug.WriteLine($"SudokuSolver: FindValidNumbers() solution found! cell [{m}, {n}] = {num + 1}");

                                    EnterSolution(m, n, num, solution, validNums);

                                    // Log that at least one cell was solved in this iteration
                                    isAnyCellSolved = true;

                                    // The number was found, the remaining numbers will all be false, so break
                                    break;
                                }
                            }
                        }
                        // Else multiple solutions were found
                        else
                        {
                            // Debug, TODO remove
                            string validNumsString = "";
                            for (int debugI = 0; debugI < validNumsCell.Length; debugI++)
                            {
                                validNumsString += $"{debugI + 1}={validNumsCell[debugI]} ";
                            }
                            Debug.WriteLine($"SudokuSolver: FindValidNumbers() cell[{m}, {n}], possible solution are; {validNumsString}");
                        }
                    }
                    // Else this cell contains a starting value, or a solution was already found for this cell. Either way, no other number is valid
                    else
                    {
                        // Set all numbers at the cell [m, n] to false
                        for (int x = 0; x < validNums.GetLength(2); x++)
                        {
                            validNums[m, n, x] = false;
                        }
                    }
                }
            }

            return isAnyCellSolved;
        }


        /// <summary>
        /// Find all valid numbers for each cell on the specified solution grid and write them to validNums
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="validNums"></param>
        private void FindValidNumbers(int[,] solution, bool[,,] validNums)
        {
            // For every row of the sudoku
            for (int m = 0; m < solution.GetLength(0); m++)
            {
                // For every column, on every row. ie every cell
                for (int n = 0; n < solution.GetLength(1); n++)
                {
                    // If no single valid number has yet been determined for this cell, look for said number
                    if (solution[m, n] == 0)
                    {
                        // Debug statement, TODO remove
                        Debug.WriteLine($"SudokuSolver: FindValidNumbers() looking at cell [{m}, {n}]");

                        // Get all valid numbers for the current cell
                        bool[] validNumsCell = GetValidNumbers(m, n, solution);

                        // For all bools in validNumCell
                        for (int i = 0; i < validNumsCell.Length; i++)
                        {
                            // Store the valid number(s) for this cell in the grid-wide array validNums
                            validNums[m, n, i] = validNumsCell[i];
                        }

                        // Debug, TODO remove
                        string validNumsString = "";
                        for (int debugI = 0; debugI < validNumsCell.Length; debugI++)
                        {
                            validNumsString += $"{debugI + 1}={validNumsCell[debugI]} ";
                        }
                        Debug.WriteLine($"SudokuSolver: FindValidNumbers() cell[{m},{n}], possible solution are; {validNumsString}");
                    }
                    // Else this cell contains a starting value, or a solution was already found for this cell. Either way, no other number is valid
                    else
                    {
                        Debug.WriteLine($"SudokuSolver: FindValidNumbers() cell[{m},{n}] already contains a solution value, {solution[m, n]}");

                        // Set all numbers for the cell [m, n] to false
                        for (int x = 0; x < validNums.GetLength(2); x++)
                        {
                            validNums[m, n, x] = false;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Find cells that have only one valid number, meaning that number is the only possible solution
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private bool FindSingleValueCells(int[,] solution, bool[,,] validNums)
        {
            // Is any solution found by this method
            bool isAnyCellSolved = false;

            // For every row of the sudoku
            for (int m = 0; m < solution.GetLength(0); m++)
            {
                // For every column, on every row. ie every cell
                for (int n = 0; n < solution.GetLength(1); n++)
                {
                    // If no number has yet been determined for this cell, look trough validNums to see if the cell only has a single valid number
                    if (solution[m, n] == 0)
                    {
                        // Go over the valid numbers
                        bool? isSingle = null;
                        for (int o = 0; o < validNums.GetLength(2); o++)
                        {
                            // If a number is valid
                            if (validNums[m, n, o])
                            {
                                // If this number is encountered for the first time, isSingle will be null, and as a result will be changed to true
                                // If the number has been encountered already, isSingle will no longer be null, but true, and isSingle will be set to false
                                isSingle = isSingle == null;
                            }                            
                        }

                        // If not a single valid number was found for the cell, set its value to -1 to signify this failure (should not happen, unless the grid does not have a solution, or something else went wrong)
                        if (isSingle == null)
                        {
                            Debug.WriteLine($"SudokuSolver: FindSingleValueCells() could not find a solution for cell [{m}, {n}] !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            solution[m, n] = -1;
                        }
                        // If a single value was found for the cell
                        else if (isSingle == true)
                        {
                            // For each number for the current cell in validNums, see if it is the valid number
                            for (int num = 0; num < validNums.GetLength(2); num++)
                            {
                                // If it is the valid number
                                if (validNums[m, n, num])
                                {
                                    Debug.WriteLine($"SudokuSolver: FindSingleValueCells() solution found! cell [{m}, {n}] = {num + 1}");

                                    EnterSolution(m, n, num, solution, validNums);

                                    // Log that at least one cell was solved in this iteration
                                    isAnyCellSolved = true;

                                    // The number was found, the remaining numbers will all be false, so break
                                    break;
                                }
                            }
                        }
                        // Else multiple solutions were found
                        else
                        {
                            // Debug, TODO remove
                            Debug.WriteLine($"SudokuSolver: FindSingleValueCells() cell[{m}, {n}] has multiple possible solutions");                            
                            string validNumsString = "";
                            for (int debugI = 0; debugI < validNums.GetLength(2); debugI++)
                            {
                                validNumsString += $"{debugI + 1}={validNums[m, n, debugI]} ";
                            }
                            Debug.WriteLine($"SudokuSolver: FindSingleValueCells() cell[{m}, {n}], possible solution are; {validNumsString}");
                        }
                    }
                }
            }
            
            return isAnyCellSolved;
        }



        /// <summary>
        /// Find all values that occure only once in a row, meaning that that is the only place they can be entered for the solution
        /// If such a value is found, enter it into the solution and remove it from validNums
        /// Looks at all rows, since no row has any weight when considering the valid values of another
        /// </summary>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private bool FindSingleValuesRows(int[,] solution, bool[,,] validNums)
        {
            // Is any solution found by this method
            bool isAnyCellSolved = false;

            // Bool?[] to search for single occurance values
            bool?[] isSingleNum = new bool?[validNums.GetLength(2)];

            // For each row
            for (int m = 0; m < validNums.GetLength(0); m++)
            {
                // For each cell in the row
                for (int n0 = 0; n0 < validNums.GetLength(1); n0++)
                {
                    // Go over all available numbers in validNums (index = number)
                    for (int num = 0; num < validNums.GetLength(2); num++)
                    {
                        // If the current number is valid
                        if (validNums[m, n0, num])
                        {
                            // Set isSingleNum[num] on the first encounter to true, and on any subsequent encounter(s) to false
                            isSingleNum[num] = isSingleNum[num] == null;
                        }
                    }
                }

                // For all bools in isSingleNum
                for (int num = 0; num < isSingleNum.Length; num++)
                {
                    // If the number occured only once
                    if (isSingleNum[num] == true)
                    {
                        // Find its location in validNums
                        for (int n1 = 0; n1 < validNums.GetLength(2); n1++)
                        {
                            if (validNums[m, n1, num])
                            {
                                Debug.WriteLine($"SudokuSolver: FindSingleValuesRows() solution found! cell [{m}, {n1}] = {num + 1}");

                                EnterSolution(m, n1, num, solution, validNums);

                                // Log that at least one cell was solved in this iteration
                                isAnyCellSolved = true;
                            }
                        }
                    }
                }
            }

            return isAnyCellSolved;
        }


        /// <summary>
        /// Find all values that occure only once in a column, meaning that that is the only place they can be entered for the solution
        /// If such a value is found, enter it into the solution and remove it from validNums
        /// Looks at all columns, since no column has any weight when considering the valid values of another
        /// </summary>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private bool FindSingleValuesColumns(int[,] solution, bool[,,] validNums)
        {
            // Is any solution found by this method
            bool isAnyCellSolved = false;

            // Bool?[] to search for single occurance values
            bool?[] isSingleNum = new bool?[validNums.GetLength(2)];

            // For each column
            for (int n = 0; n < validNums.GetLength(1); n++)
            {
                // For each cell in the column
                for (int m0 = 0; m0 < validNums.GetLength(0); m0++)
                {
                    // Go over all available numbers in validNums (index = number)
                    for (int num = 0; num < validNums.GetLength(2); num++)
                    {
                        // If the current number is valid
                        if (validNums[m0, n, num])
                        {
                            // Set isSingleNum[num] on the first encounter to true, and on any subsequent encounters to false
                            isSingleNum[num] = isSingleNum[num] == null;
                        }
                    }
                }

                // For the bools in isSingleNum
                for (int num = 0; num < isSingleNum.Length; num++)
                {
                    // If the number occured only once
                    if (isSingleNum[num] == true)
                    {
                        // Find its location in validNums
                        for (int m1 = 0; m1 < validNums.GetLength(2); m1++)
                        {
                            if (validNums[m1, n, num])
                            {
                                Debug.WriteLine($"SudokuSolver: FindSingleValuesColumns() solution found! cell [{m1}, {n}] = {num + 1}");

                                EnterSolution(m1, n, num, solution, validNums);

                                // Log that at least one cell was solved in this iteration
                                isAnyCellSolved = true;
                            }
                        }
                    }
                }
            }

            return isAnyCellSolved;
        }

        /// <summary>
        /// Find all values that occure only once in a region, meaning that that is the only place they can be entered for the solution
        /// If such a value is found, enter it into the solution and remove it from validNums
        /// Looks at all region, since no region has any weight when considering the valid values of another
        /// </summary>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private bool FindSingleValuesRegions(int[,] solution, bool[,,] validNums)
        {
            // Is any solution found by this method
            bool isAnyCellSolved = false;

            // Bool?[] to search for single occurance values
            bool?[] isSingleNum = new bool?[validNums.GetLength(2)];

            // For each region
            for (int mReg = 0; mReg < validNums.GetLength(0); mReg += RegionDim)
            {
                for (int nReg = 0; nReg < validNums.GetLength(0); nReg += RegionDim)
                {

                    // Clear any values for isSingleNum
                    isSingleNum = new bool?[validNums.GetLength(2)];

                    // For all available numbers in validNums (index = number)
                    for (int num0 = 0; num0 < validNums.GetLength(2); num0++)
                    {
                        // For each cell in the region
                        for (int m0 = mReg; m0 < mReg + RegionDim; m0++)
                        {
                            for (int n0 = nReg; n0 < nReg + RegionDim; n0++)
                            {
                                // If the current number is valid for this cell
                                if (validNums[m0, n0, num0])
                                {
                                    Debug.WriteLine($"SudokuSolver: FindSingleValuesRegions() found valid number {num0 + 1} for [{m0}, {n0}]");

                                    // Set isSingleNum[num] on the first encounter to true, and on any subsequent encounters to false
                                    isSingleNum[num0] = isSingleNum[num0] == null;
                                }
                            }
                        }
                    }
                        

                    // For the bools in isSingleNum
                    for (int num1 = 0; num1 < isSingleNum.Length; num1++)
                    {
                        // If the number occured only once
                        if (isSingleNum[num1] == true)
                        {
                            // Find its location in validNums
                            for (int m1 = mReg; m1 < mReg + RegionDim; m1++)
                            {
                                for (int n1 = nReg; n1 < nReg + RegionDim; n1++)
                                {
                                    // If the current number is valid
                                    if (validNums[m1, n1, num1])
                                    {
                                        Debug.WriteLine($"SudokuSolver: FindSingleValuesRegions() solution found! cell [{m1}, {n1}] = {num1 + 1}");

                                        EnterSolution(m1, n1, num1, solution, validNums);

                                        // Log that at least one cell was solved in this iteration
                                        isAnyCellSolved = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return isAnyCellSolved;
        }


        /// <summary>
        /// Look at every region and check if a valid number in that region is limited to a single row or column, occuring multiple times in only that row or column and nowhere else in the region
        /// Meaning that the number must be entered somewhere in that row or column in the region, and can not occur elsewhere in the row or column outside of the region
        /// Looks at each region in turn, and removes the valid numbers elsewhere if a 'single row/column' number is found
        /// Returns true if any such number was found, meaning alterations were made to validNums to reflect the found constraints
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private bool FindValuesLimitedToSingleRowOrColumnOfRegions(int[,] solution, bool[,,] validNums)
        {
            // Method is only capable of handeling the standard 9x9 grid (TODO verify is still true)
            if (solution.GetLength(0) != 9 || solution.GetLength(1) != 9)
            {
                Debug.WriteLine($"SudokuSolver: FindRepeatingValuesinRowOrColumnOfRegions() given solution grid must be 9x9 for method to be able to process it.");
                Debug.WriteLine($"SudokuSolver: FindRepeatingValuesinRowOrColumnOfRegions() returning early !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                return false;
            }

            // Is any alteration for validNums found by this method
            bool isAnyAlterationMade = false;

            // Look at every region and check if a number in that region has to be within a single row or column in that region
            // This would mean that that number can not appear elsewhere in the same row or column outside of that region, eliminating those possibilities

            bool[,] regionNumLocs = new bool[RegionDim, RegionDim];


            // For each region
            for (int m0 = 0; m0 < solution.GetLength(0); m0 += RegionDim)
            {
                for (int n0 = 0; n0 < solution.GetLength(1); n0 += RegionDim)
                {
                    // For each number that could possibly be on the grid
                    for (int num = 0; num < validNums.GetLength(2); num++)
                    {
                        int[] firstEncounterIndex = new int[] { -1, -1 };

                        int[] rowIndex = new int[] { -1, -1 };
                        int[] columnIndex = new int[] { -1, -1 };


                        // Debug, TODO remove
                        string debugRow0 = "";
                        string debugRow1 = "";
                        string debugRow2 = "";


                        for (int m1 = 0; m1 < RegionDim; m1++)
                        {
                            for (int n1 = 0; n1 < RegionDim; n1++)
                            {
                                // If the number is valid for this cell in the row
                                if (validNums[m1 + m0, n1 + n0, num])
                                {
                                    // Debug, TODO remove
                                    if (solution[m1 + m0, n1 + n0] != 0) { Debug.WriteLine($"SudokuSolver: FindRepeatingValuesinRowOrColumnOfRegions() looking at a number that is already solved! [{m1 + m0}, {n1 + n0}] = {solution[m1 + m0, n1 + n0]} !!!!!!!!!!!!!!!!!!!!!!!!!!"); }

                                    /* OLD version
                                    // If the number has been encountered before
                                    if (isEncounteredRow)
                                    {
                                        // If the stored rowIndex is larger or equal to the current index of row, meaning this is the first row this number is valid for in this region, set rowIndex to the current row
                                        // Else set it to -1, denoting that this number is valid in multiple rows
                                        rowIndex = (m1 + m0 <= rowIndex) ? m1 + m0 : -1;
                                    }
                                    else { isEncounteredRow = true; }
                                    */

                                    // If the number has been encountered before, on this row, log the coordinates
                                    if (firstEncounterIndex[0] == m1)
                                    {
                                        rowIndex[0] = m1 + m0;
                                        rowIndex[1] = n1 + n0;
                                    }
                                    // If the number has not been encountered before, store the row-index of this cell
                                    else if (firstEncounterIndex[0] < 0) { firstEncounterIndex[0] = m1; }
                                    // Else the number is not present only in a single row
                                    else { rowIndex[0] = -1; rowIndex[1] = -1; }
                                }

                                // If the number is valid for this cell in the column
                                if (validNums[n1 + m0, m1 + n0, num])
                                {
                                    // Debug, TODO remove
                                    if (solution[n1 + m0, m1 + n0] != 0) { Debug.WriteLine($"SudokuSolver: FindRepeatingValuesinRowOrColumnOfRegions() looking at a number that is already solved! [{n1 + m0}, {m1 + n0}] = {solution[n1 + m0, m1 + n0]} !!!!!!!!!!!!!!!!!!!!!!!!!!"); }

                                    /* OLD version
                                    if (isEncounteredColumn)
                                    {
                                        // If the stored columnIndex is larger or equal to the current index of column, meaning this is the first column this number is valid for in this region, set columnIndex to the current column
                                        // Else set it to -1, denoting that this number is valid in multiple columns
                                        columnIndex = (m1 + n0 <= columnIndex) ? m1 + n0 : -1;
                                    }
                                    else { isEncounteredColumn = true; }
                                    */

                                    // If the number has been encountered before, on this column, log the coordinates
                                    if (firstEncounterIndex[1] == m1)
                                    {
                                        columnIndex[0] = n1 + m0;
                                        columnIndex[1] = m1 + n0;
                                    }
                                    // If the number has not been encountered before, store the column-index of this cell
                                    else if (firstEncounterIndex[1] < 0) { firstEncounterIndex[1] = m1; }
                                    // Else the number is not present only in a single column
                                    else { columnIndex[0] = -1; columnIndex[1] = -1; }
                                }

                                // Debug, TODO remove
                                switch (m1)
                                {
                                    case 0: { debugRow0 += (validNums[m1 + m0, n1 + n0, num] ? num + 1 : 0) + " "; break; }
                                    case 1: { debugRow1 += (validNums[m1 + m0, n1 + n0, num] ? num + 1 : 0) + " "; break; }
                                    case 2: { debugRow2 += (validNums[m1 + m0, n1 + n0, num] ? num + 1 : 0) + " "; break; }
                                    default: { break; }
                                }

                            }
                        }

                        // Debug, TODO remove
                        Debug.WriteLine($"SudokuSolver: FindValuesLimitedToSingleRowOrColumnOfRegions() checking lines for {num + 1}, starting from [{m0}, {n0}]");
                        Debug.WriteLine("SudokuSolver: FindValuesLimitedToSingleRowOrColumnOfRegions() " + debugRow0);
                        Debug.WriteLine("SudokuSolver: FindValuesLimitedToSingleRowOrColumnOfRegions() " + debugRow1);
                        Debug.WriteLine("SudokuSolver: FindValuesLimitedToSingleRowOrColumnOfRegions() " + debugRow2);
                        if (rowIndex[0] > -1) { Debug.WriteLine($"SudokuSolver: FindValuesLimitedToSingleRowOrColumnOfRegions() found single row for {num + 1} at [{rowIndex[0]}, {rowIndex[1]}]"); }
                        if (columnIndex[0] > -1) { Debug.WriteLine($"SudokuSolver: FindValuesLimitedToSingleRowOrColumnOfRegions() found single column for {num + 1} at [{columnIndex[0]}, {columnIndex[1]}]"); }


                        // If a single row was found
                        if (rowIndex[0] > -1)
                        {
                            RemoveValidNumbersFromRowExcludingLocalRegion(rowIndex[0], rowIndex[1], num, validNums);
                            isAnyAlterationMade = true;
                        }
                        else if (columnIndex[0] > -1)
                        {
                            RemoveValidNumbersFromColumnExcludingLocalRegion(columnIndex[0], columnIndex[1], num, validNums);
                            isAnyAlterationMade = true;
                        }
                    }


                }
            }

            return isAnyAlterationMade;
        }














        /// <summary>
        /// Set the number 'number' to not be a valid number on all the cells in the same row, column and region as the cell at [m, n]
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="number"></param>
        /// <param name="validNums"></param>
        private void RemoveValidNumbersFromCellAndConnectedSequences(int m, int n, int number, bool[,,] validNums)
        {
            Debug.WriteLine($"SudokuSolver: RemoveValidNumbersFromConnectedSequences() deleting {number + 1} from cells connected to [{m}, {n}]");

            // Remove all other valid numbers from the cell, and the found number from all connected sequences (assumes that the grid dimensions are all of equal length! eg 9x9)
            for (int x = 0; x < validNums.GetLength(0); x++)
            {
                validNums[m, n, x] = false;
                validNums[x, n, number] = false;
                validNums[m, x, number] = false;
                validNums[m / RegionDim * RegionDim + x / RegionDim, n / RegionDim * RegionDim + x % RegionDim, number] = false;
            }
        }

        /// <summary>
        /// Set the number 'number' to not be a valid number on all the cells in same the row, excluding those cells in the same region as cell [m, n]
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="number"></param>
        /// <param name="validNums"></param>
        private void RemoveValidNumbersFromRowExcludingLocalRegion(int m, int n, int number, bool[,,] validNums)
        {
            Debug.WriteLine($"SudokuSolver: RemoveValidNumbersFromRowExcludingLocalRegion() deleting {number + 1} from cells connected to [{m}, {n}]");

            for (int i = 0; i < validNums.GetLength(0); i++)
            {
                // If the current n-coor (i) is not in the local region
                if (n / RegionDim != i / RegionDim)
                {
                    Debug.WriteLine($"SudokuSolver: RemoveValidNumbersFromRowExcludingLocalRegion() deleting {number + 1} from [{m}, {i}]");

                    // Set the number to false for this cell
                    validNums[m, i, number] = false;
                }
            }
        }

        /// <summary>
        /// Set the number 'number' to not be a valid number on all the cells in same the column, excluding those cells in the same region as cell [m, n]
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="number"></param>
        /// <param name="validNums"></param>
        private void RemoveValidNumbersFromColumnExcludingLocalRegion(int m, int n, int number, bool[,,] validNums)
        {
            Debug.WriteLine($"SudokuSolver: RemoveValidNumbersFromColumnExcludingLocalRegion() deleting {number + 1} from cells connected to [{m}, {n}]");

            for (int i = 0; i < validNums.GetLength(0); i++)
            {
                // If the current m-coor (i) is not in the local region
                if (m / RegionDim != i / RegionDim)
                {
                    Debug.WriteLine($"SudokuSolver: RemoveValidNumbersFromColumnExcludingLocalRegion() deleting {number + 1} from [{i}, {n}]");

                    // Set the number to false for this cell
                    validNums[i, n, number] = false;
                }
            }
        }









        private void EnterSolution(int m, int n, int num, int[,] solution, bool[,,] validNums, string debugMessage = "")
        {
            Debug.WriteLine($"SudokuSolver: EnterSolution() solution entered, [{m}, {n}] =  {num + 1}");
            if (debugMessage != "") { Debug.WriteLine($"SudokuSolver: EnterSolution() debugMessage {debugMessage}"); }
            solution[m, n] = num + 1;
            RemoveValidNumbersFromCellAndConnectedSequences(m, n, num, validNums);
        }














        /// <summary>
        /// Fill the specified 2-D boolean array with the desired state
        /// </summary>
        /// <param name="array"></param>
        /// <param name="state"></param>
        private static void FillBoolArray(bool[,] array, bool state)
        {
            for (int m = 0; m < array.GetLength(0); m++)
            {
                for (int n = 0; n < array.GetLength(1); n++)
                {
                    array[m, n] = state;
                }
            }
        }


        /// <summary>
        /// Fill the specified 3-D boolean array with the desired state
        /// </summary>
        /// <param name="array"></param>
        /// <param name="state"></param>
        private static void FillBoolArray(bool[,,] array, bool state)
        {
            for (int m = 0; m < array.GetLength(0); m++)
            {
                for (int n = 0; n < array.GetLength(1); n++)
                {
                    for (int o = 0; o < array.GetLength(2); o++)
                    {
                        array[m, n, o] = state;
                    }
                }
            }
        }


        /// <summary>
        /// Applies the found single values to the solution grid and removes them from validNums
        /// </summary>
        /// <param name="singleValues"></param>
        /// <param name="solution"></param>
        /// <param name="validNums"></param>
        private void ApplyFoundSingleValues(List<int[]> singleValues, int[,] solution, bool[,,] validNums)
        {
            foreach (int[] singleValue in singleValues)
            {
                Debug.WriteLine($"SudokuSolver: ApplyFoundSingleValues() solution found! [{singleValue[0]}, {singleValue[1]}] = {singleValue[2]}");
                // Add the value to the solution
                solution[singleValue[0], singleValue[1]] = singleValue[2];
                // Remove it from validNums, as to avoid duplicates when looking at columns or regions next
                //validNums[singleValue[0], singleValue[1], singleValue[2] - 1] = false;
            }
        }


        /// <summary>
        /// Check whether the grid is full or if it contains any empty cell, ie cells containing 0
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>True if no cell on the grid is 0, false if there is</returns>
        private bool IsGridFull(int[,] grid)
        {
            foreach(int value in grid)
            {
                if (value == 0) return false;
            }
            return true;
        }

        private void PrintSudokuToOutput(int[,] grid)
        {
            Debug.WriteLine($"SudokuSolver: PrintSudokuToOutput() printing grid");
            string s = "";
            for (int m = 0; m < grid.GetLength(0); m++)
            {
                s = "";
                for (int n = 0; n < grid.GetLength(1); n++)
                {
                    s += grid[m, n] + " ";
                }
                Debug.WriteLine($"SudokuSolver: PrintSudokuToOutput() {s}");
            }
        }







    }
}

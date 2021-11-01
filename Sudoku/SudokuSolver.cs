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
            // Solution grid
            int[,] solution = grid;

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




            // Bool used to check if the solution has any empty cells left (alternative would be to count the full cells for each pass)
            bool isFull = false;
            // Bool to check if any cell resulted in a solution this iteration
            bool isAnyCellSolved = false;
            // Iteration count for while loop, to enforce a limit in case no solution is found
            int iterations = 0;

            // Try to find solutions for the cells on the grid. While the solution grid is not yet full, the previous iteration found at least one solution, and the total iterations has not exceeded the maximum allowed
            do
            {
                // Set isFull to true, as an initial value, since we will go over each cell and check if it is empty. If it is empty, isFull will be set to false
                isFull = true;
                // isAnyCellSolved to false, as an initial value. Will be set to true when a cell is solved
                isAnyCellSolved = false;
                // Set all elements in validNums back to false
                FillBoolArray(validNums, false);


                // For every row of the sudoku
                for (int m = 0; m < grid.GetLength(0); m++)
                {
                    // For every column, on every row. ie every cell
                    for (int n = 0; n < grid.GetLength(1); n++)
                    {
                        // If no single valid number has yet been determined for this cell, look for said number
                        if (solution[m, n] == 0)
                        {
                            // Set isFull to false since this cell is not yet full
                            isFull = false;

                            // Debug statement, TODO remove
                            Debug.WriteLine($"SudokuSolver: SolveHomeBrew() looking at cell [{m}, {n}]");

                            // Get all valid numbers for the current cell
                            bool[] validNumsCell = GetValidNumbers(m, n, solution);

                            // Check to see if there is only a single valid number for the cell
                            bool? isMultiple = null;
                            // Starting at index 1, since GetValidNums also returns for 0, which is always a valid number. And the index equals the actual number the bool element represents
                            for (int i1 = 0; i1 < validNumsCell.Length; i1++)
                            {
                                // If a number is valid
                                if (validNumsCell[i1])
                                {
                                    // Move the bool? isMultiple one along, from null to false, form false to true
                                    isMultiple = isMultiple != null;
                                }

                                // Also, store the valid number(s) for this cell in the grid-wide array, for later reference
                                validNums[m, n, i1] = validNumsCell[i1];
                            }

                            // If not a single valid number was found for the cell, set its value to -1 to signify this failure
                            if (isMultiple == null)
                            {
                                Debug.WriteLine($"SudokuSolver: SolveHomeBrew() could not find a solution for cell [{m}, {n}]");
                                solution[m, n] = -1;
                            }
                            // If a single value was found for the cell
                            else if (isMultiple == false)
                            {
                                Debug.WriteLine($"SudokuSolver: SolveHomeBrew() solution found for cell [{m}, {n}]");
                                // For each element of validNums, see if it is the valid number
                                for (int j1 = 0; j1 < validNums.GetLength(2); j1++)
                                {
                                    // If it is the valid number
                                    if (validNums[m, n, j1])
                                    {
                                        Debug.WriteLine($"SudokuSolver: SolveHomeBrew() solution found for cell [{m}, {n}] = {j1 + 1}");
                                        // Copy its value (index) over to the solution
                                        solution[m, n] = j1 + 1;
                                        // Log that at least one cell was solved in this iteration
                                        isAnyCellSolved = true;
                                        Debug.WriteLine($"SudokuSolver: SolveHomeBrew() solution found! cell [{m}, {n}], simple");
                                        break;
                                    }
                                }
                            }
                            // Else multiple solutions were found
                            else
                            {


                                Debug.WriteLine($"SudokuSolver: SolveHomeBrew() cell[{m},{n}], possible solution are;");
                                string validNumsString = "";
                                for (int debugI = 0; debugI < validNumsCell.Length; debugI++)
                                {
                                    validNumsString += $"{debugI + 1}={validNumsCell[debugI]} ";
                                }
                                Debug.WriteLine($"SudokuSolver: SolveHomeBrew() {validNumsString}");
                            }
                        }
                    }
                }







                // If no 'easy' solutions were found for any cell on the grid
                if (!isAnyCellSolved)
                {
                    // Check to see if a number is only valid in a single cell for an entire row, column or region
                    // If so this means that number can only be in that cell and nowhere else, and must be part of the solution

                    // For all rows, see if there are any values that are only valid in one cell
                    List<int[]> singleValues = FindSingleValuesRows(validNums);
                    // If any single value was found, 
                    if (singleValues.Count > 0)
                    {
                        // Log that at least one cell was solved in this iteration
                        isAnyCellSolved = true;

                        Debug.WriteLine($"SudokuSolver: SolveHomebrew() found {singleValues.Count} single value(s) in rows");

                        // Apply the found single values to the solution and remove them from validNums
                        ApplyFoundSingleValues(singleValues, solution, validNums);

                        Debug.WriteLine($"SudokuSolver: SolveHomeBrew() solution found! row single");
                    }
                    else
                    {
                        // For all columns, see if there are any values that are only valid in one cell
                        singleValues = FindSingleValuesColumns(validNums);
                        // If any single value was found, log that at least one cell was solved this iteration
                        if (singleValues.Count > 0)
                        {
                            // Log that at least one cell was solved in this iteration
                            isAnyCellSolved = true;

                            Debug.WriteLine($"SudokuSolver: SolveHomebrew() found {singleValues.Count} single value(s) in columns");

                            // Apply the found single values to the solution and remove them from validNums
                            ApplyFoundSingleValues(singleValues, solution, validNums);

                            Debug.WriteLine($"SudokuSolver: SolveHomeBrew() solution found! column single");
                        }
                        else
                        {
                            // For all regions, see if there are any values that are only valid in one cell
                            singleValues = FindSingleValuesRegions(validNums);
                            if (singleValues.Count > 0)
                            {
                                // Log that at least one cell was solved in this iteration
                                isAnyCellSolved = true;

                                Debug.WriteLine($"SudokuSolver: SolveHomebrew() found {singleValues.Count} single value(s) in regions");

                                // Apply the found single values to the solution and remove them from validNums
                                ApplyFoundSingleValues(singleValues, solution, validNums);

                                Debug.WriteLine($"SudokuSolver: SolveHomeBrew() solution found! region single");
                            }
                        }                        
                    }
                }

                // If still not a single solution was found
                if (!isAnyCellSolved)
                {
                    // Look at every region and check if a number in that region has to be within a single row or column in that region
                    // This would mean that that number can not appear elsewhere in the same row or column outside of that region, eliminating those possibilities

                    // For each region
                    for (int mReg = 0; mReg < grid.GetLength(0); mReg += RegionDim)
                    {
                        for (int nReg = 0; nReg < grid.GetLength(1); nReg += RegionDim)
                        {

                            // For each number that could possibly be on the grid
                            for (int nums = 0; nums < validNums.GetLength(2); nums++)
                            {
                                // Go over each row
                                for (int checkRow = mReg + 1; checkRow < mReg + RegionDim; checkRow++)
                                {
                                    
                                }


                            }


                        }
                    }


                }







                Debug.WriteLine($"SudokuSolver: SolveHomeBrew() iteration = {iterations}, isFull = {isFull}");
                iterations++;

                // Debug, TODO remove
                if (!isAnyCellSolved) { Debug.WriteLine($"SudokuSolver: SolveHomeBrew() no cell solved this iteration. Terminating loop"); }

            } while (!isFull && isAnyCellSolved && iterations < 100);



            /*
            while (!isFull && isAnyCellSolved && iterations < 100)
            {
                // Set isFull to true, as an initial value, since we will go over each cell and check if it is empty. If it is empty, isFull will be set to false
                isFull = true;
                // isAnyCellSolved to false, as an initial value. Will be set to true when a cell is solved
                isAnyCellSolved = false;
                // Set all elements in validNums back to false
                FillBoolArray(validNums, false);


                // For every row of the sudoku
                for (int m = 0; m < grid.GetLength(0); m++)
                {
                    // For every column, on every row. ie every cell
                    for (int n = 0; n < grid.GetLength(1); n++)
                    {
                        // If no single valid number has yet been determined for this cell, look for said number
                        if (solution[m, n] == 0)
                        {
                            // Set isFull to false since this cell is not yet full
                            isFull = false;

                            // Debug statement, TODO remove
                            Debug.WriteLine($"SudokuSolver: SolveHomeBrew() looking at cell [{m}, {n}]");

                            // Get all valid numbers for the current cell
                            bool[] validNumsCell = GetValidNumbers(m, n, solution);

                            // Check to see if there is only a single valid number for the cell
                            bool? isMultiple = null;
                            // Starting at index 1, since GetValidNums also returns for 0, which is always a valid number. And the index equals the actual number the bool element represents
                            for (int i1 = 0; i1 < validNumsCell.Length; i1++)
                            {
                                // If a number is valid
                                if (validNumsCell[i1])
                                {
                                    // Move the bool? isMultiple one along, from null to false, form false to true
                                    isMultiple = isMultiple != null;
                                }

                                // Also, store the valid number(s) for this cell in the grid-wide array, for later reference
                                validNums[m, n, i1] = validNumsCell[i1];
                            }

                            // If not a single valid number was found for the cell, set its value to -1 to signify this failure
                            if (isMultiple == null)
                            {
                                Debug.WriteLine($"SudokuSolver: SolveHomeBrew() could not find a solution for cell [{m}, {n}]");
                                solution[m, n] = -1;
                            }
                            // If a single value was found for the cell
                            else if (isMultiple == false)
                            {
                                Debug.WriteLine($"SudokuSolver: SolveHomeBrew() solution found for cell [{m}, {n}]");
                                // For each element of validNums, see if it is the valid number
                                for (int j1 = 0; j1 < validNums.GetLength(2); j1++)
                                {
                                    // If it is the valid number
                                    if (validNums[m, n, j1])
                                    {
                                        Debug.WriteLine($"SudokuSolver: SolveHomeBrew() solution found for cell [{m}, {n}] = {j1 + 1}");
                                        // Copy its value (index) over to the solution
                                        solution[m, n] = j1 + 1;
                                        // Log that at least one cell was solved in this iteration
                                        isAnyCellSolved = true;
                                        break;
                                    }
                                }                                
                            }
                            // Else multiple solutions were found
                            else
                            {
                                

                                Debug.WriteLine($"SudokuSolver: SolveHomeBrew() cell[{m},{n}], possible solution are;");
                                string validNumsString = "";
                                for (int debugI = 0; debugI < validNumsCell.Length; debugI++)
                                {
                                    validNumsString += $"{debugI + 1}={validNumsCell[debugI]} ";
                                }
                                Debug.WriteLine($"SudokuSolver: SolveHomeBrew() {validNumsString}");
                            }
                        }
                    }
                }







                // If no 'easy' solutions were found for any cell on the grid
                if (!isAnyCellSolved)
                {

                    // Check to see if a number is only valid in a single cell in an entire row, column or region (TODO implement)

                    // For all rows, see if there are any values that are only valid in one cell
                    List<int[]> singleValues = FindSingleValuesRows(validNums);
                    // For each found single value, when looking at the rows
                    foreach(int[] singleValue in singleValues)
                    {
                        // Add the value to the solution
                        solution[singleValue[0], singleValue[1]] = singleValue[2];
                        // Remove it from validNums, as to avoid duplicates when looking at columns or regions next
                        validNums[singleValue[0], singleValue[1], singleValue[2] - 1] = false;                        
                    }




                }







                Debug.WriteLine($"SudokuSolver: SolveHomeBrew() iteration = {iterations}, isFull = {isFull}");
                iterations++;

                // Debug, TODO remove
                if (!isAnyCellSolved) { Debug.WriteLine($"SudokuSolver: SolveHomeBrew() no cell solved this iteration. Terminating loop"); }
            }
            */

            
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
            Debug.WriteLine($"SudokuSolver: GetValidNumbersRow() found for row {m}, not {validNumsString}");

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
            Debug.WriteLine($"SudokuSolver: GetValidNumbersColumn() found for column {n}, not {validNumsString}");

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
            Debug.WriteLine($"SudokuSolver: GetValidNumbersRow() found for region around [{m},{n}], not {validNumsString}");

            return validNums;
        }








        /// <summary>
        /// Find all values that occure only once in a row, meaning that that is the only place they can be entered for the solution
        /// Looks at all rows, since no row has any weight when considering the valid values of another
        /// Returns a collection of int[]'s, one int[] for each found value, with the int[] containing the data {m, n, value}
        /// </summary>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private List<int[]> FindSingleValuesRows(bool[,,] validNums)
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
                                Debug.WriteLine($"SudokuSolver: FindSingleValuesRows() found [{mRow}, {indexRow}] = {sinNum + 1}");
                            }
                        }
                    }
                }
            }

            return singleValues;
        }




        /// <summary>
        /// Find all values that occure only once in a column, meaning that that is the only place they can be entered for the solution
        /// Looks at all columns, since no column has any weight when considering the valid values of another
        /// Returns a collection of int[]'s, one int[] for each found value, with the int[] containing the data {m, n, value}
        /// </summary>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private List<int[]> FindSingleValuesColumns(bool[,,] validNums)
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
                                Debug.WriteLine($"SudokuSolver: FindSingleValuesRows() found [{indexColumn}, {nColumn}] = {sinNum + 1}");
                            }
                        }
                    }
                }
            }

            return singleValues;
        }




        /// <summary>
        /// Find all values that occure only once in a region, meaning that that is the only place they can be entered for the solution
        /// Looks at all region, since no region has any weight when considering the valid values of another
        /// Returns a collection of int[]'s, one int[] for each found value, with the int[] containing the data {m, n, value}
        /// </summary>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private List<int[]> FindSingleValuesRegions(bool[,,] validNums)
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
                                        Debug.WriteLine($"SudokuSolver: FindSingleValuesRows() found [{mRegIndex}, {nRegIndex}] = {sinNum + 1}");
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
        /// Find the valid numbers for each cell on the grid and write them to validNums. If a cell only has a single valid number, enter it into the cell of solution (and remove it from validNums)
        /// Returns true if such a single valid number was found, else false
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private bool FindValidNumbers(int[,] solution, bool[,,] validNums)
        {
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
                        bool? isMultiple = null;
                        for (int i = 0; i < validNumsCell.Length; i++)
                        {
                            // If a number is valid
                            if (validNumsCell[i])
                            {
                                // If this number is encountered for the first time, isMultiple will be null, and as a result isMultiple be changed to true
                                // If the number has been encountered already, isMultiple will no longer be null, but true, and isMultiple will be set to false
                                isMultiple = isMultiple == null;
                            }

                            // Also, store the valid number(s) for this cell in the grid-wide array, for later reference
                            validNums[m, n, i] = validNumsCell[i];
                        }

                        // If not a single valid number was found for the cell, set its value to -1 to signify this failure (should not happen, unless the grid does not have a solution, or something else went wrong)
                        if (isMultiple == null)
                        {
                            Debug.WriteLine($"SudokuSolver: FindValidNumbers() could not find a solution for cell [{m}, {n}]");
                            solution[m, n] = -1;
                        }
                        // If a single value was found for the cell
                        else if (isMultiple == false)
                        {
                            Debug.WriteLine($"SudokuSolver: FindValidNumbers() solution found for cell [{m}, {n}]");
                            // For each number for the current cell in validNums, see if it is the valid number
                            for (int num = 0; num < validNums.GetLength(2); num++)
                            {
                                // If it is the valid number
                                if (validNums[m, n, num])
                                {
                                    // Copy its value (index) over to the solution
                                    solution[m, n] = num + 1;
                                    // Remove all other valid numbers of equal value from its row, column and region
                                    RemoveValidNumFromConnectedSequences(m, n, num, validNums);
                                    // Log that at least one cell was solved in this iteration
                                    isAnyCellSolved = true;
                                    Debug.WriteLine($"SudokuSolver: FindValidNumbers() solution found! cell [{m}, {n}] = {num + 1}");
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
                            Debug.WriteLine($"SudokuSolver: FindValidNumbers() cell[{m},{n}], possible solution are; {validNumsString}");
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
        /// Set the number 'number' to not be a valid number on all the cells in same row, column and region as the cell at [m, n]
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="number"></param>
        /// <param name="validNums"></param>
        private void RemoveValidNumFromConnectedSequences(int m, int n, int number, bool[,,] validNums)
        {
            string debugString = "";

            for (int x = 0; x < validNums.GetLength(0); x++)
            {
                validNums[x, n, number] = false;
                validNums[m, x, number] = false;
                validNums[x / RegionDim, x % RegionDim, number] = false;

                debugString += "[" + x / RegionDim + ", " + x % RegionDim + "] ";
            }

            Debug.WriteLine($"SudokuSolver: DeleteValidNumFromConnectedSequences() deleted {number} from region cells {debugString}");
        }


        private void DeleteValidNumFromRow(int m, int number, bool[,,] validNums)
        {
            for (int n = 0; n < validNums.GetLength(1); n++)
            {
                validNums[m, n, number] = false;
            }
        }

        private void DeleteValidNumFromColumn(int n, int number, bool[,,] validNums)
        {
            for (int m = 0; m < validNums.GetLength(0); m++)
            {
                validNums[m, n, number] = false;
            }
        }

        private void DeleteValidNumFromRegion(int m, int n, int number, bool[,,] validNums)
        {

        }



        /// <summary>
        /// Fill the specified 3-D boolean array with the desired state
        /// </summary>
        /// <param name="array"></param>
        /// <param name="state"></param>
        private static void FillBoolArray(bool[,,] array, bool state)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    for (int k = 0; k < array.GetLength(2); k++)
                    {
                        array[i, j, k] = state;
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
                Debug.WriteLine($"SudokuSolver: ApplyFoundSingleValues() applying [{singleValue[0]}, {singleValue[1]}] = {singleValue[2]}");
                // Add the value to the solution
                solution[singleValue[0], singleValue[1]] = singleValue[2];
                // Remove it from validNums, as to avoid duplicates when looking at columns or regions next
                //validNums[singleValue[0], singleValue[1], singleValue[2] - 1] = false;
            }
        }






        
        


    }
}

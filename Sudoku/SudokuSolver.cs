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








        public int[,] SolveHomeBrew(int[,] grid, bool useGuessOnDeadEnd = false)
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

            // Find all valid numbers for each cell on the grid
            FindValidNumbers(solution, validNums);


            /* OLD, TODO remove
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

                // If no valid numbers were removed from cells by looking for values that are limited to a single row or column within a region
                if (!isAnyCellAltered)
                {
                    Debug.WriteLine($"SudokuSolver: SolveHomeBrew() did not find any values limited to a single row or column in a region, now searching for rows or column limited to a single region");

                    // Find values of rows or columns that are limited to a single region, meaning that any other possibilities for that region are not possible since the number has to be in that row or column
                    isAnyCellAltered = FindRowOrColumnNumbersValidInSingleRegion(solution, validNums);
                }


                



                // If still not a single solution was found
                if (!isAnyCellAltered)
                {
                    Debug.WriteLine($"SudokuSolver: SolveHomeBrew() failed to find any solutions or alterations this iteration. Loop will terminate");
                }

                Debug.WriteLine($"SudokuSolver: SolveHomeBrew() iteration = {iterations}");
                iterations++;

            } while (!IsGridFull(solution) && isAnyCellAltered && iterations < maxIterations);
            */


            // Try to find the solution
            FindSolutionHomeBrew(solution, validNums, useGuessOnDeadEnd);


            






            return solution;
        }



        /// <summary>
        /// Find a solution for the specified grid, returns true if a complete solution was found
        /// Looks for cells with a single valid number, if non found looks for values with only a single valid spot in rows, columns, or regions
        /// If still not a single cell was solved, looks at regions to see if their numbers are limited to a single row or column, eliminating all other locations in that row or column
        /// If that does not result in a reduction of valid numbers for any cell, look for rows or column who have their valid numbers limited to a single region, eliminating all other locations within that region
        /// Else no solution could be found, and returns false
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private bool FindSolutionHomeBrew(int[,] solution, bool[,,] validNums, bool useGuessOnDeadEnd = false, string debugMessage = "")
        {
            if (!string.IsNullOrWhiteSpace(debugMessage)) { Debug.WriteLine($"SudokuSolver: FindSolutionHomeBrew() debugMessage, {debugMessage}"); }

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
                    Debug.WriteLine($"SudokuSolver: FindSolutionHomeBrew() did not find a cell with a single valid value this iteration, now searching sequences for single value");

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
                    Debug.WriteLine($"SudokuSolver: FindSolutionHomeBrew() did not find any single values in a row, column or region, now searching for row/column limited numbers");

                    // Find values that are limited to a single row or column within a region, meaning that that number must be entered somewhere in the row or column within the region, and can be entered in the section of the row or column that is outside of the region
                    // Does not alter solution, only removes valid numbers from validNums
                    isAnyCellAltered = FindValuesLimitedToSingleRowOrColumnOfRegions(solution, validNums);
                }

                // If no valid numbers were removed from cells by looking for values that are limited to a single row or column within a region
                if (!isAnyCellAltered)
                {
                    Debug.WriteLine($"SudokuSolver: FindSolutionHomeBrew() did not find any values limited to a single row or column in a region, now searching for rows or column limited to a single region");

                    // Find values of rows or columns that are limited to a single region, meaning that any other possibilities for that region are not possible since the number has to be in that row or column
                    isAnyCellAltered = FindRowOrColumnNumbersValidInSingleRegion(solution, validNums);
                }



                // If still not a single solution was found
                if (!isAnyCellAltered)
                {
                    Debug.WriteLine($"SudokuSolver: FindSolutionHomeBrew() failed to find any solutions or alterations this iteration. Loop will terminate");
                }

                Debug.WriteLine($"SudokuSolver: FindSolutionHomeBrew() iteration = {iterations}");
                iterations++;

            } while (isAnyCellAltered && !IsGridFull(solution) && iterations < maxIterations);



            // If still no progress was made on the solution, and useGuessOnDeadEnd is true
            if (useGuessOnDeadEnd && !IsGridFull(solution))
            {
                // Try and find a solution by making a random guess for a valid number
                Debug.WriteLine($"SudokuSolver: FindSolutionHomeBrew() starting guess");

                // Find the number/cell with the least amount of guesses
                int[,] validNumCount = new int[solution.GetLength(0), solution.GetLength(1)];
                // Go over each cell, counting the number of valid numbers for each
                for (int m0 = 0; m0 < solution.GetLength(0); m0++)
                {
                    for (int n0 = 0; n0 < solution.GetLength(1); n0++)
                    {
                        // Count the number of valid numbers
                        for (int num0 = 0; num0 < validNums.GetLength(2); num0++)
                        {
                            if (validNums[m0, n0, num0]) { validNumCount[m0, n0]++; }
                        }
                    }
                }

                // Go over the found counts of valid numbers, from lowest to highest (minimum is two as no complete solution has been found)
                bool isSolved = false;
                for (int count = 2; count < solution.GetLength(0); count++)
                {
                    // Go over each cell
                    for (int m1 = 0; m1 < solution.GetLength(0); m1++)
                    {
                        for (int n1 = 0; n1 < solution.GetLength(1); n1++)
                        {
                            if (validNumCount[m1, n1] == count)
                            {
                                int[] validNumsCell = new int[count];
                                int validNumsCellCurInd = 0;
                                // Find the valid numbers and store them in int[] validNumsCell
                                for (int num1 = 0; num1 < validNums.GetLength(2); num1++)
                                {
                                    // Find the valid number
                                    if (validNums[m1, n1, num1])
                                    {
                                        validNumsCell[validNumsCellCurInd] = num1;
                                        validNumsCellCurInd++;
                                    }
                                }

                                // Create copies of the arrays used to find the solution
                                int[,] solution1 = (int[,])solution.Clone();
                                bool[,,] validNums1 = (bool[,,])validNums.Clone();

                                // For each found valid number for the cell, try and see if it leads to a complete solution
                                foreach (int x in validNumsCell)
                                {
                                    Debug.WriteLine($"SudokuSolver: FindSolutionHomeBrew() making a guess, [{m1}, {n1}] = {x + 1}");
                                    // Apply the guess to the arrays
                                    EnterSolution(m1, n1, x, solution1, validNums1);
                                    // Try and find the solution with the guessed value
                                    isSolved = FindSolutionHomeBrew(solution1, validNums1, useGuessOnDeadEnd);

                                    // If the solution was found
                                    if (isSolved)
                                    {
                                        Debug.WriteLine($"SudokuSolver: FindSolutionHomeBrew() solution found! by guessing [{m1}, {n1}] = {x + 1}");
                                        PrintSudokuToOutput(solution1);
                                        solution = (int[,])solution1.Clone();
                                        validNums = (bool[,,])validNums1.Clone();
                                        break;
                                    }
                                }
                            }
                            if (isSolved) { break; }
                        }
                        if (isSolved) { break; }
                    }
                    if (isSolved) { break; }
                }
            }

            // Debug, TODO remove
            if (IsSudokuComplete(solution))
            {
                Debug.WriteLine($"SudokuSolver: FindSolutionHomeBrew() IsSudokuComplete() returned true!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Debug.WriteLine($"SudokuSolver: FindSolutionHomeBrew() IsSudokuComplete() returned true!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Debug.WriteLine($"SudokuSolver: FindSolutionHomeBrew() IsSudokuComplete() returned true!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                PrintSudokuToOutput(solution);
            }


            return IsSudokuComplete(solution);
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
                Debug.WriteLine($"SudokuSolver: FindRepeatingValuesinRowOrColumnOfRegions() given solution grid must be 9x9 for method to be able to process it. returning !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                return false;
            }

            // Is any alteration for validNums found by this method
            bool isAnyCellAltered = false;

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
                        /*
                        Debug.WriteLine($"SudokuSolver: FindValuesLimitedToSingleRowOrColumnOfRegions() checking lines for {num + 1}, starting from [{m0}, {n0}]");
                        Debug.WriteLine("SudokuSolver: FindValuesLimitedToSingleRowOrColumnOfRegions() " + debugRow0);
                        Debug.WriteLine("SudokuSolver: FindValuesLimitedToSingleRowOrColumnOfRegions() " + debugRow1);
                        Debug.WriteLine("SudokuSolver: FindValuesLimitedToSingleRowOrColumnOfRegions() " + debugRow2);
                        if (rowIndex[0] > -1) { Debug.WriteLine($"SudokuSolver: FindValuesLimitedToSingleRowOrColumnOfRegions() found single row for {num + 1} at [{rowIndex[0]}, {rowIndex[1]}]"); }
                        if (columnIndex[0] > -1) { Debug.WriteLine($"SudokuSolver: FindValuesLimitedToSingleRowOrColumnOfRegions() found single column for {num + 1} at [{columnIndex[0]}, {columnIndex[1]}]"); }
                        */


                        // If a single row was found
                        if (rowIndex[0] > -1)
                        {
                            isAnyCellAltered = !isAnyCellAltered ? RemoveValidNumbersFromRowExcludingLocalRegion(rowIndex[0], rowIndex[1], num, validNums) : isAnyCellAltered;
                        }
                        else if (columnIndex[0] > -1)
                        {
                            isAnyCellAltered = !isAnyCellAltered ? RemoveValidNumbersFromColumnExcludingLocalRegion(columnIndex[0], columnIndex[1], num, validNums) : isAnyCellAltered;
                        }
                    }
                }
            }

            if (!isAnyCellAltered) { Debug.WriteLine("SudokuSolver: FindValuesLimitedToSingleRowOrColumnOfRegions() did not find anything to help further the solution"); }

            return isAnyCellAltered;
        }

        /// <summary>
        /// Find a row or column is which a number can only be entered in a single region, meaning any other valid locations for that number in the region are invalid
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private bool FindRowOrColumnNumbersValidInSingleRegion(int[,] solution, bool[,,] validNums)
        {
            bool isAnyCellAltered = false;

            // Look through each region (goes through diagonally, [0, 0] -> [3, 3] -> [9, 9], so only a single counter is needed)
            for (int region = 0; region < solution.GetLength(0); region += RegionDim)
            {
                // For each possible number
                for (int num = 0; num < validNums.GetLength(2); num++)
                {
                    bool[,] isNumInRowRegion = new bool[RegionDim, RegionDim];
                    bool[,] isNumInColumnRegion = new bool[RegionDim, RegionDim];

                    // Go through the rows/columns of the current region, finding for which row(s)/column(s) the num is valid in each region
                    for (int x = region; x < region + RegionDim; x++)
                    {
                        for (int y = 0; y < solution.GetLength(0); y++)
                        {
                            // If the number is valid for the row, log it
                            if (validNums[x, y, num]) { isNumInRowRegion[x - region, y / RegionDim] = true; }
                            // If the number is valid for the column, log it
                            if (validNums[y, x, num]) { isNumInColumnRegion[y / RegionDim, x - region] = true; }
                        }
                    }

                    // Go over the found values for isNumInRowRegion and isNumInColumnRegion
                    for (int m0 = 0; m0 < RegionDim; m0++)
                    {
                        bool? isRowInSingleRegion = null;
                        bool? isColumnInSingleRegion = null;

                        for (int n0 = 0; n0 < RegionDim; n0++)
                        {
                            // Find out is the number is only in a single region for this row
                            if (isNumInRowRegion[m0, n0]) { isRowInSingleRegion = isRowInSingleRegion == null; }
                            // Find out if the number is only in a single region for this column
                            if (isNumInColumnRegion[n0, m0]) { isColumnInSingleRegion = isColumnInSingleRegion == null; }
                        }

                        // If the number was found to be limited to a single region for the entire row
                        if (isRowInSingleRegion == true)
                        {
                            Debug.WriteLine($"SudokuSolver: FindSingleRowOrColumnValidInSingleRegion() found isRowInSingleRegion == true in region {region}");

                            // Find the valid region
                            for (int validRegionRow = 0; validRegionRow < RegionDim; validRegionRow++)
                            {
                                // If the number is valid for this region in the row
                                if (isNumInRowRegion[m0, validRegionRow])
                                {
                                    Debug.WriteLine($"SudokuSolver: FindSingleRowOrColumnValidInSingleRegion() row = {m0 + region}, n = {validRegionRow * RegionDim}");

                                    // Delete the number from the rest of the region
                                    isAnyCellAltered = !isAnyCellAltered ? RemoveValidNumbersFromRegionExcludingRow(m0 + region, validRegionRow * RegionDim, num, validNums) : isAnyCellAltered;
                                    // Break, since this is the only, single, valid region for this row
                                    break;
                                }
                            }
                        }

                        // If the number was found to be limited to a single region for the entire column
                        if (isColumnInSingleRegion == true)
                        {
                            Debug.WriteLine($"SudokuSolver: FindSingleRowOrColumnValidInSingleRegion() found isColumnInSingleRegion == true in region {region}");

                            // Find the valid region
                            for (int validRegionColumn = 0; validRegionColumn < RegionDim; validRegionColumn++)
                            {
                                // If the number is valid for this region in the column
                                if (isNumInColumnRegion[validRegionColumn, m0])
                                {
                                    Debug.WriteLine($"SudokuSolver: FindSingleRowOrColumnValidInSingleRegion() column = {m0 + region}, m = {validRegionColumn * RegionDim}");

                                    // Remove the number from the rest of the region
                                    isAnyCellAltered = !isAnyCellAltered ? RemoveValidNumbersFromRegionExcludingColumn(validRegionColumn * RegionDim, m0 + region, num, validNums) : isAnyCellAltered;
                                    // Break, since this is the only, single, valid region for this column
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (!isAnyCellAltered) { Debug.WriteLine("SudokuSolver: FindSingleRowOrColumnValidInSingleRegion() did not find anything to help further the solution"); }

            return isAnyCellAltered;
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
            // Debug.WriteLine($"SudokuSolver: RemoveValidNumbersFromConnectedSequences() deleting {number + 1} from cells connected to [{m}, {n}]");

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
        /// Returns whether any cells were true and are now set to false. True if so, false if not
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="number"></param>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private bool RemoveValidNumbersFromRowExcludingLocalRegion(int m, int n, int number, bool[,,] validNums)
        {
            // Debug.WriteLine($"SudokuSolver: RemoveValidNumbersFromRowExcludingLocalRegion() deleting {number + 1} from cells connected to [{m}, {n}]");

            bool isAnyCellAltered = false;

            for (int i = 0; i < validNums.GetLength(0); i++)
            {
                // If the current n-coor (i) is not in the local region
                if (n / RegionDim != i / RegionDim)
                {
                    if (validNums[m, i, number])
                    {
                        // Debug.WriteLine($"SudokuSolver: RemoveValidNumbersFromRowExcludingLocalRegion() deleting {number + 1} from [{m}, {i}]");

                        // Set the number to false for this cell
                        validNums[m, i, number] = false;
                        isAnyCellAltered = true;
                    }
                }
            }

            return isAnyCellAltered;
        }

        /// <summary>
        /// Set the number 'number' to not be a valid number on all the cells in same the column, excluding those cells in the same region as cell [m, n]
        /// Returns whether any cells were true and are now set to false. True if so, false if not
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="number"></param>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private bool RemoveValidNumbersFromColumnExcludingLocalRegion(int m, int n, int number, bool[,,] validNums)
        {
            // Debug.WriteLine($"SudokuSolver: RemoveValidNumbersFromColumnExcludingLocalRegion() trying to remove {number + 1} from cells connected to [{m}, {n}]");

            bool isAnyCellAltered = false;

            for (int i = 0; i < validNums.GetLength(0); i++)
            {
                // If the current m-coor (i) is not in the local region
                if (m / RegionDim != i / RegionDim)
                {
                    if (validNums[i, n, number])
                    {
                        // Debug.WriteLine($"SudokuSolver: RemoveValidNumbersFromColumnExcludingLocalRegion() deleting {number + 1} from [{i}, {n}]");

                        // Set the number to false for this cell
                        validNums[i, n, number] = false;
                        isAnyCellAltered = true;
                    }
                    
                }
            }

            return isAnyCellAltered;
        }

        /// <summary>
        /// Set the specified number to be false for all cells within the region containing the cell [m, n], excluding the cells in the same row as cell [m, n]
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="number"></param>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private bool RemoveValidNumbersFromRegionExcludingRow(int m, int n, int number, bool[,,] validNums)
        {
            // Debug.WriteLine($"SudokuSolver: RemoveValidNumbersFromRegionExcludingRow() trying to remove {number + 1} from region containing [{m}, {n}]");

            bool isAnyCellAltered = false;

            // Go over all the cells within the region that contains the cell [m, n]
            for (int m0 = m / RegionDim * RegionDim; m0 < m / RegionDim * RegionDim + RegionDim; m0++)
            {
                for (int n0 = n / RegionDim * RegionDim; n0 < n / RegionDim * RegionDim + RegionDim; n0++)
                {
                    // If the current row is not the given row (ie the row to be excluded, to not be altered), and the number is valid for this cell
                    if (m0 != m && validNums[m0, n0, number])
                    {
                        // Debug.WriteLine($"SudokuSolver: RemoveValidNumbersFromRegionExcludingRow() deleting {number + 1} from [{m0}, {n0}]");

                        // Set the number to false for this cell
                        validNums[m0, n0, number] = false;
                        isAnyCellAltered = true;
                    }
                }
            }

            return isAnyCellAltered;
        }

        /// <summary>
        /// Set the specified number to be false for all cells within the region containing the cell [m, n], excludinmg the cells in the same column as cell [m, n]
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="number"></param>
        /// <param name="validNums"></param>
        /// <returns></returns>
        private bool RemoveValidNumbersFromRegionExcludingColumn(int m, int n, int number, bool[,,] validNums)
        {
            // Debug.WriteLine($"SudokuSolver: RemoveValidNumbersFromRegionExcludingColumn() trying to remove {number + 1} from region containing [{m}, {n}]");

            bool isAnyCellAltered = false;

            // Go over all the cells within the region that contains the cell [m, n]
            for (int m0 = m / RegionDim * RegionDim; m0 < m / RegionDim * RegionDim + RegionDim; m0++)
            {
                for (int n0 = n / RegionDim * RegionDim; n0 < n / RegionDim * RegionDim + RegionDim; n0++)
                {
                    // If the current column is not the given column (ie the column to be excluded, to not be altered), and the number is valid for this cell
                    if (n0 != n && validNums[m0, n0, number])
                    {
                        // Debug.WriteLine($"SudokuSolver: RemoveValidNumbersFromRegionExcludingRow() deleting {number + 1} from [{m0}, {n0}]");

                        // Set the number to false for this cell
                        validNums[m0, n0, number] = false;
                        isAnyCellAltered = true;
                    }
                }
            }

            return isAnyCellAltered;
        }










        /// <summary>
        /// Enter a found solution value for a cell into the solution grid
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="num">number to enter, minus 1! So if you want to enter 3, 'num' must be 2. This is because this method will most likely be called with an index representing the actual number, so coming from a 0-base set instead of a 1-base</param>
        /// <param name="solution"></param>
        /// <param name="validNums"></param>
        /// <param name="debugMessage"></param>
        private void EnterSolution(int m, int n, int num, int[,] solution, bool[,,] validNums, string debugMessage = "")
        {            
            if (debugMessage != "") { Debug.WriteLine($"SudokuSolver: EnterSolution() debugMessage {debugMessage}"); }
            Debug.WriteLine($"SudokuSolver: EnterSolution() solution entered, [{m}, {n}] =  {num + 1}");
            solution[m, n] = num + 1;
            RemoveValidNumbersFromCellAndConnectedSequences(m, n, num, validNums);
        }














        /// <summary>
        /// Check whether the grid is full or if it contains any empty cell, ie cells containing 0
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>True if no cell on the grid is 0, false if there is</returns>
        private bool IsGridFull(int[,] grid)
        {
            foreach (int value in grid)
            {
                if (value == 0) return false;
            }
            return true;
        }

        /// <summary>
        /// Check whether the sudoku is valid and does not contain any errors
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private bool IsSudokuValid(int[,] grid)
        {
            // Can only handle gird with 2 dimensions of equal length
            if (grid.GetLength(0) != grid.GetLength(1)) { return false; }

            bool isValid = true;

            // Validate the rows and columns
            for (int m = 0; m < grid.GetLength(0); m++)
            {
                bool?[] numbersRow = new bool?[grid.GetLength(0)];
                bool?[] numbersColumn = new bool?[grid.GetLength(0)];
                bool?[] numbersRegion = new bool?[grid.GetLength(0)];

                for (int n = 0; n < grid.GetLength(1); n++)
                {
                    // If -1 was found, meaning and error occured while trying to find the solution for that cell
                    if (grid[m, n] < 0)
                    {
                        // Set all numbers of row to false, resulting in false being returned at the end
                        Array.Fill(numbersRow, false);
                    }
                    else
                    {
                        if (grid[m, n] > 0)
                        {
                            numbersRow[grid[m, n] - 1] = numbersRow[grid[m, n] - 1] == null;
                        }
                        if (grid[n, m] > 0)
                        {
                            numbersColumn[grid[n, m] - 1] = numbersColumn[grid[n, m] - 1] == null;
                        }
                        // Calculate the coors for the region (alternatively, could be done with two for-loops instead of calculating)
                        int mReg = (m / RegionDim * RegionDim) + (n / RegionDim);
                        int nReg = (m % RegionDim * RegionDim) + (n % RegionDim);
                        if (grid[mReg, nReg] > 0)
                        {
                            numbersRegion[grid[mReg, nReg] - 1] = numbersRegion[grid[mReg, nReg] - 1] == null;
                        }
                    }
                }

                // Go over each number
                for (int num = 0; num < grid.GetLength(0); num++)
                {
                    // If any sequence was not valid
                    if (numbersRow[num] == false || numbersColumn[num] == false || numbersRegion[num] == false)
                    {
                        isValid = false;
                    }
                }
            }

            return isValid;
        }

        /// <summary>
        /// Check whether this sudoku is completed and all cell values are valid
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private bool IsSudokuComplete(int[,] grid)
        {
            // Can only handle gird with 2 dimensions of equal length
            if (grid.GetLength(0) != grid.GetLength(1)) { return false; }

            bool isValid = true;

            // Validate the rows and columns
            for (int m = 0; m < grid.GetLength(0); m++)
            {
                bool?[] numbersRow = new bool?[grid.GetLength(0)];
                bool?[] numbersColumn = new bool?[grid.GetLength(0)];
                bool?[] numbersRegion = new bool?[grid.GetLength(0)];

                for (int n = 0; n < grid.GetLength(1); n++)
                {
                    // If -1 was found, meaning and error occured while trying to find the solution for that cell
                    if (grid[m, n] <= 0)
                    {
                        // Set all numbers of row to false, resulting in false being returned at the end
                        numbersRow[0] = false;
                        break;
                    }
                    else
                    {
                        if (grid[m, n] > 0)
                        {
                            numbersRow[grid[m, n] - 1] = numbersRow[grid[m, n] - 1] == null;
                        }
                        if (grid[n, m] > 0)
                        {
                            numbersColumn[grid[n, m] - 1] = numbersColumn[grid[n, m] - 1] == null;
                        }
                        // Calculate the coors for the region (alternatively, could be done with two for-loops instead of calculating)
                        int mReg = (m / RegionDim * RegionDim) + (n / RegionDim);
                        int nReg = (m % RegionDim * RegionDim) + (n % RegionDim);
                        if (grid[mReg, nReg] > 0)
                        {
                            numbersRegion[grid[mReg, nReg] - 1] = numbersRegion[grid[mReg, nReg] - 1] == null;
                        }
                    }
                }

                // Go over each number
                for (int num = 0; num < grid.GetLength(0); num++)
                {
                    // If any sequence was not valid
                    if (numbersRow[num] == false || numbersColumn[num] == false || numbersRegion[num] == false)
                    {
                        isValid = false;
                        break;
                    }
                }
                if (!isValid) { break; }
            }

            return isValid;
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
        /// Print the specified sudoku grid to output
        /// </summary>
        /// <param name="grid"></param>
        private void PrintSudokuToOutput(int[,] grid)
        {
            Debug.WriteLine("SudokuSolver: PrintSudokuToOutput() printing grid");
            string s = "";
            for (int m = 0; m < grid.GetLength(0); m++)
            {
                s = "";
                for (int n = 0; n < grid.GetLength(1); n++)
                {
                    s += grid[m, n] + " ";
                }
                Debug.WriteLine(s);
            }
        }

        /// <summary>
        /// Print the valid locations of 'number' to output
        /// </summary>
        /// <param name="number"></param>
        /// <param name="validNums"></param>
        private void PrintValidCellsForNumberToOutput(int number, bool[,,] validNums)
        {
            if (number >= validNums.GetLength(2))
            {
                Debug.WriteLine($"SudokuSolver: PrintValidCellsForNumberToOutput() number {number} out-of-range of validNums");
                return;
            }

            Debug.WriteLine("SudokuSolver: PrintValidCellsForNumberToOutput() printing grid");
            string s = "";
            for (int m = 0; m < validNums.GetLength(0); m++)
            {
                s = "";
                for (int n = 0; n < validNums.GetLength(1); n++)
                {
                    s += (validNums[m, n, number] ? number : 0) + " ";
                }
                Debug.WriteLine(s);
            }
        }







    }
}

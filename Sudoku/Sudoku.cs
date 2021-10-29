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
        public int[,] Grid;




        public Sudoku() { }

        public Sudoku(int [,] grid)
        {
            Grid = grid;
        }











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

            return IsCellValidRow(m, n, v) && IsCellValidColumn(m, n, v) && IsCellValidRegion(m, n, v);
        }

        /// <summary>
        /// Check whether the number v is present anywhere else the row m
        /// </summary>
        /// <param name="m">Row to search through</param
        /// <param name="n">Column of the original cell with value v, needed to not hit on the original cell and its value. Provide a negative number to ignore and match with any cell</param>
        /// <param name="v">Value to match</param>
        /// <returns>True is no matching value is found. False is there is</returns>
        public bool IsCellValidRow(int m, int n, int v = -1)
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
        public bool IsCellValidColumn(int m, int n, int v = -1)
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
        public bool IsCellValidRegion(int m, int n, int v = -1)
        {
            // If no v was specified, get the current value of the Sudoku cell at [m, n]
            if (v < 0) { v = Grid[m, n]; }

            if (v != 0)
            {
                // Calculate the lengths of a single region, log debug messages if sqrt does not result in a whole integer
                if (!IsSqrtInt(Grid.GetLength(0))) { Debug.WriteLine($"MainPage: IsCellValidRegion() the square root of the grid dimension M is not a whole number, this might cause problems while determining the length for regions of the sudoku, due to required rounding."); }
                if (!IsSqrtInt(Grid.GetLength(1))) { Debug.WriteLine($"MainPage: IsCellValidRegion() the square root of the grid dimension N is not a whole number, this might cause problems while determining the length for regions of the sudoku, due to required rounding."); }
                int regLengthM = (int)Math.Sqrt(Grid.GetLength(0));
                int regLengthN = (int)Math.Sqrt(Grid.GetLength(1));

                // Test, TODO remove
                Debug.WriteLine($"MainPage(): IsCellValidRegion(m={m}, n={n}, v={v}) region dimensions are {regLengthM}, {regLengthN}");

                // Go over each cell in the region
                for (int m1 = m / regLengthM * regLengthM; m1 < m / regLengthM * regLengthM + regLengthM; m1++)
                {
                    for (int n1 = n / regLengthN * regLengthN; n1 < n / regLengthN * regLengthN + regLengthN; n1++)
                    {
                        // Test, TODO remove
                        Debug.WriteLine($"MainPage(): IsCellValidRegion(m={m}, n={n}, v={v}) checking cell {m1}, {n1}");

                        if (v == Grid[m1, n1] && m1 != m && n1 != n) { return false; }
                    }
                }
            }

            // Value is 0 or no match was found, return true
            return true;
        }

        /// <summary>
        /// Check whether the values in row m are valid or if there are any conflicts/duplicates
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool IsRowValid(int m)
        {
            if (m < 0 || m >= Grid.GetLength(0)) { return false; }

            bool?[] numbers = new bool?[Grid.GetLength(0)];

            for (int n = 0; n < Grid.GetLength(1); n++)
            {
                // If the number is encounter for the first time, set numbers[n] to true
                // If it is encounter again, numbers[n] being not null but true this time, set numbers[n] to false
                if (Grid[m, n] != 0 && numbers[n] != false) { numbers[n] = (numbers[n] == null) ? true : false; }
            }

            bool result = true;

            foreach (bool? num in numbers)
            {
                if (result != false) { result = num != false; }
            }
            return result;
        }









        /// <summary>
        /// Is the square root of x a whole number
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private bool IsSqrtInt(int x)
        {
            if (x < 0) { return false; }
            return Math.Sqrt(x) % 1 == 0;
        }

    }
}

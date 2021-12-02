
using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;


using Sudoku;



namespace UnitTests
{
    
    [TestClass]
    public class Item_T
    {        
        Random random = new Random();
        int[,] gridDefault = new int[9, 9] { { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, { 1, 2, 3, 4, 5, 6, 7, 8, 9 } };

        /// <summary>
        /// Verify that Item can store a grid
        /// </summary>
        [TestMethod]
        public void Item000_SetGrid()
        {
            int[,] grid0 = GetGridValuesRandom(9);
            Item item = new Item(grid0);

            Item_AreGridsEqual(grid0, item.Grid);
        }

        /// <summary>
        /// Verify that Item makes its own copy of the grid, and not a reference to the given grid
        /// </summary>
        [TestMethod]
        public void Item001_SetGrid_UniqueInstance()
        {
            int[,] grid0 = GetGridValuesRandom(9);
            Item item = new Item(grid0);

            if (grid0[0, 0] > 2) { grid0[0, 0]--; }
            else { grid0[0, 0]++; }
            Item_AreGridsValuesNotEqual(grid0, item.Grid);
        }

        /// <summary>
        /// Verify that the GridAsString set method of Item correctly handles a string with length from 2 to 9, resulting in a (length x length) array with values matching those of the input string
        /// </summary>
        [TestMethod]
        public void Item002_GridAsString_DimLengths_2_3_4_5_6_7_8_9()
        {
            Item item = new Item(gridDefault);

            int[] dimLengths = new int[] { 3, 4, 5, 6, 7, 8, 9 };
            foreach (int dimLength in dimLengths)
            {
                string input = GetGridValuesString(dimLength);
                item.GridAsString = input;

                int[,] grid = item.Grid;
                Assert.AreEqual(grid.Rank, 2);
                Assert.AreEqual(grid.GetLength(0), dimLength);
                Assert.AreEqual(grid.GetLength(0), grid.GetLength(1));

                bool isAnyValueNotCorrect = false;
                int v;
                int m1 = 0;
                int n1 = 0;
                for (int m = 0; m < grid.GetLength(0); m++)
                {
                    for (int n = 0; n < grid.GetLength(1); n++)
                    {
                        v = -2;
                        int.TryParse(input[m * dimLength + n].ToString(), out v);
                        if (grid[m, n] != v) { isAnyValueNotCorrect = true; m1 = m; n1 = n; break; }
                    }
                }
                Assert.IsFalse(isAnyValueNotCorrect, $"dimLength={dimLength}, Expected = {input[m1 * dimLength + n1]}, Actual [{m1},{n1}] = {grid[m1, n1]}");
            }            
        }

        /// <summary>
        /// Verify that GridAsString set method does not change/set the grid for a string with length>81
        /// </summary>
        [TestMethod]
        public void Item003_GridAsString_Length144()
        {
            Item item = new Item(gridDefault);
            int[,] grid0 = item.Grid;

            string input = GetGridValuesString(12);
            item.GridAsString = input;

            int[,] grid1 = item.Grid;

            Item_AreGridsEqual(grid0, grid1);
        }

        /// <summary>
        /// Verify that GridAsString set does not change/set the grid for a string with a length, for which the square root does not result in a whole number. eg √40 = 6.32455, != whole
        /// </summary>
        [TestMethod]
        public void Item004_GridAsString_Length40()
        {
            Item item = new Item(gridDefault);
            int[,] grid0 = item.Grid;

            string input = GetGridValuesStringRandom(40);
            item.GridAsString = input;

            int[,] grid1 = item.Grid;

            Item_AreGridsEqual(grid0, grid1);
        }

        /// <summary>
        /// Verify that GridAsStringJson set creates the correct grid based in the provided string
        /// </summary>
        [TestMethod]
        public void Item005_GridAsStringCommaSeperated()
        {
            Item item = new Item();
            
            int dimLength = 9;
            int[,] grid = new int[dimLength, dimLength];
            string jsonString = "";

            int value = 0;
            for (int i = 0; i < dimLength * dimLength; i++)
            {
                value = random.Next(1, 10);
                grid[i / dimLength, i % dimLength] = value;
                jsonString += value + ",";
            }
            jsonString.Substring(0, jsonString.Length - 1);

            item.GridAsStringCommaSeperated = jsonString;

            Item_AreGridsEqual(grid, item.Grid);
        }

        /// <summary>
        /// Verify that GridAsStringWithSeperator can handle any kind of seperator and is able to pick out the numbers from the string
        /// </summary>
        [TestMethod]
        public void Item006_GridAsStringWithSeperators()
        {
            Item item = new Item();

            int dimLength = 9;
            int[,] grid = new int[dimLength, dimLength];
            string inputString = "";

            string[] seperators = new string[] { "a", "B", "[", " ", "  ", "$% #@", "Winnie the Pooh" };

            int value = 0;
            for (int i = 0; i < (dimLength * dimLength) - 1; i++)
            {
                value = random.Next(1, 10);
                grid[i / dimLength, i % dimLength] = value;
                inputString += value + seperators[random.Next(0, seperators.Length)];
            }
            value = random.Next(1, 10);
            grid[dimLength, dimLength] = value;
            inputString += value;

            item.GridAsStringWithSeperators = inputString;
            
            Item_AreGridsEqual(grid, item.Grid);
        }




        /// <summary>
        /// Verify that all values on two 2D grids are equal
        /// </summary>
        /// <param name="grid0"></param>
        /// <param name="grid1"></param>
        private void Item_AreGridsEqual(int[,] grid0, int[,] grid1)
        {
            Assert.AreEqual(grid0.Rank, grid1.Rank);
            Assert.AreEqual(grid0.GetLength(0), grid1.GetLength(0));
            Assert.AreEqual(grid0.GetLength(1), grid1.GetLength(1));
            for (int m = 0; m < grid0.GetLength(0); m++)
            {
                for (int n = 0; n < grid0.GetLength(1); n++)
                {
                    Assert.AreEqual(grid0[m, n], grid1[m, n], $"grid0[{m},{n}]={grid0[m, n]}, grid1[{m},{n}]={grid1[m, n]}");
                }
            }
        }

        private void Item_AreGridsValuesNotEqual(int[,] grid0, int[,] grid1)
        {
            Assert.AreEqual(grid0.Rank, grid1.Rank);
            Assert.AreEqual(grid0.GetLength(0), grid1.GetLength(0));
            Assert.AreEqual(grid0.GetLength(1), grid1.GetLength(1));
            bool isAnyValueNotEqual = false;
            int m1 = -1;
            int n1 = -1;
            for (int m = 0; m < grid0.GetLength(0); m++)
            {
                for (int n = 0; n < grid0.GetLength(1); n++)
                {
                    if (grid0[m, n] != grid1[m, n]) { isAnyValueNotEqual = true; m1 = m; n1 = n; break; };
                }
                if (isAnyValueNotEqual) { break; }
            }
            Assert.IsTrue(isAnyValueNotEqual, $"grid0[{m1},{n1}]={grid0[m1, n1]}, grid1[{m1},{n1}]={grid1[m1, n1]}");
        }




        /// <summary>
        /// Sets the values of the specified grid to a random value, 1-9
        /// </summary>
        private int[,] GetGridValuesRandom(int dimLength)
        {
            int[,] grid = new int[dimLength,dimLength];
            for (int m = 0; m < dimLength; m++)
            {
                for (int n = 0; n < dimLength; n++)
                {
                    grid[m, n] = random.Next(1, 10);
                }
            }
            return grid;
        }

        /// <summary>
        /// Get a string filled with values for a grid with the specified dimension dimLength
        /// Fills each cell with its largest dimension-value, eg: [0,0] will be 0, [1,1] will be 1, [1,2] will be 2, [6,4] = 6, [9,9] = 9 
        /// </summary>
        /// <param name="dimLength"></param>
        /// <returns></returns>
        private string GetGridValuesString(int dimLength)
        {
            dimLength = Math.Abs(dimLength);
            string value = "";
            for (int i = 0; i < dimLength*dimLength; i++)
            {
                value += Math.Max(i / dimLength, i % dimLength);
            }
            return value;
        }

        private string GetGridValuesStringRandom(int length)
        {
            string value = "";
            for (int i =0; i < length; i++)
            {
                value += random.Next(1, 10);
            }
            return value;
        }


    }



    [TestClass]
    public class SudokuSolvere_HelperMethods_T
    {
        SudokuSolver Solver = new SudokuSolver();

        [TestMethod]
        public void Solver_Helpers_SquareRoot000_IdealInput()
        {
            int[] inputValues = { 2, 3, 5, 9, 12, 100 };
            foreach (int i in inputValues)
            {
                Assert.AreEqual(i, Solver.SquareRoot((int)Math.Pow(i, 2)));
            }
        }

        [TestMethod]
        public void Solver_Helper_SquareRoot001_NonIdealInput()
        {
            List<int[]> inputValues = new List<int[]>();
            inputValues.Add(new int[2] { 82, 9 });
            inputValues.Add(new int[2] { 24, 4 });
            inputValues.Add(new int[2] { 260, 16 });
            inputValues.Add(new int[2] { 1, 1 });
            inputValues.Add(new int[2] { 0, 0 });
            inputValues.Add(new int[2] { -82, 9 });
            inputValues.Add(new int[2] { -24, 4 });

            foreach (int[] input in inputValues)
            {
                Assert.AreEqual(input[1], Solver.SquareRoot(input[0]));
            }
        }

        [TestMethod]
        public void Solver_Helpers_IsSudokuValid000()
        {
            int[,] input = new int[9, 9];
            Assert.IsTrue(Solver.IsSudokuValid(input), "Input is filled with 0's");

            input[0, 0] = -1;
            Assert.IsFalse(Solver.IsSudokuValid(input), "Input cell [0, 0] = -1");

            for (int m = 0; m < input.GetLength(0); m++)
            {
                for (int n = 0; n < input.GetLength(1); n++)
                {
                    input[m, n] = 1;
                }
            }
            Assert.IsFalse(Solver.IsSudokuValid(input), "Input is filled with 1's");

            input = new int[9, 9]
            {
                { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 4, 5, 6, 7, 8, 9, 1, 2, 3 },
                { 7, 8, 9, 1, 2, 3, 4, 5, 6 },
                { 2, 3, 4, 5, 6, 7, 8, 9, 1 },
                { 5, 6, 7, 8, 9, 1 ,2, 3, 4 },
                { 8, 9, 1, 2, 3, 4, 5, 6, 7 },
                { 3, 4, 5, 6, 7, 8, 9, 1, 2 },
                { 6, 7, 8, 9, 1, 2, 3, 4, 5 },
                { 9, 1, 2, 3, 4, 5, 6, 7, 8 }
            };
            Assert.IsTrue(Solver.IsSudokuValid(input), "Full grid, no errors");

            input[0, 0] = 2;
            Assert.IsFalse(Solver.IsSudokuValid(input), "Full grid, with erroneous cell [0,0]=2");
        }
    }


    [TestClass]
    public class SudokuSolver_FindValidNums_T
    {
        [TestMethod]
        public void Solver_FindValid000()
        {
            SudokuSolver solver = new SudokuSolver();
            

        }
    }
}

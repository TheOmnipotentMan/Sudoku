using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Diagnostics;


namespace Sudoku
{
    /// <summary>
    /// Item contain a sudoku and its information
    /// </summary>
    public class Item
    {
        public string Name { get; set; }
        public bool Completed { get; set; }
        public bool Bookmarked { get; set; }
        public int Rating { get; set; }

        private int[,] grid;

        public string GridAsString
        {
            get
            {
                string x = "";
                foreach (int b in grid) { x += b; }
                return x;
            }
            set
            {
                // Currently only digits 0-9 can be handled, as each char represents a field on the grid, so a value of say 24 will be treated as 2 and 4
                if (value.Length > 1 && value.Length <= 81 && Math.Sqrt(value.Length) % 1 == 0)
                {
                    int dimLength = (int)Math.Sqrt(value.Length);
                    grid = new int[dimLength, dimLength];
                    int x;
                    for (int m = 0; m < dimLength; m++)
                    {
                        for (int n = 0; n < dimLength; n++)
                        {
                            x = -1;
                            int.TryParse(value[m * dimLength + n].ToString(), out x);
                            grid[m, n] = x;
                        }
                    }
                }
            }
        }

        public string GridAsStringCommaSeperated
        {
            set
            {
                if (value.Length > 1)
                {
                    List<int> values = new List<int>();
                    int v;
                    int start = 0;
                    // Get all the values from the string, comma seperated
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (value[i] == ',')
                        {
                            v = -1;
                            int.TryParse(value.Substring(start, i - start), out v);
                            values.Add(v);
                            i++;
                            start = i;
                        }
                    }

                    // Log a message if the square root is not a whole number
                    if (Math.Sqrt(values.Count) % 1 != 0)
                    {
                        Debug.WriteLine($"Item: GridAsStringJson().Set the number of values found does not a whole number as its square root, the values will likely not line up to their intended position and some values might be lost! values.Count = {values.Count}");
                    }

                    // Set the values to grid
                    int dimLength = (int)Math.Sqrt(values.Count);
                    grid = new int[dimLength, dimLength];
                    for (int m = 0; m < dimLength; m++)
                    {
                        for (int n = 0; n < dimLength; n++)
                        {
                            grid[m, n] = values.ElementAt(m * dimLength + n);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parse a string with undetermined seperators
        /// NOT WORKING!!
        /// </summary>
        public string GridAsStringWithSeperators
        {
            set
            {
                if (value.Length > 1)
                {
                    List<int> values = new List<int>();
                    int v;
                    int start = 0;
                    // Get all the values from the string, seperated by something that is not a digit
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (!char.IsDigit(value[i]))
                        {
                            if (start < i)
                            {
                                v = -1;
                                int.TryParse(value.Substring(start, i - start), out v);
                                values.Add(v);
                            }
                            start = i + 1;
                        }
                    }

                    // Log a message if the square root is not a whole number
                    if (Math.Sqrt(values.Count) % 1 != 0)
                    {
                        Debug.WriteLine($"Item: GridAsStringWithSeperators().Set the number of values found does not a whole number as its square root, the values will likely not line up to their intended position and some values might be lost! values.Count = {values.Count}");
                    }

                    // Set the values to grid
                    int dimLength = (int)Math.Sqrt(values.Count);
                    grid = new int[dimLength, dimLength];
                    for (int m = 0; m < dimLength; m++)
                    {
                        for (int n = 0; n < dimLength; n++)
                        {
                            grid[m, n] = values.ElementAt(m * dimLength + n);
                        }
                    }
                }
            }
        }


        public Item()
        {
            Name = "";
            Completed = false;
            Bookmarked = false;
            Rating = 0;
            grid = new int[9, 9];
        }


        /// <summary>
        /// Create a new sudoku Item
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="name"></param>
        /// <param name="completed"></param>
        /// <param name="bookmarked"></param>
        /// <param name="rating"></param>
        public Item(int[,] grid, string name = "", bool completed = false, bool bookmarked = false, int rating = 0)
        {
            this.grid = (int[,])grid.Clone();

            Name = name;
            Completed = completed;
            Rating = rating;
            Bookmarked = bookmarked;
        }

        

        public int[,] Grid
        {
            get { return (int[,])grid.Clone(); }
            set { grid = (int[,])value.Clone(); }
        }

        
    }
}

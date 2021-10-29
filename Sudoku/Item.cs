using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    /// <summary>
    /// Item contain a sudoku and its information
    /// </summary>
    public class Item
    {
        public string Name;

        public bool Completed;
        public bool Bookmarked;

        public int Rating;


        private int[,] grid;


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
            this.grid = grid;

            Name = name;
            Rating = rating;
            Completed = completed;
            Bookmarked = bookmarked;
        }

        public int[,] Grid
        {
            get { return grid; }
        }

        public string GridAsString()
        {
            string x = "";
            foreach (byte b in grid) { x += b; }
            return x;
        }
    }
}

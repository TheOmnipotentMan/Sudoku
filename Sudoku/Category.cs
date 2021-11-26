using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Text.Json.Serialization;





namespace Sudoku
{
    /// <summary>
    /// Category grouping of sudoku items.
    /// Grouping basis can be decided freely, but as an example, it can be difficulty level
    /// </summary>
    public class Category
    {

        [JsonInclude]
        public string Name { get; set; }

        [JsonInclude]
        public List<Item> Items { get; set; }

        /// <summary>
        /// Create a new category
        /// </summary>
        /// <param name="name">Name of this category, should be based on grouping logic of all categories within this StorageGroup, eg difficulty</param>
        public Category(string name)
        {
            Name = name;
            Items = new List<Item>();
        }

        /// <summary>
        /// Add a new sudoku Item to the category
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="name"></param>
        /// <param name="rating"></param>
        /// <param name="completed"></param>
        /// <param name="bookmarked"></param>
        public void AddItem(int[,] grid, string name = "", bool completed = false, bool bookmarked = false, int rating = 0)
        {
            Items.Add(new Item(grid, name, completed, bookmarked, rating));
        }

        /// <summary>
        /// Remove any/all item(s) matching the specified name from the category
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveItem(string name)
        {
            bool itemRemoved = false;
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Name == name)
                {
                    Items.RemoveAt(i);
                    itemRemoved = true;
                }
            }
            return itemRemoved;
        }

        /// <summary>
        /// Remove the item at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveItem(int index)
        {
            if (index >= Items.Count || index < 0) { return false; }
            else
            {
                Items.RemoveAt(index);
                return true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Text.Json.Serialization;



namespace Sudoku
{
    /// <summary>
    /// Sudoku storage group. Contains a collection of categories, containing sudoku items.
    /// </summary>
    public class StorageGroup
    {
        [JsonInclude]
        public string Name { get; set; }
        [JsonInclude]
        public string Source { get; set; }

        [JsonInclude]
        public List<Category> Categories { get; set; }



        public StorageGroup(string name, string source = "")
        {
            if (source == null) { source = ""; }

            Name = name;
            Source = source;
            Categories = new List<Category>();
        }




        /// <summary>
        /// Add a new category
        /// </summary>
        /// <param name="name">Name of the new category</param>
        public void AddCategory(string name)
        {
            Categories.Add(new Category(name));
        }

        /// <summary>
        /// Remove a category by name (first found)
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if succesfull, false if not</returns>
        public bool RemoveCategory(string name)
        {
            for (int i = 0; i < Categories.Count; i++)
            {
                if (Categories[i].Name == name)
                {
                    Categories.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove a category by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>True if succesfull, false if not</returns>
        public bool RemoveCategory(int index)
        {
            if (index >= Categories.Count || index < 0) { return false; }
            else
            {
                Categories.RemoveAt(index);
                return true;
            }
        }

        /// <summary>
        /// Add a new item to a category, by category name (first found)
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="grid"></param>
        /// <param name="name"></param>
        /// <param name="rating"></param>
        /// <param name="completed"></param>
        /// <param name="bookmarked"></param>
        /// <returns>True if succesfull, false if not (likely because no category was found matching the given categoryName)</returns>
        public bool AddItem(string categoryName, int[,] grid, string name = "", bool completed = false, bool bookmarked = false, int rating = 0)
        {
            for (int i = 0; i < Categories.Count; i++)
            {
                if (Categories[i].Name == categoryName)
                {
                    Categories[i].AddItem(grid, name, completed, bookmarked, rating);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add a new item to a category, by category index
        /// </summary>
        /// <param name="categoryIndex"></param>
        /// <param name="grid"></param>
        /// <param name="name"></param>
        /// <param name="rating"></param>
        /// <param name="completed"></param>
        /// <param name="bookmarked"></param>
        /// <returns>True if succesfull, false if not (likely because given index was out-of-bounds for Categories)</returns>
        public bool AddItem(int categoryIndex, int[,] grid, string name = "", bool completed = false, bool bookmarked = false, int rating = 0)
        {
            if (categoryIndex >= Categories.Count || categoryIndex < 0) { return false; }
            else
            {
                Categories[categoryIndex].AddItem(grid, name, completed, bookmarked, rating);
                return true;
            }
        }

        public bool AddItemToLastCategory(int[,] grid, string name = "", bool completed = false, bool bookmarked = false, int rating = 0)
        {
            if (Categories.Count == 0) { return false; }
            else
            {
                Categories.Last().AddItem(grid, name, completed, bookmarked, rating);
                return true;
            }
        }
    }
}

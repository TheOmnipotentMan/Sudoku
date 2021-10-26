using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;





namespace Sudoku
{
    /// <summary>
    /// Get grid values for sudoku puzzles, by varying methods
    /// </summary>
    class SudokuSource
    {
        /// <summary>
        /// Sudoku storage group. Contains a collection of categories, containing sudoku items.
        /// </summary>
        public class StorageGroup
        {
            public string Name;
            public string Source;

            public List<Category> Categories;



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
            public bool AddItem(string categoryName, int[,] grid, string name = "", byte rating = 0, bool completed = false, bool bookmarked = false)
            {
                for (int i = 0; i < Categories.Count; i++)
                {
                    if (Categories[i].Name == categoryName)
                    {
                        Categories[i].AddItem(grid, name, rating, completed, bookmarked);
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
            public bool AddItem(int categoryIndex, int[,] grid, string name = "", byte rating = 0, bool completed = false, bool bookmarked = false)
            {
                if (categoryIndex >= Categories.Count || categoryIndex < 0) { return false; }
                else
                {
                    Categories[categoryIndex].AddItem(grid, name, rating, completed, bookmarked);
                    return true;
                }
            }

            public bool AddItemToLastCategory(int[,] grid, string name = "", byte rating = 0, bool completed = false, bool bookmarked = false)
            {
                if (Categories.Count == 0) { return false; }
                else
                {
                    Categories.Last().AddItem(grid, name, rating, completed, bookmarked);
                    return true;
                }
            }


        }

        /// <summary>
        /// Category grouping of sudoku items.
        /// Grouping basis can be decided freely, but as an example, it can be difficulty level
        /// </summary>
        public class Category
        {
            /// <summary>
            /// Name of this categ
            /// </summary>
            public string Name;

            public List<Item> Items;

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
            public void AddItem(int[,] grid, string name = "", byte rating = 0, bool completed = false, bool bookmarked = false)
            {
                Items.Add(new Item(grid, name, rating, completed, bookmarked));
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

        public class Item
        {
            public string Name;

            public byte Rating;
            public bool Completed;
            public bool Bookmarked;

            private int[,] grid;

            public Item (int[,] grid, string name = "", byte rating = 0, bool completed = false, bool bookmarked = false)
            {
                // TODO check if data is actually copied over, ie persistent
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






        /// <summary>
        /// Retrieves data from a SudokuStorage xml file 
        /// </summary>
        public class XmlStorage
        {
            private string xmlFilePath;

            private XmlReader reader;
            private XmlReaderSettings settings;


            /// <summary>
            /// Create a new StorageXml class instance
            /// </summary>
            /// <param name="path">Path to xml file to read. Must be a valid path</param>
            public XmlStorage(string path)
            {
                // XmlReader
                settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas.Add("SudStorSchema.xsd", "sudStorSchema.xsd");
                xmlFilePath = path;
            }


            /// <summary>
            /// Is the xml reader able to read the file.
            /// Call after contstuctor, and before using any other method, to make sure everything behaves as it should
            /// </summary>
            /// <returns></returns>
            public bool IsFileReadable()
            {
                RestartReader();
                return reader.ReadToFollowing("Storage");
            }


            /// <summary>
            /// Get a collection of all available storages in the xml file
            /// </summary>
            /// <returns></returns>
            public string[] GetStorages()
            {
                RestartReader();

                List<string> storages = new List<string>();
                while (reader.ReadToFollowing("Storage"))
                {
                    storages.Add(reader.GetAttribute("Name"));
                    reader.Skip();
                }

                return storages.ToArray();
            }


            /// <summary>
            /// Get a collection of all available categories in the given storage in the xml file
            /// </summary>
            /// <param name="storage"></param>
            /// <returns></returns>
            public string[] GetCategories(string storage)
            {
                RestartReader();

                List<string> categories = new List<string>();

                while (reader.ReadToFollowing("Storage"))
                {
                    if (reader.GetAttribute("Name") == storage)
                    {
                        while (reader.ReadToFollowing("Category"))
                        {
                            categories.Add(reader.GetAttribute("Name"));
                            reader.Skip();
                        }
                    }
                    else
                    {
                        reader.Skip();
                    }                    
                }

                return categories.ToArray();
            }


            /// <summary>
            /// Get a collection of all available categories in the given storage in the xml file
            /// </summary>
            /// <param name="storage"></param>
            /// <returns></returns>
            public string[] GetCategories(int storageIndex)
            {
                RestartReader();

                List<string> categories = new List<string>();

                bool isEndReached = false;
                int i = 0;
                while (i < storageIndex && !isEndReached)
                {
                    isEndReached = !reader.ReadToFollowing("Storage");
                    i++;
                }

                while (reader.ReadToFollowing("Category") && !isEndReached)
                {
                    categories.Add(reader.GetAttribute("Name"));
                    reader.Skip();
                }

                return categories.ToArray();
            }



            /// <summary>
            /// Start the reader from the beginning
            /// </summary>
            private void RestartReader()
            {
                reader = XmlReader.Create(xmlFilePath, settings);
                reader.MoveToContent();
            }
            
        }







        private List<int[]> prefabs;

        private Random rand = new Random();

        private bool IsXmlAvailable = false;
        public XmlStorage StorageXml;

        public List<StorageGroup> Storages;





        public SudokuSource()
        {
            InstatiateStorage();
            PopulatePrefabList();
        }




        /// <summary>
        /// Instatiate a new empty storage collection for Storages
        /// Can be used to clear the current content Storages
        /// </summary>
        public void InstatiateStorage()
        {
            Storages = new List<StorageGroup>();
        }



        private void InstantiateStorageXml(string path)
        {
            StorageXml = new XmlStorage(path);
            IsXmlAvailable = StorageXml.IsFileReadable();
            if (!IsXmlAvailable) { Debug.WriteLine($"SudokuSource: InstantiateStorageXml() Xml is not available"); }
        }


        










        //  --------------------------------------------------------------
        //  ----------------        PREFAB METHODS        ----------------
        //  --------------------------------------------------------------

        /// <summary>
        /// Internal prefab sudokus, always available, mainly for testing
        /// </summary>
        private void PopulatePrefabList()
        {
            prefabs = new List<int[]>();
            prefabs.Add(new int[] { 3, 0, 0, 0, 6, 9, 4, 5, 0, 0, 0, 2, 0, 0, 0, 8, 0, 6, 0, 1, 0, 4, 0, 0, 0, 2, 3, 2, 0, 3, 0, 9, 0, 5, 0, 7, 0, 7, 0, 3, 2, 0, 0, 0, 9, 0, 9, 0, 0, 0, 0, 0, 3, 0, 8, 0, 0, 5, 7, 3, 0, 0, 4, 6, 3, 0, 0, 0, 0, 0, 0, 0, 7, 0, 0, 1, 4, 0, 0, 0, 0 });
            prefabs.Add(new int[] { 4, 0, 0, 3, 0, 8, 0, 0, 0, 0, 0, 7, 0, 5, 0, 1, 0, 6, 0, 0, 1, 4, 0, 7, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 8, 0, 3, 0, 0, 2, 0, 0, 0, 0, 4, 0, 5, 7, 3, 0, 0, 0, 9, 0, 8, 0, 0, 0, 0, 5, 0, 0, 0, 9, 0, 0, 0, 0, 6, 5, 0, 3, 0, 2, 0 });
        }

        /// <summary>
        /// Get a internal prefab sudoku. Small collection for testing purposes
        /// </summary>
        /// <param name="x">Index of desired sudoku. Random if not specified</param>
        /// <returns></returns>
        public int[] GetPrefab(int x = 0)
        {
            if (x < 0 || x >= prefabs.Count) { Debug.WriteLine($"SudokuGenerator.GetPrefab() out-of-bounds value for x {x}"); x = rand.Next(0, prefabs.Count); }
            return prefabs.ElementAt(x);
        }

        /// <summary>
        /// Get the total number of internal prefab sudokus
        /// </summary>
        /// <returns></returns>
        public int GetPrefabCount()
        {
            return prefabs.Count;
        }













        //  --------------------------------------------------------------
        //  ----------------         XML METHODS          ----------------
        //  --------------------------------------------------------------

        /// <summary>
        /// Load the specified xml file into the active Storages
        /// </summary>
        /// <param name="path"></param>
        public void LoadXmlFileToStorage(string path)
        {
            // Make sure the file is valid
            if (!System.IO.File.Exists(path) || !(path.Substring(path.Length - 4) == ".xml")) { Debug.WriteLine($"SudokuSource: LoadXmlFileToStorage() recieved an invalid file path, {path}"); return; }

            // Create an XmlReader for the given file at path
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler(XmlFileValidationCallBack);
            settings.Schemas.Add("SudStorSchema.xsd", "sudStorSchema.xsd");
            XmlReader reader = XmlReader.Create(path, settings);
            reader.MoveToContent();

            // Whether or not to write debug lines to the output, providing info on the reader and any relevant StartElement node it reads 
            bool showDebug = false;

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "Storage":
                            {
                                if (showDebug) { Debug.WriteLine($"SudokuSource: LoadXmlFileToStorage() reader found storage {reader.GetAttribute("Name")}"); }
                                Storages.Add(new StorageGroup(reader.GetAttribute("Name"), reader.GetAttribute("Source")));
                                break;
                            }
                        case "Category":
                            {
                                if (showDebug) { Debug.WriteLine($"SudokuSource: LoadXmlFileToStorage() reader found category {reader.GetAttribute("Name")}"); }
                                Storages.Last().AddCategory(reader.GetAttribute("Name"));
                                break;
                            }
                        case "Sudoku":
                            {
                                if (showDebug) { Debug.WriteLine($"SudokuSource: LoadXmlFileToStorage() reader found sudoku"); }
                                Storages.Last().AddItemToLastCategory(ConvertStringToIntArray(reader.ReadElementContentAsString()), reader.GetAttribute("Name"), ConvertStringToByte(reader.GetAttribute("Rating")), ConvertStringToBool(reader.GetAttribute("Completed")), ConvertStringToBool(reader.GetAttribute("Bookmarked")));
                                break;
                            }
                        // Log a debug statement if no case was found
                        default:
                            {
                                Debug.WriteLine($"SudokuSource: LoadXmlFileTOStorage() found an Element without a matching case, any data in this element is not parsed. Element = {reader.Name}");
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// ValidationCallBack for LoadXmlFileToStorage() ValidationEventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void XmlFileValidationCallBack(object sender, System.Xml.Schema.ValidationEventArgs args)
        {
            Debug.WriteLine($"SudokuSource: XmlFileValidationCallBack() validation event occured in {sender.ToString()}");
            Debug.WriteLine($"SudokuSource: XmlFileValidationCallBack() {args.Severity.ToString()}, {args.Message}");
        }










        //  --------------------------------------------------------------
        //  ----------------         XML METHODS          ----------------
        //  --------------------------------------------------------------

        /// <summary>
        /// Get a specified sudoku
        /// </summary>
        /// <param name="iStorage">Storage index</param>
        /// <param name="iCategory">Category index</param>
        /// <param name="iItem">Item (sudoku) index</param>
        /// <returns></returns>
        public int[,] GetSudoku(int iStorage, int iCategory, int iItem)
        {
            return Storages[iStorage].Categories[iCategory].Items[iItem].Grid;
        }

        /// <summary>
        /// Get a random sudoku
        /// </summary>
        /// <returns></returns>
        public int[,] GetSudokuRandom()
        {
            int i1 = rand.Next(Storages.Count);
            int i2 = rand.Next(Storages[i1].Categories.Count);
            int i3 = rand.Next(Storages[i1].Categories[i2].Items.Count);
            return Storages[i1].Categories[i2].Items[i3].Grid;
        }

        /// <summary>
        /// Write all the content of Storages to the output, debug
        /// </summary>
        public void DebugPrintStoragesContentToOutput()
        {
            
            for (int curStor = 0; curStor < Storages.Count; curStor++)
            {
                Debug.WriteLine($"SudokuSource: DebugPrintStoragesContentToOutput(), storage {curStor}, name = {Storages[curStor].Name}, source = {Storages[curStor].Source}, content# = {Storages[curStor].Categories.Count}");

                for (int curCat = 0; curCat < Storages[curStor].Categories.Count; curCat++)
                {
                    Debug.WriteLine($"SudokuSource: DebugPrintStoragesContentToOutput(), category {curCat}, name = {Storages[curStor].Categories[curCat].Name}, content# = {Storages[curStor].Categories[curCat].Items.Count}");

                    for (int curItem = 0; curItem < Storages[curStor].Categories[curCat].Items.Count; curItem++)
                    {
                        Debug.WriteLine($"SudokuSource: DebugPrintStoragesContentToOutput(), item {curItem}, name = {Storages[curStor].Categories[curCat].Items[curItem].Name}, content = {Storages[curStor].Categories[curCat].Items[curItem].GridAsString()}");
                    }
                }
            }
        }















        /// <summary>
        /// Convert a string representation of a sudoku grid to a int[9, 9].
        /// Value will default to 0 if !char.IsDigit
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private int[,] ConvertStringToIntArray(string s)
        {            
            int[,] irry = new int[9, 9];
            for (int i = 0; i < irry.Length; i++)
            {
                irry[(i - (i % 9)) / 9, i % 9] = char.IsDigit(s[i]) ? int.Parse(s[i].ToString()) : 0;
            }
            return irry;
        }

        /// <summary>
        /// Convert a string to a byte, returns 0 if unsuccesfull
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private byte ConvertStringToByte(string s)
        {
            byte b = 0;
            byte.TryParse(s, out b);
            return b;
        }

        /// <summary>
        /// Convert a string to a bool.
        /// Recognises "true", "True", "1"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool ConvertStringToBool(string s)
        {
            return (s == "true" || s == "True" || s == "1");
        }


    }
}

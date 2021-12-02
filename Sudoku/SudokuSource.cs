using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.IO;
using System.Xml;
using System.Text.Json;
using System.Diagnostics;





namespace Sudoku
{
    /// <summary>
    /// Get grid values for sudoku puzzles, by varying methods
    /// </summary>
    class SudokuSource
    {
        

        

        








        private List<int[]> prefabs;

        private Random rand = new Random();

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














        //  --------------------------------------------------------------
        //  ----------------        PREFAB METHODS        ----------------
        //  --------------------------------------------------------------
        #region PREFAB_METHODS

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

        #endregion PREFAB_METHODS








        //  --------------------------------------------------------------
        //  ----------------         XML METHODS          ----------------
        //  --------------------------------------------------------------
        #region XML_METHODS

        /// <summary>
        /// Load the specified xml file into the active Storages
        /// </summary>
        /// <param name="path"></param>
        public List<StorageGroup> GetContentFromXmlFile(string path)
        {
            // Content to return
            List<StorageGroup> content = new List<StorageGroup>();

            // Make sure the file is valid
            if (!File.Exists(path) || !(path.Substring(path.Length - 4) == ".xml")) { Debug.WriteLine($"SudokuSource: LoadXmlFileToStorage() recieved an invalid file path, {path}"); return content; }

            // Create an XmlReader for the given file at path
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler(XmlFileValidationCallBack);
            settings.Schemas.Add("SudStorSchema.xsd", "sudStorSchema.xsd");
            XmlReader reader = XmlReader.Create(path, settings);
            reader.MoveToContent();

            // Whether or not to write debug lines to the output, providing info on any relevant Element/node the reader is reading
            bool showDebug = false;

            // Sudoku values, will be default or overridden by value found in file
            int mLength;
            int nLength;
            string gridString;
            bool isCompleted;
            bool isBookmarked;
            int rating;
            XmlReader reader1;

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        // Create a new Storage
                        case "Storage":
                            {
                                if (showDebug) { Debug.WriteLine($"SudokuSource: LoadXmlFileToStorage() reader found storage {reader.GetAttribute("Name")}"); }
                                content.Add(new StorageGroup(reader.GetAttribute("Name"), reader.GetAttribute("Source")));
                                break;
                            }
                        // Create a new Category, within the most recent Storage
                        case "Category":
                            {
                                if (showDebug) { Debug.WriteLine($"SudokuSource: LoadXmlFileToStorage() reader found category {reader.GetAttribute("Name")}"); }
                                content.Last().AddCategory(reader.GetAttribute("Name"));
                                break;
                            }
                        // Create a new Sudoku, within the most recent Category
                        case "Sudoku":
                            {
                                if (showDebug) { Debug.WriteLine($"SudokuSource: LoadXmlFileToStorage() reader found sudoku"); }

                                // Set sudoku values to default
                                mLength = 9;
                                nLength = 9;
                                gridString = "";
                                isCompleted = false;
                                isBookmarked = false;
                                rating = 0;

                                // Create a reader for the Sudoku Subtree, and read()
                                reader1 = reader.ReadSubtree();                                
                                while (reader1.Read())
                                {
                                    if (reader1.IsStartElement())
                                    {
                                        switch (reader1.Name)
                                        {
                                            case "Sudoku":
                                                {
                                                    // StartElement, does contain attributes that can be obtained here if desired
                                                    // Currently, reader gets the attributes after reader1 has finished, which eliminates the need for some local variables.
                                                    // If this case if removed reader1 will hit on the default case and log a debug statement for this element
                                                    break;
                                                }
                                            case "M-Length":
                                                {
                                                    mLength = reader1.ReadElementContentAsInt();
                                                    if (showDebug) { Debug.WriteLine($"SudokuSource: LoadXmlFileToStorage() reader1 found M-Length {mLength}"); }
                                                    break;
                                                }
                                            case "N-Length":
                                                {
                                                    nLength = reader1.ReadElementContentAsInt();
                                                    if (showDebug) { Debug.WriteLine($"SudokuSource: LoadXmlFileToStorage() reader1 found N-Length {nLength}"); }
                                                    break;
                                                }
                                            case "Grid":
                                                {
                                                    gridString = RemoveWhitespaceFromString(reader1.ReadElementContentAsString());
                                                    if (showDebug) { Debug.WriteLine($"SudokuSource: LoadXmlFileToStorage() reader1 found Grid {gridString}"); }
                                                    break;
                                                }
                                            case "IsCompleted":
                                                {
                                                    isCompleted = reader1.ReadElementContentAsBoolean();
                                                    if (showDebug) { Debug.WriteLine($"SudokuSource: LoadXmlFileToStorage() reader1 found isCompleted {isCompleted}"); }
                                                    break;
                                                }
                                            case "IsBookmarked":
                                                {
                                                    isBookmarked = reader1.ReadElementContentAsBoolean();
                                                    if (showDebug) { Debug.WriteLine($"SudokuSource: LoadXmlFileToStorage() reader1 found isBookmarked {isBookmarked}"); }
                                                    break;
                                                }
                                            case "Rating":
                                                {
                                                    rating = reader1.ReadElementContentAsInt();
                                                    if (showDebug) { Debug.WriteLine($"SudokuSource: LoadXmlFileToStorage() reader1 found Rating {rating}"); }
                                                    break;
                                                }
                                            default:
                                                {
                                                    Debug.WriteLine($"SudokuSource: LoadXmlFileTOStorage() found an Element inside a sudoku without a matching case, any data in this element is not parsed. Element = {reader1.Name}");
                                                    break;
                                                }
                                        }
                                    }
                                }

                                // Read the Sudoku values from xml to Storages
                                content.Last().AddItemToLastCategory(ConvertStringToIntArray(gridString), reader.GetAttribute("Name"), isCompleted, isBookmarked, rating);
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

            return content;

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

        #endregion XML_METHODS








        //  --------------------------------------------------------------
        //  ----------------        JSON METHODS          ----------------
        //  --------------------------------------------------------------
        #region JSON_METHODS

        /// <summary>
        /// Load the content from the specified Json file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<StorageGroup> GetContentFromJsonFile(string path = "")
        {
            // Content to return
            List<StorageGroup> content = new List<StorageGroup>();

            // If no file was specified, use default
            if (string.IsNullOrWhiteSpace(path)) { path = "SudokuStorage.json"; }

            // Make sure the file exists, if not return
            if (!File.Exists(path) || !(path.Substring(path.Length - 5) == ".json"))
            {
                Debug.WriteLine($"SudokuSource: AddContentToJsonFile() recieved an invalid file, {path}");
                // return content;
            }

            // Try to retrieve content from file
            List<JsonStorageGroup> jsonStorage = new List<JsonStorageGroup>();
            try
            {
                string jsonString = File.ReadAllText(path);
                jsonStorage = JsonSerializer.Deserialize<List<JsonStorageGroup>>(jsonString);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"SudokuSource: GetContentFromJsonFile() threw exception {e}");
                return content;
            }

            content = ConvertJsonCollectionToStorage(jsonStorage);
            return content;
        }


        /// <summary>
        /// Add the specified storage collection to the default json file
        /// Currently not working, WriteToJson() is able to serialize but not actually write to file
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        public bool AddContentToJsonFile(List<StorageGroup> storage, string path = "")
        {
            // Is this method succesfull or not
            bool isSuccesfull = false;

            // If no file was specified, use default
            if (string.IsNullOrWhiteSpace(path)) { path = "SudokuStorage.json"; }

            // Make sure their is content to write to json
            if (storage.Count <= 0)
            {
                Debug.WriteLine($"SudokuSource: AddContentToJsonFile() storage did not contain any StorageGroups");
                return isSuccesfull;
            }
            else if (storage[0].Categories.Count <= 0)
            {
                Debug.WriteLine($"SudokuSource: AddContentToJsonFile() storageGroup did not contain any Categories");
                return isSuccesfull;
            }
            else if (storage[0].Categories[0].Items.Count <= 0)
            {
                Debug.WriteLine($"SudokuSource: AddContentToJsonFile() storageGroup.Categories did not contain any Items");
                return isSuccesfull;
            }

            // Make sure the file is valid, else return
            if (!File.Exists(path) || !(path.Substring(path.Length - 5) == ".json"))
            {
                Debug.WriteLine($"SudokuSource: AddContentToJsonFile() no existing json file found! Please create the file");
                //return isSuccesfull;
            }

            // Writing (serialising) options
            var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };

            WriteToJson(path, storage, options);

            return isSuccesfull;
        }

        
        /// <summary>
        /// Write the 'storage' collection to file.
        /// Currently, writing to file is not funtional, so actually writing to file is not possible. But serialization is
        /// </summary>
        /// <param name="path"></param>
        /// <param name="storage"></param>
        /// <param name="options"></param>
        private void WriteToJson(string path, List<StorageGroup> storage, JsonSerializerOptions options)
        {
            // Copy the content from 'storage' over to 
            List<JsonStorageGroup> jsonStorages = ConvertStorageToJsonReadableCollection(storage);

            string jsonString = JsonSerializer.Serialize(jsonStorages, options);
            Debug.WriteLine($"SudokuSource: WriteToJson() serialized -->");
            Debug.WriteLine(jsonString);

            Debug.WriteLine($"SudokuSource: WriteToJson() writing to file not implemented");

            // TODO implement writing to file
            /*
            File.WriteAllText(path, jsonString);
            */
        }


        /// <summary>
        /// Convert the collection of Json-objects, JsonStorageGroup containing JsonCategory containing JsonItem, to the structure of the sudoku storage collection.
        /// </summary>
        /// <param name="jsonStorage"></param>
        /// <returns></returns>
        private List<StorageGroup> ConvertJsonCollectionToStorage(List<JsonStorageGroup> jsonStorage)
        {
            List<StorageGroup> storage = new List<StorageGroup>();

            foreach (JsonStorageGroup jsonSG in jsonStorage)
            {
                storage.Add(new StorageGroup(jsonSG.StorageName, jsonSG.Source));

                foreach (JsonCategory jsonCat in jsonSG.Categories)
                {
                    storage.Last().Categories.Add(new Category(jsonCat.Name));

                    foreach (JsonItem jsonItem in jsonCat.Items)
                    {
                        Item item = new Item
                        {
                            Name = jsonItem.Name,
                            Completed = jsonItem.Completed,
                            Bookmarked = jsonItem.Bookmarked,
                            Rating = jsonItem.Rating,
                            GridAsString = jsonItem.Grid
                        };
                        storage.Last().Categories.Last().Items.Add(item);
                    }
                }
            }

            return storage;
        }


        /// <summary>
        /// Convert a sudoku storage collection to a collection of equivalent objects that are readable by JsonSerializer
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        private List<JsonStorageGroup> ConvertStorageToJsonReadableCollection(List<StorageGroup> storage)
        {
            List<JsonStorageGroup> jsonStorage = new List<JsonStorageGroup>();
            
            foreach (StorageGroup storageGroup in storage)
            {
                jsonStorage.Add(new JsonStorageGroup());
                jsonStorage.Last().StorageName = storageGroup.Name;
                jsonStorage.Last().Source = storageGroup.Source;
                jsonStorage.Last().Categories = new List<JsonCategory>();

                foreach (Category category in storageGroup.Categories)
                {
                    jsonStorage.Last().Categories.Add(new JsonCategory());
                    jsonStorage.Last().Categories.Last().Name = category.Name;
                    jsonStorage.Last().Categories.Last().Items = new List<JsonItem>();

                    foreach (Item item in category.Items)
                    {
                        jsonStorage.Last().Categories.Last().Items.Add(new JsonItem());
                        jsonStorage.Last().Categories.Last().Items.Last().Name = item.Name;
                        jsonStorage.Last().Categories.Last().Items.Last().Completed = item.Completed;
                        jsonStorage.Last().Categories.Last().Items.Last().Bookmarked = item.Bookmarked;
                        jsonStorage.Last().Categories.Last().Items.Last().Rating = item.Rating;
                        jsonStorage.Last().Categories.Last().Items.Last().Grid = item.GridAsString;
                    }
                }
            }

            return jsonStorage;
        }

        #endregion JSON_METHODS













        //  --------------------------------------------------------------
        //  ----------------       HELPER METHODS         ----------------
        //  --------------------------------------------------------------

        /// <summary>
        /// Return the input string without any whitespace
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string RemoveWhitespaceFromString(string input)
        {
            string output = "";

            foreach (char c in input)
            {
                if (!char.IsWhiteSpace(c))
                {
                    output += c;
                }
            }

            return output;
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
        /// Convert a string to a byte, returns default 0 if failed
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

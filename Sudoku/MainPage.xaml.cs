using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


using System.Diagnostics;




namespace Sudoku
{
    /// <summary>
    /// Play the game Sudoku
    /// </summary>
    public sealed partial class MainPage : Page
    {

        /// <summary>
        /// Source of Sudokus
        /// </summary>
        private SudokuSource SudokuSource;

        /// <summary>
        /// Sudoku Storage, containing StorageGroup(s), which Category(s), which contain Item(s)
        /// </summary>
        private List<StorageGroup> SudokuStorage;


        /// <summary>
        /// The current sudoku being played
        /// </summary>
        private Sudoku Sudoku;

        /// <summary>
        /// Allowed total length of sudoku, limited by UI
        /// </summary>
        private int SudokuAllowedLength = 81;
        /// <summary>
        /// Allowed length(0) of sudoku, limited by UI
        /// </summary>
        private int SudokuAllowedLength0 = 9;
        /// <summary>
        /// Allowed length(1) of sudoku, limited by UI
        /// </summary>
        private int SudokuAllowedLength1 = 9;



        /// <summary>
        /// The index values of currently selected sudoku from storage
        /// </summary>
        private int[] SelectedSudokuStorageIndex = new int[3];

        /// <summary>
        /// Should only valid input be allowed on the sudoku grid, or should mistakes be allowed
        /// </summary>
        private bool ValidInputOnly = false;

        /// <summary>
        /// Random number generator
        /// </summary>
        private Random rand = new Random();




        /// <summary>
        /// Path to provided xml file
        /// </summary>
        private string StandardXmlFilePath = "SudokuStorage.xml";

        
        // Font values for numbers on sudoku grid
        private int SudokuGridFontSize = 32;
        private Windows.UI.Text.FontWeight StartingValueFontWeight = Windows.UI.Text.FontWeights.Bold;
        private Windows.UI.Text.FontWeight DefaultFontWeight = Windows.UI.Text.FontWeights.Normal;


        private SolidColorBrush MessageTextBlockBrush;










        public MainPage()
        {
            this.InitializeComponent();

            Sudoku = new Sudoku();

            MessageTextBlockBrush = new SolidColorBrush() { Color = Windows.UI.Colors.Black };

            InstantiateSudokuSource();
            ImportSudokusFromXmlFile();
            InstaniateStorageInputGridCells();
        }








        // -----------------------------------------------------------------
        // --------------      Instantiating methods      ------------------
        // -----------------------------------------------------------------
        #region INST_METHODS

        /// <summary>
        /// Create a new SudokuSource
        /// </summary>
        private void InstantiateSudokuSource()
        {
            SudokuSource = new SudokuSource();
        }

        /// <summary>
        /// Instatiate a new empty storage collection for Storages
        /// Can be used to clear the current content Storages
        /// </summary>
        private void InstatiateSudokuStorage()
        {
            SudokuStorage = new List<StorageGroup>();
        }

        /// <summary>
        /// Create the UI elements to hold the values of the sudoku grid
        /// </summary>
        private void InstantiateSudokuGridCells()
        {
            SudokuGrid.Children.Clear();

            for (int m = 0; m < Sudoku.M; m++)
            {
                for (int n = 0; n < Sudoku.N; n++)
                {
                    SudokuGrid.Children.Add(new TextBlock() { Text = string.Empty, FontSize = SudokuGridFontSize, TextAlignment = 0, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
                    SudokuGrid.Children.Last().SetValue(Grid.RowProperty, m);
                    SudokuGrid.Children.Last().SetValue(Grid.ColumnProperty, n);
                }
            }
        }

        /// <summary>
        /// Create the UI elements to hold the values of the sudoku grid in storage
        /// </summary>
        private void InstantiateSudokuStorageGridCells()
        {
            SudokuStorageGrid.Children.Clear();

            for (int m = 0; m < Sudoku.M; m++)
            {
                for (int n = 0; n < Sudoku.N; n++)
                {
                    SudokuStorageGrid.Children.Add(new TextBlock() { Text = string.Empty, FontSize = SudokuGridFontSize, FontWeight = StartingValueFontWeight, TextAlignment = 0, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
                    SudokuStorageGrid.Children.Last().SetValue(Grid.RowProperty, m);
                    SudokuStorageGrid.Children.Last().SetValue(Grid.ColumnProperty, n);
                }
            }
        }

        private void InstaniateStorageInputGridCells()
        {
            StorageInputGrid.Children.Clear();

            for (int m = 0; m < Sudoku.M; m++)
            {
                for (int n = 0; n < Sudoku.N; n++)
                {
                    StorageInputGrid.Children.Add(new TextBox() { Text = string.Empty, FontSize = SudokuGridFontSize, FontWeight = StartingValueFontWeight, TextAlignment = 0, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Margin = new Thickness(4) });
                    StorageInputGrid.Children.Last().SetValue(Grid.RowProperty, m);
                    StorageInputGrid.Children.Last().SetValue(Grid.ColumnProperty, n);
                }
            }
        }

        #endregion INST_METHODS








        // -----------------------------------------------------------------
        // --------------      Sudoku Grid Handling      ------------------
        // -----------------------------------------------------------------
        #region GRID_HANDLING_METHODS

        /// <summary>
        /// Process the input on the sudoku grid from the player
        /// </summary>
        /// <param name="value">Value selected</param>
        /// <param name="cellCoor">Cell selected</param>
        private void ProcessSudokuInput(int value, int cellCoor = -1)
        {
            // If cellCoor was not specified, use the cell-index of the currently selected cell on SudokuCellButtonsGridView
            if (cellCoor == -1) { cellCoor = SudokuCellButtonsGridView.SelectedIndex; }

            // If any given value is smaller than 0, do nothing
            if (value < 0 || cellCoor < 0) { return; }

            int m = cellCoor / Sudoku.M;
            int n = cellCoor % Sudoku.N;

            // If the given cell contains a starting value, display an error message
            if (Sudoku.StartGrid[m, n] != 0) { ShowMessageBelowPlayGrid("Cell is staring value", 1); }
            // If only valid input is allowed, and the given value is invalid for the given cell, display an error message
            else if (ValidInputOnly && !Sudoku.IsCellValid(m, n, value)) { ShowMessageBelowPlayGrid("Invalid value", 1); }
            // Else enter the value onto the grid and clear any message
            else
            {
                LoadSingleValueToUIGridStart(m, n, value);
                ShowMessageBelowPlayGrid("");
                CheckForWinState();
            }
        }

        /// <summary>
        /// Check the current sudoku for its win state. ie if the grid is full, validate the sudoku
        /// </summary>
        private void CheckForWinState()
        {
            if (Sudoku.IsFull)
            {
                ValidateSudoku(true);
            }
        }


        /// <summary>
        /// Validate the current sudoku.
        /// Check for any errors and win-state
        /// </summary>
        private void ValidateSudoku(bool isFull = false)
        {
            if (isFull)
            {
                if (Sudoku.IsGridValid())
                {
                    ShowMessageBelowPlayGrid("CONGRATULATIONS!");
                }
            }
            else
            {
                if (Sudoku.IsGridValid())
                {
                    if (Sudoku.IsFull)
                    {
                        ShowMessageBelowPlayGrid("CONGRATULATIONS!");
                    }
                    else
                    {
                        ShowMessageBelowPlayGrid("No errors detected");
                    }
                }
                else
                {
                    ShowMessageBelowPlayGrid("Errors detected", 2);
                }
            }
        }

        /// <summary>
        /// Enable/Disable the appropriate numbers for the selected cell
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        private void SetValidNumberButtonsForCell(int m, int n)
        {
            if (ValidInputOnly)
            {
                bool[] validNums = Sudoku.GetValidNumbersForCell(m, n);
                for (int i = 0; i < validNums.Length; i++)
                {
                    SetIsEnabledForInputNumber(i, validNums[i]);
                }
            }
        }

        private async void GetSolution(bool useGuessOnDeadEnd = false)
        {
            Debug.WriteLine($"MainPage: GetSolution() starting, guessing = {useGuessOnDeadEnd}");

            // Start progress-ring to signal that the solution is being worked on
            SolveProgressRing.IsActive = true;
            
            int[,] solution = await Sudoku.GetSolution(useGuessOnDeadEnd);

            Debug.WriteLine($"MainPage: GetSolution() got solution");

            for (int m = 0; m < solution.GetLength(0); m++)
            {
                string row = "";
                for (int n = 0; n < solution.GetLength(1); n++)
                {
                    row += solution[m, n] + " ";
                    LoadSingleValueToUIGrid(m, n, solution[m, n]);
                }
                Debug.WriteLine(row);
            }

            // End the progress-ring
            SolveProgressRing.IsActive = false;
        }

        #endregion GRID_HANDLING_METHODS








        // -----------------------------------------------------------------
        // ---------------- SudokuSource, File, handeling   ----------------
        // -----------------------------------------------------------------
        #region SOURCE_METHODS

        /// <summary>
        /// Import Sudokus from the specified xml file
        /// </summary>
        /// <param name="path">file path</param>
        private void ImportSudokusFromXmlFile(string path = "")
        {
            // Use the default xml file if no file was specified
            if (path == "") { path = StandardXmlFilePath; }

            // Abort if file could not be found or is not .xml
            if (!File.Exists(path) || path.Substring(path.Length - 4) != ".xml")
            {
                Debug.WriteLine($"MainPage: ImportSudokusFromXmlFile() recieved invalid file path, {path}");
                return;
            }

            // Make sure SudokuStorage has been instantiated
            if (SudokuStorage == null) { InstatiateSudokuStorage(); }

            // Get the content from the xml file, and add it to SudokuStorage
            AddToSudokuStorage(SudokuSource.GetContentFromXmlFile(path));
        }

        /// <summary>
        /// Add a collection to the current SudokuStorage collection
        /// </summary>
        /// <param name="addition"></param>
        private void AddToSudokuStorage(IEnumerable<StorageGroup> addition)
        {
            SudokuStorage.AddRange(addition);
            ReloadStorageTreeView();
        }

        #endregion SOURCE_METHODS








        // -----------------------------------------------------------------
        // --------------------     Opening Sudoku      --------------------
        // -----------------------------------------------------------------
        #region OPEN_SUDOKU_METHODS

        /// <summary>
        /// Start a sudoku from SudokuStorage, with the specified storage, category, item index
        /// </summary>
        /// <param name="storageIndex"></param>
        /// <param name="categoryIndex"></param>
        /// <param name="itemIndex"></param>
        private void StartNewSudoku(int storageIndex, int categoryIndex, int itemIndex)
        {
            if (SudokuStorage == null || storageIndex < 0 || categoryIndex < 0 || itemIndex < 0 || SudokuStorage.Count == 0)
            {
                int[,] grid = new int[Sudoku.M, Sudoku.N];
                for (int i = 0; i < grid.Length; i++) { grid[i / Sudoku.M, i % Sudoku.N] = i; }
                LoadNewSudokuToGrid(grid);
            }
            else
            {
                LoadNewSudokuToGrid(SudokuStorage[storageIndex].Categories[categoryIndex].Items[itemIndex].Grid);
                SetCurrentSudokuDetails(storageIndex, categoryIndex, itemIndex);
            }
        }

        /// <summary>
        /// Start a random sudoku from SudokuStorage
        /// </summary>
        private void StartRandomSudoku()
        {
            if (SudokuStorage == null || SudokuStorage.Count == 0)
            {
                // Load the grid with cell-count values, signaling a sudoku could not be loaded
                int[,] grid = new int[Sudoku.M, Sudoku.N];
                for (int i = 0; i < grid.Length; i++) { grid[i / Sudoku.M, i % Sudoku.N] = i; }
                LoadNewSudokuToGrid(grid);
            }
            else
            {
                int i0 = rand.Next(SudokuStorage.Count);
                int i1 = rand.Next(SudokuStorage[i0].Categories.Count);
                int i2 = rand.Next(SudokuStorage[i0].Categories[i1].Items.Count);
                LoadNewSudokuToGrid(SudokuStorage[i0].Categories[i1].Items[i2].Grid);
                SetCurrentSudokuDetails(i0, i1, i2);
            }
        }

        /// <summary>
        /// Restart the current sudoku. Loads the starting grid to the play grid
        /// </summary>
        private void RestartSudoku()
        {
            Sudoku.RestartGrid();
            LoadNewSudokuToGrid(Sudoku.Grid);
        }

        #endregion OPEN_SUDOKU_METHODS








        // -----------------------------------------------------------------
        // ----------      Sudoku Grid (UI grid) manipulation     ----------
        // -----------------------------------------------------------------
        #region UI_PLAY_GRID_METHODS

        /// <summary>
        /// Load a new sudoku to play
        /// Use Start...Sudoku() methods instead
        /// </summary>
        /// <param name="gridValues">int[] (1x81) containing the values to load</param>
        private void LoadNewSudokuToGrid(int[] gridValues)
        {
            // Unlock any disabled number buttons


            // Make sure the provided array gridValues is of the correct length, if not then return
            if (gridValues.Length != SudokuAllowedLength)
            {
                Debug.WriteLine($"LoadSudokuToGrid: gridValues length must be equal 81. This value is required because of the UI. length = {gridValues.Length}");
                return;
            }

            // Load the gridValues as the current sudoku
            Sudoku.NewGrid(gridValues);

            // Load the sudoku to the grid on screen
            if (SudokuGrid.Children.Count != Sudoku.Length) { InstantiateSudokuGridCells(); }
            for (int m = 0; m < Sudoku.M; m++)
            {
                for (int n = 0; n < Sudoku.N; n++)
                {
                    LoadSingleValueToUIGridStart(m, n, Sudoku.Grid[m, n], true);
                }
            }

            // Set whether to allow only valid input for this game
            ValidInputOnly = ValidNumberEntryCheckBox.IsChecked ?? false;
            // Enable all numbers
            SetIsEnabledForInputNumber(-1, true);
            // Clear any message from the previous game
            ShowMessageBelowPlayGrid("");
        }

        /// <summary>
        /// Load a new sudoku play
        /// Use Start...Sudoku() methods instead
        /// </summary>
        /// <param name="gridValues">int[9, 9] containing the values to load</param>
        private void LoadNewSudokuToGrid(int[,] gridValues)
        {
            // Make sure the provided array gridValues is of the correct length, if not then return
            if (gridValues.Rank != 2 || gridValues.GetLength(0) != SudokuAllowedLength0 || gridValues.GetLength(1) != SudokuAllowedLength1)
            {
                Debug.WriteLine($"LoadSudokuToGrid: gridValues must be 9x9. This value is required because of the UI. length0 = {gridValues.GetLength(0)}, length1 = {gridValues.GetLength(1)}");
                return;
            }

            // Populated with clean elements/cells
            InstantiateSudokuGridCells();

            // Load the gridValues as the current sudoku
            Sudoku.NewGrid(gridValues);

            // Load the sudoku to the grid on screen
            for (int m = 0; m < Sudoku.M; m++)
            {
                for (int n = 0; n < Sudoku.N; n++)
                {
                    LoadSingleValueToUIGridStart(m, n, Sudoku.Grid[m, n], true);
                }
            }

            // Set whether to allow only valid input for this game
            ValidInputOnly = ValidNumberEntryCheckBox.IsChecked ?? false;
            // Enable all numbers
            SetIsEnabledForInputNumber(-1, true);
            // Clear any message from the previous game
            ShowMessageBelowPlayGrid("");
        }

        /// <summary>
        /// Load a value to the specified element on SudokuGrid, when starting a new Sudoku
        /// </summary>
        /// <param name="m">M-coordinate. must be (Sudoku.M > m >= 0)</param>
        /// <param name="n">N-coordinate. must be (Sudoku.N > n >= 0)</param>
        /// <param name="value">value to load to grid element/cell</param>
        /// <param name="isStart">Whether this element is a starting element of the sudoku, and should be noted in bold</param>
        private void LoadSingleValueToUIGridStart(int m, int n, int value, bool isStart = false)
        {
            // If any invalid value given, return
            if (m < 0 || n < 0 || m >= Sudoku.M || n >= Sudoku.N) { return; }
                        
            if (isStart) { SudokuGrid.Children.ElementAt(m * Sudoku.N + n).SetValue(TextBlock.FontWeightProperty, StartingValueFontWeight); }
            else { SudokuGrid.Children.ElementAt(m * Sudoku.N + n).SetValue(TextBlock.FontWeightProperty, DefaultFontWeight); }

            SudokuGrid.Children.ElementAt(m * Sudoku.N + n).SetValue(TextBlock.TextProperty, value == 0 ? string.Empty : value.ToString());
        }

        /// <summary>
        /// Load a value to the specified element on SudokuGrid, when starting a new Sudoku
        /// </summary>
        /// <param name="m">M-coordinate. must be (Sudoku.M > m >= 0)</param>
        /// <param name="n">N-coordinate. must be (Sudoku.N > n >= 0)</param>
        /// <param name="value">value to load to grid element/cell</param>
        /// <param name="isStart">Whether this element is a starting element of the sudoku, and should be noted in bold</param>
        private void LoadSingleValueToUIGridStart(int m, int n, string value, bool isStart = false)
        {
            // If any invalid value given, return
            if (m < 0 || n < 0 || m >= Sudoku.M || n >= Sudoku.N) { return; }

            if (isStart) { SudokuGrid.Children.ElementAt(m * Sudoku.N + n).SetValue(TextBlock.FontWeightProperty, StartingValueFontWeight); }
            else { SudokuGrid.Children.ElementAt(m * Sudoku.N + n).SetValue(TextBlock.FontWeightProperty, DefaultFontWeight); }

            SudokuGrid.Children.ElementAt(m * Sudoku.N + n).SetValue(TextBlock.TextProperty, value == "0" ? string.Empty : value.ToString());
        }

        /// <summary>
        /// Load a value to the specified element on SudokuGrid
        /// </summary>
        /// <param name="m">M-coordinate. must be (Sudoku.M > m >= 0)</param>
        /// <param name="n">N-coordinate. must be (Sudoku.N > n >= 0)</param>
        /// <param name="value">value to load to grid element/cell</param>
        private void LoadSingleValueToUIGrid(int m, int n, int value)
        {
            // If any invalid value given, return
            if (m < 0 || n < 0 || m >= Sudoku.M || n >= Sudoku.N || m * n >= SudokuGrid.Children.Count) { return; }

            SudokuGrid.Children.ElementAt(m * Sudoku.N + n).SetValue(TextBlock.TextProperty, value == 0 ? string.Empty : value.ToString());
        }

        /// <summary>
        /// Load a value to the specified element on SudokuGrid
        /// </summary>
        /// <param name="m">M-coordinate. must be (Sudoku.M > m >= 0)</param>
        /// <param name="n">N-coordinate. must be (Sudoku.N > n >= 0)</param>
        /// <param name="value">value to load to grid element/cell</param>
        private void LoadSingleValueToUIGrid(int m, int n, string value)
        {
            // If any invalid value given, return
            if (m < 0 || n < 0 || m >= Sudoku.M || n >= Sudoku.N || m * n >= SudokuGrid.Children.Count) { return; }

            SudokuGrid.Children.ElementAt(m * Sudoku.N + n).SetValue(TextBlock.TextProperty, value == "0" ? string.Empty : value.ToString());
        }

        #endregion UI_PLAY_GRID_METHODS








        // -----------------------------------------------------------------
        // --------     Sudoku Storage Grid, UI, manipulation      ---------
        // -----------------------------------------------------------------
        #region UI_STORAGE_GRID_METHODS

        /// <summary>
        /// Load a sudoku, from storage, to the StorageGrid
        /// </summary>
        /// <param name="storageIndex"></param>
        /// <param name="categoryIndex"></param>
        /// <param name="itemIndex"></param>
        private void LoadSudokuToStorageGrid(int storageIndex, int categoryIndex, int itemIndex)
        {
            // grid to load
            int[,] grid;

            // Ensure the given values are valid. Should be redundant since the values are to be gotten from StorageTreeView, which is based on SudokuStorage, but just in case
            if (SudokuStorage == null || storageIndex < 0 || categoryIndex < 0 || itemIndex < 0 ||
                storageIndex >= SudokuStorage.Count || categoryIndex >= SudokuStorage[storageIndex].Categories.Count || itemIndex >= SudokuStorage[storageIndex].Categories[categoryIndex].Items.Count)
            {
                // Load the grid with cell-count values, signaling a sudoku could not be loaded
                grid = new int[9, 9];
                for (int i = 0; i < grid.Length; i++) { grid[i / 9, i % 9] = i; }
            }
            else
            {
                // Else load the desired sudoku from SudokuStorage
                grid = SudokuStorage[storageIndex].Categories[categoryIndex].Items[itemIndex].Grid;
            }

            // Make sure the grid has been populated with the required elements/cells
            if (SudokuStorageGrid.Children.Count != Sudoku.Length)
            {
                InstantiateSudokuStorageGridCells();
            }

            for (int m = 0; m < Sudoku.M; m++)
            {
                for (int n = 0; n < Sudoku.N; n++)
                {
                    SudokuStorageGrid.Children.ElementAt(m * Sudoku.N + n).SetValue(TextBlock.TextProperty, grid[m, n] == 0 ? string.Empty : grid[m, n].ToString());
                }
            }
        }

        #endregion UI_STORAGE_GRID_METHODS








        // -----------------------------------------------------------------
        // --------------      UI elements manipulation      ---------------
        // -----------------------------------------------------------------
        #region UI_ELEMENT_METHODS

        /// <summary>
        /// Read entire SudokuStorage collection to StorageTreeView
        /// </summary>
        private void ReloadStorageTreeView()
        {
            // Make sure SudokuStorage has content, else return
            if (SudokuStorage == null || SudokuStorage.Count == 0) { return; }

            // Clear any old content from the TreeView
            StorageTreeView.RootNodes.Clear();

            // Object/Content to add to the TreeView
            TreeViewNode storageNode;
            TreeViewNode categoryNode;
            TreeViewNode itemNode;

            // For each storage in Storages
            for (int i0 = 0; i0 < SudokuStorage.Count; i0++)
            {
                // Create a node
                storageNode = new TreeViewNode() { Content = SudokuStorage[i0].Name };
                // Assign negative values to the tag. This prevents a "System.Exception: Catastrophic failure" from occuring at StorageTreeView_ItemInvoked(). The node tag of all nodes can/might be read, but only the itemNode contains needed values
                storageNode.SetValue(TagProperty, new int[3] { -2, -2, -2 });

                // For each Category in Storage
                for (int i1 = 0; i1 < SudokuStorage[i0].Categories.Count; i1++)
                {
                    // Create a node
                    categoryNode = new TreeViewNode() { Content = SudokuStorage[i0].Categories[i1].Name };
                    // Assign negative values to the tag. This prevents a "System.Exception: Catastrophic failure" from occuring at StorageTreeView_ItemInvoked(). The node tag of all nodes can/might be read, but only the itemNode contains needed values
                    categoryNode.SetValue(TagProperty, new int[3] { -2, -2, -2 });

                    // For each Item in Category
                    for (int i2 = 0; i2 < SudokuStorage[i0].Categories[i1].Items.Count; i2++)
                    {
                        // Create a node
                        itemNode = new TreeViewNode() { Content = string.IsNullOrWhiteSpace(SudokuStorage[i0].Categories[i1].Items[i2].Name) ? $"Sudoku {i2}" : SudokuStorage[i0].Categories[i1].Items[i2].Name };
                        // Add tag to node with index info, to retrieve item from SudokuStorage. Tag consists of int[3] { StorageGroup-Index, Category-Index, Item-Index}
                        itemNode.SetValue(TagProperty, new int[3] { i0, i1, i2 });
                        // Add the Item to the current Category node
                        categoryNode.Children.Add(itemNode);
                    }
                    // Add the Category to the current Storage node
                    storageNode.Children.Add(categoryNode);
                }
                // Add the storage node to the TreeView as a RootNode
                StorageTreeView.RootNodes.Add(storageNode);
            }
        }

        /// <summary>
        /// Show the details of a sudoku next to the SudokuGrid.
        /// </summary>
        /// <param name="storageIndex"></param>
        /// <param name="categoryIndex"></param>
        /// <param name="itemIndex"></param>
        private void SetCurrentSudokuDetails(int storageIndex, int categoryIndex, int itemIndex)
        {
            CurrentSudokuStorageNameTextBlock.Text = SudokuStorage[storageIndex].Name;
            CurrentSudokuCategoryNameTextBlock.Text = SudokuStorage[storageIndex].Categories[categoryIndex].Name;
            CurrentSudokuItemNameTextBlock.Text = string.IsNullOrWhiteSpace(SudokuStorage[storageIndex].Categories[categoryIndex].Items[itemIndex].Name) ? itemIndex.ToString() : SudokuStorage[storageIndex].Categories[categoryIndex].Items[itemIndex].Name;
        }

        /// <summary>
        /// Set IsEnabled for the specified NumSelButton
        /// </summary>
        /// <param name="number">Between 0 & 9, anything else will effect all buttons</param>
        /// <param name="isEnabled"></param>
        private void SetIsEnabledForInputNumber(int number, bool isEnabled)
        {
            switch (number)
            {
                case 1: { NumSelButton1.IsEnabled = isEnabled; break; }
                case 2: { NumSelButton2.IsEnabled = isEnabled; break; }
                case 3: { NumSelButton3.IsEnabled = isEnabled; break; }
                case 4: { NumSelButton4.IsEnabled = isEnabled; break; }
                case 5: { NumSelButton5.IsEnabled = isEnabled; break; }
                case 6: { NumSelButton6.IsEnabled = isEnabled; break; }
                case 7: { NumSelButton7.IsEnabled = isEnabled; break; }
                case 8: { NumSelButton8.IsEnabled = isEnabled; break; }
                case 9: { NumSelButton9.IsEnabled = isEnabled; break; }
                case 0: { NumSelButton0.IsEnabled = isEnabled; break; }
                default:
                    {
                        NumSelButton1.IsEnabled = isEnabled;
                        NumSelButton2.IsEnabled = isEnabled;
                        NumSelButton3.IsEnabled = isEnabled;
                        NumSelButton4.IsEnabled = isEnabled;
                        NumSelButton5.IsEnabled = isEnabled;
                        NumSelButton6.IsEnabled = isEnabled;
                        NumSelButton7.IsEnabled = isEnabled;
                        NumSelButton8.IsEnabled = isEnabled;
                        NumSelButton9.IsEnabled = isEnabled;
                        NumSelButton0.IsEnabled = isEnabled;
                        break;
                    }
            }
        }

        /// <summary>
        /// Display a message just below the Sudoku Grid
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="severity">Severity of message, determines the colour of the text. 0 = black, 1 = orange, 2 = red</param>
        /// <param name="time">Time after which the message will be deleted from screen</param>
        private void ShowMessageBelowPlayGrid(string message, int severity = 0)
        {
            // Change the colour of the text according to the severity
            switch (severity)
            {
                case 0: { MessageTextBlockBrush.Color = Windows.UI.Colors.Black; break; }
                case 1: { MessageTextBlockBrush.Color = Windows.UI.Colors.Orange; break; }
                case 2: { MessageTextBlockBrush.Color = Windows.UI.Colors.Red; break; }
            }
            MessageTextBlock.Text = message;
        }

        #endregion UI_ELEMENT_METHODS








        // -----------------------------------------------------------------
        // -------------------      Helper Methods      --------------------
        // -----------------------------------------------------------------
        #region HELPER_METHODS

        /// <summary>
        /// Get the current sudoku as a single string
        /// </summary>
        /// <returns></returns>
        private string SudokuAsString()
        {
            string s = "";
            foreach (int i in Sudoku.Grid) { s += i; }
            return s;
        }

        /// <summary>
        /// Get the starting state of the current sudoku as a single string
        /// </summary>
        /// <returns></returns>
        private string SudokuStartAsString()
        {
            string s = "";
            foreach (int i in Sudoku.StartGrid) { s += i; }
            return s;
        }

        /// <summary>
        /// Write all the content of Storages to the output, debug
        /// </summary>
        private void PrintSudokuStorageContentDebug()
        {
            for (int curStor = 0; curStor < SudokuStorage.Count; curStor++)
            {
                Debug.WriteLine($"SudokuSource: DebugPrintStoragesContentToOutput(), storage {curStor}, name = {SudokuStorage[curStor].Name}, source = {SudokuStorage[curStor].Source}, content# = {SudokuStorage[curStor].Categories.Count}");

                for (int curCat = 0; curCat < SudokuStorage[curStor].Categories.Count; curCat++)
                {
                    Debug.WriteLine($"SudokuSource: DebugPrintStoragesContentToOutput(), category {curCat}, name = {SudokuStorage[curStor].Categories[curCat].Name}, content# = {SudokuStorage[curStor].Categories[curCat].Items.Count}");

                    for (int curItem = 0; curItem < SudokuStorage[curStor].Categories[curCat].Items.Count; curItem++)
                    {
                        Debug.WriteLine($"SudokuSource: DebugPrintStoragesContentToOutput(), item {curItem}, name = {SudokuStorage[curStor].Categories[curCat].Items[curItem].Name}, content = {SudokuStorage[curStor].Categories[curCat].Items[curItem].GridAsString}");
                    }
                }
            }
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

        #endregion HELPER_METHODS











        // -----------------------------------------------------------------
        // ----------------------      Events      -------------------------
        // -----------------------------------------------------------------
        #region EVENT_METHODS

        // Load the specified sudoku to grid, for quick loading of the same sudoku when debugging/testin
        private void LoadSpecificSudokuButton_Click(object sender, RoutedEventArgs e)
        {
            if (SudokuStorage.Count > 0)
            {
                StartNewSudoku(0, 3, 2);
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            GetSolution(true);
        }

        private void Test2Button_Click(object sender, RoutedEventArgs e)
        {
            int[,] grid = new int[9, 9];
            int[,] grid2 = new int[9, 9];
            for (int m = 0; m < grid.GetLength(0); m++)
            {
                for (int n = 0; n < grid.GetLength(1); n++)
                {
                    grid[m, n] = (m + n) % 9;
                    grid2[m, n] = n;
                }
            }

            List<StorageGroup> storage = new List<StorageGroup>();
            storage.Add(new StorageGroup("JsonTestGroup", "JsonTestSource"));
            storage[0].AddCategory("JsonTestCategory");
            storage[0].Categories[0].AddItem(grid, "JsonTestItem");

            SudokuSource.AddContentToJsonFile(SudokuStorage);
        }

        private void SudokuCellButtonsGridView_Click(object sender, ItemClickEventArgs e)
        {
            // Click event is too quick for SelectedIndex to be updated, click happens before GridView.SelectedIndex has updated, use SudokuCellButtonsGridView_SelectionChanged() instead
            // Debug.WriteLine($"MainPage: SudokuCellButtonsGridView_Click() {SudokuCellButtonsGridView.SelectedIndex}");
        }

        private void SudokuCellButtonsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* Test
            Debug.WriteLine($"MainPage: SudokuCellButtonsGridView_SelectionChanged() {SudokuCellButtonsGridView.SelectedIndex}");
            bool[] validNums = Sudoku.GetValidNumbersForCell(SudokuCellButtonsGridView.SelectedIndex / 9, SudokuCellButtonsGridView.SelectedIndex % 9);
            for (int i = 0; i < validNums.Length; i++)
            {
                Debug.WriteLine($"MainPage: SudokuCellButtonsGridView_SelectionChanged() {i} = {validNums[i]}");
             */
            SetValidNumberButtonsForCell(SudokuCellButtonsGridView.SelectedIndex / SudokuAllowedLength0, SudokuCellButtonsGridView.SelectedIndex % SudokuAllowedLength1);
        }

        private void NumSel_Click(object sender, RoutedEventArgs e)
        {
            // Debug.WriteLine($"SudokuNumSel_Click() clicked for {((Button)sender).Tag}");

            int i = -1;
            try
            {
                int.TryParse(((Button)sender).Tag.ToString(), out i);
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"SudokuNumSel_Click() caught Exception {exception}");
            }
            ProcessSudokuInput(i, SudokuCellButtonsGridView.SelectedIndex);
        }

        private void GetRandomSudokuButton_Click(object sender, RoutedEventArgs e)
        {
            StartRandomSudoku();
        }

        private void StoragesDropDownButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SavedGridsTestButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadStorageTreeView();
        }

        private void StorageTreeView_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            int[] tag = new int[3] { -1, -1, -1 };
            try
            {
                tag = ((TreeViewNode)args.InvokedItem).GetValue(TagProperty) as int[];
                // Only the TreeViewNode of a sudoku Item contains positive values, Storage and Category contain negatives
                if (tag[0] >= 0)
                {
                    SelectedSudokuStorageIndex = tag;
                    LoadSudokuToStorageGrid(tag[0], tag[1], tag[2]);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"MainPage: StorageTreeView_ItemInvoked() TreeViewNode.GetValue(TagProperty) threw exception {e}");
            }            
        }

        private void LoadSudokuToPlayGridButton_Click(object sender, RoutedEventArgs e)
        {
            StartNewSudoku(SelectedSudokuStorageIndex[0], SelectedSudokuStorageIndex[1], SelectedSudokuStorageIndex[2]);
            MainPagePivot.SelectedIndex = 0;
        }

        private void MoveToAddSudokuButton_Click(object sender, RoutedEventArgs e)
        {
            MainPagePivot.SelectedIndex = 2;
        }

        private void AddSudokuToStorageButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            RestartSudoku();
        }

        private void CheckCurrentStateForErrorsButton_Click(object sender, RoutedEventArgs e)
        {
            ValidateSudoku();
        }

        private void SolveSudokuButton_Click(object sender, RoutedEventArgs e)
        {
            GetSolution();
        }

        private void SudokuPivotItem_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Number0:
                case Windows.System.VirtualKey.Number1:
                case Windows.System.VirtualKey.Number2:
                case Windows.System.VirtualKey.Number3:
                case Windows.System.VirtualKey.Number4:
                case Windows.System.VirtualKey.Number5:
                case Windows.System.VirtualKey.Number6:
                case Windows.System.VirtualKey.Number7:
                case Windows.System.VirtualKey.Number8:
                case Windows.System.VirtualKey.Number9:
                    {
                        ProcessSudokuInput((int)e.Key - (int)Windows.System.VirtualKey.Number0, SudokuCellButtonsGridView.SelectedIndex);
                        break;
                    }
                case Windows.System.VirtualKey.NumberPad0:
                case Windows.System.VirtualKey.NumberPad1:
                case Windows.System.VirtualKey.NumberPad2:
                case Windows.System.VirtualKey.NumberPad3:
                case Windows.System.VirtualKey.NumberPad4:
                case Windows.System.VirtualKey.NumberPad5:
                case Windows.System.VirtualKey.NumberPad6:
                case Windows.System.VirtualKey.NumberPad7:
                case Windows.System.VirtualKey.NumberPad8:
                case Windows.System.VirtualKey.NumberPad9:
                    {
                        ProcessSudokuInput((int)e.Key - (int)Windows.System.VirtualKey.NumberPad0, SudokuCellButtonsGridView.SelectedIndex);
                        break;
                    }

                default:
                    {
                        Debug.WriteLine($"MainPage: SudokuPivotItem() KeyUp {e.Key.ToString()}");
                        break;
                    }
            }
        }

        



        #endregion EVENT_METHODS


    }
}

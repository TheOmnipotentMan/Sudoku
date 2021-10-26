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
        SudokuSource SudokuSource;

        /// <summary>
        /// The starting grid of the current sudoku, starting state
        /// </summary>
        int[,] SudokuStart = new int[9, 9];

        /// <summary>
        /// The current state of the sudoku, cell values, must be 9x9 to ensure conformity with the UI
        /// </summary>
        int[,] Sudoku = new int[9, 9];


        private string standardXmlFilePath = "SudokuStorage.xml";





        public MainPage()
        {
            this.InitializeComponent();

            InstantiateSudokuSource();

        }


        /// <summary>
        /// Create a new SudokuSource
        /// </summary>
        private void InstantiateSudokuSource()
        {
            SudokuSource = new SudokuSource();
        }







        private void InstantiateSudokuGridCells()
        {
            for (int m = 0; m < Sudoku.GetLength(0); m++)
            {
                for (int n = 0; n < Sudoku.GetLength(1); n++)
                {
                    SudokuGrid.Children.Add(new TextBlock() { Text = string.Empty, FontSize = 24, TextAlignment = 0, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
                    SudokuGrid.Children.Last().SetValue(Grid.RowProperty, m);
                    SudokuGrid.Children.Last().SetValue(Grid.ColumnProperty, n);


                    // TEST
                    SudokuGrid.Children.Last().SetValue(TextBlock.TextProperty, (m * 9 + n).ToString());
                }
            }
        }


        /// <summary>
        /// Load a sudoku to the grid on screen
        /// </summary>
        /// <param name="gridValues">Array of </param>
        private void LoadNewSudokuToGrid(int[] gridValues)
        {
            // Make sure the provided array gridValues is of the correct length, if not then return
            if (gridValues.Length != Sudoku.Length)
            {
                Debug.WriteLine($"LoadSudokuToGrid: gridValues length must be equal 81. This value is required because of the UI. length = {gridValues.Length}");
                return;
            }

            // Make sure the grid has been populated with the required elements/cells
            if (SudokuGrid.Children.Count != Sudoku.Length)
            {
                InstantiateSudokuGridCells();
            }

            // Load the gridValues to the grid on screen
            int x;
            for (int m = 0; m < Sudoku.GetLength(0); m++)
            {
                for (int n = 0; n < Sudoku.GetLength(1); n++)
                {
                    x = m * Sudoku.GetLength(1) + n;
                    SudokuGrid.Children.ElementAt(x).SetValue(TextBlock.TextProperty, gridValues[x] == 0 ? string.Empty : gridValues[x].ToString());
                }
            }
        }

        private void LoadNewSudokuToGrid(int[,] gridValues)
        {
            // Make sure the provided array gridValues is of the correct length, if not then return
            if (gridValues.Length != Sudoku.Length)
            {
                Debug.WriteLine($"LoadSudokuToGrid: gridValues length must be equal 81. This value is required because of the UI. length = {gridValues.Length}");
                return;
            }

            // Make sure the grid has been populated with the required elements/cells
            if (SudokuGrid.Children.Count != Sudoku.Length)
            {
                InstantiateSudokuGridCells();
            }
        }




        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewSudokuToGrid(SudokuSource.GetSudokuRandom());
        }

        private void Test2Button_Click(object sender, RoutedEventArgs e)
        {
            SudokuSource.LoadXmlFileToStorage(standardXmlFilePath);
        }

        private void SudokuCellButtonsGridView_Click(object sender, ItemClickEventArgs e)
        {

        }

        private void SudokuCellButtonsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SudokuNumSel_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"SudokuNumSel_Click() clicked for {((Button)sender).Tag}");
        }
    }
}

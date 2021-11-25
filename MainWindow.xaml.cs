using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;


namespace Sudoku2._0
{
    public partial class MainWindow : Window
    {
        #region Variables
        private Button[,] buttonGrid;
        private Grid grid = new Grid();

        private DateTime startTime;
        private DateTime endTime;
        private string elapsedTime;

        private int value = 1;

        private int hintCount;
        private string difficulty = "Easy";

        private string saveDataPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\data.txt";
        private string saveDataStats = System.AppDomain.CurrentDomain.BaseDirectory + "\\stat.txt";
        #endregion

        #region Constructors
        public MainWindow()
        {

            InitializeComponent();
            CreateInsertGrid();
            CreateButtonGrid();

            mainGrid.IsEnabled = false;
            insertNumberGrid.IsEnabled = false;
            cleanButton.IsEnabled = false;
            checkButton.IsEnabled = false;
            hintButton.IsEnabled = false;
        }
        #endregion

        #region Inizialization buttons
        private void CreateButtonGrid()
        {
            buttonGrid = new Button[Grid.GridSize, Grid.GridSize];

            for (int i = 0; i < Grid.GridSize; i++)
            {
                for (int j = 0; j < Grid.GridSize; j++)
                {
                    buttonGrid[i, j] = new Button();

                    buttonGrid[i, j].SetValue(System.Windows.Controls.Grid.RowProperty, i);
                    buttonGrid[i, j].SetValue(System.Windows.Controls.Grid.ColumnProperty, j);
                    buttonGrid[i, j].Content = string.Empty;
                    buttonGrid[i, j].FontSize = 20;
                    buttonGrid[i, j].FontWeight = FontWeights.Bold;
                    buttonGrid[i, j].Click += InsertNumber;

                    if (j == 2 || j == 5)
                    {
                        var margin = buttonGrid[i, j].Margin;
                        margin.Right = 1;
                        buttonGrid[i, j].Margin = margin;
                    }
                    if (j == 3 || j == 6)
                    {
                        var margin = buttonGrid[i, j].Margin;
                        margin.Left = 1;
                        buttonGrid[i, j].Margin = margin;
                    }
                    if (i == 2 || i == 5)
                    {
                        var margin = buttonGrid[i, j].Margin;
                        margin.Bottom = 1;
                        buttonGrid[i, j].Margin = margin;
                    }
                    if (i == 3 || i == 6)
                    {
                        var margin = buttonGrid[i, j].Margin;
                        margin.Top = 1;
                        buttonGrid[i, j].Margin = margin;
                    }


                    mainGrid.Children.Add(buttonGrid[i, j]);

                }
            }

            
        }

        private void CreateInsertGrid()
        {
            for (int i = 0; i < Grid.GridSize; i++)
            {
                Button insertButtonBox = new Button();

                insertButtonBox.SetValue(System.Windows.Controls.Grid.ColumnProperty, i);
                insertButtonBox.Content = (i + 1).ToString();
                insertButtonBox.FontSize = 20;
                insertButtonBox.FontWeight = FontWeights.Bold;
                insertButtonBox.Width = 36;
                insertButtonBox.Height = 36;
                insertButtonBox.Background = Brushes.AliceBlue;
                insertButtonBox.Click += ChooseNumber;
                insertNumberGrid.Children.Add(insertButtonBox);

                
            }

            
        }

        private void RefreshButtonGrid()
        {
            ClearButtonGrid();

            for (int i = 0; i < Grid.GridSize; i++)
            {
                for (int j = 0; j < Grid.GridSize; j++)
                {
                    if (grid.GetCellValue(i, j) != 0)
                    {
                        if (grid.IsClueCell(i, j))
                        {
                            buttonGrid[i, j].IsEnabled = false;
                        }
                        else
                        {
                            buttonGrid[i, j].IsEnabled = true;
                        }
                        buttonGrid[i, j].Content = grid.GetCellValue(i, j).ToString();

                    }
                    else
                    {
                        buttonGrid[i, j].Content = string.Empty;
                    }
                }
            }
        }

        private void ClearButtonGrid()
        {
            for (int i = 0; i < Grid.GridSize; i++)
            {
                for (int j = 0; j < Grid.GridSize; j++)
                {
                    buttonGrid[i, j].Content = String.Empty;
                    buttonGrid[i, j].IsEnabled = true;
                }
            }
        }
        #endregion

        #region User button methods
        private void NewGame(object sender, RoutedEventArgs e)
        {
            mainGrid.IsEnabled = true;
            insertNumberGrid.IsEnabled = true;
            cleanButton.IsEnabled = true;
            checkButton.IsEnabled = true;
            hintButton.IsEnabled = true;

            grid.InizializeGrid();
            RefreshButtonGrid();

            hintCount = 0;
            startTime = DateTime.Now;
            elapsedTime = string.Empty;
        }
        

        private void QuitButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CheckButton(object sender, RoutedEventArgs e)
        {
            if (grid.CheckGrid())
            {
                endTime = DateTime.Now;
                
                MessageBox.Show("COMPLETE! \nTime to complete: " + (endTime - startTime).ToString()
                    + "\nGame Difficult: " +  difficulty + "\nHints used: " + hintCount.ToString(), "Victory", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                mainGrid.IsEnabled = false;
                insertNumberGrid.IsEnabled = false;
                cleanButton.IsEnabled = false;
                hintButton.IsEnabled = false;
            }
            else
                MessageBox.Show("SBAGLIATO!", "Riprova", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CleanButton(object sender, RoutedEventArgs e)
        {
            grid.ClearNotClueValue();
            RefreshButtonGrid();
            hintCount = 0;
        }


        private bool handle = true;
        private void ChangeGameDifficult(object sender, RoutedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            handle = !cmb.IsDropDownOpen;
            Handle();
        }

        private void ComboBoxDropDownClosed(object sender, EventArgs e)
        {
            if (handle) Handle();
            handle = true;
        }

        private void Handle()
        {
            difficulty = comboBoxDifficulty.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();

            switch (difficulty)
            {
                case "Easy":
                    grid.SetNumberOfClues(40);
                    break;
                case "Normal":
                    grid.SetNumberOfClues(32);
                    break;
                case "Hard":
                    grid.SetNumberOfClues(24);
                    break;
                case "Very Hard":
                    grid.SetNumberOfClues(17);
                    break;
            }

            
        }

        private void CancelButton(object sender, RoutedEventArgs e)
        {
            value = 0;
        }

        private void HintButton(object sender, RoutedEventArgs e)
        {
            if (grid.GetHint())
                hintCount++;

            RefreshButtonGrid();
        }

        private void LoadGame(object sender, RoutedEventArgs e)
        {
            grid.ClearGrid();

            if (grid.LoadGrid(saveDataPath))
            {
                MessageBox.Show("Game loaded successfully!", "Game Loaded", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                RefreshButtonGrid();
                LoadStats();
                mainGrid.IsEnabled = true;
                insertNumberGrid.IsEnabled = true;
                cleanButton.IsEnabled = true;
                checkButton.IsEnabled = true;
                hintButton.IsEnabled = true;
            }
            else
                MessageBox.Show("No Game Saved!", "Error.", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SaveGame(object sender, RoutedEventArgs e)
        {
            if (grid.SaveGrid(saveDataPath))
            {
                MessageBox.Show("Game saved!", "save", MessageBoxButton.OK, MessageBoxImage.None);
                SaveStats();
            }
            else
                MessageBox.Show("Unable to save game.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        

        private void ChooseNumber(object sender, RoutedEventArgs e)
        {
            value = int.Parse((sender as Button).Content.ToString());
        }

        private void InsertNumber(object sender, RoutedEventArgs e)
        {
            if (value != 0)
                (sender as Button).Content = value.ToString();
            else
                (sender as Button).Content = string.Empty;

            grid.SetCellValue((int)(sender as Button).GetValue(System.Windows.Controls.Grid.RowProperty),
                (int)(sender as Button).GetValue(System.Windows.Controls.Grid.ColumnProperty), value);
        }

        private void SaveStats()
        {
            endTime = DateTime.Now;
            elapsedTime = (endTime - startTime).ToString();

            try
            {
                using StreamWriter file = new(saveDataStats);

                file.WriteLine(hintCount.ToString());
                file.WriteLine(difficulty);
                file.WriteLine(elapsedTime);

            }
            catch (IOException)
            {
                MessageBox.Show("Error while trying to save statistics.", "error occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStats()
        {
            try
            {
                string[] lines = File.ReadAllLines(saveDataStats);

                hintCount = int.Parse(lines[0]);
                difficulty = lines[1];
                elapsedTime = lines[2];

            }
            catch (IOException)
            {
                MessageBox.Show("Error while trying to load statistics.", "error occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}

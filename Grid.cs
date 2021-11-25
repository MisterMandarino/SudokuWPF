using System;
using System.IO;

namespace Sudoku2._0
{
    class Grid
    {       
        public const int GridSize = 9;

        #region Private Variables
        private int NumberOfClues = 40;

        private Cell[,] grid;
        private Cell[,] completeGrid;

        private Random rand = new Random();
        #endregion

        #region Constructor
        public Grid()
        {
            grid = new Cell[GridSize, GridSize];

            for(int i = 0; i < GridSize; i++)
            {
                for(int j = 0; j < GridSize; j++)
                {
                    grid[i, j] = new Cell();
                }
            }
        }
        #endregion

        #region Inizialization and validation Grid with backtracking search

        public void InizializeGrid()
        {
            
            int rowRand, colRand, valueRand;

            do
            {
                ClearGrid();

                for (int i = 0; i < NumberOfClues; i++)
                {
                    do
                    {
                        rowRand = rand.Next(GridSize);
                        colRand = rand.Next(GridSize);
                        valueRand = rand.Next(1, 10);
                    } while (!IsValidCell(rowRand, colRand, valueRand));

                    grid[rowRand, colRand].SetIsClue(true);
                    grid[rowRand, colRand].SetValue(valueRand);
                }

            } while (!IsValidGrid(grid));

            SaveCompleteGrid();
            ClearNotClueValue();
        }

        private void SaveCompleteGrid()
        {
            completeGrid = new Cell[GridSize, GridSize];
            for(int i = 0; i < GridSize; i++)
            {
                for(int j = 0; j < GridSize; j++)
                {
                    completeGrid[i, j] = new Cell(grid[i, j].GetValue());
                }
            }
        }
        private bool IsValidCell(int row, int col, int value)
        {
            if (grid[row, col].GetValue() != 0)
                return false;

            for (int i = 0; i < GridSize; i++)
            {

                if (grid[i, col].GetValue() == value)
                    return false;


                if (grid[row, i].GetValue() == value)
                    return false;

            }


            for (int i = 0; i < (GridSize / 3); i++)
            {
                int rowIndex = i + (row / 3) * 3;
                for (int j = 0; j < (GridSize / 3); j++)
                {
                    int colIndex = j + (col / 3) * 3;

                    if (grid[rowIndex, colIndex].GetValue() == value)
                        return false;

                }
            }

            return true;
        }

        private bool IsValidGrid(Cell[,] g)
        {
            for (int i = 0; i < g.GetLength(0); i++)
            {
                for (int j = 0; j < g.GetLength(1); j++)
                {
                    if (g[i, j].GetValue() == 0)
                    {
                        for (int c = 1; c < 10; c++)
                        {
                            if (IsValidCell(i, j, c))
                            {
                                g[i, j].SetValue(c);

                                if (IsValidGrid(g))
                                    return true;
                                else
                                    g[i, j].SetValue(0);
                            }
                        }

                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region Check valid grid methods
        public bool CheckGrid()
        {
            int rowSum;
            int colSum;

            for (int i = 0; i < GridSize; i++)
            {
                colSum = 0;
                rowSum = 0;
                for (int j = 0; j < GridSize; j++)
                {
                    if ( (grid[i, j].GetValue() == 0) || (grid[j, i].GetValue() == 0) )
                        return false;
                    else
                    {
                        colSum += grid[i, j].GetValue();
                        rowSum += grid[j, i].GetValue();
                    }
                }

                if ((colSum != 45) || (rowSum != 45))
                    return false;
            }

            if (CheckSubGrid())
                return false;
            return true;
        }

        private bool CheckSubGrid()
        {
            int sum;
            for (int z = 0; z < GridSize; z++)
            {
                sum = 0;
                for (int i = 0; i < (GridSize / 3); i++)
                {
                    for (int j = 0; j < (GridSize / 3); j++)
                    {
                        if (z < 3)
                            sum += grid[i + (z * 3), j].GetValue();
                        else if (z >= 3 && z < 6)
                            sum += grid[i + ((z - 3) * 3), j + 3].GetValue();
                        else
                            sum += grid[i + ((z - 6) * 3), j + 6].GetValue();
                    }
                }

                if (sum != 45)
                    return true;
            }
            return false;
        }

        private bool IsFullGrid()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (grid[i, j].GetValue() == 0)
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region Public methods
        public void SetNumberOfClues(int number)
        {
            this.NumberOfClues = number;
        }

        public int GetCellValue(int row, int col)
        {
            return grid[row, col].GetValue();
        }

        public void SetCellValue(int row, int col, int value)
        {
            grid[row, col].SetValue(value);
        }
        public bool IsClueCell(int row, int col)
        {
            return grid[row, col].IsClue();
        }
        
        public void ClearGrid()
        {
            for(int i = 0; i < GridSize; i++)
            {
                for(int j = 0; j < GridSize; j++)
                {
                    grid[i, j].SetValue(0);
                    grid[i, j].SetIsClue(false);
                }
            }
        }

        public void ClearNotClueValue()
        {
            for(int i = 0; i < GridSize; i++)
            {
                for(int j = 0; j < GridSize; j++)
                {
                    if(!grid[i,j].IsClue())
                    {
                        grid[i, j].SetValue(0);
                    }
                }
            }
        }

        public bool GetHint()
        {
            if (!IsFullGrid())
            {
                int row, col;
                do
                {
                    row = rand.Next(GridSize);
                    col = rand.Next(GridSize);
                } while (grid[row, col].GetValue() != 0);

                grid[row, col].SetValue(completeGrid[row, col].GetValue());
                return true;
            }
            else
                return false;

        }
        #endregion

        #region Saving data methods
        public bool LoadGrid(string path)
        {
            try
            {
                SaveCompleteGrid();
                string[] lines = File.ReadAllLines(path);
                int cont1 = 0, cont2 = 1;
                for(int i = 0; i < GridSize; i++)
                {
                    for(int j = 0; j < GridSize; j++)
                    {
                        grid[i, j].SetValue(int.Parse(lines[cont1]));
                        completeGrid[i, j].SetValue(int.Parse(lines[cont2]));

                        cont1 += 2;
                        cont2 += 2;
                    }                  
                }
                return true;
            }
            catch(IOException)
            {
                return false;
            }
        }

        public bool SaveGrid(string path)
        {
            try
            {
                using StreamWriter file = new(path);
                
                for(int i = 0; i < GridSize; i++)
                {
                    for(int j = 0; j < GridSize; j++)
                    {
                        file.WriteLine(grid[i,j].GetValue().ToString());
                        file.WriteLine(completeGrid[i, j].GetValue().ToString());
                    }
                }
            }
            catch(IOException)
            {
                return false;
            }

            return true;
            
        }
        #endregion
    }
}

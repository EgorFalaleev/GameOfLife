using System;
using System.Threading;
using System.Text;

namespace GameOfLife
{
    class GameController
    {
        private bool[,] currentGeneration;
        private int numberOfRows;
        private int numberOfColumns;
        private int generationNumber;
        private Random random;

        public GameController()
        {
            numberOfRows = 15;
            numberOfColumns = 15;
            currentGeneration = new bool[numberOfRows, numberOfColumns];
            random = new Random();
        }

        public void StartGame()
        {
            int cellsAlive = RequestNumberOfAliveCells();

            GenerateStartingState(currentGeneration, cellsAlive);

            Console.Write("Для запуска нажмите Enter.");
            Console.ReadLine();

            while (true)
            {
                PopulateNextGeneration(currentGeneration);
                Thread.Sleep(100);
            }
        }

        private int RequestNumberOfAliveCells()
        {
            Console.Write($"Введите количество живых клеток в начале игры (от 0 до {currentGeneration.Length}): ");
            int startingNumberOfCellsAlive = 0;

            // проверка введённого значения на правильность
            try
            {
                startingNumberOfCellsAlive = Convert.ToInt32(Console.ReadLine());
                CheckNumberOfCellsCorrectness(startingNumberOfCellsAlive);
            }
            catch (FormatException)
            {
                Console.WriteLine("Введён неверный формат.");
                startingNumberOfCellsAlive = RequestNumberOfAliveCells();
            }
            catch (NumberOutOfRangeException)
            {
                Console.WriteLine($"Введено недопустимое число. Введённое число должно быть в диапазоне от 0 до {currentGeneration.Length}");
                startingNumberOfCellsAlive = RequestNumberOfAliveCells();
            }
            catch(Exception)
            {
                Console.WriteLine("Возникла ошибка. Пожалуйста, попробуйте ещё раз.");
                startingNumberOfCellsAlive = RequestNumberOfAliveCells();
            }

            return startingNumberOfCellsAlive;
        }

        private void CheckNumberOfCellsCorrectness(int numberToCheck)
        {
            if (numberToCheck < 0 || numberToCheck > numberOfColumns * numberOfRows) throw new NumberOutOfRangeException("Введено недопустимое число.");
        }

        private void DisplayGeneration(bool[,] generationToDisplay)
        {
            Console.Clear();
            StringBuilder gameField = new StringBuilder(generationToDisplay.Length * 2);
            Console.WriteLine($"Поколение {generationNumber}\n" +
                $"\"O\" - живая клетка, \".\" - мёртвая клетка.\n");

            // заполнение строки, представляющей игровое поле
            for (int i = 0; i < numberOfRows; i++)
            {
                for (int j = 0; j < numberOfColumns; j++)
                {
                    if (generationToDisplay[i, j]) gameField.Append("O ");
                    else gameField.Append(". ");
                }
                gameField.Append('\n');
            }
         
            Console.WriteLine(gameField);
        }

        private void GenerateStartingState(bool[,] generation, int cellsAlive)
        {
            int randomRow;
            int randomColumn;

            // сделать случайные клетки сетки "живыми"
            for (int k = cellsAlive; k > 0; k--)
            {
                randomRow = random.Next(numberOfRows);
                randomColumn = random.Next(numberOfColumns);

                while (generation[randomRow, randomColumn])
                {
                    randomRow = random.Next(numberOfRows);
                    randomColumn = random.Next(numberOfColumns);
                }

                generation[randomRow, randomColumn] = true;
            }

            generationNumber = 1;
            DisplayGeneration(generation);
        }

        private void PopulateNextGeneration(bool[,] generation)
        {
            bool[,] nextGeneration = new bool[numberOfRows, numberOfColumns];

            for (int i = 0; i < numberOfRows; i++)
            {
                for (int j = 0; j < numberOfColumns; j++)
                {
                    nextGeneration[i, j] = CalculateCellState(i, j, currentGeneration[i, j]);
                }
            }

            generationNumber++;
            DisplayGeneration(nextGeneration);
            currentGeneration = nextGeneration;
        }

        private int CountNeighboursAlive(int row, int column)
        {
            int neighboursAlive = 0;

            if (row - 1 >= 0 && column - 1 >= 0)
            if (currentGeneration[row - 1, column - 1]) neighboursAlive++; // сверху слева

            if (row - 1 >= 0 )
            if (currentGeneration[row - 1, column]) neighboursAlive++;  // сверху

            if (row - 1 >= 0 && column + 1 < numberOfColumns)
            if (currentGeneration[row - 1, column + 1]) neighboursAlive++; // сверху справа

            if (column - 1 >= 0 )
            if (currentGeneration[row, column - 1]) neighboursAlive++; // слева

            if (column + 1 < numberOfColumns)
            if (currentGeneration[row, column + 1]) neighboursAlive++; // справа

            if (row + 1 < numberOfRows && column - 1 >= 0)
            if (currentGeneration[row + 1, column - 1]) neighboursAlive++; // снизу слева

            if (row + 1 < numberOfRows)
            if (currentGeneration[row + 1, column]) neighboursAlive++; // снизу

            if (row + 1 < numberOfRows && column + 1 < numberOfColumns)
            if (currentGeneration[row + 1, column + 1]) neighboursAlive++; // снизу справа

            return neighboursAlive;
        }

        private bool CalculateCellState(int row, int column, bool isCurrentCellAlive)
        {
            int neighboursAlive = CountNeighboursAlive(row, column);

            if (!isCurrentCellAlive)
            {
                if (neighboursAlive == 3) return true;
                else return false;
            }
            else
            {
                if (neighboursAlive == 2 || neighboursAlive == 3) return true;
                else return false;
            }
        }
    }
}

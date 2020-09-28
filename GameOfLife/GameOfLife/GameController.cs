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

        /// <summary>
        /// Определяет начальное состояние и запускает моделирование поколений
        /// </summary>
        public void StartGame()
        {
            if (AskForTestSimulation() == 1)
            {
                currentGeneration[5, 7] = true;
                currentGeneration[6, 6] = true;
                currentGeneration[6, 8] = true;
                currentGeneration[7, 6] = true;
                currentGeneration[7, 8] = true;
                currentGeneration[8, 6] = true;
                currentGeneration[8, 8] = true;
                currentGeneration[7, 5] = true;
                currentGeneration[7, 9] = true;
                currentGeneration[9, 7] = true;

                DisplayGeneration(currentGeneration);
            }
            else
            {
                int cellsAlive = RequestNumberOfAliveCells();

                GenerateStartingState(currentGeneration, cellsAlive);
            }

            Console.Write("Для запуска нажмите Enter.");
            Console.ReadLine();
            
            
            while (generationNumber < int.MaxValue)
            {
                PopulateNextGeneration();
                Thread.Sleep(100);
            }
            
        }

        /// <summary>
        /// Решает, использовать ли в качестве начального состояния конфигурацию из примера или сгенерировать случайную конфигурацию
        /// </summary>
        /// <returns>Число, на основе которого делается вывод о начальном состоянии</returns>
        private int AskForTestSimulation()
        {
            Console.Write("Если хотите сгенерировать случайное поколение, введите 0. Если хотите запустить игру с тестовым поколением, введите 1.");

            int decisionNumber = 0;

            try
            {
                decisionNumber = Convert.ToInt32(Console.ReadLine());
                CheckNumberCorrectness(decisionNumber, 0, 1);
            }
            catch(NumberOutOfRangeException)
            {
                Console.Clear();
                Console.WriteLine("Введено недопустимое число. Пожалуйста, попробуйте ещё раз.");
                decisionNumber = AskForTestSimulation();
            }
            catch(Exception)
            {
                Console.Clear();
                Console.WriteLine("Произошла ошибка. Пожалуйста, попробуйте ещё раз.");
                decisionNumber = AskForTestSimulation();
            }

            return decisionNumber;
        }

        /// <summary>
        /// Считывает с клавиатуры число живых клеток для генерации начального состояния
        /// </summary>
        /// <returns>Возвращает количество живых клеток в начале игры</returns>
        private int RequestNumberOfAliveCells()
        {
            Console.Write($"Введите количество живых клеток в начале игры (от 0 до {currentGeneration.Length}): ");
            int startingNumberOfCellsAlive = 0;

            // проверка введённого значения на правильность
            try
            {
                startingNumberOfCellsAlive = Convert.ToInt32(Console.ReadLine());
                CheckNumberCorrectness(startingNumberOfCellsAlive, 0, numberOfColumns * numberOfRows);
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

        /// <summary>
        /// Проверяет введённое число на принадлежность диапазону допустимых чисел
        /// </summary>
        /// <param name="numberToCheck">Число, которое требуется проверить</param>
        /// <param name="min">Нижняя граница</param>
        /// <param name="max">Верхняя граница</param>
        private void CheckNumberCorrectness(int numberToCheck, int min, int max)
        {
            if (numberToCheck < min || numberToCheck > max) throw new NumberOutOfRangeException("Введено недопустимое число.");
        }

        /// <summary>
        /// Выводит поколение в консоль
        /// </summary>
        /// <param name="generationToDisplay">Поколение, которое нужно вывести</param>
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

        /// <summary>
        /// Генерирует стартовое состояние
        /// </summary>
        /// <param name="generation">Начальное поколение</param>
        /// <param name="cellsAlive">Количество живых клеток в начале игры</param>
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

            generationNumber = 0;
            DisplayGeneration(generation);
        }

        /// <summary>
        /// Заполняет следующее поколение согласно правилам игры
        /// </summary>
        private void PopulateNextGeneration()
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

        /// <summary>
        /// Рассчитывает количество живых соседей для клетки
        /// </summary>
        /// <param name="row">Индекс ряда, в котором находится клетка</param>
        /// <param name="column">Индекс колонки, в которой находится клетка</param>
        /// <returns>Возвращает количество живых соседей у клетки</returns>
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

        /// <summary>
        /// Применяет правила игры для определения состояния клетки
        /// </summary>
        /// <param name="row">Индекс ряда, в котором находится клетка</param>
        /// <param name="column">Индекс колонки, в которой находится клетка</param>
        /// <param name="isCurrentCellAlive">Текущее состояние клетки</param>
        /// <returns>Возвращает состояние клетки в следующем поколении</returns>
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

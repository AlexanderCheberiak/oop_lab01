namespace Lab01
{
    public partial class MainPage : ContentPage
    {
        int CountColumn = 20; // кількість стовпчиків (A to Z)
        int CountRow = 21; // кількість рядків
        private Dictionary<string, string> cellFormulas = new Dictionary<string, string>(); // Словник для зберігання виразів
        public MainPage()
        {
            InitializeComponent();
            CreateGrid();
        }
        //створення таблиці
        private void CreateGrid()
        {
            AddColumnsAndColumnLabels();
            AddRowsAndCellEntries();
        }
        private void AddColumnsAndColumnLabels()
        {
            // Додати стовпці та підписи для стовпців
            for (int col = 0; col < CountColumn + 1; col++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                if (col > 0)
                {
                    var label = new Label
                    {
                        Text = GetColumnName(col),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    Grid.SetRow(label, 0);
                    Grid.SetColumn(label, col);
                    grid.Children.Add(label);
                }
            }
        }
        private void AddRowsAndCellEntries()
        {
            // Додати рядки, підписи для рядків та комірки
            for (int row = 0; row < CountRow; row++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                // Додати підпис для номера рядка
                var label = new Label
                {
                    Text = (row + 1).ToString(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                Grid.SetRow(label, row + 1);
                Grid.SetColumn(label, 0);
                grid.Children.Add(label);

                // Додати комірки (Entry) для вмісту
                for (int col = 0; col < CountColumn; col++)
                {
                    var entry = new Entry
                    {
                        Text = "",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    entry.Unfocused += Entry_Unfocused; // обробник події Unfocused
                    Grid.SetRow(entry, row + 1);
                    Grid.SetColumn(entry, col + 1);
                    grid.Children.Add(entry);
                }
            }
        }


        private string GetColumnName(int colIndex)
        {
            int dividend = colIndex;
            string columnName = string.Empty;
            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }
            return columnName;
        }

        // викликається, коли користувач вийде зі зміненої клітинки(втратить фокус)
        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            var row = Grid.GetRow(entry) - 1;
            var col = Grid.GetColumn(entry) - 1;
            var cellAddress = $"{GetColumnName(col + 1)}{row + 1}";
            var content = entry.Text;

            // Зберігаємо початковий вираз при втраті фокусу
            if (!string.IsNullOrEmpty(content))
            {
                cellFormulas[cellAddress] = content;
            }
        }
        private void CalculateSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                // Перемикач увімкнено: обчислюємо значення комірок
                CalculateAllCells();
            }
            else
            {
                // Перемикач вимкнений: відновлюємо формули
                RestoreFormulas();
            }
        }

        // Метод для обчислення значень усіх комірок
        private void CalculateAllCells()
        {
            var cellValues = GetAllCellValues();

            foreach (var entry in grid.Children.OfType<Entry>())
            {
                var cellAddress = $"{GetColumnName(Grid.GetColumn(entry))}{Grid.GetRow(entry)}";

                if (cellFormulas.TryGetValue(cellAddress, out var cellContent) && !string.IsNullOrEmpty(cellContent))
                {
                    try
                    {
                        // Обчислюємо формулу і показуємо результат
                        var result = Calculator.Evaluate(cellContent, cellValues);
                        entry.Text = result.ToString();
                    }
                    catch
                    {
                        entry.Text = "Помилка обчислення."; 
                    }
                }
            }
        }

        private void RestoreFormulas()
        {
            // Відновлюємо вихідні формули в кожній комірці
            foreach (var entry in grid.Children.OfType<Entry>())
            {
                var cellAddress = $"{GetColumnName(Grid.GetColumn(entry))}{Grid.GetRow(entry)}";

                if (cellFormulas.TryGetValue(cellAddress, out var formula))
                {
                    entry.Text = formula;
                }
            }
        }

        // Метод для вилучення всіх значень комірок і формування словника
        private Dictionary<string, double> GetAllCellValues()
        {
            var cellValues = new Dictionary<string, double>();

            for (int row = 1; row <= CountRow; row++)
            {
                for (int col = 1; col <= CountColumn; col++)
                {
                    var cellAddress = $"{GetColumnName(col)}{row}";
                    var entry = GetCellEntry(row, col);

                    if (entry != null && double.TryParse(entry.Text, out double cellValue))
                    {
                        cellValues[cellAddress] = cellValue;
                    }
                }
            }

            return cellValues;
        }

        // Метод для отримання комірки Entry по рядку та стовпчику
        private Entry GetCellEntry(int row, int col)
        {
            return grid.Children
                .OfType<Entry>()
                .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);
        }

        // Метод для обчислення значення однієї комірки з використанням класу Calculator
        private void CalculateCell(Entry entry, Dictionary<string, double> cellValues)
        {
            var cellContent = entry.Text;

            if (!string.IsNullOrEmpty(cellContent))
            {
                try
                {
                    var result = Calculator.Evaluate(cellContent, cellValues);
                    entry.Text = result.ToString();
                }
                catch
                {
                    entry.Text = "Синтаксична помилка.";
                }
            }
        }

        private async void ExitButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Підтвердження", "Ви дійсно хочете вийти ? ", "Так", "Ні");
            if (answer)
            {
                System.Environment.Exit(0);
            }
        }
        private async void HelpButton_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Довідка", "Лабораторна робота 1(Варіант 23)\nСтудента Чеберяка Олександра\nДоступні операції:\nАрифметичні: +, -, *, /, ^, mod, dіv\nЛогічні: =, <, >, not", "OK");
        }
        private void DeleteRowButton_Clicked(object sender, EventArgs e)
        {
            if (grid.RowDefinitions.Count > 1)
            {
                int lastRowIndex = grid.RowDefinitions.Count - 1;
                grid.RowDefinitions.RemoveAt(lastRowIndex);
                grid.Children.RemoveAt(lastRowIndex * (CountColumn + 1)); // Прибираємо Label
                for (int col = 0; col < CountColumn; col++)
                {
                    grid.Children.RemoveAt((lastRowIndex * CountColumn) + col + 1); // Прибираємо Entry
                }
            }
            CountRow--;
        }



        private void AddRowButton_Clicked(object sender, EventArgs e)
        {
            int newRow = grid.RowDefinitions.Count;

            // Додаємо нове визначення рядка
            grid.RowDefinitions.Add(new RowDefinition());

            // Додаємо номер рядка
            var label = new Label
            {
                Text = newRow.ToString(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(label, newRow);
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);

            // Створюємо комірки для нового рядка
            for (int col = 0; col < CountColumn; col++)
            {
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };

                entry.Unfocused += Entry_Unfocused;

                Grid.SetRow(entry, newRow);
                Grid.SetColumn(entry, col + 1);
                grid.Children.Add(entry);
            }
            CountRow++;
        }


        private void AddColumnButton_Clicked(object sender, EventArgs e)
        {
            // Додати новий стовпець у визначення стовпців
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            // Визначити номер нового стовпця (CountColumn + 1, оскільки починаємо з 0)
            int newColIndex = CountColumn + 1;

            // Додати заголовок для нового стовпця
            var headerLabel = new Label
            {
                Text = GetColumnName(newColIndex),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(headerLabel, 0);
            Grid.SetColumn(headerLabel, newColIndex);
            grid.Children.Add(headerLabel);

            // Пройтися всіма наявними рядками і додати нову комірку (Entry) у новий стовпець
            for (int row = 0; row < CountRow; row++)
            {
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                entry.Unfocused += Entry_Unfocused;
                Grid.SetRow(entry, row + 1);
                Grid.SetColumn(entry, newColIndex);
                grid.Children.Add(entry);
            }

            CountColumn++;
        }

        private void DeleteColumnButton_Clicked(object sender, EventArgs e)
        {
            if (CountColumn > 1)
            {
                // Видаляємо визначення останнього стовпчика
                int lastColIndex = CountColumn;
                grid.ColumnDefinitions.RemoveAt(lastColIndex);

                // Видаляємо заголовок останнього стовпчика
                var headerLabel = grid.Children
                    .OfType<Label>()
                    .FirstOrDefault(l => Grid.GetRow(l) == 0 && Grid.GetColumn(l) == lastColIndex);
                if (headerLabel != null)
                {
                    grid.Children.Remove(headerLabel);
                }

                // Видаляємо комірки (Entry) з останнього стовпця для кожного рядка
                for (int row = 1; row <= CountRow; row++)
                {
                    var cell = GetCellEntry(row, lastColIndex);
                    if (cell != null)
                    {
                        grid.Children.Remove(cell);
                    }
                }

                CountColumn--;
            }
        }

    }
}


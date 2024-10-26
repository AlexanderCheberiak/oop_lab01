namespace Lab01
{
    public partial class MainPage : ContentPage
    {
        const int CountColumn = 20; // кількість стовпчиків (A to Z)
        const int CountRow = 20; // кількість рядків
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
            var content = entry.Text;
            // Додайте додаткову логіку, яка виконується при виході зі зміненої клітинки
            //try
            //{
            //    var res = Calculator.Evaluate(content);
            //    entry.Text = res.ToString();
            //}
            //catch(Exception ex) {
            //    entry.Text = content;
            //}
        }
        private void CalculateButton_Clicked(object sender, EventArgs e)
        {
            CalculateAllCells();
        }

        // Метод для вычисления значений всех ячеек
        private void CalculateAllCells()
        {
            var cellValues = GetAllCellValues();

            foreach (var entry in grid.Children.OfType<Entry>())
            {
                var cellContent = entry.Text;

                // Пропускаем пустые ячейки
                if (!string.IsNullOrEmpty(cellContent))
                {
                    CalculateCell(entry, cellValues);
                }
            }
        }

        // Метод для извлечения всех значений ячеек и формирования словаря
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

        // Метод для получения Entry ячейки по строке и столбцу
        private Entry GetCellEntry(int row, int col)
        {
            return grid.Children
                .OfType<Entry>()
                .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);
        }

        // Метод для вычисления значения одной ячейки с использованием Calculator
        private void CalculateCell(Entry entry, Dictionary<string, double> cellValues)
        {
            var cellContent = entry.Text;

            if (!string.IsNullOrEmpty(cellContent))
            {
                try
                {
                    // Вызываем Evaluate с выражением и значениями ячеек
                    var result = Calculator.Evaluate(cellContent, cellValues);
                    entry.Text = result.ToString();
                }
                catch
                {
                    // Оставляем оригинальный текст, если ошибка вычисления
                    entry.Text = cellContent;
                }
            }
        }



        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            // Обробка кнопки "Зберегти"
        }
        private void ReadButton_Clicked(object sender, EventArgs e)
        {
            // Обробка кнопки "Прочитати"
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
            await DisplayAlert("Довідка", "Лабораторна робота 1. Студента Чеберяка Олександра", "OK");
        }
        private void DeleteRowButton_Clicked(object sender, EventArgs e)
        {
            if (grid.RowDefinitions.Count > 1)
            {
                int lastRowIndex = grid.RowDefinitions.Count - 1;
                grid.RowDefinitions.RemoveAt(lastRowIndex);
                grid.Children.RemoveAt(lastRowIndex * (CountColumn + 1)); // Remove label
                for (int col = 0; col < CountColumn; col++)
                {
                    grid.Children.RemoveAt((lastRowIndex * CountColumn) + col + 1); // Remove entry
                }
            }
        }

        private void AddRowButton_Clicked(object sender, EventArgs e)
        {
            int newRow = grid.RowDefinitions.Count;

            // Add a new row definition
            grid.RowDefinitions.Add(new RowDefinition());

            // Add label for the row number
            var label = new Label
            {
                Text = newRow.ToString(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(label, newRow);
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);

            // Add entry cells for the new row
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
        }
    }
}
using System.Windows;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using OfficeOpenXml;
using static AdminPanel.MainWindow;
using System.Data;


namespace AdminPanel
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Server=TEMHANLAPTOP\\TDG2022;Database=restaurant;Integrated Security=True;TrustServerCertificate=true;MultipleActiveResultSets=true;";

        public MainWindow()
        {
            InitializeComponent();
            LoadAllData();
            LoadAuthorIds();
            LoadCategoryDishAndMenuIds();
            LoadCategoryDataForUpdate();
            LoadMenuIds();
            LoadOrderIds();
            LoadDishIds();
            LoadCategoryData();
            LoadCategoryIds();
        }

        private void LoadAllData()
        {
            LoadData("SELECT * FROM authors", AuthorsDataGrid);
            LoadData("SELECT * FROM categories", CategoriesDataGrid);
            LoadData("SELECT * FROM dishes", DishesDataGrid);
            LoadData("SELECT * FROM menu", MenuDataGrid);
            LoadData("SELECT * FROM orders", OrdersDataGrid);
        }

        private void LoadData(string query, DataGrid dataGrid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                System.Data.DataTable table = new System.Data.DataTable();
                adapter.Fill(table);
                dataGrid.ItemsSource = table.DefaultView;
            }
        }

        private void LoadAuthorsIds()
        {
            AuthorIdToUpdateComboBox.ItemsSource = null;
            AuthorIdToDeleteComboBox.ItemsSource = null;


            List<Author> authors = new List<Author>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id, ФИО FROM authors";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            authors.Add(new Author
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            AuthorIdToUpdateComboBox.ItemsSource = authors;
            AuthorIdToDeleteComboBox.ItemsSource = authors;
        }

        private void LoadAuthorIds()
        {
            AuthorIdToUpdateComboBox.ItemsSource = null;
            AuthorIdToDeleteComboBox.ItemsSource = null;
            UpdatedDishAuthorIdInput.ItemsSource = null;
            DishAuthorIdInput.ItemsSource = null;

            List<Author> authors = new List<Author>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id, ФИО FROM authors";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Author author = new Author
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            };
                            authors.Add(author);
                        }
                    }
                }
            }

            AuthorIdToUpdateComboBox.ItemsSource = authors;
            AuthorIdToDeleteComboBox.ItemsSource = authors;
            UpdatedDishAuthorIdInput.ItemsSource = authors;
            DishAuthorIdInput.ItemsSource = authors;
        }

        private void LoadAuthorsToDataGrid(string searchQuery = null)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM authors";
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query += " WHERE ФИО LIKE @search";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        command.Parameters.AddWithValue("@search", $"%{searchQuery}%");
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            AuthorsDataGrid.ItemsSource = dataTable.DefaultView; // Устанавливаем источник данных
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchTextBox.Text;
            LoadAuthorsToDataGrid(string.IsNullOrWhiteSpace(searchQuery) ? null : searchQuery);
        }

        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "SELECT * FROM authors";

                DataTable dataTable = new DataTable();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                }

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = Path.Combine(desktopPath, "authors.xlsx");

                using (ExcelPackage package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Authors");

                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = dataTable.Columns[i].ColumnName;
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    }

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            var cell = worksheet.Cells[i + 2, j + 1];
                            var value = dataTable.Rows[i][j];

                            if (value is DateTime dateValue)
                            {
                                cell.Value = dateValue;
                                cell.Style.Numberformat.Format = "dd.MM.yyyy";
                            }
                            else if (value is int || value is long || value is double || value is decimal)
                            {
                                cell.Value = value;
                            }
                            else
                            {
                                cell.Value = value?.ToString();
                            }
                        }
                    }

                    worksheet.Cells.AutoFitColumns();

                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                }

                MessageBox.Show($"Данные успешно экспортированы в Excel.\nФайл сохранен по пути: {filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LoadCategoryDishAndMenuIds()
        {
            List<Category> categories = new List<Category>();
            List<Dish> dishes = new List<Dish>();
            List<Menu> menus = new List<Menu>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id, название FROM categories";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }

                query = "SELECT id, название FROM dishes";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dishes.Add(new Dish
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }

                query = "SELECT id, название FROM menu";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            menus.Add(new Menu
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            CategoryDishIdInput.ItemsSource = dishes;
            CategoryMenuIdInput.ItemsSource = menus;

            CategoryIdToUpdateComboBox.ItemsSource = categories;
            CategoryIdToDeleteComboBox.ItemsSource = categories;

            CategoryIdToUpdateComboBox.SelectedValuePath = "Id";
            CategoryIdToDeleteComboBox.SelectedValuePath = "Id";
        }

        private void AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            string name = AuthorNameInput.Text;
            string dob = AuthorDOBInput.SelectedDate?.ToString("yyyy-MM-dd") ?? "NULL";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO authors (ФИО, дата_рождения) VALUES (@name, @dob)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@dob", dob == "NULL" ? (object)DBNull.Value : dob);
                    command.ExecuteNonQuery();
                }
            }
            LoadAuthorIds();
            LoadAllData();
        }

        private void UpdateAuthor_Click(object sender, RoutedEventArgs e)
        {
            var selectedAuthor = AuthorIdToUpdateComboBox.SelectedItem as Author;

            if (selectedAuthor != null)
            {
                int id = selectedAuthor.Id;
                string name = UpdatedAuthorNameInput.Text;

                object dob = UpdatedAuthorDOBInput.SelectedDate.HasValue ?
                             (object)UpdatedAuthorDOBInput.SelectedDate.Value.ToString("yyyy-MM-dd") :
                             DBNull.Value;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE authors SET ФИО = @name, дата_рождения = @dob WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@dob", dob);
                        command.ExecuteNonQuery();
                    }
                }

                LoadAuthorIds();
                LoadAllData();
            }
            else
            {
                MessageBox.Show("Выберите автора для обновления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteAuthor_Click(object sender, RoutedEventArgs e)
        {
            var selectedAuthor = AuthorIdToDeleteComboBox.SelectedItem as Author;

            if (selectedAuthor != null)
            {
                int id = selectedAuthor.Id;

                ExecuteNonQuery("DELETE FROM orders WHERE dish_id IN (SELECT id FROM dishes WHERE автор_id = @id)", new SqlParameter("@id", id));

                ExecuteNonQuery("DELETE FROM dishes WHERE автор_id = @id", new SqlParameter("@id", id));

                ExecuteNonQuery("DELETE FROM authors WHERE id = @id", new SqlParameter("@id", id));

                LoadAuthorIds();
                LoadDishIds();
                LoadAllData();
            }
            else
            {
                MessageBox.Show("Выберите автора для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            string name = CategoryNameInput.Text;

            Dish selectedDish = CategoryDishIdInput.SelectedItem as Dish;
            Menu selectedMenu = CategoryMenuIdInput.SelectedItem as Menu;

            int? dishId = selectedDish?.Id;
            int? menuId = selectedMenu?.Id;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO categories (название, dish_id, menu_id) VALUES (@name, @dishId, @menuId)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@dishId", dishId.HasValue ? (object)dishId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@menuId", menuId.HasValue ? (object)menuId.Value : DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }

            LoadCategoryDishAndMenuIds();
            LoadCategoryIds();
            LoadAllData();
        }

        private void LoadCategoryDataForUpdate()
        {
            var categories = GetCategoriesFromDatabase();
            CategoryIdToUpdateComboBox.ItemsSource = categories;

            var dishes = GetDishesFromDatabase();
            UpdatedCategoryDishIdInput.ItemsSource = dishes;

            var menus = GetMenusFromDatabase();
            UpdatedCategoryMenuIdInput.ItemsSource = menus;
        }

        private List<Category> GetCategoriesFromDatabase()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT id, название FROM categories";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    var reader = command.ExecuteReader();
                    var categories = new List<Category>();
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                    return categories;
                }
            }
        }

        private List<Dish> GetDishesFromDatabase()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT id, название FROM dishes";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    var reader = command.ExecuteReader();
                    var dishes = new List<Dish>();
                    while (reader.Read())
                    {
                        dishes.Add(new Dish
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                    return dishes;
                }
            }
        }

        private List<Menu> GetMenusFromDatabase()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT id, название FROM menu";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    var reader = command.ExecuteReader();
                    var menus = new List<Menu>();
                    while (reader.Read())
                    {
                        menus.Add(new Menu
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                    return menus;
                }
            }
        }

        private void UpdateCategory_Click(object sender, RoutedEventArgs e)
        {
            string id = CategoryIdToUpdateComboBox.SelectedValue?.ToString();
            string name = UpdatedCategoryNameInput.Text;
            string dishId = UpdatedCategoryDishIdInput.SelectedValue?.ToString() ?? "NULL";
            string menuId = UpdatedCategoryMenuIdInput.SelectedValue?.ToString() ?? "NULL";

            if (id != null)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE categories SET название = @name, dish_id = @dishId, menu_id = @menuId WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@dishId", dishId == "NULL" ? (object)DBNull.Value : dishId);
                        command.Parameters.AddWithValue("@menuId", menuId == "NULL" ? (object)DBNull.Value : menuId);
                        command.ExecuteNonQuery();
                    }
                }
            }

            LoadCategoryDishAndMenuIds();
            LoadDataForComboBoxes();
            LoadCategoryDataForUpdate();
            LoadAllData();
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryIdToDeleteComboBox.SelectedItem is Category selectedCategory)
            {
                int id = selectedCategory.Id;
                ExecuteNonQuery("DELETE FROM categories WHERE id = @id", new SqlParameter("@id", id));
                LoadCategoryDishAndMenuIds();
                LoadAllData();
            }

            LoadCategoryDishAndMenuIds();
        }

        private void LoadCategoryIds()
        {
            List<Category> categories = new List<Category>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id, название FROM categories";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            CategoryIdToUpdateComboBox.ItemsSource = categories;
            CategoryIdToDeleteComboBox.ItemsSource = categories;
        }

        private void AddDish_Click(object sender, RoutedEventArgs e)
        {
            string name = DishNameInput.Text;
            string creationDate = DishCreationDateInput.SelectedDate?.ToString("yyyy-MM-dd") ?? "NULL";
            int authorId = (DishAuthorIdInput.SelectedItem as dynamic)?.Id ?? 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO dishes (название, дата_создания, автор_id) VALUES (@name, @creationDate, @authorId)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@creationDate", creationDate == "NULL" ? (object)DBNull.Value : creationDate);
                    command.Parameters.AddWithValue("@authorId", authorId == 0 ? (object)DBNull.Value : authorId);
                    command.ExecuteNonQuery();
                }
            }
            LoadAuthorIds();
            LoadAllData();
        }

        private void UpdateDish_Click(object sender, RoutedEventArgs e)
        {
            int id = (DishIdToUpdateComboBox.SelectedItem as dynamic)?.Id ?? 0;
            string name = UpdatedDishNameInput.Text;
            string creationDate = UpdatedDishCreationDateInput.SelectedDate?.ToString("yyyy-MM-dd") ?? "NULL";
            int authorId = (UpdatedDishAuthorIdInput.SelectedItem as dynamic)?.Id ?? 0;

            if (id != 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE dishes SET название = @name, дата_создания = @creationDate, автор_id = @authorId WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@creationDate", creationDate == "NULL" ? (object)DBNull.Value : creationDate);
                        command.Parameters.AddWithValue("@authorId", authorId == 0 ? (object)DBNull.Value : authorId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            LoadAuthorIds();
            LoadDishIds();
            LoadAllData();
        }

        private void DeleteDish_Click(object sender, RoutedEventArgs e)
        {
            int id = (DishIdToDeleteComboBox.SelectedItem as dynamic)?.Id ?? 0;

            if (id != 0)
            {
                ExecuteNonQuery("DELETE FROM orders WHERE dish_id = @id", new SqlParameter("@id", id));

                ExecuteNonQuery("DELETE FROM dishes WHERE id = @id", new SqlParameter("@id", id));

                LoadAuthorIds();
                LoadDishIds();
                LoadAllData();
                LoadOrderIds();
            }
        }

        private void LoadDishIds()
        {
            OrderDishIdInput.Items.Clear();
            UpdatedOrderDishIdInput.Items.Clear();
            DishIdToUpdateComboBox.Items.Clear();
            DishIdToDeleteComboBox.Items.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id, название FROM dishes";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int dishId = reader.GetInt32(0);
                            string dishName = reader.GetString(1);

                            Dish dish = new Dish { Id = dishId, Name = dishName };
                            OrderDishIdInput.Items.Add(dish);
                            UpdatedOrderDishIdInput.Items.Add(dish);
                            DishIdToUpdateComboBox.Items.Add(dish);
                            DishIdToDeleteComboBox.Items.Add(dish);
                        }
                    }
                }
            }
        }


        private List<Dish> GetDishesByCategoryAndDate(int categoryId, DateTime date)
        {
            List<Dish> dishes = new List<Dish>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT d.id, d.название
            FROM dishes d
            JOIN categories c ON d.id = c.dish_id
            JOIN menu m ON c.menu_id = m.id
            WHERE c.id = @CategoryId AND m.дата = @Date";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryId", categoryId);
                    command.Parameters.AddWithValue("@Date", date);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dishes.Add(new Dish
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return dishes;
        }

        private List<Dish> GetPopularDishes(DateTime startDate, DateTime endDate)
        {
            List<Dish> popularDishes = new List<Dish>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT d.id, d.название, COUNT(o.dish_id) AS Популярность
                    FROM orders o
                    JOIN dishes d ON o.dish_id = d.id
                    WHERE o.Дата BETWEEN @StartDate AND @EndDate
                    GROUP BY d.id, d.название
                    ORDER BY Популярность DESC";


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            popularDishes.Add(new Dish
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return popularDishes;
        }


        private void LoadCategoryData()
        {
            var categories = GetCategoriesFromDatabase();
            CategoryFilterComboBox.ItemsSource = categories;
        }



        private void GetDishesByCategoryAndDate_Click(object sender, RoutedEventArgs e)
        {
            int categoryId = (CategoryFilterComboBox.SelectedItem as Category)?.Id ?? 0;
            DateTime? date = DishDatePicker.SelectedDate;

            if (categoryId != 0 && date.HasValue)
            {
                var dishes = GetDishesByCategoryAndDate(categoryId, date.Value);
                DishesDataGrid.ItemsSource = dishes;
            }
            else
            {
                MessageBox.Show("Выберите категорию и дату.");
            }
        }


        private void GetPopularDishes_Click(object sender, RoutedEventArgs e)
        {
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;

            if (startDate.HasValue && endDate.HasValue)
            {
                var popularDishes = GetPopularDishes(startDate.Value, endDate.Value);
                DishesDataGrid.ItemsSource = popularDishes;
            }
            else
            {
                MessageBox.Show("Выберите начальную и конечную дату.");
            }
        }




        private void AddMenu_Click(object sender, RoutedEventArgs e)
        {
            string name = MenuNameInput.Text;
            DateTime? date = MenuDateInput.SelectedDate;

            if (date != null)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO menu (название, Дата) VALUES (@name, @date)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@date", date.Value);
                        command.ExecuteNonQuery();
                    }
                }
                LoadAllData();
            }
            else
            {
                MessageBox.Show("Выберите дату меню.");
            }
            LoadMenuIds();

        }

        private void UpdateMenu_Click(object sender, RoutedEventArgs e)
        {
            Menu selectedMenu = MenuIdToUpdateComboBox.SelectedItem as Menu;
            string name = UpdatedMenuNameInput.Text;
            DateTime? date = UpdatedMenuDateInput.SelectedDate;

            if (selectedMenu != null && date != null)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE menu SET название = @name, Дата = @date WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", selectedMenu.Id);
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@date", date.Value);
                        command.ExecuteNonQuery();
                    }
                }
                LoadAllData();
            }
            else
            {
                MessageBox.Show("Выберите дату меню и ID.");
            }
            LoadMenuIds();

        }


        private void DeleteMenu_Click(object sender, RoutedEventArgs e)
        {
            var selectedMenu = MenuIdToDeleteComboBox.SelectedItem as Menu;

            if (selectedMenu != null)
            {
                int menuId = selectedMenu.Id;

                ExecuteNonQuery("DELETE FROM categories WHERE menu_id = @menuId", new SqlParameter("@menuId", menuId));

                ExecuteNonQuery("DELETE FROM menu WHERE id = @menuId", new SqlParameter("@menuId", menuId));

                LoadMenuIds();
                LoadAllData();
            }
            else
            {
                MessageBox.Show("Выберите меню для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LoadMenuIds()
        {
            MenuIdToUpdateComboBox.Items.Clear();
            MenuIdToDeleteComboBox.Items.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT id, название FROM menu";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int menuId = reader.GetInt32(0);
                                string menuName = reader.GetString(1);
                                var menu = new Menu { Id = menuId, Name = menuName };

                                MenuIdToUpdateComboBox.Items.Add(menu);
                                MenuIdToDeleteComboBox.Items.Add(menu);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            string customerName = OrderCustomerNameInput.Text;
            string orderDate = OrderDateInput.SelectedDate?.ToString("yyyy-MM-dd") ?? "NULL";
            int dishId = (OrderDishIdInput.SelectedItem as dynamic)?.Id ?? 0;
            int quantity = int.TryParse(OrderQuantityInput.Text, out var q) ? q : 0;

            if (dishId != 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO orders (client_name, Дата, dish_id, Количество) VALUES (@customerName, @orderDate, @dishId, @quantity)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@customerName", customerName);
                        command.Parameters.AddWithValue("@orderDate", orderDate == "NULL" ? (object)DBNull.Value : orderDate);
                        command.Parameters.AddWithValue("@dishId", dishId == 0 ? (object)DBNull.Value : dishId);
                        command.Parameters.AddWithValue("@quantity", quantity);
                        command.ExecuteNonQuery();
                    }
                }
                LoadOrderIds();
                LoadDishIds();
                LoadAllData();
            }
            else
            {
                MessageBox.Show("Выберите корректное блюдо.");
            }
        }


        private void UpdateOrder_Click(object sender, RoutedEventArgs e)
        {
            string id = (OrderIdToUpdateComboBox.SelectedItem as dynamic)?.Id.ToString();
            string customerName = UpdatedOrderCustomerNameInput.Text;
            string orderDate = UpdatedOrderDateInput.SelectedDate?.ToString("yyyy-MM-dd") ?? "NULL";
            int dishId = (UpdatedOrderDishIdInput.SelectedItem as dynamic)?.Id ?? 0;
            int quantity = int.TryParse(UpdatedOrderQuantityInput.Text, out var q) ? q : 0;

            if (id != null && dishId != 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE orders SET client_name = @customerName, Дата = @orderDate, dish_id = @dishId, Количество = @quantity WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@customerName", customerName);
                        command.Parameters.AddWithValue("@orderDate", orderDate == "NULL" ? (object)DBNull.Value : orderDate);
                        command.Parameters.AddWithValue("@dishId", dishId == 0 ? (object)DBNull.Value : dishId);
                        command.Parameters.AddWithValue("@quantity", quantity);
                        command.ExecuteNonQuery();
                    }
                }
                LoadOrderIds();
                LoadAllData();
            }
            else
            {
                MessageBox.Show("Выберите корректное блюдо и ID заказа.");
            }
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            string id = (OrderIdToDeleteComboBox.SelectedItem as dynamic)?.Id.ToString();
            if (id != null)
            {
                ExecuteNonQuery("DELETE FROM orders WHERE id = @id", new SqlParameter("@id", id));
            }
            else
            {
                MessageBox.Show("Выберите корректный заказ для удаления.");
            }
            LoadAllData();
            LoadOrderIds();
        }


        private void LoadOrderIds()
        {
            ObservableCollection<Order> orderItems = new ObservableCollection<Order>();

            try
            {
                OrderIdToUpdateComboBox.Items.Clear();
                OrderIdToDeleteComboBox.Items.Clear();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT id, client_name FROM orders";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int orderId = reader.GetInt32(0);
                                string customerName = reader.GetString(1);
                                var orderItem = new Order { Id = orderId, Name = customerName };

                                OrderIdToUpdateComboBox.Items.Add(orderItem);
                                OrderIdToDeleteComboBox.Items.Add(orderItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDataForComboBoxes()
        {
            var dishes = GetDishesFromDatabase();
            CategoryDishIdInput.ItemsSource = dishes;
            UpdatedCategoryDishIdInput.ItemsSource = dishes;

            var menus = GetMenusFromDatabase();
            CategoryMenuIdInput.ItemsSource = menus;
            UpdatedCategoryMenuIdInput.ItemsSource = menus;
        }


        private void ExecuteNonQuery(string query, SqlParameter parameter)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(parameter);
                    command.ExecuteNonQuery();
                }
            }
        }

        public class Author
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime? Birth { get; set; }


            public override string ToString()
            {
                return $"ID: {Id} - {Name}";
            }
        }

        public class Category
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return $"ID: {Id} - {Name}";
            }
        }


        public class Menu
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return $"ID: {Id} - {Name}";
            }
        }

        public class Dish
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return $"ID: {Id} - {Name}";
            }
        }

        public class Order
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return $"ID: {Id} - {Name}";
            }
        }
    }
}

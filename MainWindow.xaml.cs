using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace BuyersAndOrders
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Список всех клиентов.
        /// </summary>
        public static List<Client> AllClients = new List<Client>();

        /// <summary>
        /// Список всех продуктов.
        /// </summary>
        public static List<Item> Products = new List<Item>();

        /// <summary>
        /// Бракованный товар.
        /// </summary>
        Item DefectItem { get; set; }

        /// <summary>
        /// Авторизовавшийся клиент.
        /// </summary>
        Client CurrentClient { get; set; }

        /// <summary>
        /// Текущий заказ клиента.
        /// </summary>
        Order CurrentOrder { get; set; }

        /// <summary>
        /// Флаг, показывающий является ли авторизавнный пользователь продавцом.
        /// </summary>
        bool IsSeller { get; set; }

        /// <summary>
        /// Флаг, показывающий нужно ли отображать только активные заказы.
        /// </summary>
        bool IsOnlyActiveOrders { get; set; } = false;

        /// <summary>
        /// Инициализаци главного окна.
        /// </summary>
        public MainWindow()
        {
            try
            {

                InitializeComponent();
                // Десериализация.
                Deserialize();
                // Авторизация пользователя.
                var logInDialog = new LogInDialog(AllClients);
                logInDialog.ShowDialog();
                if (logInDialog.DialogResult != true)
                    Close();
                else
                {
                    IsSeller = logInDialog.Login == "seller@gmail.com";
                    if (!IsSeller)
                        CurrentClient = AllClients.Single(client => logInDialog.Login == client.Login);
                    if (IsSeller)
                        SellerMode();
                    else
                        ClientMode();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Установка интерфейса клиента.
        /// </summary>
        void ClientMode()
        {
            try
            {
                ShowCartButton.Visibility = Visibility.Visible;
                ItemsDataGrid.Visibility = Visibility.Visible;
                OrdersDataGrid.Visibility = Visibility.Hidden;
                ListOfCLientsButton.Visibility = Visibility.Hidden;
                ListOfOrdersLabel.Visibility = Visibility.Visible;
                ShowOrdersButton.Visibility = Visibility.Visible;
                OnlyActiveOrdersButton.Visibility = Visibility.Hidden;
                ClientsDataGrid.Visibility = Visibility.Hidden;
                DefectItemLabel.Visibility = Visibility.Hidden;
                ListOfCLientsWithDefectItemButton.Visibility = Visibility.Hidden;
                ListOfCLientsMoreThanPriceButton.Visibility = Visibility.Hidden;
                MinPriceLable.Visibility = Visibility.Hidden;
                MinPriceTextBox.Visibility = Visibility.Hidden;
                MakeDefectMenuItem.Visibility = Visibility.Hidden;
                ListOfOrdersLabel.Content = "Список всех товаров:";
                StatusLabel.Content = $"Статус: Клиент {CurrentClient.Login}";
                ItemsDataGrid.Items.Clear();
                foreach (var item in Products)
                {
                    ItemsDataGrid.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Установка интерфейса продавца.
        /// </summary>
        void SellerMode()
        {
            try
            {
                ShowCartButton.Visibility = Visibility.Hidden;
                MakeDefectMenuItem.Visibility = Visibility.Visible;
                ItemsDataGrid.Visibility = Visibility.Hidden;
                ListOfCLientsButton.Visibility = Visibility.Visible;
                OrdersDataGrid.Visibility = Visibility.Visible;
                ListOfOrdersLabel.Visibility = Visibility.Visible;
                ShowOrdersButton.Visibility = Visibility.Visible;
                OnlyActiveOrdersButton.Visibility = Visibility.Visible;
                ClientsDataGrid.Visibility = Visibility.Hidden;
                ListOfCLientsWithDefectItemButton.Visibility = Visibility.Visible;
                DefectItemLabel.Visibility = Visibility.Visible;
                DefectItemLabel.Content = "Бракованный товар:";
                ListOfCLientsMoreThanPriceButton.Visibility = Visibility.Visible;
                MinPriceLable.Visibility = Visibility.Visible;
                MinPriceTextBox.Visibility = Visibility.Visible;
                ListOfOrdersLabel.Content = "Список всех оформленных заказов:";
                StatusLabel.Content = "Статус: Продавец";
                OrdersDataGrid.Items.Clear();
                foreach (var user in AllClients)
                {
                    foreach (var order in user.Orders)
                        OrdersDataGrid.Items.Add(order);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Автосохранение состояние проекта.
        /// </summary>
        private void Serialize()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            var json = JsonConvert.SerializeObject(AllClients, settings);
            File.WriteAllText("clients.json", json);
            json = JsonConvert.SerializeObject(Products, settings);
            File.WriteAllText("items.json", json);
        }

        /// <summary>
        ///  Сериализация при закрии программы.
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Очистка корзины и добавление некупленных товаро обратно в список.
            if (CurrentOrder != null)
            {
                foreach (var item in CurrentOrder.OrderedItems)
                    Products.Single(product => product.ArticleNumber == item.ArticleNumber).AmountLeft += item.AmountLeft;
            }
            Serialize();
        }

        /// <summary>
        /// Десериализация
        /// </summary>
        /// <param name="pathToCLients"> Путь к сериализированным пользователям. </param>
        /// <param name="pathToItems"> Путь к сериализированным товарам.</param>
        private void Deserialize(string pathToCLients = "clients.json", string pathToItems = "items.json")
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
                Products = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(pathToItems), settings);
                AllClients = JsonConvert.DeserializeObject<List<Client>>(File.ReadAllText(pathToCLients), settings);
                // Установка заказчика для каждого заказа.
                foreach (var client in AllClients)
                {
                    foreach (var order in client.Orders)
                        order.OrderingClient = client;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка десериализации! Будет создан новый пустой проект.");
                AllClients = new List<Client>();
                if (Products == null)
                    Products = new List<Item>();
            }
        }

        /// <summary>
        /// Добавление товара в корзину.
        /// </summary>
        private void AddToCartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedProduct = (Item)ItemsDataGrid.SelectedItem;
                if (selectedProduct.AmountLeft == 0)
                    throw new Exception("Вы не можете добавить этот товар на склад, так как его пока нет в наличии.");
                if (CurrentOrder == null)
                    CurrentOrder = new Order(CurrentClient);

                if (CurrentOrder.OrderedItems.Any(item => item.ArticleNumber == selectedProduct.ArticleNumber))
                    CurrentOrder.OrderedItems.First(item => item.ArticleNumber == selectedProduct.ArticleNumber).AmountLeft += 1;
                else
                    CurrentOrder.OrderedItems.Add(new Item(selectedProduct.Name, selectedProduct.ArticleNumber, 1, selectedProduct.Price));
                selectedProduct.AmountLeft -= 1;

                ReloadDataAboutProducts();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Отображение корзины.
        /// </summary>
        private void ShowCartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cartDialog = new CartDialog(CurrentOrder);
                cartDialog.ShowDialog();
                CurrentOrder = cartDialog.CurrentOrder;
                ReloadDataAboutProducts();
                if (cartDialog.DialogResult != true)
                    return;
                CurrentClient.Orders.Add(cartDialog.CurrentOrder);
                MessageBox.Show("Ваш заказ успешно оформлен!");
                CurrentOrder = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Обновление списка товаров.
        /// </summary>
        public void ReloadDataAboutProducts()
        {
            ItemsDataGrid.Items.Clear();
            foreach (var item in Products)
            {
                ItemsDataGrid.Items.Add(item);
            }
        }

        /// <summary>
        /// Установка доступности элементов контекстного меню.
        /// </summary>
        private void ItemsDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            try
            {
                AddToCartMenuItem.IsEnabled = false;
                MakeDefectMenuItem.IsEnabled = false;
                var selectedRow = (Item)ItemsDataGrid.SelectedItem;
                if (selectedRow != null)
                {
                    if (IsSeller)
                        MakeDefectMenuItem.IsEnabled = true;
                    else
                        AddToCartMenuItem.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");

            }
        }

        /// <summary>
        /// Отображение списка оформленных заказов.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Отображение списка всех оформленных заказов для продавца.
                if (IsSeller)
                {
                    IsOnlyActiveOrders = false;
                    OrdersDataGrid.Visibility = Visibility.Visible;
                    ClientsDataGrid.Visibility = Visibility.Hidden;
                    ItemsDataGrid.Visibility = Visibility.Hidden;
                    ReloadDataAboutOrders();
                    ListOfOrdersLabel.Content = "Список всех оформленных заказов:";
                }
                // Отображение оформленных заказов авторизованного клиента.
                else
                {
                    var orderDialog = new FormedOrdersDialog(CurrentClient, IsSeller);
                    orderDialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Список клиентов для продавца.
        /// </summary>
        private void ListOfCLientsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OrdersDataGrid.Visibility = Visibility.Hidden;
                ClientsDataGrid.Visibility = Visibility.Visible;
                ItemsDataGrid.Visibility = Visibility.Hidden;
                ClientsDataGrid.Items.Clear();
                ListOfOrdersLabel.Content = "Для просмотра списка заказов клиента дважды кликните по нему.\nСписок клиентов:";
                if (AllClients.Count == 0 || AllClients == null)
                    MessageBox.Show("У Вас пока нет ни одного клиента.");
                else
                {
                    // УДаление ненужных столбцов.
                    if (ClientsDataGrid.Columns.Any(item => item.Header.ToString() == "Время заказа"))
                        ClientsDataGrid.Columns.Remove(ClientsDataGrid.Columns.First(item => item.Header.ToString() == "Время заказа"));
                    if (ClientsDataGrid.Columns.Any(item => item.Header.ToString() == "Потраченная сумма"))
                        ClientsDataGrid.Columns.Remove(ClientsDataGrid.Columns.First(item => item.Header.ToString() == "Потраченная сумма"));
                    foreach (var client in AllClients)
                    {
                        ClientsDataGrid.Items.Add(client);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Отображение заказов клиента для продавца.
        /// </summary>
        private void ClientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selectedRow = (Client)ClientsDataGrid.SelectedItem;
                if (selectedRow == null)
                    return;
                var orderDialog = new FormedOrdersDialog(selectedRow, true);
                orderDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Установка доступности элементов контекстного меню.
        /// </summary>
        private void OrdersDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            try
            {
                MakeProcessedMenuItem.IsEnabled = false;
                MakeShippedMenuItem.IsEnabled = false;
                MakeExecutedMenuItem.IsEnabled = false;
                OrderItemsMenuItem.IsEnabled = false;
                var selectedRow = (Order)OrdersDataGrid.SelectedItem;
                if (selectedRow != null)
                {
                    OrderItemsMenuItem.IsEnabled = true;
                    // Проверки статуса заказа.
                    if ((selectedRow.Status & OrderStatus.Processed) != OrderStatus.Processed)
                    {
                        MakeProcessedMenuItem.IsEnabled = true;
                        return;
                    }
                    if (selectedRow.Status >= OrderStatus.Paid && selectedRow.Status < OrderStatus.Shipped)
                    {
                        MakeShippedMenuItem.IsEnabled = true;
                        return;
                    }
                    if (selectedRow.Status >= OrderStatus.Paid && selectedRow.Status < OrderStatus.Executed)
                    {
                        MakeExecutedMenuItem.IsEnabled = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Установка статуса "Обработан".
        /// </summary>
        private void MakeProcessedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = (Order)OrdersDataGrid.SelectedItem;
                if (selectedRow != null)
                {
                    selectedRow.Status |= OrderStatus.Processed;
                }
                if (IsOnlyActiveOrders)
                    ReloadDataOnlyAboutActiveOrders();
                else
                    ReloadDataAboutOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");

            }
        }

        /// <summary>
        /// Установка статуса "Отгружен".
        /// </summary>
        private void MakeShippedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = (Order)OrdersDataGrid.SelectedItem;
                if (selectedRow != null)
                {
                    selectedRow.Status |= OrderStatus.Shipped;
                }
                if (IsOnlyActiveOrders)
                    ReloadDataOnlyAboutActiveOrders();
                else
                    ReloadDataAboutOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");

            }
        }

        /// <summary>
        /// Установка статуса "Исполнен".
        /// </summary>
        private void MakeExecutedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = (Order)OrdersDataGrid.SelectedItem;
                if (selectedRow != null)
                {
                    selectedRow.Status |= OrderStatus.Executed;
                }
                if (IsOnlyActiveOrders)
                    ReloadDataOnlyAboutActiveOrders();
                else
                    ReloadDataAboutOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");

            }
        }

        /// <summary>
        /// Обновление данных о всех оформленных заказах.
        /// </summary>
        void ReloadDataAboutOrders()
        {
            try
            {
                OrdersDataGrid.Items.Clear();
                foreach (var user in AllClients)
                {
                    foreach (var order in user.Orders)
                        OrdersDataGrid.Items.Add(order);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");

            }
        }

        /// <summary>
        /// Обновление данных об активных заказах.
        /// </summary>
        void ReloadDataOnlyAboutActiveOrders()
        {
            try
            {
                OrdersDataGrid.Items.Clear();
                foreach (var user in AllClients)
                {
                    foreach (var order in user.Orders)
                    {
                        if (order.Status < OrderStatus.Executed)
                            OrdersDataGrid.Items.Add(order);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");

            }
        }

        /// <summary>
        /// Детали заказа -- список его товаров.
        /// </summary>
        private void OrderItemsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = (Order)OrdersDataGrid.SelectedItem;
                if (selectedRow != null)
                {
                    var itemsDialog = new CartDialog(selectedRow);
                    itemsDialog.Title = $"Список товаров заказа {selectedRow.OrderNumber}";
                    itemsDialog.MakeOrderButton.Visibility = Visibility.Hidden;
                    itemsDialog.CartItemsContextMenu.Visibility = Visibility.Hidden;
                    itemsDialog.ShowDialog();
                }
                ReloadDataAboutOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Отображение только активных заказов.
        /// </summary>
        private void OnlyActiveOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            OrdersDataGrid.Visibility = Visibility.Visible;
            ClientsDataGrid.Visibility = Visibility.Hidden;
            ItemsDataGrid.Visibility = Visibility.Hidden;
            IsOnlyActiveOrders = true;
            ListOfOrdersLabel.Content = "Список только активных заказов:";
            ReloadDataOnlyAboutActiveOrders();
        }

        /// <summary>
        /// Отображение всех товаров.
        /// </summary>
        private void ListOfItemsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OrdersDataGrid.Visibility = Visibility.Hidden;
                ClientsDataGrid.Visibility = Visibility.Hidden;
                ItemsDataGrid.Visibility = Visibility.Visible;
                ListOfOrdersLabel.Content = "Список всех товаров:";
                if (IsSeller)
                    AddToCartMenuItem.Visibility = Visibility.Hidden;
                else
                    MakeDefectMenuItem.Visibility = Visibility.Hidden;
                ReloadDataAboutProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Список заказов и клиентов, купивших бракованный товар.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListOfCLientsWithDefectItemButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DefectItem == null)
                    throw new Exception("Вы пока не выбрали товар с браком, поэтому эта операция невозможна." +
                        "\nЧтобы сделать это, откройте список товаров и устнавите дефектный товар, кликнув на него правой кнопкой мыши.");
                OrdersDataGrid.Visibility = Visibility.Hidden;
                ClientsDataGrid.Visibility = Visibility.Visible;
                ItemsDataGrid.Visibility = Visibility.Hidden;
                ClientsDataGrid.Items.Clear();
                if (ClientsDataGrid.Columns.Any(item => item.Header.ToString() == "Потраченная сумма"))
                    ClientsDataGrid.Columns.Remove(ClientsDataGrid.Columns.First(item => item.Header.ToString() == "Потраченная сумма"));
                if (!ClientsDataGrid.Columns.Any(item => item.Header.ToString() == "Время заказа"))
                    ClientsDataGrid.Columns.Add(new DataGridTextColumn { IsReadOnly = true, Header = "Время заказа" });
                ListOfOrdersLabel.Content = $"Список клиентов, заказавших бракованный товар -- {DefectItem.Name}";
                var i = 0;
                foreach (var client in AllClients)
                {
                    foreach (var order in client.Orders)
                    {
                        if (order.OrderedItems.Any(item => item.Name == DefectItem.Name && item.ArticleNumber == DefectItem.ArticleNumber))
                        {
                            client.DateOfDefectOrder = order.OrderDate;
                            ClientsDataGrid.Items.Add(client);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }

        }

        /// <summary>
        /// Выбор продавцом бракованного товара.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MakeDefectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedProduct = (Item)ItemsDataGrid.SelectedItem;
                if (selectedProduct != null)
                {
                    DefectItem = selectedProduct;
                    DefectItemLabel.Content = $"Бракованный товар:\n\t{DefectItem.Name}";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Отображение клиентов, оплативших заказы на сумму, превышающую заданную продавцом.
        /// </summary>
        private void ListOfCLientsMoreThanPriceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MinPriceTextBox.Text == "")
                    throw new Exception("Для начала задайте сумму в поле ниже.");
                if (!double.TryParse(MinPriceTextBox.Text, out var minPrice) || minPrice < 0)
                    throw new Exception("Сумма должна быть положительным вещественным числом.");
                OrdersDataGrid.Visibility = Visibility.Hidden;
                ClientsDataGrid.Visibility = Visibility.Visible;
                ItemsDataGrid.Visibility = Visibility.Hidden;
                ClientsDataGrid.Items.Clear();
                if (ClientsDataGrid.Columns.Any(item => item.Header.ToString() == "Время заказа"))
                    ClientsDataGrid.Columns.Remove(ClientsDataGrid.Columns.First(item => item.Header.ToString() == "Время заказа"));
                if (!ClientsDataGrid.Columns.Any(item => item.Header.ToString() == "Потраченная сумма"))
                    ClientsDataGrid.Columns.Add(new DataGridTextColumn { IsReadOnly = true, Header = "Потраченная сумма", Binding = new Binding("PaidSum") });
                ListOfOrdersLabel.Content = $"Список клиентов, оплативших заказы на сумму не меньше {minPrice}.";
                var listOfCLients = AllClients.Where(client => client.PaidSum >= minPrice).OrderByDescending(client => client.PaidSum);
                foreach (var client in listOfCLients)
                {
                    ClientsDataGrid.Items.Add(client);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Выход из учетной записи.
        /// </summary>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Очистка корзины, если она не пустая.
            if (CurrentOrder != null)
            {
                foreach (var item in CurrentOrder.OrderedItems)
                    Products.Single(product => product.ArticleNumber == item.ArticleNumber).AmountLeft += item.AmountLeft;
            }
            Serialize();
            CurrentClient = null;
            CurrentOrder = null;
            IsSeller = false;
            IsOnlyActiveOrders = false;
            Hide();
            var logInDialog = new LogInDialog(AllClients);
            logInDialog.ShowDialog();
            if (logInDialog.DialogResult != true)
                Close();
            else
            {
                Show();
                IsSeller = logInDialog.Login == "seller@gmail.com";
                if (!IsSeller)
                    CurrentClient = AllClients.Single(client => logInDialog.Login == client.Login);
                if (IsSeller)
                    SellerMode();
                else
                    ClientMode();
            }
        }
    }
}

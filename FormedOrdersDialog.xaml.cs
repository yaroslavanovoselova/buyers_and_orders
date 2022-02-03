using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BuyersAndOrders
{
    /// <summary>
    /// Логика взаимодействия для FormedOrdersDialog.xaml
    /// </summary>
    public partial class FormedOrdersDialog : Window
    {
        /// <summary>
        /// Клиент, чьи заказы необходимо просмотреть.
        /// </summary>
        public Client CurrentClient { get; set; }

        /// <summary>
        /// Флаг, показывающий авторизовался ли продавец.
        /// </summary>
        bool IsSeller { get; set; }

        /// <summary>
        /// Оплаченная клиентом сумма.
        /// </summary>
        double PaidSum
        {
            get => CurrentClient.Orders.Where(order => (order.Status & OrderStatus.Paid) == OrderStatus.Paid)
                                        .Select(order => order.OrderPrice)
                                        .Sum();
        }

        /// <summary>
        /// Инициализация окна оформленных заказов.
        /// </summary>
        /// <param name="currClient"> Клиент, чьи заказы необходимо просмотреть. </param>
        /// <param name="isSeller"> Флаг, показывающий авторизовался ли продавец. </param>
        public FormedOrdersDialog(Client currClient, bool isSeller)
        {
            InitializeComponent();
            InfoLabel.Content = "Здесь Вы можете просматривать список оформленных заказов, а также оплачивать* их, \nвыделив необходимый и кликнув на него правой кнопкой мыши.";
            IsSeller = isSeller;
            CurrentClient = currClient;
            if (CurrentClient != null)
            {
                ReloadData();
            }
        }

        /// <summary>
        /// Присваивание заказы статуса "Оплачен".
        /// </summary>
        private void PayForOrderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = (Order)OrdersDataGrid.SelectedItem;
                if (selectedRow != null && !IsSeller)
                {
                    selectedRow.Status |= OrderStatus.Paid;
                    ReloadData();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Установка доступности контекстного меню DataGrid.
        /// </summary>
        private void ItemsDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            try
            {
                OrderItemsMenuItem.IsEnabled = false;
                PayForOrderMenuItem.IsEnabled = false;
                ItemsContextMenu.Visibility = Visibility.Visible;
                var selectedRow = (Order)OrdersDataGrid.SelectedItem;
                if (selectedRow != null)
                {
                    OrderItemsMenuItem.IsEnabled = true;
                    if ((selectedRow.Status & OrderStatus.Processed) == OrderStatus.Processed && (selectedRow.Status & OrderStatus.Paid) != OrderStatus.Paid)
                    {
                        PayForOrderMenuItem.IsEnabled = true;
                    }
                    if (IsSeller)
                        PayForOrderMenuItem.Visibility = Visibility.Hidden;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Обнавление данных DataGrid.
        /// </summary>
        public void ReloadData()
        {
            try
            {
                if (IsSeller)
                {
                    InfoAboutClientLabel.Visibility = Visibility.Visible;
                    InfoAboutClientLabel.Content = $"Клиент: {CurrentClient.Surname} {CurrentClient.Name} {CurrentClient.Patrinymic}" +
                        $"\nE-Mail: {CurrentClient.Login}" +
                        $"\nОплаченная сумма по всем заказам: {PaidSum}";
                }
                else
                {
                    InfoLabel.Visibility = Visibility.Visible;
                }
                OrdersDataGrid.Items.Clear();
                foreach (var order in CurrentClient.Orders)
                {
                    OrdersDataGrid.Items.Add(order);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }
    
        /// <summary>
        /// Отображение деталей заказа -- списка товаров заказа.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }
    }
}

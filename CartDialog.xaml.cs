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
    /// Логика взаимодействия для CartDialog.xaml
    /// </summary>
    public partial class CartDialog : Window
    {
        /// <summary>
        /// Текущий заказ.
        /// </summary>
        public Order CurrentOrder { get; set; }

        /// <summary>
        /// Инициализация корзины-формы.
        /// </summary>
        /// <param name="currOrder"> Текущий заказ. </param>
        public CartDialog(Order currOrder)
        {
            InitializeComponent();
            CurrentOrder = currOrder;
            // Выводим список товаров заказа.
            if (CurrentOrder != null)
            {
                OrderPriceLabel.Content = $"Общая стоимость: {CurrentOrder.OrderPrice}";
                foreach (var item in CurrentOrder.OrderedItems)
                {
                    ItemsDataGrid.Items.Add(item);
                }

            }
        }

        /// <summary>
        /// Оформление заказа.
        /// </summary>
        private void MakeOrderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentOrder == null || CurrentOrder.OrderedItems.Count == 0)
                    MessageBox.Show("В корзине нет товаров, поэтому заказ не может быть оформлен.");
                else
                {
                    DialogResult = true;
                    CurrentOrder.OrderDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Удаление товара из корзины.
        /// </summary>
        private void DeleteFromCartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedProduct = (Item)ItemsDataGrid.SelectedItem;
                MainWindow.Products.First(item => item.ArticleNumber == selectedProduct.ArticleNumber).AmountLeft += selectedProduct.AmountLeft;
                CurrentOrder.OrderedItems.Remove(selectedProduct);
                ItemsDataGrid.Items.Clear();
                foreach (var item in CurrentOrder.OrderedItems)
                {
                    ItemsDataGrid.Items.Add(item);
                }
                OrderPriceLabel.Content = $"Общая стоимость: {CurrentOrder.OrderPrice}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Установка доступности элементов контекстного меню DataGrid.
        /// </summary>
        private void ItemsDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            try
            {
                var selectedRow = (Item)ItemsDataGrid.SelectedItem;
                if (selectedRow != null)
                {
                    DeleteFromCartMenuItem.IsEnabled = true;
                }
                else
                {
                    DeleteFromCartMenuItem.IsEnabled = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }
    }
}

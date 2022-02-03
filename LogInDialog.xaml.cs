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
    /// Логика взаимодействия для LogInDialog.xaml
    /// </summary>
    public partial class LogInDialog : Window
    {
        /// <summary>
        /// Список всех пользователей.
        /// </summary>
        public List<Client> AllUsers = new List<Client>();

        /// <summary>
        /// Введенный логин клиента.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Инициализация формы.
        /// </summary>
        /// <param name="users"> Список всех клиентов. </param>
        public LogInDialog(List<Client> users)
        {
            InitializeComponent();
            AllUsers = users;
        }

        /// <summary>
        /// Авторизация пользователя.
        /// </summary>
        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var login = LoginTextBox.Text;
                var pass = PasswordTextBox.Text;
                if (login.Length == 0 || pass.Length == 0)
                {
                    MessageBox.Show("Заполните все необходимые поля.");
                    return;
                }
                // Вход продавца.
                if (login == "seller@gmail.com")
                {
                    if (pass != "12345678")
                    {
                        MessageBox.Show("Введен некорректный пароль");
                        return;
                    }
                    else
                        Login = login;
                }
                // Вход пользователя.
                else
                {
                    if (AllUsers.All(x => x.Login != login))
                    {
                        MessageBox.Show("Введен некорректный логин");
                        return;
                    }
                    else if (AllUsers.Single(x => x.Login == login).Hash != Client.GetSHA256(login + pass))
                    {
                        MessageBox.Show("Введен некорректный пароль");
                        return;
                    }
                    else
                    {
                        Login = login;
                    }
                }
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }

        /// <summary>
        /// Регистрация пользователя. 
        /// </summary>
        private void CreateAccButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var registrationDialog = new RegistrationDialog(AllUsers);
                registrationDialog.ShowDialog();
                if (registrationDialog.NewUser != null)
                {
                    Login = registrationDialog.NewUser.Login;
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка!");
            }
        }
    }
}

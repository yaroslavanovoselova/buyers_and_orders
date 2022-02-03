using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Linq;

namespace BuyersAndOrders
{
    /// <summary>
    /// Логика взаимодействия для RegistrationDialog.xaml
    /// </summary>
    public partial class RegistrationDialog : Window
    {
        /// <summary>
        /// Новый зарегистрировавшийся пользователь.
        /// </summary>
        public Client NewUser;

        /// <summary>
        /// Список всех клиентов.
        /// </summary>
        List<Client> AllUsers = new List<Client>();

        /// <summary>
        /// Инициализация окна регистрации.
        /// </summary>
        /// <param name="allUsers"> Список всех пользователей. </param>
        public RegistrationDialog(List<Client> allUsers)
        {
            InitializeComponent();
            AllUsers = allUsers;
        }

        /// <summary>
        /// Завершение регистрации.
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var surname = SurnameTextBox.Text.Trim();
                var name = NameTextBox.Text.Trim();
                var patrinymic = PatrinymicTextBox.Text.Trim();
                var adress = AdressTextBox.Text.Trim();
                var email = LoginTextBox.Text.Trim();
                var pass = PasswordTextBox.Text.Trim();
                if (surname.Length == 0 || name.Length == 0 || email.Length == 0 ||
                    adress.Length == 0 || pass.Length == 0 || PhoneNumberTextBox.Text.Trim().Length == 0)
                    throw new Exception("Вы не заполнили какое-то из обязательных полей.");
                // Проверка на корректность EMail.
                if (!IsValidEmail(email))
                    throw new Exception("Неправильный формат почты.");
                // Проверка на корректность номера телефона.
                if (!ulong.TryParse(PhoneNumberTextBox.Text.Trim(), out var phoneNumber) || PhoneNumberTextBox.Text.Trim()[0] != '8'
                    || PhoneNumberTextBox.Text.Trim().Length!=11)
                    throw new Exception("Неправильный формат телефона. \nНеобходимый формат:8XXXXXXX.");
                if (AllUsers.Any(client => client.Login == email))
                    throw new Exception("Пользователь с таким логином уже существует.");
                if (pass.Length<8)
                    throw new Exception("Длина пароля должна быть не меньше 8 символов.");
                NewUser = new Client(name, surname, patrinymic, phoneNumber, adress, email, pass);
                MessageBox.Show("Пользователь успешно создан!");
                MainWindow.AllClients.Add(NewUser);
                Close();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Проверка на корректность EMail.
        /// </summary>
        /// <param name="emailaddress"></param>
        /// <returns></returns>
        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}

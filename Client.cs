using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BuyersAndOrders
{
    public class Client
    {
        /// <summary>
        /// Имя клиента.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Фамилия клиента.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Отчество клиента.
        /// </summary>
        public string Patrinymic { get; set; } = "";

        /// <summary>
        /// Адрес клиента.
        /// </summary>
        public string Adress { get; set; }

        /// <summary>
        /// Номер телефона клиента.
        /// </summary>
        public ulong  PhoneNumber { get; set; }

        /// <summary>
        /// Логин-почта клиента.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Захэшированный пароль клиента.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Дата оформление заказа, с бракованным товаром.
        /// </summary>
        [JsonIgnore]
        public DateTime DateOfDefectOrder { get; set; }

        /// <summary>
        /// Оплаченная клиентом сумма.
        /// </summary>
        [JsonIgnore]
        public double PaidSum { get => Orders.Where(order => order.Status >= OrderStatus.Paid).Sum(order => order.OrderPrice); }

        /// <summary>
        /// ФИО клиента.
        /// </summary>
        public string FullName { get => $"{Surname} {Name} {Patrinymic}".Trim(); }

        /// <summary>
        /// Список заказов клиента.
        /// </summary>
        public List<Order> Orders = new List<Order>();

        /// <summary>
        /// Конструктор клиента.
        /// </summary>
        /// <param name="name"> Имя.</param>
        /// <param name="surname"> Фамилия. </param>
        /// <param name="patrinymic"> Отчество. </param>
        /// <param name="phoneNumber"> Номер телефона. </param>
        /// <param name="adress"> Адрес клиента. </param>
        /// <param name="login"> Логин-почта клиента. </param>
        /// <param name="password"> Пароль клиента. </param>
        public Client(string name, string surname, string patrinymic, ulong phoneNumber, string adress, string login, string password)
        {
            Name = name;
            Surname = surname;
            Patrinymic = patrinymic;
            PhoneNumber = phoneNumber;
            Adress = adress;
            Login = login;
            Hash = GetSHA256(Login + password);
        }

        /// <summary>
        /// Хэширование пароля.
        /// </summary>
        /// <param name="randomString"> Пароль + соль(логин клиента). </param>
        /// <returns></returns>
        public static string GetSHA256(string randomString)
        {
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2"); 
            }
            return hash;
        }
    }
}

# WPF. Механизм работы с заказами.
Разработанное приложение позволяет клиенту:
1. зарегистрироваться или авторизоваться, если он уже зарегистрирован;
2. выбрать товары для заказа (сформировать корзину) и оформить заказ (при оформлении заказу присваивается уникальный номер, а статус не содержит никаких значений);
3. просмотреть список оформленных заказов и их статусов;
4. “оплатить” заказ, добавив значение “оплачен” в статус заказа.

Разработанное приложение позволяет продавцу:
1. зарегистрироваться или авторизоваться, если он уже зарегистрирован;
2. Просматривать список клиентов (при выборе клиента есть возможность просмотра списка всех заказов клиента и отображения оплаченной суммы по всем заказам);
3. Просматривать список всех заказов;
4. Выбрать заказ и изменить его статус (добавить в статус заказа значения: “обработан”, “отгружен”, “исполнен”);
5. Просматривать список активных заказов (активным считается заказ, не имеющий статуса “исполнен”);
6. Создать отчет, позволяющий вывести клиентов, оплативших заказы на сумму, превышающую заданную продавцом + саму эту сумму. Клиенты упорядочены по убыванию потраченной на заказы суммы;
7. Создать отчет, который будет выводить список клиентов, заказавших заданный продавцом товар и дату оформления соответствующего заказа.

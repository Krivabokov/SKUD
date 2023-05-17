using SKUD;
using System;
using System.Collections.Generic;

namespace EmployeeRegistration
{
    class Program
    {
        static void Main(string[] args)
        {
            // Соединяемся с базой данных
            ADODB_DS db = new ADODB_DS();

            // Вводим данные для нового сотрудника
            Console.Write("Введите ФИО: ");
            string fio = Console.ReadLine();

            Console.Write("Введите ссылку на фото: ");
            string photo = Console.ReadLine();

            Console.Write("Введите RFID: ");
            string rfid = Console.ReadLine();

            // Получаем список отделов
            List<string> departments = db.GetDepartments();

            // Предлагаем пользователю выбрать отдел
            Console.WriteLine("Выберите отдел из списка:");
            for (int i = 0; i < departments.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {departments[i]}");
            }

            int departmentIndex = GetIndexFromUser(departments.Count);
            string selectedDepartment = departments[departmentIndex];

            // Получаем список должностей
            List<string> positions = db.GetPositions();

            // Предлагаем пользователю выбрать должность
            Console.WriteLine("Выберите должность из списка:");
            for (int i = 0; i < positions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {positions[i]}");
            }

            int positionIndex = GetIndexFromUser(positions.Count);
            string selectedPosition = positions[positionIndex];

            // Сохраняем данные в таблицы Person и Employee
            bool result1 = db.AddPerson(fio, photo, rfid);
            bool result2 = db.AddEmployee(fio, selectedDepartment, selectedPosition);

            if (result1 && result2)
            {
                Console.WriteLine("Новый сотрудник зарегистрирован.");
            }
            else
            {
                Console.WriteLine("Не удалось зарегистрировать нового сотрудника.");
            }

            Console.ReadLine();
        }

        static int GetIndexFromUser(int maxIndex)
        {
            int index;

            while (true)
            {
                Console.Write("Выберите номер из списка: ");

                if (int.TryParse(Console.ReadLine(), out index) && index >= 1 && index <= maxIndex)
                {
                    return index - 1;
                }

                Console.WriteLine("Некорректный ввод. Попробуйте снова.");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace knowledgetester
{
    public partial class authorization : Form
    {
        private static bool CheckLoginPassword(string login, string password)
        {
            string line;
            using (var sr = new StreamReader("users.txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == $"{login}:{password}") return true;
                }
            }
            return false;
        }

        public authorization()
        {
            InitializeComponent();
            form2 = new testing(); // привязка первой формы ко второй, чтобы они видели друг друга
        }

        testing form2; //привязали форму2 к первой форме


        private void button1_Click_1(object sender, EventArgs e)
        {
            if (File.Exists("users.txt") && File.Exists("File.txt")) 
            {
                string Log = textBox1.Text; // что пользователь ввёл в login
                string Pas = textBox2.Text; // что пользователь ввёл в password
                if (CheckLoginPassword(Log, Pas)) //проверка правильности ввода данных
                {
                    form2.Show(); //Показ второй формы(testing)
                    Hide(); //Скрытие текущей формы(authorization)
                }
                else
                {
                    MessageBox.Show("Не удаётся войти.\nПожалуйста, проверьте правильность написания логина и пароля.", "Ошибка.");
                }
            }
            else
            {
                MessageBox.Show("Отсутствует файл users.txt или файл File.txt. Проверьте наличие файлов.", "Ошибка");
                Environment.Exit(0);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Для корректной работы программы нужно:\n\n\nПоместить программу и " +
                "txt файлы(с названием <File.txt> и <users.txt>) в одну папку.\n\n\nПример оформления txt файла:\n\n\nА1. " +
                "Сам вопрос:\n1.вариант ответа\n2.вариант ответа\n3.вариант ответа\n4.вариант ответа\n" +
                "Ответ: номер ответа\n\nТак же помимо типа вопроса A доступны типы B (несколько вариантов ответа) и С (развернутый вариант ответа).", "INFO");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

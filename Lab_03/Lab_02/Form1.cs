using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Lab_02
{
    public partial class Form1 : Form
    {
        private int currentIndex = -1;

        public Form1()
        {
            InitializeComponent();
        }

        // КНОПКА "СОХРАНИТЬ"
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Student st = ReadStudentFromForm();
                StudentRepository.Add(st);

                currentIndex = StudentRepository.Students.Count - 1;
                UpdateObjectCounter();

                XmlHelper.SaveToXml(StudentRepository.Students, "students.xml");

                UpdateStatus("Сохранение");
                MessageBox.Show("Студент сохранён!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        // КНОПКА "ЗАГРУЗИТЬ"
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists("students.xml"))
                {
                    MessageBox.Show("Файл students.xml не найден");
                    return;
                }

                StudentRepository.Students =
                    XmlHelper.LoadFromXml<List<Student>>("students.xml");

                if (StudentRepository.Students.Count > 0)
                {
                    currentIndex = 0;
                    WriteStudentToForm(StudentRepository.Students[currentIndex]);
                }
                else
                {
                    currentIndex = -1;
                    ClearForm();
                }

                UpdateObjectCounter();
                UpdateStatus("Загрузка");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        // КНОПКА "БЮДЖЕТ"
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                double avg = double.Parse(textBox1.Text);
                int course = checkedListBox1.SelectedIndex + 1;
                int exp = int.Parse(textBox4.Text);

                double budget = avg * course * 1000 - exp * 500;

                UpdateStatus("Расчёт бюджета");
                MessageBox.Show($"Бюджет университета: {budget:F2} руб.");
            }
            catch
            {
                MessageBox.Show("Проверьте корректность введённых данных");
            }
        }

        // КНОПКА "СКРЫТЬ/ПОКАЗАТЬ ПАНЕЛЬ"
        private void button4_Click(object sender, EventArgs e)
        {
            toolStrip1.Visible = !toolStrip1.Visible;
            UpdateStatus(toolStrip1.Visible ? "Панель показана" : "Панель скрыта");
        }
        // ЧТЕНИЕ ДАННЫХ ИЗ ФОРМЫ
        private Student ReadStudentFromForm()
        {
            Student st = new Student
            {
                FullName = maskedTextBox1.Text,
                BirthDate = dateTimePicker1.Value,
                Age = int.Parse(comboBox1.Text),
                Course = checkedListBox1.SelectedIndex + 1,
                Group = numericUpDown1.Value.ToString(),
                AverageScore = double.Parse(textBox1.Text),
                Gender = radioButton1.Checked ? "М" : "Ж",

                Work = new WorkPlace
                {
                    Company = textBox2.Text,
                    Position = textBox3.Text,
                    Experience = int.Parse(textBox4.Text)
                },

                Address = new Address
                {
                    City = textBox7.Text,
                    Street = textBox6.Text,
                    House = textBox5.Text,
                    Flat = textBox8.Text
                }
            };

            ValidateStudent(st);
            return st;
        }

        // ЗАПИСЬ ДАННЫХ В ФОРМУ
        private void WriteStudentToForm(Student st)
        {
            maskedTextBox1.Text = st.FullName;
            dateTimePicker1.Value = st.BirthDate;
            comboBox1.Text = st.Age.ToString();
            checkedListBox1.SelectedIndex = st.Course - 1;
            numericUpDown1.Value = decimal.Parse(st.Group);
            textBox1.Text = st.AverageScore.ToString();

            if (st.Gender == "М") radioButton1.Checked = true;
            else radioButton2.Checked = true;

            textBox2.Text = st.Work.Company;
            textBox3.Text = st.Work.Position;
            textBox4.Text = st.Work.Experience.ToString();

            textBox7.Text = st.Address.City;
            textBox6.Text = st.Address.Street;
            textBox5.Text = st.Address.House;
            textBox8.Text = st.Address.Flat;
        }

        // ВАЛИДАЦИЯ СТУДЕНТА
        private void ValidateStudent(Student st)
        {
            var context = new ValidationContext(st);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(st, context, results, true))
            {
                string errors = string.Join("\n", results.Select(r => r.ErrorMessage));
                throw new Exception(errors);
            }
        }

        // ОБНОВЛЕНИЕ СТРОКИ СОСТОЯНИЯ
        private void UpdateStatus(string action)
        {
            toolStripStatusLabel1.Text = "Действие: " + action;
            toolStripStatusLabel2.Text = "Время: " + DateTime.Now.ToLongTimeString();
        }

        // ОБНОВЛЕНИЕ СЧЁТЧИКА ОБЪЕКТОВ
        private void UpdateObjectCounter()
        {
            if (StudentRepository.Students.Count == 0 || currentIndex < 0)
                toolStripStatusLabel3.Text = "Объектов: 0";
            else
                toolStripStatusLabel3.Text =
                    $"Объект {currentIndex + 1} из {StudentRepository.Students.Count}";
        }

        // РАЗРЕШИТЬ ТОЛЬКО ЦИФРЫ
        private void OnlyDigits_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        // ВАЛИДАЦИЯ СРЕДНЕГО БАЛЛА
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
          
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {        }
        private List<Student> lastSearchResult;

        // ПОИСК: ПОЛНОЕ СОВПАДЕНИЕ
        private void полноеСовпадениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string query = Interaction.InputBox("Введите ФИО:", "Поиск");
            var result = StudentRepository.Students
                .Where(s => s.FullName.Equals(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

            ShowSearchResult(result);
            UpdateStatus("Поиск: полное совпадение");
        }

        // ПОИСК: РЕГУЛЯРНОЕ ВЫРАЖЕНИЕ
        private void регулярноеВыражениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pattern = Interaction.InputBox("Введите регулярное выражение:", "Поиск");
            var result = StudentRepository.Students
                .Where(s => Regex.IsMatch(s.FullName, pattern))
                .ToList();

            ShowSearchResult(result);
            UpdateStatus("Поиск: регулярное выражение");
        }

        // ПОИСК: КОНСТРУКТОР ЗАПРОСОВ
        private void конструкторЗапросовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string city = Interaction.InputBox("Введите город:", "Поиск");
            var result = StudentRepository.Students
                .Where(s => s.Address.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                .ToList();

            ShowSearchResult(result);
            UpdateStatus("Поиск: по городу");
        }

        // СОРТИРОВКА ПО ФИО
        private void поФИОToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StudentRepository.Students = StudentRepository.Students
                .OrderBy(s => s.FullName)
                .ToList();

            if (StudentRepository.Students.Count > 0)
            {
                currentIndex = 0;
                WriteStudentToForm(StudentRepository.Students[currentIndex]);
            }

            UpdateObjectCounter();
            UpdateStatus("Сортировка по ФИО");
        }

        // СОРТИРОВКА ПО СРЕДНЕМУ БАЛЛУ
        private void среднемуБаллуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StudentRepository.Students = StudentRepository.Students
                .OrderByDescending(s => s.AverageScore)
                .ToList();

            if (StudentRepository.Students.Count > 0)
            {
                currentIndex = 0;
                WriteStudentToForm(StudentRepository.Students[currentIndex]);
            }

            UpdateObjectCounter();
            UpdateStatus("Сортировка по среднему баллу");
        }

        // СОХРАНЕНИЕ РЕЗУЛЬТАТОВ ПОИСКА
        private void результатыПоискаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lastSearchResult == null || lastSearchResult.Count == 0)
            {
                MessageBox.Show("Нет данных для сохранения");
                return;
            }

            XmlHelper.SaveToXml(lastSearchResult, "search.xml");
            UpdateStatus("Сохранение результатов поиска");
            MessageBox.Show("Результаты поиска сохранены");
        }

        // СОХРАНЕНИЕ СОРТИРОВКИ
        private void сортировкиВXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlHelper.SaveToXml(StudentRepository.Students, "sorted.xml");
            UpdateStatus("Сохранение сортировки");
            MessageBox.Show("Сортировка сохранена");
        }

        // О ПРОГРАММЕ
        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Лабораторная работа №3\nВерсия 1.0\nАвтор: Mikhail");
        }

        // ПОКАЗ РЕЗУЛЬТАТОВ ПОИСКА
        private void ShowSearchResult(List<Student> list)
        {
            lastSearchResult = list;

            if (list.Count == 0)
            {
                MessageBox.Show("Ничего не найдено");
                return;
            }

            string msg = string.Join("\n", list.Select(s => s.FullName));
            MessageBox.Show(msg, "Результаты поиска");
        }

        // ОЧИСТКА ФОРМЫ
        private void ClearForm()
        {
            maskedTextBox1.Clear();
            comboBox1.Text = "";
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();

            radioButton1.Checked = false;
            radioButton2.Checked = false;

            checkedListBox1.ClearSelected();
            numericUpDown1.Value = 1;
        }
        public class AverageScoreAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value == null)
                    return new ValidationResult("Средний балл обязателен");

                if (!double.TryParse(value.ToString(), out double score))
                    return new ValidationResult("Средний балл должен быть числом");

                if (score < 0 || score > 10)
                    return new ValidationResult("Средний балл должен быть от 0 до 10");

                return ValidationResult.Success;
            }
        }

        // КЛАСС АДРЕС
        public class Address
        {
            [Required(ErrorMessage = "Город обязателен")]
            public string City { get; set; }

            [Required(ErrorMessage = "Улица обязательна")]
            public string Street { get; set; }

            [Required(ErrorMessage = "Дом обязателен")]
            public string House { get; set; }

            [Required(ErrorMessage = "Квартира обязательна")]
            public string Flat { get; set; }
        }

        // КЛАСС МЕСТО РАБОТЫ
        public class WorkPlace
        {
            [Required(ErrorMessage = "Компания обязательна")]
            public string Company { get; set; }

            [Required(ErrorMessage = "Должность обязательна")]
            public string Position { get; set; }

            [Range(0, 100, ErrorMessage = "Стаж должен быть от 0 до 100")]
            public int Experience { get; set; }
        }

        // КЛАСС СТУДЕНТ
        public class Student
        {
            [Required(ErrorMessage = "ФИО обязательно")]
            public string FullName { get; set; }

            public DateTime BirthDate { get; set; }

            [Range(1, 120, ErrorMessage = "Возраст должен быть от 1 до 120")]
            public int Age { get; set; }

            [Range(1, 4, ErrorMessage = "Курс должен быть от 1 до 4")]
            public int Course { get; set; }

            [Required(ErrorMessage = "Группа обязательна")]
            public string Group { get; set; }

            [AverageScore]
            public double AverageScore { get; set; }


            [Required(ErrorMessage = "Пол обязателен")]
            public string Gender { get; set; }

            public WorkPlace Work { get; set; }
            public Address Address { get; set; }
        }
        // РЕПОЗИТОРИЙ СТУДЕНТОВ
        public static class StudentRepository
        {
            public static List<Student> Students { get; set; } = new List<Student>();

            public static void Add(Student st)
            {
                Students.Add(st);
            }
        }

        // XML
        public static class XmlHelper
        {
            // СОХРАНЕНИЕ В XML
            public static void SaveToXml<T>(T obj, string fileName)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    serializer.Serialize(fs, obj);
                }
            }

            // ЗАГРУЗКА ИЗ XML
            public static T LoadFromXml<T>(string fileName)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    return (T)serializer.Deserialize(fs);
                }
            }
        }
        // КНОПКА "ОЧИСТИТЬ" 
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ClearForm();
            currentIndex = -1;
            UpdateObjectCounter();
            UpdateStatus("Очистка формы");
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            toolStripButton1_Click(sender, e);
        }

        // КНОПКА "УДАЛИТЬ" 
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (StudentRepository.Students.Count == 0)
            {
                MessageBox.Show("Нет объектов для удаления");
                return;
            }

            StudentRepository.Students.RemoveAt(currentIndex);

            if (StudentRepository.Students.Count == 0)
            {
                ClearForm();
                currentIndex = -1;
                UpdateObjectCounter();
                UpdateStatus("Удаление");
                return;
            }

            if (currentIndex >= StudentRepository.Students.Count)
                currentIndex = StudentRepository.Students.Count - 1;

            WriteStudentToForm(StudentRepository.Students[currentIndex]);
            UpdateObjectCounter();
            UpdateStatus("Удаление");
        }

        // КНОПКА "ВПЕРЁД" 
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (StudentRepository.Students.Count == 0)
                return;

            currentIndex++;
            if (currentIndex >= StudentRepository.Students.Count)
                currentIndex = 0;

            WriteStudentToForm(StudentRepository.Students[currentIndex]);
            UpdateObjectCounter();
            UpdateStatus("Вперёд");
        }

        // КНОПКА "НАЗАД" 
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (StudentRepository.Students.Count == 0)
                return;

            currentIndex--;
            if (currentIndex < 0)
                currentIndex = StudentRepository.Students.Count - 1;

            WriteStudentToForm(StudentRepository.Students[currentIndex]);
            UpdateObjectCounter();
            UpdateStatus("Назад");
        }


        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e) { }
        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void groupBox2_Enter(object sender, EventArgs e) { }
        private void textBox8_TextChanged(object sender, EventArgs e) { }
        private void textBox5_TextChanged(object sender, EventArgs e) { }
        private void textBox6_TextChanged(object sender, EventArgs e) { }
        private void textBox7_TextChanged(object sender, EventArgs e) { }

        private void поискToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void сортировкаToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e) { }
        private void toolStripDropDownButton2_Click(object sender, EventArgs e) { }
        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }
        private void toolStripStatusLabel1_Click(object sender, EventArgs e) { }
        private void toolStripStatusLabel2_Click(object sender, EventArgs e) { }
        private void toolStripStatusLabel3_Click(object sender, EventArgs e) { }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) { }
        private void radioButton2_CheckedChanged(object sender, EventArgs e) { }

        private void Form1_Load(object sender, EventArgs e) { }
        private void полноеСовпвдениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            полноеСовпадениеToolStripMenuItem_Click(sender, e);
        }

        private void регулярноеВыражениеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            регулярноеВыражениеToolStripMenuItem_Click(sender, e);
        }

        private void конструкторЗапросовToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            конструкторЗапросовToolStripMenuItem_Click(sender, e);
        }

        private void поФИОToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            поФИОToolStripMenuItem_Click(sender, e);
        }

        private void поСреднемуБаллуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            среднемуБаллуToolStripMenuItem_Click(sender, e);
        }
        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            toolStripButton2_Click(sender, e);
        }
    }
}

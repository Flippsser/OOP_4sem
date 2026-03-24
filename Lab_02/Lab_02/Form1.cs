using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Lab_02
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            textBox1.KeyPress += textBox1_KeyPress;
            textBox1.TextChanged += textBox1_TextChanged;

            textBox5.KeyPress += OnlyDigits_KeyPress;
            textBox8.KeyPress += OnlyDigits_KeyPress;
            textBox4.KeyPress += OnlyDigits_KeyPress;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Student st = ReadStudentFromForm();
                XmlHelper.SaveToXml(st, "student.xml");
                MessageBox.Show("Данные успешно сохранены!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists("student.xml"))
                {
                    MessageBox.Show("Файл student.xml не найден");
                    return;
                }

                Student st = XmlHelper.LoadFromXml<Student>("student.xml");
                WriteStudentToForm(st);
                MessageBox.Show("Данные успешно загружены!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                double avg = double.Parse(textBox1.Text);
                int course = checkedListBox1.SelectedIndex + 1;
                int exp = int.Parse(textBox4.Text);

                double budget = avg * course * 1000 - exp * 500;

                MessageBox.Show($"Бюджет университета: {budget:F2} руб.");
            }
            catch
            {
                MessageBox.Show("Проверьте корректность введённых данных");
            }
        }

        private Student ReadStudentFromForm()
        {
            if (!maskedTextBox1.MaskFull)
                throw new Exception("Поле ФИО заполнено некорректно");

            if (!int.TryParse(comboBox1.Text, out int age))
                throw new Exception("Некорректный возраст");

            if (!double.TryParse(textBox1.Text, out double avg))
                throw new Exception("Некорректный средний балл");

            if (!int.TryParse(textBox4.Text, out int exp))
                throw new Exception("Некорректный стаж");

            string gender = radioButton1.Checked ? "М" :
                            radioButton2.Checked ? "Ж" : "";

            if (gender == "")
                throw new Exception("Не выбран пол");

            Student st = new Student
            {
                FullName = maskedTextBox1.Text,
                BirthDate = dateTimePicker1.Value,
                Age = age,
                Specialty = listBox1.Text,
                Course = checkedListBox1.SelectedIndex + 1,
                Group = numericUpDown1.Value.ToString(),
                AverageScore = avg,
                Gender = gender,

                WorkPlace = new WorkPlace
                {
                    Company = textBox2.Text,
                    Position = textBox3.Text,
                    Experience = exp
                },

                Address = new Address
                {
                    City = textBox7.Text,
                    Street = textBox6.Text,
                    House = textBox5.Text,
                    Apartment = textBox8.Text
                }
            };

            return st;
        }

        private void WriteStudentToForm(Student st)
        {
            maskedTextBox1.Text = st.FullName;
            dateTimePicker1.Value = st.BirthDate;
            comboBox1.Text = st.Age.ToString();
            listBox1.Text = st.Specialty;
            checkedListBox1.SelectedIndex = st.Course - 1;
            numericUpDown1.Value = int.Parse(st.Group);
            textBox1.Text = st.AverageScore.ToString();

            if (st.Gender == "М") radioButton1.Checked = true;
            else radioButton2.Checked = true;

            textBox2.Text = st.WorkPlace.Company;
            textBox3.Text = st.WorkPlace.Position;
            textBox4.Text = st.WorkPlace.Experience.ToString();

            textBox7.Text = st.Address.City;
            textBox6.Text = st.Address.Street;
            textBox5.Text = st.Address.House;
            textBox8.Text = st.Address.Apartment;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(textBox1.Text, out double value))
            {
                if (value > 10)
                {
                    MessageBox.Show("Средний балл не может быть больше 10");
                    textBox1.Text = "10";
                }
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (i != checkedListBox1.SelectedIndex)
                    checkedListBox1.SetItemChecked(i, false);
            }
        }

        private void OnlyDigits_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void textBox7_TextChanged(object sender, EventArgs e) { }
        private void textBox6_TextChanged(object sender, EventArgs e) { }
        private void textBox5_TextChanged(object sender, EventArgs e) { }
        private void textBox8_TextChanged(object sender, EventArgs e) { }
        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void groupBox2_Enter(object sender, EventArgs e) { }
        private void radioButton1_CheckedChanged(object sender, EventArgs e) { }
        private void radioButton2_CheckedChanged(object sender, EventArgs e) { }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e) { }
    }

    [Serializable]
    public class Address
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Apartment { get; set; }
    }

    [Serializable]
    public class WorkPlace
    {
        public string Company { get; set; }
        public string Position { get; set; }
        public int Experience { get; set; }
    }

    [Serializable]
    public class Student
    {
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public string Specialty { get; set; }
        public int Course { get; set; }
        public string Group { get; set; }
        public double AverageScore { get; set; }
        public string Gender { get; set; }

        public Address Address { get; set; }
        public WorkPlace WorkPlace { get; set; }
    }

    public static class XmlHelper
    {
        public static void SaveToXml<T>(T obj, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                serializer.Serialize(fs, obj);
            }
        }

        public static T LoadFromXml<T>(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                return (T)serializer.Deserialize(fs);
            }
        }
    }
}

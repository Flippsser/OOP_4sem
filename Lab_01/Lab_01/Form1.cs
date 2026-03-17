using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lab_01
{
    public partial class Form1 : Form
    {
        private readonly Calculator _calculator;
        private readonly CheckBox[] _inBoxes;
        private readonly CheckBox[] _outBoxes;
        private readonly string[] _units = { "метры", "футы", "килограммы", "фунты", "литры", "галлоны" };

        public Form1()
        {
            InitializeComponent();

            _calculator = new Calculator();
            _calculator.ConversionCompleted += (result, from, to) => textBox2.Text = $"{result:F4}";
            _calculator.ConversionError += msg => MessageBox.Show(msg, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

            _inBoxes = new[] { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6 };
            _outBoxes = new[] { checkBox12, checkBox11, checkBox10, checkBox9, checkBox8, checkBox7 };
            checkBox1.Checked = true;
            checkBox11.Checked = true;
        }

        // Общий обработчик для всех чекбоксов
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox current && current.Checked)
            {
                // Определяем группу (In или Out)
                CheckBox[] group = Array.IndexOf(_inBoxes, current) >= 0 ? _inBoxes : _outBoxes;
                foreach (var cb in group)
                    if (cb != current) cb.Checked = false;
            }
        }

        // Индивидуальные обработчики для дизайнера
        private void checkBox1_CheckedChanged(object sender, EventArgs e) => CheckBox_CheckedChanged(sender, e);
        private void checkBox2_CheckedChanged(object sender, EventArgs e) => CheckBox_CheckedChanged(sender, e);
        private void checkBox3_CheckedChanged(object sender, EventArgs e) => CheckBox_CheckedChanged(sender, e);
        private void checkBox4_CheckedChanged(object sender, EventArgs e) => CheckBox_CheckedChanged(sender, e);
        private void checkBox5_CheckedChanged(object sender, EventArgs e) => CheckBox_CheckedChanged(sender, e);
        private void checkBox6_CheckedChanged(object sender, EventArgs e) => CheckBox_CheckedChanged(sender, e);
        private void checkBox7_CheckedChanged(object sender, EventArgs e) => CheckBox_CheckedChanged(sender, e);
        private void checkBox8_CheckedChanged(object sender, EventArgs e) => CheckBox_CheckedChanged(sender, e);
        private void checkBox9_CheckedChanged(object sender, EventArgs e) => CheckBox_CheckedChanged(sender, e);
        private void checkBox10_CheckedChanged(object sender, EventArgs e) => CheckBox_CheckedChanged(sender, e);
        private void checkBox11_CheckedChanged(object sender, EventArgs e) => CheckBox_CheckedChanged(sender, e);
        private void checkBox12_CheckedChanged(object sender, EventArgs e) => CheckBox_CheckedChanged(sender, e);

        // Обработчик кнопки
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string from = GetSelected(_inBoxes);
                string to = GetSelected(_outBoxes);

                if (from == null) throw new InvalidOperationException("Выберите единицу в группе In.");
                if (to == null) throw new InvalidOperationException("Выберите единицу в группе Out.");

                if (!double.TryParse(textBox1.Text, out double value) || value < 0)
                    throw new FormatException("Введите корректное число.");

                _calculator.Convert(from, to, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Получение выбранной единицы по индексу в массиве
        private string GetSelected(CheckBox[] boxes)
        {
            for (int i = 0; i < boxes.Length; i++)
                if (boxes[i].Checked) return _units[i];
            return null;
        }

        // Пустые обработчики для дизайнера
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
    }

    // Делегат и класс калькулятора
    public delegate double ConversionDelegate(double value);

    public class Calculator
    {
        public event Action<double, string, string> ConversionCompleted;
        public event Action<string> ConversionError;

        private readonly Dictionary<(string from, string to), ConversionDelegate> _converters;

        public Calculator()
        {
            _converters = new Dictionary<(string, string), ConversionDelegate>
            {
                { ("метры", "футы"), m => m * 3.28084 },
                { ("футы", "метры"), f => f / 3.28084 },
                { ("килограммы", "фунты"), kg => kg * 2.20462 },
                { ("фунты", "килограммы"), lb => lb / 2.20462 },
                { ("литры", "галлоны"), l => l * 0.264172 },
                { ("галлоны", "литры"), gal => gal / 0.264172 }
            };
        }

        public void Convert(string fromUnit, string toUnit, double value)
        {
            try
            {
                if (_converters.TryGetValue((fromUnit, toUnit), out ConversionDelegate converter))
                {
                    double result = converter(value);
                    ConversionCompleted?.Invoke(result, fromUnit, toUnit);
                }
                else
                {
                    ConversionError?.Invoke($"Конвертация из '{fromUnit}' в '{toUnit}' не поддерживается.");
                }
            }
            catch (Exception ex)
            {
                ConversionError?.Invoke($"Ошибка при конвертации: {ex.Message}");
            }
        }
    }
}
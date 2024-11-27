using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Sort;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Task2.xaml
    /// </summary>
    public partial class Task2 : Window
    {
        public Task2()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TbInput.Text == "Введите по какому столбцу будет сортировка")
            {
                TbInput.Text = string.Empty;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TbInput.Text))
            {
                TbInput.Text = "Введите по какому столбцу будет сортировка";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedItemFile = (ComboBoxItem)CmbFile.SelectedItem;
            ComboBoxItem selectedItemSort = (ComboBoxItem)CmbSort.SelectedItem;


            string? selectedFile = selectedItemFile.Content.ToString();
            string? selectedSort = selectedItemSort.Content.ToString();

            string pathFile = "";

            switch (selectedFile)
            {
                case "Countries":
                    pathFile = "Tabels\\ReadyTabels\\Countries.csv";
                    break;
                case "Chemical elements":
                    pathFile = "Tabels\\ReadyTabels\\Chemical elements.csv";
                    break;
                case "Custom":
                    pathFile = "Tabels\\ReadyTabels\\Custom.csv";
                    break;

            }

            if (int.TryParse(TbInput.Text, out int input))
            {
                switch (selectedSort)
                {
                    case "Прямое слияние":
                        DirectOuterSort directOuterSort = new DirectOuterSort(input, pathFile);
                        directOuterSort.Sort();
                        TbOutput.Text = String.Join('\r',directOuterSort._result);
                        break;
                    case "Естественное слияние":
                        NaturalOuterSort natural = new NaturalOuterSort(input,pathFile);
                        natural.Sort();
                        TbOutput.Text = string.Join('\r', natural._result);
                        break;
                    case "Многопутевое слияние":
                        break;
                }
            }
            else
            {
                MessageBox.Show("Введите число по какому столбцу будет сортировка");
            }
  
        }
    }
}

using Giny.IO.D2I;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Giny.D2I
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ID2IEntry CurrentEntry
        {
            get;
            set;
        }
        private D2IFile D2IFile
        {
            get;
            set;
        }
        public MainWindow()
        {
            InitializeComponent();
            data.CurrentCellChanged += Data_CurrentCellChanged;
            canvas.Visibility = Visibility.Hidden;
        }

        public void RestrictColumnsSize()
        {
            foreach (var column in data.Columns)
            {
                column.MinWidth = column.ActualWidth;
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
        }

        private void Data_CurrentCellChanged(object sender, EventArgs e)
        {
            if (data.CurrentItem != null)
            {
                var value = (ID2IEntry)data.CurrentItem;
              
                CurrentEntry = value;
                textBox.Text = value.GetText();
            }
        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "D2I files (*.d2i)|*.d2i";

            if (dialog.ShowDialog().Value)
            {
                D2IFile = new D2IFile(dialog.FileName);
                DisplayD2IItems();
            }


        }


        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (D2IFile != null)
            {
                try
                {
                    D2IFile.Save();
                    MessageBox.Show("File saved sucessfully !", "Informations", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }

                catch
                {
                    MessageBox.Show("Unable to save file. Please very the file is not used by another process", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveAsClick(object sender, RoutedEventArgs e)
        {

        }

        private void DisplayD2IItems()
        {
            bool uiText = checkbox.IsChecked.Value;

            if (uiText)
            {
                data.ItemsSource = D2IFile.GetAllUiText().Where(x => x.Key.ToLower().Contains(search.Text.ToLower()) || (x.UseUndiactricalText ? x.UnDiactricialText.ToLower() : x.Text.ToLower()).Contains(search.Text.ToLower()));
            }
            else
            {
                data.ItemsSource = D2IFile.GetAllText().Where(x => x.Key.ToString().Contains(search.Text.ToLower()) || (x.UseUndiactricalText ? x.UnDiactricialText.ToLower() : x.Text.ToLower()).Contains(search.Text.ToLower()));
            }

            RestrictColumnsSize();

            canvas.Visibility = Visibility.Visible;

        }


        private void DataGridCell_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (data.CurrentItem != null)
            {
                var value = (KeyValuePair<int, string>)data.CurrentItem;
                textBox.Text = value.Value;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DisplayD2IItems();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string content = textBox.Text;

            if (CurrentEntry is D2IEntry<int>)
            {
                D2IFile.SetText(((D2IEntry<int>)CurrentEntry).Key, ValidateContent(content));
            }
            else if (CurrentEntry is D2IEntry<string>)
            {
                D2IFile.SetText(((D2IEntry<string>)CurrentEntry).Key, ValidateContent(content));
            }
            DisplayD2IItems();
        }
        string ValidateContent(string content)
        {
            return content.Replace("\r", "").Replace("\n", "");

        }
        string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                rtb.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                rtb.Document.ContentEnd
            );

            // The Text property on a TextRange object returns a string
            // representing the plain text content of the TextRange.
            return textRange.Text;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            DisplayD2IItems();
        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayD2IItems();
        }
    }

}

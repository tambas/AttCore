using Giny.ORM;
using Giny.World.Managers.Effects;
using Giny.World.Records.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Giny.SpellExplorer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            contentControl.Content = new Loading(this);

        }

        
        private SpellRecord GetSelectedSpell()
        {
            return (SpellRecord)spellSelector.SelectedItem;
        }
        private void UpdateSpellList()
        {
            IEnumerable<SpellRecord> searchResult = SpellRecord.GetSpellRecords().Where(x => x.ToString().ToLower().Contains(textBox.Text.ToLower()));

            spellSelector.Items.Clear();

            foreach (var result in searchResult)
            {
                spellSelector.Items.Add(result);
            }

        }
        public void OnLoadingEnd()
        {
            contentControl.Content = string.Empty;
            UpdateSpellList();
            textBox.TextChanged += TextBox_TextChanged;
            unhandledBtn.Visibility = Visibility.Visible;
            triggersBtn.Visibility = Visibility.Visible;
            spellSelector.SelectionChanged += SpellSelector_SelectionChanged;
        }
        private void SpellSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (spellSelector.SelectedItem != null)
            {
                contentControl.Content = new SpellView((SpellRecord)spellSelector.SelectedItem);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSpellList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EffectsRelator view = new EffectsRelator();
            view.Show();
        }

        private void triggersBtn_Click(object sender, RoutedEventArgs e)
        {
            TriggersRelator view = new TriggersRelator();
            view.Show();
        }
    }
}

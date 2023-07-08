using Giny.IO.D2O;
using Giny.IO.D2OClasses;
using Giny.ORM;
using Giny.Protocol.Enums;
using Giny.World.Managers.Effects;
using Giny.World.Records.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Giny.Items
{
    /// <summary>
    /// Logique d'interaction pour Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        private ItemRecord CurrentItem => (ItemRecord)itemList.SelectedItem;

        private EffectDice CurrentEffect => effects.SelectedItem as EffectDice;

        public Editor()
        {
            InitializeComponent();
            DisplayItems();
            DisplayNewEffects();

        }

        private void DisplayNewEffects()
        {
            newEffect.Items.Clear();

            var values = Enum.GetValues(typeof(EffectsEnum)).OfType<object>().Where(x => x.ToString().ToLower().Contains(newEffectSearch.Text.ToLower()));

            foreach (var value in values)
            {
                newEffect.Items.Add(value);
            }
        }
        private void DisplayItems()
        {
            string searchText = search.Text;

            itemList.Items.Clear();

            foreach (var item in ItemRecord.GetItems().Where(x => x.Name.ToLower().Contains(searchText.ToLower()) || x.Id.ToString() == searchText))
            {
                itemList.Items.Add(item);
            }
        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayItems();
        }

        private void itemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayCurrentItem();
        }
        private void DisplayCurrentItem()
        {
            if (CurrentItem == null)
            {
                return;
            }

            id.Content = CurrentItem.Id;
            name.Text = CurrentItem.Name;
            level.Content = CurrentItem.Level;
            price.Text = CurrentItem.Price.ToString();
            type.Content = CurrentItem.TypeEnum;

            effects.Items.Clear();

            foreach (var effect in CurrentItem.Effects)
            {
                effects.Items.Add(effect);
            }
        }

        private void effects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentEffect != null)
            {
                min.Text = CurrentEffect.Min.ToString();
                max.Text = CurrentEffect.Max.ToString();
                value.Text = CurrentEffect.Value.ToString();

                if (CurrentEffect.EffectEnum == EffectsEnum.Effect_CastSpell_1175)
                {
                    var id = CurrentEffect.Min;

                    if (D2OManager.ObjectExists("Spells.d2o", id))
                    {
                        var spell = D2OManager.GetObject<Spell>("Spells.d2o", id);
                        textId.Text = Loader.D2IFile.GetText((int)spell.DescriptionId);
                    }

                }
                else
                {
                    textId.Text = string.Empty;
                }
            }
        }

        private void newEffectSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayNewEffects();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CurrentItem.Effects.Clear();
            DisplayCurrentItem();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void price_LostFocus(object sender, RoutedEventArgs e)
        {
            CurrentItem.Price = double.Parse(price.Text);
            UpdateItem();
        }

        private void UpdateItem()
        {
            if (WeaponRecord.GetWeapon(CurrentItem.Id) != null)
            {
                var weaponRecord = WeaponRecord.GetWeapon(CurrentItem.Id);
                weaponRecord.Effects = CurrentItem.Effects;
                weaponRecord.Price = CurrentItem.Price;
                weaponRecord.Name = CurrentItem.Name;
                weaponRecord.UpdateInstantElement();
            }
            else
            {
                CurrentItem.UpdateInstantElement();
            }
        }
        private void name_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Item item = D2OManager.GetObject<Item>("Items.d2o", (int)CurrentItem.Id);
                Loader.D2IFile.SetText((int)item.NameId, name.Text);
                Loader.D2IFile.Save();
                CurrentItem.Name = name.Text;
                UpdateItem();
            }
            catch
            {
                MessageBox.Show("Unable to update item name. Please d2i file might not be available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void RemoveEffectClick(object sender, RoutedEventArgs e)
        {
            CurrentItem.Effects.Remove(CurrentEffect);
            UpdateItem();

            DisplayCurrentItem();
        }

        private void min_LostFocus(object sender, RoutedEventArgs e)
        {
            if (min.Text != string.Empty && CurrentEffect != null)
            {
                CurrentEffect.Min = int.Parse(min.Text);
                UpdateItem();
            }
        }

        private void max_LostFocus(object sender, RoutedEventArgs e)
        {
            if (max.Text != string.Empty && CurrentEffect != null)
            {
                CurrentEffect.Max = int.Parse(max.Text);
                UpdateItem();
            }
        }

        private void value_LostFocus(object sender, RoutedEventArgs e)
        {
            if (value.Text != string.Empty && CurrentEffect != null)
            {
                CurrentEffect.Value = int.Parse(value.Text);
                UpdateItem();
            }
        }

        private void AddEffectClick(object sender, RoutedEventArgs e)
        {
            EffectDice effect = new EffectDice((EffectsEnum)newEffect.SelectedItem, int.Parse(newMin.Text),
                int.Parse(newMax.Text), int.Parse(newValue.Text));

            CurrentItem.Effects.Add(effect);

            UpdateItem();
            DisplayCurrentItem();
        }


        private void MoveUpClick(object sender, RoutedEventArgs e)
        {
            var indice = effects.SelectedIndex;

            if (indice - 1 < 0)
            {
                return;
            }
            var current = CurrentItem.Effects[indice];
            var next = CurrentItem.Effects[indice - 1];

            CurrentItem.Effects[indice] = next;
            CurrentItem.Effects[indice - 1] = current;

            DisplayCurrentItem();

            UpdateItem();

            effects.SelectedIndex = indice - 1;
        }

        private void MoveDownClick(object sender, RoutedEventArgs e)
        {
            var indice = effects.SelectedIndex;

            if (indice + 1 >= CurrentItem.Effects.Count())
            {
                return;
            }
            var current = CurrentItem.Effects[indice];
            var next = CurrentItem.Effects[indice + 1];

            CurrentItem.Effects[indice] = next;
            CurrentItem.Effects[indice + 1] = current;

            DisplayCurrentItem();

            UpdateItem();

            effects.SelectedIndex = indice + 1;
        }

        private void textId_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentEffect != null && CurrentEffect.EffectEnum == EffectsEnum.Effect_CastSpell_1175)
            {
                var id = CurrentEffect.Min;

                if (D2OManager.ObjectExists("Spells.d2o", id))
                {
                    var spell = D2OManager.GetObject<Spell>("Spells.d2o", id);
                    Loader.D2IFile.SetText((int)spell.DescriptionId, textId.Text);

                    try
                    {
                        Loader.D2IFile.Save();
                    }
                    catch
                    {
                        MessageBox.Show("Unable to update item name. Please d2i file might not be available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
        }

    }
}
}

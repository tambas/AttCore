using Giny.Protocol.Enums;
using Giny.World.Managers.Fights.Effects;
using Giny.World.Records.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Giny.SpellExplorer
{
    /// <summary>
    /// Logique d'interaction pour NotHandledEffects.xaml
    /// </summary>
    public partial class EffectsRelator : Window
    {
        Dictionary<EffectsEnum, List<SpellRecord>> Effects = new Dictionary<EffectsEnum, List<SpellRecord>>();

        private CancellationTokenSource CancelSource
        {
            get;
            set;
        }

        private Action OnCancel;

        private bool loading;

        public EffectsRelator()
        {
            InitializeComponent();
            effects.SelectionChanged += Effects_SelectionChanged;
            Load();
        }


        protected override void OnClosed(EventArgs e)
        {
            CancelSource.Cancel();
            base.OnClosed(e);
        }
        private void InternalLoad()
        {
            CancelSource = new CancellationTokenSource();

            bool unhandledOnly = false;

            string searchStr = "";

            Task.Run(() =>
            {
                loading = true;

                this.Dispatcher.Invoke(() =>
                {
                    spells.Items.Clear();
                    effects.Items.Clear();
                    Effects.Clear();
                    searchStr = search.Text;
                    unhandledOnly = unhandleds.IsChecked.Value;
                    count.Content = string.Empty;
                });

                var spellRecords = SpellRecord.GetSpellRecords();
                int i = 0;
                int n = spellRecords.Count();

                foreach (var spell in spellRecords)
                {
                    foreach (var level in spell.Levels)
                    {
                        foreach (var effect in level.Effects)
                        {
                            if (SpellEffectManager.Instance.Exists(effect.EffectEnum) && unhandledOnly)
                            {
                                continue;
                            }

                            if (searchStr == string.Empty || effect.EffectEnum.ToString().ToLower().Contains(searchStr.ToLower()))
                            {
                                if (!Effects.ContainsKey(effect.EffectEnum))
                                {
                                    Effects.Add(effect.EffectEnum, new List<SpellRecord>());

                                    this.Dispatcher.Invoke(() =>
                                    {
                                        effects.Items.Add(effect.EffectEnum.ToString());
                                        count.Content = "Count : " + Effects.Count;
                                    });
                                }

                                if (!Effects[effect.EffectEnum].Contains(spell))
                                    Effects[effect.EffectEnum].Add(spell);

                                this.Dispatcher.Invoke(() =>
                                {
                                    if (effects.SelectedItem != null && effects.SelectedValue.ToString() == effect.EffectEnum.ToString())
                                    {
                                        spells.Items.Add(spell);
                                    }
                                });
                            }

                        }
                    }
                    i++;

                    if (CancelSource.IsCancellationRequested)
                    {
                        OnCancel?.Invoke();

                        break;
                    }
                    this.Dispatcher.Invoke(() =>
                    {
                        progress.Value = (i / (double)n) * 100d;
                    });

                }

                loading = false;
            });
        }

        private void DisplaySpells()
        {
            if (effects.SelectedItem == null)
            {
                return;
            }
            EffectsEnum effect = 0;

            if (Enum.TryParse<EffectsEnum>(effects.SelectedItem.ToString(), out effect))
            {
                spells.Items.Clear();

                foreach (var spell in Effects[effect])
                {
                    spells.Items.Add(spell);
                }
            }
            else
            {
                MessageBox.Show("Unknown Effects Enum " + effects.SelectedItem.ToString());
            }
        }
        private void Effects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplaySpells();
        }

        private void ExploreClick(object sender, RoutedEventArgs e)
        {
            if (spells.SelectedItem != null)
            {
                CastSpell castSpell = new CastSpell((SpellRecord)spells.SelectedItem);
                castSpell.Show();
            }
        }
        private void UnhandledOnlyClicked(object sender, RoutedEventArgs e)
        {
            Load();
        }
        private void Load()
        {
            if (loading)
            {
                OnCancel = new Action(() =>
                {
                    OnCancel = null;
                    InternalLoad();
                });

                CancelSource.Cancel();

            }
            else
            {
                InternalLoad();
            }
        }
        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Load();
        }
    }
}

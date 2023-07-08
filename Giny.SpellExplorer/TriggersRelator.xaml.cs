using Giny.Core.Extensions;
using Giny.Protocol.Enums;
using Giny.World.Managers.Fights.Effects;
using Giny.World.Managers.Fights.Triggers;
using Giny.World.Records.Breeds;
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
    public partial class TriggersRelator : Window
    {
        Dictionary<string, List<SpellRecord>> TriggerSpells = new Dictionary<string, List<SpellRecord>>();

        private CancellationTokenSource CancelSource
        {
            get;
            set;
        }

        private Action OnCancel;

        private bool loading;

        public TriggersRelator()
        {
            InitializeComponent();
            triggers.SelectionChanged += Effects_SelectionChanged;
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

            Task.Run(() =>
            {
                loading = true;

                bool breedSpellOnly = false;

                this.Dispatcher.Invoke(() =>
                {
                    breedSpellOnly = breedSpells.IsChecked.Value;
                    spells.Items.Clear();
                    triggers.Items.Clear();
                    TriggerSpells.Clear();
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
                            int index = 0;
                            foreach (var trigger in effect.RawTriggers.Split('|'))
                            {
                                string triggerName = trigger.RemoveNumbers() + " (" + effect.Triggers[index].Type + ")";

                                if (!TriggerSpells.ContainsKey(triggerName))
                                {
                                    TriggerSpells.Add(triggerName, new List<SpellRecord>());

                                    this.Dispatcher.Invoke(() =>
                                    {

                                        triggers.Items.Add(triggerName);
                                    });
                                }

                                if (breedSpellOnly)
                                {
                                    if (IsBreedSpell(spell))
                                    {
                                        if (!TriggerSpells[triggerName].Contains(spell))
                                            TriggerSpells[triggerName].Add(spell);
                                    }
                                }
                                else
                                {
                                    if (!TriggerSpells[triggerName].Contains(spell))
                                        TriggerSpells[triggerName].Add(spell);
                                }
                                index++;
                            }

                            this.Dispatcher.Invoke(() =>
                            {
                                if (triggers.SelectedItem != null && triggers.SelectedValue.ToString() == effect.EffectEnum.ToString())
                                {
                                    spells.Items.Add(spell);
                                }
                            });

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
        private bool IsBreedSpell(SpellRecord spell)
        {
            foreach (var breed in BreedRecord.GetBreeds())
            {
                if (breed.SpellIds.Contains(spell.Id))
                {
                    return true;
                }
            }
            return false;
        }
        private void DisplaySpells()
        {
            if (triggers.SelectedItem == null)
            {
                return;
            }

            spells.Items.Clear();

            foreach (var spell in TriggerSpells[triggers.SelectedItem.ToString()])
            {
                spells.Items.Add(spell);
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

        private void breedSpells_Click(object sender, RoutedEventArgs e)
        {
            Load();

        }
    }
}

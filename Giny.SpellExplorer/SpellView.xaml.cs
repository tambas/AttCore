using Giny.Protocol.Enums;
using Giny.World.Managers.Effects;
using Giny.World.Managers.Effects.Targets;
using Giny.World.Records.Effects;
using Giny.World.Records.Monsters;
using Giny.World.Records.Spells;
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

namespace Giny.SpellExplorer
{
    /// <summary>
    /// Logique d'interaction pour SpellView.xaml
    /// </summary>
    public partial class SpellView : UserControl
    {
        private SpellRecord Spell
        {
            get;
            set;
        }
        public SpellView(SpellRecord spell, byte? gradeId = null)
        {
            this.Spell = spell;
            InitializeComponent();
            effectsList.SelectionChanged += EffectsList_SelectionChanged;
            levelsList.SelectionChanged += LevelsList_SelectionChanged;
            UpdateSpellInfo();

            if (gradeId.HasValue && gradeId.Value - 1 < levelsList.Items.Count)
            {
                levelsList.SelectedItem = levelsList.Items.GetItemAt(gradeId.Value - 1);
                levelsList.Focus();
            }
        }

        private void LevelsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var level = (SpellLevelRecord)levelsList.SelectedItem;

            effectsList.Items.Clear();

            foreach (var effect in level.Effects)
            {
                effectsList.Items.Add(effect);
            }


        }
        private string TriggersToString(IEnumerable<World.Managers.Fights.Triggers.Trigger> triggers)
        {
            string result = string.Empty;

            foreach (var trigger in triggers)
            {
                result += trigger.Type;

                if (trigger.Value.HasValue)
                {
                    result += " (" + trigger.Value + ")";
                }

                if (trigger != triggers.Last())
                    result += ",";
            }

            return result;
        }
        private void EffectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            effectInfo.Items.Clear();

            EffectDice effect = (EffectDice)effectsList.SelectedItem;

            if (effect == null)
            {
                return;
            }

            EffectRecord effectRecord = EffectRecord.GetEffectRecord(effect.EffectEnum);

            effectInfo.Items.Add("Effect : " + effect.EffectEnum);
            // effectInfo.Items.Add("Effect Description : " + effectRecord.Description);
            effectInfo.Items.Add("Min,Max : (" + effect.Min + "," + effect.Max + ")");
            effectInfo.Items.Add("Value : " + effect.Value);
            effectInfo.Items.Add("Duration : " + effect.Duration);
            effectInfo.Items.Add("Delay : " + effect.Delay);
            effectInfo.Items.Add("TargetMask : " + effect.TargetMask);

            string targets = string.Join(",", effect.GetTargets());

            if (targets == string.Empty)
            {
                targets = "ALL";
            }
            effectInfo.Items.Add("Targets : " + targets);
            effectInfo.Items.Add("Triggers : " + effect.RawTriggers);
            effectInfo.Items.Add("Triggers Enum : " + TriggersToString(effect.Triggers));
            effectInfo.Items.Add("Raw Zone : " + effect.RawZone);
            effectInfo.Items.Add("Order : " + effect.Order);
            effectInfo.Items.Add("Modificator : " + effect.Modificator);
            effectInfo.Items.Add("Record Priority : " + effectRecord.Priority);
            effectInfo.Items.Add("Group : " + effect.Group);
            effectInfo.Items.Add("TargetId : " + effect.TargetId);
            effectInfo.Items.Add("Dispellable : " + (FightDispellableEnum)effect.Dispellable);
            effectInfo.Items.Add("Trigger : " + effect.Trigger);
            effectInfo.Items.Add("Random : " + effect.Random);

            switch (effect.EffectEnum)
            {
                case EffectsEnum.Effect_Summon:
                case EffectsEnum.Effect_SummonSlave:
                    MonsterRecord monster = MonsterRecord.GetMonsterRecord((short)effect.Min);

                    if (monster != null)
                    {
                        effectInfo.Items.Add("Summoned : " + monster.Name);
                    }
                    else
                    {
                        effectInfo.Items.Add("Unknown Summon.");
                    }
                    break;
                case EffectsEnum.Effect_CastSpell_793:
                case EffectsEnum.Effect_CastSpell_792:
                case EffectsEnum.Effect_CastSpell_2794:
                case EffectsEnum.Effect_CastSpell_2160:
                case EffectsEnum.Effect_CastSpell_1175:
                case EffectsEnum.Effect_CastSpell_1160:
                case EffectsEnum.Effect_CastSpell_1019:
                case EffectsEnum.Effect_CastSpell_1018:
                case EffectsEnum.Effect_CastSpell_1017:
                case EffectsEnum.Effect_CastSpell_2792:
                case EffectsEnum.Effect_Trap:

                    Button button = new Button();
                    button.Content = "Explore";
                    button.Width = 100;
                    button.Height = 30;

                    SpellRecord spell = SpellRecord.GetSpellRecord((short)effect.Min);

                    if (spell != null)
                    {
                        effectInfo.Items.Add("Target Spell : " + spell.Name);

                        button.Click += (object o, RoutedEventArgs args) =>
                        {



                            if (spell.Id == Spell.Id)
                            {
                                levelsList.SelectedItem = levelsList.Items.GetItemAt(effect.Max - 1);
                                levelsList.Focus();
                            }
                            else
                            {
                                CastSpell castSpell = new CastSpell(spell, (byte)effect.Max);
                                castSpell.Show();
                            }
                        };

                        effectInfo.Items.Add(button);
                    }
                    else
                    {
                        effectInfo.Items.Add("Unknown Target Spell : " + effect.Min);
                    }
                    break;
                default:
                    break;
            }


            if (effect.EffectEnum == EffectsEnum.Effect_AddState || effect.EffectEnum == EffectsEnum.Effect_DispelState)
            {
                var state = SpellStateRecord.GetSpellStateRecord(effect.Value);
                effectInfo.Items.Add("State : " + state.Name);
            }

            if (effect.EffectEnum == EffectsEnum.Effect_SpellBoostBaseDamage)
            {
                effectInfo.Items.Add("Boosted Spell : " + SpellRecord.GetSpellRecord((short)effect.Min).Name);
            }
            if (effect.EffectEnum == EffectsEnum.Effect_RemoveSpellEffects)
            {
                var spell = SpellRecord.GetSpellRecord((short)effect.Value);

                if (spell != null)
                {
                    effectInfo.Items.Add("Dispelled Spell : " + spell.Name);
                }
                else
                {
                    effectInfo.Items.Add("Dispelled Spell : Not found");
                }
            }




        }
        private void UpdateSpellInfo()
        {
            spellInfo.Items.Clear();

            spellInfo.Items.Add("Id : " + Spell.Id);

            if (Spell.Description != string.Empty)
            {
                spellInfo.Items.Add("Description : " + Spell.Description);
            }

            spellInfo.Items.Add("Name : " + Spell.Name);

            levelsList.Items.Clear();

            foreach (var level in Spell.Levels)
            {
                levelsList.Items.Add(level);
            }

            var monster = MonsterRecord.GetMonsterRecords().FirstOrDefault(x => x.Spells.Contains(Spell.Id));

            if (monster != null)
            {
                spellInfo.Items.Add("Monster owner name : " + monster.Name + " (" + monster.Id + ")");
            }

        }
    }
}

using Giny.Core.Extensions;
using Giny.IO.D2O;
using Giny.IO.D2OClasses;
using Giny.ORM;
using Giny.World.Records.Maps;
using Giny.World.Records.Monsters;
using Giny.World.Records.Npcs;
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

namespace Giny.Dungeons
{
    /// <summary>
    /// Logique d'interaction pour Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        private DungeonRecord SelectedDungeon
        {
            get
            {
                return (DungeonRecord)dungeons.SelectedItem;
            }
        }


        private MonsterRecord SearchedMonster
        {
            get
            {
                return (MonsterRecord)searchMonsters.SelectedItem;
            }
        }
        public Editor()
        {
            InitializeComponent();


            foreach (var dungeon in DungeonRecord.GetDungeonRecords())
            {
                dungeons.Items.Add(dungeon);
            }

            mapsCanvas.Visibility = Visibility.Hidden;
            monsterCanvas.Visibility = Visibility.Hidden;

        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dungeons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mapName.Content = string.Empty;

            if (SelectedDungeon != null)
            {
                DisplayDungeonMaps();
            }
        }
        private void DisplayDungeonMaps()
        {
            mapsCanvas.Visibility = Visibility.Visible;
            monsterCanvas.Visibility = Visibility.Hidden;

            maps.Items.Clear();

            foreach (var mapId in SelectedDungeon.Rooms.Keys)
            {
                maps.Items.Add(mapId);
            }

            entrance.Text = SelectedDungeon.EntranceMapId.ToString();
            exit.Text = SelectedDungeon.ExitMapId.ToString();
        }
        private void maps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (maps.SelectedItem != null)
            {
                long mapId = (long)maps.SelectedItem;

                mapIdRoom.Text = mapId.ToString();

                monsterCanvas.Visibility = Visibility.Visible;

                monsters.Items.Clear();

                foreach (var monster in SelectedDungeon.Rooms[mapId].MonsterIds)
                {
                    monsters.Items.Add(MonsterRecord.GetMonsterRecord(monster));
                }

                MapPositionRecord positionRecord = MapPositionRecord.GetMapPosition(mapId);

                if (positionRecord != null)
                {
                    mapName.Content = positionRecord.Name;

                }
                else
                {
                    mapName.Content = "None";
                }

                respawnDelay.Text = SelectedDungeon.Rooms[mapId].RespawnDelay.ToString();

            }
        }

        private void monsters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (monsters.SelectedItem != null)
            {
                MonsterRecord monsterId = (MonsterRecord)monsters.SelectedItem;

            }
        }

        private void searchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var results = MonsterRecord.GetMonsterRecords().Where(x => x.Name.ToLower().Contains(searchText.Text.ToLower()) || x.Id.ToString() == searchText.Text);

            searchMonsters.Items.Clear();

            foreach (var result in results)
            {
                searchMonsters.Items.Add(result);
            }
        }

        private void addDungeon_Click(object sender, RoutedEventArgs e)
        {
            DungeonRecord record = new DungeonRecord();

            record.Id = TableManager.Instance.PopId<DungeonRecord>();

            record.Name = djName.Text;
            record.Rooms = new Dictionary<long, MonsterRoom>();

            record.EntranceMapId = 0;
            record.ExitMapId = 0;

            record.AddInstantElement();

            dungeons.Items.Add(record);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            long id = long.Parse(mapId.Text);

            if (SelectedDungeon.Rooms.ContainsKey(id))
            {
                return;
            }
            SelectedDungeon.Rooms.Add(id, new MonsterRoom());
            SelectedDungeon.UpdateInstantElement();
            maps.Items.Add(id);

            mapId.Text = string.Empty;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (maps.SelectedItem != null)
            {
                long mapId = (long)maps.SelectedItem;
                SelectedDungeon.Rooms.Remove(mapId);
                maps.Items.Remove(maps.SelectedItem);
                SelectedDungeon.UpdateInstantElement();
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (SearchedMonster != null)
            {
                InsertMonster(SearchedMonster);
            }
        }
        private void InsertMonster(MonsterRecord record)
        {
            long mapId = (long)maps.SelectedItem;
            SelectedDungeon.Rooms[mapId].MonsterIds.Add((short)record.Id);
            SelectedDungeon.UpdateInstantElement();
            monsters.Items.Add(record);
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (SelectedDungeon != null)
            {
                SelectedDungeon.RemoveInstantElement();
                dungeons.Items.Remove(SelectedDungeon);
            }
        }

        private void entrance_LostFocus(object sender, RoutedEventArgs e)
        {
            SelectedDungeon.EntranceMapId = long.Parse(entrance.Text);
            SelectedDungeon.UpdateInstantElement();
        }

        private void exit_LostFocus(object sender, RoutedEventArgs e)
        {
            SelectedDungeon.ExitMapId = long.Parse(exit.Text);
            SelectedDungeon.UpdateInstantElement();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var monsters = idList.Text.Split(',');

            foreach (var monsterId in monsters)
            {
                InsertMonster(MonsterRecord.GetMonsterRecord(short.Parse(monsterId)));
            }

            idList.Text = string.Empty;
        }

        private void MoveUpClick(object sender, RoutedEventArgs e)
        {
            int index = maps.SelectedIndex;

            if (index == 0)
            {
                return;
            }

            var list = SelectedDungeon.Rooms.ToList();
            list.Swap(index, index - 1);
            SelectedDungeon.Rooms = list.ToDictionary(obj => obj.Key, obj => obj.Value);
            SelectedDungeon.UpdateInstantElement();
            DisplayDungeonMaps();


            maps.SelectedIndex = index - 1;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            int index = maps.SelectedIndex;

            if (index == maps.Items.Count - 1)
            {
                return;
            }

            var list = SelectedDungeon.Rooms.ToList();
            list.Swap(index, index + 1);
            SelectedDungeon.Rooms = list.ToDictionary(obj => obj.Key, obj => obj.Value);
            SelectedDungeon.UpdateInstantElement();
            DisplayDungeonMaps();

            maps.SelectedIndex = index + 1;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            string search = (SelectedDungeon.Name + " - Sortie").ToLower();

            var exit = MapPositionRecord.GetMapPositions().FirstOrDefault(x => x.Name.ToLower() == search);

            if (exit != null && !SelectedDungeon.Rooms.ContainsKey(exit.Id))
            {
                SelectedDungeon.Rooms.Add(exit.Id, new MonsterRoom());
                SelectedDungeon.UpdateInstantElement();
                DisplayDungeonMaps();
                maps.SelectedItem = exit.Id;
            }

        }

        private void searchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            dungeons.Items.Clear();

            var search = searchName.Text.ToLower();

            var results = DungeonRecord.GetDungeonRecords().Where(x => x.Name.ToLower().Contains(search));

            foreach (var result in results)
            {
                dungeons.Items.Add(result);
            }
        }

        private void respawnDelay_LostFocus(object sender, RoutedEventArgs e)
        {
            long mapId = (long)maps.SelectedItem;
            SelectedDungeon.Rooms[mapId].RespawnDelay = int.Parse(respawnDelay.Text);
            SelectedDungeon.UpdateInstantElement();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            var values = names.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<MonsterRecord> monsters = new List<MonsterRecord>();

            foreach (var value in values)
            {
                var name = Regex.Match(value.Trim(), @"(.*)\s*(\(.*\))").Groups[1].Value.Trim();

                MonsterRecord record = MonsterRecord.GetMonsterRecords().FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

                if (record == null)
                {
                    name = Regex.Match(value.Trim(), @"\(.*\)(.*)\s*").Groups[1].Value.Trim();
                    record = MonsterRecord.GetMonsterRecords().FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
                }


                if (record != null)
                {
                    monsters.Add(record);

                }
                else
                {
                    MessageBox.Show("Unknown monster :" + value + " aborting.");
                    return;

                }
            }

            foreach (var monster in monsters)
            {
                InsertMonster(monster);
            }

            names.Text = string.Empty;

        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            if (monsters.SelectedIndex >= 0)
            {
                long mapId = (long)maps.SelectedItem;
                SelectedDungeon.Rooms[mapId].MonsterIds.RemoveAt(monsters.SelectedIndex);
                SelectedDungeon.UpdateInstantElement();
                monsters.Items.RemoveAt(monsters.SelectedIndex);
                monsters.SelectedIndex = 0;
            }
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            long mapId = (long)maps.SelectedItem;
            SelectedDungeon.Rooms[mapId].MonsterIds.Clear();
            SelectedDungeon.UpdateInstantElement();
            monsters.Items.Clear();
        }
    }
}

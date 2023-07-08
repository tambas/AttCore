using Giny.IO.D2O;
using Giny.IO.D2OClasses;
using Giny.Npcs.Actions;
using Giny.ORM;
using Giny.Protocol.Custom.Enums;
using Giny.World.Managers.Entities.Look;
using Giny.World.Records.Maps;
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

namespace Giny.Npcs
{
    /// <summary>
    /// Logique d'interaction pour Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        public Editor()
        {
            InitializeComponent();
            SearchNpcs();
            editorCanvas.Visibility = Visibility.Hidden;
        }

        private NpcSpawnRecord CurrentNpc
        {
            get
            {
                return (NpcSpawnRecord)npcs.SelectedItem;
            }
        }
        private NpcActionRecord CurrentAction
        {
            get
            {
                return (NpcActionRecord)actions.SelectedItem;
            }
        }

        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            SearchNpcs();
        }
        private void SearchNpcs()
        {
            if (searchText.Text != string.Empty)
            {
                var npcs = NpcSpawnRecord.GetNpcSpawns().Where(x => x.ToString().ToLower().Contains(searchText.Text));
                DisplayNpcs(npcs);
            }
            else
            {
                DisplayNpcs(NpcSpawnRecord.GetNpcSpawns());
            }
        }
        private void DisplayNpcs(IEnumerable<NpcSpawnRecord> records)
        {
            npcs.Items.Clear();

            foreach (var npc in records)
            {
                npcs.Items.Add(npc);
            }
        }

        private void NpcSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentNpc != null)
            {
                editorCanvas.Visibility = Visibility.Visible;
                actions.Items.Clear();
                actionsContent.Content = null;
                DisplayActions(CurrentNpc);

                mapId.Text = CurrentNpc.MapId.ToString();
                cellId.Text = CurrentNpc.CellId.ToString();

                direction.Items.Clear();

                npcName.Text = Loader.D2IFile.GetText((int)D2OManager.GetObject<Npc>("Npcs.d2o", CurrentNpc.TemplateId).nameId);

                foreach (var value in Enum.GetValues(typeof(DirectionsEnum)))
                {
                    direction.Items.Add(value);
                }

                direction.SelectedItem = CurrentNpc.Direction;

                if (actions.Items.Count > 0)
                {
                    actions.SelectedItem = actions.Items[0];
                 }

            }
        }

        public static BitmapSource Convert(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            return bitmapSource;
        }


        public void DisplayActions(NpcSpawnRecord record)
        {
            actions.Items.Clear();

            foreach (var action in record.Actions)
            {
                actions.Items.Add(action);
            }
        }


        private void ActionsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentAction == null)
            {
                return;
            }

            switch (CurrentAction.Action)
            {
                case NpcActionsEnum.BUY_SELL:
                    actionsContent.Content = new BuySell(CurrentAction);
                    break;
                case NpcActionsEnum.TALK:
                    actionsContent.Content = new Talk(CurrentNpc, CurrentAction);
                    break;
            }

        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void UpdateRecord()
        {
            CurrentNpc.Template = NpcRecord.GetNpcRecord(CurrentNpc.TemplateId);
            CurrentNpc.MapId = int.Parse(mapId.Text);
            CurrentNpc.CellId = short.Parse(cellId.Text);
            CurrentNpc.Direction = (DirectionsEnum)direction.SelectedItem;
            CurrentNpc.UpdateInstantElement();
        }
        private void RefreshNpcList()
        {
            var current = CurrentNpc;
            DisplayNpcs(NpcSpawnRecord.GetNpcSpawns());
            npcs.SelectedItem = current;
        }


        private void mapId_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateRecord();
            RefreshNpcList();
        }

        private void cellId_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateRecord();
        }

        private void direction_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateRecord();
        }

        public void AddAction(NpcActionsEnum action)
        {
            NpcActionRecord record = new NpcActionRecord()
            {
                Action = action,
                Id = TableManager.Instance.PopId<NpcActionRecord>(),
                NpcSpawnId = CurrentNpc.Id,
                Param1 = string.Empty,
                Param2 = string.Empty,
                Param3 = string.Empty,
                Criteria = string.Empty,
            };

            record.AddInstantElement();
            CurrentNpc.Actions.Add(record);
            actions.Items.Add(record);

            actions.SelectedItem = record;
        }

        private void AddActionClick(object sender, RoutedEventArgs e)
        {
            AddAction window = new AddAction(this, CurrentNpc.Template, CurrentNpc.Actions.Select(x => x.Action));
            window.Show();
        }

        private void RemoveCurrentActionClick(object sender, RoutedEventArgs e)
        {
            if (actions.SelectedItem == null)
                return;

            var npcAction = (NpcActionRecord)actions.SelectedItem;
            RemoveNpcAction(npcAction);
            CurrentNpc.Actions.Remove(npcAction);
            actions.Items.Remove(npcAction);
            actionsContent.Content = string.Empty;
        }

        public static void RemoveNpcAction(NpcActionRecord actionRecord)
        {
            if (actionRecord.Action == NpcActionsEnum.TALK)
            {
                if (actionRecord.Param1 != string.Empty)
                {
                    var replies = NpcReplyRecord.GetNpcReplies(actionRecord.NpcSpawnId, int.Parse(actionRecord.Param1));
                    replies.RemoveInstantElements();
                }
            }
            actionRecord.RemoveInstantElement();
        }

        private void Savei18nClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Loader.D2IFile.Save();
                MessageBox.Show("D2I file saved sucessfully !", "Informations", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch
            {
                MessageBox.Show("Unable to save file. Please very the file is not used by another process", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void npcName_LostFocus(object sender, RoutedEventArgs e)
        {
            CurrentNpc.Template.Name = npcName.Text;
            var d2oNpc = D2OManager.GetObject<Npc>("Npcs.d2o", CurrentNpc.TemplateId);
            Loader.D2IFile.SetText((int)d2oNpc.NameId, npcName.Text);
            RefreshNpcList();
            CurrentNpc.Template.UpdateInstantElement();

        }
    }
}

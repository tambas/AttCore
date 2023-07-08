using Giny.Protocol.Custom.Enums;
using Giny.World.Records.Npcs;
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

namespace Giny.Npcs.Actions
{
    /// <summary>
    /// Logique d'interaction pour AddAction.xaml
    /// </summary>
    public partial class AddAction : Window
    {
        Editor Editor
        {
            get;
            set;
        }

        public AddAction(Editor editor, NpcRecord npcRecord, IEnumerable<NpcActionsEnum> currentActions)
        {
            this.Editor = editor;
            InitializeComponent();

            foreach (var value in npcRecord.Actions)
            {
                NpcActionsEnum actionEnum = (NpcActionsEnum)value;
                actions.Items.Add(actionEnum);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Editor.AddAction((NpcActionsEnum)actions.SelectedItem);
            this.Close();
        }
    }
}

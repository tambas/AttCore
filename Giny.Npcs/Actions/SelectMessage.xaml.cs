using Giny.IO.D2O;
using Giny.IO.D2OClasses;
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
    /// Logique d'interaction pour SelectMessage.xaml
    /// </summary>
    public partial class SelectMessage : Window
    {
        private Talk Talk
        {
            get;
            set;
        }
        public SelectMessage(Talk talk, int npcId)
        {
            this.Talk = talk;
            Npc npc = D2OManager.GetObject<Npc>("Npcs.d2o", npcId);

            InitializeComponent();

            foreach (var msg in npc.dialogMessages)
            {
                DialogMessage message = new DialogMessage(msg[0], Loader.D2IFile.GetText(msg[1]));
                messages.Items.Add(message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogMessage current = (DialogMessage)messages.SelectedItem;
            Talk.SelectMessage(current.Id);

            this.Close();
        }
    }
    public class DialogMessage
    {
        public int Id
        {
            get;
            private set;
        }
        public string Text
        {
            get;
            private set;
        }
        public DialogMessage(int id,string text)
        {
            this.Id = id;
            this.Text = text;
        }
        public override string ToString()
        {
            return Text;
        }
    }
}

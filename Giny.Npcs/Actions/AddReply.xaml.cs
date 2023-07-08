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

namespace Giny.Npcs
{
    /// <summary>
    /// Logique d'interaction pour AddReply.xaml
    /// </summary>
    public partial class AddReply : Window
    {
        private int NpcId
        {
            get;
            set;
        }
        private Talk Talk
        {
            get;
            set;
        }
        public AddReply(Talk talk, int npcId)
        {
            this.NpcId = npcId;
            InitializeComponent();

            this.Talk = talk;

            Npc npc = D2OManager.GetObject<Npc>("Npcs.d2o", NpcId);

            foreach (var reply in npc.DialogReplies)
            {
                int replyId = reply[0];
                int messageId = reply[1];

                var text = Loader.D2IFile.GetText(messageId);
                replies.Items.Add(new NpcD2OReply(replyId, text, messageId));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Talk.AddReply((NpcD2OReply)replies.SelectedItem);
            this.Close();
        }
    }
    public class NpcD2OReply
    {
        public int ReplyId
        {
            get;
            set;
        }
        public string Text
        {
            get;
            set;
        }
        public int TextId
        {
            get;
            set;
        }
        public NpcD2OReply(int replyId, string text, int textId)
        {
            this.ReplyId = replyId;
            this.Text = text;
            this.TextId = textId;
        }
        public override string ToString()
        {
            return "{" + ReplyId + "} " + Text;
        }
    }
}

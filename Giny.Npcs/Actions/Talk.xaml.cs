using Giny.IO.D2O;
using Giny.IO.D2OClasses;
using Giny.Npcs.Actions;
using Giny.ORM;
using Giny.ORM.Interfaces;
using Giny.World.Managers.Generic;
using Giny.World.Records.Maps;
using Giny.World.Records.Npcs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Giny.Npcs
{
    /// <summary>
    /// Logique d'interaction pour Talk.xaml
    /// </summary>
    public partial class Talk : UserControl
    {
        private NpcActionRecord ActionRecord
        {
            get;
            set;
        }
        private NpcSpawnRecord SpawnRecord
        {
            get;
            set;
        }
        private bool ReplyLoaded
        {
            get;
            set;
        }
        public Talk(NpcSpawnRecord spawn, NpcActionRecord record)
        {
            this.ActionRecord = record;
            this.SpawnRecord = spawn;

            InitializeComponent();

            DisplayMessage();
            DisplayReplies();

            messageCriteria.Text = record.Criteria;

            replyCanvas.Visibility = Visibility.Hidden;

            foreach (var action in Enum.GetValues(typeof(GenericActionEnum)))
            {
                actions.Items.Add(action);
            }

            if (ActionRecord.Param1 == string.Empty)
            {
                OpenMessageSelectionDialog();
            }
        }


        public void DisplayMessage()
        {
            if (ActionRecord.Param1 == string.Empty)
            {
                return;
            }
            var messageId = int.Parse(ActionRecord.Param1);

            var npcMessage = D2OManager.GetObject<NpcMessage>("NpcMessages.d2o", messageId);
            string text = Loader.D2IFile.GetText((int)npcMessage.MessageId);

            messageText.Text = text;
        }
        public void DisplayReplies()
        {
            if (ActionRecord.Param1 == string.Empty)
            {
                return;
            }
            replies.Items.Clear();
            var messageId = int.Parse(ActionRecord.Param1);

            Npc npc = D2OManager.GetObject<Npc>("Npcs.d2o", SpawnRecord.TemplateId);

            foreach (var replyRecord in NpcReplyRecord.GetNpcReplies(SpawnRecord.Id, messageId))
            {
                var test = npc.dialogReplies.FirstOrDefault(x => x[0] == replyRecord.ReplyId);

                var text = Loader.D2IFile.GetText(test[1]);

                replies.Items.Add(new NpcReply(test[1], text, replyRecord));
            }
        }

        public void AddReply(NpcD2OReply reply)
        {
            NpcReplyRecord replyRecord = new NpcReplyRecord()
            {
                ReplyId = reply.ReplyId,
                NpcSpawnId = SpawnRecord.Id,
                ActionIdentifier = GenericActionEnum.None,
                Id = TableManager.Instance.PopId<NpcReplyRecord>(), // TODO
                MessageId = int.Parse(ActionRecord.Param1),
            };

            replyRecord.AddInstantElement();

            replies.Items.Add(new NpcReply(reply.TextId, reply.Text, replyRecord));
        }

        private void RepliesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReplyLoaded = false;

            replyCanvas.Visibility = Visibility.Visible;

            NpcReply reply = (NpcReply)replies.SelectedItem;

            if (reply != null)
            {
                actions.SelectedItem = reply.Record.ActionIdentifier;
                param1.Text = reply.Record.Param1;
                param2.Text = reply.Record.Param2;
                param3.Text = reply.Record.Param3;
                criterias.Text = reply.Record.Criteria;
                replyText.Text = reply.Text;
                ReplyLoaded = true;
            }
            else
            {
                replyCanvas.Visibility = Visibility.Hidden;
            }
        }

        private void ActionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            param1.Text = string.Empty;
            param2.Text = string.Empty;
            param3.Text = string.Empty;
            criterias.Text = string.Empty;

            if ((GenericActionEnum)actions.SelectedItem == GenericActionEnum.Reach)
            {
                param1.Text = PopNextReachId().ToString();
            }
            else if ((GenericActionEnum)actions.SelectedItem == GenericActionEnum.Fight)
            {
                param2.Text = PopNextReachId().ToString();
            }

            UpdateReply();
        }
        private int PopNextReachId()
        {
            short maxId = 0;

            foreach (var reply in NpcReplyRecord.GetNpcReplies())
            {
                if (reply.ActionIdentifier == GenericActionEnum.Reach && reply.Param1 != string.Empty)
                {
                    short value = short.Parse(reply.Param1);

                    if (value > maxId)
                    {
                        maxId = value;
                    }
                }
                if (reply.ActionIdentifier == GenericActionEnum.Fight && reply.Param2 != string.Empty)
                {
                    short value = short.Parse(reply.Param2);

                    if (value > maxId)
                    {
                        maxId = value;
                    }
                }
            }

            foreach (var interactiveSkill in InteractiveSkillRecord.GetInteractiveSkills())
            {
                if (interactiveSkill.ActionIdentifier == GenericActionEnum.Reach && interactiveSkill.Param1 != string.Empty)
                {
                    short value = short.Parse(interactiveSkill.Param1);

                    if (value > maxId)
                    {
                        maxId = value;
                    }
                }
            }

            return maxId + 1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ActionRecord.Param1 != string.Empty)
            {
                var replyDialog = new AddReply(this, SpawnRecord.TemplateId);
                replyDialog.Show();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (replies.SelectedItem != null)
            {
                ((NpcReply)replies.SelectedItem).Record.RemoveInstantElement();
                replies.Items.Remove(replies.SelectedItem);
            }

        }

        private void UpdateReply()
        {
            if (!ReplyLoaded)
            {
                return;
            }
            var reply = (NpcReply)replies.SelectedItem;

            reply.Record.Param1 = param1.Text;
            reply.Record.Param2 = param2.Text;
            reply.Record.Param3 = param3.Text;
            reply.Record.Criteria = criterias.Text;
            reply.Record.ActionIdentifier = (GenericActionEnum)actions.SelectedItem;
            reply.Record.UpdateInstantElement();
        }
        private void param1_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateReply();
        }

        private void param2_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateReply();
        }

        private void param3_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateReply();
        }

        private void criterias_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateReply();
        }

        public void OpenMessageSelectionDialog()
        {
            SelectMessage message = new SelectMessage(this, SpawnRecord.TemplateId);
            message.Show();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenMessageSelectionDialog();
        }
        public void SelectMessage(int messageId)
        {
            ActionRecord.Param1 = messageId.ToString();
            ActionRecord.UpdateInstantElement();
            DisplayMessage();
        }

        private void messageCriteria_LostFocus(object sender, RoutedEventArgs e)
        {
            ActionRecord.Criteria = messageCriteria.Text;
            ActionRecord.UpdateInstantElement();
        }

        private void messageText_LostFocus(object sender, RoutedEventArgs e)
        {
            var messageId = int.Parse(ActionRecord.Param1);
            var npcMessage = D2OManager.GetObject<NpcMessage>("NpcMessages.d2o", messageId);
            string text = messageText.Text;
            Loader.D2IFile.SetText((int)npcMessage.MessageId, text);
        }

        private void replyText_LostFocus(object sender, RoutedEventArgs e)
        {
            var indice = replies.SelectedIndex;
            NpcReply reply = (NpcReply)replies.SelectedItem;
            Loader.D2IFile.SetText((int)reply.TextId, replyText.Text);
            DisplayReplies();
            replies.SelectedIndex = indice;
        }
    }
    public class NpcReply
    {
        public int TextId
        {
            get;
            set;
        }
        public string Text
        {
            get;
            set;
        }
        public NpcReplyRecord Record
        {
            get;
            set;
        }

        public NpcReply(int textId, string text, NpcReplyRecord record)
        {
            this.TextId = textId;
            this.Text = text;
            this.Record = record;
        }
        public override string ToString()
        {
            return "{" + TextId + "} " + Text;
        }
    }
}

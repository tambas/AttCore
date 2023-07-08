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
using System.Windows.Shapes;

namespace Giny.SpellExplorer
{
    /// <summary>
    /// Logique d'interaction pour CastSpell.xaml
    /// </summary>
    public partial class CastSpell : Window
    {
        public CastSpell(SpellRecord spell, byte? gradeId = null)
        {
            InitializeComponent();

            this.contentControl.Content = new SpellView(spell, gradeId);
        }
    }
}

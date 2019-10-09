using DestinyPCLoadoutManager.Controls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DestinyPCLoadoutManager.Controls
{
    /// <summary>
    /// Interaction logic for CharacterList.xaml
    /// </summary>
    public partial class CharacterList : UserControl
    {
        public CharacterList()
        {
            InitializeComponent();
        }

        public void SetCharacterIds(IEnumerable<long> ids)
        {
            icCharacterList.ItemsSource = ids.Select(id => new CharacterListItem
            {
                Title = id.ToString()
            });
        }
    }
}

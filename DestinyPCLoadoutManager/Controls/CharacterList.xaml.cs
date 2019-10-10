using DestinyPCLoadoutManager.API.Models;
using DestinyPCLoadoutManager.Controls.Models;
using DestinyPCLoadoutManager.Logic.Models;
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
        private IEnumerable<Character> characters;

        public CharacterList()
        {
            InitializeComponent();
        }

        public void SetCharacters(IEnumerable<Character> characters)
        {
            this.characters = characters;

            icCharacterList.ItemsSource = this.characters.Select(character => new CharacterListItem
            {
                Title = string.Format("{0}, Light {1}", character.classType.ToString(), character.light),
                Action = new CommandLambda(() =>
                {
                    System.Diagnostics.Debug.WriteLine(character.id);
                })
            });
        }
    }
}

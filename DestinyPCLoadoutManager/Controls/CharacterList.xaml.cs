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
        private Dictionary<long, Character> characters;
        private Character selectedCharacter;

        private Dictionary<long, CharacterListItem> items;

        public CharacterList()
        {
            InitializeComponent();
        }

        public void SetCharacters(Dictionary<long, Character> characters)
        {
            this.characters = characters;

            var selected = Properties.Settings.Default.SelectedGuardian;

            items = this.characters.ToDictionary(kvp => kvp.Key, kvp => new CharacterListItem
            {
                Title = string.Format("{0}, Light {1}", kvp.Value.ClassType.ToString(), kvp.Value.Light),
                Id = kvp.Value.Id
            });

            icCharacterList.ItemsSource = items.Values;

            if (selected != -1 && this.characters.ContainsKey(selected))
            {
                SelectGuardian(selected);
            }
        }

        public void SelectGuardianEvent(object sender, SelectionChangedEventArgs e)
        {
            if (icCharacterList.SelectedItem != null)
            {
                SelectGuardian(((CharacterListItem)icCharacterList.SelectedItem).Id);
            }
        }

        private void SelectGuardian(long id)
        {
            this.selectedCharacter = characters.GetValueOrDefault(id);
            var item = items.GetValueOrDefault(id);

            if (item != null)
            {
                item.IsSelected = true;

                foreach (var deselectItem in items.Where(i => i.Key != id))
                {
                    deselectItem.Value.IsSelected = false;
                }
            }

            Properties.Settings.Default.SelectedGuardian = id;
            Properties.Settings.Default.Save();
        }
    }
}

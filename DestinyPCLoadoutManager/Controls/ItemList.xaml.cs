using DestinyPCLoadoutManager.API.Models;
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
    /// Interaction logic for ItemList.xaml
    /// </summary>
    public partial class ItemList : UserControl
    {
        private IEnumerable<Item> items;

        private IEnumerable<CharacterListItem> listItems;

        public ItemList()
        {
            InitializeComponent();
        }

        public void SetItems(IEnumerable<Item> items)
        {
            this.items = items;

            listItems = this.items.Select(item => new CharacterListItem
            {
                Title = string.Format("{0}, {1}: {2}", item.Name, item.Type, item.SubType),
                Id = item.Id,
            });

            itemList.ItemsSource = listItems;
        }
    }
}

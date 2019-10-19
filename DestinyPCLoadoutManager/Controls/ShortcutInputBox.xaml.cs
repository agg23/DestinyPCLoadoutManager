using DestinyPCLoadoutManager.Logic.Models;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ShortcutInputBox.xaml
    /// </summary>
    public partial class ShortcutInputBox : UserControl
    {
        private bool isEditMode = false;

        private Key key;
        private ModifierKeys modifiers;
        private Action<Key, ModifierKeys> editAction;
        private Action<Key, ModifierKeys> saveAction;
        
        public ShortcutInputBox()
        {
            InitializeComponent();

            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            textBox.IsEnabled = false;

            isEditMode = false;
        }

        public void SetShortcut(Action<Key, ModifierKeys> editAction, Action<Key, ModifierKeys> saveAction)
        {
            this.editAction = editAction;
            this.saveAction = saveAction;
        }

        public void SetShortcut(Key key, ModifierKeys modifiers, Action<Key, ModifierKeys> editAction, Action<Key, ModifierKeys> saveAction)
        {
            this.key = key;
            this.modifiers = modifiers;
            SetShortcut(editAction, saveAction);

            textBox.Text = textFromShortcut(key, modifiers);
        }

        public void ClickEdit(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                // Save
                saveAction(key, modifiers);
            }
            else
            {
                // Edit
                editAction(key, modifiers);
            }

            isEditMode = !isEditMode;
            textBox.IsEnabled = isEditMode;
            
            if (isEditMode)
            {
                textBox.Focus();
            }

            editButton.Content = isEditMode ? "Save" : "Edit";
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // The text box grabs all input.
            e.Handled = true;

            // Fetch the actual shortcut key.
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            // Ignore modifier keys.
            if (key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftAlt || key == Key.RightAlt
                || key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            this.key = key;
            this.modifiers = Keyboard.Modifiers;

            textBox.Text = textFromShortcut(key, Keyboard.Modifiers);
        }

        private string textFromShortcut(Key key, ModifierKeys modifiers)
        {
            // Build the shortcut key name.
            StringBuilder shortcutText = new StringBuilder();
            if ((modifiers & ModifierKeys.Control) != 0)
            {
                shortcutText.Append("Ctrl+");
            }
            if ((modifiers & ModifierKeys.Shift) != 0)
            {
                shortcutText.Append("Shift+");
            }
            if ((modifiers & ModifierKeys.Alt) != 0)
            {
                shortcutText.Append("Alt+");
            }
            shortcutText.Append(key.ToString());

            return shortcutText.ToString();
        }
    }
}

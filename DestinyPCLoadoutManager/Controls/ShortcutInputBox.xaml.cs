using DestinyPCLoadoutManager.API.Models;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DestinyPCLoadoutManager.Controls
{
    /// <summary>
    /// Interaction logic for ShortcutInputBox.xaml
    /// </summary>
    public partial class ShortcutInputBox : UserControl
    {
        private static string NO_SHORTCUT = "No set shortcut";

        private bool isEditMode = false;

        private Key key;
        private ModifierKeys modifiers;
        private Action<Shortcut> editAction;
        private Action<Shortcut> saveAction;
        
        public ShortcutInputBox()
        {
            InitializeComponent();

            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            textBox.IsEnabled = false;
            textBox.Text = NO_SHORTCUT;

            isEditMode = false;
        }

        public void SetShortcut(Action<Shortcut> editAction, Action<Shortcut> saveAction)
        {
            this.editAction = editAction;
            this.saveAction = saveAction;
        }

        public void SetShortcut(Shortcut shortcut, Action<Shortcut> editAction, Action<Shortcut> saveAction)
        {
            this.key = shortcut.Key;
            this.modifiers = shortcut.Modifiers;
            SetShortcut(editAction, saveAction);

            textBox.Text = textFromShortcut(key, modifiers);
        }

        public void ClickEdit(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                // Save
                if (key != Key.None)
                {
                    saveAction(new Shortcut(key, modifiers));
                }
            }
            else
            {
                // Edit
                editAction(new Shortcut(key, modifiers));
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

            if (key == Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
            {
                // Clear input
                key = Key.None;
                modifiers = ModifierKeys.None;
            }

            this.key = key;
            this.modifiers = Keyboard.Modifiers;

            textBox.Text = textFromShortcut(key, Keyboard.Modifiers);
        }

        private string textFromShortcut(Key key, ModifierKeys modifiers)
        {
            if (key == Key.None && modifiers == ModifierKeys.None)
            {
                return NO_SHORTCUT;
            }
            
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

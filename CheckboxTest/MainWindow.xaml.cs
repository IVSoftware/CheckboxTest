using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;

namespace CheckboxTest
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Content.KeyUp += Content_KeyUp;
        }

        private async void Content_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Delete)
            {
                DeleteConfirmationDialog.XamlRoot = Content.XamlRoot;
                if (await DeleteConfirmationDialog.ShowAsync() == ContentDialogResult.Primary)
                {
                    // Do delete operation...
                }
            }
        }

        private void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            if (sender is ContentDialog dialog)
            {
                InspectContentDialog(dialog);
                if (dialog.FindName("DeleteDontAskCheckbox") is CheckBox checkbox)
                {
                    dialog.DispatcherQueue.TryEnqueue(() =>
                    {
                        dialog.Focus(FocusState.Programmatic);
                    });
                }
            }
        }
        private void InspectContentDialog(ContentDialog dialog)
        {
            Popup popup = FindPopup(dialog);

            if (popup != null && popup.Child is FrameworkElement popupContent)
            {
                Debug.WriteLine($"Popup found: {popupContent.GetType().Name}");
                InventoryControls(popupContent);
            }
            else
            {
                Debug.WriteLine("Popup or its content not found.");
            }
        }

        private Popup FindPopup(DependencyObject parent)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is Popup popup)
                {
                    return popup;
                }
                Popup foundPopup = FindPopup(child);
                if (foundPopup != null)
                {
                    return foundPopup;
                }
            }
            return null;
        }
        private void InventoryControls(DependencyObject parent, int level = 0)
        {
            if (parent == null) return;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                string name = (child as FrameworkElement)?.Name ?? "Unnamed";
                string type = child.GetType().Name;

                Debug.WriteLine($"{new string('-', level * 2)} Type: {type}, Name: {name}");
                InventoryControls(child, level + 1);
            }
        }
    }
}

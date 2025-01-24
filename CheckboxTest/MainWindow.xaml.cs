using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
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
                foreach (var control in Traverse(dialog))
                {
                    if (control is FrameworkElement element)
                    {
                        Debug.WriteLine($"Type: {element.GetType().Name}, Name: {element.Name}");
                    }
                    else
                    {
                        Debug.WriteLine($"Type: {control.GetType().Name}");
                    }
                }
                if (dialog.FindName("DeleteDontAskCheckbox") is CheckBox checkbox)
                {
                    dialog.DispatcherQueue.TryEnqueue(() =>
                    {
                        dialog.Focus(FocusState.Programmatic);
                    });
                }
            }
        }
        private IEnumerable<DependencyObject> Traverse(DependencyObject parent)
        {
            if (parent == null)
                yield break;

            yield return parent; 
            if (parent is Popup popup && popup.Child is DependencyObject popupContent)
            {
                foreach (var descendant in Traverse(popupContent))
                {
                    yield return descendant;
                }
            }
            else
            {
                int childCount = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < childCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    foreach (var descendant in Traverse(child))
                    {
                        yield return descendant;
                    }
                }
            }
        }

    }
}

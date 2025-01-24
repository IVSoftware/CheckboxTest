using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

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
        private void OnContentDialogOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            if (sender is ContentDialog dialog)
            {
                const string BUTTON_TO_FOCUS = "Cancel";
                // Allowing for possible race condition...
                if (dialog.FindName("DeleteDontAskCheckbox") is CheckBox checkbox)
                {
                    // Prevent the thing you don't want
                    checkbox.GettingFocus += (sender, e) =>
                    {
                        e.Handled = e.Cancel = true;
                        localFocusButton();
                    };
                    // Request the thing you do want
                    localFocusButton();

                    #region L o c a l F x
                    void localFocusButton()
                    {                        
                        if (Traverse(dialog)
                            .OfType<Button>()
                            .FirstOrDefault(_ => _.Content?.ToString() == BUTTON_TO_FOCUS) is { } button)
                        {
                            if (button.FocusState == FocusState.Unfocused)
                            {
                                button.Focus(FocusState.Programmatic);
                            }
                        }
                    }
                    #endregion L o c a l F x
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

Granted, this is the more complicated answer but when I tried simply setting `IsTabStop` to `false` I couldn't get that to work, and that doesn't provide the opportunity to focus the intended button. I tested this as a proof of concept and will link my repo in the comments.

**Step One**, we need an iterator for the `ContentDiaolg` controls (because it's predefined, not "our" dialog, not "our" controls).

~~~
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
~~~

___

**Step Two**, use the iterator to obtain the controls you want to manipulate and "do whatever you want".

_THIS IS JUST AN EXAMPLE. You may find that you need to play around with it._

~~~
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
~~~



[![initial focus][1]][1]


  [1]: https://i.sstatic.net/E4fb5BbZ.png
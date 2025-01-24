I see there is an answer that provides a simple way to keep `DeleteDontAskCheckbox` from taking the focus. In terms of the other requirement of accessing the buttons, an enumerator of all the controls might work where `FindName` won't. 

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


You said:

>I'm aware of `VisualTreeHelper.GetOpenPopupsForXamlRoot()` and I've experimented with that in the dialog's Opened event handler, but navigating the subsequent hierarchy is not straightforward (FindName("CloseButton") does not work, for example), and I can't help thinking there's either a more direct way of accessing the button, or someone has written a helper to do the same.

The usage of the iterator, the (somewhat) "more direct way", would look something like this:

~~~
var buttonCancel = Traverse(dialog)
    .OfType<Button>()
    .FirstOrDefault(_ => _.Content?.ToString() == "Cancel");
~~~
___

Using, as you mentioned, the dialog's `Opened` event handler, I tested this as a proof of concept only. You may still have to play around with it a bit. I'll put a link to the code I used to test it if you want to make sure it works on your end.


~~~
private void OnContentDialogOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
{
    if (sender is ContentDialog dialog)
    {
        if( !dialog.DispatcherQueue.TryEnqueue(() => 
        {
            const string BUTTON_TO_FOCUS = "Cancel";

            // Find by text
            if (Traverse(dialog)
                .OfType<Button>()
                .FirstOrDefault(_ => _.Content?.ToString() == BUTTON_TO_FOCUS) is { } button)
            {
                if (button.FocusState == FocusState.Unfocused)
                {
                    button.Focus(FocusState.Programmatic);
                }
            }
        }))
        {
            Debug.WriteLine("Failed to enqueue action.");
        }
    }
}
~~~

___

**XAML with `Opened` event handler**

~~~
<Window
    x:Class="CheckboxTest.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:CheckboxTest"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    Title="ContentDialog Focus Test">

    <StackPanel Orientation="Horizontal" HorizontalAlignment="Center" VerticalAlignment="Center">
        <ContentDialog
            x:Name="DeleteConfirmationDialog"
            Title="Delete file"
            PrimaryButtonText="Move to Recycle Bin"
            CloseButtonText="Cancel"
            DefaultButton="Close"
            Opened="OnContentDialogOpened">
            <StackPanel VerticalAlignment="Stretch" HorizontalAlignment="Stretch" Spacing="12">
                <TextBlock TextWrapping="Wrap" Text="Are you sure you want to move file 'somefile.jpg' to the Recycle Bin?" />
                <CheckBox x:Name="DeleteDontAskCheckbox" IsTabStop="False"  Content="Don't ask me again" />
            </StackPanel>
        </ContentDialog>
    </StackPanel>
</Window>
~~~
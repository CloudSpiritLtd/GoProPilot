using System.Threading.Tasks;
using FluentAvalonia.UI.Controls;

namespace GoProPilot;

public static class Utils
{
    public static Task<ContentDialogResult> ShowErrorMessageAsync(string message)
    {
        var dialog = new ContentDialog()
        {
            Title = "Error",
            Content = message,
            PrimaryButtonText = "OK",
        };
        return dialog.ShowAsync();
    }
}

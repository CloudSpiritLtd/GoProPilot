using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using GoProPilot.ViewModels;

namespace GoProPilot;

public class ViewLocator : IDataTemplate
{
    public IControl Build(object? data)
    {
        if (data == null)
            return new TextBlock { Text = "Not Found: data is null" };

        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}

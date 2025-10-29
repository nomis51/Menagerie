using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Menagerie.ViewModels;
using Menagerie.ViewModels.Abstractions;

namespace Menagerie;

public class ViewLocator : IDataTemplate
{
    #region Public methods

    public Control? Build(object? param)
    {
        if (param is null) return null;

        var paramType = param.GetType();
        if (string.IsNullOrEmpty(paramType.FullName)) return null;

        var name = paramType.FullName.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type is null) return new TextBlock { Text = "Not Found: " + name };

        return (Control)Activator.CreateInstance(type)!;
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }

    #endregion
}
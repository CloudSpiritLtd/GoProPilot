using GoProPilot.Models;
using GoProPilot.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;

namespace GoProPilot.ViewModels;

public class MediaListViewModel : ViewModelBase
{
    public MediaListViewModel()
    {
        GetMediasCommand = ReactiveCommand.Create(ExecuteGetMedias);
    }

    private async void ExecuteGetMedias()
    {
        try
        {
            Medias = await WebApi.Instance.GetMediaGroupsAsync();
        }
        catch (Exception ex)
        {
            await Utils.ShowErrorMessageAsync(ex.Message);
        }
    }

    public ReactiveCommand<Unit, Unit> GetMediasCommand { get; }

    [Reactive]
    public MediaDirectory[] Medias { get; set; } = Array.Empty<MediaDirectory>();
}

using Menagerie.Core.Enums;
using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class AudioService : IAudioService
{
    #region Public methods

    public Task PlayEffectAsync(AudioEffect effect)
    {
        // TODO: implement
        return Task.CompletedTask;
    }

    #endregion
}
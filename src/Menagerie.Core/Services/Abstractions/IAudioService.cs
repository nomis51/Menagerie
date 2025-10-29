using Menagerie.Core.Enums;

namespace Menagerie.Core.Services.Abstractions;

public interface IAudioService
{
    Task PlayEffectAsync(AudioEffect effect);
}
using System.Media;
using Menagerie.Shared.Abstractions;

namespace Menagerie.Application.Services;

#pragma warning disable CA1416
public class AudioService : IService
{
    #region Constants

    private const string NewOfferFilePath = "./assets/sounds/newOffer.wav";
    private const string ClickFilePath = "./assets/sounds/click.wav";
    private const string PlayerJoinFilePath = "./assets/sounds/playerJoin.wav";

    #endregion

    #region Members

#pragma warning disable CS8618
    private SoundPlayer _newOfferSoundPlayer;
    private SoundPlayer _clickSoundPlayer;
    private SoundPlayer _playerJoinSoundPlayer;
#pragma warning restore CS8618

    #endregion

    #region Public methods

    public void Initialize()
    {
        _newOfferSoundPlayer = new SoundPlayer(NewOfferFilePath);
        _clickSoundPlayer = new SoundPlayer(ClickFilePath);
        _playerJoinSoundPlayer = new SoundPlayer(PlayerJoinFilePath);
    }

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public void PlayNewOfferSoundEffect()
    {
        _newOfferSoundPlayer.Play();
    }
    
    public void PlayClickSoundEffect()
    {
        _clickSoundPlayer.Play();
    }
    
    public void PlayPlayerJoinSoundEffect()
    {
        _playerJoinSoundPlayer.Play();
    }

    #endregion
}
#pragma warning restore CA1416

using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class AppService : IAppService
{
    #region Singleton

    private static readonly object LockInstance = new();
#pragma warning disable CS8618
    private static AppService _instance;
#pragma warning restore CS8618

    public static AppService Instance
    {
        get
        {
            lock (LockInstance)
            {
                // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
                _instance ??= new AppService();
            }

            return _instance;
        }
    }

    #endregion

    #region Public methods

    public string GetLogsLocation()
    {
        return string.Empty;
    }

    public void Initialize()
    {
    }

    #endregion
}
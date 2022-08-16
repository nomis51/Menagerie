using System.Collections.Concurrent;
using System.Text;
using Menagerie.Data.Providers;
using Menagerie.Shared.Abstractions;
using Menagerie.Shared.Extensions;
using Menagerie.Shared.Models.Poe;
using Menagerie.Shared.Models.Poe.Stash;

namespace Menagerie.Data.Services;

public class StashService : IService
{
    #region Members

    private readonly ConcurrentDictionary<int, StashTab> _indexToStashTab;

    #endregion

    #region Constructors

    public StashService()
    {
        _indexToStashTab = new ConcurrentDictionary<int, StashTab>();
    }

    #endregion

    #region Public methods

    public void Initialize()
    {
    }

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public StashTab? GetStashTab(int index)
    {
        return !_indexToStashTab.ContainsKey(index) ? default : _indexToStashTab[index];
    }

    public void SetStashTab(int index, StashTab stashTab, bool save = true)
    {
        _indexToStashTab[index] = stashTab;
    }

    #endregion
}
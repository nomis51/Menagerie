﻿using Menagerie.Shared.Abstractions;
using Menagerie.Shared.Helpers;
using Menagerie.Shared.Models.Trading;

namespace Menagerie.Data.Services;

public class ClipboardService : IService
{
    #region Constants

    private const int PollingRate = 500;
    private const int MaxExclusionQueueSize = 10;

    #endregion

    #region Members

    private string _lastValue = string.Empty;
    private Queue<string> _exclusionQueue = new();

    #endregion

    #region Public methods

    public void Initialize()
    {
    }

    public Task Start()
    {
        Listen();
        return Task.CompletedTask;
    }

    #endregion

    #region Private methods

    private void Listen()
    {
        var thread = new Thread(() =>
        {
            _lastValue = ClipboardHelper.GetClipboardValue();

            while (true)
            {
                var value = ClipboardHelper.GetClipboardValue();

                var inExclusionQueue = _exclusionQueue.Contains(value);

                if (!inExclusionQueue)
                {
                    if (value != _lastValue)
                    {
                        AppDataService.Instance.NewClipboardLine(value);
                    }

                    if (!string.IsNullOrEmpty(_lastValue))
                    {
                        _exclusionQueue.Enqueue(value);

                        while (_exclusionQueue.Count > MaxExclusionQueueSize)
                        {
                            _exclusionQueue.Dequeue();
                        }
                    }
                }

                _lastValue = value;

                Thread.Sleep(PollingRate);
            }
        })
        {
            IsBackground = true
        };
        thread.Start();
    }

    #endregion
}
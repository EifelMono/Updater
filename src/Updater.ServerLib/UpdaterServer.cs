﻿using Updater.CoreLib.udp;

namespace Updater.ServerLib;

public class UpdaterServer : IDisposable
{
    public void Dispose()
    {
    }

    public async Task NotifyUpdaterAvailableAsync()
    {
        await new CoreLib.udp.UdpClient().SendAsync(Environment.MachineName);
    }

    public Task NotifyUpdateAvailableAsync()
    {
        return Task.CompletedTask;
    }

    protected Action _onStartUpdate { get; set; }
    public UpdaterServer OnStartUpdate(Action action)
    {
        _onStartUpdate = action;
        return this;
    }

    public Task StartUpdateQueryAsync()
    {
        return Task.CompletedTask;

    }
    public Task StartUpdateAsync()
    {

        return Task.CompletedTask;
    }

}

namespace Updater.CoreLib.udp;

public enum Command
{
    Unknown,
    GrpcReconnect,

    UpdaterAvailable,
    Inventory,

    UpdateAvailable,
    ConfirmUpdate,
    StartUpdate,
}

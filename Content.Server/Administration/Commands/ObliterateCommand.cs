using Content.Server.Administration.Managers;
using Content.Server.Administration.Systems;
using Content.Shared.Administration;
using Content.Shared.Database;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Ban)]
public sealed class ObliterateCommand : LocalizedCommands
{
    [Dependency] private readonly IPlayerLocator _locator = default!;
    [Dependency] private readonly IBanManager _bans = default!;
    [Dependency] private readonly AdminSystem _adminSystem = default!;
    
    public override string Command => "obliterate";

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        string target;
        string reason;

        switch (args.Length)
        {
            case 1:
                target = args[0];
                reason = "Raider.";
                break;
            case 2:
                target = args[0];
                reason = args[1];
                break;
            default:
                shell.WriteLine(Loc.GetString("cmd-obliterate-invalid-arguments"));
                shell.WriteLine(Help);
                return;
        }
        
        var located = await _locator.LookupIdByNameOrIdAsync(target);
        var player = shell.Player;
        if (located == null)
        {
            shell.WriteError(Loc.GetString("cmd-obliterate-player"));
            return;
        }
        
        var targetUid = located.UserId;
        var targetHWid = located.LastHWId;
        
        _bans.CreateServerBan(targetUid, target, player?.UserId, null, targetHWid, null, NoteSeverity.High, reason);
        _adminSystem.Erase(targetUid);
    }
}
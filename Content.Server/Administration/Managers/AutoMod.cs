using Robust.Server.Player;
using Robust.Shared.Player;

namespace Content.Server.Administration.Managers;

public sealed class AutoMod : IAutoMod
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IBanManager _banManager = default!;

    public void RaiseHeat(ICommonSession session, uint amount)
    {
        throw new NotImplementedException();
    }

    public void LowerHeat(ICommonSession session, uint amount)
    {
        throw new NotImplementedException();
    }
}

struct PlayerHeat
{
    private uint heat;
}
using Robust.Shared.Player;

namespace Content.Server.Administration.Managers;

public sealed class AutoMod: IAutoMod
{
    public void RaiseHeat(ICommonSession session, uint amount)
    {
        throw new NotImplementedException();
    }

    public void LowerHeat(ICommonSession session, uint amount)
    {
        throw new NotImplementedException();
    }
}
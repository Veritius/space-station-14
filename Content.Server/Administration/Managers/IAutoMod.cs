using Robust.Shared.Player;

namespace Content.Server.Administration.Managers;

public interface IAutoMod
{
    /// <summary>
    /// Raises the player's 'heat'
    /// </summary>
    void RaiseHeat(ICommonSession session, uint amount);
    
    /// <summary>
    /// Lowers the player's 'heat'
    /// </summary>
    void LowerHeat(ICommonSession session, uint amount);
}

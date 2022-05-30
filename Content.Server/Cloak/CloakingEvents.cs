namespace Content.Server.Cloak
{
    public record DoDecloakEvent(EntityUid Uid, CloakingComponent Comp);
    public record DoCloakEvent(EntityUid Uid, CloakingComponent Comp);
}

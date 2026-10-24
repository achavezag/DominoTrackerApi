namespace DominoTracker.Application.Constants;

public static class RoleNames
{
    public const string SuperAdmin = "super_admin";
    public const string SessionAdmin = "session_admin";
    public const string User = "user";

    // Roles con permiso de escritura (crear/editar/eliminar), para usar en [Authorize(Roles = ...)]
    public const string AdminRoles = SuperAdmin + "," + SessionAdmin;
}

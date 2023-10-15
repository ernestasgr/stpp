using Microsoft.AspNetCore.SignalR.Protocol;

namespace backend.Data;

public static class UserRoles
{
    public const string Admin = nameof(Admin);
    public const string Viewer = nameof(Viewer);

    public static readonly IReadOnlyCollection<string> All = new[] { Admin, Viewer };
}
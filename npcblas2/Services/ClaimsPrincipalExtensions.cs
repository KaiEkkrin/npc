using System;
using System.Linq;
using System.Security.Claims;
using npcblas2.Models;

namespace npcblas2.Services
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetHandle(this ClaimsPrincipal claimsPrincipal) =>
            claimsPrincipal.FindFirstValue(ApplicationClaimType.Handle);

        public static bool HasValidHandle(this ClaimsPrincipal claimsPrincipal) =>
            !string.IsNullOrWhiteSpace(claimsPrincipal.GetHandle());

        public static string GetUserId(this ClaimsPrincipal claimsPrincipal) =>
            claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        public static bool IsAdmin(this ClaimsPrincipal claimsPrincipal) =>
            claimsPrincipal.HasClaim(c => c.Type == ApplicationClaimType.Permission && c.Value == ApplicationClaimValue.Admin);
    }
}
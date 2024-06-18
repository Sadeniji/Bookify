﻿using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;

internal sealed class AuthorizationService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;

    public AuthorizationService(ApplicationDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        var cacheKey = $"auth:roles-{identityId}";

        var cacheRoles = await _cacheService.GetAsync<UserRolesResponse>(cacheKey);

        if (cacheRoles is not null)
        {
            return cacheRoles;
        }
        var roles = await _context.Set<User>()
            .Where(user => user.IdentityId == identityId)
            .Select(user => new UserRolesResponse
            {
                Id = user.Id,
                Roles = user.Roles.ToList()
            })
            .FirstAsync();

        await _cacheService.SetAsync(cacheKey, roles);
        
        return roles;
    }

    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        var cacheKey = $"auth:roles-{identityId}";

        var cachePermissions = await _cacheService.GetAsync<HashSet<string>>(cacheKey);

        if (cachePermissions is not null)
        {
            return cachePermissions;
        }
        var permissions = await _context.Set<User>()
            .Where(user => user.IdentityId == identityId)
            .SelectMany(user => user.Roles.Select(role => role.Permissions))
            .FirstAsync();

        var permissionsSet = permissions.Select(p => p.Name).ToHashSet();

        await _cacheService.SetAsync(cacheKey, permissionsSet);
        
        return permissionsSet;
    }
}
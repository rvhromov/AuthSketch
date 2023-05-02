using AuthSketch.Exceptions;
using AuthSketch.Models.Users;
using AuthSketch.Persistence.Contexts;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AuthSketch.Services.Users;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserService(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<(int totalCount, List<GetUsersResponse>)> GetUsersAsync(GetUsersRequest request)
    {
        var (skip, take, name) = request;

        var usersQuery = _dbContext.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            usersQuery = usersQuery.Where(u => u.Name.Contains(name));
        }

        var totalCount = await usersQuery.CountAsync();

        usersQuery = usersQuery.OrderBy(u => u.Name);

        var users = await usersQuery
            .Skip(skip)
            .Take(take)
            .ProjectTo<GetUsersResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return (totalCount, users);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new NotFoundException("User not found.");

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }
}

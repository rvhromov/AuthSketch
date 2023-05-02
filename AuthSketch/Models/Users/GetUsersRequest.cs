namespace AuthSketch.Models.Users;

public sealed record GetUsersRequest(int Skip, int Take, string Name);
namespace BapMate.Infrastructure.Data;

public partial class Friend1
{
    public string OwnerId { get; set; } = string.Empty;

    // Added so UsersController can reference FriendId
    public string FriendId { get; set; } = string.Empty;
}
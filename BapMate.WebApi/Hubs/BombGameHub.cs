using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace BapMate.WebApi.Hubs;

/// <summary>
/// SignalR Hub for the Bomb Passing multiplayer game.
/// Uses a host-authoritative model: the host runs the game loop and broadcasts state.
/// </summary>
public class BombGameHub : Hub
{
    // In-memory room registry (lives as long as the server process)
    private static readonly ConcurrentDictionary<string, BombRoom> Rooms = new();

    // User name → ConnectionId mapping (online users)
    private static readonly ConcurrentDictionary<string, string> UserConnections = new();

    // Pending invites for offline users (userName → list of invites)
    private static readonly ConcurrentDictionary<string, List<GameInviteMessage>> PendingInvites = new();

    // ─── Room management ────────────────────────────────────────────────

    /// <summary>Host creates a new room.</summary>
    public async Task CreateRoom(string roomId, string hostName, string settingsJson)
    {
        if (Rooms.TryGetValue(roomId, out var existingRoom))
        {
            if (existingRoom.HostName == hostName)
            {
                existingRoom.HostConnectionId = Context.ConnectionId;
                var hostPlayer = existingRoom.Players.FirstOrDefault(p => p.Name == hostName);
                if (hostPlayer != null)
                {
                    hostPlayer.ConnectionId = Context.ConnectionId;
                }
                else
                {
                    existingRoom.Players.Add(new BombPlayer { Name = hostName, ConnectionId = Context.ConnectionId, IsHost = true });
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
                await Clients.Caller.SendAsync("RoomCreated", roomId, hostName);
                return;
            }
            else
            {
                await Clients.Caller.SendAsync("Error", "이미 존재하는 방 ID입니다.");
                return;
            }
        }

        var room = new BombRoom
        {
            RoomId = roomId,
            HostConnectionId = Context.ConnectionId,
            HostName = hostName,
            SettingsJson = settingsJson,
            Players = [new BombPlayer { Name = hostName, ConnectionId = Context.ConnectionId, IsHost = true }]
        };

        if (!Rooms.TryAdd(roomId, room))
        {
            await Clients.Caller.SendAsync("Error", "이미 존재하는 방 ID입니다.");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Caller.SendAsync("RoomCreated", roomId, hostName);
    }

    /// <summary>Guest joins an existing room.</summary>
    public async Task JoinRoom(string roomId, string playerName)
    {
        if (!Rooms.TryGetValue(roomId, out var room))
        {
            await Clients.Caller.SendAsync("Error", "존재하지 않는 방입니다.");
            return;
        }

        if (room.IsStarted && !room.Players.Any(p => p.Name == playerName))
        {
            await Clients.Caller.SendAsync("Error", "이미 시작된 게임입니다.");
            return;
        }

        // Prevent duplicate names
        if (room.Players.Any(p => p.Name == playerName))
        {
            // If same name reconnects, update connection id
            var existing = room.Players.First(p => p.Name == playerName);
            var oldConnId = existing.ConnectionId;
            existing.ConnectionId = Context.ConnectionId;

            await Groups.RemoveFromGroupAsync(oldConnId, roomId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            if (playerName == room.HostName)
            {
                room.HostConnectionId = Context.ConnectionId;
            }
        }
        else
        {
            room.Players.Add(new BombPlayer
            {
                Name = playerName,
                ConnectionId = Context.ConnectionId,
                IsHost = false
            });
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }

        // Notify everyone
        var playerNames = room.Players.Select(p => p.Name).ToList();
        await Clients.Group(roomId).SendAsync("PlayerJoined", playerName, playerNames);

        // Send current room state to the joiner
        await Clients.Caller.SendAsync("RoomState", room.HostName, playerNames, room.SettingsJson);
    }

    /// <summary>Player leaves a room.</summary>
    public async Task LeaveRoom(string roomId)
    {
        if (!Rooms.TryGetValue(roomId, out var room)) return;

        var player = room.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
        if (player == null) return;

        room.Players.Remove(player);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

        if (player.IsHost || room.Players.Count == 0)
        {
            // Host left — destroy room
            Rooms.TryRemove(roomId, out _);
            await Clients.Group(roomId).SendAsync("RoomDestroyed", "방장이 나갔습니다.");
        }
        else
        {
            var playerNames = room.Players.Select(p => p.Name).ToList();
            await Clients.Group(roomId).SendAsync("PlayerLeft", player.Name, playerNames);
        }
    }

    // ─── Game flow ──────────────────────────────────────────────────────

    /// <summary>Host starts the game.</summary>
    public async Task StartGame(string roomId, string initialStateJson)
    {
        if (!Rooms.TryGetValue(roomId, out var room)) return;
        if (room.HostConnectionId != Context.ConnectionId) return; // Only host can start

        room.IsStarted = true;
        await Clients.Group(roomId).SendAsync("GameStarted", initialStateJson);
    }

    /// <summary>Player sends movement input to host.</summary>
    public async Task SendInput(string roomId, string playerName, float dx, float dy)
    {
        if (!Rooms.TryGetValue(roomId, out var room)) return;

        // Relay input to host only
        await Clients.Client(room.HostConnectionId).SendAsync("PlayerInput", playerName, dx, dy);
    }

    /// <summary>Host broadcasts game state to all players.</summary>
    public async Task BroadcastGameState(string roomId, string gameStateJson)
    {
        if (!Rooms.TryGetValue(roomId, out var room)) return;
        if (room.HostConnectionId != Context.ConnectionId) return;

        // Send to everyone except host (host has authoritative state)
        await Clients.GroupExcept(roomId, Context.ConnectionId).SendAsync("GameStateUpdate", gameStateJson);
    }

    /// <summary>Host broadcasts game over.</summary>
    public async Task EndGame(string roomId, string loserName, int transferCount)
    {
        if (!Rooms.TryGetValue(roomId, out var room)) return;
        if (room.HostConnectionId != Context.ConnectionId) return;

        room.IsStarted = false;
        await Clients.Group(roomId).SendAsync("GameOver", loserName, transferCount);

        // Clean up room after game ends
        Rooms.TryRemove(roomId, out _);
    }

    // ─── Disconnect handling ────────────────────────────────────────────

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Clean up UserConnections
        var userName = UserConnections.FirstOrDefault(kv => kv.Value == Context.ConnectionId).Key;
        if (userName != null)
        {
            UserConnections.TryRemove(userName, out _);
        }

        // We do NOT remove players or destroy rooms immediately on disconnection.
        // Mobile backgrounding/screen locking frequently cycles the socket connection.
        // Leaving it alive allows transparent reconnection and re-joining.
        // Rooms are cleaned up when users explicitly call LeaveRoom or the game ends (EndGame).

        await base.OnDisconnectedAsync(exception);
    }

    // ─── User registration & messaging ──────────────────────────────────

    /// <summary>Register a user's name to their connection (for messaging).</summary>
    public async Task RegisterUser(string userName)
    {
        UserConnections[userName] = Context.ConnectionId;

        // Deliver any pending invites
        if (PendingInvites.TryRemove(userName, out var pendingList))
        {
            foreach (var invite in pendingList)
            {
                await Clients.Caller.SendAsync("GameInvite", invite.FromUser, invite.RoomId, invite.GameType, invite.Timestamp);
            }
        }
    }

    /// <summary>Send a game invite to a specific user.</summary>
    public async Task SendGameInvite(string roomId, string fromUser, string toUser, string gameType)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        if (UserConnections.TryGetValue(toUser, out var targetConnId))
        {
            // User is online — deliver immediately
            await Clients.Client(targetConnId).SendAsync("GameInvite", fromUser, roomId, gameType, timestamp);
        }
        else
        {
            // User is offline — queue for later
            var invite = new GameInviteMessage
            {
                FromUser = fromUser,
                RoomId = roomId,
                GameType = gameType,
                Timestamp = timestamp
            };
            PendingInvites.AddOrUpdate(toUser,
                _ => [invite],
                (_, existing) => { existing.Add(invite); return existing; });
        }

        // Notify sender about delivery status
        var delivered = UserConnections.ContainsKey(toUser);
        await Clients.Caller.SendAsync("InviteStatus", toUser, delivered);
    }

    /// <summary>Send game invites to all specified users.</summary>
    public async Task SendBulkInvites(string roomId, string fromUser, string[] toUsers, string gameType)
    {
        var results = new Dictionary<string, bool>();
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        foreach (var toUser in toUsers)
        {
            if (toUser == fromUser) continue; // Skip self

            if (UserConnections.TryGetValue(toUser, out var targetConnId))
            {
                await Clients.Client(targetConnId).SendAsync("GameInvite", fromUser, roomId, gameType, timestamp);
                results[toUser] = true;
            }
            else
            {
                var invite = new GameInviteMessage
                {
                    FromUser = fromUser,
                    RoomId = roomId,
                    GameType = gameType,
                    Timestamp = timestamp
                };
                PendingInvites.AddOrUpdate(toUser,
                    _ => [invite],
                    (_, existing) => { existing.Add(invite); return existing; });
                results[toUser] = false;
            }
        }

        // Notify sender with bulk results: {"name": delivered, ...}
        await Clients.Caller.SendAsync("BulkInviteResults", System.Text.Json.JsonSerializer.Serialize(results));
    }
}

// ─── Supporting types ───────────────────────────────────────────────────

public class BombRoom
{
    public required string RoomId { get; set; }
    public required string HostConnectionId { get; set; }
    public required string HostName { get; set; }
    public string SettingsJson { get; set; } = "{}";
    public List<BombPlayer> Players { get; set; } = [];
    public bool IsStarted { get; set; }
}

public class BombPlayer
{
    public required string Name { get; set; }
    public required string ConnectionId { get; set; }
    public bool IsHost { get; set; }
}

public class GameInviteMessage
{
    public required string FromUser { get; set; }
    public required string RoomId { get; set; }
    public required string GameType { get; set; }
    public long Timestamp { get; set; }
}

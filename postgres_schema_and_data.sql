-- ===========================================================
-- BapMate V2 GameRooms Table Creation Schema
-- This script creates the 'gamerooms' table in lowercase.
-- ===========================================================

-- 1. Create the gamerooms table if it does not already exist
CREATE TABLE IF NOT EXISTS gamerooms (
    -- id: The unique 6-digit room code (Primary Key)
    id VARCHAR(50) NOT NULL,
    
    -- hostname: The name of the player who hosted/created the room
    hostname VARCHAR(100) NOT NULL,
    
    -- settingsjson: Game settings (timer, game mode, etc.) stored in JSON format
    settingsjson TEXT NOT NULL DEFAULT '{}',
    
    -- playersjson: The list of player names currently in the room stored in JSON format
    playersjson TEXT NOT NULL DEFAULT '[]',
    
    -- isstarted: Boolean flag indicating if the game has started
    isstarted BOOLEAN NOT NULL DEFAULT FALSE,
    
    -- isended: Boolean flag indicating if the game session has completed/ended
    isended BOOLEAN NOT NULL DEFAULT FALSE,
    
    -- createdat: Timestamp representing when the room was created
    createdat TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- updatedat: Timestamp representing when the room state was last modified
    updatedat TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Primary Key Constraint
    CONSTRAINT PK_gamerooms PRIMARY KEY (id)
);

-- 2. Create an index on the isended and isstarted columns to optimize queries
CREATE INDEX IF NOT EXISTS IX_gamerooms_status ON gamerooms (isstarted, isended);

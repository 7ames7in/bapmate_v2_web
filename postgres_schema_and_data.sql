-- ===========================================================
-- BapMate V2 Table Creation Schema
-- This script creates the 'gamerooms', 'users', and 'friends' tables in lowercase.
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

-- 3. Create the users table in lowercase
CREATE TABLE IF NOT EXISTS users (
    id VARCHAR(100) NOT NULL,
    username VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL,
    passwordhash VARCHAR(200) NOT NULL,
    name VARCHAR(100) NOT NULL,
    gender VARCHAR(20) NOT NULL,
    birthyear INT NOT NULL,
    avatar VARCHAR(300) NOT NULL DEFAULT '',
    bio VARCHAR(500) NOT NULL DEFAULT '',
    phone VARCHAR(50) NOT NULL,
    carrier VARCHAR(50) NOT NULL DEFAULT '',
    reliabilityscore DOUBLE PRECISION NOT NULL DEFAULT 88.0,
    walletbalance NUMERIC(18,2) NOT NULL DEFAULT 50000.00,
    escrowbalance NUMERIC(18,2) NOT NULL DEFAULT 0.00,
    badgesjson TEXT NOT NULL DEFAULT '[]',
    matchpreferencesjson TEXT NOT NULL DEFAULT '{}',
    defaultgamesettingsjson TEXT NOT NULL DEFAULT '{}',
    createdat TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updatedat TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    password VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT PK_users PRIMARY KEY (id)
);

-- 4. Create the friends table in lowercase
CREATE TABLE IF NOT EXISTS friends (
    id VARCHAR(100) NOT NULL,
    ownerid VARCHAR(100) NOT NULL,
    name VARCHAR(100) NOT NULL,
    avatar VARCHAR(300) NOT NULL DEFAULT '',
    trustlevel INT NOT NULL DEFAULT 3,
    lastmeal VARCHAR(100) NOT NULL DEFAULT '',
    tagsjson TEXT NOT NULL DEFAULT '[]',
    memo VARCHAR(500),
    phone VARCHAR(50),
    identifier VARCHAR(100),
    CONSTRAINT PK_friends PRIMARY KEY (id, ownerid)
);

-- PostgreSQL Schema and Mock Data Initialization for BapMate V2
-- Target DB: bapmatedb, User: n8n_user on srv1651644.hstgr.cloud

-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Clean up existing tables if they exist (order is important due to foreign keys)
DROP TABLE IF EXISTS game_invitations CASCADE;
DROP TABLE IF EXISTS game_participants CASCADE;
DROP TABLE IF EXISTS game_sessions CASCADE;
DROP TABLE IF EXISTS user_social_accounts CASCADE;
DROP TABLE IF EXISTS "Friends" CASCADE;
DROP TABLE IF EXISTS users CASCADE;

-- 1. Create users table
CREATE TABLE users (
    id VARCHAR(100) PRIMARY KEY DEFAULT uuid_generate_v4()::text,
    username VARCHAR(50) UNIQUE NULL,
    email VARCHAR(100) UNIQUE NULL,
    password_hash VARCHAR(255) NULL,
    name VARCHAR(100) NOT NULL,
    gender VARCHAR(10) NOT NULL,             -- 'male' / 'female' / 'other'
    birth_year INT NOT NULL,                 -- e.g., 1995
    avatar VARCHAR(255) NULL,
    bio VARCHAR(255) NULL,
    phone VARCHAR(20) NULL,
    carrier VARCHAR(20) NULL,
    reliability_score DOUBLE PRECISION DEFAULT 0.0,
    wallet_balance DECIMAL(18, 2) DEFAULT 0.00,
    escrow_balance DECIMAL(18, 2) DEFAULT 0.00,
    badges_json JSONB DEFAULT '[]',
    match_preferences_json JSONB DEFAULT '{}',
    default_game_settings_json JSONB DEFAULT '{}',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- 2. Create user_social_accounts table
CREATE TABLE user_social_accounts (
    id BIGSERIAL PRIMARY KEY,
    user_id VARCHAR(100) NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    provider VARCHAR(20) NOT NULL,                     -- 'google', 'kakao', 'naver'
    provider_user_id VARCHAR(255) NOT NULL,            -- Provider's unique user ID
    connected_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_provider_and_id UNIQUE (provider, provider_user_id)
);

-- 2b. Create Friends table
CREATE TABLE "Friends" (
    "Id" VARCHAR(100) PRIMARY KEY,
    "OwnerId" VARCHAR(100) NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    "Name" VARCHAR(100) NOT NULL,
    "Avatar" VARCHAR(255) NOT NULL DEFAULT '',
    "TrustLevel" INT NOT NULL DEFAULT 0,
    "LastMeal" VARCHAR(255) NOT NULL DEFAULT '',
    "TagsJson" TEXT NOT NULL DEFAULT '[]',
    "Memo" TEXT NULL,
    "Phone" VARCHAR(50) NULL,
    "Identifier" VARCHAR(100) NULL
);

-- 3. Create game_sessions table
CREATE TABLE game_sessions (
    id VARCHAR(100) PRIMARY KEY,
    game_type VARCHAR(50) NOT NULL,                    -- 'bomb', 'roulette'
    status VARCHAR(20) NOT NULL DEFAULT 'waiting',     -- 'waiting', 'playing', 'completed', 'cancelled'
    host_id VARCHAR(100) NULL REFERENCES users(id) ON DELETE SET NULL,
    winner_id VARCHAR(100) NULL REFERENCES users(id) ON DELETE SET NULL,
    wager VARCHAR(255) NULL,
    max_participants INT NULL,
    description VARCHAR(255) NULL,
    game_mode VARCHAR(50) NULL,
    game_result_json JSONB NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    started_at TIMESTAMP WITH TIME ZONE NULL,
    ended_at TIMESTAMP WITH TIME ZONE NULL
);

-- 4. Create game_participants table
CREATE TABLE game_participants (
    game_id VARCHAR(100) NOT NULL REFERENCES game_sessions(id) ON DELETE CASCADE,
    user_id VARCHAR(100) NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    role VARCHAR(20) NOT NULL DEFAULT 'player',         -- 'host', 'player', 'spectator'
    score INT DEFAULT 0,
    is_winner BOOLEAN DEFAULT FALSE,
    joined_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (game_id, user_id)
);

-- 5. Create game_invitations table
CREATE TABLE game_invitations (
    id VARCHAR(100) PRIMARY KEY DEFAULT uuid_generate_v4()::text,
    game_id VARCHAR(100) NOT NULL REFERENCES game_sessions(id) ON DELETE CASCADE,
    inviter_id VARCHAR(100) NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    invitee_id VARCHAR(100) NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    status VARCHAR(20) NOT NULL DEFAULT 'pending',     -- 'pending', 'accepted', 'rejected', 'expired'
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    responded_at TIMESTAMP WITH TIME ZONE NULL
);

-- Insert Mock Users
-- Using hardcoded UUIDs so references in mock data are predictable
INSERT INTO users (id, username, email, password_hash, name, gender, birth_year, reliability_score) VALUES
('usr_bapmate_uuid', 'bapmate', 'bapmate@example.com', 'password123', '밥메이트', 'male', 1995, 99.0),
('usr_admin_uuid', 'admin', 'admin@example.com', 'adminpassword', '관리자', 'female', 1990, 100.0),
('usr_user1_uuid', 'user1', 'user1@example.com', 'user1password', '김철수', 'male', 1998, 85.5);

-- Insert a Mock Game Session (Completed)
INSERT INTO game_sessions (id, game_type, status, host_id, winner_id, wager, max_participants, created_at, started_at, ended_at) VALUES
('game_mock_1', 'bomb', 'completed', 'usr_bapmate_uuid', 'usr_admin_uuid', '커피 쏘기', 3, NOW() - INTERVAL '1 hour', NOW() - INTERVAL '50 minutes', NOW() - INTERVAL '40 minutes');

-- Insert Participants for the game
INSERT INTO game_participants (game_id, user_id, role, score, is_winner) VALUES
('game_mock_1', 'usr_bapmate_uuid', 'host', 120, FALSE),
('game_mock_1', 'usr_admin_uuid', 'player', 250, TRUE),
('game_mock_1', 'usr_user1_uuid', 'player', 80, FALSE);

-- Insert a Mock Game Invitation (Pending)
INSERT INTO game_invitations (id, game_id, inviter_id, invitee_id, status, created_at) VALUES
('invite_mock_1', 'game_mock_1', 'usr_bapmate_uuid', 'usr_user1_uuid', 'accepted', NOW() - INTERVAL '55 minutes');

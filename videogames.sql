-- 1. Adatbázis létrehozása UTF-8 és COLLATE beállítással
CREATE DATABASE videogames
CHARACTER SET utf8
COLLATE utf8_hungarian_ci;

USE videogames;

-- 2. Developer tábla
CREATE TABLE Developer (
    DeveloperID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Country VARCHAR(50)
);

-- 3. Platform tábla
CREATE TABLE Platform (
    PlatformID INT AUTO_INCREMENT PRIMARY KEY,
    PlatformName VARCHAR(100) NOT NULL,
    Manufacturer VARCHAR(100)
);

-- 4. Game tábla
CREATE TABLE Game (
    GameID INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(150) NOT NULL,
    ReleaseYear YEAR,
    DeveloperID INT,
    FOREIGN KEY (DeveloperID) REFERENCES Developer(DeveloperID)
);

-- 5. Review tábla
CREATE TABLE Review (
    ReviewID INT AUTO_INCREMENT PRIMARY KEY,
    UserName VARCHAR(100) NOT NULL,
    GameID INT,
    Rating INT CHECK (Rating BETWEEN 1 AND 10),
    Comment TEXT,
    FOREIGN KEY (GameID) REFERENCES Game(GameID)
);

-- 6. Feltöltés: Developer
INSERT INTO Developer (Name, Country) VALUES
('Rockstar Games', 'USA'),         -- GTA V
('Mojang Studios', 'Sweden'),     -- Minecraft
('Valve', 'USA'),                 -- CS:GO
('Epic Games', 'USA');            -- Fortnite

-- 7. Feltöltés: Platform
INSERT INTO Platform (PlatformName, Manufacturer) VALUES
('PlayStation 5', 'Sony'),
('Xbox Series X', 'Microsoft'),
('Nintendo Switch', 'Nintendo'),
('PC', 'Various');

-- 8. Feltöltés: Game
INSERT INTO Game (Title, ReleaseYear, DeveloperID) VALUES
('Grand Theft Auto V', 2013, 1),
('Minecraft', 2011, 2),
('Counter-Strike: Global Offensive', 2012, 3),
('Fortnite', 2017, 4);

-- 9. Feltöltés: Review
INSERT INTO Review (UserName, GameID, Rating, Comment) VALUES
('gamer_joe', 1, 9, 'A GTA Online hatalmas élmény.'),
('anita23', 2, 10, 'Kreatív, pihentető, imádom!'),
('link_fan', 3, 8, 'Régen jobb volt, de még most is király.'),
('darklord95', 4, 7, 'Élvezetes, de néha túl pörgős.'),
('zsolesz', 1, 10, 'GTA mindig a szívem csücske.');

-- 10. Kapcsoló tábla
CREATE TABLE GamePlatform (
    GameID INT,
    PlatformID INT,
    PRIMARY KEY (GameID, PlatformID),
    FOREIGN KEY (GameID) REFERENCES Game(GameID),
    FOREIGN KEY (PlatformID) REFERENCES Platform(PlatformID)
);

-- 11. Feltöltés: Kapcsoló tábla
-- GTA V → PC, PS5
INSERT INTO GamePlatform (GameID, PlatformID) VALUES
(1, 1), -- PS5
(1, 4); -- PC

-- Minecraft → PC, Switch
INSERT INTO GamePlatform (GameID, PlatformID) VALUES
(2, 3), -- Switch
(2, 4); -- PC

-- CS:GO → PC
INSERT INTO GamePlatform (GameID, PlatformID) VALUES
(3, 4); -- PC

-- Fortnite → PC, PS5, Xbox
INSERT INTO GamePlatform (GameID, PlatformID) VALUES
(4, 1), -- PS5
(4, 2), -- Xbox
(4, 4); -- PC
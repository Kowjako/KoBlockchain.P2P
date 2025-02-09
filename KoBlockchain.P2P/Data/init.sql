CREATE TABLE Blocks (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,           
    Hash TEXT NOT NULL,     
    Data BLOB NOT NULL,
    PreviousBlockHash TEXT,                      
    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP   
);

CREATE TABLE LatestBlock (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Hash TEXT NOT NULL,
    Data BLOB NOT NULL,
    PreviousBlockHash TEXT,
    Timestamp DATETIME NOT NULL
);

CREATE TABLE Transactions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,           
    BlockId INTEGER,                               
    SenderAddress TEXT,                            
    ReceiverAddress TEXT,                         
    Amount DECIMAL(20, 8),
    CreatedAt DATETIME NOT NULL
);

CREATE TABLE Addresses (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,           
    Address TEXT NOT NULL,                          
    Balance DECIMAL(20, 8) DEFAULT 0                
);

CREATE TABLE Peers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PeerUrl TEXT NOT NULL
);
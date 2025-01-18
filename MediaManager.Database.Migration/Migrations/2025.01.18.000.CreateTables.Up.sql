CREATE TABLE SiteProviders (
    Id TEXT PRIMARY KEY,
    [Name] TEXT,
    Domain TEXT
);

CREATE TABLE Channels (
    Id TEXT PRIMARY KEY,
    SiteId TEXT,
    [Url] TEXT,
    [Name] TEXT,
    [Description] TEXT,
    BlackListed INT,
    IsAddedBySingleVideo INT,
    NotFound INT,
    LastCheckedOut INT,
    LastActivityDate INT,
    SiteProvider_Id TEXT,
    FOREIGN KEY(SiteProvider_Id) REFERENCES SiteProviders(Id)
);

CREATE TABLE ChannelThumbnails (
    Channel_Id TEXT PRIMARY KEY,
    [Url] TEXT,
    Width INT,
    Height INT,
    FOREIGN KEY(Channel_Id) REFERENCES Channels(Id)
);

CREATE TABLE Metadatas (
    Id TEXT PRIMARY KEY,
    SiteId TEXT,
    [Url] TEXT,
    Title TEXT,
    Descritpion TEXT,
    [Status] TEXT,
    IsBeingDownload INT,
    PublishedAt INT,
    Channel_Id TEXT,
    FOREIGN KEY(Channel_Id) REFERENCES Channels(Id)
);

CREATE TABLE MetadataThumbnails (
    Metadata_Id TEXT PRIMARY KEY,
    [Url] TEXT,
    Width INT,
    Height INT,
    FOREIGN KEY(Metadata_Id) REFERENCES Metadatas(Id)
);
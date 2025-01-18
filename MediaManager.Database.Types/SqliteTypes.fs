namespace MediaManager.Database.Types

module SqliteTypes =
    type SqliteGuid = string
    type SqliteUnixTime = int
    type SqliteMetadataStatus = New | ToDownload | Wait | Ignore | Downloaded

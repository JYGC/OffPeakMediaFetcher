namespace MediaManager.Database.Types

open System

module SqliteTypes =
    type SqliteGuid = string
    type SqliteUnixTime = int
    type SqliteMetadataStatus = New | ToDownload | Wait | Ignore | Downloaded

    let convertToSqliteGuid (guid: Guid): SqliteGuid = guid.ToString()
    let convertFromSqliteGuid (sqliteGuid: SqliteGuid): Guid = new Guid(sqliteGuid)

    let convertToSqliteUnixTime (datetime: DateTime): SqliteUnixTime =
        datetime.Subtract(new DateTime(1970, 1, 1)).Seconds
    let convertFromSqliteUnixTime (sqliteUnixTime: SqliteUnixTime): DateTime =
        (new DateTime(1970, 1, 1)).AddSeconds(sqliteUnixTime)

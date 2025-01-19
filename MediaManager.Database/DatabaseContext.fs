namespace MediaManager.Database

open System.Data.SQLite
open Dapper.FSharp.SQLite

module DatabaseContext =
    OptionTypes.register()
    let databaseFilePath = "C:\\Users\\jygcn\\AppData\\Local\\OffPeakMediaFetcherDev\\Databases\\test.db"
    let databaseConnection = new SQLiteConnection($"Data Source={databaseFilePath}")
    let databaseTables = new DatabaseSchemas.DatabaseTables()
namespace MediaManager.Database

open System.Data.SQLite
open Dapper.FSharp.SQLite

module DatabaseContext =
    OptionTypes.register()
    let databaseConnection = new SQLiteConnection("Data Source=C:\\Users\\jygcn\\AppData\\Local\\OffPeakMediaFetcherDev\\Databases\\test.db")
    let databaseTables = new DatabaseSchemas.DatabaseTables()
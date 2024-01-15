using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using LiteDB;
using LiteDB.Engine;

[Tool]
public partial class LiteDBNode : Node, ILiteDatabase, IDisposable
{
    private LiteDatabase _instance;
    private string _saveFilePath = string.Empty;

    public LiteDBNode()
    {
        TimeoutInSeconds = 60;
        LimitSize = 100000000;
        CheckpointSize = 1000;
    }

    /// <summary>
    /// Gets and sets the path where the database will be saved and loaded from.
    /// This should be Guodot paths such as user://database.db.
    /// </summary>
    [Export(PropertyHint.SaveFile, "*.db")]
    public string SaveFilePath 
    { 
        get => _saveFilePath; 
        set
        {
            if (_instance is not null)
            {
                throw new InvalidOperationException("The path cannot be modified once the database has been instanced.");
            }
            _saveFilePath = value;
        }
    }

    /// <summary>
    /// Transforms the SaveFilePath to a system file path.
    /// </summary>
    public string GlobalPath => ProjectSettings.GlobalizePath(SaveFilePath);
    public BsonMapper Mapper => _instance.Mapper;

    public ILiteStorage<string> FileStorage => _instance.FileStorage;


    private double _timeout = 60;

    /// <summary>
    /// Configures the timeout for database access in seconds.
    /// </summary>
    [Export(PropertyHint.Range, "0,300,or_greater,suffix:s")]
    public double TimeoutInSeconds
    { 
        get => _instance?.Timeout.TotalSeconds ?? _timeout; 
        set 
        {
            _timeout = value;
            if (_instance is not null)
            {
                _instance.Timeout = TimeSpan.FromSeconds(value);
            }
        }
    }

    public int UserVersion 
    { 
        get => _instance?.UserVersion ?? -1; 
        set 
        {
            if (_instance is not null)
            {
                _instance.UserVersion = value;
            }
        }
    }

    public TimeSpan Timeout 
    { 
        get => _instance.Timeout; 
        set => _instance.Timeout = value; 
    }
    private bool _utcDate;
    [Export]
    public bool UtcDate 
    { 
        get => _instance?.UtcDate ?? _utcDate; 
        set 
        {
            if (_instance is not null)
            {
                _instance.UtcDate = value;
            }
            _utcDate = value;
        }
    }

    private long _limitSize;

    /// <summary>
    /// Get/Set database limit size (in bytes). New value must be equals or larger than current database size.
    /// </summary>
    [Export(PropertyHint.Range, "0,1000000000,or_greater,suffix:bytes")]
    public long LimitSize 
    { 
        get => _instance?.LimitSize ?? _limitSize; 
        set 
        {
            if (_instance is not null)
            {
                _instance.LimitSize = value;
            }
            _limitSize = value;
        }  
    }

    private int _checkpointSize = 1000;
    /// <summary>
    /// Get/Set in how many pages (8 Kb each page) log file will auto checkpoint (copy
    //     from log file to data file). Use 0 to manual-only checkpoint (and no checkpoint
    //     on dispose) Default: 1000 pages
    /// </summary>
    [Export(PropertyHint.Range, "0,100000,or_greater,suffix:Pages")]
    public int CheckpointSize 
    { 
        get => _instance?.CheckpointSize ?? _checkpointSize; 
        set 
        {
            if (_instance is not null)
            {
                _instance.CheckpointSize = value;
            }
            _checkpointSize = value;
        } 
    }
    
    public Collation Collation => _instance.Collation;

    public override void _Ready()
    {
        _instance = new LiteDatabase(GlobalPath, null)
        {
            Timeout = TimeSpan.FromSeconds(TimeoutInSeconds),
            UtcDate = UtcDate,
            CheckpointSize = CheckpointSize,
            LimitSize = LimitSize
        };
    }

    public override void _ExitTree()
    {
        _instance?.Dispose();
    }

    public ILiteCollection<T> GetCollection<T>(string name, BsonAutoId autoId = BsonAutoId.ObjectId)
    {
        return _instance.GetCollection<T>(name, autoId);
    }

    public ILiteCollection<T> GetCollection<T>()
    {
        return _instance.GetCollection<T>();
    }

    public ILiteCollection<T> GetCollection<T>(BsonAutoId autoId)
    {
        return _instance.GetCollection<T>(autoId);
    }

    public ILiteCollection<BsonDocument> GetCollection(string name, BsonAutoId autoId = BsonAutoId.ObjectId)
    {
        return _instance.GetCollection(name, autoId);
    }

    public bool BeginTrans()
    {
        return _instance.BeginTrans();
    }

    public bool Commit()
    {
        return _instance.Commit();
    }

    public bool Rollback()
    {
        return _instance.Rollback();
    }

    public ILiteStorage<TFileId> GetStorage<TFileId>(string filesCollection = "_files", string chunksCollection = "_chunks")
    {
        return _instance.GetStorage<TFileId>(filesCollection, chunksCollection);
    }

    public IEnumerable<string> GetCollectionNames()
    {
        return _instance.GetCollectionNames();
    }

    public bool CollectionExists(string name)
    {
        return _instance.CollectionExists(name);
    }

    public bool DropCollection(string name)
    {
        return _instance.DropCollection(name);
    }

    public bool RenameCollection(string oldName, string newName)
    {
        return _instance.RenameCollection(oldName, newName);
    }

    public IBsonDataReader Execute(TextReader commandReader, BsonDocument parameters = null)
    {
        return _instance.Execute(commandReader, parameters);
    }

    public IBsonDataReader Execute(string command, BsonDocument parameters = null)
    {
        return _instance.Execute(command, parameters);
    }

    public IBsonDataReader Execute(string command, params BsonValue[] args)
    {
        return _instance.Execute(command, args);
    }

    public void Checkpoint()
    {
        _instance?.Checkpoint();
    }

    public long Rebuild()
    {
        return _instance.Rebuild(null);
    }

    public long Rebuild(RebuildOptions options)
    {
        return _instance.Rebuild(options);
    }

    public BsonValue Pragma(string name)
    {
        return _instance.Pragma(name);
    }

    public BsonValue Pragma(string name, BsonValue value)
    {
        return _instance.Pragma(name, value);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using Godot;
using LiteDB;
using LiteDB.Engine;

[Tool]
public partial class LiteDBNode : Node, ILiteDatabase, IDisposable
{
    private string _saveFilePath = string.Empty;

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

    public string GlobalPath => ProjectSettings.GlobalizePath(SaveFilePath);

    public BsonMapper Mapper => _instance.Mapper;

    public ILiteStorage<string> FileStorage => _instance.FileStorage;

    public int UserVersion { get => _instance.UserVersion; set => _instance.UserVersion = value; }
    public TimeSpan Timeout { get => _instance.Timeout; set => _instance.Timeout = value; }
    public bool UtcDate { get => _instance.UtcDate; set => _instance.UtcDate = value; }
    public long LimitSize { get => _instance.LimitSize; set => _instance.LimitSize = value; }
    public int CheckpointSize { get => _instance.CheckpointSize; set => _instance.CheckpointSize = value; }

    public Collation Collation => ((ILiteDatabase)_instance).Collation;

    private LiteDatabase _instance;

    public override void _Ready()
    {
        _instance = new LiteDatabase(GlobalPath);   
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
using Godot;
using LiteDB;
using System;
using System.Linq;

public partial class Demonstartion : Control
{
    private Message _currentMessage;
    private ILiteCollection<Message> _collection;
    [Export(PropertyHint.NodeType, "LiteDBNode")]
    public LiteDBNode Database { get; set; }

    
    [Export(PropertyHint.NodeType, "TextEdit")]
    public TextEdit Input { get; set; }

    public override void _Ready()
    {
        _collection = Database.GetCollection<Message>("Messages", BsonAutoId.ObjectId);
    }

    public void SaveInput()
    {
        var newID = _collection.Insert(new Message(0, Input.Text));
        _currentMessage = new Message(newID, Input.Text);
    }

    public void LoadNext()
    {
        if (_collection.Count() == 0)
        {
            _currentMessage = null;
            Input.Text = "No messages are available.";
            return;
        }

        if (_currentMessage is null )
        {
            _currentMessage = _collection.FindById(_collection.Min());
            Input.Text = _currentMessage.Text;
            return;
        }
        
        _currentMessage = _collection.FindOne(k => k.Id > _currentMessage.Id);
        Input.Text = (_currentMessage is null) ? 
            "No more messages available. Press \"Next\" to return to the start." :
            _currentMessage.Text;
    }

    public void LoadPrevious()
    {
        if (_collection.Count() == 0)
        {
            _currentMessage = null;
            Input.Text = "No messages are available.";
            return;
        }

        if (_currentMessage is null)
        {
            _currentMessage = _collection.FindById(_collection.Max());
            Input.Text = _currentMessage.Text;
            return;
        }
        
        _currentMessage = _collection.Find(k => k.Id < _currentMessage.Id).Last();
        Input.Text = (_currentMessage is null) ? 
            "No more messages available. Press \"Previous\" to go return to the final message." :
            _currentMessage.Text;
    }
}

public sealed record Message(int Id, string Text);

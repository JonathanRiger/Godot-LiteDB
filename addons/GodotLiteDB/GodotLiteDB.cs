#if TOOLS
using Godot;
using System;

[Tool]
public partial class GodotLiteDB : EditorPlugin
{
	private const string PLUGIN_NAME = "LiteDBNode";
	public override void _EnterTree()
	{
        var script = GD.Load<Script>("res://addons/GodotLiteDB/LiteDBNode.cs");
        var texture = GD.Load<Texture2D>("res://addons/GodotLiteDB/Icon.png");
        AddCustomType(PLUGIN_NAME, "Node", script, texture);
	}

	public override void _ExitTree()
	{
		RemoveCustomType(PLUGIN_NAME);
	}
}
#endif

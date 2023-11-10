 /*
 @author Alexander Venezia (Blunderguy)
*/

using Combat;
using Data;
using Godot;
using System;
using System.Collections.Generic;

public partial class MasterScene : Node
{
    [Export] private Godot.Collections.Array<string> _packedSceneUIDs = new();
    [Export] private string _defaultSceneUID;
    [Export] private string _combatSceneUID;
    private Dictionary<string, Node> _loadedScenes;
    private string _activeScene = "";
    private string _lastScene;

    public override void _Ready() 
    {
        _loadedScenes = new();

        LoadScene(_combatSceneUID);
        LoadScene(_defaultSceneUID);
        
		ActivateScene(_defaultSceneUID, false);
		_activeScene = _defaultSceneUID;
		_lastScene = "";
    }

    public void LoadScene(string sceneUID)
    {
        if (_loadedScenes.ContainsKey(sceneUID))
            throw new Exception("ERROR: Attempted to load scene which is already loaded");
        PackedScene scene = GD.Load<PackedScene>(sceneUID);
        _loadedScenes.Add(sceneUID, scene.Instantiate());
    }

    public void ActivateScene(string uid, bool includeLoad)
    {
        if (!_loadedScenes.ContainsKey(uid))
        {
            if (!includeLoad)
                throw new Exception("ERROR: Attempted to active scene which has not been loaded");
            LoadScene(uid);
            GD.Print("Loading as activation");
        }

        _lastScene = _activeScene;
        if (_activeScene != "")
            RemoveChild(_loadedScenes[_activeScene]);
        _activeScene = uid;
        AddChild(_loadedScenes[_activeScene]);
    }

    public void ActivateSceneAndWipeCurrent(string destUID)
    {
        ActivateScene(destUID, false);
        WipeScene(_lastScene);
        _lastScene = "";
    }

    public void ActivatePreviousScene()
    {
        ActivateScene(_lastScene, false);
    }

    public void WipeScene(string uid)
    {
        if (!_loadedScenes.ContainsKey(uid))
            throw new Exception("ERROR: Attempted to delete scene which has not been loaded");

        _loadedScenes[uid].QueueFree();
        _loadedScenes.Remove(uid);
    }
}

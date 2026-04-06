using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(WebsocketManager))]
public class WebsocketManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        WebsocketManager socket = target as WebsocketManager;
        if (GUILayout.Button("Start Game"))
        {
            socket.LaunchGame();
        }
    }
}

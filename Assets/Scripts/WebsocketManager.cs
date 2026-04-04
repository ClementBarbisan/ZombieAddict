using System;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.InputSystem;

public class WebsocketManager : MonoBehaviour
{
    [Serializable]
    struct Buttons
    {
        public bool a;
        public bool b;
    }
    
    [Serializable]
    struct Joystick
    {
        public float x;
        public float y;
    }
    
    [Serializable]
    struct InputWebSocket
    {
        public string input;
        public string clientId;
        public string nickName;
        public Joystick joystick;
        public Buttons buttons;
    }
    [Serializable]
    struct HelloMessage
    {
        public string type;
        public string clientId;
        public string nickname;
    }
    
    [SerializeField] private string _ip = "192.168.1.127";
    [SerializeField] private string _port = "8080";
    private WebSocket _websocket;
    private Dictionary<string, PlayerController> _players = new Dictionary<string, PlayerController>();
    private PlayersManager _playersManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        Application.runInBackground = true; // Recommended for WebGL

        _websocket = new WebSocket("ws://" + _ip + ":" + _port);

        _websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            var hello = JsonUtility.ToJson(new HelloMessage
            {
                type = "hello",
                clientId = "unity-client",
                nickname = "Unity"
            });
            _websocket.SendText(hello);
        };
        _websocket.OnError += (e) => Debug.Log("Error! " + e);
        _websocket.OnClose += (code) => Debug.Log("Connection closed!");

        _websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received: " + message);
            InputWebSocket player = JsonUtility.FromJson<InputWebSocket>(message);
            if (_players.ContainsKey(player.clientId))
            {
                _players.Add(player.clientId, _playersManager.CreateNewPlayer(player.nickName));
            }
            else
            {
                _players[player.clientId].HandleInputs(new Vector2(player.joystick.x, player.joystick.y),
                    player.buttons.a, player.buttons.b);
            }
        };

        //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        await _websocket.Connect();
    }

    async void SendWebSocketMessage()
    {
        if (_websocket.State == WebSocketState.Open)
        {
            //await websocket.Send(new byte[] { 10, 20, 30 });
            await _websocket.SendText("plain text message");
        }
    }
    
    private async void OnApplicationQuit()
    {
        await _websocket.Close();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

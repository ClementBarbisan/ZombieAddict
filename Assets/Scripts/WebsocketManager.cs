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
    struct Input
    {
        public Joystick joystick;
        public Buttons buttons;
    }
    
    [Serializable]
    struct PlayerInput
    {
        public string clientId;
        public string nickname;
        public string ready;
        public Input input;
    }
    
    [Serializable]
    struct InputWebSocket
    {
        public string type;
        public string phase;
        public PlayerInput[] players;
        public string t;
    }
    
    [Serializable]
    struct Player
    {
        public string clientId;
        public string clientType;
        public string nickname;
        public string avatar;
        public string ready;
    }
    
    [Serializable]
    struct JoinMessage
    {
        public string type;
        public Player player;
        public string phase;
        public string t;
    }
    
    [Serializable]
    struct HelloMessage
    {
        public string type;
        public string clientId;
        public string nickname;
        public string clientType;
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
        _playersManager = FindAnyObjectByType<PlayersManager>();
        _websocket = new WebSocket("ws://" + _ip + ":" + _port);

        _websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            var hello = JsonUtility.ToJson(new HelloMessage
            {
                type = "hello",
                clientId = "unity-client",
                nickname = "Unity",
                clientType = "unity"
            });
            _websocket.SendText(hello);
        };
        _websocket.OnError += (e) => Debug.Log("Error! " + e);
        _websocket.OnClose += (code) => Debug.Log("Connection closed!");

        _websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received: " + message);
            
            /*if (message.Contains("player_joined"))
            {
                JoinMessage player = JsonUtility.FromJson<JoinMessage>(message);
                _players.Add(player.player.clientId, _playersManager.CreateNewPlayer(player.player.nickname));
            }
            else*/
            if (message.Contains("inputs_snapshot"))
            {
                InputWebSocket player = JsonUtility.FromJson<InputWebSocket>(message);
                foreach (PlayerInput playerInput in player.players)
                {
                    if (!_players.ContainsKey(playerInput.clientId))
                    {
                        _players.Add(playerInput.clientId, _playersManager.CreateNewPlayer(playerInput.nickname));
                    }
                    _players[playerInput.clientId].HandleInputs(new Vector2(playerInput.input.joystick.x, playerInput.input.joystick.y),
                        playerInput.input.buttons.a, playerInput.input.buttons.b);
                }
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

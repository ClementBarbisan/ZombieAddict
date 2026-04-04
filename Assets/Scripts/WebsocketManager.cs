using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.SceneManagement;

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
    struct Position
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
    struct InputZombie
    {
        public string type;
        public string clientId;
        public string role;
        public Position position;
        public string troupes;
    }
    
    [Serializable]
    struct InputSurvivor
    {
        public string type;
        public string clientId;
        public Joystick joystick;
        public Buttons buttons;
    }
    
    [Serializable]
    struct Player
    {
        public string clientId;
        public string clientType;
        public string nickname;
        public string avatar;
        public string ready;
        public string role;
    }
    
    [Serializable]
    struct JoinLeaveMessage
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

    [Serializable]
    struct InfosPlayer
    {
        public string clientId;
        public int damages;
        public int enemyKilled;
        public float walkDistance;
        public int shootFired;
        public int shootSuccessfull;
        public float accuracy;
    }
    
    public static WebsocketManager Instance; 
    [SerializeField] private string _ip = "192.168.1.127";
    [SerializeField] private string _port = "8080";
    [SerializeField] private float _timeToStart = 5;
    [SerializeField] private string _sceneName = "Game";
    //[SerializeField] private Material _fog;
    private GraphicsBuffer _bufferPos;
    private WebSocket _websocket;
    private Dictionary<string, PlayerController> _players = new Dictionary<string, PlayerController>();
    private Dictionary<string, Player> _playersAbstract = new Dictionary<string, Player>();
    private Dictionary<string, InfosPlayer> _playersInfos = new Dictionary<string, InfosPlayer>();
    private Vector3[] _positionsPlayer;
    private PlayersManager _playersManager;
    private ZombieManager _zombieManager;
    private bool _gameLaunched;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        Application.runInBackground = true; // Recommended for WebGL
        _playersManager = FindAnyObjectByType<PlayersManager>();
        _websocket = new WebSocket("ws://" + _ip + ":" + _port);
        _zombieManager = FindAnyObjectByType<ZombieManager>();
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
            if (message.Contains("player_joined"))
            {
                JoinLeaveMessage player = JsonUtility.FromJson<JoinLeaveMessage>(message);
                if (!_playersAbstract.ContainsKey(player.player.clientId) && player.player.nickname != "Unity")
                {
                    _playersAbstract.Add(player.player.clientId, player.player);
                    string base64 = player.player.avatar.Replace("data:image/jpeg;base64,", "");
                    try
                    {
                        byte[] tmpBytes = Convert.FromBase64String(base64);
                        Texture2D imgTexture = new Texture2D(256, 256);
                        imgTexture.LoadImage(tmpBytes);
                        _playersManager.SetupAvatar(imgTexture, player.player.nickname, player.player.clientId);
                        if (player.player.nickname != "Unity")
                        {
                            if (!_playersAbstract.ContainsKey(player.player.clientId))
                            {
                                _playersAbstract.Add(player.player.clientId, player.player);
                            }
                            else
                            {
                                _playersAbstract[player.player.clientId] = player.player;
                            }
                        }
                    }
                    catch
                    {
                        
                    }
                }
            }
            else if (message.Contains("player_left"))
            {
                JoinLeaveMessage player = JsonUtility.FromJson<JoinLeaveMessage>(message);
                if (_players.ContainsKey(player.player.clientId))
                {
                    _players.Remove(player.player.clientId);
                    _playersManager.DeletePlayer(player.player.clientId);
                }
            }
            else if (message.Contains("input_survivor"))
            {
                InputSurvivor player = JsonUtility.FromJson<InputSurvivor>(message);
                if (_players.ContainsKey(player.clientId))
                    _players[player.clientId].HandleInputs(new Vector2(player.joystick.x, player.joystick.y),
                        player.buttons.a, player.buttons.b);
            }
            else if (message.Contains("input_zombie"))
            {
                InputZombie zombie = JsonUtility.FromJson<InputZombie>(message);
                _zombieManager.SpawnZombie(new Vector2(zombie.position.x, zombie.position.y), zombie.troupes);
            }
            else if (message.Contains("player_updated"))
            {
                JoinLeaveMessage player = JsonUtility.FromJson<JoinLeaveMessage>(message);
                string base64 = player.player.avatar.Replace("data:image/jpeg;base64,", "");
                byte[] tmpBytes = Convert.FromBase64String(base64);
                Texture2D imgTexture = new Texture2D(256, 256);
                imgTexture.LoadImage(tmpBytes);
                _playersManager.SetupAvatar(imgTexture, player.player.nickname, player.player.clientId);
                if (player.player.nickname != "Unity")
                {
                    if (!_playersAbstract.ContainsKey(player.player.clientId))
                    {
                        _playersAbstract.Add(player.player.clientId, player.player);
                    }
                    else
                    {
                        _playersAbstract[player.player.clientId] = player.player;
                    }
                }
            }
        };

        //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        await _websocket.Connect();
    }

    private void Update()
    {
        /*if (_bufferPos.IsValid())
        {
            int index = 0;
            foreach (KeyValuePair<string, PlayerController> player in _players)
            {
                _positionsPlayer[index] = player.Value.transform.position;
                index++;
            }
            _bufferPos.SetData(_positionsPlayer);
        }*/
        
        if (_gameLaunched || _playersAbstract.Count == 0)
            return;
        bool allReady = true;
        foreach (KeyValuePair<string, Player> player in _playersAbstract)
        {
            if (!bool.Parse(player.Value.ready))
            {
                allReady = false;
            }
        }

        if (allReady)
        {
            _gameLaunched = true;
            StartCoroutine(WaitToLaunch());
        }
    }

    private IEnumerator WaitToLaunch()
    {
        yield return new WaitForSeconds(_timeToStart);
        SceneManager.LoadScene(_sceneName);
        yield return new WaitForSeconds(1f);
        foreach (KeyValuePair<string, Player> player in _playersAbstract)
        {
            string base64 = player.Value.avatar.Replace("data:image/jpeg;base64,", "");
            byte[] tmpBytes = Convert.FromBase64String(base64);
            Texture2D imgTexture = new Texture2D(256, 256);
            imgTexture.LoadImage(tmpBytes);
            _playersManager.SetupAvatar(imgTexture, player.Value.nickname, player.Value.clientId);
            if (player.Value.role == "survivor")
            {
                _players.Add(player.Value.clientId, _playersManager.CreateNewPlayer(player.Value.clientId, player.Value.nickname));
            }
        }
        /*_bufferPos = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _players.Count, 3 * sizeof(float));
        _fog.SetBuffer("_posPlayers", _bufferPos);
        _positionsPlayer = new Vector3[_players.Count];*/
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
}

using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayersManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private GameObject _prefabAvatar;
    [SerializeField] private GameObject _avatars;
    private Dictionary<string, PlayerController> _players = new Dictionary<string, PlayerController>();
    private Dictionary<string, Image> _playersAvatar = new Dictionary<string, Image>();
    public PlayerController CreateNewPlayer( string clientId, string name)
    {
        PlayerController newPlayer = Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f)),
            Quaternion.identity);
        _players.Add(clientId, newPlayer);
        newPlayer.Init(name);
        return newPlayer;
    }

    public void SetupAvatar(Texture2D avatar, string name, string clientId)
    {
        if (!_playersAvatar.ContainsKey(clientId))
        {
            GameObject imageObj = Instantiate(_prefabAvatar, _avatars.transform);
            Image image = imageObj.GetComponent<Image>();
            image.GetComponentInChildren<TextMeshProUGUI>().text = name;
            image.sprite = Sprite.Create(avatar, new Rect(0, 0, avatar.width, avatar.height), new Vector2(1.0f, 1.0f));
            _playersAvatar.Add(clientId, image);
        }
    }
    
    public void DeletePlayer(string clientId)
    {
        if (_players.ContainsKey(clientId))
        {
            if (_playersAvatar.ContainsKey(clientId))
            {
                Destroy(_playersAvatar[clientId].gameObject);
                _playersAvatar.Remove(clientId);
            }
            Destroy(_players[clientId].gameObject);
            _players.Remove(clientId);
        }
    }
}

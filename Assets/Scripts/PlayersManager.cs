using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayersManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private GameObject _prefabAvatar;
    private GameObject _avatars;
    private Dictionary<string, PlayerController> _players = new Dictionary<string, PlayerController>();
    private Dictionary<string, AvatarImageReference> _playersAvatar = new Dictionary<string, AvatarImageReference>();

    public PlayerController CreateNewPlayer( string clientId, string name)
    {
        PlayerController newPlayer = Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f)),
            Quaternion.identity);
        _players.Add(clientId, newPlayer);
        //_players[clientId].OnHit.AddListener(() => _playersAvatar[clientId].GetComponent<HitEffect>().OnHit());
        newPlayer.Init(name);
        return newPlayer;
    }

    public void SetupAvatar(Texture2D avatar, string name, string clientId)
    {
        if(_avatars == null)
            _avatars = FindAnyObjectByType<Avatars>().gameObject;
        if (!_playersAvatar.ContainsKey(clientId))
        {
            GameObject imageObj = Instantiate(_prefabAvatar, _avatars.transform);
            AvatarImageReference imageRef = imageObj.GetComponent<AvatarImageReference>();
            Image image = imageRef.imageAvatar;
            imageRef.name.text = name;
            image.sprite = Sprite.Create(avatar, new Rect(0, 0, avatar.width, avatar.height), new Vector2(1.0f, 1.0f));
            _playersAvatar.Add(clientId, imageRef);
        }
        else
        {
            _playersAvatar[clientId].imageAvatar.sprite = Sprite.Create(avatar, new Rect(0, 0, avatar.width, avatar.height),
                new Vector2(1.0f, 1.0f));
            _playersAvatar[clientId].name.text = name;
        }
    }

    public void ClearAvatar()
    {
        _playersAvatar.Clear();
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

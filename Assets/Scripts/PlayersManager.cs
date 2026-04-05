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
    
    #region COLOR
    private int _index;
    private static readonly Color[] Palette = new Color[]
    {
        HexToColor("EF476F"),
        HexToColor("FFD166"),
        HexToColor("06D6A0"),
        HexToColor("118AB2"),
        HexToColor("073B4C"),
        HexToColor("7742A3"),
        HexToColor("E37140"),
        HexToColor("EF91CC"),
        HexToColor("895E3E"),
        HexToColor("A81631"),
    };
    static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString("#" + hex, out Color c)) return c;
        Debug.LogError($"Invalid hex: {hex}");
        return Color.magenta;
    }
    #endregion

    public PlayerController CreateNewPlayer( string clientId, string name)
    {
        PlayerController newPlayer = Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f)),
            Quaternion.identity);
        if (_players.ContainsKey(clientId))
            _players.Add(clientId, newPlayer);
        else
        {
            _players[clientId] = newPlayer;
        }
        _players[clientId].OnHit.AddListener((float x) => _playersAvatar[clientId].GetComponent<HitEffect>()
            .OnHit(x, _playersAvatar[clientId].healthPlayer));
        _players[clientId].OnKillEnemy.AddListener(() => _playersAvatar[clientId].GetComponent<KillEffect>()
            .OnKill());
        _players[clientId].OnDeath.AddListener(() => _playersAvatar[clientId].GetComponent<DeathEffect>()
            .OnDeath());
        newPlayer.Init(name, Palette[_index]);
        return newPlayer;
    }

    public void SetupAvatar(Texture2D avatar, string name, string clientId)
    {
        if(_avatars == null)
            _avatars = FindAnyObjectByType<Avatars>().gameObject;
        if (!_playersAvatar.ContainsKey(clientId))
        {
            GameObject imageObj = Instantiate(_prefabAvatar, _avatars.transform);
            CircleLayoutManager managerLayout = _avatars.GetComponent<CircleLayoutManager>();
            if (managerLayout != null)
                managerLayout.AddItem(imageObj.GetComponent<RectTransform>());
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
        if (_playersAvatar.ContainsKey(clientId))
        {
            Destroy(_playersAvatar[clientId].gameObject);
            _playersAvatar.Remove(clientId);
        }
        if (_players.ContainsKey(clientId))
        {
            Destroy(_players[clientId].gameObject);
            //_players.Remove(clientId);
        }
    }
}

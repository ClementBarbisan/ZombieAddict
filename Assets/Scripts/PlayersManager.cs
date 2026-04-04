using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerPrefab;
    private Dictionary<string, PlayerController> _players = new Dictionary<string, PlayerController>();
    public PlayerController CreateNewPlayer(string name, string clientId)
    {
        PlayerController newPlayer = Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f)),
            Quaternion.identity);
        _players.Add(clientId, newPlayer);
        newPlayer.Init(name);
        return newPlayer;
    }

    public void DeletePlayer(string clientId)
    {
        if (_players.ContainsKey(clientId))
        {
            Destroy(_players[clientId].gameObject);
            _players.Remove(clientId);
        }
    }
}

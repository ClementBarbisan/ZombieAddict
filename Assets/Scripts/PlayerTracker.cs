using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    public static PlayerTracker Instance { get; private set; }

    private List<Transform> _players = new List<Transform>();

    public IReadOnlyList<Transform> Players => _players;

    private void Awake()
    {
        Instance = this;
    }
    
    public void Register(Transform player)
    {
        if (!_players.Contains(player))
            _players.Add(player);
    }
    
    public void Unregister(Transform player)
    {
        _players.Remove(player);
    }
}

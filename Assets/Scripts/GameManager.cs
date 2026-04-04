using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayersManager _players;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _players = FindAnyObjectByType<PlayersManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        
    }
}

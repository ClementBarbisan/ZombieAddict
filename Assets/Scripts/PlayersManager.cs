using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerPrefab;
    
    public PlayerController CreateNewPlayer(string name)
    {
        return Instantiate(playerPrefab, new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f)),
            Quaternion.identity);
    }
}

using TMPro;
using UnityEngine;

public class SetupEndGame : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        if (WebsocketManager.Instance.humansWin)
        {
            _textMesh.text = "Humans win!";
        }
        else
        {
            _textMesh.text = "Robots win!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using UnityEngine;

public class BallBomb : MonoBehaviour
{
    private bool _isOn;
    private Material _mat;
    private float _time;

    private void Start()
    {
        _mat = GetComponent<Renderer>().material;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (!_isOn)
                StartCoroutine(TikTak());
        }
    }

    private IEnumerator TikTak()
    {
        _mat.EnableKeyword("_EMISSION");
        yield return null;
    }
}

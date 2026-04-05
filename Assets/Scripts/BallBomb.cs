using System.Collections;
using UnityEngine;

public class BallBomb : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            
        }
    }

    private IEnumerator TikTak()
    {
        yield return null;
    }
}

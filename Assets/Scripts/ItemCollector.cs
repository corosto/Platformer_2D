using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    private int collectedItems = 0;
    [SerializeField] private Text collectablesText;

    [SerializeField] private AudioSource soundCollect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Collectable"))
        {
            Destroy(collision.gameObject);
            soundCollect.Play();
            collectedItems++;
            collectablesText.text = "Collected: " + collectedItems;
        }
    }
}

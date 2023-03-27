using UnityEngine;
using TMPro;

public class ItemCollector : MonoBehaviour
{
    private int collectedItems = 0;
    [SerializeField] private TMP_Text collectablesText;
    [SerializeField] private AudioSource soundCollect;

    private void Start()
    {
        collectedItems = PlayerPrefs.GetInt("collectables");
        collectablesText.text = "Collected: " + collectedItems;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Collectable"))
        {
            Destroy(collision.gameObject);
            soundCollect.Play();
            collectedItems++;
            collectablesText.text = "Collected: " + collectedItems;
            PlayerPrefs.SetInt("collectables", collectedItems);
        }
    }
}

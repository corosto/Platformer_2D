using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using TMPro;

public class PlayerLife : MonoBehaviour
{
    private Animator playerAnimator;
    private Rigidbody2D playerBody;
    private int playerLives = 3;
    private bool canBeDamaged = true;
    private Vector3 respawnPoint;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private AudioSource soundHit;
    [SerializeField] private AudioSource soundDeath;
    [SerializeField] private GameObject fallDetector;

    // Start is called before the first frame update
    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerBody = GetComponent<Rigidbody2D>();
        respawnPoint = transform.position;
        fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.position.y);
    }

    private void Update()
    {
        MoveFallDetector();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap") && canBeDamaged)
        {
            removeLife();
        }
    }

    private void Die()
    {
        soundDeath.Play();
        playerBody.bodyType = RigidbodyType2D.Static;
        playerAnimator.SetTrigger("death");
        PlayerPrefs.SetInt("collectables", 0);
    }

    private void Hurt()
    {
        canBeDamaged = false;
        Task.Delay(1500).ContinueWith((task) => { canBeDamaged = true; });
        soundHit.Play();
        playerAnimator.SetTrigger("hit");
        playerBody.AddForce(new Vector2(0, 16), ForceMode2D.Impulse);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(1);
    }

    private void MoveFallDetector()
    {
        fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Fall Detector")
        {
            removeLife();
            if(playerLives > 0)
                transform.position = respawnPoint;
        }
    }

    private void removeLife()
    {
        playerLives--;
        livesText.text = "Lives: " + playerLives;

        if (playerLives <= 0)
        {
            Die();
        }
        else
        {
            Hurt();
        }
    }
}

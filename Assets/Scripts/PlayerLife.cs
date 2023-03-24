using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    private Animator playerAnimator;
    private Rigidbody2D playerBody;
    private int playerLives = 3;
    private bool canBeDamaged = true;

    [SerializeField] private Text livesText;
    [SerializeField] private AudioSource soundHit;
    [SerializeField] private AudioSource soundDeath;

    // Start is called before the first frame update
    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerBody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap") && canBeDamaged)
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

    private void Die()
    {
        soundDeath.Play();
        playerBody.bodyType = RigidbodyType2D.Static;
        playerAnimator.SetTrigger("death");
    }

    private void Hurt()
    {
        soundHit.Play();
        canBeDamaged = false;
        playerAnimator.SetTrigger("hit");
        playerBody.AddForce(new Vector2(0, 16), ForceMode2D.Impulse);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ToggleInvincibility() {
        canBeDamaged = !canBeDamaged;
    }
}

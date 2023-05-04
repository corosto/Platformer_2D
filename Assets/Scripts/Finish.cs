using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Finish : MonoBehaviour
{
    [SerializeField] private AudioSource soundFinish;
    [SerializeField] Animator transitionAnim;
    private bool finished = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !finished)
        {
            finished = true;
            soundFinish.Play();
            Invoke("CompleteLevel", 1.3f);
        }
    }

    private void CompleteLevel()
    {
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel() {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        transitionAnim.SetTrigger("Start");
    }
}

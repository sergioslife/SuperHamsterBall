using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para reiniciar la escena.

public class RestartLevelOnCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Verifica si el objeto con el que colisiona es el plano.
        if (collision.gameObject.CompareTag("Plane"))
        {
            // Reinicia la escena actual.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

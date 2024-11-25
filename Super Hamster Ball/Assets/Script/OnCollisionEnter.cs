using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para reiniciar la escena.

public class RestartOnCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Verifica si el objeto con el que colisiona es el plano.
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("La esfera tocó el plano. Reiniciando el nivel...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

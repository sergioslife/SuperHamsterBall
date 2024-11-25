using System.Collections;
using UnityEngine;

public class HideCanvasAfterTime : MonoBehaviour
{
    public Canvas tutorialCanvas; // Asigna el Canvas del tutorial en el Inspector.
    public Canvas pointCanvas;    // Asigna el Canvas de "Point" en el Inspector.
    public float hideDelay = 2f;  // Tiempo en segundos antes de ocultar el Canvas del tutorial y activar el Canvas "Point".

    void Start()
    {
        if (tutorialCanvas != null && pointCanvas != null)
        {
            StartCoroutine(HandleCanvasCoroutine());
        }
        else
        {
            Debug.LogWarning("No se ha asignado uno o ambos Canvas en el script.");
        }
    }

    IEnumerator HandleCanvasCoroutine()
    {
        // Pausar la escena.
        Time.timeScale = 0f;  // Detiene el tiempo en el juego.

        // Espera 2 segundos en el tiempo real.
        yield return new WaitForSecondsRealtime(hideDelay);

        // Reanudar el tiempo en el juego.
        Time.timeScale = 1f;

        // Desactivar el Canvas del tutorial.
        tutorialCanvas.gameObject.SetActive(false);

        // Activar el Canvas "Point".
        pointCanvas.gameObject.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused; // Indica si el juego está en pausa.
    public GameObject pauseMenu; // Referencia al menú de pausa.
    public GameObject pointCanvas; // Referencia al Canvas de "Point" que se activará/desactivará.

    // Método que reinicia la escena actual.
    public void Restart()
    {
        // Reanudar el tiempo antes de reiniciar.
        Time.timeScale = 1;

        // Recargar la escena actual.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Update se ejecuta una vez por frame.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    // Método para alternar la pausa del juego.
    public void Pause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused); // Activa/desactiva el menú de pausa.
        Time.timeScale = isPaused ? 0 : 1; // Pausa o reanuda el juego.

        // Desactivar o activar el Canvas de "Point" dependiendo de si está en pausa.
        if (pointCanvas != null)
        {
            pointCanvas.SetActive(!isPaused); // Desactiva el canvas de "Point" cuando se pausa y lo activa cuando se reanuda.
        }
    }

    // Método para regresar al menú principal.
    public void Menu()
    {
        Time.timeScale = 1; // Reanudar el tiempo.
        SceneManager.LoadScene(0); // Cargar la escena del menú principal.
    }
}

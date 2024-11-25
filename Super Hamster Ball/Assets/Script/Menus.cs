using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    // Método para cerrar la aplicación
    public void Salir()
    {
#if UNITY_EDITOR
        // Detiene el modo de juego si estás en el editor de Unity.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Cierra la aplicación si está en la build.
        Application.Quit();
#endif
    }

    // Método para cargar una escena específica
    public void Iniciar(string nivel)
    {
        // Carga la escena especificada por el nombre.
        SceneManager.LoadScene(nivel);
    }

   
}

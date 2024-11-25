using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class SerialTest : MonoBehaviour
{
    SerialPort stream = new SerialPort("COM3", 115200); // Configura el puerto serial.
    public string strReceived;
    public string[] strData = new string[4];
    public float qw, qx, qy, qz;

    public float smoothFactor = 2f; // Factor para suavizar la interpolación.
    private Quaternion smoothedRotation; // Rotación suavizada acumulada.

    private bool initialCalibrationDone = false;
    private float initialQzOffset = 0f; // Offset inicial de Qz.

    // Límites de rotación (en grados) para cada eje.
    private Vector3 minRotation = new Vector3(-30f, -45f, -30f);
    private Vector3 maxRotation = new Vector3(30f, 45f, 30f);

    // Velocidad adicional para la suavización de las transiciones.
    private Vector3 rotationVelocity = Vector3.zero;
    public float dampingSpeed = 0.1f; // Velocidad del amortiguador.

    private Rigidbody rb; // Rigidbody de la plataforma.
    private Rigidbody playerRb; // Rigidbody de la esfera.

    void Start()
    {
        stream.Open(); // Abre el flujo serial.
        smoothedRotation = transform.rotation; // Iniciar la rotación suavizada.

        // Obtener el Rigidbody de la plataforma.
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Poner la plataforma en modo cinemático (sin física).
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Suavizar el movimiento de la plataforma.
        }

        // Obtener el Rigidbody de la esfera.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.useGravity = true; // Asegúrate de que la gravedad afecta la esfera.
                playerRb.interpolation = RigidbodyInterpolation.Interpolate; // Suavizar el movimiento de la esfera.
            }
        }
    }

    void Update()
    {
        if (stream.IsOpen)
        {
            try
            {
                strReceived = stream.ReadLine(); // Lee la información del puerto serial.
                strData = strReceived.Split(',');

                if (strData.Length == 4 &&
                    !string.IsNullOrEmpty(strData[0]) &&
                    !string.IsNullOrEmpty(strData[1]) &&
                    !string.IsNullOrEmpty(strData[2]) &&
                    !string.IsNullOrEmpty(strData[3])) // Asegúrate de que los datos estén listos.
                {
                    // Asignar los valores recibidos.
                    qw = float.Parse(strData[0]);
                    qx = float.Parse(strData[1]);
                    qy = float.Parse(strData[2]);
                    qz = float.Parse(strData[3]);

                    // Calibrar la diferencia inicial de Qz si no se ha hecho.
                    if (!initialCalibrationDone)
                    {
                        initialQzOffset = 90f - qz; // Suponiendo 90° como base.
                        initialCalibrationDone = true;
                    }

                    // Ajustar Qz con el offset calculado.
                    qz += initialQzOffset;

                    // Crear el quaternion recibido.
                    Quaternion receivedRotation = new Quaternion(-qy, -qz, qx, qw);

                    // Suavizar los datos de rotación con un filtro exponencial.
                    smoothedRotation = Quaternion.Slerp(smoothedRotation, receivedRotation, Time.deltaTime * smoothFactor);

                    // Limitar y suavizar los ángulos de rotación.
                    smoothedRotation = LimitAndDampenRotation(smoothedRotation, minRotation, maxRotation);

                    // Aplicar la rotación suavizada limitada a la plataforma.
                    transform.rotation = smoothedRotation;

                    // Si la esfera está sobre la plataforma, aplicar la misma rotación suavizada.
                    if (playerRb != null)
                    {
                        Vector3 relativeForce = rb.angularVelocity * Time.fixedDeltaTime;
                        playerRb.AddForce(relativeForce, ForceMode.VelocityChange);
                    }

                    // Actualizar la rotación de los cubos "Wall".
                    GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
                    foreach (var wall in walls)
                    {
                        wall.transform.rotation = smoothedRotation; // Rotar junto con la plataforma.
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error leyendo el puerto serial: " + ex.Message);
            }
        }
    }

    void OnDestroy()
    {
        if (stream.IsOpen)
        {
            stream.Close(); // Cierra el puerto serial al salir.
        }
    }

    // Función para limitar y amortiguar los ángulos de rotación.
    Quaternion LimitAndDampenRotation(Quaternion rotation, Vector3 minAngles, Vector3 maxAngles)
    {
        // Convertir a ángulos de Euler.
        Vector3 eulerRotation = rotation.eulerAngles;

        // Normalizar ángulos entre -180 y 180.
        eulerRotation.x = NormalizeAngle(eulerRotation.x);
        eulerRotation.y = NormalizeAngle(eulerRotation.y);
        eulerRotation.z = NormalizeAngle(eulerRotation.z);

        // Limitar y suavizar cada ángulo con amortiguación.
        eulerRotation.x = Mathf.SmoothDamp(eulerRotation.x, Mathf.Clamp(eulerRotation.x, minAngles.x, maxAngles.x), ref rotationVelocity.x, dampingSpeed);
        eulerRotation.y = Mathf.SmoothDamp(eulerRotation.y, Mathf.Clamp(eulerRotation.y, minAngles.y, maxAngles.y), ref rotationVelocity.y, dampingSpeed);
        eulerRotation.z = Mathf.SmoothDamp(eulerRotation.z, Mathf.Clamp(eulerRotation.z, minAngles.z, maxAngles.z), ref rotationVelocity.z, dampingSpeed);

        return Quaternion.Euler(eulerRotation); // Convertir de nuevo a cuaternión.
    }

    // Normaliza un ángulo a -180 y 180 grados.
    float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }
}

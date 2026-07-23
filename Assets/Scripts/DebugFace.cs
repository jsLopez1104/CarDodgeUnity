using UnityEngine;
using TMPro;

public class DebugFace : MonoBehaviour
{
    public TextMeshProUGUI textoDebug;

    void Update()
    {
        if (FaceTracker.instancia == null) return;

        textoDebug.text =
            "Yaw: " + FaceTracker.instancia.yawActual.ToString("F1") +
            "\nInput: " + FaceTracker.instancia.inputCabeza.ToString("F1");
    }
}
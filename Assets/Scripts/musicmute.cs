using UnityEngine;
using UnityEngine.UI;

public class musicmute : MonoBehaviour
{
    [Header("Sprites UI")]
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;

    private void Start()
    {
        ActualizarUI();
    }

    private void OnEnable()
    {
        Volumen.Cambio += ActualizarUI;
        ActualizarUI();
    }

    private void OnDisable()
    {
        Volumen.Cambio -= ActualizarUI;
    }

    public void ToggleMusic()
    {
        Volumen.PonerSilencio(!Volumen.Silenciado);
    }

    private void ActualizarUI()
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = Volumen.Silenciado ? musicOffSprite : musicOnSprite;
        }
    }
}
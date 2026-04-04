using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PaletteColorAssigner : MonoBehaviour
{
    [Tooltip("The TextMeshPro component to colorize. Leave empty to search on this GameObject.")]
    public TMP_Text targetText;

    [Tooltip("The Image component to colorize. Leave empty to search on this GameObject.")]
    public Image targetImage;

    [Tooltip("Parent whose child index determines the color. Leave empty to use this object's own parent.")]
    public Transform colorSourceParent;

    private static readonly Color[] Palette = new Color[]
    {
        HexToColor("EF476F"),
        HexToColor("FFD166"),
        HexToColor("06D6A0"),
        HexToColor("118AB2"),
        HexToColor("073B4C"),
        HexToColor("7742A3"),
        HexToColor("E37140"),
        HexToColor("EF91CC"),
        HexToColor("895E3E"),
        HexToColor("A81631"),
    };

    void Awake()
    {
        if (!Application.isPlaying) return;

        if (targetText == null)
            targetText = GetComponent<TMP_Text>();

        if (targetImage == null)
            targetImage = GetComponent<Image>();
    }

    void Start()
    {
        if (!Application.isPlaying) return;

        Apply();
    }

    public void Apply()
    {
        if (!Application.isPlaying) return;

        Transform source = colorSourceParent != null ? colorSourceParent : transform;

        if (source.parent == null)
        {
            Debug.LogWarning($"[PaletteColorAssigner] '{source.name}' has no parent — can't determine sibling index.", this);
            return;
        }

        int index = source.GetSiblingIndex();
        Color color = Palette[index % Palette.Length];

        bool applied = false;

        if (targetText != null)
        {
            targetText.color = color;
            applied = true;
        }

        if (targetImage != null)
        {
            targetImage.color = color;
            applied = true;
        }

        if (!applied)
        {
            Debug.LogWarning($"[PaletteColorAssigner] No TMP_Text or Image found on '{name}'.", this);
        }
    }

    static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString("#" + hex, out Color c)) return c;
        Debug.LogError($"Invalid hex: {hex}");
        return Color.magenta;
    }
}
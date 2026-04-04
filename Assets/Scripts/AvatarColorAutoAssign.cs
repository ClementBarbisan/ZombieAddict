using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
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

    void OnEnable() => Apply();
    void OnValidate() => Apply();

    [ContextMenu("Apply Palette Color")]
    public void Apply()
    {
        // Resolve sibling index source
        Transform source = colorSourceParent != null ? colorSourceParent : transform;
        if (source.parent == null)
        {
            Debug.LogWarning($"[PaletteColorAssigner] '{source.name}' has no parent — can't determine sibling index.", this);
            return;
        }

        int index = source.GetSiblingIndex();
        Color color = Palette[index % Palette.Length];

        bool applied = false;

        // --- TMP_Text ---
        TMP_Text tmp = targetText != null ? targetText : GetComponent<TMP_Text>();
        if (tmp != null)
        {
            tmp.color = color;
            applied = true;
            Debug.Log($"[PaletteColorAssigner] TMP_Text '{tmp.name}' → index {index} → #{ColorToHex(color)}");
        }

        // --- Image ---
        Image img = targetImage != null ? targetImage : GetComponent<Image>();
        if (img != null)
        {
            img.color = color;
            applied = true;
            Debug.Log($"[PaletteColorAssigner] Image '{img.name}' → index {index} → #{ColorToHex(color)}");
        }

        if (!applied)
            Debug.LogWarning($"[PaletteColorAssigner] No TMP_Text or Image found on '{name}'. Assign targets manually or add a supported component.", this);
    }

    static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString("#" + hex, out Color c)) return c;
        Debug.LogError($"[PaletteColorAssigner] Invalid hex: {hex}");
        return Color.magenta;
    }

    static string ColorToHex(Color c) =>
        ((Color32)c).r.ToString("X2") +
        ((Color32)c).g.ToString("X2") +
        ((Color32)c).b.ToString("X2");
}
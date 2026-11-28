using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Data kepala kartu tutorial: nomor langkah (mulai 1), jumlah langkah, dan judul langkah.</summary>
public struct TutorialDialogueInfo
{
    public int StepNumber;
    public int StepCount;
    public string Title;

    public TutorialDialogueInfo(int stepNumber, int stepCount, string title)
    {
        StepNumber = stepNumber;
        StepCount = stepCount;
        Title = title;
    }
}

/// <summary>
/// Tampilan "kartu panduan" untuk pesan tutorial TARS (USULAN), sengaja berbeda dari dialog biasa agar pemain
/// langsung tahu ini arahan tutorial. Gaya: satu keluarga warna hangat terakota (cocok dengan Mars), kotak berbingkai
/// gelap, kotak portrait TARS di kiri, tab nama "TARS | PANDUAN" di tepi atas, lalu nomor langkah, judul, isi cerita,
/// dan kotak "TUJUAN SEKARANG" (warna lebih gelap dari kotak, bukan warna lain) untuk baris yang diawali "Sekarang:".
/// Kartu ada di TENGAH ATAS layar (dialog biasa di bawah). Hanya tampilan: input (E, Spasi, klik), IsOpen, dan callback
/// tetap dikelola DialogueUI. UI dibuat lewat kode (tanpa edit scene) sebagai saudara DialogueRoot di Canvas yang sama.
/// Portrait diambil dari sprite hadap-bawah TARS (TarsWanderer.PortraitSprite). Posisi dan warna adalah konstanta di bawah.
/// </summary>
public class TutorialDialogueSkin : MonoBehaviour
{
    private const float CardWidth = 1000f;
    private const float TopOffset = 190f;     // tab nama menonjol 44 di atas kartu; harus di bawah panel timer (110 + margin 20)
    private const float PortraitSize = 200f;
    private const float FadeSeconds = 0.15f;
    private const string ObjectivePrefix = "Sekarang:";

    // Satu keluarga warna terakota + satu warna gelap untuk bingkai dan teks.
    private static readonly Color BoxColor = new Color(0.94f, 0.60f, 0.45f, 1f);
    private static readonly Color DarkBlockColor = new Color(0.72f, 0.36f, 0.26f, 1f);
    private static readonly Color BorderColor = new Color(0.30f, 0.20f, 0.20f, 1f);
    private static readonly Color InkColor = new Color(0.20f, 0.10f, 0.08f, 1f);
    private static readonly Color MutedInkColor = new Color(0.45f, 0.22f, 0.16f, 1f);
    private static readonly Color CreamColor = new Color(1f, 0.94f, 0.86f, 1f);

    private RectTransform card;
    private CanvasGroup group;
    private Text stepLabel;
    private Text titleText;
    private Text bodyText;
    private Text objectiveText;
    private Text footerText;
    private GameObject objectiveStrip;
    private RectTransform closeRect;
    private Canvas canvas;
    private float shownAt;

    /// <summary>Dipanggil saat tombol X pada kartu diklik.</summary>
    public event Action OnCloseClicked;

    public static TutorialDialogueSkin Create(Transform parent, int siblingIndex, Font font, Sprite portrait)
    {
        GameObject go = new GameObject("TutorialDialogueSkin", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        go.transform.SetSiblingIndex(siblingIndex);

        TutorialDialogueSkin skin = go.AddComponent<TutorialDialogueSkin>();
        skin.Build(font, portrait);
        return skin;
    }

    public void Open(TutorialDialogueInfo info)
    {
        titleText.text = info.Title ?? string.Empty;
        stepLabel.text = "Langkah " + info.StepNumber + " dari " + info.StepCount;
        shownAt = Time.unscaledTime;
        group.alpha = 0f;
        card.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (card != null) card.gameObject.SetActive(false);
    }

    /// <summary>Menampilkan satu baris. Baris terakhir yang diawali "Sekarang:" tampil di kotak TUJUAN SEKARANG.</summary>
    public void Render(int lineIndex, int lineCount, string text)
    {
        text = text ?? string.Empty;
        bool isLast = lineIndex >= lineCount - 1;
        bool objective = isLast && text.StartsWith(ObjectivePrefix, StringComparison.Ordinal);

        if (objective)
        {
            objectiveText.text = text.Substring(ObjectivePrefix.Length).Trim();
            bodyText.gameObject.SetActive(false);
        }
        else
        {
            bodyText.text = text;
            bodyText.gameObject.SetActive(true);
        }
        objectiveStrip.SetActive(objective);

        string part = lineCount > 1 ? "Bagian " + (lineIndex + 1) + " dari " + lineCount + "   |   " : string.Empty;
        footerText.text = part + (isLast ? "[E] selesai" : "[E] lanjut");

        LayoutRebuilder.ForceRebuildLayoutImmediate(card);
    }

    public bool IsPointerOverClose(Vector2 screenPoint)
    {
        if (closeRect == null || !closeRect.gameObject.activeInHierarchy) return false;

        Camera uiCamera = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;
        return RectTransformUtility.RectangleContainsScreenPoint(closeRect, screenPoint, uiCamera);
    }

    private void Update()
    {
        if (card == null || !card.gameObject.activeSelf) return;
        group.alpha = Mathf.Clamp01((Time.unscaledTime - shownAt) / FadeSeconds);
    }

    // ---------- Pembuatan UI ----------

    private void Build(Font font, Sprite portrait)
    {
        canvas = GetComponentInParent<Canvas>();

        RectTransform self = (RectTransform)transform;
        self.anchorMin = Vector2.zero;
        self.anchorMax = Vector2.one;
        self.offsetMin = Vector2.zero;
        self.offsetMax = Vector2.zero;

        // Kartu: kotak terakota berbingkai gelap; anak kiri = portrait, anak kanan = isi.
        GameObject cardGo = new GameObject("TutorialCard", typeof(RectTransform), typeof(Image), typeof(CanvasGroup),
            typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter), typeof(Outline));
        card = cardGo.GetComponent<RectTransform>();
        card.SetParent(transform, false);
        card.anchorMin = new Vector2(0.5f, 1f);
        card.anchorMax = new Vector2(0.5f, 1f);
        card.pivot = new Vector2(0.5f, 1f);
        card.anchoredPosition = new Vector2(0f, -TopOffset);
        card.sizeDelta = new Vector2(CardWidth, 0f);

        Image background = cardGo.GetComponent<Image>();
        background.color = BoxColor;
        background.raycastTarget = false;
        AddBorder(cardGo.GetComponent<Outline>(), 4f);

        group = cardGo.GetComponent<CanvasGroup>();
        group.blocksRaycasts = true;

        HorizontalLayoutGroup row = cardGo.GetComponent<HorizontalLayoutGroup>();
        row.padding = new RectOffset(0, 0, 0, 0);
        row.spacing = 0f;
        row.childAlignment = TextAnchor.UpperLeft;
        row.childControlWidth = true;
        row.childControlHeight = true;
        row.childForceExpandWidth = false;
        row.childForceExpandHeight = true;

        ContentSizeFitter fitter = cardGo.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        BuildPortrait(portrait);
        RectTransform content = BuildContentColumn();
        BuildHeader(content, font);
        titleText = CreateText("Title", content, font, 36, InkColor, TextAnchor.UpperLeft, FontStyle.Bold);
        bodyText = CreateText("Body", content, font, 28, InkColor, TextAnchor.UpperLeft, FontStyle.Normal);
        BuildObjectiveStrip(content, font);
        footerText = CreateText("Footer", content, font, 20, MutedInkColor, TextAnchor.MiddleRight, FontStyle.Normal);
        BuildNameTab(font);

        card.gameObject.SetActive(false);
    }

    private void BuildPortrait(Sprite portrait)
    {
        GameObject blockGo = new GameObject("PortraitBlock", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
        RectTransform block = blockGo.GetComponent<RectTransform>();
        block.SetParent(card, false);

        Image blockImage = blockGo.GetComponent<Image>();
        blockImage.color = DarkBlockColor;
        blockImage.raycastTarget = false;

        LayoutElement size = blockGo.GetComponent<LayoutElement>();
        size.minWidth = PortraitSize;
        size.preferredWidth = PortraitSize;
        size.minHeight = PortraitSize;

        GameObject imageGo = new GameObject("TarsPortrait", typeof(RectTransform), typeof(Image));
        RectTransform imageRect = imageGo.GetComponent<RectTransform>();
        imageRect.SetParent(block, false);
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.offsetMin = new Vector2(14f, 14f);
        imageRect.offsetMax = new Vector2(-14f, -14f);

        Image image = imageGo.GetComponent<Image>();
        image.sprite = portrait;
        image.preserveAspect = true;
        image.raycastTarget = false;
        image.enabled = portrait != null; // tanpa sprite, hanya kotak polos
    }

    private RectTransform BuildContentColumn()
    {
        GameObject contentGo = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement));
        RectTransform content = contentGo.GetComponent<RectTransform>();
        content.SetParent(card, false);
        contentGo.GetComponent<LayoutElement>().flexibleWidth = 1f;

        VerticalLayoutGroup layout = contentGo.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(26, 26, 18, 18);
        layout.spacing = 10f;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        return content;
    }

    private void BuildHeader(RectTransform content, Font font)
    {
        GameObject headerGo = new GameObject("Header", typeof(RectTransform), typeof(LayoutElement));
        RectTransform header = headerGo.GetComponent<RectTransform>();
        header.SetParent(content, false);
        headerGo.GetComponent<LayoutElement>().preferredHeight = 40f;

        // Nomor langkah (kiri).
        stepLabel = CreateText("StepLabel", header, font, 24, MutedInkColor, TextAnchor.MiddleLeft, FontStyle.Bold);
        RectTransform stepRect = stepLabel.rectTransform;
        stepRect.anchorMin = new Vector2(0f, 0.5f);
        stepRect.anchorMax = new Vector2(0f, 0.5f);
        stepRect.pivot = new Vector2(0f, 0.5f);
        stepRect.anchoredPosition = Vector2.zero;
        stepRect.sizeDelta = new Vector2(320f, 38f);

        // Tombol X (kanan): menutup kartu, melewati sisa baris.
        GameObject closeGo = new GameObject("CloseButton", typeof(RectTransform), typeof(Image), typeof(Button));
        closeRect = closeGo.GetComponent<RectTransform>();
        closeRect.SetParent(header, false);
        closeRect.anchorMin = new Vector2(1f, 0.5f);
        closeRect.anchorMax = new Vector2(1f, 0.5f);
        closeRect.pivot = new Vector2(1f, 0.5f);
        closeRect.anchoredPosition = Vector2.zero;
        closeRect.sizeDelta = new Vector2(52f, 38f);
        Image closeImage = closeGo.GetComponent<Image>();
        closeImage.color = BorderColor;
        closeImage.raycastTarget = true;
        Button closeButton = closeGo.GetComponent<Button>();
        closeButton.targetGraphic = closeImage;
        closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
        Text closeLabel = CreateText("Label", closeRect, font, 26, CreamColor, TextAnchor.MiddleCenter, FontStyle.Bold);
        closeLabel.text = "X";
        Stretch(closeLabel.rectTransform);
    }

    private void BuildObjectiveStrip(RectTransform content, Font font)
    {
        objectiveStrip = new GameObject("ObjectiveStrip", typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup), typeof(Outline));
        RectTransform strip = objectiveStrip.GetComponent<RectTransform>();
        strip.SetParent(content, false);

        Image image = objectiveStrip.GetComponent<Image>();
        image.color = DarkBlockColor;
        image.raycastTarget = false;
        AddBorder(objectiveStrip.GetComponent<Outline>(), 2f);

        VerticalLayoutGroup layout = objectiveStrip.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(18, 18, 12, 14);
        layout.spacing = 4f;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        Text label = CreateText("ObjectiveLabel", strip, font, 20, CreamColor, TextAnchor.UpperLeft, FontStyle.Bold);
        label.text = "TUJUAN SEKARANG";
        objectiveText = CreateText("ObjectiveText", strip, font, 32, CreamColor, TextAnchor.UpperLeft, FontStyle.Bold);

        objectiveStrip.SetActive(false);
    }

    // Tab nama di tepi atas kartu (di sebelah kotak portrait), seperti label nama pembicara pada game dialog klasik.
    private void BuildNameTab(Font font)
    {
        GameObject tabGo = new GameObject("NameTab", typeof(RectTransform), typeof(Image), typeof(LayoutElement), typeof(Outline));
        RectTransform tab = tabGo.GetComponent<RectTransform>();
        tab.SetParent(card, false);
        tabGo.GetComponent<LayoutElement>().ignoreLayout = true;
        tab.anchorMin = new Vector2(0f, 1f);
        tab.anchorMax = new Vector2(0f, 1f);
        tab.pivot = new Vector2(0f, 0f);
        tab.anchoredPosition = new Vector2(PortraitSize + 24f, -2f);
        tab.sizeDelta = new Vector2(300f, 46f);

        Image image = tabGo.GetComponent<Image>();
        image.color = BoxColor;
        image.raycastTarget = false;
        AddBorder(tabGo.GetComponent<Outline>(), 3f);

        Text label = CreateText("TabText", tab, font, 28, InkColor, TextAnchor.MiddleCenter, FontStyle.Bold);
        label.text = "TARS  |  PANDUAN";
        Stretch(label.rectTransform);
    }

    private static void AddBorder(Outline outline, float thickness)
    {
        outline.effectColor = BorderColor;
        outline.effectDistance = new Vector2(thickness, -thickness);
        outline.useGraphicAlpha = false;
    }

    private static Text CreateText(string name, Transform parent, Font font, int size, Color color, TextAnchor anchor, FontStyle style)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        Text text = go.GetComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = anchor;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        return text;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}

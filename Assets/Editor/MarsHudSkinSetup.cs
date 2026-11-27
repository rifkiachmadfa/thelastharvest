#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu Editor: memberi HUD yang sudah ada (Mars > Setup HUD and Time, Suit, Hunger) tampilan Pixel UI Sci-Fi Free
/// (CC BY 4.0, lihat Assets/UI/PixelUISciFi/CREDITS.txt): panel 9-slice, ikon per resource, dan bar isi bersegmen.
/// Teks, event, dan logika HudUI tidak diubah; bar hanya tambahan tampilan. Aman dijalankan berulang kali.
/// Mars > Apply HUD Skin (Pixel UI Sci-Fi)
/// </summary>
public static class MarsHudSkinSetup
{
    private const string AssetRoot = "Assets/UI/PixelUISciFi/";

    // Seni 16 px digambar UiScale kali di Canvas 1920x1080 (USULAN, bukan angka final; ubah di sini bila terlalu kecil/besar).
    private const float UiScale = 2f;

    // Tata letak satu baris (teks di atas, bar di bawah, ikon di kiri), dalam unit Canvas.
    private const float PanelWidth = 340f;
    private const float PanelPad = 10f;
    private const float SidePad = 12f;
    private const float IconSize = 32f;
    private const float TextLeft = 52f;
    private const float TextHeight = 24f;
    private const float TextGap = 2f;
    private const float BarHeight = 22f;
    private const float BarInset = 6f; // border progress_bg 3 px x UiScale
    private const float RowHeight = TextHeight + TextGap + BarHeight;
    private const float RowPitch = RowHeight + 6f;
    private const float ScreenMargin = 20f;
    private const float PanelGap = 10f;
    private const int TextSize = 24;

    private class Row
    {
        public readonly string TextField;
        public readonly string BarField;
        public readonly string Key;
        public readonly string IconPath;
        public readonly string FillPath;

        public Row(string textField, string barField, string key, string iconPath, string fillPath)
        {
            TextField = textField;
            BarField = barField;
            Key = key;
            IconPath = iconPath;
            FillPath = fillPath;
        }
    }

    // Pilihan ikon dan warna bar adalah usulan; ganti path di sini lalu jalankan menu lagi.
    private static readonly Row[] ResourceRows =
    {
        new Row("foodText", "foodBar", "Food", "icons/health.png", "bars/bar_green.png"),
        new Row("waterText", "waterBar", "Water", "icons/fuel.png", "bars/bar_blue.png"),
        new Row("oxygenText", "oxygenBar", "Oxygen", "icons/shield.png", "cyan/progress_fill.png"),
        new Row("energyText", "energyBar", "Energy", "icons/energy.png", "bars/bar_gold.png"),
    };
    private static readonly Row HungerRow = new Row("hungerText", "hungerBar", "Hunger", "icons/heart.png", "bars/bar_red.png");
    private static readonly Row HpRow = new Row("hpText", "hpBar", "Hp", "icons/heart.png", "bars/bar_red.png");
    private static readonly Row SuitRow = new Row("suitOxygenText", "suitBar", "Suit", "icons/astronaut.png", "cyan/progress_fill.png");

    [MenuItem("Mars/Apply HUD Skin (Pixel UI Sci-Fi)")]
    public static void Apply()
    {
        StringBuilder report = new StringBuilder();

        HudUI hud = Object.FindFirstObjectByType<HudUI>();
        if (hud == null)
        {
            EditorUtility.DisplayDialog("Mars Setup",
                "HudUI tidak ditemukan. Jalankan dulu menu Mars > Setup HUD and Time.", "OK");
            return;
        }

        Sprite panelSprite = PrepareSprite("cyan/panel.png", 4, 4, 4, 4, report);
        if (panelSprite == null || PrepareSprite("cyan/progress_bg.png", 3, 3, 3, 3, report) == null)
        {
            EditorUtility.DisplayDialog("Mars Setup",
                "Aset Pixel UI Sci-Fi tidak ditemukan di " + AssetRoot + ".\n\n" + report, "OK");
            return;
        }

        // 1. Panel waktu (Day + timer): hanya ganti sprite panel.
        Text dayText = GetRef<Text>(hud, "dayText");
        if (dayText != null) SkinPanel(dayText.transform.parent, panelSprite);

        // 2. Panel resource: Food, Water, Oxygen, Energy.
        int rows = 0;
        Transform resourcePanel = null;
        foreach (Row row in ResourceRows)
        {
            Text text = GetRef<Text>(hud, row.TextField);
            if (text == null) continue;
            resourcePanel = text.transform.parent;
            BuildRow(hud, text, row, rows, report);
            rows++;
        }
        float resourceHeight = 0f;
        if (resourcePanel != null)
        {
            SkinPanel(resourcePanel, panelSprite);
            resourceHeight = PlacePanel(resourcePanel, new Vector2(1f, 1f), new Vector2(-ScreenMargin, -ScreenMargin), rows);
            report.AppendLine("- Panel resource: " + rows + " baris (ikon + teks + bar).");
        }

        // 3. Kenyang (opsional): kiri atas. HP (opsional) tepat di bawahnya.
        float hungerHeight = 0f;
        Text hungerText = GetRef<Text>(hud, HungerRow.TextField);
        if (hungerText != null)
        {
            BuildRow(hud, hungerText, HungerRow, 0, report);
            Transform hungerPanel = hungerText.transform.parent;
            SkinPanel(hungerPanel, panelSprite);
            hungerHeight = PlacePanel(hungerPanel, new Vector2(0f, 1f), new Vector2(ScreenMargin, -ScreenMargin), 1);
            report.AppendLine("- Panel Kenyang diberi skin.");
        }

        Text hpText = GetRef<Text>(hud, HpRow.TextField);
        if (hpText != null)
        {
            BuildRow(hud, hpText, HpRow, 0, report);
            Transform hpPanel = hpText.transform.parent;
            SkinPanel(hpPanel, panelSprite);
            float y = -(ScreenMargin + hungerHeight + (hungerHeight > 0f ? PanelGap : 0f));
            PlacePanel(hpPanel, new Vector2(0f, 1f), new Vector2(ScreenMargin, y), 1);
            report.AppendLine("- Panel HP diberi skin.");
        }

        // 4. Suit O2 (opsional): kiri bawah.
        Text suitText = GetRef<Text>(hud, SuitRow.TextField);
        if (suitText != null)
        {
            BuildRow(hud, suitText, SuitRow, 0, report);
            Transform suitPanel = suitText.transform.parent;
            SkinPanel(suitPanel, panelSprite);
            PlacePanel(suitPanel, new Vector2(0f, 0f), new Vector2(ScreenMargin, ScreenMargin), 1);
            report.AppendLine("- Panel Suit O2 diberi skin.");
        }

        EditorSceneManager.MarkSceneDirty(hud.gameObject.scene);
        report.AppendLine();
        report.AppendLine("Tekan Ctrl+S untuk menyimpan scene. Jangan lupa kredit CC BY 4.0 (Assets/UI/PixelUISciFi/CREDITS.txt).");
        Debug.Log("MarsHudSkinSetup:\n" + report);
        EditorUtility.DisplayDialog("Mars Setup", report.ToString(), "OK");
    }

    // Satu baris: ikon, teks, dan bar. Mengatur ulang tata letak teks yang sudah ada.
    private static void BuildRow(HudUI hud, Text text, Row row, int index, StringBuilder report)
    {
        Transform parent = text.transform.parent;
        float top = -PanelPad - index * RowPitch;

        Undo.RecordObject(text, "Skin HUD row");
        Undo.RecordObject(text.rectTransform, "Skin HUD row");
        text.fontSize = TextSize;
        text.alignment = TextAnchor.MiddleLeft;
        StretchRow(text.rectTransform, top, TextHeight);

        // Ikon
        Image icon = GetOrCreateImage(parent, row.Key + "Icon");
        Undo.RecordObject(icon, "Skin HUD row");
        icon.sprite = PrepareSprite(row.IconPath, 0, 0, 0, 0, report);
        icon.type = Image.Type.Simple;
        icon.preserveAspect = true;
        icon.color = Color.white;
        RectTransform iconRect = icon.rectTransform;
        Undo.RecordObject(iconRect, "Skin HUD row");
        iconRect.anchorMin = new Vector2(0f, 1f);
        iconRect.anchorMax = new Vector2(0f, 1f);
        iconRect.pivot = new Vector2(0f, 1f);
        iconRect.anchoredPosition = new Vector2(SidePad, top - (RowHeight - IconSize) * 0.5f);
        iconRect.sizeDelta = new Vector2(IconSize, IconSize);

        // Latar bar
        Image barImage = GetOrCreateImage(parent, row.Key + "Bar");
        Undo.RecordObject(barImage, "Skin HUD row");
        barImage.sprite = PrepareSprite("cyan/progress_bg.png", 3, 3, 3, 3, report);
        SetSliced(barImage, Image.Type.Sliced);
        StretchRow(barImage.rectTransform, top - TextHeight - TextGap, BarHeight);

        // Area isi (inset sebesar border latar) dan isi bar bersegmen
        Transform areaTransform = barImage.transform.Find("FillArea");
        RectTransform area;
        if (areaTransform != null)
        {
            area = areaTransform as RectTransform;
        }
        else
        {
            GameObject areaGO = new GameObject("FillArea", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(areaGO, "Create FillArea");
            areaGO.transform.SetParent(barImage.transform, false);
            area = areaGO.GetComponent<RectTransform>();
        }
        Undo.RecordObject(area, "Skin HUD row");
        area.anchorMin = Vector2.zero;
        area.anchorMax = Vector2.one;
        area.pivot = new Vector2(0.5f, 0.5f);
        area.offsetMin = new Vector2(BarInset, BarInset);
        area.offsetMax = new Vector2(-BarInset, -BarInset);

        Image fill = GetOrCreateImage(area, "Fill");
        Undo.RecordObject(fill, "Skin HUD row");
        fill.sprite = PrepareSprite(row.FillPath, 2, 3, 2, 2, report);
        SetSliced(fill, Image.Type.Tiled);
        RectTransform fillRect = fill.rectTransform;
        Undo.RecordObject(fillRect, "Skin HUD row");
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        HudBar bar = barImage.GetComponent<HudBar>();
        if (bar == null) bar = Undo.AddComponent<HudBar>(barImage.gameObject);
        MarsUISetup.SetRef(bar, "fill", fillRect);
        MarsUISetup.SetRef(hud, row.BarField, bar);
    }

    private static void SkinPanel(Transform panel, Sprite sprite)
    {
        if (panel == null) return;
        Image image = panel.GetComponent<Image>();
        if (image == null) return;

        Undo.RecordObject(image, "Skin HUD panel");
        image.sprite = sprite;
        SetSliced(image, Image.Type.Sliced);
        image.color = Color.white;
    }

    private static void SetSliced(Image image, Image.Type type)
    {
        image.type = type;
        image.fillCenter = true;
        image.pixelsPerUnitMultiplier = 1f / UiScale;
        image.raycastTarget = false;
    }

    // Menaruh panel di sudut layar dan menghitung tingginya dari jumlah baris. Mengembalikan tinggi panel.
    private static float PlacePanel(Transform panel, Vector2 anchor, Vector2 position, int rows)
    {
        float height = PanelPad * 2f + (rows - 1) * RowPitch + RowHeight;
        RectTransform rect = panel as RectTransform;
        if (rect == null) return height;

        Undo.RecordObject(rect, "Place HUD panel");
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(PanelWidth, height);
        return height;
    }

    // Membentang di lebar panel (dengan ruang ikon di kiri), menempel di tepi atas pada posisi y.
    private static void StretchRow(RectTransform rect, float y, float height)
    {
        Undo.RecordObject(rect, "Skin HUD row");
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(-(TextLeft + SidePad), height);
        rect.anchoredPosition = new Vector2((TextLeft - SidePad) * 0.5f, y);
    }

    private static Image GetOrCreateImage(Transform parent, string name)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            Image existingImage = existing.GetComponent<Image>();
            if (existingImage == null) existingImage = Undo.AddComponent<Image>(existing.gameObject);
            return existingImage;
        }

        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        image.raycastTarget = false;
        return image;
    }

    private static T GetRef<T>(Object target, string fieldName) where T : Object
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty(fieldName);
        return prop == null ? null : prop.objectReferenceValue as T;
    }

    // Memastikan impor sprite benar (Sprite, Point, tanpa kompresi, border 9-slice) lalu memuatnya.
    // Border dalam urutan kiri, atas, kanan, bawah (sama dengan slices.json milik aset).
    private static Sprite PrepareSprite(string relativePath, int left, int top, int right, int bottom, StringBuilder report)
    {
        string path = AssetRoot + relativePath;
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            string warning = "- PERINGATAN: aset tidak ditemukan: " + path;
            if (report.ToString().IndexOf(warning) < 0) report.AppendLine(warning);
            return null;
        }

        bool dirty = false;
        if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; dirty = true; }
        if (importer.spriteImportMode != SpriteImportMode.Single) { importer.spriteImportMode = SpriteImportMode.Single; dirty = true; }
        if (!Mathf.Approximately(importer.spritePixelsPerUnit, 100f)) { importer.spritePixelsPerUnit = 100f; dirty = true; }
        if (importer.filterMode != FilterMode.Point) { importer.filterMode = FilterMode.Point; dirty = true; }
        if (importer.mipmapEnabled) { importer.mipmapEnabled = false; dirty = true; }
        if (importer.textureCompression != TextureImporterCompression.Uncompressed)
        {
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            dirty = true;
        }

        Vector4 border = new Vector4(left, bottom, right, top); // x=kiri, y=bawah, z=kanan, w=atas
        if (importer.spriteBorder != border) { importer.spriteBorder = border; dirty = true; }

        if (dirty) importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
#endif

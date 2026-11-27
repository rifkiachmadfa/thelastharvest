using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Panel kontrol mesin dengan tampilan sendiri (bukan dialog TARS): Water Purifier (tombol PROSES, bar proses,
/// Energy, tangki Water), Oxygen Generator (tombol NYALAKAN/MATIKAN, status, oksigen habitat, Water, Energy) dan
/// Solar Panel (status normal/rusak, output panel, output total, Energy, tombol PERBAIKI yang membuka minigame kabel).
/// Tutup dengan tombol Tutup atau Esc. Selama terbuka, gerakan Player dikunci (nilai sebelumnya dipulihkan saat
/// ditutup); waktu hari dan mesin TETAP berjalan (sama seperti StorageUI), jadi bar ikut bergerak.
/// Seluruh tampilan dibuat lewat kode saat Awake di bawah Canvas (UnityEngine.UI.Text legacy), jadi hanya perlu
/// dipasang pada objek Canvas (menu Mars > Setup Machine Panels).
/// Hanya tampilan dan input panel: aturan mesin tetap di WaterPurifier, OxygenGenerator, SolarPanel dan RepairPoint.
/// </summary>
public class MachinePanelUI : MonoBehaviour
{
    [SerializeField] private KeyCode closeKey = KeyCode.Escape;
    [Tooltip("Panel diperbarui sekian detik sekali (tombol Esc tetap dicek tiap frame).")]
    [SerializeField, Min(0.02f)] private float refreshInterval = 0.1f;
    [Tooltip("Lama pesan hasil tombol (misalnya alasan PROSES ditolak) ditampilkan.")]
    [SerializeField, Min(0.5f)] private float feedbackSeconds = 3f;

    [Header("Referensi (otomatis dicari jika kosong)")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private PlayerMovement player;

    private enum Mode { None, Purifier, Generator, Solar }

    private sealed class Gauge
    {
        public Text label;
        public RectTransform fill;
        public Image fillImage;
        public Text value;
    }

    private static readonly Color GoodColor = new Color(0.42f, 1f, 0.54f);
    private static readonly Color WarnColor = new Color(1f, 0.82f, 0.3f);
    private static readonly Color BadColor = new Color(1f, 0.4f, 0.35f);
    private static readonly Color ButtonColor = new Color(0.2f, 0.3f, 0.45f, 1f);

    private readonly Gauge[] gauges = new Gauge[3];
    private GameObject root;
    private Text titleText;
    private Text statusText;
    private Text infoText;
    private Text hintText;
    private Button primaryButton;
    private Text primaryLabel;
    private Font font;

    private Mode mode = Mode.None;
    private WaterPurifier purifier;
    private OxygenGenerator generator;
    private SolarPanel solarPanel;
    private SolarPanel[] allPanels = new SolarPanel[0];
    private Action onRepairRequested;
    private Action onClosed;

    private bool previousMovementEnabled;
    private int openedFrame = -1;
    private float refreshTimer;
    private string feedback;
    private float feedbackUntil;

    /// <summary>True selama panel mesin terbuka.</summary>
    public static bool IsOpen { get; private set; }

    private void Awake()
    {
        IsOpen = false;

        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("MachinePanelUI: Canvas tidak ditemukan. Pasang komponen ini pada Canvas.", this);
            enabled = false;
            return;
        }

        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        BuildUI();
        root.SetActive(false);
    }

    private void Start()
    {
        if (player == null) player = FindFirstObjectByType<PlayerMovement>();
    }

    private void OnDestroy()
    {
        IsOpen = false;
    }

    // ---------- Membuka dan menutup ----------

    /// <summary>Membuka panel Water Purifier. Mengembalikan false bila sudah ada panel terbuka atau belum siap.</summary>
    public bool Open(WaterPurifier target, Action onClosedCallback)
    {
        if (target == null || !BeginOpen(Mode.Purifier, "Water Purifier", onClosedCallback)) return false;

        purifier = target;
        FinishOpen("Ubah es menjadi Water. Es diambil dari tas.");
        return true;
    }

    /// <summary>Membuka panel Oxygen Generator. Mengembalikan false bila sudah ada panel terbuka atau belum siap.</summary>
    public bool Open(OxygenGenerator target, Action onClosedCallback)
    {
        if (target == null || !BeginOpen(Mode.Generator, "Oxygen Generator", onClosedCallback)) return false;

        generator = target;
        FinishOpen(null);
        return true;
    }

    /// <summary>
    /// Membuka panel Solar Panel. repairCallback dipanggil (setelah panel menutup) saat tombol PERBAIKI ditekan;
    /// pemanggil yang membuka minigame. initialFeedback boleh null.
    /// </summary>
    public bool Open(SolarPanel target, Action repairCallback, Action onClosedCallback, string initialFeedback)
    {
        if (target == null || !BeginOpen(Mode.Solar, string.IsNullOrEmpty(target.name) ? "Solar Panel" : target.name, onClosedCallback)) return false;

        solarPanel = target;
        onRepairRequested = repairCallback;
        allPanels = FindObjectsByType<SolarPanel>(FindObjectsSortMode.None);
        FinishOpen(initialFeedback);
        return true;
    }

    private bool BeginOpen(Mode newMode, string title, Action onClosedCallback)
    {
        if (!enabled || IsOpen || root == null) return false;

        mode = newMode;
        onClosed = onClosedCallback;
        titleText.text = title;

        if (player != null)
        {
            previousMovementEnabled = player.MovementEnabled;
            player.MovementEnabled = false;
        }

        openedFrame = Time.frameCount;
        IsOpen = true;
        return true;
    }

    private void FinishOpen(string initialFeedback)
    {
        feedback = initialFeedback;
        feedbackUntil = Time.unscaledTime + feedbackSeconds;
        refreshTimer = 0f;
        root.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        if (!IsOpen) return;

        root.SetActive(false);
        IsOpen = false;
        mode = Mode.None;
        purifier = null;
        generator = null;
        solarPanel = null;
        onRepairRequested = null;

        if (player != null) player.MovementEnabled = previousMovementEnabled;

        Action callback = onClosed;
        onClosed = null;
        callback?.Invoke();
    }

    private void Update()
    {
        if (!IsOpen || Time.frameCount == openedFrame) return;

        if (Input.GetKeyDown(closeKey))
        {
            Close();
            return;
        }

        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer > 0f) return;

        refreshTimer = refreshInterval;
        Refresh();
    }

    // ---------- Tombol utama ----------

    private void OnPrimaryClicked()
    {
        if (!IsOpen) return;

        switch (mode)
        {
            case Mode.Purifier:
                if (purifier == null) break;
                string message;
                purifier.TryStartProcess(out message);
                ShowFeedback(message);
                break;

            case Mode.Generator:
                if (generator == null) break;
                generator.SetOn(!generator.IsOn);
                ShowFeedback(generator.IsOn
                    ? "Oxygen Generator dinyalakan."
                    : "Oxygen Generator dimatikan. Water dan Energy tidak dipakai mesin ini.");
                break;

            case Mode.Solar:
                if (solarPanel == null || !solarPanel.IsDamaged) break;
                Action repair = onRepairRequested;
                Close();               // minigame memulihkan dan mengunci gerakan sendiri
                repair?.Invoke();
                return;
        }

        Refresh();
    }

    private void ShowFeedback(string message)
    {
        feedback = message;
        feedbackUntil = Time.unscaledTime + feedbackSeconds;
    }

    // ---------- Pembaruan tampilan ----------

    private void Refresh()
    {
        switch (mode)
        {
            case Mode.Purifier: RefreshPurifier(); break;
            case Mode.Generator: RefreshGenerator(); break;
            case Mode.Solar: RefreshSolar(); break;
        }

        bool showFeedback = !string.IsNullOrEmpty(feedback) && Time.unscaledTime < feedbackUntil;
        if (showFeedback) hintText.text = feedback;
    }

    private void RefreshPurifier()
    {
        if (purifier == null) { Close(); return; }

        ResourceManager resources = ResourceManager.Instance;
        InventorySystem inventory = InventorySystem.Instance;
        bool processing = purifier.IsProcessing;
        string blocker = purifier.GetStartBlocker();

        if (processing)
            SetStatus("MEMPROSES", WarnColor, "  sisa " + Mathf.CeilToInt(purifier.RemainingSeconds) + " dtk");
        else
            SetStatus("SIAP", GoodColor, string.Empty);

        SetGauge(0, "Proses", purifier.Progress01, processing ? Mathf.CeilToInt(purifier.RemainingSeconds) + " dtk" : "-", WarnColor);

        if (resources != null)
        {
            float energy = resources.Get(ResourceType.Energy);
            SetGauge(1, "Energy", Fraction(energy, resources.GetMax(ResourceType.Energy)),
                Mathf.FloorToInt(energy) + " / " + Mathf.RoundToInt(resources.GetMax(ResourceType.Energy)) +
                "   (butuh " + Mathf.CeilToInt(purifier.EnergyCost) + ")", energy >= purifier.EnergyCost ? GoodColor : BadColor);

            float water = resources.Get(ResourceType.Water);
            SetGauge(2, "Water tangki", Fraction(water, resources.GetMax(ResourceType.Water)),
                Mathf.FloorToInt(water) + " / " + Mathf.RoundToInt(resources.GetMax(ResourceType.Water)) +
                "   (+" + Mathf.CeilToInt(purifier.WaterOutput) + "/proses)", GoodColor);
        }

        int ice = inventory != null ? inventory.GetBag(ItemType.Ice) : 0;
        infoText.text = "Es di tas: " + ice + "   (butuh " + purifier.IceCost + " per proses)\n" +
            purifier.IceCost + " Es + " + Mathf.CeilToInt(purifier.EnergyCost) + " Energy  \u2192  " +
            Mathf.CeilToInt(purifier.WaterOutput) + " Water   |   Durasi " + Mathf.CeilToInt(purifier.ProcessSeconds) + " dtk";

        primaryLabel.text = "PROSES";
        primaryButton.interactable = blocker == null;
        hintText.text = blocker ?? string.Empty;
    }

    private void RefreshGenerator()
    {
        if (generator == null) { Close(); return; }

        ResourceManager resources = ResourceManager.Instance;
        string status = generator.StatusText;

        Color statusColor;
        if (!generator.IsOn) statusColor = BadColor;
        else if (generator.IsRunning) statusColor = GoodColor;
        else if (status.StartsWith("berhenti")) statusColor = BadColor;
        else statusColor = WarnColor;
        SetStatus(char.ToUpper(status[0]) + status.Substring(1), statusColor, string.Empty);

        if (resources != null)
        {
            float oxygen = resources.Get(ResourceType.HabitatOxygen);
            float oxygenMax = resources.GetMax(ResourceType.HabitatOxygen);
            SetGauge(0, "Oxygen habitat", Fraction(oxygen, oxygenMax),
                Mathf.FloorToInt(oxygen) + " / " + Mathf.RoundToInt(oxygenMax), GoodColor);

            float water = resources.Get(ResourceType.Water);
            SetGauge(1, "Water", Fraction(water, resources.GetMax(ResourceType.Water)),
                Mathf.FloorToInt(water) + " / " + Mathf.RoundToInt(resources.GetMax(ResourceType.Water)) +
                "   (-" + generator.WaterCostPerCycle.ToString("0.#") + "/siklus)",
                water >= generator.WaterCostPerCycle ? GoodColor : BadColor);

            float energy = resources.Get(ResourceType.Energy);
            SetGauge(2, "Energy", Fraction(energy, resources.GetMax(ResourceType.Energy)),
                Mathf.FloorToInt(energy) + " / " + Mathf.RoundToInt(resources.GetMax(ResourceType.Energy)) +
                "   (-" + generator.EnergyCostPerCycle.ToString("0.#") + "/siklus)",
                energy >= generator.EnergyCostPerCycle ? GoodColor : BadColor);
        }

        infoText.text = "Tiap " + generator.CycleSeconds.ToString("0.#") + " dtk: " +
            generator.WaterCostPerCycle.ToString("0.#") + " Water + " + generator.EnergyCostPerCycle.ToString("0.#") +
            " Energy  \u2192  " + generator.OxygenPerCycle.ToString("0.#") + " Oxygen\n" +
            "Mulai mengisi di bawah " + Mathf.RoundToInt(generator.RestartBelowFraction * 100f) +
            " persen oksigen habitat, berhenti saat penuh.";

        primaryLabel.text = !generator.AllowManualToggle ? "OTOMATIS" : (generator.IsOn ? "MATIKAN" : "NYALAKAN");
        primaryButton.interactable = generator.AllowManualToggle;
        hintText.text = generator.AllowManualToggle
            ? "Matikan mesin untuk menghemat Water bagi tanaman."
            : "Mesin selalu menyala (tombol manual dimatikan di Inspector).";
    }

    private void RefreshSolar()
    {
        if (solarPanel == null) { Close(); return; }

        ResourceManager resources = ResourceManager.Instance;
        bool damaged = solarPanel.IsDamaged;

        if (damaged) SetStatus("RUSAK", BadColor, "  produksi Energy menurun");
        else SetStatus("NORMAL", GoodColor, string.Empty);

        float panelNow = solarPanel.CurrentEnergyPerSecond;
        float panelMax = solarPanel.NormalEnergyPerSecond;
        SetGauge(0, "Output panel ini", Fraction(panelNow, panelMax),
            panelNow.ToString("0.0#") + " / " + panelMax.ToString("0.0#") + "/dtk", damaged ? BadColor : GoodColor);

        float totalNow = 0f;
        float totalMax = 0f;
        int damagedCount = 0;
        for (int i = 0; i < allPanels.Length; i++)
        {
            if (allPanels[i] == null) continue;
            totalNow += allPanels[i].CurrentEnergyPerSecond;
            totalMax += allPanels[i].NormalEnergyPerSecond;
            if (allPanels[i].IsDamaged) damagedCount++;
        }
        SetGauge(1, "Output total", Fraction(totalNow, totalMax),
            totalNow.ToString("0.0#") + " / " + totalMax.ToString("0.0#") + "/dtk", damagedCount > 0 ? WarnColor : GoodColor);

        if (resources != null)
        {
            float energy = resources.Get(ResourceType.Energy);
            SetGauge(2, "Energy habitat", Fraction(energy, resources.GetMax(ResourceType.Energy)),
                Mathf.FloorToInt(energy) + " / " + Mathf.RoundToInt(resources.GetMax(ResourceType.Energy)), GoodColor);
        }

        infoText.text = "Panel rusak: " + damagedCount + " dari " + allPanels.Length + "\n" +
            "Panel rusak hanya menghasilkan sebagian kecil Energy. Perbaiki lewat minigame kabel.";

        primaryLabel.text = "PERBAIKI";
        primaryButton.interactable = damaged;
        if (damaged)
        {
            hintText.text = "Sambungkan kabel dengan warna yang sama untuk memperbaiki panel.";
        }
        else if (damagedCount > 0)
        {
            // Panel ini normal, tombol PERBAIKI nonaktif: tunjukkan panel mana yang rusak.
            System.Text.StringBuilder names = new System.Text.StringBuilder();
            for (int i = 0; i < allPanels.Length; i++)
            {
                if (allPanels[i] == null || !allPanels[i].IsDamaged) continue;
                if (names.Length > 0) names.Append(", ");
                names.Append(allPanels[i].name);
            }
            hintText.text = "Panel ini normal. Panel yang rusak: " + names + ".";
        }
        else
        {
            hintText.text = "Panel berfungsi normal.";
        }
    }

    private static float Fraction(float value, float max) => max > 0f ? Mathf.Clamp01(value / max) : 0f;

    private void SetStatus(string label, Color color, string suffix)
    {
        statusText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + label + "</color>" + suffix;
    }

    private void SetGauge(int index, string label, float fraction, string value, Color color)
    {
        Gauge g = gauges[index];
        g.label.text = label;
        g.value.text = value;
        g.fill.anchorMax = new Vector2(Mathf.Clamp01(fraction), 1f);
        g.fillImage.color = color;
    }

    // ---------- Pembuatan tampilan ----------

    private void BuildUI()
    {
        root = new GameObject("MachinePanelRoot", typeof(RectTransform), typeof(Image));
        root.transform.SetParent(canvas.transform, false);
        Place((RectTransform)root.transform, Vector2.zero, new Vector2(820f, 560f));
        root.GetComponent<Image>().color = new Color(0.05f, 0.07f, 0.1f, 0.95f);

        titleText = MakeText(root.transform, "Title", string.Empty, 32, TextAnchor.MiddleCenter);
        Place(titleText.rectTransform, new Vector2(0f, 240f), new Vector2(760f, 50f));

        statusText = MakeText(root.transform, "Status", string.Empty, 26, TextAnchor.MiddleCenter);
        statusText.supportRichText = true;
        Place(statusText.rectTransform, new Vector2(0f, 190f), new Vector2(760f, 40f));

        for (int i = 0; i < gauges.Length; i++) gauges[i] = BuildGauge(110f - i * 70f);

        infoText = MakeText(root.transform, "Info", string.Empty, 20, TextAnchor.MiddleCenter);
        Place(infoText.rectTransform, new Vector2(0f, -105f), new Vector2(760f, 70f));

        hintText = MakeText(root.transform, "Hint", string.Empty, 20, TextAnchor.MiddleCenter);
        hintText.color = WarnColor;
        Place(hintText.rectTransform, new Vector2(0f, -165f), new Vector2(760f, 40f));

        GameObject primary = MakeButton("Primary", new Vector2(-120f, -228f), OnPrimaryClicked);
        primaryButton = primary.GetComponent<Button>();
        primaryLabel = primary.GetComponentInChildren<Text>();
        primaryLabel.fontSize = 24;

        MakeButton("Tutup", new Vector2(120f, -228f), Close);
    }

    private Gauge BuildGauge(float y)
    {
        Gauge g = new Gauge();

        g.label = MakeText(root.transform, "GaugeLabel", string.Empty, 22, TextAnchor.MiddleLeft);
        Place(g.label.rectTransform, new Vector2(-290f, y), new Vector2(200f, 30f));

        GameObject back = new GameObject("GaugeBack", typeof(RectTransform), typeof(Image));
        back.transform.SetParent(root.transform, false);
        Place((RectTransform)back.transform, new Vector2(-30f, y), new Vector2(300f, 24f));
        back.GetComponent<Image>().color = new Color(0.15f, 0.18f, 0.22f, 1f);

        GameObject fill = new GameObject("GaugeFill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(back.transform, false);
        g.fill = (RectTransform)fill.transform;
        g.fill.anchorMin = Vector2.zero;
        g.fill.anchorMax = new Vector2(0f, 1f);
        g.fill.offsetMin = Vector2.zero;
        g.fill.offsetMax = Vector2.zero;
        g.fillImage = fill.GetComponent<Image>();
        g.fillImage.color = GoodColor;
        g.fillImage.raycastTarget = false;

        g.value = MakeText(root.transform, "GaugeValue", string.Empty, 18, TextAnchor.MiddleLeft);
        Place(g.value.rectTransform, new Vector2(265f, y), new Vector2(270f, 30f));
        return g;
    }

    private GameObject MakeButton(string label, Vector2 position, UnityAction onClick)
    {
        GameObject go = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(root.transform, false);
        Place((RectTransform)go.transform, position, new Vector2(220f, 56f));

        Image image = go.GetComponent<Image>();
        image.color = ButtonColor;
        Button button = go.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        Text text = MakeText(go.transform, "Label", label, 22, TextAnchor.MiddleCenter);
        RectTransform r = text.rectTransform;
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
        return go;
    }

    private Text MakeText(Transform parent, string name, string content, int size, TextAnchor alignment)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        Text text = go.GetComponent<Text>();
        text.font = font;
        text.text = content;
        text.fontSize = size;
        text.color = Color.white;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        return text;
    }

    private static void Place(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }
}

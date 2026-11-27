using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Otak TARS (Milestone 11): asisten habitat berbasis event dan ambang, BUKAN pemberi quest dan BUKAN AI generatif.
/// Semua teks tetap (predefined). TARS hanya memberi tahu dan mengingatkan; tidak menentukan urutan aksi pemain.
///
/// Yang dikerjakan:
/// 1. Antrean pesan dengan PRIORITAS (Info/Warning/Critical), COOLDOWN per pesan, jeda antar pesan, dan kedaluwarsa,
///    agar dialog tidak menumpuk atau berulang-ulang. Pesan hanya tampil saat tidak ada dialog/panel lain
///    dan tidak ada transisi tidur atau minigame (TimeManager.ExternalPause).
/// 2. Pemantau ambang (Water, Oxygen habitat, Energy, stok Food di storage, Oxygen suit): peringatan di batas
///    "menipis" dan "kritis" (default 30 persen dan 10 persen, dapat diubah) dan saat habis. Tiap tingkat hanya
///    diumumkan sekali per penurunan; setelah pulih, bisa diumumkan lagi.
/// 3. Solar Panel rusak, pengingat akhir hari, pesan awal hari (termasuk pesan dari Bumi: cerita, bukan quest).
/// 4. Pengingat "lepas suit" bila suit dikenakan di dalam habitat terlalu lama (dipindahkan dari SuitReminder).
/// 5. Laporan atas permintaan: dialog E pada NPC TARS (lihat TarsWanderer) memakai BuildConversationLines.
///
/// Sistem lain boleh memakai Announce(...) untuk pesan TARS (HungerReminder, ending Day 7 nanti).
/// Semua angka adalah USULAN; ubah lewat Inspector setelah playtest. Tampilan dialog ada di DialogueUI.
/// </summary>
public class TARSSystem : MonoBehaviour
{
    public enum Priority { Info = 1, Warning = 2, Critical = 3 }

    private class Message
    {
        public string key;
        public Priority priority;
        public Func<string[]> buildLines;
        public Func<bool> isStillValid;
        public float queuedAt;
        public int order;
    }

    public static TARSSystem Instance { get; private set; }

    /// <summary>True bila masih ada pesan TARS yang mengantre (akan tampil sebagai dialog). Dipakai TutorialGuideUI agar garis arah tidak tampil bersamaan dengan dialog.</summary>
    public bool HasPendingMessages => pending.Count > 0;

    [Header("Referensi (otomatis dicari jika kosong)")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private string speakerName = "TARS";

    [Header("Ambang resource (USULAN, dapat dikonfigurasi)")]
    [Tooltip("Bagian dari kapasitas maksimum di bawah ini dianggap menipis (0,3 = 30 persen).")]
    [SerializeField, Range(0f, 1f)] private float warnFraction = 0.3f;
    [Tooltip("Bagian dari kapasitas maksimum di bawah ini dianggap kritis (0,1 = 10 persen).")]
    [SerializeField, Range(0f, 1f)] private float dangerFraction = 0.1f;

    [Header("Cooldown dan antrean (USULAN)")]
    [Tooltip("Jeda minimum setelah satu pesan TARS selesai sebelum pesan TARS berikutnya tampil (detik).")]
    [SerializeField, Min(0f)] private float globalCooldownSeconds = 4f;
    [Tooltip("Pesan dengan kunci yang sama tidak diumumkan lagi sebelum jeda ini lewat sejak tampil (detik).")]
    [SerializeField, Min(0f)] private float repeatCooldownSeconds = 45f;
    [Tooltip("Pesan Info/Warning yang menunggu lebih lama dari ini dibuang (detik). Pesan Critical tidak dibuang.")]
    [SerializeField, Min(1f)] private float expirySeconds = 90f;

    [Header("Akhir hari (USULAN)")]
    [Tooltip("Pengingat akhir hari saat sisa waktu sama dengan atau kurang dari angka ini (detik).")]
    [SerializeField, Min(1f)] private float endOfDayWarnSeconds = 60f;

    [Header("Pengingat suit di dalam habitat (dipindahkan dari SuitReminder, USULAN)")]
    [SerializeField, Min(1f)] private float suitIndoorFirstSeconds = 30f;
    [SerializeField, Min(1f)] private float suitIndoorRepeatSeconds = 90f;

    [Header("Pesan awal hari")]
    [Tooltip("Tampilkan pesan awal hari juga untuk hari yang sedang berjalan saat game mulai (Day 1).")]
    [SerializeField] private bool announceDayAtStart = true;
    [Tooltip("Pesan dari Bumi per hari (cerita, BUKAN quest). Elemen 0 = Day 1. Kosongkan elemen untuk tanpa pesan. " +
             "Isi bawaan hanyalah PLACEHOLDER; tulis teks cerita Anda sendiri.")]
    [TextArea(2, 4)]
    [SerializeField] private string[] earthMessages =
    {
        "[PLACEHOLDER] Bumi: Sinyal Anda terdengar. Tim penjemput sedang dalam perjalanan.",
        "[PLACEHOLDER] Bumi: Kami menerima laporan habitat Anda. Bertahanlah.",
        "[PLACEHOLDER] Bumi: Penjemputan masih sesuai jadwal.",
        "[PLACEHOLDER] Bumi: Pertengahan perjalanan. Jaga kondisi Anda.",
        "[PLACEHOLDER] Bumi: Tim penjemput mendekati orbit Mars.",
        "[PLACEHOLDER] Bumi: Tinggal sedikit lagi.",
        "[PLACEHOLDER] Bumi: Tim penjemput tiba. Bertahanlah sampai akhir hari ini."
    };

    private static readonly int ResourceCount = Enum.GetValues(typeof(ResourceType)).Length;

    private readonly List<Message> pending = new List<Message>();
    private readonly Dictionary<string, float> nextAllowedByKey = new Dictionary<string, float>();
    private readonly int[] announcedLevel = new int[ResourceCount];

    private ResourceManager resources;
    private TimeManager time;
    private SuitSystem suit;
    private float nextShowTime;
    private int orderCounter;
    private bool endOfDayWarned;
    private float suitIndoorSeconds;
    private float suitIndoorThreshold;

    private static readonly ResourceType[] Monitored =
        { ResourceType.Water, ResourceType.HabitatOxygen, ResourceType.Energy, ResourceType.Food };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("TARSSystem: sudah ada instance lain, objek ini dihapus.", this);
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (dialogueUI == null) dialogueUI = FindFirstObjectByType<DialogueUI>();
        resources = ResourceManager.Instance;
        time = TimeManager.Instance;
        suit = SuitSystem.Instance != null ? SuitSystem.Instance : FindFirstObjectByType<SuitSystem>();
        suitIndoorThreshold = suitIndoorFirstSeconds;

        if (dialogueUI == null)
            Debug.LogWarning("TARSSystem: DialogueUI tidak ditemukan; pesan hanya masuk Console.", this);

        // Berlangganan di Start (bukan OnEnable) karena urutan Awake/OnEnable antar objek tidak terjamin.
        if (resources != null)
        {
            resources.OnResourceChanged += HandleResourceChanged;
            foreach (ResourceType type in Monitored) UpdateResourceLevel(type);
        }
        else
        {
            Debug.LogError("TARSSystem: ResourceManager tidak ada di scene.", this);
        }

        if (suit != null) suit.OnEquippedChanged += HandleEquippedChanged;

        if (time != null)
        {
            time.OnDayStarted += HandleDayStarted;
            time.OnDayTimeEnded += HandleDayTimeEnded;
            if (announceDayAtStart) AnnounceDayStart(time.CurrentDay);
        }

        // Semua Solar Panel berbagi satu status rusak, jadi cukup satu langganan.
        SolarPanel.OnArrayDamagedChanged += HandleSolarDamaged;
        if (SolarPanel.ArrayDamaged) HandleSolarDamaged(true);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;

        if (resources != null) resources.OnResourceChanged -= HandleResourceChanged;
        if (suit != null) suit.OnEquippedChanged -= HandleEquippedChanged;
        if (time != null)
        {
            time.OnDayStarted -= HandleDayStarted;
            time.OnDayTimeEnded -= HandleDayTimeEnded;
        }

        SolarPanel.OnArrayDamagedChanged -= HandleSolarDamaged;
    }

    private void Update()
    {
        CheckEndOfDay();
        CheckSuitIndoorReminder();
        ProcessQueue();
    }

    // ---------- API untuk sistem lain ----------

    /// <summary>
    /// Mengantrekan pesan TARS. Kunci yang sama tidak diantrekan ganda (isi diperbarui) dan tidak diumumkan ulang
    /// selama cooldown. buildLines dipanggil saat pesan akan tampil, jadi angka di dalamnya selalu terbaru.
    /// isStillValid (opsional) dipanggil sebelum tampil; bila false pesan dibuang (misalnya masalah sudah teratasi).
    /// </summary>
    public void Announce(string key, Priority priority, Func<string[]> buildLines, Func<bool> isStillValid = null)
    {
        if (string.IsNullOrEmpty(key) || buildLines == null) return;
        if (nextAllowedByKey.TryGetValue(key, out float allowedAt) && Time.time < allowedAt) return;

        foreach (Message existing in pending)
        {
            if (existing.key != key) continue;

            existing.priority = priority;
            existing.buildLines = buildLines;
            existing.isStillValid = isStillValid;
            return;
        }

        pending.Add(new Message
        {
            key = key,
            priority = priority,
            buildLines = buildLines,
            isStillValid = isStillValid,
            queuedAt = Time.time,
            order = orderCounter++
        });
    }

    /// <summary>Versi sederhana: satu teks tetap.</summary>
    public void Announce(string key, Priority priority, string text)
    {
        Announce(key, priority, () => new[] { text });
    }

    /// <summary>
    /// Baris dialog saat pemain mengajak TARS bicara (E): pengantar dari Inspector TarsWanderer, pesan Bumi hari ini,
    /// laporan fasilitas, kondisi pemain, dan ringkasan stok. Hanya informasi, tanpa quest.
    /// </summary>
    public string[] BuildConversationLines(string[] introLines)
    {
        List<string> lines = new List<string>();
        if (introLines != null) lines.AddRange(introLines);

        string earth = GetEarthMessage(time != null ? time.CurrentDay : 1);
        if (!string.IsNullOrEmpty(earth)) lines.Add(earth);

        lines.Add(BuildFacilityReport());
        lines.Add(BuildPlayerReport());
        lines.Add(BuildStockReport());
        return lines.ToArray();
    }

    // ---------- Resource ----------

    private void HandleResourceChanged(ResourceType type, float value, float max)
    {
        if (type == ResourceType.SuitOxygen) UpdateSuitLevel();
        else if (Array.IndexOf(Monitored, type) >= 0) UpdateResourceLevel(type);
    }

    /// <summary>0 = aman, 1 = menipis, 2 = kritis, 3 = habis.</summary>
    private int GetLevel(float value, float max)
    {
        if (max <= 0f) return 0;
        if (value <= 0.001f) return 3;

        float fraction = value / max;
        if (fraction <= dangerFraction) return 2;
        if (fraction <= warnFraction) return 1;
        return 0;
    }

    private void UpdateResourceLevel(ResourceType type)
    {
        int index = (int)type;
        int level = GetLevel(resources.Get(type), resources.GetMax(type));

        if (level < announcedLevel[index])
        {
            announcedLevel[index] = level; // pulih sebagian; tingkat ini boleh diumumkan lagi bila turun lagi
            return;
        }

        if (level == announcedLevel[index]) return;

        announcedLevel[index] = level;
        if (level == 0) return;

        int announced = level;
        Announce("res:" + type + ":" + level,
            level == 1 ? Priority.Warning : Priority.Critical,
            () => new[] { BuildResourceLine(type, announced) },
            () => GetLevel(resources.Get(type), resources.GetMax(type)) >= announced);
    }

    private string BuildResourceLine(ResourceType type, int level)
    {
        string label = ResourceLabel(type);
        string amount = Mathf.RoundToInt(resources.Get(type)) + " / " + Mathf.RoundToInt(resources.GetMax(type));
        string head;
        switch (level)
        {
            case 1: head = label + " menipis: " + amount + "."; break;
            case 2: head = "Peringatan. " + label + " sangat rendah: " + amount + "."; break;
            default: head = label + " habis."; break;
        }

        return head + " " + ResourceHint(type);
    }

    private static string ResourceLabel(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Water: return "Stok Water";
            case ResourceType.HabitatOxygen: return "Oxygen habitat";
            case ResourceType.Energy: return "Energy";
            case ResourceType.Food: return "Stok Food di storage";
            default: return type.ToString();
        }
    }

    private static string ResourceHint(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Water: return "Water dipakai untuk farming dan Oxygen Generator, dan dihasilkan Water Purifier dari Es.";
            case ResourceType.HabitatOxygen: return "Oxygen habitat diisi Oxygen Generator dari Water dan Energy.";
            case ResourceType.Energy: return "Energy dihasilkan Solar Panel; panel rusak menghasilkan lebih sedikit.";
            case ResourceType.Food: return "Kentang dipanen dari Farming Plot dan disimpan di storage.";
            default: return string.Empty;
        }
    }

    // ---------- Suit ----------

    private int announcedSuitLevel;

    private void HandleEquippedChanged(bool equipped)
    {
        announcedSuitLevel = 0;
        suitIndoorSeconds = 0f;
        suitIndoorThreshold = suitIndoorFirstSeconds;
        if (equipped) UpdateSuitLevel();
    }

    private void UpdateSuitLevel()
    {
        if (resources == null || suit == null) return;

        if (!suit.IsEquipped)
        {
            announcedSuitLevel = 0;
            return;
        }

        int level = GetLevel(resources.Get(ResourceType.SuitOxygen), resources.GetMax(ResourceType.SuitOxygen));
        if (level < announcedSuitLevel)
        {
            announcedSuitLevel = level;
            return;
        }

        if (level == announcedSuitLevel) return;

        announcedSuitLevel = level;
        if (level == 0) return;

        int announced = level;
        Announce("suit:oxygen:" + level,
            level == 1 ? Priority.Warning : Priority.Critical,
            () => new[] { BuildSuitOxygenLine(announced) },
            () => suit != null && suit.IsEquipped &&
                  GetLevel(resources.Get(ResourceType.SuitOxygen), resources.GetMax(ResourceType.SuitOxygen)) >= announced);
    }

    private string BuildSuitOxygenLine(int level)
    {
        float max = resources.GetMax(ResourceType.SuitOxygen);
        int percent = max > 0f ? Mathf.RoundToInt(resources.Get(ResourceType.SuitOxygen) / max * 100f) : 0;
        bool outdoors = suit != null && suit.IsOutdoors;

        if (level >= 3)
            return "Oksigen suit habis." + (outdoors ? " Anda masih di luar habitat. Kembali ke dalam secepatnya." : "");

        string head = level == 2 ? "Peringatan. Oksigen suit kritis: " : "Oksigen suit menipis: ";
        string tail = outdoors
            ? " Kembali ke habitat sebelum oksigen suit habis."
            : " Lepas suit di Suit Station bila Anda tidak akan keluar.";
        return head + percent + " persen." + tail;
    }

    // Dipindahkan dari SuitReminder (11.3.1): suit dikenakan di dalam habitat terlalu lama.
    private void CheckSuitIndoorReminder()
    {
        if (suit == null || resources == null) return;

        if (!suit.IsEquipped || suit.IsOutdoors)
        {
            suitIndoorSeconds = 0f;
            suitIndoorThreshold = suitIndoorFirstSeconds;
            return;
        }

        if (time != null && time.IsPaused) return;
        if (dialogueUI != null && dialogueUI.IsOpen) return;

        suitIndoorSeconds += Time.deltaTime;
        if (suitIndoorSeconds < suitIndoorThreshold) return;

        suitIndoorSeconds = 0f;
        suitIndoorThreshold = suitIndoorRepeatSeconds;

        Announce("suit:indoor", Priority.Info,
            () => new[]
            {
                "Anda masih mengenakan suit di dalam habitat. Oksigen suit terus berkurang. " +
                "Lepas suit di Suit Station jika Anda tidak akan keluar. Oksigen suit: " +
                Mathf.RoundToInt(resources.Get(ResourceType.SuitOxygen)) + " / " +
                Mathf.RoundToInt(resources.GetMax(ResourceType.SuitOxygen)) + "."
            },
            () => suit != null && suit.IsEquipped && !suit.IsOutdoors);
    }

    // ---------- Solar Panel ----------

    private void HandleSolarDamaged(bool damaged)
    {
        if (!damaged) return;

        // Satu kunci untuk seluruh Solar Panel (awalan "panel:" dipakai ProcessQueue agar pesan tidak kedaluwarsa).
        Announce("panel:solar", Priority.Warning,
            () => new[]
            {
                "Solar Panel mengalami gangguan. Produksi energi berhenti sampai diperbaiki. " +
                "Solar Panel dapat diperbaiki di Repair Point outdoor."
            },
            () => SolarPanel.ArrayDamaged);
    }

    // ---------- Hari ----------

    private void HandleDayStarted(int day)
    {
        endOfDayWarned = false;
        AnnounceDayStart(day);
    }

    private void AnnounceDayStart(int day)
    {
        Announce("day:start:" + day, Priority.Info, () =>
        {
            string head = "Hari " + day + " dari " + (time != null ? time.TotalDays : 7) + " dimulai.";
            string earth = GetEarthMessage(day);
            return string.IsNullOrEmpty(earth) ? new[] { head } : new[] { head, earth };
        });
    }

    private string GetEarthMessage(int day)
    {
        if (earthMessages == null || day < 1 || day > earthMessages.Length) return null;
        return earthMessages[day - 1];
    }

    private void CheckEndOfDay()
    {
        if (time == null || endOfDayWarned || time.IsDayOver) return;
        if (time.RemainingSeconds > endOfDayWarnSeconds) return;

        endOfDayWarned = true;
        Announce("day:end", Priority.Warning, () =>
        {
            bool outdoors = suit != null && suit.IsOutdoors;
            string outdoorNote = outdoors ? " Anda masih di luar habitat; sebaiknya kembali ke dalam." : "";
            return new[]
            {
                "Waktu hari ini hampir habis (sisa sekitar " + Mathf.CeilToInt(time.RemainingSeconds) +
                " detik). Hari berakhir saat Anda tidur di tempat tidur Control Room." + outdoorNote
            };
        });
    }

    private void HandleDayTimeEnded()
    {
        Announce("day:over", Priority.Critical,
            "Waktu hari ini habis. Kembali ke Control Room dan gunakan tempat tidur untuk mengakhiri hari.");
    }

    // ---------- Antrean ----------

    private void ProcessQueue()
    {
        if (pending.Count == 0 || Time.time < nextShowTime) return;
        if (dialogueUI != null && dialogueUI.IsOpen) return;
        if (time != null && time.ExternalPause) return;   // tidur atau minigame sedang berjalan
        if (StorageUI.IsOpen || MachinePanelUI.IsOpen) return;

        for (int i = pending.Count - 1; i >= 0; i--)
        {
            Message m = pending[i];
            bool invalid = m.isStillValid != null && !m.isStillValid();
            // Pesan panel rusak tidak kedaluwarsa selama panel masih rusak (isStillValid yang membuangnya).
            bool neverExpires = m.priority == Priority.Critical || m.key.StartsWith("panel:");
            bool expired = !neverExpires && Time.time - m.queuedAt > expirySeconds;
            if (invalid || expired) pending.RemoveAt(i);
        }

        if (pending.Count == 0) return;

        Message best = pending[0];
        foreach (Message m in pending)
        {
            if (m.priority > best.priority || (m.priority == best.priority && m.order < best.order)) best = m;
        }

        pending.Remove(best);
        Show(best);
    }

    private void Show(Message message)
    {
        string[] lines = message.buildLines();
        if (lines == null || lines.Length == 0) return;

        nextAllowedByKey[message.key] = Time.time + repeatCooldownSeconds;

        if (dialogueUI == null)
        {
            Debug.Log("TARS: " + string.Join(" ", lines));
            nextShowTime = Time.time + globalCooldownSeconds;
            return;
        }

        nextShowTime = Time.time + globalCooldownSeconds;
        dialogueUI.Show(speakerName, lines, () => nextShowTime = Time.time + globalCooldownSeconds);
    }

    // ---------- Laporan (dialog E pada TARS) ----------

    private string BuildFacilityReport()
    {
        StringBuilder sb = new StringBuilder("Laporan fasilitas. ");

        SolarPanel[] panels = FindObjectsByType<SolarPanel>(FindObjectsSortMode.None);
        if (panels.Length == 0)
        {
            sb.Append("Solar Panel: tidak terdeteksi. ");
        }
        else
        {
            float energyPerSecond = 0f;
            foreach (SolarPanel panel in panels) energyPerSecond += panel.CurrentEnergyPerSecond;

            if (SolarPanel.ArrayDamaged)
                sb.Append("Solar Panel: RUSAK, produksi Energy berhenti. Diperbaiki di Repair Point outdoor. ");
            else
                sb.Append("Solar Panel: normal. Produksi Energy sekitar " + energyPerSecond.ToString("0.0") + " per detik. ");
        }

        foreach (WaterPurifier purifier in FindObjectsByType<WaterPurifier>(FindObjectsSortMode.None))
            sb.Append("Water Purifier: " + purifier.StatusText + ". ");
        foreach (OxygenGenerator generator in FindObjectsByType<OxygenGenerator>(FindObjectsSortMode.None))
            sb.Append("Oxygen Generator: " + generator.StatusText + ". ");

        return sb.ToString().TrimEnd();
    }

    private string BuildPlayerReport()
    {
        StringBuilder sb = new StringBuilder("Kondisi Anda. ");

        if (suit != null && resources != null)
        {
            sb.Append(suit.IsEquipped ? "Suit dikenakan" : "Suit belum dikenakan");
            sb.Append(", oksigen suit " + Mathf.RoundToInt(resources.Get(ResourceType.SuitOxygen)) + " / " +
                      Mathf.RoundToInt(resources.GetMax(ResourceType.SuitOxygen)) + ". ");
        }

        HungerSystem hunger = HungerSystem.Instance;
        if (hunger != null)
        {
            sb.Append("Rasa kenyang " + Mathf.RoundToInt(hunger.Fraction * 100f) + " persen");
            if (hunger.IsStarving)
                sb.Append(" (kelaparan, sisa waktu untuk makan sekitar " + Mathf.CeilToInt(hunger.StarvationSecondsLeft) + " detik)");
            else if (hunger.Fraction <= hunger.WarnFraction)
                sb.Append(" (mulai lapar)");
            sb.Append(". ");
        }

        return sb.ToString().TrimEnd();
    }

    private string BuildStockReport()
    {
        StringBuilder sb = new StringBuilder();

        if (resources != null)
        {
            sb.Append("Stok. Food " + Fmt(ResourceType.Food) + ", Water " + Fmt(ResourceType.Water) +
                      ", Oxygen habitat " + Fmt(ResourceType.HabitatOxygen) + ", Energy " + Fmt(ResourceType.Energy) + ". ");
        }

        if (time != null)
        {
            int seconds = Mathf.CeilToInt(time.RemainingSeconds);
            sb.Append("Hari " + time.CurrentDay + ", sisa waktu " + (seconds / 60).ToString("00") + ":" +
                      (seconds % 60).ToString("00") + ".");
        }

        return sb.ToString().TrimEnd();
    }

    private string Fmt(ResourceType type)
    {
        return Mathf.RoundToInt(resources.Get(type)) + "/" + Mathf.RoundToInt(resources.GetMax(type));
    }
}

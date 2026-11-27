using UnityEngine;

/// <summary>
/// Titik perbaikan Solar Panel di Outdoor. SATU RepairPoint melayani SEMUA Solar Panel di scene (servesAllPanels,
/// bawaan): panel yang dibuka adalah panel rusak pertama (urut nama lalu posisi kiri-ke-kanan); bila tidak ada yang
/// rusak, panel utama (field Panel) atau panel pertama. Selesai memperbaiki satu panel, panel rusak berikutnya
/// (bila ada) langsung ditawarkan. Matikan servesAllPanels bila ingin RepairPoint khusus satu panel. Tekan E: MachinePanelUI Solar Panel terbuka (status, output,
/// Energy); tombol PERBAIKI pada panel (aktif bila panel rusak) membuka minigame kabel (WiringMinigameUI), dan panel
/// diperbaiki (SolarPanel.Repair()) hanya setelah semua kabel tersambung. Esc membatalkan.
/// Tanpa MachinePanelUI di scene, E langsung membuka minigame bila panel rusak (perilaku lama, dialog bila normal).
/// Bila WiringMinigameUI tidak ada di scene, perbaikan langsung berhasil (cadangan, dengan peringatan di Console).
/// Pasang pada objek ber-layer Interactable dengan Collider2D.
/// </summary>
public class RepairPoint : MonoBehaviour, IInteractable
{
    [Tooltip("Panel utama / cadangan. Kosong boleh bila servesAllPanels aktif (dipakai panel pertama di scene).")]
    [SerializeField] private SolarPanel panel;
    [Tooltip("Satu RepairPoint melayani semua Solar Panel di scene. Matikan untuk melayani hanya Panel di atas.")]
    [SerializeField] private bool servesAllPanels = true;

    [Header("Referensi (otomatis dicari jika kosong)")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private WiringMinigameUI minigame;
    [SerializeField] private MachinePanelUI panelUI;

    [SerializeField, Min(0f)] private float reinteractCooldown = 0.3f;

    private float nextInteractTime;
    private SolarPanel[] allPanels = new SolarPanel[0];
    private SolarPanel target;

    /// <summary>Panel utama yang diisi di Inspector (bisa kosong).</summary>
    public SolarPanel Panel => panel;

    /// <summary>True bila RepairPoint ini dapat memperbaiki panel tersebut.</summary>
    public bool Serves(SolarPanel candidate)
    {
        if (candidate == null) return false;
        return servesAllPanels || candidate == panel;
    }

    public string InteractionPrompt
    {
        get
        {
            SolarPanel current = PickTarget();
            int damagedCount = CountDamaged();
            bool damaged = current != null && current.IsDamaged;
            string label = current != null && !string.IsNullOrEmpty(current.name) ? current.name : "Solar Panel";
            string suffix = damagedCount > 0 ? " (" + damagedCount + " rusak)" : string.Empty;
            if (panelUI != null) return damaged ? "Periksa " + label + " (RUSAK)" + suffix : "Periksa Solar Panel" + suffix;
            return damaged ? "Perbaiki " + label + suffix : "Periksa Solar Panel" + suffix;
        }
    }

    private void Start()
    {
        if (dialogueUI == null) dialogueUI = FindFirstObjectByType<DialogueUI>();
        if (minigame == null) minigame = FindFirstObjectByType<WiringMinigameUI>();
        if (panelUI == null) panelUI = FindFirstObjectByType<MachinePanelUI>();
        allPanels = FindObjectsByType<SolarPanel>(FindObjectsSortMode.None);
        System.Array.Sort(allPanels, ComparePanels);

        if (minigame == null)
            Debug.LogWarning("RepairPoint: WiringMinigameUI tidak ditemukan; perbaikan akan langsung berhasil. Jalankan menu Mars > Setup Panel Damage and Wiring Minigame.", this);

        if (PickTarget() == null)
            Debug.LogError("RepairPoint: tidak ada Solar Panel yang dilayani (isi Panel di Inspector atau pastikan ada SolarPanel di scene).", this);
    }

    public bool CanInteract(GameObject interactor)
    {
        if (PickTarget() == null || Time.time < nextInteractTime || MachinePanelUI.IsOpen) return false;
        if (minigame != null && minigame.IsOpen) return false;
        return dialogueUI == null || !dialogueUI.IsOpen;
    }

    public void Interact(GameObject interactor)
    {
        target = PickTarget();
        if (target == null) return;

        if (panelUI != null && panelUI.Open(target, BeginRepair, OnPanelClosed, null)) return;

        if (!target.IsDamaged)
        {
            ShowMessage("Solar Panel berfungsi normal. Produksi: " +
                target.CurrentEnergyPerSecond.ToString("0.0") + " Energy per detik.");
            return;
        }

        BeginRepair();
    }

    // Panel yang ditangani: rusak pertama; bila tidak ada, panel utama atau panel pertama di scene.
    private SolarPanel PickTarget()
    {
        if (!servesAllPanels) return panel;

        SolarPanel firstDamaged = null;
        foreach (SolarPanel candidate in allPanels)
        {
            if (candidate != null && candidate.IsDamaged) { firstDamaged = candidate; break; } // allPanels sudah terurut
        }
        if (firstDamaged != null) return firstDamaged;

        if (panel != null) return panel;
        foreach (SolarPanel candidate in allPanels)
        {
            if (candidate != null) return candidate;
        }
        return null;
    }

    private int CountDamaged()
    {
        if (!servesAllPanels) return panel != null && panel.IsDamaged ? 1 : 0;

        int count = 0;
        foreach (SolarPanel candidate in allPanels)
        {
            if (candidate != null && candidate.IsDamaged) count++;
        }
        return count;
    }

    private static int ComparePanels(SolarPanel a, SolarPanel b)
    {
        if (a == b) return 0;
        if (a == null) return 1;
        if (b == null) return -1;

        int byName = string.CompareOrdinal(a.name, b.name);
        if (byName != 0) return byName;
        return a.transform.position.x.CompareTo(b.transform.position.x);
    }

    // Membuka minigame kabel (dipanggil langsung atau lewat tombol PERBAIKI di MachinePanelUI).
    private void BeginRepair()
    {
        if (target == null || !target.IsDamaged) return;

        if (minigame == null)
        {
            OnRepairSucceeded(); // cadangan: tanpa minigame di scene, perbaikan langsung berhasil.
            return;
        }

        minigame.Open(target.name, OnRepairSucceeded, OnRepairCancelled);
    }

    private void OnPanelClosed()
    {
        nextInteractTime = Time.time + reinteractCooldown;
    }

    private void OnRepairSucceeded()
    {
        SolarPanel repaired = target;
        if (repaired == null) return;

        repaired.Repair();
        FloatingPopupManager.Show(FloatingPopupManager.AnchorAbove(repaired), PopupIcon.Repair, "Panel diperbaiki");

        // Tawarkan panel rusak berikutnya (bila ada); kalau tidak, tampilkan panel yang baru diperbaiki.
        SolarPanel next = PickTarget();
        if (next != null && next.IsDamaged) target = next; else target = repaired;

        int remaining = CountDamaged();
        string note = remaining > 0
            ? "Panel diperbaiki. Masih ada " + remaining + " panel rusak; tekan PERBAIKI untuk panel berikutnya."
            : "Panel diperbaiki. Produksi Energy kembali normal.";

        // Dengan panel mesin: kembali ke panel agar hasilnya (status, output) terlihat.
        if (panelUI != null && panelUI.Open(target, BeginRepair, OnPanelClosed, note)) return;

        ShowMessage(note);
    }

    private void OnRepairCancelled()
    {
        nextInteractTime = Time.time + reinteractCooldown;
    }

    private void ShowMessage(string message)
    {
        if (dialogueUI == null)
        {
            Debug.Log("RepairPoint: " + message);
            nextInteractTime = Time.time + reinteractCooldown;
            return;
        }

        dialogueUI.Show("Repair Point", new[] { message }, () => nextInteractTime = Time.time + reinteractCooldown);
    }
}

using System.Collections;
using UnityEngine;

/// <summary>
/// Pintu otomatis dengan aturan buka yang sama seperti SuitGate. Pintu terbuka bila pemain berada dalam
/// jarak openDistance dari objek ini DAN boleh lewat: suit dikenakan dengan oksigen cukup (SuitSystem.CanExit),
/// atau pemain sudah di luar (SuitSystem.IsOutdoors, agar pemain bisa masuk lagi seperti tembok SuitGate yang mati).
/// Saat terbuka, sprite berganti berurutan sesuai urutan array frames (frames[0] tertutup, frames[^1] terbuka penuh).
/// Saat syarat tidak terpenuhi lagi, sprite mundur sampai frames[0]. Perubahan arah di tengah animasi langsung
/// berbalik dari frame saat itu. Collider penghalang (opsional) hanya aktif saat pintu tertutup penuh.
/// Jarak dihitung dari posisi objek pemilik script ke posisi pemain. Hanya tampilan dan collision;
/// tidak mengubah resource, waktu, atau save. Nama class dipertahankan agar referensi scene tidak putus.
/// </summary>
public class DoorSensor : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer doorRenderer;
    [Tooltip("Urutan frame: primary_pintu1 (tertutup), primary_pintu2, primary_pintu3, primary_pintu4 (terbuka penuh)")]
    [SerializeField] private Sprite[] frames;
    [SerializeField, Min(0.01f)] private float frameDelay = 0.12f;

    [Header("Collision")]
    [Tooltip("Collider non-trigger yang menghalangi jalan saat pintu tertutup (opsional)")]
    [SerializeField] private Collider2D blockingCollider;

    [Header("Jarak buka")]
    [Tooltip("Pintu mulai terbuka bila pemain lebih dekat dari jarak ini (satuan unit dunia).")]
    [SerializeField, Min(0.1f)] private float openDistance = 2f;
    [Tooltip("Tambahan jarak sebelum pintu menutup lagi, agar tidak berkedip di tepi jarak.")]
    [SerializeField, Min(0f)] private float closeMargin = 0.3f;

    [Header("Referensi (otomatis dicari jika kosong)")]
    [SerializeField] private SuitSystem suit;
    [SerializeField] private PlayerMovement player;

    private int currentFrame;
    private int targetFrame;
    private Coroutine animRoutine;

    private bool HasFrames => doorRenderer != null && frames != null && frames.Length > 0;

    private void Awake()
    {
        if (!HasFrames)
            Debug.LogWarning("DoorSensor: isi Door Renderer dan Frames (primary_pintu1, 2, 3, 4) di Inspector.", this);
    }

    private void Start()
    {
        if (player == null) player = FindFirstObjectByType<PlayerMovement>();
        if (suit == null) suit = SuitSystem.Instance != null ? SuitSystem.Instance : FindFirstObjectByType<SuitSystem>();

        if (player == null || suit == null)
            Debug.LogWarning("DoorSensor: PlayerMovement atau SuitSystem tidak ditemukan; pintu tetap tertutup. Jalankan menu Mars > Setup Suit and Gate.", this);

        ApplyFrame(0);
    }

    private void OnDisable()
    {
        if (animRoutine != null)
        {
            StopCoroutine(animRoutine);
            animRoutine = null;
        }
        targetFrame = 0;
        if (HasFrames) ApplyFrame(0);
    }

    private void Update()
    {
        if (!HasFrames) return;

        SetTarget(ShouldBeOpen() ? frames.Length - 1 : 0);
    }

    private bool ShouldBeOpen()
    {
        if (player == null || suit == null) return false;

        // Aturan yang sama dengan SuitGate: boleh lewat bila suit siap keluar, atau pemain sudah di luar.
        bool allowed = suit.CanExit || suit.IsOutdoors;
        if (!allowed) return false;

        // Jarak tutup sedikit lebih besar dari jarak buka (histeresis) selama pintu sedang terbuka.
        float limit = targetFrame > 0 ? openDistance + closeMargin : openDistance;
        float distance = Vector2.Distance(player.transform.position, transform.position);
        return distance <= limit;
    }

    private void SetTarget(int index)
    {
        targetFrame = index;
        if (animRoutine == null && currentFrame != targetFrame)
            animRoutine = StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        while (currentFrame != targetFrame)
        {
            ApplyFrame(currentFrame + (currentFrame < targetFrame ? 1 : -1));
            yield return new WaitForSeconds(frameDelay);
        }
        animRoutine = null;
    }

    private void ApplyFrame(int index)
    {
        currentFrame = index;
        if (HasFrames) doorRenderer.sprite = frames[index];

        // Penghalang aktif hanya saat pintu tertutup penuh (frame 0)
        if (blockingCollider != null) blockingCollider.enabled = (index == 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.3f, 0.9f, 1f, 0.9f);
        Gizmos.DrawWireSphere(transform.position, openDistance);
    }
}

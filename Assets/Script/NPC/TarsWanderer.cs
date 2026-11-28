using System.Collections;
using UnityEngine;

/// <summary>
/// TARS berjalan acak di dalam area tertentu. Saat pemain berinteraksi, TARS berhenti,
/// menghadap pemain, lalu lanjut berjalan setelah jeda (atau saat Resume() dipanggil).
/// TARS hanya memberi informasi; bukan pemberi quest.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class TarsWanderer : MonoBehaviour, IInteractable
{
    private enum State { Idle, Moving, Interacting }

    private enum Facing { Down, Up, Left, Right }

    [Header("Wander Area")]
    [Tooltip("BoxCollider2D (Is Trigger) yang menandai area jelajah TARS.")]
    [SerializeField] private BoxCollider2D wanderArea;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float arriveDistance = 0.1f;
    [SerializeField] private float minIdleTime = 1f;
    [SerializeField] private float maxIdleTime = 3f;
    [SerializeField] private float stuckCheckInterval = 0.75f;

    [Header("Natural Wander")]
    [Tooltip("Jarak langkah minimum sekali jalan (unit). TARS berjalan lurus 4 arah, bukan diagonal.")]
    [SerializeField] private float minStepDistance = 0.8f;
    [Tooltip("Jarak langkah maksimum sekali jalan (unit).")]
    [SerializeField] private float maxStepDistance = 2.5f;
    [Tooltip("Jarak aman dari dinding/objek saat TARS berhenti (unit).")]
    [SerializeField] private float obstacleMargin = 0.1f;
    [Tooltip("Peluang TARS menoleh ke arah acak saat berhenti (0-1).")]
    [Range(0f, 1f)]
    [SerializeField] private float lookAroundChance = 0.4f;

    [Header("Interaction")]
    [SerializeField] private string interactionPrompt = "Bicara dengan TARS";
    [Tooltip("Dipakai hanya jika Dialogue UI kosong: lama TARS diam setelah diajak bicara (detik).")]
    [SerializeField] private float interactionPauseDuration = 3f;
    [Tooltip("Jeda singkat setelah dialog selesai agar tombol E yang sama tidak memulai dialog lagi.")]
    [SerializeField] private float reinteractCooldown = 0.3f;

    [Header("Dialogue")]    
    [SerializeField] private DialogueUI dialogueUI;
    [Tooltip("Opsional. Bila ada, dialog E ditambah laporan (pesan Bumi, fasilitas, kondisi, stok) dari TARSSystem. Dicari otomatis jika kosong.")]
    [SerializeField] private TARSSystem tarsSystem;
    [SerializeField] private string speakerName = "TARS";
    [TextArea(2, 4)]
    [SerializeField] private string[] dialogueLines =
    {
        "Selamat datang kembali. Saya TARS, asisten habitat ini.",
        "Saya memantau kondisi habitat dan akan memberi pengingat bila perlu."
    };

    [Header("Animation (sprite per arah)")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float framesPerSecond = 6f;
    [SerializeField] private Sprite[] downFrames;
    [SerializeField] private Sprite[] upFrames;
    [SerializeField] private Sprite[] leftFrames;
    [SerializeField] private Sprite[] rightFrames;

    private Rigidbody2D rb;
    private State state = State.Idle;
    private Facing facing = Facing.Down;
    private Vector2 targetPosition;
    private float idleTimer;
    private float animTimer;
    private Vector2 lastCheckPosition;
    private float stuckTimer;
    private Coroutine interactionRoutine;
    private float nextInteractTime;
    private Vector2 lastMoveDirection = Vector2.zero;
    private readonly RaycastHit2D[] castHits = new RaycastHit2D[8];
    private static readonly Vector2[] CardinalDirections =
        { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    public string InteractionPrompt => interactionPrompt;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (wanderArea == null)
        {
            Debug.LogError("TarsWanderer: wanderArea belum diisi di Inspector.", this);
            enabled = false;
        }
    }

    private void Start()
    {
        if (tarsSystem == null) tarsSystem = FindFirstObjectByType<TARSSystem>();
        IgnorePlayerCollision();
        StartIdle();
        ApplyFrame(0);
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                idleTimer -= Time.deltaTime;
                if (idleTimer <= 0f) StartMoving();
                ApplyFrame(0);
                break;

            case State.Moving:
                CheckStuck();
                AnimateWalk();
                break;

            case State.Interacting:
                ApplyFrame(0);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (state != State.Moving) return;

        Vector2 current = rb.position;
        Vector2 toTarget = targetPosition - current;

        if (toTarget.magnitude <= arriveDistance)
        {
            StartIdle();
            return;
        }

        UpdateFacing(toTarget);
        rb.MovePosition(Vector2.MoveTowards(current, targetPosition, moveSpeed * Time.fixedDeltaTime));
    }

    // Player dan TARS saling menembus: TARS tidak terdorong/terlempar oleh Player.
    // Interaksi (tombol E) tetap berfungsi karena memakai pengecekan area, bukan tabrakan.
    private void IgnorePlayerCollision()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player == null) return;

        Collider2D[] mine = GetComponentsInChildren<Collider2D>();
        Collider2D[] theirs = player.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D a in mine)
            foreach (Collider2D b in theirs)
                Physics2D.IgnoreCollision(a, b, true);
    }

    // ---------- Wander ----------

    private void StartIdle()
    {
        state = State.Idle;
        idleTimer = Random.Range(minIdleTime, maxIdleTime);

        // Kadang menoleh ke arah acak saat berhenti agar terlihat hidup.
        if (Random.value < lookAroundChance)
            facing = (Facing)Random.Range(0, 4);
    }

    // TARS berjalan lurus ke satu dari 4 arah dengan jarak pendek, lalu berhenti sejenak.
    // Arah yang menabrak dinding atau keluar area dilewati, jadi TARS tidak menabrak lalu macet.
    private void StartMoving()
    {
        Bounds b = wanderArea.bounds;
        Vector2 origin = rb.position;

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;

        int startIndex = Random.Range(0, CardinalDirections.Length);
        for (int i = 0; i < CardinalDirections.Length; i++)
        {
            Vector2 dir = CardinalDirections[(startIndex + i) % CardinalDirections.Length];

            // Hindari langsung berbalik arah kecuali tidak ada pilihan lain.
            if (i < CardinalDirections.Length - 1 && dir == -lastMoveDirection) continue;

            float distance = Random.Range(minStepDistance, maxStepDistance);
            distance = Mathf.Min(distance, DistanceToBounds(b, origin, dir));

            int hitCount = rb.Cast(dir, filter, castHits, distance + obstacleMargin);
            for (int h = 0; h < hitCount; h++)
                distance = Mathf.Min(distance, castHits[h].distance - obstacleMargin);

            if (distance < minStepDistance * 0.5f) continue;

            targetPosition = origin + dir * distance;
            lastMoveDirection = dir;
            UpdateFacing(dir);
            lastCheckPosition = origin;
            stuckTimer = 0f;
            animTimer = 0f;
            state = State.Moving;
            return;
        }

        // Semua arah terhalang: diam dulu, coba lagi nanti.
        StartIdle();
    }

    private static float DistanceToBounds(Bounds b, Vector2 origin, Vector2 dir)
    {
        if (dir.x > 0f) return b.max.x - origin.x;
        if (dir.x < 0f) return origin.x - b.min.x;
        if (dir.y > 0f) return b.max.y - origin.y;
        return origin.y - b.min.y;
    }

    // Jika TARS tertahan dinding/objek, berhenti sebentar lalu pilih tujuan baru.
    private void CheckStuck()
    {
        stuckTimer += Time.deltaTime;
        if (stuckTimer < stuckCheckInterval) return;

        float movedDistance = Vector2.Distance(rb.position, lastCheckPosition);
        float expectedDistance = moveSpeed * stuckCheckInterval;

        if (movedDistance < expectedDistance * 0.2f)
        {
            StartIdle();
            return;
        }

        lastCheckPosition = rb.position;
        stuckTimer = 0f;
    }

    // ---------- Interaction ----------

    /// <summary>Sprite hadap-bawah pertama, dipakai sebagai portrait TARS di kartu panduan tutorial (null bila belum diisi).</summary>
    public Sprite PortraitSprite => downFrames != null && downFrames.Length > 0 ? downFrames[0] : null;

    /// <summary>Dipanggil saat pemain mengajak TARS bicara (E) dan dialog dibuka. Dipakai TutorialManager.</summary>
    public event System.Action OnPlayerTalked;

    public bool CanInteract(GameObject interactor)
    {
        // Tidak boleh membuka dialog baru saat dialog lain (misalnya pengingat TARS) masih terbuka.
        if (dialogueUI != null && dialogueUI.IsOpen) return false;
        return state != State.Interacting && Time.time >= nextInteractTime;
    }

    public void Interact(GameObject interactor)
    {
        if (state == State.Interacting) return;

        state = State.Interacting;

        // Menghadap pemain
        Vector2 toInteractor = (Vector2)interactor.transform.position - rb.position;
        if (toInteractor != Vector2.zero) UpdateFacing(toInteractor);

        if (dialogueUI != null && dialogueLines != null && dialogueLines.Length > 0)
        {
            // TARS diam sampai dialog selesai, lalu Resume() dipanggil otomatis.
            string[] lines = tarsSystem != null ? tarsSystem.BuildConversationLines(dialogueLines) : dialogueLines;
            dialogueUI.Show(speakerName, lines, Resume);
            OnPlayerTalked?.Invoke();
            return;
        }

        // Cadangan jika Dialogue UI belum dipasang.
        Debug.LogWarning("TarsWanderer: Dialogue UI belum diisi, memakai jeda waktu saja.", this);
        if (interactionRoutine != null) StopCoroutine(interactionRoutine);
        interactionRoutine = StartCoroutine(ResumeAfterDelay());
    }

    /// <summary>Melanjutkan TARS berjalan (dipanggil otomatis, atau oleh sistem dialog nanti).</summary>
    public void Resume()
    {
        if (interactionRoutine != null)
        {
            StopCoroutine(interactionRoutine);
            interactionRoutine = null;
        }

        if (state == State.Interacting)
        {
            nextInteractTime = Time.time + reinteractCooldown;
            StartIdle();
        }
    }

    private IEnumerator ResumeAfterDelay()
    {
        yield return new WaitForSeconds(interactionPauseDuration);
        interactionRoutine = null;
        Resume();
    }

    // ---------- Animation ----------

    private void UpdateFacing(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            facing = direction.x > 0f ? Facing.Right : Facing.Left;
        else
            facing = direction.y > 0f ? Facing.Up : Facing.Down;
    }

    private void AnimateWalk()
    {
        Sprite[] frames = GetFrames(facing);
        if (frames == null || frames.Length == 0) return;

        animTimer += Time.deltaTime * framesPerSecond;
        ApplyFrame(Mathf.FloorToInt(animTimer) % frames.Length);
    }

    private void ApplyFrame(int index)
    {
        if (spriteRenderer == null) return;

        Sprite[] frames = GetFrames(facing);
        if (frames == null || frames.Length == 0) return;

        spriteRenderer.sprite = frames[Mathf.Clamp(index, 0, frames.Length - 1)];
    }

    private Sprite[] GetFrames(Facing dir)
    {
        switch (dir)
        {
            case Facing.Up: return upFrames;
            case Facing.Left: return leftFrames;
            case Facing.Right: return rightFrames;
            default: return downFrames;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (wanderArea == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(wanderArea.bounds.center, wanderArea.bounds.size);
    }
}

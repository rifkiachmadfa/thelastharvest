# Project Status

Terakhir diperbarui: 2026-11-27 (HP pemain, HUD kenyang kiri atas, hapus ResourceDebugPanel; basis `0435d7fa`)
Dasar audit: repo `rifkiachmadfa/martian`, branch `main`, commit `d96d9b6f` (audit lama; patch tutorial 2026-11-17 memakai `80bc5a6d`) (dicek ulang 2026-11-16; kode tidak berubah sejak `afc2453c`). Patch objek interaktif sudah ter-commit [Repo]. Developer melaporkan tes interaksi objek interaktif lolos semua [Dev]; scene lokal belum di-commit.

Label sumber: **[Repo]** terverifikasi dari file di repo, **[Dev]** dilaporkan developer (belum bisa dicek AI), **[?]** belum diverifikasi.

## Tahap saat ini
Fondasi pemain dan NPC. Setara sekitar hari produksi 1–2 dari jadwal 15 hari: movement, sistem interaksi modular, dan TARS yang berjalan acak serta bisa diajak bicara. Sudah ada ResourceManager (M1), TimeManager + HUD (M2), dan Bed/tidur/pergantian hari (M3, menunggu tes). Suit (Milestone A) dan objek interaktif Outdoor/Farming Area/Control Room (patch 2026-11-15, hanya script + menu setup) sudah dibuat tetapi belum dikompilasi dan dites di Unity. Belum ada minigame kabel, TARSSystem, Victory/Game Over, atau save.

## Status fitur
| Fitur | Status | Bukti / catatan |
|---|---|---|
| Movement & collision | Ada | [Repo] `Assets/Script/PlayerMovement.cs` (Rigidbody2D.MovePosition, `Input.GetAxisRaw`). Tes collision: [?] |
| Camera follow | Ada | [Repo] `CinemachineCamera` di `SampleScene` |
| Directional animation player | Bermasalah menurut catatan lama | [Repo] Animator `Square.controller` hanya punya state `Down` dan `up`; belum ada kiri/kanan. Re-test: [?] |
| Sistem interaksi (`IInteractable`, `InteractionDetector`, prompt UI) | Ada | [Repo] kode di `Assets/Script/Interaction/`. Berfungsi di Unity: [Dev] |
| TARS berjalan acak di area | Ada | [Repo] `Assets/Script/NPC/TarsWanderer.cs`. Berfungsi di Unity: [Dev] |
| TARS berhenti saat interaksi + dialog | Ada | [Repo] `Assets/Script/Dialogue/DialogueUI.cs`. Berfungsi di Unity: [Dev] |
| Menu setup UI (Mars > Setup Dialogue and Prompt UI) | Ada | [Repo] `Assets/Editor/MarsUISetup.cs`. Sudah dijalankan: [Dev] |
| Sprite animasi TARS | Sebagian | [Repo] folder `Assets/Robot` punya Left, Right, Up (3 frame tiap arah). Folder **Down tidak ada** |
| Resource systems | Ada | [Repo] `Assets/Script/Resource/`. Playtest panel uji: [Dev] berjalan baik. Komponen di scene lokal dan belum ter-push ke GitHub saat audit 2026-11-15: [Dev] |
| Timer harian (TimeManager) | Ada | [Repo] `Assets/Script/Time/TimeManager.cs`. Playtest: [Dev] "sudah bagus" (butir tidak dirinci). Sejak 2026-11-15 timer tetap berjalan saat dialog (flag `pauseDuringDialogue`, default mati); `ExternalPause` untuk transisi |
| HUD (Day, timer, 4 resource) | Ada | [Repo] `Assets/Script/UI/HudUI.cs`, `Assets/Editor/MarsHudSetup.cs`. Playtest: [Dev] "sudah bagus" |
| Skin HUD (Pixel UI Sci-Fi Free) | Script + aset dibuat 2026-11-18 (belum dites) | [Repo setelah patch] `Assets/UI/PixelUISciFi/*`, `Assets/Script/UI/HudBar.cs`, `Assets/Editor/MarsHudSkinSetup.cs`, field bar opsional di `HudUI`. Belum dikompilasi/dites di Unity [?]. Scene diubah developer lewat menu `Mars > Apply HUD Skin (Pixel UI Sci-Fi)` [?]. Kredit CC BY 4.0 wajib di layar credits |
| Bed, tidur, fade, pergantian hari | Script dibuat (Milestone 3) | [Repo] `Assets/Script/Interaction/Bed.cs`, `Assets/Script/UI/FadeUI.cs`, `Assets/Editor/MarsSleepSetup.cs`. Belum dites di Unity |
| Ending Victory / Game Over | Belum ada | [Repo] tidak ada script. Bed hari terakhir hanya menampilkan pesan placeholder |
| Suit, Suit Station, gerbang keluar, oksigen suit (berkurang saat dikenakan) | Script dibuat (Milestone A + revisi: tombol Suit Station hanya kenakan/lepas dengan isi tabung otomatis 2026-11-16, lepas otomatis saat tidur, pengingat TARS `SuitReminder`) | [Repo setelah patch] `Assets/Script/Suit/*`, `Assets/Editor/MarsSuitSetup.cs`, `HudUI` (panel suit). Belum dikompilasi/dites di Unity [?]. Objek scene (SuitStation, IndoorZone, SuitGate) dibuat lewat menu dan diposisikan developer [?] |
| Hunger dan makan (kentang dari tas) | Script dibuat 2026-11-16 (belum dites) | [Repo setelah patch] `Assets/Script/Survival/{HungerSystem,HungerReminder}.cs`, `Assets/Editor/MarsHungerSetup.cs`, `HudUI.hungerText`, baris Kenyang di `ResourceMonitor` (sudah dihapus 2026-11-16). Belum dikompilasi/dites di Unity [?]. Game Over kelaparan belum ada (hanya event) |
| Tas pemain, hotbar, storage, bibit | Script dibuat 2026-11-16 (belum dites) | [Repo setelah patch] `Assets/Script/Inventory/*`, `Assets/Script/UI/{InventoryHotbarUI,StorageUI}.cs`, `Assets/Script/Station/FoodStorage.cs`, `IItemReceiver`, perubahan di `InteractionDetector`, `FarmingPlot`, `IceDeposit`, `WaterPurifier`, `Assets/Editor/MarsInventorySetup.cs`. Belum dikompilasi/dites di Unity [?]. Objek scene dibuat lewat menu `Mars > Setup Inventory and Storage`; Food di Inspector ResourceManager perlu diubah manual [?] |
| Countdown darurat / Game Over oksigen suit | Belum ada | `SuitSystem.OnSuitOxygenEmpty` hanya event + log |
| Ice Deposit (kumpul es, inventory Ice) | Script dibuat; tes interaksi dilaporkan lolos | [Repo setelah patch] `Assets/Script/Outdoor/IceDeposit.cs`; HUD menampilkan Ice lewat `HudUI.iceText`. Tes interaksi: [Dev] lolos 2026-11-16 (butir tidak dirinci; scene lokal belum di-commit) |
| Water Purifier | Script dibuat; tes interaksi dilaporkan lolos | [Repo setelah patch] `Assets/Script/Machines/WaterPurifier.cs`. Tes interaksi: [Dev] lolos 2026-11-16 (butir tidak dirinci; scene lokal belum di-commit) |
| Oxygen Generator | Script dibuat; tes interaksi dilaporkan lolos | [Repo setelah patch] `Assets/Script/Machines/OxygenGenerator.cs` (saklar E = usulan di luar GDD). Tes interaksi: [Dev] lolos 2026-11-16 (butir tidak dirinci; scene lokal belum di-commit) |
| Solar Panel (produksi Energy, status rusak) | Script dibuat; tes interaksi dilaporkan lolos; pemicu kerusakan acak 1x per hari dibuat 2026-11-16 (belum dites) | [Repo setelah patch] `Assets/Script/Outdoor/SolarPanel.cs`. Pemicu otomatis: `Assets/Script/Outdoor/SolarPanelDamageScheduler.cs` [Repo setelah patch, belum dites di Unity]. Tes interaksi: [Dev] lolos 2026-11-16 (butir tidak dirinci; scene lokal belum di-commit) |
| Repair Point | Script dibuat; kini membuka minigame kabel (patch 2026-11-16, belum dites); tes lama (perbaikan langsung) dilaporkan lolos | [Repo setelah patch] `Assets/Script/Outdoor/RepairPoint.cs`. Tes interaksi: [Dev] lolos 2026-11-16 (butir tidak dirinci; scene lokal belum di-commit) |
| Wiring minigame | Script dibuat (belum dites) | [Repo setelah patch] `Assets/Script/UI/WiringMinigameUI.cs`, `Assets/Editor/MarsRepairSetup.cs`. Belum dikompilasi/dites di Unity [?] |
| Resource Monitor | DIHAPUS 2026-11-16 (permintaan developer); HUD dan Storage menggantikan, fungsi laporan fasilitas pindah ke TARS (`PHASE.md` 11.4) | [Dev] |
| Terminal komunikasi | DIHAPUS 2026-11-16 (permintaan developer); pesan Bumi pindah ke TARS (`PHASE.md` 11.4.1) | [Dev] |
| Farming / Potato | Script dibuat; tes interaksi dilaporkan lolos (satu petak per objek, tanpa sprite) | [Repo setelah patch] `Assets/Script/Farming/FarmingPlot.cs`: tanam, siram, tumbuh saat hari berganti, panen. Tes interaksi: [Dev] lolos 2026-11-16 (butir tidak dirinci; scene lokal belum di-commit) |
| TARSSystem (antrean, ambang resource, panel rusak, akhir hari, pesan Bumi, laporan dialog E) | Script dibuat 2026-11-17 (belum dites) | [Repo setelah patch] `Assets/Script/TARS/TARSSystem.cs`, `Assets/Editor/MarsTarsSystemSetup.cs`; `TarsWanderer`, `SuitReminder`, `HungerReminder`, `SolarPanelDamageScheduler` disambungkan. Belum dikompilasi/dites di Unity [?]. Objek `TARSSystem` di scene dibuat lewat menu `Mars > Setup TARS System` [?]. Teks `earthMessages` bawaan hanyalah placeholder |
| Tutorial Day 1 | Script dibuat 2026-11-17 (belum dites) | [Repo setelah patch] `Assets/Script/Tutorial/{TutorialManager,TutorialStepId,TutorialStepData,TutorialGuideUI}.cs`, `Assets/Editor/MarsTutorialSetup.cs`, event `TarsWanderer.OnPlayerTalked`. 8 langkah dengan syarat USULAN (dikunci, urutan bebas; langkah RemoveSuit ditambah 2026-11-17), arahan TARS PLACEHOLDER, status selesai di `PlayerPrefs`, lewati dengan Tab, garis arah UI ke tujuan tiap langkah. Belum dikompilasi/dites di Unity [?] |
| Popup mengambang ikon + teks (Es +1, Kentang +2, Water -2, Oxygen +10, makan) | Script dibuat 2026-11-17 (belum dites) | [Repo setelah patch] `Assets/Script/UI/FloatingPopupManager.cs`, `Assets/Editor/MarsPopupSetup.cs`; hook di `IceDeposit`, `FarmingPlot`, `WaterPurifier`, `OxygenGenerator`, `RepairPoint`, `InventorySystem`. Objek `FloatingPopupManager` di scene dibuat lewat menu `Mars > Setup Floating Popups` [?]. Ikon bawaan placeholder; sprite final belum diimpor |
| HP pemain (berkurang tiap 2 dtk saat kenyang 0) | Script dibuat 2026-11-27 (belum dites) | [Repo setelah patch] `Assets/Script/Survival/HealthSystem.cs`, `Assets/Editor/MarsHealthSetup.cs`, `HudUI.hpText/hpBar`. Belum dikompilasi/dites di Unity [?]. Panel HP dibuat lewat menu `Mars > Setup Health and HP HUD` [?]. Angka (100 HP, 3 HP/2 dtk, pulih +2 HP/2 dtk bila kenyang > 80 persen) USULAN. Game Over belum ada (hanya event `OnDied`) |
| Pintu otomatis (terbuka berurutan saat pemain dekat dan bersuit, aturan seperti SuitGate) | Script + menu setup dibuat 2026-11-28 (belum dites). Temuan 2026-11-28: `SampleScene` di repo belum berisi objek pintu (GUID `DoorSensor` dan sprite pintu tidak direferensikan scene/prefab mana pun) | [Repo] `Assets/Script/Door/DoorSensor.cs`, `Assets/Editor/MarsDoorSetup.cs`. Objek pintu di scene lokal developer [Dev, belum di-commit]. Belum dikompilasi/dites di Unity [?] |
| Save system | Belum ada | [Repo] |
| Tiga scene area (Control Room, Farming, Outdoor) | Belum ada | [Repo] hanya `SampleScene` |

## Kondisi scene (penting)
- **[Repo]** `SampleScene.unity` di GitHub hanya berisi: Main Camera, CinemachineCamera, Global Light 2D, Player, Grid (Ground, Wall, PasirMars, Tangga). Tidak ada referensi ke `TarsWanderer`, `DialogueUI`, `InteractionDetector`, atau `InteractionPromptUI`.
- **[Dev]** Scene lokal developer berisi tambahan: `TARS`, `TarsArea`, `Canvas` (DialogueRoot, PromptRoot), `EventSystem`, dan `InteractionDetector` di Player. Belum di-commit.
- **[Dev]** Layer `Interactable` dipakai oleh TARS. Layer ini belum ada di `TagManager.asset` di repo.

## Belum diverifikasi / risiko
- (2026-11-28) Kartu panduan tutorial (`tutorial-card-ui.patch`): belum dikompilasi/dites di Unity. Risiko: tata letak (VerticalLayoutGroup + ContentSizeFitter dibuat lewat kode) belum dilihat; posisi tengah atas bisa menimpa HUD di layar sangat sempit.
- (2026-11-28) Tutorial bercerita (`tutorial-story.patch`): belum dikompilasi/dites di Unity. Dugaan penyebab tutorial membingungkan (belum dikonfirmasi): teks hanya instruksi tanpa alasan, teks PLACEHOLDER di scene, pemain belum lapar saat langkah Makan. Balancing: Food awal 50 cukup sekitar 13 hari (P-8).
- Isi Inspector TARS dan Player di scene lokal (belum di-commit).
- Milestone 3: kompilasi dan logika hanya dicek dengan stub di luar Unity [?]. Menu `Mars > Setup Bed and Fade`, fade, perpindahan Player ke SpawnPoint, dan interaksi E dengan Bed belum dites di Unity [?]. Collider dan posisi Bed/SpawnPoint ditentukan developer di scene [?].
- GitHub belum memuat Milestone 1 (komponen ResourceManager di scene dan file .meta) saat audit; basis patch 2 adalah `20c1fc28` + patch 1 [?].
- Tidak ada compile atau run Unity oleh AI; semua kode hanya diperiksa lewat `git apply --check`.
- Gambar arah bawah untuk TARS (`Down Frames`) belum ada.
- Collider Player di scene belum diperiksa.
- Player masih bisa bergerak saat dialog terbuka (kini disengaja: waktu dan resource tetap berjalan saat dialog).
- **[Repo, d96d9b6f]** `SampleScene.unity` yang ter-commit belum berisi objek interaktif baru (tidak ada GUID script objek interaktif di scene). **[Dev]** setup di scene lokal sudah dilakukan dan tes interaksi lolos; belum di-commit. Layer `Interactable` ada di `TagManager.asset` [Repo].
- Objek interaktif (patch 2026-11-15): tidak ada compiler C# atau Unity di lingkungan AI; kode hanya diperiksa dengan membaca dan `git apply --check` [?]. Posisi, sprite, dan collider objek di scene ditentukan developer [?].
- Keseimbangan (DIREVISI 2026-11-17): sebelumnya `OxygenGenerator` praktis menyala terus (mengisi tiap oksigen turun sedikit dari penuh) dan memakai sekitar 1 Water per 5 detik [Repo, dibaca dari kode]. Patch balancing 2026-11-17 menambah histeresis (mulai di <= 40 persen, isi sampai penuh), `oxygenPerCycle` 10, dan `SuitSystem.drainPerSecond` 0,2 (script dan `SampleScene.unity`). Belum dikompilasi atau dites di Unity [?]; angka perlu playtest, terutama kebutuhan Water per hari terhadap jumlah petak. Energy tidak dihasilkan kecuali ada `SolarPanel` di scene [?].
- Satu `FarmingPlot` = satu petak; petak tambahan dibuat dengan menduplikasi objek. Pertumbuhan sejak 2026-11-17 waktu nyata (`FarmingPlot.Update`, durasi acak setelah siram), bukan lewat `OnDayStarted`.

- Bug panel rusak (2026-11-27): [Repo] scene punya 9 SolarPanel dan 1 RepairPoint; scheduler kini hanya memilih panel yang dilayani RepairPoint. KOREKSI: desainnya satu RepairPoint melayani semua panel; `RepairPoint` kini menangani panel rusak pertama (`servesAllPanels`). Belum diverifikasi: menu `Setup Solar Panel Repair Points` di Unity (ukuran/posisi collider anak), dan penyebab notifikasi TARS tidak tampil (hipotesis: pesan Warning kedaluwarsa 90 dtk di antrean; belum dibuktikan) [?]. `SolarPanelDamageScheduler.firstDamageDay` di scene = 1 (bukan 2), jadi Day 1 tutorial bisa kena kerusakan [Repo].
- HP dan HUD (2026-11-27): kode ditulis tanpa Unity [?]. Belum diverifikasi: kompilasi di 6000.6.8f1, menu `Setup Health and HP HUD` (gandaan HungerPanel, referensi `HudBar.fill` pada gandaan), tata letak kiri atas tidak menabrak TimePanel (tengah atas, lebar 360), tidak ada komponen/referensi hilang setelah ResourceDebugPanel dihapus dari scene.

- Milestone 10 (2026-11-16): kode ditulis tanpa compiler/Unity [?]. Belum diverifikasi: kompilasi, menu `Setup Panel Damage and Wiring Minigame`, tampilan dan hit-test drag-and-drop pada Canvas (Scale With Screen Size), jeda timer/oksigen suit selama minigame, durasi nyata 15-30 detik, urutan dialog TARS saat panel rusak. Frekuensi kerusakan (1 per hari mulai Day 2) mungkin tinggi untuk balancing.

- Milestone 11 (2026-11-17): kode ditulis tanpa compiler/Unity [?]. Belum diverifikasi: kompilasi, menu `Setup TARS System`, urutan dan cooldown pesan di Play Mode (terutama saat banyak resource rendah bersamaan), pesan awal hari setelah fade tidur, kenyamanan dialog TARS spontan saat pemain sibuk, nilai ambang 30/10 persen dan Energy/Water awal (Water awal 10/30 sudah dekat batas menipis), dan teks laporan dialog E.

- Tutorial Day 1 (2026-11-17): kode ditulis tanpa Unity; sintaks hanya dicek dengan `mcs` + stub [?]. Belum diverifikasi: kompilasi di Unity 6000.6.8f1, menu `Setup Tutorial`, tiap syarat langkah di Play Mode (terutama CollectIce: butuh `IsOutdoors` benar; PurifyWater; OxygenGenerator lewat `InteractionDetector.CurrentTarget`), arahan muncul lewat `TARSSystem` (Warning dapat kedaluwarsa 90 dtk; diulang tiap 120 dtk), simpan/lewati lewat PlayerPrefs dan Tab, Day 2 menyelesaikan tutorial. Di Day 1 `TARSSystem` tetap mengumumkan pesan Bumi/resource/hunger sehingga dapat menumpuk dengan arahan [?].
- Garis arah tutorial (2026-11-17): belum diverifikasi: kompilasi, posisi garis tepat di Player dan tujuan pada Canvas Scale With Screen Size (terutama Overlay vs Screen Space Camera), keterlambatan satu frame terhadap Cinemachine, tampilan di bawah HUD/dialog/fade [?].
- Perbaikan CollectIce (2026-11-17): belum diverifikasi di Unity bahwa penyebab bug adalah syarat lama `IsOutdoors`; syarat baru memakai `IceDeposit.OnCollected` [?].

- Label hitung mundur panen (2026-11-17): belum diverifikasi: posisi label terhadap sprite petak, keterbacaan (ukuran `statusLabelSize` 0,8), tumpang tindih antar petak berdekatan [?].
- Popup mengambang (2026-11-17): kode ditulis tanpa Unity [?]. Belum diverifikasi: kompilasi di Unity 6000.6.8f1, font bawaan `LegacyRuntime.ttf` pada `TextMesh`, ukuran teks/ikon terhadap kamera Cinemachine (`characterSize` 0,045 dan `iconWorldSize` 0,45 hanya usulan), tata letak ikon-teks (lebar dihitung dari `MeshRenderer.bounds`), urutan gambar terhadap Tilemap (layer `Default`, order 1000), penumpukan popup bersamaan, popup tetap tampil saat timer berhenti.

## Housekeeping
- `phase-sync.patch` ter-commit di root repo (`interactive-objects.patch` sudah dihapus di `b09da2e3`). File `.patch` tidak perlu ikut repo; hapus dan tambahkan `*.patch` ke `.gitignore`.
- `Assets/_Recovery/0.unity` masih ter-commit [Repo].
- Ada `Assets/popopop.meta` tanpa folder/aset pasangannya.
- `.gitignore` belum mengecualikan `.vscode/`.

## Aturan update
Cantumkan tanggal, bukti file/scene, hasil tes, dan hal yang belum diverifikasi. Jangan tandai Selesai hanya karena script ada. Pembaruan wajib ikut di setiap patch (lihat `04_CODE_RULES.md`).

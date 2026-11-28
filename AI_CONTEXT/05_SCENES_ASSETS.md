# Scenes & Assets

Terakhir diperbarui: 2026-11-17 (basis `80bc5a6d` + patch tutorial Day 1). Label: [Repo] terverifikasi, [Dev] dilaporkan developer.

## Area desain yang direncanakan
- Control Room
- Farming Area
- Outdoor

## Scene yang ada
| Scene | Isi | Sumber |
|---|---|---|
| `Assets/Scenes/SampleScene.unity` (satu-satunya di Build Settings) | Main Camera, CinemachineCamera, Global Light 2D, Player, Grid (Ground, Wall, PasirMars, Tangga) | [Repo] |
| `SampleScene` versi lokal | Tambahan: TARS, TarsArea, Canvas (DialogueRoot, PromptRoot), EventSystem; Player memakai InteractionDetector | [Dev], belum di-commit |
| Scene area Control Room / Farming / Outdoor | Belum dibuat | [Repo] |

Scene lain di repo (`URP2DSceneTemplate`, demo asset pack `COPY SPRIGHT`) bukan bagian game.

## Script
| Path | Fungsi |
|---|---|
| `Assets/Script/PlayerMovement.cs` | Gerak 4 arah, arah terakhir, parameter Animator `IsMoving`, `DirectionX`, `DirectionY`; `MovementEnabled` untuk mengunci gerak |
| `Assets/Script/Interaction/IInteractable.cs` | Kontrak: `InteractionPrompt`, `CanInteract`, `Interact` |
| `Assets/Script/Interaction/InteractionDetector.cs` | Di Player. Cari objek interaktif terdekat (OverlapCircle + layer), tombol E |
| `Assets/Script/Interaction/InteractionPromptUI.cs` | Menampilkan "[E] ..." (UI Text legacy) |
| `Assets/Script/Dialogue/DialogueUI.cs` | Kotak dialog; E/Spasi/klik di mana saja untuk lanjut, tombol Tutup (opsional) untuk menutup langsung; callback saat selesai |
| `Assets/Script/NPC/TarsWanderer.cs` | TARS acak di `BoxCollider2D` area; state Idle/Moving/Interacting; sprite per arah lewat array Inspector; event `OnPlayerTalked` saat dialog E dibuka (dipakai `TutorialManager`) |
| `Assets/Script/Resource/ResourceType.cs` | Enum jenis resource (Ice, Water, HabitatOxygen, SuitOxygen, Energy, Food) |
| `Assets/Script/Resource/ResourceManager.cs` | Stok, kapasitas, `Add`/`TryConsume`, event perubahan. Dipasang pada GameObject `ResourceManager` di scene |
| `Assets/Script/Time/TimeManager.cs` | Hari, timer harian, timer harian (pause saat dialog opsional lewat `pauseDuringDialogue`, default mati), event hari. Dipasang pada GameObject `TimeManager` |
| `Assets/Script/UI/HudUI.cs` | Tampilan Day, timer, Food, Water, Oxygen, Energy, Ice (opsional, `iceText`). Dipasang pada `HudRoot` di Canvas |
| `Assets/Script/Interaction/Bed.cs` | Tempat tidur (IInteractable): tidur, fade, hari berikutnya. Dipasang pada GameObject `Bed` (layer `Interactable`, BoxCollider2D, anak `SpawnPoint`) |
| `Assets/Script/UI/FadeUI.cs` | Layar hitam untuk fade. Dipasang pada `Canvas/FadeOverlay` |
| `Assets/Editor/MarsUISetup.cs` | Menu Editor: membuat/memperbaiki Canvas, dialog, prompt, dan menyambungkan ke TARS/Player. Helper `internal` dipakai `MarsHudSetup` |
| `Assets/Editor/MarsHudSetup.cs` | Menu Editor `Mars > Setup HUD and Time`: membuat TimeManager, HudRoot (TimePanel, ResourcePanel), dan menyambungkan referensi |
| `Assets/Script/UI/HudBar.cs` | Bar isi HUD (hanya tampilan): `SetFraction(0..1)` mengatur anchor isi. Dipasang pada objek `<Key>Bar` di panel HUD |
| `Assets/Editor/MarsHudSkinSetup.cs` | Menu Editor `Mars > Apply HUD Skin (Pixel UI Sci-Fi)`: mengatur impor sprite, memberi skin panel, membuat ikon + bar per baris, menyambungkan field bar di `HudUI` |
| `Assets/Script/Suit/SuitSystem.cs` | Status suit, isi ulang dari oksigen habitat, lepas suit, konsumsi oksigen suit selama dikenakan. Dipasang pada GameObject `SuitSystem` |
| `Assets/Script/Suit/SuitStation.cs` | Suit Station (IInteractable): "Kenakan suit" atau "Lepas suit"; tabung diisi otomatis dari oksigen habitat saat dikenakan dan saat dilepas. Layer `Interactable`, Collider2D Is Trigger |
| `Assets/Script/Suit/IndoorZone.cs` | Trigger area dalam habitat; di luar semua zona = pemain di luar |
| `Assets/Script/Suit/SuitReminder.cs` | Pengingat TARS "lepas suit" (dialog, cooldown). Dipasang pada GameObject `SuitSystem` oleh menu setup. Menonaktifkan diri bila `TARSSystem` ada (logikanya pindah ke sana) |
| `Assets/Script/TARS/TARSSystem.cs` | Antrean pesan TARS (prioritas, cooldown), pemantau ambang resource/oksigen suit, panel rusak, awal/akhir hari, pesan Bumi, laporan dialog E. Objek `TARSSystem` di scene dibuat menu setup |
| `Assets/Script/Tutorial/TutorialManager.cs` | Tutorial Day 1: 8 langkah (termasuk RemoveSuit, 2026-11-17), syarat dikunci (urutan bebas), arahan TARS via `TARSSystem.Announce`, event, `TryCompleteStep`, simpan/lewati (PlayerPrefs, Tab), selesai saat Day 2. Objek `TutorialManager` di scene dibuat menu setup |
| `Assets/Script/Tutorial/TutorialStepId.cs` | Enum 8 langkah tutorial (GDD 5.2 + RemoveSuit); urutan langkah = array `steps`, bukan nilai enum |
| `Assets/Script/Tutorial/TutorialStepData.cs` | Data satu langkah (id, judul, baris TARS) |
| `Assets/Script/Dialogue/TutorialDialogueSkin.cs` | Kartu panduan tutorial (UI dibuat lewat kode; hanya tampilan; dikelola `DialogueUI`). `.meta` dibuat Unity saat impor |
| `Assets/Script/Tutorial/TutorialGuideUI.cs` | Garis UI dari tubuh Player ke tujuan langkah tutorial (hanya tampilan). Dipasang pada Canvas oleh menu setup |
| `Assets/Editor/MarsTutorialSetup.cs` | Menu Editor `Mars > Setup Tutorial`: membuat objek `TutorialManager` dan memasang `TutorialGuideUI` pada Canvas |
| `Assets/Editor/MarsTarsSystemSetup.cs` | Menu Editor `Mars > Setup TARS System`: membuat objek `TARSSystem`, menyambungkan `DialogueUI` dan `TarsWanderer.tarsSystem` |
| `Assets/Script/Suit/SuitGate.cs` | Tembok tak terlihat di pintu keluar (BoxCollider2D solid) |
| `Assets/Editor/MarsSuitSetup.cs` | Menu Editor `Mars > Setup Suit and Gate`: membuat SuitSystem, SuitStation, IndoorZone, SuitGate, dan `HudRoot/SuitPanel` |
| `Assets/Editor/MarsSleepSetup.cs` | Menu Editor `Mars > Setup Bed and Fade`: membuat FadeOverlay dan Bed (+SpawnPoint), menyambungkan referensi tanpa menimpa isian manual |
| `Assets/Script/Outdoor/IceDeposit.cs` | Deposit es (IInteractable): E, pemain berhenti beberapa detik, Ice +1 ke inventory (batas = kapasitas Ice). Perlu suit. Layer `Interactable`, Collider2D |
| `Assets/Script/Outdoor/SolarPanel.cs` | Panel surya: Energy otomatis per tick; `Damage()`/`Repair()`, `IsDamaged`, event `OnDamagedChanged`. Bukan IInteractable. Beberapa panel menjumlah |
| `Assets/Script/Outdoor/RepairPoint.cs` | Titik perbaikan (IInteractable) untuk satu `SolarPanel` (field `panel`). Membuka `WiringMinigameUI`; panel diperbaiki setelah minigame sukses (cadangan langsung bila minigame tidak ada) |
| `Assets/Script/Outdoor/SolarPanelDamageScheduler.cs` | Satu kerusakan acak per hari pada satu panel normal; pengumuman lewat `TARSSystem` (dialog langsung hanya bila `TARSSystem` tidak ada). Objek `SolarPanelDamageScheduler` di scene (dibuat menu setup) |
| `Assets/Script/UI/WiringMinigameUI.cs` | Minigame kabel (3-4 kabel, drag-and-drop mouse). Dipasang pada Canvas; UI dibuat lewat kode saat Awake (anak `WiringMinigameRoot`) |
| `Assets/Editor/MarsRepairSetup.cs` | Menu Editor `Mars > Setup Panel Damage and Wiring Minigame`: pasang `WiringMinigameUI` di Canvas, buat `SolarPanelDamageScheduler`, sambungkan `RepairPoint` |
| `Assets/Script/Survival/HungerSystem.cs` | Kenyang pemain (0-100), berkurang selama gameplay, `Feed()` dipanggil InventorySystem saat makan kentang, event `OnSatietyChanged`, `OnStarvationFailure`. Dipasang pada GameObject `HungerSystem` |
| `Assets/Script/Survival/HealthSystem.cs` | HP pemain (0-100). Berkurang tiap `damageInterval` (2 dtk) sebesar `damageAmount` (3) selama kenyang 0; ikut `TimeManager.IsPaused`; event `OnHealthChanged`, `OnDied` (hanya log). Dipasang pada GameObject `HealthSystem` lewat menu (2026-11-27, belum dites) |
| `Assets/Script/Survival/HungerReminder.cs` | Pesan TARS (teks tetap) saat kenyang melewati 30%, 10%, 0; lewat antrean `TARSSystem` bila ada, dialog langsung bila tidak. Dipasang pada GameObject `HungerSystem` oleh menu setup |
| `Assets/Script/Door/DoorSensor.cs` | Pintu otomatis (2026-11-28, belum dites): terbuka bila pemain dalam `openDistance` dan `SuitSystem.CanExit` atau `IsOutdoors` (aturan sama dengan `SuitGate`); menutup lagi dengan histeresis `closeMargin`. Sprite `frames` berganti berurutan (primary_pintu1 tertutup, 2, 3, 4 terbuka penuh); `blockingCollider` opsional aktif hanya saat tertutup. Tanpa trigger/Tag; jarak dari pusat sprite `doorRenderer` (atau posisi objek bila kosong) ke `PlayerMovement`; menu klik kanan `Cek Status Pintu` mencatat sebab pintu belum terbuka. Susunan scene dibuat developer [Dev, belum di-commit]. Sprite `primary_pintu1-4` ada di repo [Repo] |
| `Assets/Editor/MarsDoorSetup.cs` | Menu Editor `Mars > Setup Door` (2026-11-28, belum dites): membuat objek `Door` (SpriteRenderer + `DoorSensor`) di posisi `SuitGate`, mengisi `Frames` dari `Assets/object/primary_pintu1..4`, menyambungkan `doorRenderer`; tidak menimpa objek/isian yang sudah ada |
| `Assets/Script/Station/FoodStorage.cs` | Storage di Control Room (IInteractable "Buka storage"): membuka `StorageUI`. Nama class lama dipertahankan. Layer `Interactable`, Collider2D Is Trigger |
| `Assets/Script/Inventory/ItemType.cs` | Enum item tas: Potato (1), Ice (2), Seed (3) |
| `Assets/Script/Inventory/InventorySystem.cs` | Tas pemain (kentang 10, es 10, bibit 5), storage es dan bibit, pilihan slot (tombol 1-3), aksi bawaan (makan). Dipasang pada GameObject `InventorySystem` |
| `Assets/Script/Interaction/IItemReceiver.cs` | Antarmuka opsional untuk objek yang menerima item tas (FarmingPlot, WaterPurifier) |
| `Assets/Script/UI/InventoryHotbarUI.cs` | Hotbar 3 slot di bawah tengah layar (dibuat lewat kode di bawah Canvas) |
| `Assets/Script/UI/StorageUI.cs` | Panel storage (dibuat lewat kode di bawah Canvas): Simpan/Ambil dengan mouse, Esc menutup |
| `Assets/Editor/MarsInventorySetup.cs` | Menu Editor `Mars > Setup Inventory and Storage`: InventorySystem, hotbar, StorageUI, objek Storage, menyembunyikan IcePanel lama |
| `Assets/Editor/MarsHungerSetup.cs` | Menu Editor `Mars > Setup Hunger and Food Storage`: membuat HungerSystem, HungerReminder, FoodStorage, dan `HudRoot/HungerPanel` (teks Kenyang, kiri atas sejak 2026-11-27) |
| `Assets/Editor/MarsSolarRepairSetup.cs` | Menu Editor `Mars > Setup Solar Panel Names`: ganti nama "SolarPanel" menjadi "Solar Panel N"; hapus RepairPoint anak panel buatan menu lama. Satu RepairPoint melayani semua panel |
| `Assets/Editor/MarsHealthSetup.cs` | Menu Editor `Mars > Setup Health and HP HUD`: membuat `HealthSystem` dan `HudRoot/HpPanel` (gandaan HungerPanel, di bawahnya, kiri atas), mengisi `HudUI.hpText`/`hpBar` |
| `Assets/Script/Machines/WaterPurifier.cs` | Water Purifier (IInteractable): Ice + Energy -> Water, proses beberapa detik, `IsProcessing`, `OnProcessingChanged`, indikator opsional |
| `Assets/Script/Machines/OxygenGenerator.cs` | Oxygen Generator (IInteractable): Water + Energy -> HabitatOxygen otomatis per siklus (10 Oxygen per 1 Water sejak 2026-11-17); siaga sampai oksigen habitat <= `restartBelowFraction` (0,4) lalu mengisi sampai penuh; E = nyalakan/matikan (USULAN); `IsOn`, `IsRunning`, `StatusText`. Nilai serialized di `SampleScene.unity` (`oxygenPerCycle`, `SuitSystem.drainPerSecond`) ikut diubah 2026-11-17 |
| `Assets/Script/Farming/FarmingPlot.cs` | Satu petak kentang (IInteractable): tanam, siram (Water), tumbuh waktu nyata setelah satu kali siram, panen acak (kentang dan bibit). Satu objek = satu petak |
| `Assets/Editor/MarsObjectsSetup.cs` | Menu Editor `Mars > Setup Interactive Objects`: membuat 7 objek interaktif placeholder + `SolarPanel` + panel Ice di HUD |
| `Assets/Welcome/Welcome2DScript.cs` | Bawaan template Unity; bukan bagian game |
| `Assets/Script/UI/FloatingPopupManager.cs` | Popup mengambang ikon + teks di dunia game (SpriteRenderer + TextMesh, pool, ikon pixel 8x8 bawaan lewat kode); API statis `ShowAmount`, `Show`, `AnchorAbove`, `CreateWorldLabel` (label tetap untuk `FarmingPlot`); enum `PopupIcon` di file yang sama. Satu instance di scene |
| `Assets/Editor/MarsPopupSetup.cs` | Menu Editor `Mars > Setup Floating Popups`: membuat objek `FloatingPopupManager` (tidak menimpa yang sudah ada) |
| `Assets/Script/UI/MachinePanelUI.cs` | Panel kontrol mesin (Water Purifier, Oxygen Generator, Solar Panel) dengan tombol dan bar; UI dibuat lewat kode di bawah Canvas; `MachinePanelUI.IsOpen` statis |
| `Assets/Editor/MarsMachinePanelSetup.cs` | Menu Editor `Mars > Setup Machine Panels`: memasang `MachinePanelUI` pada Canvas dan menyambungkan `WaterPurifier`, `OxygenGenerator`, `RepairPoint` |

## Aset
| Path | Isi |
|---|---|
| `Assets/Player/` | Sprite pemain (depan, atas), `Down.anim`, `Top/up.anim`, `Square.controller` |
| `Assets/Robot/` | Sprite TARS: `Left/` (3), `Right/` (3), `Up/` (3). **Folder Down belum ada** |
| `Assets/COPY SPRIGHT/` | Asset pack astronot (demo scene) |
| `Assets/UI/PixelUISciFi/` | Pixel UI Sci-Fi Free (heyheythere, CC BY 4.0): `cyan/` (panel, progress), `bars/`, `icons/` (16x16), `CREDITS.txt`, `LICENSE.txt`. Border 9-slice dari `slices.json` aset |

## Setup objek di scene (agar bisa direproduksi)
**TARS:** SpriteRenderer, Rigidbody2D (Dynamic, Gravity Scale 0, Freeze Rotation Z), BoxCollider2D (bukan trigger), layer `Interactable`, komponen `TarsWanderer` (isi Wander Area, Dialogue UI, Tars System (otomatis dicari), array frame per arah).
**TarsArea:** GameObject dengan BoxCollider2D, Is Trigger dicentang.
**Player:** komponen `InteractionDetector` dengan Interactable Layer = `Interactable`.
**ResourceManager:** GameObject kosong `ResourceManager` di scene dengan komponen `ResourceManager`. Komponen `ResourceDebugPanel` dihapus 2026-11-27 (script dan scene di patch).
**TimeManager dan HUD:** dibuat lewat menu Mars > Setup HUD and Time (GameObject `TimeManager`; `Canvas/HudRoot` berisi `TimePanel` dan `ResourcePanel`). Perubahan scene dilakukan developer, belum diverifikasi AI [?].
**Bed dan Fade:** dibuat lewat menu Mars > Setup Bed and Fade (`Canvas/FadeOverlay`; GameObject `Bed` dengan anak `SpawnPoint`). Sprite Bed belum ada (placeholder oleh developer). Posisi di scene: [Dev], belum diverifikasi [?].
**Suit dan Gate:** dibuat lewat menu Mars > Setup Suit and Gate (placeholder di sekitar Player). WAJIB diposisikan developer: `IndoorZone` menutupi seluruh interior dengan tepi sejajar pintu; `SuitGate` di pintu dan menutup seluruh lebar pintu; `SuitStation` di Control Room dengan sprite. Posisi akhir: [Dev], belum diverifikasi [?].
**Objek interaktif (patch 2026-11-15):** dibuat lewat menu Mars > Setup Interactive Objects: `IceDeposit`, `SolarPanel`, `RepairPoint`, `WaterPurifier`, `OxygenGenerator`, `FarmingPlot` (SpriteRenderer kotak placeholder, BoxCollider2D trigger 1.5x1.5, layer `Interactable`, diletakkan di sekitar Player) dan `HudRoot/IcePanel` (+ `iceText`). WAJIB developer: memindahkan tiap objek ke lokasi akhirnya (Outdoor: IceDeposit, SolarPanel, RepairPoint; Farming Area: WaterPurifier, OxygenGenerator, FarmingPlot; Control Room: tidak ada; ResourceMonitor dan CommunicationTerminal dihapus 2026-11-16), mengganti sprite placeholder, dan menyimpan scene. Posisi akhir: [Dev], belum diverifikasi [?]. Objek belum ada di `SampleScene.unity` di repo [Repo].
**Status scene objek interaktif (2026-11-16):** [Repo, d96d9b6f] `SampleScene.unity` yang ter-commit belum berisi objek tersebut. [Dev] scene lokal sudah berisi objek hasil setup dan tes interaksi lolos; belum di-commit.
**Kerusakan panel dan minigame (patch 2026-11-16):** jalankan menu Mars > Setup Panel Damage and Wiring Minigame (komponen `WiringMinigameUI` pada Canvas; GameObject `SolarPanelDamageScheduler`; field Minigame pada `RepairPoint`). Simpan scene dengan Ctrl+S. [Belum dijalankan/dites]
**Popup mengambang (patch 2026-11-17):** jalankan menu Mars > Setup Floating Popups (GameObject `FloatingPopupManager`). Ikon final: impor PNG (Sprite, Filter Mode Point, Compression None), seret ke slot ikon; font piksel: seret TTF ke slot Font. Simpan scene dengan Ctrl+S. [Belum dijalankan/dites]
**Panel mesin (patch 2026-11-18):** jalankan menu Mars > Setup Machine Panels (komponen `MachinePanelUI` pada Canvas; mengisi field Panel UI pada mesin dan RepairPoint). Simpan scene dengan Ctrl+S. [Belum dijalankan/dites]
**Skin HUD (patch 2026-11-18):** jalankan menu Mars > Apply HUD Skin (Pixel UI Sci-Fi) setelah Setup HUD and Time (+ Suit/Hunger bila dipakai), lalu simpan scene. Menambah anak `<Key>Icon` dan `<Key>Bar` (`FillArea/Fill`) di tiap panel dan mengisi field bar `HudUI`. Posisi akhir: [Dev], belum diverifikasi [?].
**Canvas:** dibuat lewat menu Mars > Setup Dialogue and Prompt UI (berisi `DialogueUI` dan `InteractionPromptUI`).

## Saat menambah/mengubah asset
- Pertahankan pasangan `.meta` dan GUID.
- Jangan menghapus atau mengganti asset yang mungkin direferensikan scene/prefab tanpa pemeriksaan.
- Berikan langkah Inspector secara eksplisit: GameObject, komponen, field, nilai, dan referensi yang perlu diisi.
- Untuk perubahan binary/serialized yang tidak aman dibuat lewat patch teks, jelaskan langkah manual.

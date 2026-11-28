# PHASE.md — Pelacak Fase MARS: LAST HARVEST

> File konteks untuk AI dan developer. Baca bersama GDD v1.2 dan Project Instructions.
> GDD = otoritas desain. Repository = otoritas implementasi. File ini = peta kemajuan per milestone dan subphase.
> **WAJIB:** setiap patch yang mengubah kode, scene, aset, desain, atau keputusan harus ikut memperbarui file ini (lihat bagian 1).

## 0. Snapshot

| Item | Nilai |
|---|---|
| Repo | https://github.com/rifkiachmadfa/martian.git |
| Branch | `main` |
| Revisi terakhir diperiksa | `a0e7727f` (commit "solar fix", clone `main` 2026-11-28); patch `tutorial-story.patch` (tutorial Day 1 bercerita) dibuat di atas revisi ini; patch `tutorial-card-ui.patch` (kartu panduan tutorial) bertumpuk di atas `tutorial-story.patch`; patch `tutorial-card-style.patch` (gaya terakota + portrait TARS) bertumpuk di atas `tutorial-card-ui.patch`. Sebelumnya: `7af5e74a` (2026-11-17, commit "final tutorial", clone `main`); patch `floating-popups.patch` (Milestone 14, popup ikon + teks) dibuat di atas revisi ini. Patch `farming-status-label.patch` (label hitung mundur panen) bertumpuk di atas `floating-popups.patch`. Sebelumnya: `80bc5a6d` (commit "phase11"); patch tutorial Day 1 (Milestone 12, `TutorialManager`) dibuat di atas revisi ini; patch `tutorial-eat-sleep.patch` dibuat di atas `fe6dc9ae` (commit "tutorial update"); patch `farming-realtime.patch` bertumpuk di atasnya |
| Unity | 6000.6.8f1 |
| Paket penting | Input System 1.20.1 (kode memakai `Input.GetAxisRaw` / `KeyCode`), URP 17.6.0, 2D Animation 16.0.1, Tilemap, Cinemachine |
| Scene | Satu scene: `Assets/Scenes/SampleScene.unity` |
| Fase aktif | Fitur inti milestone 1–9 sudah ada di kode; butir sisa tiap milestone tercantum di bagian 3. Fase berikutnya menunggu keputusan developer (bagian 2) |
| Catatan | AI tidak menjalankan Unity atau compiler. Status berasal dari membaca repo. Objek interaktif di scene: lokal developer, belum di-commit (**Dilaporkan developer**). |

## 1. Aturan PHASE.md (WAJIB)

**Legenda tanda** (menunjukkan *implementasi di repo*, bukan hasil tes):
- `[x]` sudah diimplementasikan di `main` (kode/aset ada).
- `[~]` sebagian, placeholder, atau sementara (alasan ditulis di butir).
- `[ ]` belum dikerjakan.

**Struktur:** Milestone (jadwal 15 hari GDD) → subphase (`N.M`) → fitur (`N.M.K`); fitur dapat berkelompok (`N.M.K.J`), contoh Resource System di 3.3. ID bersifat tetap; jangan dipakai ulang bila butir dihapus.

**Setiap perubahan wajib:**
1. Sebelum membuat perubahan: baca PHASE.md, tentukan milestone dan subphase yang terdampak, dan periksa tidak melanggar daftar Won't Have (GDD 18.3).
2. Di patch yang sama: ubah tanda butir yang terdampak. Fitur baru yang belum punya tempat ditambahkan sebagai subphase atau butir baru dengan ID berikutnya (setelah dicek terhadap GDD).
3. Perbarui tabel ringkasan di bagian 2 (jumlah dan status milestone), snapshot revisi di bagian 0, dan baris baru di Riwayat (bagian 9).
4. Perbarui bagian 4 (penyimpangan) dan bagian 5 (keputusan) bila ada.
5. Jangan menandai `[x]` untuk fitur yang hanya direncanakan. Patch tanpa pembaruan PHASE.md dianggap belum lengkap.

Hasil tes (bila ada) dicatat terpisah di `09_TEST_CHECKLIST.md`, bukan di sini.

## 2. Ringkasan progres

Dihitung dari checklist bagian 3 (revisi `7af5e74a` + patch popup 2026-11-17).

| # | Milestone | Status | Selesai | Sebagian | Belum | Total |
|---|---|---|---|---|---|---|
| 1 | Setup Unity, input, movement, kamera, collision | SEBAGIAN | 8 | 1 | 0 | 9 |
| 2 | Tiga lokasi Tilemap dan perpindahan area | SEBAGIAN | 4 | 4 | 3 | 11 |
| 3 | Interaction System, Resource System, data persisten | SEBAGIAN | 17 | 0 | 2 | 19 |
| 4 | HUD, timer, pergantian hari | SEBAGIAN | 9 | 0 | 2 | 11 |
| 5 | Astronaut Suit, Suit Station, oksigen suit | SEBAGIAN | 9 | 0 | 0 | 9 |
| 6 | Ice Gathering dan Water Purifier | SEBAGIAN | 6 | 0 | 1 | 7 |
| 7 | Farming Plot (tanam, siram, tumbuh, panen) | SEBAGIAN | 6 | 2 | 1 | 9 |
| 8 | Oxygen Generator, alur Water/Energy/Oxygen | SEBAGIAN | 5 | 1 | 0 | 6 |
| 9 | Solar Panel dan produksi Energy | SEBAGIAN | 7 | 0 | 1 | 8 |
| 10 | Panel rusak dan minigame kabel | SEBAGIAN | 10 | 1 | 0 | 11 |
| 11 | TARS monitoring dan reminder | SEBAGIAN | 14 | 2 | 2 | 18 |
| 12 | Integrasi loop dan tutorial Day 1 | SEBAGIAN | 18 | 0 | 1 | 19 |
| 13 | Day 2–7, Game Over/Victory, balancing | SEBAGIAN | 0 | 1 | 12 | 13 |
| 14 | Audio, animasi, polish | SEBAGIAN | 3 | 3 | 11 | 17 |
| 15 | Playtest penuh dan build | BELUM | 0 | 0 | 4 | 4 |
| | **Total** | | **115** | **15** | **40** | **170** |

Usulan urutan berikutnya (**belum disetujui developer**): 13 (loop menang/kalah minimal: Food berkurang, Game Over, Victory, perilaku `Bed` di Day 7) agar game bisa dimainkan Day 1 sampai Day 7, lalu 11 (`TARSSystem` sudah dibuat, menunggu tes Unity), 12 (tutorial), 14, 15. Urutan jadwal GDD: 10, 11, 12, 13, 14, 15. Build WebGL percobaan sebaiknya lebih awal.
Jika jadwal mundur: kurangi polish dan fitur Should Have dulu; jangan korbankan loop resource inti dan sistem tujuh hari.

## 3. Checklist per milestone

### Milestone 1 — Setup Unity, input, movement, kamera, collision
Acuan: GDD 19 hari 1. Status: **SEBAGIAN** (8 selesai, 1 sebagian, 0 belum dari 9).

**1.1 Setup proyek** (3/3)
- [x] `1.1.1` Unity 6000.6.8f1 dengan URP 17.6.0, 2D Animation, Tilemap, Cinemachine
- [x] `1.1.2` Input memakai Input Manager lama (`Input.GetAxisRaw`/`KeyCode`); paket Input System terpasang tetapi belum dipakai (jangan ganti tanpa konfirmasi)
- [x] `1.1.3` Layer `Interactable` terdaftar di `TagManager`

**1.2 Player** (3/4)
- [x] `1.2.1` Gerak WASD/Arrow dengan `Rigidbody2D`, input dinormalisasi (`PlayerMovement`)
- [x] `1.2.2` `MovementEnabled` untuk mengunci gerak (tidur, mengumpulkan es)
- [x] `1.2.3` Parameter Animator `IsMoving`, `DirectionX`, `DirectionY`
- [~] `1.2.4` Animasi arah Player: controller `Square` baru punya state `Down` dan `up` (kiri/kanan belum)

**1.3 Kamera** (1/1)
- [x] `1.3.1` `CinemachineCamera` mengikuti Player

**1.4 Collision** (1/1)
- [x] `1.4.1` Collision dinding lewat `TilemapCollider2D` di scene

### Milestone 2 — Tiga lokasi Tilemap dan perpindahan area
Acuan: GDD 19 hari 2; GDD 3. Status: **SEBAGIAN** (4 selesai, 4 sebagian, 3 belum dari 11). Butir 2.1.3 (Resource Monitor dan Terminal) dihapus 2026-11-16, lihat 11.4.

**2.1 Control Room** (2/2)
- [x] `2.1.1` Tilemap interior dan dinding (`Ground`, `Wall`, `Tangga`) serta `IndoorZone`
- [x] `2.1.2` `Bed` + `SpawnPoint`, `SuitStation`, dan `TARS` ada di scene

**2.2 Outdoor** (1/2)
- [x] `2.2.1` Tilemap permukaan Mars (`PasirMars`) dan `SuitGate` di pintu keluar
- [~] `2.2.2` `IceDeposit`, `SolarPanel`, `RepairPoint` ada di scene lokal developer, belum di-commit

**2.3 Farming Area** (0/2)
- [ ] `2.3.1` Tilemap/area Farming Area
- [~] `2.3.2` `WaterPurifier`, `OxygenGenerator`, `FarmingPlot` ada di scene lokal developer, belum di-commit

**2.4 Perpindahan area** (1/3)
- [x] `2.4.1` Control Room ke Outdoor lewat `SuitGate`/`IndoorZone` (satu scene)
- [ ] `2.4.2` Farming Area terhubung ke Control Room
- [~] `2.4.3` Keputusan struktur scene (P-3): saat ini satu scene; rekomendasi AI tetap satu scene

**2.5 Aset visual lingkungan** (0/2)
- [~] `2.5.1` Objek interaktif memakai placeholder kotak berwarna
- [ ] `2.5.2` Sprite final objek interaktif dan dekorasi Tilemap

### Milestone 3 — Interaction System, Resource System, data persisten
Acuan: GDD 19 hari 3; GDD 7, 16. Status: **SEBAGIAN** (17 selesai, 0 sebagian, 2 belum dari 19).

**3.1 Interaction System** (3/3)
- [x] `3.1.1` `IInteractable` (prompt, `CanInteract`, `Interact`)
- [x] `3.1.2` `InteractionDetector` (OverlapCircle, layer `Interactable`, tombol E)
- [x] `3.1.3` `InteractionPromptUI` menampilkan "[E] ..."

**3.2 Dialogue UI** (3/3)
- [x] `3.2.1` `DialogueUI` baris demi baris (E, Spasi, klik)
- [x] `3.2.2` Tombol X menutup dialog langsung
- [x] `3.2.3` Callback saat dialog ditutup

**3.3 Resource System (`ResourceManager`)** (10/10)
- **3.3.1 Model data**
  - [x] `3.3.1.1` Enum `ResourceType`: Ice, Water, HabitatOxygen, SuitOxygen, Energy, Food
  - [x] `3.3.1.2` Stok awal dan kapasitas tiap resource lewat Inspector (angka usulan)
  - [x] `3.3.1.3` Satu stok Water bersama (GDD 7.1)
  - [x] `3.3.1.4` HabitatOxygen dan SuitOxygen terpisah (GDD 7.2)
  - [x] `3.3.1.5` Ice sebagai inventory terbatas (kapasitas 5, usulan)
- **3.3.2 API**
  - [x] `3.3.2.1` `Get`, `GetMax`, `Has`, `Add`, `TryConsume`
  - [x] `3.3.2.2` Event `OnResourceChanged` untuk HUD
  - [x] `3.3.2.3` Singleton `ResourceManager.Instance`
- **3.3.3 Aturan konsumsi**
  - [x] `3.3.3.1` Water dipakai Farming dan Oxygen Generator; Energy dipakai Water Purifier dan Oxygen Generator
- **3.3.4 Alat bantu**
  - [x] `3.3.4.1` `ResourceDebugPanel` (alat uji sementara)

**3.4 Persistensi** (1/3)
- [x] `3.4.1` State resource tetap selama satu scene (tidak reset antar lokasi)
- [ ] `3.4.2` `SaveSystem` (simpan antar hari/scene ke disk)
- [ ] `3.4.3` Simpan state saat tidur

### Milestone 4 — HUD, timer, pergantian hari
Acuan: GDD 19 hari 4; GDD 5, 13. Status: **SEBAGIAN** (9 selesai, 0 sebagian, 2 belum dari 11). Resource Monitor dihapus 2026-11-16; HUD dan storage menggantikannya.

**4.1 TimeManager** (3/5)
- [x] `4.1.1` Hari 1 sampai 7, timer 600 detik per hari (usulan)
- [x] `4.1.2` Event `OnDayStarted`, `OnDayTimeEnded`, method `StartDay`
- [x] `4.1.3` `ExternalPause` dan opsi `pauseDuringDialogue`
- [ ] `4.1.4` Grace period saat waktu habis di Outdoor
- [ ] `4.1.5` Peringatan akhir hari (warna timer)

**4.2 HUD** (3/3)
- [x] `4.2.1` HUD Day, timer, Food, Water, Oxygen, Energy lewat event (`HudUI`)
- [x] `4.2.2` Panel Suit O2 (kuning di 30 persen, merah di 10 persen)
- [x] `4.2.3` Panel Ice (opsional)

**4.4 Tidur dan pergantian hari** (3/3)
- [x] `4.4.1` `Bed` + `FadeUI`: kunci gerak, fade hitam, `StartDay(hari+1)`, pindah ke `SpawnPoint`
- [x] `4.4.2` Suit dilepas otomatis saat tidur
- [x] `4.4.3` Aturan tidur: hanya saat sisa waktu <= 120 detik atau waktu habis (usulan)

### Milestone 5 — Astronaut Suit, Suit Station, oksigen suit
Acuan: GDD 19 hari 5; GDD 9. Status: **SEBAGIAN** (9 selesai, 0 sebagian, 0 belum dari 9).

**5.1 SuitSystem** (5/5)
- [x] `5.1.1` Kenakan dan lepas suit; isi ulang dari HabitatOxygen
- [x] `5.1.2` Oksigen suit berkurang selama suit dikenakan, termasuk di dalam habitat (P-1, disetujui 2026-11-15; GDD belum direvisi)
- [x] `5.1.3` Oksigen minimum untuk keluar (20, usulan)
- [x] `5.1.4` `IsOutdoors` ditentukan lewat `IndoorZone`
- [x] `5.1.5` Event `OnSuitOxygenEmpty`

**5.2 Suit Station dan gerbang** (3/3)
- [x] `5.2.1` `SuitStation`: kenakan, isi, lepas
- [x] `5.2.2` `SuitGate` menahan pemain tanpa suit/oksigen minimum, TARS menjelaskan alasan
- [x] `5.2.3` `IndoorZone` (trigger area dalam habitat)

**5.3 Pengingat suit** (1/1)
- [x] `5.3.1` Pengingat "lepas suit" di dalam habitat kini dikerjakan `TARSSystem` (`SuitReminder` lama menonaktifkan diri bila `TARSSystem` ada)

### Milestone 6 — Ice Gathering dan Water Purifier
Acuan: GDD 19 hari 6; GDD 9.1, 11.1. Status: **SEBAGIAN** (6 selesai, 0 sebagian, 1 belum dari 7).

**6.1 Ice Deposit** (3/4)
- [x] `6.1.1` Interaksi E, pemain berhenti sekitar 3 detik, Ice +1, batas kapasitas
- [x] `6.1.2` Wajib mengenakan suit
- [x] `6.1.3` Deposit tidak habis (usulan; keputusan terbuka)
- [ ] `6.1.4` Sprite dan efek deposit es

**6.2 Water Purifier** (3/3)
- [x] `6.2.1` Ice + Energy menjadi Water per batch (1 + 5 menjadi 4 dalam 4 detik; usulan)
- [x] `6.2.2` Penolakan bila Ice atau Energy kurang, atau tangki Water tidak muat
- [x] `6.2.3` Indikator aktif/nonaktif (slot GameObject)

### Milestone 7 — Farming Plot (tanam, siram, tumbuh, panen)
Acuan: GDD 19 hari 7; GDD 10. Status: **SEBAGIAN** (6 selesai, 2 sebagian, 1 belum dari 9).

**7.1 Tanam dan siram** (2/2)
- [x] `7.1.1` Tanam benih kentang (gratis, tanpa stok seed)
- [x] `7.1.2` Siram memakai 2 Water (usulan), cukup SATU kali per tanaman (revisi 2026-11-17, sebelumnya satu kali per hari)

**7.2 Pertumbuhan** (2/2)
- [x] `7.2.1` 3 tahap visual; setelah disiram tumbuh waktu nyata selama durasi acak 60 sampai 120 detik (USULAN, `growSecondsMin/Max`), lalu matang; timer ikut jeda `TimeManager.IsPaused`; pergantian hari tidak mempengaruhi (revisi 2026-11-17, permintaan developer; menggantikan "maju saat `OnDayStarted`", lihat P-6)
- [x] `7.2.2` Tanaman tidak disiram tidak maju dan tidak mati

**7.3 Panen** (2/2)
- [x] `7.3.1` Panen memberi 2 sampai 4 kentang dan 1 sampai 2 bibit secara acak (USULAN, `potatoMin/Max`, `seedMin/Max`; diundi saat disiram); kentang masuk tas, bibit masuk tas atau storage bila tas penuh (revisi 2026-11-17, sebelumnya +20 Food)
- [x] `7.3.2` Panen ditunda bila tas kentang tidak muat hasil panen

**7.4 Visual dan penataan** (0/3)
- [~] `7.4.1` Warna placeholder sebagai penanda tahap
- [ ] `7.4.2` Sprite 2 sampai 3 tahap tanaman
- [~] `7.4.3` Satu objek = satu petak (petak tambahan lewat duplikasi); penataan beberapa petak di Farming Area belum ada

### Milestone 8 — Oxygen Generator, alur Water/Energy/Oxygen
Acuan: GDD 19 hari 8; GDD 11.2. Status: **SEBAGIAN** (5 selesai, 1 sebagian, 0 belum dari 6).

**8.1 Produksi** (3/3)
- [x] `8.1.1` Otomatis per siklus 5 detik: 1 Water + 2 Energy menjadi 5 Oxygen habitat (usulan)
- [x] `8.1.2` Berhenti sendiri bila Water/Energy kurang atau oksigen habitat penuh
- [x] `8.1.3` Indikator aktif/nonaktif (slot GameObject)

**8.2 Kontrol pemain** (0/1)
- [~] `8.2.1` Saklar E menyalakan/mematikan mesin (di luar GDD; menunggu keputusan P-5)

**8.3 Alur** (2/2)
- [x] `8.3.1` HabitatOxygen menjadi sumber pengisian suit
- [x] `8.3.2` Status mesin tersedia lewat `StatusText` (dulu dibaca Resource Monitor; kini untuk TARS, lihat 11.4.2)

### Milestone 9 — Solar Panel dan produksi Energy
Acuan: GDD 19 hari 9; GDD 11.3. Status: **SEBAGIAN** (7 selesai, 0 sebagian, 1 belum dari 8).

**9.1 Produksi Energy** (3/3)
- [x] `9.1.1` Energy otomatis per tick (0,5 per detik per panel normal; usulan)
- [x] `9.1.2` Banyak panel menjumlah
- [x] `9.1.3` Produksi berhenti saat jeda (`TimeManager.IsPaused`)

**9.2 Status rusak** (3/3)
- [x] `9.2.1` `Damage()`/`Repair()`, `IsDamaged`, event `OnDamagedChanged`
- [x] `9.2.2` Panel rusak menghasilkan 25 persen (usulan), tidak mematikan habitat
- [x] `9.2.3` Tampilan rusak lewat tint atau slot sprite rusak

**9.3 Konsumsi Energy** (1/2)
- [x] `9.3.1` Water Purifier dan Oxygen Generator mengonsumsi Energy
- [ ] `9.3.2` Sprite panel normal/rusak final

### Milestone 10 — Panel rusak dan minigame kabel
Acuan: GDD 19 hari 10; GDD 11.4. Status: **SEBAGIAN** (10 selesai, 1 sebagian, 0 belum dari 11). Kode ditulis 2026-11-16; BELUM dikompilasi/dites di Unity.

**10.1 Pemicu kerusakan** (3/3)
- [x] `10.1.1` Menu konteks Debug tetap ada untuk uji (`SolarPanel` dan `SolarPanelDamageScheduler`)
- [x] `10.1.2` Keputusan pemicu: **satu kejadian acak per hari** (developer, 2026-11-16)
- [x] `10.1.3` Pemicu otomatis: `SolarPanelDamageScheduler` (waktu acak dalam hari, satu panel normal dipilih acak, pengumuman dialog TARS)

**10.2 Repair Point** (2/2)
- [x] `10.2.1` `RepairPoint` terhubung ke satu `SolarPanel`
- [x] `10.2.2` `RepairPoint` membuka minigame; perbaikan langsung hanya sebagai cadangan bila `WiringMinigameUI` tidak ada di scene

**10.3 Minigame kabel** (5/6)
- [x] `10.3.1` UI 3 sampai 4 kabel berwarna dan konektor (`WiringMinigameUI`, dibuat lewat kode di bawah Canvas)
- [x] `10.3.2` Drag-and-drop dengan mouse
- [x] `10.3.3` Salah sambung boleh dicoba lagi
- [~] `10.3.4` Durasi sekitar 15 sampai 30 detik: tidak ada batas waktu; hanya ukuran teka-teki (4 kabel). Durasi nyata belum diukur
- [x] `10.3.5` Timer hari berhenti selama minigame (`ExternalPause`, nilai sebelumnya dipulihkan)
- [x] `10.3.6` Sukses memanggil `SolarPanel.Repair()` (lewat callback `RepairPoint`)

### Milestone 11 — TARS monitoring dan reminder
Acuan: GDD 19 hari 11; GDD 12. Status: **SEBAGIAN** (14 selesai, 2 sebagian, 2 belum dari 18). Kode dibuat 2026-11-17, belum dikompilasi/dites di Unity.

**11.1 TARS (NPC)** (3/3)
- [x] `11.1.1` `TarsWanderer`: jalan 4 arah, langkah pendek, cek rintangan, menembus Player
- [x] `11.1.2` Animasi sprite 3 frame per arah (atas, bawah, kiri, kanan)
- [x] `11.1.3` Dialog saat E; TARS berhenti dan menghadap Player

**11.2 TARSSystem** (7/8) — `Assets/Script/TARS/TARSSystem.cs`, menu `Mars > Setup TARS System`
- **11.2.1 Pemantauan**
  - [x] `11.2.1.1` Pemantau ambang resource (Water, Oxygen habitat, Energy, stok Food di storage; Oxygen suit saat suit dikenakan). Ambang 30 persen / 10 persen / habis, dapat diubah di Inspector
  - [x] `11.2.1.2` Antrean dengan prioritas (Info/Warning/Critical), cooldown per pesan, jeda antar pesan, kedaluwarsa, dan validasi sebelum tampil
  - [x] `11.2.1.3` Dialog predefined berbasis event/ambang, tanpa AI generatif (aturan proyek)
- **11.2.2 Reminder**
  - [x] `11.2.2.1` Resource rendah (Water, Food storage, Energy, Oxygen habitat; kenyang lewat `HungerReminder` yang kini memakai antrean TARS)
  - [~] `11.2.2.2` Isi suit sebelum keluar: pengisian tabung otomatis saat suit dikenakan, `SuitGate` dan `SuitStation` menjelaskan bila belum cukup; TARS hanya memperingatkan oksigen suit rendah (belum ada pengingat khusus "isi suit")
  - [x] `11.2.2.3` Oksigen suit menipis (30 persen dan 10 persen dapat dikonfigurasi, dan habis)
  - [x] `11.2.2.4` Solar Panel rusak (TARS berlangganan `SolarPanel.OnDamagedChanged`; scheduler tidak lagi mengumumkan sendiri bila `TARSSystem` ada)
  - [x] `11.2.2.5` Akhir hari (sisa waktu <= 60 dtk, usulan), waktu habis, dan pesan awal hari

**11.3 Migrasi dan variasi** (1/2)
- [x] `11.3.1` Logika `SuitReminder` dipindahkan ke `TARSSystem`; `HungerReminder` dan pengumuman panel `SolarPanelDamageScheduler` dialihkan ke antrean TARS
- [ ] `11.3.2` Variasi komentar TARS (Should Have)

**11.4 Pengganti Resource Monitor dan Terminal komunikasi** (3/5)
Latar: keduanya dihapus 2026-11-16 atas permintaan developer (HUD dan Storage sudah menampilkan stok). Fungsi yang belum tertutup HUD/Storage diambil TARS lewat dialog E pada NPC `TARS` (TarsWanderer), tanpa quest.
- [~] `11.4.1` Pesan dari Bumi per hari (cerita, bukan quest): disampaikan di awal hari dan saat TARS diajak bicara; array `earthMessages` di Inspector `TARSSystem`. Teks bawaan hanya PLACEHOLDER, **teks cerita ditulis developer**
- [x] `11.4.2` Laporan fasilitas atas permintaan (dialog E): Solar Panel (normal/rusak, Energy per detik), `WaterPurifier.StatusText`, `OxygenGenerator.StatusText`
- [x] `11.4.3` Laporan kenyang dari `HungerSystem` dan kondisi suit bila pemain bertanya
- [x] `11.4.4` Ringkasan stok dan sisa waktu hari (dalam dialog E)
- [ ] `11.4.5` Pesan penutup Day 7 (konfirmasi penjemputan) milik ending Victory (13.3.3); API siap: `TARSSystem.Announce(key, prioritas, teks)`

### Milestone 12 — Integrasi loop dan tutorial Day 1
Acuan: GDD 19 hari 12; GDD 5.2. Status: **SEBAGIAN** (18 selesai, 0 sebagian, 1 belum dari 19). Kode dibuat 2026-11-17, direvisi 2026-11-28 (tutorial bercerita); belum dikompilasi/dites di Unity. Semua syarat langkah dan teks cerita adalah USULAN (teks ditulis AI, perlu ditinjau developer).

**12.1 Alur tutorial Day 1** (12/12)
- [x] `12.1.1` Mengenal Control Room, TARS, HUD, dan Storage (Resource Monitor sudah dihapus): selesai bila pemain mengajak TARS bicara DAN membuka Storage (yang ada di scene)
- [x] `12.1.2` Mengambil dan mengisi Astronaut Suit: selesai bila `SuitSystem.IsEquipped` (Suit Station mengisi tabung otomatis)
- [x] `12.1.3` Keluar dan mengumpulkan Ice dalam jumlah kecil: selesai bila 5 es (`iceToCollect`, USULAN, dibatasi kapasitas tas) berhasil dikumpulkan di Ice Deposit (`IceDeposit.OnCollected`) atau tas es penuh
- [x] `12.1.4` Membawa Ice ke Water Purifier dan menghasilkan Water: selesai bila satu batch `WaterPurifier` selesai (`OnProcessingChanged` false)
- [x] `12.1.5` Menanam dan menyiram di Farming Plot: selesai bila ada `FarmingPlot` dengan `HasPlant` dan `IsWatered`
- [x] `12.1.6` Mengenal Oxygen Generator dan hubungan Water/Energy: selesai bila `OxygenGenerator` menjadi target interaksi pemain (didekati); penjelasan Water/Energy ada di arahan TARS (keputusan USULAN, lihat P-5)
- [x] `12.1.7` Kembali ke Control Room dan tidur: selesai saat Day 2 mulai (`Bed` memanggil `StartDay`). Arahan TARS dan garis arah ke Bed baru muncul SETELAH timer hari habis (`TimeManager.IsDayOver`; `OnDayTimeEnded` memicu pengumuman), bukan saat langkah dimulai (permintaan developer 2026-11-17)
- [x] `12.1.8` Melepas suit di Suit Station (ditambahkan 2026-11-17; tindak lanjut keputusan 2026-11-15 "Day 1 tutorial perlu menjelaskan melepas suit"): selesai bila `SuitSystem.OnEquippedChanged(false)`. Letak dalam alur: setelah 12.1.4 (array `steps`); `TutorialStepId.RemoveSuit` bernilai 7 agar ID lama di scene tidak bergeser
- [x] `12.1.9` Makan kentang (ditambahkan 2026-11-17, permintaan developer): selesai bila kenyang naik (`HungerSystem.OnSatietyChanged`; satu-satunya jalan naik adalah `Feed`). Letak dalam alur: setelah 12.1.6 dan sebelum Sleep (array `steps`); `TutorialStepId.Eat` bernilai 8. Garis arah ke Storage bila tas tanpa kentang; bila tas berisi kentang tidak ada garis (pemain tekan 1 lalu E). Sejak 2026-11-28: garis ke petak matang bila tas tanpa kentang, baru Storage
- [x] `12.1.10` Adegan lapar `Hungry` (2026-11-28, permintaan developer: tutorial bercerita; hasil uji player menyebut tutorial membingungkan): saat langkah mulai, `HungerSystem.LowerSatietyTo(hungerBeatFraction)` menurunkan kenyang sekali (USULAN 32 persen, hanya menurunkan); TARS menjelaskan kenyang, HP, dan kentang sebelum langkah Farming. Selesai saat dialog dibaca (`TARSSystem.Announce` parameter `onClosed`); dilewati bila pemain sudah menanam dan menyiram. `TutorialStepId.Hungry` bernilai 9
- [x] `12.1.11` Panen `Harvest` (2026-11-28): selesai bila petak yang tadi bertanaman dan disiram menjadi kosong (dipanen), dideteksi `TutorialManager` tanpa mengubah `FarmingPlot`. Garis arah ke petak bertanaman. `TutorialStepId.Harvest` bernilai 10
- [x] `12.1.12` Ringkasan `Recap` (2026-11-28): TARS merangkum rantai Solar Panel > Energy > Water Purifier > Water > kebun dan Oxygen Generator; selesai saat dialog dibaca; tidak menyuruh tidur sebelum timer habis. `TutorialStepId.Recap` bernilai 11

**12.2 Mekanik tutorial** (7/7)
- [x] `12.2.1` Arahan TARS langkah demi langkah (bukan quest harian): `TutorialManager` mengumumkan arahan tiap langkah lewat `TARSSystem.Announce`. Sejak 2026-11-28 teks berupa cerita sebab-akibat (apa, mengapa, lalu baris "Sekarang: ..."), tanpa prefix PLACEHOLDER; teks USULAN AI, perlu ditinjau developer
- [x] `12.2.2` Status tutorial selesai tersimpan: `PlayerPrefs` kunci `Mars.TutorialCompleted` (USULAN sementara sampai ada SaveSystem; hanya disimpan bila tutorial selesai penuh, bukan dilewati)
- [x] `12.2.3` Tutorial dapat dilewati setelah pernah dimainkan (Should Have): bila status tersimpan ada, TARS memberi tahu dan tombol `skipKey` (default Tab, USULAN) melewati tutorial; flag Inspector `skipTutorial` untuk uji
- [x] `12.2.4` `TutorialManager`: sembilan langkah (`TutorialStepId`), data (`TutorialStepData`), event, `TryCompleteStep`, syarat dikunci (latched) sehingga urutan bebas, pengumuman ulang arahan tiap `reminderSeconds` (120, USULAN), selesai otomatis saat Day 2 mulai
- [x] `12.2.5` Garis arah tutorial (USULAN, hanya Day 1): `TutorialGuideUI` menggambar garis UI dari tubuh pemain ke tujuan langkah aktif (`TutorialManager.GetGuideTarget`: Intro = Storage lalu TARS, Suit = Suit Station, CollectIce = Ice Deposit, PurifyWater = Ice Deposit bila tas tanpa es lalu Water Purifier, Farming = petak/Storage sesuai kebutuhan, OxygenGenerator, Sleep = Bed); disembunyikan saat tutorial tidak aktif, panel Storage terbuka, pemain sudah dekat, dialog terbuka, atau masih ada pesan TARS yang mengantre (`TARSSystem.HasPendingMessages`): dialog selesai dulu, baru garis (2026-11-17)

- [x] `12.2.6` Urutan langkah baru (2026-11-28): Intro, Suit, CollectIce, PurifyWater, RemoveSuit, Hungry, Farming, OxygenGenerator, Harvest, Eat, Recap, Sleep (urutan item GDD 5.2 tetap). Flag `useBuiltInStory` (default true) memakai teks dan urutan bawaan kode saat Play karena scene lama menyimpan teks PLACEHOLDER; pengingat 120 detik hanya mengulang baris terakhir langkah
- [x] `12.2.7` Kartu panduan tutorial (2026-11-28, permintaan developer: tampilan berbeda dari dialog biasa): pesan tutorial tampil di `TutorialDialogueSkin` (tengah atas layar; sejak 2026-11-28 gaya satu keluarga warna terakota dengan bingkai gelap, kotak portrait TARS di kiri dari `TarsWanderer.PortraitSprite`, tab nama "TARS | PANDUAN", nomor langkah, judul langkah, kotak TUJUAN SEKARANG warna lebih gelap untuk baris "Sekarang: ...") lewat `DialogueUI.Show(..., tutorialInfo)`; dialog biasa tetap di bawah. Input, `IsOpen`, dan callback tetap di `DialogueUI`. UI dibuat lewat kode, tanpa edit scene; butuh file `.meta` baru yang dibuat Unity saat impor

**12.3 Integrasi** (0/1)
- [ ] `12.3.1` Loop harian berjalan dari awal sampai akhir tanpa urutan wajib

### Milestone 13 — Day 2–7, Game Over/Victory, balancing
Acuan: GDD 19 hari 13; GDD 5, 6, 17. Status: **SEBAGIAN** (1 selesai, 1 sebagian, 12 belum dari 14).

**13.1 Food dan kelaparan** (0/2)
- [ ] `13.1.1` Food berkurang bertahap selama gameplay aktif
- [ ] `13.1.2` Kelaparan mencapai batas gagal

**13.2 Game Over** (0/5)
- [ ] `13.2.1` Countdown darurat saat oksigen suit habis di Outdoor
- [ ] `13.2.2` Game Over: Food habis dan kelaparan
- [ ] `13.2.3` Game Over: oksigen habitat kritis
- [ ] `13.2.4` Peringatan sebelum Game Over (indikator dan TARS)
- [ ] `13.2.5` Layar Game Over

**13.3 Victory dan Day 7** (0/4)
- [~] `13.3.1` `Bed` memblokir tidur di Day 7 dengan pesan placeholder
- [ ] `13.3.2` Akhir Day 7 tanpa Game Over memicu Victory
- [ ] `13.3.3` Konfirmasi Bumi dan pesan penutup TARS
- [ ] `13.3.4` Layar Victory

**13.4 Terminal komunikasi** — DIHAPUS 2026-11-16; pesan Bumi pindah ke TARS (11.4.1)

**13.5 Balancing** (0/2)
- [ ] `13.5.1` Tetapkan angka final lewat playtest (stok awal, laju, kapasitas, ambang)
- [ ] `13.5.2` Pastikan Energy cukup menopang mesin (satu panel normal hanya sedikit di atas konsumsi Oxygen Generator)

### Milestone 14 — Audio, animasi, polish
Acuan: GDD 19 hari 14; GDD 14, 15. Status: **SEBAGIAN** (3 selesai, 3 sebagian, 11 belum dari 17).

**14.1 Animasi** (1/5)
- [x] `14.1.1` Animasi TARS
- [~] `14.1.2` Animasi Player idle/walk (arah kiri/kanan belum)
- [ ] `14.1.3` Animasi tahap tanaman
- [~] `14.1.4` Indikator mesin aktif/nonaktif (belum animasi)
- [ ] `14.1.5` Tampilan panel normal/rusak final

**14.2 Audio** (0/4)
- [ ] `14.2.1` Ambience Control Room dan angin Outdoor
- [ ] `14.2.2` Suara mesin dan efek interaksi (es, siram, tanam, panen, perbaikan)
- [ ] `14.2.3` Notifikasi TARS dan alarm kritis
- [ ] `14.2.4` Audio tidur dan ending

**14.3 UI** (2/5)
- [ ] `14.3.1` Main Menu (Start, Settings, Quit)
- [ ] `14.3.2` Pause dan tutup panel dengan Esc
- [x] `14.3.3` Popup mengambang ikon + teks di dekat objek (2026-11-17, permintaan developer): `FloatingPopupManager` + hook di `IceDeposit` ("Es +N"), `FarmingPlot` (tanam "Bibit -1", siram "Water -N", panen "Kentang +N" dan "Bibit +N"), `WaterPurifier` ("Es -N", "Energy -N", lalu "Water +N"), `OxygenGenerator` ("Oxygen +N" per siklus; biaya Water/Energy opsional, bawaan mati), `RepairPoint` ("Panel diperbaiki"), `InventorySystem.UseDefault` (makan: "Kentang -1" dan "Kenyang +N" di atas Player). Hanya tampilan, tidak mengubah resource. Menu `Mars > Setup Floating Popups`. Kode ditulis tanpa Unity; belum dikompilasi/dites
- [x] `14.3.5` Label status tetap di atas `FarmingPlot` (2026-11-17, permintaan developer, agar pemain tidak spam interaksi): "Belum disiram (M:SS)" sejak ditanam, "Matang M:SS" menghitung mundur setelah disiram, "Siap panen!" saat matang, kosong bila petak kosong. Durasi tumbuh kini diundi saat TANAM (sebelumnya saat siram) supaya langsung tampil; pertumbuhan tetap baru berjalan setelah disiram. Popup petak dinaikkan agar tidak menimpa label. `FloatingPopupManager.CreateWorldLabel`, flag `showStatusLabel`. Belum dikompilasi/dites
- [~] `14.3.4` Ikon dan font popup final: saat ini ikon pixel 8x8 bawaan dibuat lewat kode (placeholder); sprite dari itch.io dan font piksel belum diimpor, diisi lewat slot Inspector `FloatingPopupManager`

**14.4 Pembersihan** (1/3)
- [x] `14.4.1` Hapus `ResourceDebugPanel` (2026-11-27, permintaan developer; script dan komponen scene dihapus, belum dites di Unity)
- [ ] `14.4.2` `Assets/_Recovery`, `Assets/Welcome`, dan `.meta` tanpa folder
- [ ] `14.4.3` `.gitignore` untuk `*.patch` dan `Assets/_Recovery/`

### Milestone 15 — Playtest penuh dan build
Acuan: GDD 19 hari 15; GDD 21. Status: **BELUM** (0 selesai, 0 sebagian, 4 belum dari 4).

**15.1 Playtest** (0/2)
- [ ] `15.1.1` Playtest penuh Day 1 sampai Day 7
- [ ] `15.1.2` Definition of Done (GDD 21) terpenuhi

**15.2 Build** (0/2)
- [ ] `15.2.1` Build Windows
- [ ] `15.2.2` Build WebGL (rekomendasi: percobaan lebih awal)

## 4. Penyimpangan dari GDD v1.2 dan keputusan terbuka

| ID | Temuan | Dampak | Status |
|---|---|---|---|
| P-1 | `SuitSystem` mengurangi oksigen suit **selama suit dikenakan, termasuk di dalam habitat** (komentar kode: "revisi GDD 2026-11-15"). GDD v1.2 bagian 7.2 dan 9 menyatakan hanya berkurang di Outdoor. | Mempengaruhi `SuitReminder`, balancing, dan alur Day 1 | **Menunggu keputusan developer.** Jika sengaja, perbarui GDD ke v1.3. |
| P-2 | Dialog tidak menjeda timer (`pauseDuringDialogue = false`). GDD: timer "dapat" berhenti saat dialog penting. | Kecil | Dianggap sesuai (opsional). |
| P-3 | Satu scene, bukan scene terpisah `ControlRoom`/`FarmingArea`/`Outdoor` seperti GDD 16.1. | Menentukan perlu tidaknya `SaveSystem` lintas scene | **Menunggu keputusan.** Rekomendasi AI: tetap satu scene. |
| P-4 | Perilaku `Bed` di Day 7 belum ada (menunggu Victory). | Blok akhir game | Dikerjakan di milestone 13. |
| P-5 | `OxygenGenerator` punya tombol E untuk menyalakan/mematikan. GDD 11.2 hanya menyebut mesin bekerja otomatis. Alasan: pemain perlu menghemat Water untuk tanaman (GDD 7.1). Default menyala, sehingga Water awal bisa habis dalam ±50 dtk bila tidak dimatikan. | Kecil di kode (`allowManualToggle`), memengaruhi balancing dan tutorial Day 1 | **Menunggu keputusan developer.** Jika ditolak, matikan `allowManualToggle`. |
| P-6 | Pertumbuhan tanaman waktu nyata (60 sampai 120 detik acak setelah satu kali siram) dan panen acak 2 sampai 4 kentang plus 1 sampai 2 bibit. GDD v1.2 bagian 5.5 dan 10 menyatakan tanaman maju satu tahap saat pergantian hari. | Rutinitas lebih cepat; satu petak bisa panen berkali-kali dalam sehari; Food dan bibit bisa berlebih (cek balancing) | Disetujui developer 2026-11-17 (permintaan eksplisit). Dokumen GDD perlu direvisi. |

| P-7 | Tutorial menurunkan kenyang pemain sekali di Day 1 (adegan lapar `Hungry`). Komentar lama `TutorialManager` menyatakan tutorial tidak mengubah resource. GDD 5.2 hanya meminta tutorial mengenalkan sistem. | Hanya Day 1, hanya menurunkan kenyang; angka 32 persen USULAN | Menunggu persetujuan developer (diminta eksplisit 2026-11-28 untuk alur bercerita). |
| P-8 | Hasil hitung dari nilai scene: stok Food awal 50 kentang (default `ResourceManager`, belum dicek override scene), kenyang turun 0,08 per detik (sekitar 48 per hari) dan satu kentang +12,5, jadi sekitar 3,8 kentang per hari; 50 kentang cukup sekitar 13 hari. Pemain secara angka tidak wajib berkebun untuk bertahan tujuh hari. | Melemahkan pesan cerita "kentang dari kebun adalah makanan kita"; teks tutorial sengaja tidak mengklaim stok terbatas | **Menunggu keputusan developer** (balancing: kurangi Food awal, atau Food storage tidak terisi saat awal). |

Keputusan terbuka lain: deposit es tak terbatas (usulan: ya), angka balancing semua mesin (termasuk frekuensi kerusakan panel: satu per hari, Day 2 dan seterusnya, mungkin terlalu sering; cek saat playtest), dan urutan fase berikutnya.

## 5. Keputusan yang sudah diambil (jangan diubah tanpa persetujuan)

- TARS bukan pemberi quest; dialog predefined, tanpa AI generatif.
- TARS berjalan lurus 4 arah, langkah pendek, menembus Player; interaksi tetap lewat E.
- Satu stok Water bersama; HabitatOxygen dan SuitOxygen terpisah (`ResourceType`).
- Resource hanya diubah lewat `ResourceManager`; HUD memakai event.
- Angka balancing di kode adalah **usulan**, bukan final.
- Alur kerja perubahan: AI menganalisis repo, lalu memberi patch `.patch`. Developer yang menerapkan (`git apply`). AI tidak commit atau push.
- Resource Monitor dan Terminal komunikasi DIHAPUS (2026-11-16, permintaan developer; menyimpang dari GDD v1.2 bagian 3.1, 12.1.1 tutorial). Fungsinya pindah ke TARS (11.4). Pesan Bumi tetap cerita, bukan quest.
- Sistem PHASE.md (2026-11-16): checklist per milestone dengan subphase; wajib diperbarui di setiap patch.
- Popup mengambang (2026-11-17, permintaan developer): umpan balik visual ikon + teks di dekat objek saat resource/item berubah. Hanya tampilan (tidak mengubah resource), dipanggil langsung dari skrip interaksi setelah perubahan terjadi. Bukan quest dan bukan fitur Won't Have; menambah umpan balik interaksi (GDD 13.1 prompt/HUD minimal, GDD 15 efek interaksi) tanpa mengubah HUD. Perlu persetujuan developer bila ingin cakupan lebih luas (Storage, Suit Station, Solar Panel Energy per detik).
- Pemicu kerusakan Solar Panel (2026-11-16, developer): satu event acak per hari. USULAN turunan (belum disetujui): mulai Day 2 (Day 1 tutorial bebas kerusakan, `firstDamageDay`), waktu kejadian acak 15-70 persen durasi hari, satu panel normal dipilih acak.

## 6. Inventaris implementasi saat ini

### Script (Assets/Script)
| Script | Fungsi singkat |
|---|---|
| `PlayerMovement` | Gerak 4 arah, `MovementEnabled` untuk mengunci gerak, kirim `IsMoving`/`DirectionX`/`DirectionY` ke Animator |
| `Interaction/IInteractable`, `InteractionDetector`, `InteractionPromptUI` | Deteksi objek interaktif dengan `OverlapCircle` (layer `Interactable`, trigger dihitung), tombol E, prompt "[E] ..." |
| `Interaction/Bed` | Tidur: kunci gerak, fade, lepas suit, `StartDay(day+1)`, pindah ke `SpawnPoint`. Boleh tidur bila sisa waktu <= 120 dtk (usulan). |
| `Resource/ResourceManager` | Satu-satunya stok: Ice, Water, HabitatOxygen, SuitOxygen, Energy, Food. API: `Get`, `GetMax`, `Has`, `Add`, `TryConsume`, event `OnResourceChanged`. |
| `Resource/ResourceType` | Enum 6 jenis stok |
| `Survival/HealthSystem` | HP pemain; berkurang tiap 2 dtk (USULAN 3 HP) selama kenyang 0; `OnHealthChanged`, `OnDied` (2026-11-27, belum dites) |
| `Time/TimeManager` | Hari, timer 600 dtk (usulan), 7 hari, `ExternalPause`, `OnDayStarted`, `OnDayTimeEnded`. Dialog tidak menjeda timer secara default. |
| `Suit/SuitSystem` | Equip/Unequip, refill dari habitat, drain oksigen, `IsOutdoors`, event `OnSuitOxygenEmpty` |
| `Suit/SuitStation`, `SuitGate`, `IndoorZone`, `SuitReminder` | Interaksi suit, tembok pintu keluar, zona dalam habitat, pengingat "lepas suit" (`SuitReminder` menonaktifkan diri bila `TARSSystem` ada) |
| `TARS/TARSSystem` | Antrean pesan TARS (prioritas, cooldown, kedaluwarsa), pemantau ambang resource dan oksigen suit, panel rusak, awal/akhir hari, pesan Bumi, laporan untuk dialog E pada NPC TARS; `Announce(...)` untuk sistem lain |
| `Tutorial/TutorialManager`, `TutorialStepId`, `TutorialStepData` | KERANGKA tutorial Day 1: urutan 7 langkah, arahan TARS per langkah via `TARSSystem.Announce`, event `OnStepStarted/OnStepCompleted/OnTutorialFinished`, `TryCompleteStep(id)`, selesai saat Day 2 mulai. Syarat selesai tiap langkah belum diisi |
| `Outdoor/IceDeposit` | Deposit es: E, pemain berhenti ±3 dtk, Ice +1 (batas kapasitas Ice), perlu suit (usulan) |
| `Outdoor/SolarPanel` | Energy otomatis per tick; `Damage()`/`Repair()`, `IsDamaged`, `OnDamagedChanged`; banyak panel menjumlah |
| `Outdoor/RepairPoint` | Membuka `WiringMinigameUI`; memperbaiki satu `SolarPanel` setelah minigame sukses (cadangan langsung bila minigame tidak ada) |
| `Outdoor/SolarPanelDamageScheduler` | Satu kejadian acak per hari: merusak satu panel normal pada waktu acak, mengumumkan lewat dialog TARS (sementara pengganti `TARSSystem`) |
| `UI/WiringMinigameUI` | Minigame kabel drag-and-drop mouse; UI dibuat lewat kode di bawah Canvas; menjeda `TimeManager` dan mengunci gerak Player selama terbuka; Esc membatalkan |
| `Machines/WaterPurifier` | Ice + Energy -> Water per batch (usulan 1 + 5 -> 4 dalam 4 dtk), `IsProcessing`, `OnProcessingChanged` |
| `Machines/OxygenGenerator` | Water + Energy -> HabitatOxygen otomatis per siklus (usulan 1 + 2 -> 5 per 5 dtk), saklar E, `IsOn`/`IsRunning` |
| `Farming/FarmingPlot` | Tanam, siram (usulan 2 Water), tumbuh 3 tahap saat hari berganti bila disiram, panen (usulan +20 Food) |
| `NPC/TarsWanderer` | TARS jalan 4 arah langkah pendek, cek rintangan (`Rigidbody2D.Cast`), animasi frame 1-2-3-2 per arah, menembus Player (`IgnoreCollision`), dialog saat E |
| `Dialogue/DialogueUI` | Dialog baris demi baris (E/Spasi/klik) |
| `UI/HudUI`, `UI/FadeUI` | HUD (hari, timer, Food, Water, Oxygen, Energy, Ice opsional, panel Suit Oxygen) dan layar fade |
| `UI/FloatingPopupManager` | Popup mengambang ikon + teks di dunia game (`ShowAmount`, `Show`, `AnchorAbove`, pool, tanpa Canvas); dipanggil `IceDeposit`, `FarmingPlot`, `WaterPurifier`, `OxygenGenerator`, `RepairPoint`, `InventorySystem`. Tanpa manager di scene, panggilan diabaikan (satu peringatan di Console) |

### Editor setup scripts (Assets/Editor) — pola kerja proyek
Menu yang aman dijalankan berulang, tidak menimpa referensi yang sudah diisi:
`Mars > Setup Dialogue and Prompt UI`, `Setup HUD and Time`, `Setup Bed and Fade`, `Setup Suit and Gate`, `Setup TARS Animation`, `Setup Interactive Objects` (8 objek placeholder + panel Ice), `Setup Panel Damage and Wiring Minigame` (pasang `WiringMinigameUI` di Canvas, buat `SolarPanelDamageScheduler`, sambungkan `RepairPoint`), `Setup TARS System` (buat objek `TARSSystem`, sambungkan `DialogueUI` dan `TarsWanderer`), `Setup Tutorial` (buat objek `TutorialManager`), `Setup Floating Popups` (baru: buat objek `FloatingPopupManager`).
Milestone baru sebaiknya memakai pola yang sama (satu menu setup per milestone, objek placeholder di sekitar Player).

### Objek penting di SampleScene
`Player`, `TARS`, `TarsArea`, `TARSSystem` (dibuat menu `Setup TARS System`, lokal developer sampai di-commit), `TutorialManager` (dibuat menu `Setup Tutorial`), `Bed`, `SpawnPoint`, `SuitStation`, `SuitGate`, `IndoorZone`, `SuitSystem`, `ResourceManager`, `TimeManager`, `DialogueManager`, `Canvas` (`HudRoot`, `DialogueRoot`, `PromptRoot`, `FadeOverlay`), `CinemachineCamera`, `Grid`, `Ground`, `PasirMars`, `Wall`, `Tangga`.
Objek interaktif berikut ada di scene lokal developer (**Dilaporkan developer**); belum ada di `SampleScene.unity` yang ter-commit (tidak ada GUID script objek interaktif di scene): `IceDeposit`, `SolarPanel`, `RepairPoint`, `WaterPurifier`, `OxygenGenerator`, `FarmingPlot`, `HudRoot/IcePanel` (masih di sekitar Player; developer memindahkan ke lokasi akhir). `ResourceMonitor` dan `CommunicationTerminal` dihapus 2026-11-16 (hapus juga GameObject-nya di scene).
Layer `Interactable` sudah ada (layer 3).

### Aset
`Assets/Robot` (12 sprite TARS, sudah diimpor sebagai Sprite Point filter), `Assets/Player` (animasi Player), `Assets/COPY SPRIGHT`.

## 7. Utang teknis dan kebersihan repo

- `Assets/_Recovery/0.unity` (file pemulihan otomatis Unity) ikut ter-commit; sebaiknya masuk `.gitignore`. Terverifikasi masih ada di `afc2453c`.
- `phase-sync.patch` ter-commit di root repo (`interactive-objects.patch` sudah dihapus di `b09da2e3`). File `.patch` tidak perlu ikut repo; hapus dan tambahkan `*.patch` ke `.gitignore`.
- `Assets/Suit.meta` dan `Assets/popopop.meta` adalah `.meta` tanpa folder.
- `Assets/Welcome` adalah sisa template Unity.
- `ResourceDebugPanel` masih ada (alat uji sementara).
- `SuitReminder` sudah dipindahkan ke `TARSSystem` (menonaktifkan diri); komponen dan script-nya boleh dihapus setelah `TARSSystem` lolos tes.
- `SuitGate`, `SuitStation`, dan pesan mesin masih memanggil `DialogueUI` langsung (bukan lewat antrean TARS); dialog tidak menumpuk karena semuanya mengecek `DialogueUI.IsOpen`, tetapi dua pemanggil pada frame yang sama masih mungkin saling menimpa.
- Belum ada `SaveSystem`; belum ada Game Over dan Victory.
- Kode memakai Input Manager lama (`Input.GetAxisRaw`, `KeyCode`) meski paket Input System terpasang. Jangan ganti sistem input tanpa konfirmasi.

## 8. Catatan praktis untuk patch

- File kode di repo berakhir baris LF; developer memakai Windows. Jika `git apply` gagal karena akhir baris, pakai `git apply --ignore-whitespace`.
- Patch yang membuat file baru harus menyertakan `.meta` (GUID baru) untuk script; PNG/aset biner dan perubahan scene dikerjakan lewat Editor setup script atau langkah manual.
- Perubahan scene (`SampleScene.unity`) dilakukan lewat menu Editor, lalu developer menyimpan dengan Ctrl+S. Hindari mengedit YAML scene secara manual.
- Setelah developer commit, clone ulang repo sebelum membuat patch berikutnya.

## 9. Riwayat

| Tanggal | Perubahan |
|---|---|
| 2026-11-28 | Gaya kartu panduan diubah: palet terakota tunggal (tanpa cyan/amber/merah), kotak portrait TARS di kiri, tab nama; `TarsWanderer.PortraitSprite` ditambahkan; `TutorialDialogueSkin.Create` menerima portrait. Total butir tidak berubah (115/15/40/170). Basis `a0e7727f` + `tutorial-story.patch` + `tutorial-card-ui.patch`. Belum dikompilasi/dites di Unity. |
| 2026-11-28 | Kartu panduan tutorial: `TutorialDialogueSkin` (baru), `DialogueUI.Show` overload `tutorialInfo`, `TARSSystem.Announce` parameter `tutorialInfo`, `TutorialManager` mengirim info langkah. Butir 12.2.7. Total 115/15/40/170. Basis `a0e7727f` + `tutorial-story.patch`. Belum dikompilasi/dites di Unity. |
| 2026-11-28 | Tutorial Day 1 bercerita: teks cerita sebab-akibat, langkah baru `Hungry`/`Harvest`/`Recap`, `HungerSystem.LowerSatietyTo`, `TARSSystem.Announce` parameter `onClosed`, flag `useBuiltInStory`. Penyimpangan P-7 dan P-8. Total 114/15/40/169. Basis `a0e7727f`. Belum dikompilasi/dites di Unity. |
| 2026-11-15 | Versi pertama dibuat dari pemeriksaan repo `f0df08f9`. Animasi dan gerak TARS selesai (3 patch). Milestone 6 dijadwalkan sebagai fase berikutnya. |
| 2026-11-15 | Patch objek interaktif (berbasis `a9357e29`): IceDeposit, WaterPurifier, OxygenGenerator, FarmingPlot, SolarPanel + RepairPoint, ResourceMonitor, CommunicationTerminal, menu setup, Ice di HUD. Belum dikompilasi/dites. Milestone 6–10 menjadi SEBAGIAN; penyimpangan P-5 ditambahkan. |
| 2026-11-15 | Sinkronisasi dengan repo `afc2453c`: patch objek interaktif terkonfirmasi sudah di-commit (script, `.meta`, AI_CONTEXT). Terverifikasi dari clone: `SampleScene.unity` belum memuat objek interaktif; layer `Interactable` ada di `TagManager`; `interactive-objects.patch` dan `Assets/_Recovery` masih ter-commit. Tidak ada perubahan kode. |
| 2026-11-16 | Sistem PHASE.md dirancang ulang: checklist per milestone dengan subphase dan fitur, ringkasan progres terhitung, aturan wajib pembaruan di setiap patch. Basis repo `d96d9b6f`. Tidak ada perubahan kode. |
| 2026-11-16 | Milestone 10: `SolarPanelDamageScheduler` (satu kerusakan acak per hari), `WiringMinigameUI`, `RepairPoint` membuka minigame, menu `Setup Panel Damage and Wiring Minigame`. Basis `16ca3c15`. Belum dikompilasi/dites di Unity. Butir 10.x diperbarui, total 86/14/58. |
| 2026-11-16 | Hapus `ResourceMonitor` dan `CommunicationTerminal` (permintaan developer). Butir 11.4 (pengganti lewat TARS) ditambahkan. Total 84/13/63/160. Basis `63f64857`. Belum dikompilasi/dites di Unity. |
| 2026-11-17 | Milestone 11: `TARSSystem` (antrean, ambang, panel, hari, pesan Bumi, laporan dialog E), menu `Setup TARS System`; `SuitReminder`, `HungerReminder`, `SolarPanelDamageScheduler`, `TarsWanderer` disambungkan. Total 96/14/50/160. Basis `95a2aa97`. Belum dikompilasi/dites di Unity. |
| 2026-11-17 | Milestone 12: `TutorialManager`, `TutorialStepId`, `TutorialStepData`, menu `Setup Tutorial`; semua syarat 7 langkah, simpan/lewati tutorial (PlayerPrefs, Tab), event `TarsWanderer.OnPlayerTalked`. Total 106/15/40/161. Basis `80bc5a6d`. Belum dikompilasi/dites di Unity. |
| 2026-11-17 | Milestone 12: `TutorialGuideUI` (garis arah UI ke tujuan langkah tutorial), `TutorialManager.GetGuideTarget`; menu `Setup Tutorial` memasang komponen di Canvas. Butir 12.2.5 ditambahkan. Total 107/15/40/162. Basis `80bc5a6d` + `tutorial-day1.patch`. Belum dikompilasi/dites di Unity. |
| 2026-11-17 | Perbaikan tutorial: langkah CollectIce selesai setelah 5 es dikumpulkan (event `IceDeposit.OnCollected`) atau tas es penuh, tidak lagi bergantung `IsOutdoors`; garis arah tidak macet di Ice Deposit. Total tidak berubah (107/15/40/162). |
| 2026-11-17 | Milestone 12: langkah tutorial `Eat` (makan kentang, 12.1.9); arahan dan garis arah Sleep menunggu timer hari habis; `TutorialGuideUI` disembunyikan saat dialog terbuka atau pesan TARS mengantre (`TARSSystem.HasPendingMessages`). Total 108/15/40/163. Basis `fe6dc9ae` + `tutorial-eat-sleep.patch`. Belum dikompilasi/dites di Unity. |
| 2026-11-17 | Milestone 7: pertumbuhan `FarmingPlot` waktu nyata (durasi acak 60 sampai 120 detik setelah satu kali siram), panen acak 2 sampai 4 kentang dan 1 sampai 2 bibit; tidak lagi bergantung `OnDayStarted`. Penyimpangan P-6. Total butir tidak berubah (108/15/40/163). Basis `fe6dc9ae` + `tutorial-eat-sleep.patch` + `farming-realtime.patch`. Belum dikompilasi/dites di Unity. |
| 2026-11-17 | Milestone 14: label hitung mundur panen di atas `FarmingPlot` (14.3.5); durasi tumbuh diundi saat tanam. Total 110/16/40/166. Basis `7af5e74a` + `floating-popups.patch`. Belum dikompilasi/dites di Unity. |
| 2026-11-17 | Milestone 14: popup mengambang ikon + teks (`FloatingPopupManager`, menu `Setup Floating Popups`) dan hook di `IceDeposit`, `FarmingPlot`, `WaterPurifier`, `OxygenGenerator`, `RepairPoint`, `InventorySystem`. Butir 14.3.3 (selesai di kode) dan 14.3.4 (sebagian, ikon placeholder) ditambahkan. Total 109/16/40/165. Basis `7af5e74a`. Belum dikompilasi/dites di Unity. |

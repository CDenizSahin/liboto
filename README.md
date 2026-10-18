# 📚 Kütüphane Otomasyon Sistemi

Üniversite öğrencilerinin kitap arayabildiği, ödünç alabildiği ve iade edebildiği bir kütüphane yönetim yazılımı. **.NET 9 / C#** ile geliştirilmiş, **NUnit** ile test edilmiştir.

## 🏗 Proje Yapısı

```
LibraryOto/
├── src/
│   └── KutuphaneOtomasyon.Core/     # İş Mantığı (OOP Sınıflar)
│       └── Models/
│           ├── Kullanici.cs         # Soyut taban sınıf
│           ├── Ogrenci.cs           # Öğrenci (Inheritance)
│           ├── Yonetici.cs          # Yönetici (Inheritance)
│           ├── Kitap.cs             # Kitap modeli
│           ├── OduncIslemi.cs       # Ödünç/gecikme/ceza mantığı
│           ├── Kutuphane.cs         # Merkez koordinatör
│           └── Durum.cs             # Enum
└── tests/
    └── KutuphaneOtomasyon.Tests/    # NUnit Test Katmanı
        ├── KitapTests.cs            # 10 test
        ├── OduncIslemiTests.cs      # 11 test
        ├── OgrenciTests.cs          # 18 test
        ├── KutuphaneTests.cs        # 14 test
        ├── YoneticiTests.cs         # 10 test
        └── EntegrasyonTests.cs      #  6 uçtan uca senaryo
```

## ✅ Test Sonuçları

```
Toplam test sayısı : 69
          Geçti   : 69
  Toplam süre     : ~0.59 Saniye
```

## 🚀 Çalıştırma

```bash
# Build
dotnet build KutuphaneOtomasyon.sln

# Testleri çalıştır
dotnet test KutuphaneOtomasyon.sln

# Kategoriye göre filtrele
dotnet test --filter "Category=Kitap"
dotnet test --filter "Category=Entegrasyon"
```

## 🏛 OOP Prensipleri

| Prensip | Uygulama |
|---|---|
| **Encapsulation** | `private` stok/şifre alanları, `property` getter/setter |
| **Abstraction** | `abstract class Kullanici` + `abstract bool GirisYap()` |
| **Inheritance** | `Ogrenci : Kullanici`, `Yonetici : Kullanici` |
| **Polymorphism** | `GirisYap()` her alt sınıfta farklı davranır |

## 📦 Teknolojiler

- **.NET 9 / C# 13**
- **NUnit 4** — Unit & Entegrasyon testleri
- **Solution mimarisi** — Core + Tests ayrı projeler

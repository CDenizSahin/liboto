using KutuphaneOtomasyon.Core.Models;
using NUnit.Framework;

namespace KutuphaneOtomasyon.Tests;

/// <summary>
/// OduncIslemi sınıfının gecikme, ceza ve iade mantığını test eder.
/// </summary>
[TestFixture]
[Category("OduncIslemi")]
public class OduncIslemiTests
{
    private Kitap   _kitap   = null!;
    private Ogrenci _ogrenci = null!;

    [SetUp]
    public void Setup()
    {
        _kitap = new Kitap("KIT-001", "978-X", "Test Kitabı",
                           "Yazar", "Kategori", 2, 2020);
        _ogrenci = new Ogrenci("USR-001", "Ali", "Yılmaz",
                               "ali@test.com", "1234",
                               "20210001", "Bilgisayar Müh.");
    }

    private OduncIslemi OlusturIslem(int gecikmeGun = 0)
    {
        var oduncTarihi = DateTime.Today.AddDays(-20 - gecikmeGun);
        var iadeTarihi  = DateTime.Today.AddDays(-gecikmeGun);     // Bugün veya geçmişte
        return new OduncIslemi("ISL-001", _ogrenci, _kitap,
                               oduncTarihi, iadeTarihi);
    }

    // ── GecikmeHesapla ───────────────────────────────────────────────────────

    [Test]
    public void GecikmeHesapla_IadeTarihiGecmemis_SifirDoner()
    {
        var islem = new OduncIslemi("ISL-T", _ogrenci, _kitap,
            DateTime.Today.AddDays(-5), DateTime.Today.AddDays(5));
        Assert.That(islem.GecikmeHesapla(), Is.EqualTo(0));
    }

    [Test]
    public void GecikmeHesapla_UcGunGecikme_UcDoner()
    {
        var islem = new OduncIslemi("ISL-T", _ogrenci, _kitap,
            DateTime.Today.AddDays(-17), DateTime.Today.AddDays(-3));
        Assert.That(islem.GecikmeHesapla(), Is.EqualTo(3));
    }

    // ── CezaHesapla ──────────────────────────────────────────────────────────

    [Test]
    public void CezaHesapla_UcGunGecikme_AltiliraTL()
    {
        var islem = new OduncIslemi("ISL-T", _ogrenci, _kitap,
            DateTime.Today.AddDays(-17), DateTime.Today.AddDays(-3));
        // 3 gün × 2 TL/gün = 6 TL
        Assert.That(islem.CezaHesapla(), Is.EqualTo(6.0m));
    }

    [Test]
    public void CezaHesapla_GecikmeYok_SifirTL()
    {
        var islem = new OduncIslemi("ISL-T", _ogrenci, _kitap,
            DateTime.Today.AddDays(-5), DateTime.Today.AddDays(5));
        Assert.That(islem.CezaHesapla(), Is.EqualTo(0m));
    }

    // ── IadeyiTamamla ────────────────────────────────────────────────────────

    [Test]
    public void IadeyiTamamla_ZamanindaIade_DurumIadeEdildi()
    {
        var islem = new OduncIslemi("ISL-T", _ogrenci, _kitap,
            DateTime.Today.AddDays(-5), DateTime.Today.AddDays(5));
        islem.IadeyiTamamla();
        Assert.That(islem.Durum, Is.EqualTo(Durum.IadeEdildi));
    }

    [Test]
    public void IadeyiTamamla_GecikmisIade_DurumGecikti()
    {
        var islem = new OduncIslemi("ISL-T", _ogrenci, _kitap,
            DateTime.Today.AddDays(-20), DateTime.Today.AddDays(-3));
        islem.IadeyiTamamla();
        Assert.That(islem.Durum, Is.EqualTo(Durum.Gecikti));
    }

    [Test]
    public void IadeyiTamamla_ZatenIadeEdildi_InvalidOperationException()
    {
        var islem = new OduncIslemi("ISL-T", _ogrenci, _kitap,
            DateTime.Today.AddDays(-5), DateTime.Today.AddDays(5));
        islem.IadeyiTamamla();
        Assert.Throws<InvalidOperationException>(() => islem.IadeyiTamamla());
    }

    // ── GecikiyorMu ──────────────────────────────────────────────────────────

    [Test]
    public void GecikiyorMu_AktifVeGecmis_True()
    {
        var islem = new OduncIslemi("ISL-T", _ogrenci, _kitap,
            DateTime.Today.AddDays(-20), DateTime.Today.AddDays(-3));
        Assert.That(islem.GecikiyorMu(), Is.True);
    }

    [Test]
    public void GecikiyorMu_IadeEdilmis_False()
    {
        var islem = new OduncIslemi("ISL-T", _ogrenci, _kitap,
            DateTime.Today.AddDays(-20), DateTime.Today.AddDays(-3));
        islem.IadeyiTamamla();
        Assert.That(islem.GecikiyorMu(), Is.False);
    }

    // ── RaporOlustur ─────────────────────────────────────────────────────────

    [Test]
    public void RaporOlustur_GecerliBilgiler_OgrenciAdiIcerir()
    {
        var islem = OlusturIslem();
        var rapor = islem.RaporOlustur();
        Assert.That(rapor, Does.Contain("Ali Yılmaz"));
    }

    [Test]
    public void RaporOlustur_GecerliBilgiler_KitapAdiIcerir()
    {
        var islem = OlusturIslem();
        Assert.That(islem.RaporOlustur(), Does.Contain("Test Kitabı"));
    }
}

using KutuphaneOtomasyon.Core.Models;
using NUnit.Framework;

namespace KutuphaneOtomasyon.Tests;

/// <summary>
/// Ogrenci sınıfının ödünç alma, iade, arama ve limit kontrollerini test eder.
/// </summary>
[TestFixture]
[Category("Ogrenci")]
public class OgrenciTests
{
    private Ogrenci    _ogrenci   = null!;
    private Kitap      _kitap     = null!;
    private Kutuphane  _kutuphane = null!;

    [SetUp]
    public void Setup()
    {
        _kutuphane = new Kutuphane("Test Kütüphanesi", "Test Adres");
        _kitap     = new Kitap("KIT-001", "978-X", "Temiz Kod",
                               "Robert Martin", "Programlama", 3, 2008);
        _ogrenci   = new Ogrenci("USR-001", "Ali", "Yılmaz",
                                 "ali@test.com", "ali123",
                                 "20210001", "Bilgisayar Müh.");
        _kutuphane.KitapEkle(_kitap);
        _kutuphane.UyeEkle(_ogrenci);
    }

    // ── GirisYap ─────────────────────────────────────────────────────────────

    [Test]
    public void GirisYap_DogruBilgiler_True()
        => Assert.That(_ogrenci.GirisYap("ali@test.com", "ali123"), Is.True);

    [Test]
    public void GirisYap_YanlisEmail_False()
        => Assert.That(_ogrenci.GirisYap("yanlis@test.com", "ali123"), Is.False);

    [Test]
    public void GirisYap_YanlisSifre_False()
        => Assert.That(_ogrenci.GirisYap("ali@test.com", "yanlis"), Is.False);

    // ── KitapAra ─────────────────────────────────────────────────────────────

    [Test]
    public void KitapAra_VarOlanBaslik_SonucGetir()
    {
        var sonuc = _ogrenci.KitapAra("Temiz", _kutuphane.TumKitaplariGetir());
        Assert.That(sonuc, Has.Count.EqualTo(1));
    }

    [Test]
    public void KitapAra_VarOlanYazar_SonucGetir()
    {
        var sonuc = _ogrenci.KitapAra("Martin", _kutuphane.TumKitaplariGetir());
        Assert.That(sonuc, Has.Count.EqualTo(1));
    }

    [Test]
    public void KitapAra_VarOlmayanAnahtar_BosListe()
    {
        var sonuc = _ogrenci.KitapAra("XYZBilinmeyen", _kutuphane.TumKitaplariGetir());
        Assert.That(sonuc, Is.Empty);
    }

    [Test]
    public void KitapAra_BuyukKucukHarfDuyarsiz_SonucGetir()
    {
        var sonuc = _ogrenci.KitapAra("temiz kod", _kutuphane.TumKitaplariGetir());
        Assert.That(sonuc, Has.Count.EqualTo(1));
    }

    // ── KitapOduncAl ─────────────────────────────────────────────────────────

    [Test]
    public void KitapOduncAl_MusaitKitap_IslemDoner()
    {
        var islem = _ogrenci.KitapOduncAl(_kitap, _kutuphane);
        Assert.That(islem, Is.Not.Null);
    }

    [Test]
    public void KitapOduncAl_MusaitKitap_StokAzalir()
    {
        _ogrenci.KitapOduncAl(_kitap, _kutuphane);
        Assert.That(_kitap.MevcutSayi, Is.EqualTo(2));
    }

    [Test]
    public void KitapOduncAl_MusaitKitap_AktifSayiArtar()
    {
        _ogrenci.KitapOduncAl(_kitap, _kutuphane);
        Assert.That(_ogrenci.AktifOduncSayisi, Is.EqualTo(1));
    }

    [Test]
    public void KitapOduncAl_MusaitKitap_IslemKutuphaneyeKaydedilir()
    {
        _ogrenci.KitapOduncAl(_kitap, _kutuphane);
        Assert.That(_kutuphane.TumIslemleriGetir(), Has.Count.EqualTo(1));
    }

    [Test]
    public void KitapOduncAl_StokYokKitap_NullDoner()
    {
        var doluKitap = new Kitap("KIT-DOLU", "X", "Dolu", "Y", "K", 1, 2020);
        doluKitap.StokAzalt(); // Stoku bitir
        var islem = _ogrenci.KitapOduncAl(doluKitap, _kutuphane);
        Assert.That(islem, Is.Null);
    }

    [Test]
    public void KitapOduncAl_LimitDolunca_NullDoner()
    {
        // 5 farklı kitap oluştur ve ödünç al (limit = 5)
        for (int i = 0; i < Ogrenci.MaxOduncLimit; i++)
        {
            var k = new Kitap($"KIT-{i}", "X", $"Kitap-{i}", "Y", "K", 1, 2020);
            _ogrenci.KitapOduncAl(k, _kutuphane);
        }
        var ekstraKitap = new Kitap("KIT-EXTRA", "X", "Extra", "Y", "K", 1, 2020);
        var islem = _ogrenci.KitapOduncAl(ekstraKitap, _kutuphane);
        Assert.That(islem, Is.Null);
    }

    // ── KitapIade ────────────────────────────────────────────────────────────

    [Test]
    public void KitapIade_GecerliIslem_AktifSayiAzalir()
    {
        var islem = _ogrenci.KitapOduncAl(_kitap, _kutuphane)!;
        _ogrenci.KitapIade(islem);
        Assert.That(_ogrenci.AktifOduncSayisi, Is.EqualTo(0));
    }

    [Test]
    public void KitapIade_GecerliIslem_StokArtir()
    {
        var islem = _ogrenci.KitapOduncAl(_kitap, _kutuphane)!;
        _ogrenci.KitapIade(islem);
        Assert.That(_kitap.MevcutSayi, Is.EqualTo(3));
    }

    [Test]
    public void KitapIade_BaskaOgrencininIslemi_InvalidOperationException()
    {
        var digerOgrenci = new Ogrenci("USR-002", "Mehmet", "Çelik",
                                       "mehmet@test.com", "1234",
                                       "20210002", "Elektrik Müh.");
        var islem = new OduncIslemi("ISL-BASKA", digerOgrenci, _kitap,
                                    DateTime.Today.AddDays(-5),
                                    DateTime.Today.AddDays(9));
        Assert.Throws<InvalidOperationException>(() => _ogrenci.KitapIade(islem));
    }

    // ── OduncGecmisiniGor ────────────────────────────────────────────────────

    [Test]
    public void OduncGecmisiniGor_IkiIslemSonrasi_IkiKayit()
    {
        var k2 = new Kitap("KIT-002", "Y", "Kitap 2", "Y", "K", 1, 2021);
        _ogrenci.KitapOduncAl(_kitap, _kutuphane);
        _ogrenci.KitapOduncAl(k2, _kutuphane);
        Assert.That(_ogrenci.OduncGecmisiniGor(), Has.Count.EqualTo(2));
    }

    // ── Email Setter ─────────────────────────────────────────────────────────

    [Test]
    public void EmailSetter_GecersizEmail_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _ogrenci.Email = "gecersizEmail");
    }

    [Test]
    public void EmailSetter_GecerliEmail_GuncelleniYapar()
    {
        _ogrenci.Email = "yeni@test.com";
        Assert.That(_ogrenci.Email, Is.EqualTo("yeni@test.com"));
    }
}

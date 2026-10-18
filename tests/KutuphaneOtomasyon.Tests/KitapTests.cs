using KutuphaneOtomasyon.Core.Models;
using NUnit.Framework;

namespace KutuphaneOtomasyon.Tests;

/// <summary>
/// Kitap sınıfının tüm davranışlarını test eder.
/// Kapsam: stok yönetimi, müsaitlik kontrolü, hata durumları.
/// </summary>
[TestFixture]
[Category("Kitap")]
public class KitapTests
{
    private Kitap _kitap = null!;

    [SetUp]
    public void Setup()
    {
        _kitap = new Kitap("KIT-001", "978-0-13-468599-1",
                           "Yazılım Mühendisliği", "Ian Sommerville",
                           "Mühendislik", 3, 2016);
    }

    // ── Oluşturma ─────────────────────────────────────────────────────────────

    [Test]
    public void Olustur_GecerliParametreler_OzelliklerDogru()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_kitap.KitapId,   Is.EqualTo("KIT-001"));
            Assert.That(_kitap.Baslik,    Is.EqualTo("Yazılım Mühendisliği"));
            Assert.That(_kitap.Yazar,     Is.EqualTo("Ian Sommerville"));
            Assert.That(_kitap.StokSayisi, Is.EqualTo(3));
            Assert.That(_kitap.MevcutSayi, Is.EqualTo(3));
        });
    }

    [Test]
    public void Olustur_NegatifStok_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Kitap("K", "I", "B", "Y", "K", -1, 2020));
    }

    // ── Müsaitlik ─────────────────────────────────────────────────────────────

    [Test]
    public void MusaitMi_BaslangicStokluKitap_True()
        => Assert.That(_kitap.MusaitMi(), Is.True);

    [Test]
    public void MusaitMi_TumStokAzaltilinca_False()
    {
        _kitap.StokAzalt();
        _kitap.StokAzalt();
        _kitap.StokAzalt();
        Assert.That(_kitap.MusaitMi(), Is.False);
    }

    // ── StokAzalt ────────────────────────────────────────────────────────────

    [Test]
    public void StokAzalt_BirAzaltilinca_MevcutSayiEksilir()
    {
        _kitap.StokAzalt();
        Assert.That(_kitap.MevcutSayi, Is.EqualTo(2));
    }

    [Test]
    public void StokAzalt_StokSifirken_InvalidOperationException()
    {
        _kitap.StokAzalt(); _kitap.StokAzalt(); _kitap.StokAzalt();
        Assert.Throws<InvalidOperationException>(() => _kitap.StokAzalt());
    }

    // ── StokArttir ───────────────────────────────────────────────────────────

    [Test]
    public void StokArttir_AzaltildiktanSonra_GeriArtar()
    {
        _kitap.StokAzalt();
        _kitap.StokArttir();
        Assert.That(_kitap.MevcutSayi, Is.EqualTo(3));
    }

    [Test]
    public void StokArttir_LimitAsilinca_InvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => _kitap.StokArttir());
    }

    // ── BilgileriGoster ──────────────────────────────────────────────────────

    [Test]
    public void BilgileriGoster_MevcutKitap_MüsaitIcerir()
        => Assert.That(_kitap.BilgileriGoster(), Does.Contain("Müsait"));

    [Test]
    public void BilgileriGoster_StokBitince_StokYokIcerir()
    {
        _kitap.StokAzalt(); _kitap.StokAzalt(); _kitap.StokAzalt();
        Assert.That(_kitap.BilgileriGoster(), Does.Contain("Stokta Yok"));
    }
}

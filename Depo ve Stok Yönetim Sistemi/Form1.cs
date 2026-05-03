using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Depo_ve_Stok_Yönetim_Sistemi
{
    public class Urun
    {
        [DisplayName("Ürün ID")]
        public int urun_id { get; set; }

        [DisplayName("Ürün Adı")]
        public string ad { get; set; }

        [DisplayName("Mevcut Stok")]
        public int stok { get; set; }

        [DisplayName("Birim Fiyat (TL)")]
        public double fiyat { get; set; }

        public void stok_arttir(int miktar)
        {
            this.stok += miktar;
        }

        public bool stok_azalt(int miktar)
        {
            if (this.stok >= miktar)
            {
                this.stok -= miktar;
                return true;
            }
            return false;
        }

        public override string ToString() => ad;
    }

    public class Siparis
    {
        [DisplayName("Sipariş ID")]
        public int siparis_id { get; set; }

        [DisplayName("Sipariş Verilen Ürün")]
        public string urun { get; set; }

        [DisplayName("Sipariş Adedi")]
        public int adet { get; set; }

        [DisplayName("Toplam Tutar (TL)")]
        public double toplam_tutar { get; set; }

        public Siparis siparis_olustur(Urun secilenUrun, int siparisAdedi, int islemId)
        {
            bool stokYeterliMi = secilenUrun.stok_azalt(siparisAdedi);

            if (stokYeterliMi)
            {
                return new Siparis
                {
                    siparis_id = islemId,
                    urun = secilenUrun.ad,
                    adet = siparisAdedi,
                    toplam_tutar = secilenUrun.fiyat * siparisAdedi
                };
            }
            return null;
        }
    }

    public partial class Form1 : Form
    {
        List<Urun> urunler = new List<Urun>();
        List<Siparis> siparisler = new List<Siparis>();

        int urunSayac = 1001;
        int siparisSayac = 50001;

        TabControl sekmeler;
        TabPage sekmeUrun, sekmeStok, sekmeSiparis;
        DataGridView dgvUrunler, dgvSiparisler;
        ComboBox cmbStokUrun, cmbSiparisUrun;
        TextBox txtUrunAd, txtFiyat, txtStokMiktar, txtSiparisMiktar;

        public Form1()
        {
            this.Text = "Depo ve Stok Yönetim Sistemi - 2300005412 Fırat Diricanlı";
            this.Size = new Size(1150, 750);
            this.StartPosition = FormStartPosition.CenterScreen;

            SistemVerileriniHazirla();
            ArayuzuInsaEt();
        }

        private void SistemVerileriniHazirla()
        {
            urunler.Add(new Urun { urun_id = 101, ad = "Asus Monitör 24 inç", stok = 50, fiyat = 4500.00 });
            urunler.Add(new Urun { urun_id = 102, ad = "Logitech Kablosuz Mouse", stok = 120, fiyat = 450.00 });
            urunler.Add(new Urun { urun_id = 103, ad = "Mekanik Oyuncu Klavyesi", stok = 85, fiyat = 1250.00 });
            urunler.Add(new Urun { urun_id = 104, ad = "Samsung 1TB NVMe SSD", stok = 200, fiyat = 1800.00 });
            urunler.Add(new Urun { urun_id = 105, ad = "Intel Core i7 İşlemci", stok = 30, fiyat = 8500.00 });
            urunler.Add(new Urun { urun_id = 106, ad = "Nvidia RTX 4060 Ekran Kartı", stok = 15, fiyat = 14500.00 });
            urunler.Add(new Urun { urun_id = 107, ad = "Corsair 16GB DDR4 RAM", stok = 150, fiyat = 1100.00 });
            urunler.Add(new Urun { urun_id = 108, ad = "HP Lazer Yazıcı", stok = 40, fiyat = 3200.00 });
        }

        private void ArayuzuInsaEt()
        {
            sekmeler = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

            sekmeUrun = new TabPage("Ürün ve Envanter Yönetimi");
            sekmeStok = new TabPage("Stok Giriş / Çıkış İşlemleri");
            sekmeSiparis = new TabPage("Sipariş Yönetimi");

            Panel pnlUrun = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = Color.WhiteSmoke };
            txtUrunAd = new TextBox { Location = new Point(20, 30), Width = 250, PlaceholderText = "Sisteme Eklenecek Ürün Adı" };
            txtFiyat = new TextBox { Location = new Point(290, 30), Width = 150, PlaceholderText = "Birim Fiyatı (TL)" };
            Button btnUrunEkle = new Button { Text = "YENİ ÜRÜN KAYDET", Location = new Point(460, 28), Size = new Size(200, 32), BackColor = Color.SteelBlue, ForeColor = Color.White };
            btnUrunEkle.Click += (s, e) => UrunKaydet();

            dgvUrunler = TabloOlustur();
            pnlUrun.Controls.AddRange(new Control[] { txtUrunAd, txtFiyat, btnUrunEkle });
            sekmeUrun.Controls.Add(dgvUrunler);
            sekmeUrun.Controls.Add(pnlUrun);

            Panel pnlStok = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Color.WhiteSmoke };
            Label l1 = new Label { Text = "İşlem Yapılacak Ürün:", Location = new Point(20, 30), AutoSize = true };
            cmbStokUrun = new ComboBox { Location = new Point(180, 28), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            Label l2 = new Label { Text = "İşlem Miktarı:", Location = new Point(20, 70), AutoSize = true };
            txtStokMiktar = new TextBox { Location = new Point(180, 68), Width = 150 };

            Button btnStokArtir = new Button { Text = "STOK ARTIR", Location = new Point(350, 65), Size = new Size(160, 32), BackColor = Color.SeaGreen, ForeColor = Color.White };
            btnStokArtir.Click += (s, e) => StokIslemiYap("Artir");

            Button btnStokAzalt = new Button { Text = "STOK AZALT (FİRE/HASAR)", Location = new Point(520, 65), Size = new Size(220, 32), BackColor = Color.Crimson, ForeColor = Color.White };
            btnStokAzalt.Click += (s, e) => StokIslemiYap("Azalt");

            pnlStok.Controls.AddRange(new Control[] { l1, cmbStokUrun, l2, txtStokMiktar, btnStokArtir, btnStokAzalt });
            sekmeStok.Controls.Add(pnlStok);

            Panel pnlSiparis = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.WhiteSmoke };
            Label l3 = new Label { Text = "Sipariş Edilecek Ürün:", Location = new Point(20, 30), AutoSize = true };
            cmbSiparisUrun = new ComboBox { Location = new Point(180, 28), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            Label l4 = new Label { Text = "Sipariş Adedi:", Location = new Point(450, 30), AutoSize = true };
            txtSiparisMiktar = new TextBox { Location = new Point(560, 28), Width = 100 };

            Button btnSiparisVer = new Button { Text = "SİPARİŞİ OLUŞTUR", Location = new Point(680, 25), Size = new Size(200, 35), BackColor = Color.Indigo, ForeColor = Color.White };
            btnSiparisVer.Click += (s, e) => YeniSiparisOlustur();

            dgvSiparisler = TabloOlustur();
            pnlSiparis.Controls.AddRange(new Control[] { l3, cmbSiparisUrun, l4, txtSiparisMiktar, btnSiparisVer });
            sekmeSiparis.Controls.Add(dgvSiparisler);
            sekmeSiparis.Controls.Add(pnlSiparis);

            sekmeler.TabPages.AddRange(new TabPage[] { sekmeUrun, sekmeStok, sekmeSiparis });
            this.Controls.Add(sekmeler);

            TablolariGuncelle();
        }

        private DataGridView TabloOlustur()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
        }

        private void TablolariGuncelle()
        {
            dgvUrunler.DataSource = null; dgvUrunler.DataSource = urunler.ToList();
            dgvSiparisler.DataSource = null; dgvSiparisler.DataSource = siparisler.ToList();

            cmbStokUrun.Items.Clear();
            cmbSiparisUrun.Items.Clear();

            foreach (var u in urunler)
            {
                cmbStokUrun.Items.Add(u);
                cmbSiparisUrun.Items.Add(u);
            }
        }

        private void UrunKaydet()
        {
            if (!string.IsNullOrWhiteSpace(txtUrunAd.Text) && double.TryParse(txtFiyat.Text, out double fiyatSistemi))
            {
                urunler.Add(new Urun
                {
                    urun_id = urunSayac++,
                    ad = txtUrunAd.Text,
                    stok = 0,
                    fiyat = fiyatSistemi
                });

                txtUrunAd.Clear(); txtFiyat.Clear();
                TablolariGuncelle();
                MessageBox.Show("Yeni ürün envantere başarıyla eklenmiştir.", "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Lütfen ürün adı ve sayısal bir birim fiyatı giriniz.", "Eksik Veri", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void StokIslemiYap(string islemTuru)
        {
            if (cmbStokUrun.SelectedItem is Urun seciliUrun && int.TryParse(txtStokMiktar.Text, out int miktar) && miktar > 0)
            {
                if (islemTuru == "Artir")
                {
                    seciliUrun.stok_arttir(miktar);
                    TablolariGuncelle();
                    MessageBox.Show("Stok artırma işlemi başarıyla gerçekleştirilmiştir.", "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (islemTuru == "Azalt")
                {
                    bool sonuc = seciliUrun.stok_azalt(miktar);
                    if (sonuc)
                    {
                        TablolariGuncelle();
                        MessageBox.Show("Stok azaltma işlemi başarıyla gerçekleştirilmiştir.", "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Belirtilen miktar mevcut stoktan fazla olamaz.", "Yetersiz Stok", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                txtStokMiktar.Clear();
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir ürün seçiniz ve pozitif bir miktar giriniz.", "Hatalı Veri", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void YeniSiparisOlustur()
        {
            if (cmbSiparisUrun.SelectedItem is Urun seciliUrun && int.TryParse(txtSiparisMiktar.Text, out int miktar) && miktar > 0)
            {
                Siparis islemMerkezi = new Siparis();
                Siparis yeniSiparis = islemMerkezi.siparis_olustur(seciliUrun, miktar, siparisSayac++);

                if (yeniSiparis != null)
                {
                    siparisler.Add(yeniSiparis);
                    TablolariGuncelle();
                    txtSiparisMiktar.Clear();
                    MessageBox.Show("Siparişiniz başarıyla oluşturulmuş ve ilgili ürünün stoku güncellenmiştir.", "Sipariş Onayı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Seçili ürün için yeterli stok bulunmamaktadır. İşlem iptal edildi.", "Yetersiz Stok", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen sipariş edilecek ürünü seçiniz ve geçerli bir adet giriniz.", "Hatalı Veri", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
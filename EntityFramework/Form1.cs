using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntityFramework
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        KuzeyYeliDataContext ctx = new KuzeyYeliDataContext();
        private void Form1_Load(object sender, EventArgs e)
        {
            GridiLinqSorgusuileDoldur();

            cmbkategori.DisplayMember = "KategoriAdi";
            cmbkategori.ValueMember = "KategoriID";
            cmbkategori.DataSource = ctx.Kategorilers;

            cmbtedarikci.DisplayMember = "SirketAdi";
            cmbtedarikci.ValueMember = "TedarikciID";
            cmbtedarikci.DataSource = ctx.Tedarikcilers;




        }

        private void GridiLinqSorgusuileDoldur()
        {
            var sonuc = from u in ctx.Urunlers
                        join k in ctx.Kategorilers
                        on u.KategoriID equals k.KategoriID
                        join t in ctx.Tedarikcilers
                        on u.TedarikciID equals t.TedarikciID
                        where u.Sonlandi == false
                        select new
                        {
                           ÜrünID= u.UrunID,
                           ÜrünAdi= u.UrunAdi,
                           ÜrünFiyat=u.Fiyat,
                           ÜrünStok= u.Stok,
                            u.KategoriID,
                            u.TedarikciID,
                            k.KategoriAdi,
                            t.SirketAdi
                        };
            var sonuc2 = from u in ctx.Urunlers
                         join sd in ctx.SatisDetays
                         on u.UrunID equals sd.UrunID
                         join s in ctx.Satislars
                         on sd.SatisID equals s.SatisID
                         join m in ctx.Musterilers
                         on s.MusteriID equals m.MusteriID
                         group sd by u.UrunAdi into grup
                         orderby grup.Key
                         // orderby u.UrunAdi
                         select new
                         {
                             grup.Key,
                             satis = grup.Count(),
                             SatisTutar=grup.Sum(x=>x.Adet*x.Fiyat)
                             //u.UrunAdi,
                             //sd.Fiyat,
                             //sd.Adet,
                             //m.MusteriAdi,
                             //u.Stok,
                             //u.TedarikciID,
                             //u.KategoriID,
                             //u.UrunID
                         };

            var sonuc3 = ctx.Urunlers.Join(ctx.Kategorilers,u=>u.KategoriID,k=>k.KategoriID,(u,k)=>
           new { u.UrunAdi,k.KategoriAdi}).OrderBy(x=>x.UrunAdi).Take(10);
            dataGridView1.DataSource = sonuc2;
        }
        private void GridiLamdaKullanarakDoldur()
        {
            dataGridView1.DataSource = ctx.Urunlers;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns["TedarikciID"].Visible = false;
            dataGridView1.Columns["KategoriID"].Visible = false;
            dataGridView1.Columns["Kategoriler"].Visible = false;
            dataGridView1.Columns["Tedarikciler"].Visible = false;
        }

        private void btn_ekle_Click(object sender, EventArgs e)
        {
            Urunler u = new Urunler();
            u.UrunAdi = txturunAdi.Text;
            u.Fiyat = numfiyat.Value;
            u.Stok = (short)numstok.Value;
            u.TedarikciID = (int)cmbtedarikci.SelectedValue;
            u.KategoriID = (int)cmbtedarikci.SelectedValue;
            ctx.Urunlers.InsertOnSubmit(u);
            ctx.SubmitChanges();
            ctx = new KuzeyYeliDataContext();
            dataGridView1.DataSource = ctx.Urunlers;

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
          
           
            txturunAdi.Text = dataGridView1.CurrentRow.Cells["UrunAdi"].Value.ToString();
            numfiyat.Value = (decimal)dataGridView1.CurrentRow.Cells["Fiyat"].Value;
            numstok.Value = (short)dataGridView1.CurrentRow.Cells["Stok"].Value;
            if (dataGridView1.CurrentRow.Cells["KategoriID"] != null)
                cmbkategori.SelectedValue = dataGridView1.CurrentRow.Cells["KategoriID"];
            if (dataGridView1.CurrentRow.Cells["TedarikciID"] != null)
                cmbtedarikci.SelectedValue = dataGridView1.CurrentRow.Cells["TedarikciID"];
            txturunAdi.Tag = dataGridView1.CurrentRow.Cells["UrunID"].Value;
            //ctx = new KuzeyYeliDataContext();
        }

        private void btn_guncelle_Click(object sender, EventArgs e)
        {
            int id = (int)txturunAdi.Tag;
            Urunler u = ctx.Urunlers.SingleOrDefault(x=>x.UrunID==id);
            u.UrunAdi = txturunAdi.Text;
            u.Fiyat = numfiyat.Value;
            u.Stok = (short)numstok.Value;
            u.TedarikciID = (int)cmbtedarikci.SelectedValue;
            u.KategoriID = (int)cmbkategori.SelectedValue;
            ctx.SubmitChanges();
           
        }

        private void btn_sil_Click(object sender, EventArgs e)
        {
            int id = (int)txturunAdi.Tag;
            Urunler u = ctx.Urunlers.SingleOrDefault(x => x.UrunID == id);
            ctx.Urunlers.DeleteOnSubmit(u);
            ctx.SubmitChanges();
            ctx = new KuzeyYeliDataContext();
            dataGridView1.DataSource = ctx.Urunlers.Where(x=>x.Sonlandi==false);

        }

        private void txtara_TextChanged(object sender, EventArgs e)
        {
            ctx = new KuzeyYeliDataContext();
            dataGridView1.DataSource = 
            ctx.Urunlers.Where(x => x.UrunAdi.Contains(txtara.Text) && x.Sonlandi==false);

        }
    }
}

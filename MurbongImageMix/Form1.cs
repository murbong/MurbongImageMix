using MaterialSkin.Controls;
using MaterialSkin;
using System;
using System.Drawing;
using System.Windows.Forms;
using MurbongImageMix.lib;
using System.Diagnostics;
using Palc.Imaging;

namespace MurbongImageMix
{

    public partial class Form1 : MaterialForm
    {
        public static int value;

        public static void SetProgress(int progress)
        {
            value = progress;
        }
        public Form1()
        {
            InitializeComponent();
            Debug.WriteLine("HI");
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            MaterialProgressBar materialProgressBar1 = new MaterialProgressBar();

            materialSkinManager.ColorScheme = new ColorScheme(
        Primary.Blue400, Primary.Blue500,
        Primary.Blue500, Accent.LightBlue200,
        TextShade.BLACK
    );
        }

        private Bitmap OpenBitMap()
        {
            Bitmap img;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "불러오기";
            openFileDialog.InitialDirectory = Application.StartupPath;
            openFileDialog.DefaultExt = "png";
            openFileDialog.Filter = "png(*.png)|*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                img = new Bitmap(path);
            }
            else
            {
                img = null;
            }

            return img;
        }

        private String SaveFilePath()
        {
            string path;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "저장하기";
            saveFileDialog.InitialDirectory = Application.StartupPath;
            saveFileDialog.DefaultExt = "png";
            saveFileDialog.Filter = "png(*.png)|*.png";

            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = saveFileDialog.FileName;
            }
            else
            {
                path = "error.png";
            }
            return path;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {

            try
            {
                Bitmap img1 = OpenBitMap();
                pictureBox1.Image = img1;
                Bitmap img2 = OpenBitMap();


                if (img1.Size == img2.Size)
                {


                    img1 = Util.GammaCorrection(img1, 0.023,1);
                    img2 = Util.GammaCorrection(img2, 1, 0.8);



                    Bitmap out1 = Util.MergeBitmap(img1, img2);
                    pictureBox1.Image = out1;





                    string path = SaveFilePath();

                    out1.Save(path);

                    Png png = new Png(path);

                    png.RemoveChunk(Png.ChunkType.RgbColorSpace);
                    png.SetGammaChunk();

                    png.Save(path);
                }
                else
                {
                    MessageBox.Show("두 이미지는 사이즈가 같아야합니다.");
                }
            }
            catch
            {
                MessageBox.Show("잘못된 접근입니다.");

            }

        }
    }
}

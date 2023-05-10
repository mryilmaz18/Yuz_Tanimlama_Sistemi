using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Face;
using Emgu.CV.CvEnum;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Yuz_Tanımlama
{
    public partial class Form1 : Form
    {
        #region Degiskenler
        int testid = 0;
        private Capture yakala = null;
        private Image<Bgr, Byte> mevcutyuz = null;
        Mat cerceve = new Mat();
        private bool yuzualgıla = false;
        CascadeClassifier YuzCascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt.xml");
        Image<Bgr, Byte> yuzsonuc = null;
        List<Image<Gray, Byte>> denemeyuz = new List<Image<Gray, byte>>();
        List<int> kisiler = new List<int>();

        bool yuzkaydedici = false;
        private bool durum = false;
        EigenFaceRecognizer recognizer;
        List<string> kisiad = new List<string>();

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_kamera_Click(object sender, EventArgs e)
        {
            //KAMERAYI AÇMA
            if (yakala != null) yakala.Dispose();
            yakala = new Capture();
            Application.Idle += Yakala_Cerceve;
        }

        private void Yakala_Cerceve(object sender, EventArgs e)
        {
            //VIDEO YAKALAMA
            if (yakala != null && yakala.Ptr != IntPtr.Zero)
            {
                yakala.Retrieve(cerceve, 0);
                mevcutyuz = cerceve.ToImage<Bgr, Byte>().Resize(pictureKamera.Width, pictureKamera.Height, Inter.Cubic);

                //YUZ TANIMA
                if (yuzualgıla)
                {

                    //Arka Plandan Gri Görüntüye Dönüştür
                    Mat Gribox = new Mat();
                    CvInvoke.CvtColor(mevcutyuz, Gribox, ColorConversion.Bgr2Gray);
                    //Daha iyi sonuç almak için görüntüyü geliştirin
                    CvInvoke.EqualizeHist(Gribox, Gribox);

                    Rectangle[] yuzler = YuzCascadeClassifier.DetectMultiScale(Gribox, 1.1, 3, Size.Empty, Size.Empty);
                    //Yüzler algılanırsa
                    if (yuzler.Length > 0)


                        foreach (var yuz in yuzler)
                        {
                            // Her yüzün çevresine kare çizin
                            CvInvoke.Rectangle(mevcutyuz, yuz, new Bgr(Color.Red).MCvScalar, 2);

                            //KISI EKLEME
                            //Yüzü pictureYuz'e atayın 
                            Image<Bgr, Byte> resultImage = mevcutyuz.Convert<Bgr, Byte>();
                            resultImage.ROI = yuz;
                            pictureYuz.SizeMode = PictureBoxSizeMode.StretchImage;
                            pictureYuz.Image = resultImage.Bitmap;

                            if (yuzkaydedici)
                            {
                                //Mevcut değilse bir dizin oluşturacağız!
                                string path = Directory.GetCurrentDirectory() + @"\YakalananResimler";
                                if (!Directory.Exists(path))
                                    Directory.CreateDirectory(path);
                                //her görüntü için saniye gecikmeli 10 görüntü kaydedeceğiz 
                                //askıda kalma GUI'sinden kaçınmak için yeni bir görev oluşturacağız
                                Task.Factory.StartNew(() =>
                                {
                                    for (int i = 0; i < 10; i++)
                                    {
                                        //görüntüyü yeniden boyutlandırın ve kaydedin
                                        resultImage.Resize(200, 200, Inter.Cubic).Save(path + @"\" + maskedTC.Text + "_" + DateTime.Now.ToString("dd-mm-yyyy-hh-mm-ss") + ".jpg");
                                        Thread.Sleep(1000);
                                    }
                                });

                            }
                            yuzkaydedici = false;

                            if (btnEkle.InvokeRequired)
                            {
                                btnEkle.Invoke(new ThreadStart(delegate
                                {
                                    btnEkle.Enabled = true;
                                }));
                            }

                            //YUZU TANIMAK
                            if (durum)
                            {
                                Image<Gray, Byte> grayFaceResult = resultImage.Convert<Gray, Byte>().Resize(100, 100, Inter.Cubic);
                                CvInvoke.EqualizeHist(grayFaceResult, grayFaceResult);
                                var result = recognizer.Predict(grayFaceResult);
                                pictureBox1.Image = grayFaceResult.Bitmap;
                                pictureBox2.Image = denemeyuz[result.Label].Bitmap;
                                Debug.WriteLine(result.Label + ". " + result.Distance);
                                //Sonuçlar bilinen yüzleri buldu
                                if (result.Label != -1 && result.Distance < 2000)
                                {
                                    CvInvoke.PutText(mevcutyuz, kisiad[result.Label], new Point(yuz.X - 2, yuz.Y - 2),
                                        FontFace.HersheyComplex, 1.0, new Bgr(Color.Blue).MCvScalar);
                                    CvInvoke.Rectangle(mevcutyuz, yuz, new Bgr(Color.Green).MCvScalar, 2);
                                }
                                //burada sonuçlar bilinen herhangi bir yüz bulamadı
                                else
                                {
                                    CvInvoke.PutText(mevcutyuz, "TANINAMADI", new Point(yuz.X - 2, yuz.Y - 2),
                                        FontFace.HersheyComplex, 1.0, new Bgr(Color.Orange).MCvScalar);
                                    CvInvoke.Rectangle(mevcutyuz, yuz, new Bgr(Color.Red).MCvScalar, 2);

                                }
                            }
                        }
                }
            }
            //Yakalanan videoyu Picture Box picKamera içine işleyin
            pictureKamera.Image = mevcutyuz.Bitmap;

        }

        private void btn_Algıla_Click(object sender, EventArgs e)
        {
            yuzualgıla = true;
        }

        bool durum2;
        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maskedTC.Text.Trim()))
            {
                durum2 = false;
                yuzkaydedici = false;
                MessageBox.Show("Hata: Lütfen Kişi TC Giriniz!");
                return;
            }
            else
            {
                btnEkle.Enabled = false;
                yuzkaydedici = true;
                MessageBox.Show("3 Saniye Boyunca Kafanızı Sağ-Sol Yönünde Hareket Ettiriniz");
                return;

            }

        }

        private void btn_goruntu_Click(object sender, EventArgs e)
        {
            DizinGoruntuleri();
        }
        //Önceki örnekten kaydedilen görüntüleri kullanacağız
        private bool DizinGoruntuleri()
        {
            int ImagesCount = 0;
            double Threshold = 2000;
            denemeyuz.Clear();
            kisiler.Clear();
            kisiad.Clear();
            try
            {
                string path = Directory.GetCurrentDirectory() + @"\YakalananResimler";
                string[] files = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    Image<Gray, byte> trainedImage = new Image<Gray, byte>(file).Resize(100, 100, Inter.Cubic);
                    CvInvoke.EqualizeHist(trainedImage, trainedImage);
                    denemeyuz.Add(trainedImage);
                    kisiler.Add(ImagesCount);
                    string name = file.Split('\\').Last().Split('_')[0];
                    kisiad.Add(name);
                    ImagesCount++;
                    Debug.WriteLine(ImagesCount + ". " + name);

                }

                if (denemeyuz.Count() > 0)
                {
                    
                    recognizer = new EigenFaceRecognizer(ImagesCount, Threshold);
                    recognizer.Train(denemeyuz.ToArray(), kisiler.ToArray());
                    durum = true;
                    return true;
                }
                else
                {
                    durum = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                durum = false;
                MessageBox.Show("Yüz İçin Hatalı Deneme " + ex.Message);
                return false;
            }
        }

        private void btnkaydet_Click(object sender, EventArgs e)
        {
            //uygulamayı daha hızlı reboot etmek için
            Application.Exit();
            // Uygulama hemen sonlandırılıyor.
            System.Diagnostics.Process.Start(Application.ExecutablePath);
            // İşletim sistemi tarafından uygulama tekrar başlatılıyor.
        }
    }
}

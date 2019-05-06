using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace ControleDePonto
{
    public partial class CaptureDevice : Form
    {
        private VideoCaptureDevice videoSource;
        public CaptureDevice()
        {
            InitializeComponent();

            // Capturando Array com todas as câmeras disponíveis
            FilterInfoCollection videoSources = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoSources != null && videoSources.Count > 0)
            {
                // Capturando primeira câmera
                // MonikerString é algo semelhante ao ID da câmera
                videoSource = new VideoCaptureDevice(videoSources[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Fazer dispose para evitar o consumo de memória
            if (pbCamera.Image != null)
            {
                pbCamera.Image.Dispose();
            }
            
            // Capturar Frame da câmera e exibir no pitcure box
            pbCamera.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void BtnCapturar_Click(object sender, EventArgs e)
        {
            if (pbCamera.Image != null)
            {
                try
                {
                    // Parar o frame no momento que o click aconteceu
                    videoSource.NewFrame -= VideoSource_NewFrame;

                    // Abrindo dialog para escolher onde salvar
                    using (var dialog = new SaveFileDialog())
                    {
                        dialog.DefaultExt = "png";
                        dialog.AddExtension = true; // Adicionando extension automaticamente

                        // Se houve click em OK...
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            pbCamera.Image.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                }
                finally // Independente do que acontecer execute isso
                {
                    videoSource.NewFrame += VideoSource_NewFrame;
                }
            }
        }

        // Parar câmera quando o formulário fechar 
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (videoSource.IsRunning)
            {
                videoSource.Stop();
            }
            base.OnFormClosing(e);
        }
        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void CaptureDevice_Load(object sender, EventArgs e)
        {

        }
    }
}

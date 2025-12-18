using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace JuegoDeAvion.CrearAvion
{
    public class CrearAvion : Form
    {
        private PictureBox picOriginal;
        private PictureBox picProcesada;
        private Button btnCargarImagen;
        private Button btnGuardarAvion;
        private TrackBar trackBarUmbral;
        private Label lblUmbral;
        private ColorDialog colorDialog;
        private Button btnSeleccionarColor;
        private Color colorSeleccionado = Color.Red;
        private Bitmap imagenOriginal;
        private Bitmap imagenProcesada;
        private string rutaImagenOriginal;

        public CrearAvion()
        {
            InicializarComponentes();
            this.Text = "Crear Nuevo Avión";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void InicializarComponentes()
        {
            // Panel principal
            TableLayoutPanel panelPrincipal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(10)
            };
            panelPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panelPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panelPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            panelPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            panelPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));

            // PictureBox para la imagen original
            picOriginal = new PictureBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black
            };

            // PictureBox para la imagen procesada
            picProcesada = new PictureBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black
            };

            // Panel de controles
            Panel panelControles = new Panel { Dock = DockStyle.Fill };
            FlowLayoutPanel flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            // Botón para cargar imagen
            btnCargarImagen = new Button
            {
                Text = "Cargar Imagen",
                AutoSize = true,
                Margin = new Padding(5)
            };
            btnCargarImagen.Click += BtnCargarImagen_Click;

            // Selector de color
            btnSeleccionarColor = new Button
            {
                Text = "Color del Avión",
                BackColor = colorSeleccionado,
                AutoSize = true,
                Margin = new Padding(5)
            };
            btnSeleccionarColor.Click += BtnSeleccionarColor_Click;
            colorDialog = new ColorDialog { Color = colorSeleccionado };

            // TrackBar para el umbral
            lblUmbral = new Label
            {
                Text = "Umbral: 128",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(5, 15, 5, 5)
            };

            trackBarUmbral = new TrackBar
            {
                Minimum = 1,
                Maximum = 255,
                Value = 128,
                Width = 200,
                TickFrequency = 10,
                Margin = new Padding(5)
            };
            trackBarUmbral.Scroll += TrackBarUmbral_Scroll;

            // Botón para guardar el avión
            btnGuardarAvion = new Button
            {
                Text = "Guardar Avión",
                AutoSize = true,
                Margin = new Padding(5),
                Enabled = false
            };
            btnGuardarAvion.Click += BtnGuardarAvion_Click;

            // Agregar controles al panel de flujo
            flowPanel.Controls.Add(btnCargarImagen);
            flowPanel.Controls.Add(btnSeleccionarColor);
            flowPanel.Controls.Add(lblUmbral);
            flowPanel.Controls.Add(trackBarUmbral);
            flowPanel.Controls.Add(btnGuardarAvion);

            panelControles.Controls.Add(flowPanel);

            // Agregar controles al panel principal
            panelPrincipal.Controls.Add(new Panel { Dock = DockStyle.Fill, Controls = { new Label { Text = "Imagen Original", Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.White } } }, 0, 0);
            panelPrincipal.Controls.Add(picOriginal, 0, 0);
            panelPrincipal.Controls.Add(new Panel { Dock = DockStyle.Fill, Controls = { new Label { Text = "Vista Previa", Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.White } } }, 1, 0);
            panelPrincipal.Controls.Add(picProcesada, 1, 0);
            panelPrincipal.Controls.Add(panelControles, 0, 1);
            panelPrincipal.SetColumnSpan(panelControles, 2);

            this.Controls.Add(panelPrincipal);
        }

        private void BtnCargarImagen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp|Todos los archivos|*.*";
                openFileDialog.Title = "Seleccionar imagen del avión";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        rutaImagenOriginal = openFileDialog.FileName;
                        imagenOriginal = new Bitmap(Image.FromFile(rutaImagenOriginal));
                        picOriginal.Image = imagenOriginal;
                        ProcesarImagen();
                        btnGuardarAvion.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al cargar la imagen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnSeleccionarColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                colorSeleccionado = colorDialog.Color;
                btnSeleccionarColor.BackColor = colorSeleccionado;
                if (imagenOriginal != null)
                {
                    ProcesarImagen();
                }
            }
        }

        private void TrackBarUmbral_Scroll(object sender, EventArgs e)
        {
            lblUmbral.Text = $"Umbral: {trackBarUmbral.Value}";
            if (imagenOriginal != null)
            {
                ProcesarImagen();
            }
        }

        private void ProcesarImagen()
        {
            if (imagenOriginal == null) return;

            try
            {
                // Crear una copia de la imagen original para procesar
                Bitmap bmp = new Bitmap(imagenOriginal);
                int ancho = bmp.Width;
                int alto = bmp.Height;

                // Crear una nueva imagen para el resultado
                imagenProcesada = new Bitmap(ancho, alto, PixelFormat.Format32bppArgb);

                // Obtener el umbral actual
                int umbral = trackBarUmbral.Value;

                // Crear un objeto Graphics para dibujar en la imagen de destino
                using (Graphics g = Graphics.FromImage(imagenProcesada))
                {
                    // Hacer que el fondo sea transparente
                    g.Clear(Color.Transparent);

                    // Crear una textura con el patrón de ajedrez para el fondo transparente
                    using (Brush checkerBrush = CrearPincelAjedrez(10, Color.Gray, Color.DimGray))
                    {
                        g.FillRectangle(checkerBrush, 0, 0, ancho, alto);
                    }

                    // Crear una máscara de transparencia basada en el umbral
                    for (int y = 0; y < alto; y++)
                    {
                        for (int x = 0; x < ancho; x++)
                        {
                            Color pixel = bmp.GetPixel(x, y);
                            // Calcular el brillo del píxel
                            int brillo = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
                            
                            // Si el píxel es más oscuro que el umbral, lo consideramos parte del avión
                            if (brillo < umbral && pixel.A > 0)
                            {
                                // Aplicar el color seleccionado al píxel
                                imagenProcesada.SetPixel(x, y, Color.FromArgb(pixel.A, colorSeleccionado));
                            }
                        }
                    }

                    // Aplicar un suavizado a los bordes
                    SuavizarBordes(imagenProcesada, 1);
                }

                // Mostrar la imagen procesada
                picProcesada.Image = imagenProcesada;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar la imagen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Brush CrearPincelAjedrez(int tamanoCelda, Color color1, Color color2)
        {
            Bitmap bmp = new Bitmap(tamanoCelda * 2, tamanoCelda * 2);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                using (SolidBrush brush1 = new SolidBrush(color1))
                using (SolidBrush brush2 = new SolidBrush(color2))
                {
                    g.FillRectangle(brush1, 0, 0, tamanoCelda, tamanoCelda);
                    g.FillRectangle(brush2, tamanoCelda, 0, tamanoCelda, tamanoCelda);
                    g.FillRectangle(brush2, 0, tamanoCelda, tamanoCelda, tamanoCelda);
                    g.FillRectangle(brush1, tamanoCelda, tamanoCelda, tamanoCelda, tamanoCelda);
                }
            }
            TextureBrush textureBrush = new TextureBrush(bmp);
            textureBrush.WrapMode = WrapMode.Tile;
            return textureBrush;
        }

        private void SuavizarBordes(Bitmap bmp, int radioSuavizado)
        {
            // Este método aplica un desenfoque gaussiano simple a los bordes para suavizarlos
            if (radioSuavizado < 1) return;

            int ancho = bmp.Width;
            int alto = bmp.Height;

            // Crear una copia temporal para el procesamiento
            using (Bitmap tempBmp = new Bitmap(bmp))
            {
                for (int y = 0; y < alto; y++)
                {
                    for (int x = 0; x < ancho; x++)
                    {
                        Color pixel = tempBmp.GetPixel(x, y);
                        
                        // Solo procesar píxeles que no son completamente transparentes
                        if (pixel.A > 0)
                        {
                            int r = 0, g = 0, b = 0, a = 0, count = 0;
                            
                            // Promediar los píxeles vecinos
                            for (int dy = -radioSuavizado; dy <= radioSuavizado; dy++)
                            {
                                for (int dx = -radioSuavizado; dx <= radioSuavizado; dx++)
                                {
                                    int nx = x + dx;
                                    int ny = y + dy;
                                    
                                    if (nx >= 0 && nx < ancho && ny >= 0 && ny < alto)
                                    {
                                        Color neighbor = tempBmp.GetPixel(nx, ny);
                                        r += neighbor.R;
                                        g += neighbor.G;
                                        b += neighbor.B;
                                        a += neighbor.A;
                                        count++;
                                    }
                                }
                            }
                            
                            // Aplicar el promedio si hay píxeles para promediar
                            if (count > 0)
                            {
                                r /= count;
                                g /= count;
                                b /= count;
                                a = pixel.A; // Mantener la opacidad original
                                
                                // Aplicar el color promediado
                                bmp.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                            }
                        }
                    }
                }
            }
        }

        private void BtnGuardarAvion_Click(object sender, EventArgs e)
        {
            if (imagenProcesada == null) return;

            try
            {
                // Crear la carpeta Aviones si no existe
                string carpetaAviones = Path.Combine(Application.StartupPath, "Aviones");
                if (!Directory.Exists(carpetaAviones))
                {
                    Directory.CreateDirectory(carpetaAviones);
                }

                // Encontrar el siguiente número de avión disponible
                int numeroAvion = 1;
                string rutaImagen;
                do
                {
                    rutaImagen = Path.Combine(carpetaAviones, $"avion{numeroAvion}.png");
                    numeroAvion++;
                } while (File.Exists(rutaImagen) && numeroAvion <= 100);

                if (numeroAvion > 100)
                {
                    MessageBox.Show("Se ha alcanzado el límite de aviones (100). Por favor, elimine algunos aviones existentes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Guardar la imagen procesada
                imagenProcesada.Save(rutaImagen, ImageFormat.Png);

                // Guardar una copia de la imagen original en una subcarpeta "originales"
                string carpetaOriginales = Path.Combine(carpetaAviones, "originales");
                if (!Directory.Exists(carpetaOriginales))
                {
                    Directory.CreateDirectory(carpetaOriginales);
                }
                string nombreArchivo = Path.GetFileName(rutaImagen);
                string rutaOriginal = Path.Combine(carpetaOriginales, nombreArchivo);
                imagenOriginal.Save(rutaOriginal, ImageFormat.Png);

                MessageBox.Show($"¡Avión guardado correctamente como {Path.GetFileName(rutaImagen)}!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Actualizar la lista de aviones en el juego
                ActualizarListaAviones();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el avión: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarListaAviones()
        {
            // Este método se puede usar para notificar al juego principal que se ha añadido un nuevo avión
            // Por ahora, simplemente mostramos un mensaje
            // En una implementación real, podrías querer disparar un evento o actualizar la UI del juego
        }
    }
}

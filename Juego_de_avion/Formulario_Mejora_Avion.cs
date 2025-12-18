using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace JuegoDeAvion
{
    public class Formulario_Mejora_Avion : Form
    {
        private TipoAvion avionAMejorar;
        private Label lblPuntos;
        private int escalaVisualizada = 0;
        
        // Tamaños de visualización para cada escala
        private Size[] tamanosEscala = new Size[] { 
            new Size(50, 70),   // Escala 0
            new Size(75, 105),  // Escala 1 (ajustado)
            new Size(100, 140), // Escala 2 (ajustado)
            new Size(125, 175)  // Escala 3 (ajustado)
        };

        public Formulario_Mejora_Avion()
        {
            avionAMejorar = DatosGlobales.Aviones[DatosGlobales.IndiceAvionSeleccionado];
            escalaVisualizada = avionAMejorar.NivelEscala;
            lblPuntos = new Label();
            InitializeUserInterface();
        }

        private void InitializeUserInterface()
        {
            this.Controls.Clear();

            Text = "Astillero de Mejoras";
            this.Size = new Size(960, 540); // Mantenemos la resolución de celular
            this.MinimumSize = new Size(800, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;

            // Layout Principal
            TableLayoutPanel panelPrincipal = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 3, Padding = new Padding(10) };
            panelPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            panelPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            panelPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            panelPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // Título
            panelPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Puntos
            panelPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Nave y Stats
            panelPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F)); // Botón de acción
            this.Controls.Add(panelPrincipal);

            // Título
            Label lblTitulo = new Label { Text = $"NAVE: {avionAMejorar.Nombre.ToUpper()}", Font = new Font("Courier New", 24, FontStyle.Bold), ForeColor = Color.Lime, AutoSize = true, Anchor = AnchorStyles.None };
            panelPrincipal.Controls.Add(lblTitulo, 1, 0);

            // Puntos
            lblPuntos.Font = new Font("Courier New", 18, FontStyle.Bold);
            lblPuntos.ForeColor = Color.White;
            lblPuntos.AutoSize = true;
            lblPuntos.Anchor = AnchorStyles.None;
            ActualizarTextoPuntos();
            panelPrincipal.Controls.Add(lblPuntos, 1, 1);

            // Botones de Navegación
            Button btnPrev = CrearBoton("<", new Size(50, 50), new Font("Courier New", 20, FontStyle.Bold));
            Button btnNext = CrearBoton(">", new Size(50, 50), new Font("Courier New", 20, FontStyle.Bold));
            btnPrev.Click += (s, e) => CambiarEscala(-1);
            btnNext.Click += (s, e) => CambiarEscala(1);
            panelPrincipal.Controls.Add(btnPrev, 0, 2);
            panelPrincipal.Controls.Add(btnNext, 2, 2);

            // Panel Central (Contiene la info de la escala y la imagen)
            TableLayoutPanel panelContenidoCentral = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
            panelContenidoCentral.RowStyles.Add(new RowStyle(SizeType.Percent, 70F)); // PictureBox
            panelContenidoCentral.RowStyles.Add(new RowStyle(SizeType.Percent, 30F)); // Stats
            panelPrincipal.Controls.Add(panelContenidoCentral, 1, 2);

            // PictureBox para la nave
            PictureBox picNave = new PictureBox {
                Size = tamanosEscala[escalaVisualizada],
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(20, 20, 20),
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.None // Para centrarlo en la celda
            };
            picNave.Paint += PicNave_Paint;
            panelContenidoCentral.Controls.Add(picNave, 0, 0);

            // Label de Escala e Stats
            Label lblEscalaInfo = new Label { 
                Text = $"ESCALA {escalaVisualizada}\nVELOCIDAD: {avionAMejorar.Velocidad + (escalaVisualizada * 2)}\nVIDA: {avionAMejorar.VidaMaxima + escalaVisualizada}", 
                Font = new Font("Courier New", 16, FontStyle.Bold), 
                ForeColor = Color.Cyan, 
                AutoSize = true, 
                Anchor = AnchorStyles.None,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelContenidoCentral.Controls.Add(lblEscalaInfo, 0, 1);

            // Botón de Acción (Mejorar/Estado)
            Button btnAccion = CrearBoton("", new Size(300, 50), new Font("Courier New", 16, FontStyle.Bold));
            ConfigurarBotonAccion(btnAccion);
            panelPrincipal.Controls.Add(btnAccion, 1, 3);

            // Botón Volver
            Button btnVolver = CrearBoton("VOLVER", new Size(100, 40), new Font("Courier New", 12, FontStyle.Bold));
            btnVolver.Click += (s, e) => this.Close();
            panelPrincipal.Controls.Add(btnVolver, 2, 0); 
        }

        private void CambiarEscala(int direccion)
        {
            escalaVisualizada += direccion;
            if (escalaVisualizada < 0) escalaVisualizada = 0;
            if (escalaVisualizada > 3) escalaVisualizada = 3;
            InitializeUserInterface();
        }

        private void ConfigurarBotonAccion(Button btn)
        {
            if (escalaVisualizada <= avionAMejorar.NivelEscala)
            {
                btn.Text = "ADQUIRIDA";
                btn.Enabled = false;
                btn.ForeColor = Color.Lime;
                btn.FlatAppearance.BorderColor = Color.Lime;
            }
            else if (escalaVisualizada == avionAMejorar.NivelEscala + 1)
            {
                int costo = 1000 * (escalaVisualizada + 1);
                btn.Text = $"MEJORAR ({costo} P)";
                btn.Enabled = DatosGlobales.PuntosTotales >= costo;
                btn.ForeColor = Color.White;
                btn.Click += (s, e) => ComprarMejora(costo);
            }
            else
            {
                btn.Text = "BLOQUEADO";
                btn.Enabled = false;
                btn.ForeColor = Color.Gray;
                btn.FlatAppearance.BorderColor = Color.Gray;
            }
        }

        private void ComprarMejora(int costo)
        {
            if (DatosGlobales.PuntosTotales >= costo)
            {
                DatosGlobales.PuntosTotales -= costo;
                avionAMejorar.NivelEscala = escalaVisualizada;
                MessageBox.Show($"¡Nave mejorada a Escala {escalaVisualizada}!");
                InitializeUserInterface();
            }
        }

        private void PicNave_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not PictureBox pic) return;

            Point[] rawPoints = Avion.GetShipPoints(avionAMejorar.TipoPoligono);
            if (rawPoints == null || rawPoints.Length < 3) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.Clear(pic.BackColor);

            int minX = rawPoints.Min(p => p.X); int maxX = rawPoints.Max(p => p.X);
            int minY = rawPoints.Min(p => p.Y); int maxY = rawPoints.Max(p => p.Y);
            float anchoOriginal = Math.Max(1, maxX - minX); float altoOriginal = Math.Max(1, maxY - minY);
            
            float ratioX = (float)(pic.Width - 10) / anchoOriginal;
            float ratioY = (float)(pic.Height - 10) / altoOriginal;

            PointF[] scaledPoints = rawPoints.Select(p => new PointF(5 + (p.X - minX) * ratioX, 5 + (p.Y - minY) * ratioY)).ToArray();
            
            Color color = (escalaVisualizada <= avionAMejorar.NivelEscala) ? avionAMejorar.ColorPrincipal : Color.Gray;

            using (SolidBrush b = new SolidBrush(color))
            using (Pen p = new Pen(Color.White, 2))
            {
                g.FillPolygon(b, scaledPoints);
                g.DrawPolygon(p, scaledPoints);
            }
        }

        private void ActualizarTextoPuntos()
        {
            lblPuntos.Text = $"PUNTOS: {DatosGlobales.PuntosTotales}";
        }

        private Button CrearBoton(string texto, Size tamano, Font fuente)
        {
            Button btn = new Button { Text = texto, Size = tamano, Font = fuente, Anchor = AnchorStyles.None };
            btn.BackColor = Color.Black; btn.ForeColor = Color.White; btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.Lime; btn.FlatAppearance.BorderSize = 2;
            btn.MouseEnter += (s, e) => { if(btn.Enabled) { btn.BackColor = Color.Lime; btn.ForeColor = Color.Black; } };
            btn.MouseLeave += (s, e) => { if(btn.Enabled) { btn.BackColor = Color.Black; btn.ForeColor = Color.White; } };
            return btn;
        }
    }
}
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace JuegoDeAvion
{
    public class Formulario_Seleccion_Avion : Form
    {
        private int indiceActual;
        private Label lblNombreAvion;
        private Label lblStats;
        private PictureBox picAvion;
        private Button btnSeleccionar;

        public Formulario_Seleccion_Avion()
        {
            Text = "Hangar de Naves";
            this.Size = new Size(1280, 720);
            this.MinimumSize = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;

            indiceActual = DatosGlobales.IndiceAvionSeleccionado;

            // Panel principal para centrar todo
            TableLayoutPanel panelCentral = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 3 };
            panelCentral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            panelCentral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panelCentral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            panelCentral.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            panelCentral.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            panelCentral.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            this.Controls.Add(panelCentral);

            // Controles
            lblNombreAvion = new Label { Font = new Font("Courier New", 40, FontStyle.Bold), ForeColor = Color.Lime, AutoSize = true, Anchor = AnchorStyles.None };
            picAvion = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.CenterImage };
            picAvion.Paint += PicAvion_Paint;
            lblStats = new Label { Font = new Font("Courier New", 28), ForeColor = Color.White, AutoSize = true, Anchor = AnchorStyles.Top, TextAlign = ContentAlignment.TopCenter };
            
            Button btnAnterior = CrearBoton("<", new Size(100, 100), new Font("Courier New", 36, FontStyle.Bold));
            Button btnSiguiente = CrearBoton(">", new Size(100, 100), new Font("Courier New", 36, FontStyle.Bold));
            btnSeleccionar = CrearBoton("SELECCIONAR", new Size(600, 90), new Font("Courier New", 24, FontStyle.Bold));
            
            // Añadir controles al panel
            panelCentral.Controls.Add(lblNombreAvion, 1, 0);
            panelCentral.Controls.Add(btnAnterior, 0, 1);
            panelCentral.Controls.Add(picAvion, 1, 1);
            panelCentral.Controls.Add(btnSiguiente, 2, 1);
            
            // Panel para los stats y el botón de seleccionar
            FlowLayoutPanel panelInferior = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
            panelInferior.Controls.Add(lblStats);
            panelInferior.Controls.Add(btnSeleccionar);
            panelCentral.Controls.Add(panelInferior, 1, 2);

            // Eventos
            btnAnterior.Click += (s, e) => Navegar(-1);
            btnSiguiente.Click += (s, e) => Navegar(1);
            btnSeleccionar.Click += SeleccionarOComprar;

            MostrarAvion();
        }

        private void PicAvion_Paint(object? sender, PaintEventArgs e)
        {
            // (El código de dibujo del polígono es el mismo, no necesita cambios)
            TipoAvion avion = DatosGlobales.Aviones[indiceActual];
            Point[] rawPoints = DrawHelper.GetShipPoints(avion.TipoPoligono);
            if (rawPoints == null || rawPoints.Length < 3) return;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.Clear(this.BackColor);
            int minX = rawPoints.Min(p => p.X); int maxX = rawPoints.Max(p => p.X);
            int minY = rawPoints.Min(p => p.Y); int maxY = rawPoints.Max(p => p.Y);
            float anchoOriginal = Math.Max(1, maxX - minX); float altoOriginal = Math.Max(1, maxY - minY);
            float ratio = Math.Min((picAvion.Width - 40) / anchoOriginal, (picAvion.Height - 40) / altoOriginal);
            PointF[] scaledPoints = rawPoints.Select(p => new PointF(20 + (p.X - minX) * ratio, 20 + (p.Y - minY) * ratio)).ToArray();
            using (SolidBrush b = new SolidBrush(avion.ColorPrincipal))
            using (Pen p = new Pen(Color.White, 5))
            {
                g.FillPolygon(b, scaledPoints);
                g.DrawPolygon(p, scaledPoints);
            }
        }

        private void Navegar(int direccion)
        {
            indiceActual = (indiceActual + direccion + DatosGlobales.Aviones.Count) % DatosGlobales.Aviones.Count;
            MostrarAvion();
        }

        private void MostrarAvion()
        {
            TipoAvion avion = DatosGlobales.Aviones[indiceActual];
            lblNombreAvion.Text = avion.Nombre.ToUpper();
            lblStats.Text = $"VELOCIDAD: {avion.Velocidad}\n     VIDA: {avion.VidaMaxima}";
            picAvion.Refresh();

            if (avion.Desbloqueado)
            {
                btnSeleccionar.Text = "SELECCIONAR";
                btnSeleccionar.Enabled = true;
            }
            else
            {
                btnSeleccionar.Text = $"COMPRAR ({avion.Precio} PUNTOS)";
                btnSeleccionar.Enabled = DatosGlobales.PuntosTotales >= avion.Precio;
            }
        }

        private void SeleccionarOComprar(object? sender, EventArgs e)
        {
            TipoAvion avion = DatosGlobales.Aviones[indiceActual];
            if (avion.Desbloqueado)
            {
                DatosGlobales.IndiceAvionSeleccionado = indiceActual;
                MessageBox.Show($"{avion.Nombre} seleccionado!");
                this.Close();
            }
            else if (DatosGlobales.PuntosTotales >= avion.Precio)
            {
                DatosGlobales.PuntosTotales -= avion.Precio;
                avion.Desbloqueado = true;
                MessageBox.Show($"¡{avion.Nombre} comprado!");
                MostrarAvion();
            }
        }
        
        private Button CrearBoton(string texto, Size tamano, Font fuente)
        {
            Button btn = new Button { Text = texto, Size = tamano, Font = fuente, Anchor = AnchorStyles.None };
            btn.BackColor = Color.Black; btn.ForeColor = Color.White; btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.Lime; btn.FlatAppearance.BorderSize = 3;
            btn.MouseEnter += (s, e) => { btn.BackColor = Color.Lime; btn.ForeColor = Color.Black; };
            btn.MouseLeave += (s, e) => { btn.BackColor = Color.Black; btn.ForeColor = Color.White; };
            return btn;
        }
    }
}
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

        public Formulario_Mejora_Avion()
        {
            InitializeUserInterface();
        }

        private void InitializeUserInterface()
        {
            this.Controls.Clear();

            Text = "Astillero de Mejoras";
            this.Size = new Size(1280, 720);
            this.MinimumSize = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;

            avionAMejorar = DatosGlobales.Aviones[DatosGlobales.IndiceAvionSeleccionado];

            // Panel principal
            TableLayoutPanel panelPrincipal = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4 };
            panelPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            panelPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            panelPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            panelPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            this.Controls.Add(panelPrincipal);

            // Título y Puntos
            Label lblTitulo = new Label { Text = $"NAVE: {avionAMejorar.Nombre.ToUpper()}", Font = new Font("Courier New", 40, FontStyle.Bold), ForeColor = Color.Lime, AutoSize = true, Anchor = AnchorStyles.None };
            lblPuntos = new Label { Font = new Font("Courier New", 28, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Anchor = AnchorStyles.None };
            panelPrincipal.Controls.Add(lblTitulo, 0, 0);
            panelPrincipal.Controls.Add(lblPuntos, 0, 1);

            // Panel para las tarjetas
            FlowLayoutPanel panelTarjetas = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, AutoSize = true, Anchor = AnchorStyles.None };
            panelPrincipal.Controls.Add(panelTarjetas, 0, 2);

            for (int i = 0; i < 4; i++)
            {
                Panel card = new Panel { Size = new Size(220, 300), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(20, 20, 20), Margin = new Padding(20) };
                
                Label lblEscala = new Label { Text = $"ESCALA {i}", Font = new Font("Courier New", 16, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(10, 10) };
                card.Controls.Add(lblEscala);

                int vel = avionAMejorar.Velocidad + (i * 2);
                int vida = avionAMejorar.VidaMaxima + i;
                Label lblStats = new Label { Text = $"Vel: {vel}\nVida: {vida}", Font = new Font("Courier New", 14), ForeColor = Color.Cyan, AutoSize = true, Location = new Point(10, 50) };
                card.Controls.Add(lblStats);

                Button btnMejorar = new Button { Size = new Size(180, 50), Location = new Point(20, 230), Font = new Font("Courier New", 12, FontStyle.Bold) };
                btnMejorar.Tag = i;
                
                if (i <= avionAMejorar.NivelEscala)
                {
                    btnMejorar.Text = "ADQUIRIDA";
                    btnMejorar.Enabled = false;
                    card.ForeColor = Color.Lime;
                }
                else if (i == avionAMejorar.NivelEscala + 1)
                {
                    int costo = 1000 * (i + 1);
                    btnMejorar.Text = $"MEJORAR ({costo} P)";
                    btnMejorar.Enabled = DatosGlobales.PuntosTotales >= costo;
                    btnMejorar.Click += BtnMejorar_Click;
                }
                else
                {
                    btnMejorar.Text = "BLOQUEADO";
                    btnMejorar.Enabled = false;
                }
                card.Controls.Add(btnMejorar);
                panelTarjetas.Controls.Add(card);
            }
            
            Button btnVolver = new Button { Text = "VOLVER", Size = new Size(450, 90), Font = new Font("Courier New", 24, FontStyle.Bold), Anchor = AnchorStyles.None };
            btnVolver.Click += (s, e) => this.Close();
            panelPrincipal.Controls.Add(btnVolver, 0, 3);

            ActualizarLabels();
        }

        private void BtnMejorar_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not int nivelTarget) return;
            int costo = 1000 * (nivelTarget + 1);
            if (DatosGlobales.PuntosTotales >= costo)
            {
                DatosGlobales.PuntosTotales -= costo;
                avionAMejorar.NivelEscala = nivelTarget;
                MessageBox.Show("¡Nave mejorada a Escala " + nivelTarget + "!");
                InitializeUserInterface();
            }
        }

        private void ActualizarLabels()
        {
            lblPuntos.Text = $"PUNTOS DISPONIBLES: {DatosGlobales.PuntosTotales}";
        }
    }
}
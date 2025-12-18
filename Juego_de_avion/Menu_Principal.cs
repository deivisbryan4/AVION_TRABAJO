using System;
using System.Windows.Forms;
using System.Drawing;

namespace JuegoDeAvion
{
    public class Menu_Principal : Form
    {
        private int nivelSeleccionado = 1;
        private Label lblNivel;
        private Label lblTitulo;
        private Button btnNivelMenos, btnNivelMas, btnJugar, btnSeleccionar, btnMejorar, btnSalir;

        public Menu_Principal()
        {
            // Cargar todos los aviones al iniciar el juego
            DatosGlobales.CargarAviones();

            Text = "Menú Principal";
            this.Size = new Size(960, 540);
            this.MinimumSize = new Size(800, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;

            lblTitulo = new Label { Text = "GALAXY DEFENDER", Font = new Font("Courier New", 48, FontStyle.Bold), ForeColor = Color.Lime, AutoSize = true };
            lblNivel = new Label { Text = $"NIVEL: {nivelSeleccionado}", Font = new Font("Courier New", 24, FontStyle.Bold), ForeColor = Color.White, AutoSize = true };
            
            btnNivelMenos = CrearBoton("<", 0, 0, new Size(50, 50), new Font("Courier New", 20, FontStyle.Bold));
            btnNivelMas = CrearBoton(">", 0, 0, new Size(50, 50), new Font("Courier New", 20, FontStyle.Bold));
            
            Size tamanoBoton = new Size(300, 50);
            Font fuenteBoton = new Font("Courier New", 16, FontStyle.Bold);
            btnJugar = CrearBoton("INICIAR MISIÓN", 0, 0, tamanoBoton, fuenteBoton);
            btnSeleccionar = CrearBoton("HANGAR", 0, 0, tamanoBoton, fuenteBoton);
            btnMejorar = CrearBoton("ASTILLERO (MEJORAS)", 0, 0, tamanoBoton, fuenteBoton);
            btnSalir = CrearBoton("SALIR", 0, 0, tamanoBoton, fuenteBoton);

            this.Controls.AddRange(new Control[] { lblTitulo, lblNivel, btnNivelMenos, btnNivelMas, btnJugar, btnSeleccionar, btnMejorar, btnSalir });

            btnNivelMenos.Click += (s, e) => CambiarNivel(-1);
            btnNivelMas.Click += (s, e) => CambiarNivel(1);
            btnJugar.Click += (s, e) => {
                Formulario_Principal juego = new Formulario_Principal(nivelSeleccionado);
                juego.FormClosed += (s, args) => this.Show(); 
                this.Hide();
                juego.Show();
            };
            btnSeleccionar.Click += (s, e) => new Formulario_Seleccion_Avion().ShowDialog();
            btnMejorar.Click += (s, e) => new Formulario_Mejora_Avion().ShowDialog();
            btnSalir.Click += (s, e) => this.Close();
            
            this.Resize += (s, e) => PosicionarControles();
            PosicionarControles();
        }

        private void PosicionarControles()
        {
            int centroX = this.ClientSize.Width / 2;
            
            lblTitulo.Location = new Point(centroX - (lblTitulo.Width / 2), 50);
            lblNivel.Location = new Point(centroX - (lblNivel.Width / 2), 150);
            btnNivelMenos.Location = new Point(lblNivel.Left - btnNivelMenos.Width - 10, 145);
            btnNivelMas.Location = new Point(lblNivel.Right + 10, 145);

            int inicioY = 220;
            int espacioEntreBotones = 60;
            btnJugar.Location = new Point(centroX - btnJugar.Width / 2, inicioY);
            btnSeleccionar.Location = new Point(centroX - btnSeleccionar.Width / 2, inicioY + espacioEntreBotones);
            btnMejorar.Location = new Point(centroX - btnMejorar.Width / 2, inicioY + espacioEntreBotones * 2);
            btnSalir.Location = new Point(centroX - btnSalir.Width / 2, inicioY + espacioEntreBotones * 3);
        }

        private void CambiarNivel(int direccion)
        {
            nivelSeleccionado += direccion;
            if (nivelSeleccionado < 1) nivelSeleccionado = 5;
            if (nivelSeleccionado > 5) nivelSeleccionado = 1;
            lblNivel.Text = $"NIVEL: {nivelSeleccionado}";
            PosicionarControles();
        }

        private Button CrearBoton(string texto, int x, int y, Size tamano, Font fuente)
        {
            Button btn = new Button { Text = texto, Size = tamano, Font = fuente };
            btn.Location = new Point(x - (tamano.Width / 2), y);
            btn.BackColor = Color.Black;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.Lime;
            btn.FlatAppearance.BorderSize = 2;
            btn.MouseEnter += (s, e) => { btn.BackColor = Color.Lime; btn.ForeColor = Color.Black; };
            btn.MouseLeave += (s, e) => { btn.BackColor = Color.Black; btn.ForeColor = Color.White; };
            return btn;
        }
    }
}
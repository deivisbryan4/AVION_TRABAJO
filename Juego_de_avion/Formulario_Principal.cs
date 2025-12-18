using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace JuegoDeAvion
{
    public partial class Formulario_Principal : Form
    {
        private Avion avion;
        private Escenario escenario;
        private List<Enemigo> enemigos;
        private List<Bala> balasJugador;
        private List<BalaEnemiga> balasEnemigas;
        private InputHandler inputHandler; // NUEVO

        private System.Windows.Forms.Timer gameLoop;
        private bool disparoBloqueado = false;
        private bool juegoTerminado = false;
        private int puntuacionPartida = 0;
        private int nivelActual;
        private int probabilidadEnemigo = 3;
        private Random rnd = new Random();

        public Formulario_Principal(int nivelSeleccionado = 1)
        {
            this.nivelActual = nivelSeleccionado;
            this.DoubleBuffered = true;
            this.Size = new Size(1280, 720); 
            this.MinimumSize = new Size(800, 600);
            this.Text = $"Juego de Avión - Nivel {nivelActual}";
            this.BackColor = Color.Black;
            this.StartPosition = FormStartPosition.CenterScreen;

            inputHandler = new InputHandler(); // Inicializamos el manejador de input
            avion = new Avion(this.ClientSize.Width / 2, this.ClientSize.Height - 150);
            escenario = new Escenario(this.ClientSize.Width, this.ClientSize.Height, 150);
            escenario.CambiarNivel(nivelActual);
            
            enemigos = new List<Enemigo>();
            balasJugador = new List<Bala>();
            balasEnemigas = new List<BalaEnemiga>();

            this.KeyDown += (s, e) => inputHandler.ProcesarTecla(e, true); // Delegamos al handler
            this.KeyUp += (s, e) => inputHandler.ProcesarTecla(e, false);   // Delegamos al handler
            this.Resize += (s, e) => escenario?.Redimensionar(this.ClientSize.Width, this.ClientSize.Height);

            gameLoop = new System.Windows.Forms.Timer { Interval = 16 };
            gameLoop.Tick += GameLoop_Tick;
            gameLoop.Start();
        }

        private void GameLoop_Tick(object? sender, EventArgs e)
        {
            if (juegoTerminado) return;

            // Usamos las propiedades del InputHandler
            if (inputHandler.Izquierda) avion.MoverIzquierda();
            if (inputHandler.Derecha) avion.MoverDerecha(this.ClientSize.Width);
            if (inputHandler.Arriba) avion.MoverArriba();
            if (inputHandler.Abajo) avion.MoverAbajo(this.ClientSize.Height);
            if (inputHandler.Salir) this.Close();

            GestionarDisparos();
            escenario.Mover();
            GenerarEnemigos();
            MoverEntidades();
            VerificarColisiones();
            
            this.Invalidate();
        }

        private void GestionarDisparos()
        {
            if (inputHandler.Disparar && !disparoBloqueado)
            {
                Point p = avion.Centro;
                balasJugador.Add(new Bala(p.X, p.Y - 10));
                disparoBloqueado = true;
                
                var cooldown = new System.Windows.Forms.Timer { Interval = 200 };
                cooldown.Tick += (s, args) => { disparoBloqueado = false; cooldown.Stop(); cooldown.Dispose(); };
                cooldown.Start();
            }
        }

        private void GenerarEnemigos()
        {
            if (rnd.Next(100) < probabilidadEnemigo)
            {
                int x = rnd.Next(0, this.ClientSize.Width - 60);
                int velocidad = rnd.Next(4, 8 + nivelActual);
                bool dispara = rnd.Next(100) < (10 + nivelActual * 5);
                enemigos.Add(new Enemigo(x, -60, 60, 60, velocidad, dispara, this.ClientSize.Width));
            }
        }

        private void MoverEntidades()
        {
            foreach (var enemigo in enemigos)
            {
                enemigo.Mover(this.ClientSize.Width);
                if (enemigo.QuiereDisparar(rnd))
                {
                    balasEnemigas.Add(new BalaEnemiga(enemigo.Bounds.X + enemigo.Bounds.Width / 2, enemigo.Bounds.Y + enemigo.Bounds.Height));
                }
            }
            enemigos.RemoveAll(e => e.FueraDePantalla(this.ClientSize.Height));
            balasJugador.ForEach(b => b.Mover());
            balasJugador.RemoveAll(b => !b.Activa);
            balasEnemigas.ForEach(b => b.Mover(this.ClientSize.Height));
            balasEnemigas.RemoveAll(b => !b.Activa);
        }

        private void VerificarColisiones()
        {
            for (int i = enemigos.Count - 1; i >= 0; i--)
            {
                if (avion.ObtenerRegion().IsVisible(enemigos[i].Bounds))
                {
                    avion.RecibirDano(40);
                    enemigos.RemoveAt(i);
                    if (avion.VidaActual <= 0) { GameOver(); return; }
                }
            }
            for (int i = balasEnemigas.Count - 1; i >= 0; i--)
            {
                if (avion.ObtenerRegion().IsVisible(balasEnemigas[i].Bounds))
                {
                    avion.RecibirDano(25);
                    balasEnemigas.RemoveAt(i);
                    if (avion.VidaActual <= 0) { GameOver(); return; }
                }
            }

            for (int i = balasJugador.Count - 1; i >= 0; i--)
            {
                for (int j = enemigos.Count - 1; j >= 0; j--)
                {
                    if (balasJugador[i].Bounds.IntersectsWith(enemigos[j].Bounds))
                    {
                        enemigos[j].RecibirDano(50);
                        balasJugador.RemoveAt(i);
                        if (enemigos[j].VidaActual <= 0)
                        {
                            enemigos.RemoveAt(j);
                            puntuacionPartida += 50;
                        }
                        break; 
                    }
                }
            }
        }

        private void GameOver()
        {
            juegoTerminado = true;
            gameLoop.Stop();
            DatosGlobales.PuntosTotales += puntuacionPartida;
            MessageBox.Show($"¡Juego Terminado!\nPuntuación: {puntuacionPartida}\nPuntos Totales: {DatosGlobales.PuntosTotales}", "Game Over");
            this.Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            escenario.Dibujar(g);
            avion.Dibujar(g);

            balasJugador.ForEach(b => b.Dibujar(g));
            balasEnemigas.ForEach(b => b.Dibujar(g));
            enemigos.ForEach(e => e.Dibujar(g));

            string textoUI = $"PUNTOS: {puntuacionPartida} | NIVEL: {nivelActual}";
            g.DrawString(textoUI, new Font("Courier New", 28, FontStyle.Bold), Brushes.White, 20, 20);
            
            float porcentajeVida = (float)avion.VidaActual / avion.VidaMaxima;
            g.FillRectangle(Brushes.Red, 20, 70, 400, 30);
            g.FillRectangle(Brushes.Green, 20, 70, 400 * porcentajeVida, 30);
            g.DrawRectangle(new Pen(Color.White, 3), 20, 70, 400, 30);
        }
    }
}
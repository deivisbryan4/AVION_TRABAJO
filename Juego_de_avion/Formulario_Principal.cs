using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace JuegoDeAvion
{
    public partial class Formulario_Principal : Form
    {
        private Jugador jugador;
        private Escenario escenario;
        private List<Enemigo> enemigos;
        private List<Obstaculo> obstaculos;
        private List<Bala> balasJugador;
        private List<BalaEnemiga> balasEnemigas;
        private EntradaDeTeclado entradaDeTeclado;

        private System.Windows.Forms.Timer gameLoop;
        private bool disparoBloqueado = false;
        private bool juegoTerminado = false;
        private int puntuacionPartida = 0;
        private int nivelActual;
        private Random rnd = new Random();

        public Formulario_Principal(int nivelSeleccionado = 1)
        {
            this.nivelActual = nivelSeleccionado;
            this.DoubleBuffered = true;
            this.Size = new Size(960, 540); 
            this.MinimumSize = new Size(800, 450);
            this.Text = $"Juego de Avión - Nivel {nivelActual}";
            this.BackColor = Color.Black;
            this.StartPosition = FormStartPosition.CenterScreen;

            entradaDeTeclado = new EntradaDeTeclado();
            jugador = new Jugador(this.ClientSize.Width / 2, this.ClientSize.Height - 100);
            escenario = new Escenario(this.ClientSize.Width, this.ClientSize.Height, 100);
            escenario.CambiarNivel(nivelActual);
            
            enemigos = new List<Enemigo>();
            obstaculos = new List<Obstaculo>();
            balasJugador = new List<Bala>();
            balasEnemigas = new List<BalaEnemiga>();

            this.KeyDown += (s, e) => entradaDeTeclado.ProcesarTecla(e, true);
            this.KeyUp += (s, e) => entradaDeTeclado.ProcesarTecla(e, false);
            this.Resize += (s, e) => escenario?.Redimensionar(this.ClientSize.Width, this.ClientSize.Height);

            gameLoop = new System.Windows.Forms.Timer { Interval = 16 };
            gameLoop.Tick += GameLoop_Tick;
            gameLoop.Start();
        }

        private void GameLoop_Tick(object? sender, EventArgs e)
        {
            if (juegoTerminado) return;

            if (entradaDeTeclado.Izquierda) jugador.MoverIzquierda();
            if (entradaDeTeclado.Derecha) jugador.MoverDerecha(this.ClientSize.Width);
            if (entradaDeTeclado.Arriba) jugador.MoverArriba();
            if (entradaDeTeclado.Abajo) jugador.MoverAbajo(this.ClientSize.Height);
            if (entradaDeTeclado.Salir) this.Close();

            GestionarDisparos();
            escenario.Mover();
            GenerarEntidades();
            MoverEntidades();
            VerificarColisiones();
            
            this.Invalidate();
        }

        private void GestionarDisparos()
        {
            if (entradaDeTeclado.Disparar && !disparoBloqueado)
            {
                Point p = jugador.Centro;
                balasJugador.Add(new Bala(p.X, p.Y - 10));
                disparoBloqueado = true;
                
                var cooldown = new System.Windows.Forms.Timer { Interval = 200 };
                cooldown.Tick += (s, args) => { disparoBloqueado = false; cooldown.Stop(); cooldown.Dispose(); };
                cooldown.Start();
            }
        }

        private void GenerarEntidades()
        {
            // Generar un tipo de enemigo aleatorio
            if (rnd.Next(100) < (2 + nivelActual))
            {
                int tipoEnemigo = rnd.Next(1, 4); // Genera un tipo entre 1 y 3
                int x = rnd.Next(0, this.ClientSize.Width - 50);
                int velocidad = rnd.Next(3, 6 + nivelActual);
                enemigos.Add(new Enemigo(tipoEnemigo, x, -50, 50, 50, velocidad));
            }
            
            // Generar asteroides
            if (rnd.Next(100) < 4)
            {
                int x = rnd.Next(0, this.ClientSize.Width - 60);
                int velocidad = rnd.Next(2, 5);
                obstaculos.Add(new Obstaculo(x, -60, 60, 60, velocidad));
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
            
            obstaculos.ForEach(o => o.Mover(this.ClientSize.Width));
            obstaculos.RemoveAll(o => o.FueraDePantalla(this.ClientSize.Height));

            balasJugador.ForEach(b => b.Mover());
            balasJugador.RemoveAll(b => !b.Activa);
            balasEnemigas.ForEach(b => b.Mover(this.ClientSize.Height));
            balasEnemigas.RemoveAll(b => !b.Activa);
        }

        private void VerificarColisiones()
        {
            for (int i = enemigos.Count - 1; i >= 0; i--)
            {
                if (jugador.ObtenerRegion().IsVisible(enemigos[i].Bounds))
                {
                    jugador.RecibirDano(40);
                    enemigos.RemoveAt(i);
                    if (jugador.VidaActual <= 0) { GameOver(); return; }
                }
            }
            for (int i = obstaculos.Count - 1; i >= 0; i--)
            {
                if (jugador.ObtenerRegion().IsVisible(obstaculos[i].Bounds))
                {
                    jugador.RecibirDano(60);
                    obstaculos.RemoveAt(i);
                    if (jugador.VidaActual <= 0) { GameOver(); return; }
                }
            }
            for (int i = balasEnemigas.Count - 1; i >= 0; i--)
            {
                if (jugador.ObtenerRegion().IsVisible(balasEnemigas[i].Bounds))
                {
                    jugador.RecibirDano(25);
                    balasEnemigas.RemoveAt(i);
                    if (jugador.VidaActual <= 0) { GameOver(); return; }
                }
            }

            for (int i = balasJugador.Count - 1; i >= 0; i--)
            {
                bool balaChoco = false;
                for (int j = enemigos.Count - 1; j >= 0; j--)
                {
                    if (balasJugador[i].Bounds.IntersectsWith(enemigos[j].Bounds))
                    {
                        enemigos[j].RecibirDano(50);
                        if (enemigos[j].VidaActual <= 0) { enemigos.RemoveAt(j); puntuacionPartida += 100; }
                        balasJugador.RemoveAt(i);
                        balaChoco = true;
                        break; 
                    }
                }
                if (balaChoco) continue;

                for (int k = obstaculos.Count - 1; k >= 0; k--)
                {
                    if (balasJugador[i].Bounds.IntersectsWith(obstaculos[k].Bounds))
                    {
                        obstaculos[k].RecibirDano(50);
                        if (obstaculos[k].VidaActual <= 0) { obstaculos.RemoveAt(k); puntuacionPartida += 20; }
                        balasJugador.RemoveAt(i);
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
            jugador.Dibujar(g);

            balasJugador.ForEach(b => b.Dibujar(g));
            balasEnemigas.ForEach(b => b.Dibujar(g));
            enemigos.ForEach(e => e.Dibujar(g));
            obstaculos.ForEach(o => o.Dibujar(g));

            string textoUI = $"PUNTOS: {puntuacionPartida} | NIVEL: {nivelActual}";
            g.DrawString(textoUI, new Font("Courier New", 18, FontStyle.Bold), Brushes.White, 10, 10);
            
            float porcentajeVida = (float)jugador.VidaActual / jugador.VidaMaxima;
            g.FillRectangle(Brushes.Red, 10, 40, 300, 20);
            g.FillRectangle(Brushes.Green, 10, 40, 300 * porcentajeVida, 20);
            g.DrawRectangle(new Pen(Color.White, 2), 10, 40, 300, 20);
        }
    }
}
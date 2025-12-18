using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace JuegoDeAvion
{
    /// <summary>
    /// Formulario principal del juego. Contiene el bucle principal (Game Loop)
    /// y gestiona la interacción entre todos los elementos del juego (jugador, enemigos, etc.).
    /// </summary>
    public partial class Formulario_Principal : Form
    {
        #region Campos Privados
        
        private Jugador jugador;
        private Escenario escenario;
        private List<Enemigo> enemigos;
        private List<GalaxianPixelEnemy> galaxianEnemies;
        private List<PixelArtEnemy> pixelArtEnemies;
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

        // Control de dificultad
        private int maxEnemigosSimultaneos;
        private double probabilidadEnemigoBase;
        
        // Control de victoria y niveles
        private int puntajeParaGanar = 1000; // Puntaje necesario para ganar el nivel
        private int maxNivel = 5; // Máximo número de niveles

        #endregion

        public Formulario_Principal(int nivelSeleccionado = 1)
        {
            this.nivelActual = nivelSeleccionado;
            
            // Configurar dificultad según el nivel
            // Nivel 1: Max 3 enemigos, 0.5% prob
            // Nivel 5: Max 11 enemigos, 2.5% prob
            this.maxEnemigosSimultaneos = 3 + (nivelActual * 2); 
            this.probabilidadEnemigoBase = 0.5 + (nivelActual * 0.5);

            // Configuración inicial del formulario
            this.DoubleBuffered = true; 
            this.Size = new Size(960, 540); 
            this.MinimumSize = new Size(800, 450);
            this.Text = $"Juego de Avión - Nivel {nivelActual}";
            this.BackColor = Color.Black;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Inicialización de objetos del juego
            entradaDeTeclado = new EntradaDeTeclado();
            jugador = new Jugador(this.ClientSize.Width / 2, this.ClientSize.Height - 100);
            escenario = new Escenario(this.ClientSize.Width, this.ClientSize.Height, 100);
            escenario.CambiarNivel(nivelActual);
            
            enemigos = new List<Enemigo>();
            galaxianEnemies = new List<GalaxianPixelEnemy>();
            pixelArtEnemies = new List<PixelArtEnemy>();
            obstaculos = new List<Obstaculo>();
            balasJugador = new List<Bala>();
            balasEnemigas = new List<BalaEnemiga>();

            // Suscripción a eventos
            this.KeyDown += (s, e) => entradaDeTeclado.ProcesarTecla(e, true);
            this.KeyUp += (s, e) => entradaDeTeclado.ProcesarTecla(e, false);
            this.Resize += (s, e) => escenario?.Redimensionar(this.ClientSize.Width, this.ClientSize.Height);

            // Iniciar el bucle de juego
            gameLoop = new System.Windows.Forms.Timer { Interval = 16 }; // Aproximadamente 60 FPS
            gameLoop.Tick += GameLoop_Tick;
            gameLoop.Start();
        }

        /// <summary>
        /// El corazón del juego. Este método se ejecuta en cada "tick" del temporizador.
        /// </summary>
        private void GameLoop_Tick(object? sender, EventArgs e)
        {
            if (juegoTerminado) return;

            // 1. Procesar entrada del jugador
            if (entradaDeTeclado.Izquierda) jugador.MoverIzquierda();
            if (entradaDeTeclado.Derecha) jugador.MoverDerecha(this.ClientSize.Width);
            if (entradaDeTeclado.Arriba) jugador.MoverArriba();
            if (entradaDeTeclado.Abajo) jugador.MoverAbajo(this.ClientSize.Height);
            if (entradaDeTeclado.Salir) this.Close();
            GestionarDisparos();

            // 2. Actualizar estado del mundo
            escenario.Mover();
            GenerarEntidades();
            MoverEntidades();
            
            // 3. Comprobar lógica del juego
            VerificarColisiones();
            VerificarCondicionVictoria();
            
            // 4. Redibujar la pantalla
            this.Invalidate();
        }

        #region Lógica del Juego

        /// <summary>Crea un nuevo disparo del jugador si se presiona la tecla y no hay cooldown.</summary>
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

        /// <summary>Genera nuevos enemigos y obstáculos de forma aleatoria y controlada.</summary>
        private void GenerarEntidades()
        {
            // Solo generar si no hemos alcanzado el límite de población
            if (enemigos.Count < maxEnemigosSimultaneos)
            {
                // Usamos NextDouble() que devuelve 0.0 a 1.0
                if (rnd.NextDouble() * 100 < probabilidadEnemigoBase)
                {
                    int tipoEnemigo = rnd.Next(1, 4);
                    int x = rnd.Next(0, this.ClientSize.Width - 50);
                    int velocidad = rnd.Next(3, 5 + nivelActual); // Velocidad moderada
                    enemigos.Add(new Enemigo(tipoEnemigo, x, -50, 50, 50, velocidad));
                }
            }
            
            // Generar obstáculos (asteroides) independientemente, pero con baja probabilidad
            if (obstaculos.Count < 5 + nivelActual) // Límite de asteroides
            {
                if (rnd.Next(100) < 1) // 1% fijo
                {
                    int x = rnd.Next(0, this.ClientSize.Width - 60);
                    int velocidad = rnd.Next(2, 5);
                    obstaculos.Add(new Obstaculo(x, -60, 60, 60, velocidad));
                }
            }

            // Generar GalaxianPixel enemies periódicamente como asteroides
            if (galaxianEnemies.Count < 3) // Límite específico para GalaxianPixel
            {
                if (rnd.Next(100) < 2) // 2% de probabilidad, similar a asteroides
                {
                    int x = rnd.Next(0, this.ClientSize.Width - 62);
                    int velocidad = rnd.Next(2, 4 + nivelActual);
                    galaxianEnemies.Add(new GalaxianPixelEnemy(x, -60, 62, 60, velocidad));
                }
            }

            // Generar PixelArt enemies periódicamente como asteroides
            if (pixelArtEnemies.Count < 3) // Límite específico para PixelArt
            {
                if (rnd.Next(100) < 2) // 2% de probabilidad, similar a asteroides
                {
                    int x = rnd.Next(0, this.ClientSize.Width - 64);
                    int velocidad = rnd.Next(2, 4 + nivelActual);
                    pixelArtEnemies.Add(new PixelArtEnemy(x, -60, 64, 64, velocidad));
                }
            }
        }

        /// <summary>Mueve todas las entidades activas en el juego.</summary>
        private void MoverEntidades()
        {
            // Mover enemigos normales
            foreach (var enemigo in enemigos)
            {
                enemigo.Mover(this.ClientSize.Width);
                if (enemigo.QuiereDisparar(rnd))
                {
                    balasEnemigas.Add(new BalaEnemiga(enemigo.Bounds.X + enemigo.Bounds.Width / 2, enemigo.Bounds.Y + enemigo.Bounds.Height));
                }
            }
            enemigos.RemoveAll(e => e.FueraDePantalla(this.ClientSize.Height));
            
            // Mover GalaxianPixel enemies
            foreach (var galaxian in galaxianEnemies)
            {
                galaxian.Mover(this.ClientSize.Width);
                if (galaxian.QuiereDisparar(rnd))
                {
                    balasEnemigas.Add(new BalaEnemiga(galaxian.Bounds.X + galaxian.Bounds.Width / 2, galaxian.Bounds.Y + galaxian.Bounds.Height));
                }
            }
            galaxianEnemies.RemoveAll(e => e.FueraDePantalla(this.ClientSize.Height));
            
            // Mover PixelArt enemies
            foreach (var pixelArt in pixelArtEnemies)
            {
                pixelArt.Mover(this.ClientSize.Width);
                if (pixelArt.QuiereDisparar(rnd))
                {
                    balasEnemigas.Add(new BalaEnemiga(pixelArt.Bounds.X + pixelArt.Bounds.Width / 2, pixelArt.Bounds.Y + pixelArt.Bounds.Height));
                }
            }
            pixelArtEnemies.RemoveAll(e => e.FueraDePantalla(this.ClientSize.Height));
            
            obstaculos.ForEach(o => o.Mover(this.ClientSize.Width));
            obstaculos.RemoveAll(o => o.FueraDePantalla(this.ClientSize.Height));

            balasJugador.ForEach(b => b.Mover());
            balasJugador.RemoveAll(b => !b.Activa);
            balasEnemigas.ForEach(b => b.Mover(this.ClientSize.Height));
            balasEnemigas.RemoveAll(b => !b.Activa);
        }

        /// <summary>Comprueba y gestiona todas las colisiones del juego.</summary>
        private void VerificarColisiones()
        {
            // Jugador vs Enemigos normales
            for (int i = enemigos.Count - 1; i >= 0; i--)
            {
                if (jugador.ObtenerRegion().IsVisible(enemigos[i].Bounds))
                {
                    jugador.RecibirDano(40);
                    enemigos.RemoveAt(i);
                    if (jugador.VidaActual <= 0) { GameOver(); return; }
                }
            }
            
            // Jugador vs GalaxianPixel enemies
            for (int i = galaxianEnemies.Count - 1; i >= 0; i--)
            {
                if (jugador.ObtenerRegion().IsVisible(galaxianEnemies[i].Bounds))
                {
                    jugador.RecibirDano(45);
                    galaxianEnemies.RemoveAt(i);
                    if (jugador.VidaActual <= 0) { GameOver(); return; }
                }
            }
            
            // Jugador vs PixelArt enemies
            for (int i = pixelArtEnemies.Count - 1; i >= 0; i--)
            {
                if (jugador.ObtenerRegion().IsVisible(pixelArtEnemies[i].Bounds))
                {
                    jugador.RecibirDano(35);
                    pixelArtEnemies.RemoveAt(i);
                    if (jugador.VidaActual <= 0) { GameOver(); return; }
                }
            }
            
            // Jugador vs Obstáculos
            for (int i = obstaculos.Count - 1; i >= 0; i--)
            {
                if (jugador.ObtenerRegion().IsVisible(obstaculos[i].Bounds))
                {
                    jugador.RecibirDano(60);
                    obstaculos.RemoveAt(i);
                    if (jugador.VidaActual <= 0) { GameOver(); return; }
                }
            }
            
            // Jugador vs Balas enemigas
            for (int i = balasEnemigas.Count - 1; i >= 0; i--)
            {
                if (jugador.ObtenerRegion().IsVisible(balasEnemigas[i].Bounds))
                {
                    jugador.RecibirDano(25);
                    balasEnemigas.RemoveAt(i);
                    if (jugador.VidaActual <= 0) { GameOver(); return; }
                }
            }

            // Bala del Jugador vs Entidades
            for (int i = balasJugador.Count - 1; i >= 0; i--)
            {
                bool balaChoco = false;
                
                // vs Enemigos normales
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
                
                // vs GalaxianPixel enemies
                for (int j = galaxianEnemies.Count - 1; j >= 0; j--)
                {
                    if (balasJugador[i].Bounds.IntersectsWith(galaxianEnemies[j].Bounds))
                    {
                        galaxianEnemies[j].RecibirDano(50);
                        if (galaxianEnemies[j].VidaActual <= 0) { galaxianEnemies.RemoveAt(j); puntuacionPartida += 150; }
                        balasJugador.RemoveAt(i);
                        balaChoco = true;
                        break; 
                    }
                }
                if (balaChoco) continue;
                
                // vs PixelArt enemies
                for (int j = pixelArtEnemies.Count - 1; j >= 0; j--)
                {
                    if (balasJugador[i].Bounds.IntersectsWith(pixelArtEnemies[j].Bounds))
                    {
                        pixelArtEnemies[j].RecibirDano(50);
                        if (pixelArtEnemies[j].VidaActual <= 0) { pixelArtEnemies.RemoveAt(j); puntuacionPartida += 125; }
                        balasJugador.RemoveAt(i);
                        balaChoco = true;
                        break; 
                    }
                }
                if (balaChoco) continue;

                // vs Obstáculos
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

        /// <summary>Verifica si el jugador ha alcanzado el puntaje necesario para ganar el nivel.</summary>
        private void VerificarCondicionVictoria()
        {
            if (puntuacionPartida >= puntajeParaGanar)
            {
                Victoria();
            }
        }

        /// <summary>Maneja la victoria del nivel actual.</summary>
        private void Victoria()
        {
            juegoTerminado = true;
            gameLoop.Stop();
            DatosGlobales.PuntosTotales += puntuacionPartida;

            if (nivelActual >= maxNivel)
            {
                // El jugador ha completado todos los niveles
                MessageBox.Show($"¡FELICIDADES! Has completado todos los niveles.\nPuntuación final: {puntuacionPartida}\nPuntos totales: {DatosGlobales.PuntosTotales}", "¡VICTORIA COMPLETA!");
                this.Close();
            }
            else
            {
                // Pasar al siguiente nivel
                DialogResult result = MessageBox.Show($"¡Nivel {nivelActual} completado!\nPuntuación: {puntuacionPartida}\n\n¿Deseas continuar al nivel {nivelActual + 1}?", "¡VICTORIA!", MessageBoxButtons.YesNo);
                
                if (result == DialogResult.Yes)
                {
                    // Iniciar el siguiente nivel
                    SiguienteNivel();
                }
                else
                {
                    // Volver al menú principal
                    this.Close();
                }
            }
        }

        /// <summary>Inicia el siguiente nivel con mayor dificultad.</summary>
        private void SiguienteNivel()
        {
            nivelActual++;
            puntuacionPartida = 0;
            juegoTerminado = false;
            
            // Aumentar dificultad
            maxEnemigosSimultaneos = 3 + (nivelActual * 2);
            probabilidadEnemigoBase = 0.5 + (nivelActual * 0.5);
            puntajeParaGanar = 1000 * nivelActual; // Aumentar puntaje necesario
            
            // Resetear entidades
            enemigos.Clear();
            galaxianEnemies.Clear();
            pixelArtEnemies.Clear();
            obstaculos.Clear();
            balasJugador.Clear();
            balasEnemigas.Clear();
            
            // Resetear jugador
            jugador = new Jugador(this.ClientSize.Width / 2, this.ClientSize.Height - 100);
            
            // Actualizar escenario
            escenario.CambiarNivel(nivelActual);
            
            // Actualizar título
            this.Text = $"Juego de Avión - Nivel {nivelActual}";
            
            // Reiniciar game loop
            gameLoop.Start();
        }

        /// <summary>Finaliza la partida, muestra la puntuación y cierra el formulario.</summary>
        private void GameOver()
        {
            juegoTerminado = true;
            gameLoop.Stop();
            DatosGlobales.PuntosTotales += puntuacionPartida;
            MessageBox.Show($"¡PERDISTE!\nPuntuación: {puntuacionPartida}\nPuntos Totales: {DatosGlobales.PuntosTotales}", "Game Over");
            this.Close();
        }

        /// <summary>
        /// Dibuja todos los elementos del juego en la pantalla.
        /// Se llama automáticamente después de Invalidate().
        /// </summary>
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
            
            // Dibujar enemigos normales
            enemigos.ForEach(e => e.Dibujar(g));
            
            // Dibujar GalaxianPixel enemies
            galaxianEnemies.ForEach(ge => ge.Dibujar(g));
            
            // Dibujar PixelArt enemies
            pixelArtEnemies.ForEach(pe => pe.Dibujar(g));
            
            obstaculos.ForEach(o => o.Dibujar(g));

            // Dibujar HUD
            string textoUI = $"PUNTOS: {puntuacionPartida}/{puntajeParaGanar} | NIVEL: {nivelActual}";
            g.DrawString(textoUI, new Font("Courier New", 18, FontStyle.Bold), Brushes.White, 10, 10);
            
            float porcentajeVida = (float)jugador.VidaActual / jugador.VidaMaxima;
            g.FillRectangle(Brushes.Red, 10, 40, 300, 20);
            g.FillRectangle(Brushes.Green, 10, 40, 300 * porcentajeVida, 20);
            g.DrawRectangle(new Pen(Color.White, 2), 10, 40, 300, 20);
        }
        #endregion
    }
}
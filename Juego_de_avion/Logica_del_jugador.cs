using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace JuegoDeAvion
{
    /// <summary>
    /// Representa la nave controlada por el jugador.
    /// Gestiona su posición, estadísticas, movimiento y dibujo.
    /// </summary>
    public class Jugador 
    {
        #region Propiedades
        
        /// <summary>Posición horizontal (coordenada X) de la nave.</summary>
        public float X { get; private set; }
        
        /// <summary>Posición vertical (coordenada Y) de la nave.</summary>
        public float Y { get; private set; }
        
        /// <summary>Ancho de la nave, puede cambiar según las mejoras.</summary>
        public int Ancho { get; private set; }
        
        /// <summary>Alto de la nave, puede cambiar según las mejoras.</summary>
        public int Alto { get; private set; }
        
        /// <summary>Puntos de vida actuales de la nave.</summary>
        public int VidaActual { get; private set; }
        
        /// <summary>Máximos puntos de vida que la nave puede tener.</summary>
        public int VidaMaxima { get; private set; }
        
        /// <summary>Punto central de la nave, útil para disparar.</summary>
        public Point Centro => new Point((int)(X + Ancho / 2), (int)(Y + Alto / 2));

        #endregion

        #region Campos Privados
        
        private int velocidad;
        private Color color;
        private Point[] _puntosBase = new Point[0];    // Puntos del polígono normalizados (0,0)
        private Point[] _puntosActuales = new Point[0]; // Puntos del polígono en la posición actual

        #endregion

        /// <summary>
        /// Crea una nueva instancia del jugador en una posición inicial.
        /// Carga las estadísticas y el diseño del avión seleccionado en DatosGlobales.
        /// </summary>
        /// <param name="startX">Posición X inicial.</param>
        /// <param name="startY">Posición Y inicial.</param>
        public Jugador(int startX, int startY)
        {
            X = startX;
            Y = startY;

            // Carga el tipo de avión seleccionado por el jugador
            TipoAvion tipo = DatosGlobales.Aviones[DatosGlobales.IndiceAvionSeleccionado];
            
            // Las estadísticas finales dependen de las base + el nivel de escala
            this.velocidad = tipo.Velocidad + (tipo.NivelEscala * 2); 
            this.color = tipo.ColorPrincipal;
            this.VidaMaxima = (tipo.VidaMaxima + tipo.NivelEscala) * 100;
            this.VidaActual = this.VidaMaxima;
            
            // El tamaño también se ve afectado por la escala
            this.Ancho = 50 + (tipo.NivelEscala * 5);
            this.Alto = 50 + (tipo.NivelEscala * 5);

            // Carga y procesa la forma del polígono
            Point[] rawPoints = Avion.GetShipPoints(tipo.TipoPoligono);
            NormalizarYEscalarPuntos(rawPoints);
            ActualizarPosicion();
        }

        #region Métodos Públicos
        
        /// <summary>Reduce la vida del jugador.</summary>
        /// <param name="cantidad">Cantidad de daño a recibir.</param>
        public void RecibirDano(int cantidad)
        {
            VidaActual -= cantidad;
            if (VidaActual < 0) VidaActual = 0;
        }

        /// <summary>Mueve la nave hacia la izquierda, respetando los límites de la pantalla.</summary>
        public void MoverIzquierda() { X -= velocidad; if (X < 0) X = 0; ActualizarPosicion(); }
        
        /// <summary>Mueve la nave hacia la derecha, respetando los límites de la pantalla.</summary>
        public void MoverDerecha(int anchoPantalla) { X += velocidad; if (X + Ancho > anchoPantalla) X = anchoPantalla - Ancho; ActualizarPosicion(); }
        
        /// <summary>Mueve la nave hacia arriba, respetando los límites de la pantalla.</summary>
        public void MoverArriba() { Y -= velocidad; if (Y < 0) Y = 0; ActualizarPosicion(); }
        
        /// <summary>Mueve la nave hacia abajo, respetando los límites de la pantalla.</summary>
        public void MoverAbajo(int altoPantalla) { Y += velocidad; if (Y + Alto > altoPantalla) Y = altoPantalla - Alto; ActualizarPosicion(); }

        /// <summary>Dibuja la nave en la pantalla.</summary>
        public void Dibujar(Graphics g)
        {
            if (_puntosActuales == null || _puntosActuales.Length < 2) return;
            using (SolidBrush brocha = new SolidBrush(this.color))
            using (Pen lapiz = new Pen(Color.White, 2))
            {
                g.FillPolygon(brocha, _puntosActuales);
                g.DrawPolygon(lapiz, _puntosActuales);
            }
        }

        /// <summary>Obtiene la región poligonal de la nave para colisiones precisas.</summary>
        public Region ObtenerRegion()
        {
            if (_puntosActuales == null || _puntosActuales.Length < 3) return new Region(new Rectangle((int)X, (int)Y, Ancho, Alto));
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(_puntosActuales);
            return new Region(path);
        }

        #endregion

        #region Métodos Privados
        
        /// <summary>
        /// Toma los puntos de un polígono y los escala para que encajen en el Ancho y Alto de la nave.
        /// El resultado se guarda en _puntosBase.
        /// </summary>
        private void NormalizarYEscalarPuntos(Point[] raw)
        {
            if (raw == null || raw.Length == 0) return;
            int minX = raw.Min(p => p.X); int maxX = raw.Max(p => p.X);
            int minY = raw.Min(p => p.Y); int maxY = raw.Max(p => p.Y);
            float anchoOriginal = Math.Max(1, maxX - minX); float altoOriginal = Math.Max(1, maxY - minY);
            _puntosBase = new Point[raw.Length]; _puntosActuales = new Point[raw.Length];
            float ratioX = this.Ancho / anchoOriginal; float ratioY = this.Alto / altoOriginal;
            for (int i = 0; i < raw.Length; i++)
            {
                _puntosBase[i] = new Point((int)((raw[i].X - minX) * ratioX), (int)((raw[i].Y - minY) * ratioY));
            }
        }

        /// <summary>
        /// Actualiza la posición del polígono en pantalla (_puntosActuales)
        /// basándose en la posición actual (X, Y) de la nave.
        /// </summary>
        private void ActualizarPosicion()
        {
            if (_puntosBase == null) return;
            for (int i = 0; i < _puntosBase.Length; i++)
            {
                _puntosActuales[i] = new Point(_puntosBase[i].X + (int)X, _puntosBase[i].Y + (int)Y);
            }
        }

        #endregion
    }
}
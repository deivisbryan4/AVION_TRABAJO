using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace JuegoDeAvion
{
    public class Jugador 
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public int Ancho { get; private set; }
        public int Alto { get; private set; }
        public int VidaActual { get; private set; }
        public int VidaMaxima { get; private set; }
        public Point Centro => new Point((int)(X + Ancho / 2), (int)(Y + Alto / 2));

        private int velocidad;
        private Color color;
        private Point[]? _puntosBase;    // Puede ser nulo si se usa imagen
        private Point[]? _puntosActuales;
        private Image? _imagenNave;      // Imagen para naves personalizadas

        public Jugador(int startX, int startY)
        {
            X = startX;
            Y = startY;

            TipoAvion tipo = DatosGlobales.Aviones[DatosGlobales.IndiceAvionSeleccionado];
            
            this.velocidad = tipo.Velocidad + (tipo.NivelEscala * 2); 
            this.color = tipo.ColorPrincipal;
            this.VidaMaxima = (tipo.VidaMaxima + tipo.NivelEscala) * 100;
            this.VidaActual = this.VidaMaxima;
            
            this.Ancho = 50 + (tipo.NivelEscala * 5);
            this.Alto = 50 + (tipo.NivelEscala * 5);

            // Decidir si usar polígono o imagen
            if (tipo.TipoPoligono > 0 && tipo.RutaImagen == null)
            {
                // Usar polígono
                Point[] rawPoints = Avion.GetShipPoints(tipo.TipoPoligono);
                NormalizarYEscalarPuntos(rawPoints);
                ActualizarPosicion();
            }
            else if (tipo.RutaImagen != null)
            {
                // Usar imagen
                try
                {
                    using (var fs = new FileStream(tipo.RutaImagen, FileMode.Open, FileAccess.Read))
                    {
                        _imagenNave = Image.FromStream(fs);
                    }
                }
                catch { _imagenNave = null; }
            }
        }

        public void RecibirDano(int cantidad)
        {
            VidaActual -= cantidad;
            if (VidaActual < 0) VidaActual = 0;
        }

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

        private void ActualizarPosicion()
        {
            if (_puntosBase == null || _puntosActuales == null) return;
            for (int i = 0; i < _puntosBase.Length; i++)
            {
                _puntosActuales[i] = new Point(_puntosBase[i].X + (int)X, _puntosBase[i].Y + (int)Y);
            }
        }

        public void MoverIzquierda() { X -= velocidad; if (X < 0) X = 0; ActualizarPosicion(); }
        public void MoverDerecha(int anchoPantalla) { X += velocidad; if (X + Ancho > anchoPantalla) X = anchoPantalla - Ancho; ActualizarPosicion(); }
        public void MoverArriba() { Y -= velocidad; if (Y < 0) Y = 0; ActualizarPosicion(); }
        public void MoverAbajo(int altoPantalla) { Y += velocidad; if (Y + Alto > altoPantalla) Y = altoPantalla - Alto; ActualizarPosicion(); }

        public void Dibujar(Graphics g)
        {
            if (_imagenNave != null)
            {
                // Dibujar imagen
                g.DrawImage(_imagenNave, X, Y, Ancho, Alto);
            }
            else if (_puntosActuales != null && _puntosActuales.Length > 1)
            {
                // Dibujar polígono
                using (SolidBrush brocha = new SolidBrush(this.color))
                using (Pen lapiz = new Pen(Color.White, 2))
                {
                    g.FillPolygon(brocha, _puntosActuales);
                    g.DrawPolygon(lapiz, _puntosActuales);
                }
            }
        }

        public Region ObtenerRegion()
        {
            // Para simplificar, la colisión siempre será rectangular
            return new Region(new Rectangle((int)X, (int)Y, Ancho, Alto));
        }
    }
}
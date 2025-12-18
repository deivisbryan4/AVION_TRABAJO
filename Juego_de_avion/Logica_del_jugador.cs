using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace JuegoDeAvion
{
    public class Avion
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public int Ancho { get; private set; } = 50;
        public int Alto { get; private set; } = 50;
        public int VidaActual { get; private set; }
        public int VidaMaxima { get; private set; }
        public Point Centro => new Point((int)(X + Ancho / 2), (int)(Y + Alto / 2));

        private int velocidad;
        private Color color;
        private Point[] _puntosBase = new Point[0];
        private Point[] _puntosActuales = new Point[0];

        public Avion(int startX, int startY)
        {
            X = startX;
            Y = startY;

            TipoAvion tipo = DatosGlobales.Aviones[DatosGlobales.IndiceAvionSeleccionado];
            
            this.velocidad = tipo.Velocidad + (tipo.NivelEscala * 2); 
            this.color = tipo.ColorPrincipal;
            this.VidaMaxima = (tipo.VidaMaxima + tipo.NivelEscala) * 100; // Vida base x 100 para tener más granularidad
            this.VidaActual = this.VidaMaxima;
            
            this.Ancho = 50 + (tipo.NivelEscala * 5);
            this.Alto = 50 + (tipo.NivelEscala * 5);

            Point[] rawPoints = DrawHelper.GetShipPoints(tipo.TipoPoligono);
            NormalizarYEscalarPuntos(rawPoints);
            ActualizarPosicion();
        }

        public void RecibirDano(int cantidad)
        {
            VidaActual -= cantidad;
            if (VidaActual < 0) VidaActual = 0;
        }

        private void NormalizarYEscalarPuntos(Point[] raw)
        {
            if (raw == null || raw.Length == 0) return;

            int minX = raw.Min(p => p.X);
            int maxX = raw.Max(p => p.X);
            int minY = raw.Min(p => p.Y);
            int maxY = raw.Max(p => p.Y);

            float anchoOriginal = Math.Max(1, maxX - minX);
            float altoOriginal = Math.Max(1, maxY - minY);

            _puntosBase = new Point[raw.Length];
            _puntosActuales = new Point[raw.Length];

            float ratioX = this.Ancho / anchoOriginal;
            float ratioY = this.Alto / altoOriginal;

            for (int i = 0; i < raw.Length; i++)
            {
                int localX = (int)((raw[i].X - minX) * ratioX);
                int localY = (int)((raw[i].Y - minY) * ratioY);
                _puntosBase[i] = new Point(localX, localY);
            }
        }

        private void ActualizarPosicion()
        {
            if (_puntosBase == null) return;
            for (int i = 0; i < _puntosBase.Length; i++)
            {
                _puntosActuales[i].X = _puntosBase[i].X + (int)X;
                _puntosActuales[i].Y = _puntosBase[i].Y + (int)Y;
            }
        }

        public void MoverIzquierda()
        {
            X -= velocidad;
            if (X < 0) X = 0;
            ActualizarPosicion();
        }

        public void MoverDerecha(int anchoPantalla)
        {
            X += velocidad;
            if (X + Ancho > anchoPantalla) X = anchoPantalla - Ancho;
            ActualizarPosicion();
        }

        public void MoverArriba()
        {
            Y -= velocidad;
            if (Y < 0) Y = 0;
            ActualizarPosicion();
        }

        public void MoverAbajo(int altoPantalla)
        {
            Y += velocidad;
            if (Y + Alto > altoPantalla) Y = altoPantalla - Alto;
            ActualizarPosicion();
        }

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

        public Region ObtenerRegion()
        {
            if (_puntosActuales == null || _puntosActuales.Length < 3)
                return new Region(new Rectangle((int)X, (int)Y, Ancho, Alto));

            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(_puntosActuales);
            return new Region(path);
        }
    }
}
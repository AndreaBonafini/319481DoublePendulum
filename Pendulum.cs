using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;

namespace _319481CulminatingProject
{
    class Pendulum
    {
        //Physics Constants
        private double FinalAngle1 = 0;
        private double FinalAngle2 = 0;
        private double InitialAngle1 = 0;
        private double InitialAngle2 = 0;
        public double Phi1 = 0 * (Math.PI) / 2;
        public double Phi2 = 2.3 * (Math.PI) / 2;
        public double M1 = 10;
        public double M2 = 10;
        private double L1 = 150;
        private double L2 = 150;
        private double X0 = 350;
        private double Y0 = 60;
        private double gravity = 9.8;
        private double time = 0.02;

        private Line Arm1 = new Line { StrokeThickness = 5, Stroke = Brushes.Red };
        private Line Arm2 = new Line { StrokeThickness = 5, Stroke = Brushes.Red };
        private Ellipse mass1 = new Ellipse { Fill = Brushes.Black };
        private Ellipse mass2 = new Ellipse { Fill = Brushes.Black };

        private Canvas _canvas;
        private InkCanvas _ink;
        public StylusPointCollection col;
        public Stroke stroke;

        public Pendulum(Canvas canvas, InkCanvas ink)
        {
            _canvas = canvas;
            _ink = ink;

            canvas.Children.Add(Arm1);
            canvas.Children.Add(Arm2);
            canvas.Children.Add(mass1);
            canvas.Children.Add(mass2);
        }

        private void SetSize(FrameworkElement element, double size)
        {
            element.Width = size;
            element.Height = size;
        }

        public void SetPosition(UIElement element, double x, double y)
        {
            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
        }

        public void SetPosition(Line line, double x1, double y1, double x2, double y2)
        {
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
        }

        public void Update()
        {
            double Mass1X = X0 + L1 * Math.Sin(Phi1);
            double Mass1Y = Y0 + L1 * Math.Cos(Phi1);
            double Mass2X = Mass1X + L2 * Math.Sin(Phi2);
            double Mass2Y = Mass1Y + L2 * Math.Cos(Phi2);
            SetSize(mass1, 2 * M1);
            SetSize(mass2, 2 * M2);
            SetPosition(mass1, Mass1X - M1, Mass1Y - M1);
            SetPosition(mass2, Mass2X - M2, Mass2Y - M2);
            SetPosition(Arm1, X0, Y0, Mass1X, Mass1Y);
            SetPosition(Arm2, Mass1X, Mass1Y, Mass2X, Mass2Y);

            var sp = new StylusPoint(Mass2X, Mass2Y);
            if (col == null || col.Count >= 1000)
            {
                var ncol = new StylusPointCollection();
                if (col != null)
                    ncol.Add(col.Last());
                ncol.Add(sp);
                stroke = new Stroke(ncol);
                stroke.DrawingAttributes.Color = Colors.Blue;
                _ink.Strokes.Add(stroke);
                col = ncol;
            }
            else
            {
                col.Add(sp);
            }
        }

        public void Animate()
        {
            lock (this)
            {
                double mu = 1 + M1 / M2;
                FinalAngle1 = Calc_d2Phi1(mu);
                FinalAngle2 = Calc_d2Phi2(mu);
                InitialAngle1 += FinalAngle1 * time;
                InitialAngle2 += FinalAngle2 * time;
                Phi1 += InitialAngle1 * time;
                Phi2 += InitialAngle2 * time;
                Update();
            }
        }

        private double Calc_d2Phi1(double mu)
        {
            return
                (gravity * (Math.Sin(Phi2) * Math.Cos(Phi1 - Phi2) - mu * Math.Sin(Phi1)) - (L2 * InitialAngle2 * InitialAngle2 + L1 *
                    InitialAngle1 * InitialAngle1 * Math.Cos(Phi1 - Phi2)) * Math.Sin(Phi1 - Phi2))
                /
                (L1 * (mu - Math.Cos(Phi1 - Phi2) * Math.Cos(Phi1 - Phi2)));
        }

        private double Calc_d2Phi2(double mu)
        {
            return
                (mu * gravity * (Math.Sin(Phi1) * Math.Cos(Phi1 - Phi2) - Math.Sin(Phi2)) + (mu * L1 * InitialAngle1 * InitialAngle1
                    + L2 * InitialAngle2 * InitialAngle2 * Math.Cos(Phi1 - Phi2)) * Math.Sin(Phi1 - Phi2))
                /
                (L2 * (mu - Math.Cos(Phi1 - Phi2) * Math.Cos(Phi1 - Phi2)));
        }
    }
}

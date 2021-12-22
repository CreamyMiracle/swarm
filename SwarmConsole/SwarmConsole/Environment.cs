using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SwarmConsole
{
    public class Environment
    {
        #region Public Methods
        public Environment(int x, int y, int z, int boids)
        {
            Volume = new Rect3D(new Point3D(0, 0, 0), new Size3D(x, y, z));
            Center = new Point3D(x / 2, y / 2, z / 2);
            GenerateBoids(boids);
        }
        public async Task Run()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    foreach (Boid boid in Boids)
                    {
                        boid.Advance();
                    }
                    Draw();
                    Thread.Sleep(5);
                }
            });
        }
        #endregion

        #region Private Methods
        private void GenerateBoids(int count)
        {
            int minX = Convert.ToInt32(Volume.X);
            int maxX = Convert.ToInt32(Volume.SizeX);

            int minY = Convert.ToInt32(Volume.Y);
            int maxY = Convert.ToInt32(Volume.SizeY);

            int minZ = Convert.ToInt32(Volume.Z);
            int maxZ = Convert.ToInt32(Volume.SizeZ);

            for (int i = 0; i < count; i++)
            {
                int px = rnd.Next(minX, maxX);
                int py = rnd.Next(minY, maxY);
                int pz = 0;//rnd.Next(minZ, maxZ);

                int dx = rnd.Next(-maxX, maxX);
                int dy = rnd.Next(-maxY, maxY);
                int dz = 0;//rnd.Next(-minZ, maxZ);

                Point3D pos = new Point3D(px, py, pz);
                Vector3D dir = new Vector3D(dx, dy, dz);
                dir.Normalize();
                Boid boid = new Boid(pos, dir, this);

                boid.Symbol = symbols[rnd.Next(symbols.Length)];

                Boids.Add(boid);
            }
        }
        private void Draw()
        {
            int width = Convert.ToInt32(Volume.SizeX);
            int height = Convert.ToInt32(Volume.SizeY);
            int depth = Convert.ToInt32(Volume.SizeZ);

            string world = default;
            string line = default;

            Boid dummyBoid = default;
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    dummyBoid = new Boid(new Point3D(w, h, 0), new Vector3D(), this);
                    IEnumerable<Boid> matching = Boids.Where(b => b.Distance(dummyBoid) <= b.Radius);
                    if (matching.Any())
                    {
                        line += " " + matching.First().Symbol;
                    }
                    else
                    {
                        line += " .";
                    }
                }
                world += line + "\n";
                line = "";
            }
            //Console.Clear();
            Console.WriteLine(world);
        }
        #endregion

        #region Public Properties
        public List<Boid> Boids { get; set; } = new List<Boid>();
        public Rect3D Volume { get; set; }
        public Point3D Center { get; set; }
        #endregion

        #region Private Fields
        private Random rnd = new Random();
        private string symbols = "X";
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SwarmConsole
{
    public class Boid
    {
        #region Public Methods
        public Boid(Point3D position, Vector3D direction, Environment env)
        {
            Position = position;
            Direction = direction;
            Env = env;
        }
        public void Advance()
        {
            DecideDirection();
            Move();
        }
        public double Distance(Boid other)
        {
            double juu = Math.Sqrt(Math.Pow((other.Position.X - this.Position.X), 2)
                                 + Math.Pow((other.Position.Y - this.Position.Y), 2)
                                 + Math.Pow((other.Position.Z - this.Position.Z), 2));
            return juu;
        }
        #endregion

        #region Private Methods
        private void Move()
        {
            Position = Vector3D.Add(Direction * Speed, Position);

            Point3D newPos = Position;
            if (Env.Volume.SizeX < Position.X)
            {
                newPos.X = Position.X - Env.Volume.SizeX;
            }
            else if (Env.Volume.X > Position.X)
            {
                newPos.X = Position.X + Env.Volume.SizeX;
            }

            if (Env.Volume.SizeY < Position.Y)
            {
                newPos.Y = Position.Y - Env.Volume.SizeY;
            }
            else if (Env.Volume.Y > Position.Y)
            {
                newPos.Y = Position.Y + Env.Volume.SizeY;
            }

            if (Env.Volume.SizeZ < Position.Z)
            {
                newPos.Z = Position.Z - Env.Volume.SizeZ;
            }
            else if (Env.Volume.Z > Position.Z)
            {
                newPos.Z = Position.Z + Env.Volume.SizeZ;
            }

            Position = newPos;
        }

        private void DecideDirection()
        {
            CalculateNeighbors();

            // Calculate such direction that we are not colliding with other boids in this environment
            CollisionDirectionSteer();
            CommonDirectionSteer();
            SwarmCenterDirectionSteer();
            EnvCollisionSteer();
        }

        private void EnvCollisionSteer()
        {
            double margin = VisionRange / 5;
            bool contained = (Env.Volume.X + margin <= Position.X &&
                               Env.Volume.Y + margin <= Position.Y &&
                               //Env.Volume.Z + margin <= Position.Z &&

                               Env.Volume.SizeX - margin > Position.X &&
                               Env.Volume.SizeY - margin > Position.Y);
            //Env.Volume.SizeZ - margin >= boidVolume.Z + boidVolume.SizeZ);

            // if Boid's volume is partially outside the environment's volume
            if (!contained)
            {
                Vector3D awayDirComponent = Env.Center - Position;
                awayDirComponent.Z = 0;
                Vector3D scaledComponent = awayDirComponent * 0.1;
                Vector3D newDir = Direction + scaledComponent;
                newDir.Normalize();
                Direction = newDir;
            }
        }

        private void CollisionDirectionSteer()
        {
            if (NearestNeighbor != null)
            {
                double dist = Distance(NearestNeighbor);

                if (dist <= DodgeRange)
                {
                    Vector3D awayDirComponent = Position - NearestNeighbor.Position;
                    Vector3D scaledComponent = awayDirComponent * ((1 - dist / DodgeRange));
                    Vector3D newDir = Direction + scaledComponent;
                    newDir.Normalize();
                    Direction = newDir;
                }
            }
        }

        private void CommonDirectionSteer()
        {
            Vector3D commonDirComponent = new Vector3D();
            foreach (Boid neighbor in Neighbors)
            {
                commonDirComponent += neighbor.Direction;
            }

            Vector3D scaledComponent = commonDirComponent * CommonDirRate;
            Vector3D newDir = Direction + scaledComponent;
            newDir.Normalize();
            Direction = newDir;
        }

        private void SwarmCenterDirectionSteer()
        {
            if (Neighbors.Any())
            {
                List<Point3D> neighborCenters = Neighbors.Select(n => n.Position).ToList();
                Point3D swarmCenterPoint = new Point3D(neighborCenters.Average(p => p.X),
                                                       neighborCenters.Average(p => p.Y),
                                                       neighborCenters.Average(p => p.Z));

                Vector3D centerComponent = swarmCenterPoint - Position;

                Vector3D scaledComponent = centerComponent * CommonCenterRate;
                Vector3D newDir = Direction + scaledComponent;
                newDir.Normalize();
                Direction = newDir;
            }
        }

        private void CalculateNeighbors()
        {
            Neighbors.Clear();
            NearestNeighbor = null;
            double nearestDist = double.MaxValue;

            foreach (Boid boid in Env.Boids)
            {
                double dist = Distance(boid);
                if (boid != this && dist <= VisionRange)
                {
                    if (dist < nearestDist)
                    {
                        NearestNeighbor = boid;
                    }

                    Neighbors.Add(boid);
                }
            }
        }
        #endregion

        #region Public Properties
        public Point3D Position { get; set; }
        public Vector3D Direction { get; set; }
        public double Speed { get; set; } = 0.5;
        public double VisionRange { get; set; } = 10;
        public double DodgeRange { get; set; } = 5;
        public double CommonDirRate { get; set; } = 0.03;
        public double CommonCenterRate { get; set; } = 0.02;
        public double Radius { get; set; } = 1.1;
        public char Symbol { get; set; } = 'X';
        public List<Boid> Neighbors { get; set; } = new List<Boid>();
        public Boid NearestNeighbor { get; set; } = null;
        #endregion

        #region Private Fields
        private Environment Env;
        #endregion
    }
}

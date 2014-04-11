using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment_3D
{
    class Collider
    {
        public enum Face { LEFT, RIGHT, BOTTOM, TOP, BACK, FRONT, NONE }

        public Face DetermineSide(BoundingBox box, Ray ray)
        {
            Face selectedFace = Face.NONE;
            float closestDist = float.MaxValue;
            BoundingBox[] sides =
            {
                new BoundingBox(new Vector3(box.Min.X, box.Min.Y, box.Min.Z), new Vector3(box.Min.X, box.Max.Y, box.Max.Z)), //-x LEFT
                new BoundingBox(new Vector3(box.Max.X, box.Min.Y, box.Min.Z), new Vector3(box.Max.X, box.Max.Y, box.Max.Z)), //+x RIGHT
                new BoundingBox(new Vector3(box.Min.X, box.Min.Y, box.Min.Z), new Vector3(box.Max.X, box.Min.Y, box.Max.Z)), //-y TOP
                new BoundingBox(new Vector3(box.Min.X, box.Max.Y, box.Min.Z), new Vector3(box.Max.X, box.Max.Y, box.Max.Z)), //+y BOTTOM
                new BoundingBox(new Vector3(box.Min.X, box.Min.Y, box.Min.Z), new Vector3(box.Max.X, box.Max.Y, box.Min.Z)), //-z BACK
                new BoundingBox(new Vector3(box.Min.X, box.Min.Y, box.Max.Z), new Vector3(box.Max.X, box.Max.Y, box.Max.Z))  //+z FRONT
            };

            for (int i = 0; i < sides.Length; ++i)
            {
                float? d = ray.Intersects(sides[i]);
                if (d.HasValue)
                {
                    if (d.Value < closestDist)
                    {
                        closestDist = d.Value;
                        selectedFace = (Face)i;
                    }
                }
            }
            return selectedFace;
        }

        public Face DetermineSide(BoundingBox box, int radius, Camera cam)
        {
            Vector3 down = cam.GetPosition + new Vector3(0, -radius, 0);
            Vector3 up = cam.GetPosition + new Vector3(0, radius, 0);
            Vector3 left = cam.GetPosition + new Vector3(-radius, 0, 0);
            Vector3 right = cam.GetPosition + new Vector3(radius, 0, 0);
            Vector3 forward = cam.GetPosition + new Vector3(0, 0, -radius);
            Vector3 backward = cam.GetPosition + new Vector3(0, 0, radius);

            Ray downRay = new Ray(cam.GetPosition, down);
            Ray upRay = new Ray(cam.GetPosition, up);
            Ray leftRay = new Ray(cam.GetPosition, left);
            Ray rightRay = new Ray(cam.GetPosition, right);
            Ray frontRay = new Ray(cam.GetPosition, forward);
            Ray backRay = new Ray(cam.GetPosition, backward);

            float? downI = downRay.Intersects(box);
            float? upI = upRay.Intersects(box);
            float? leftI = leftRay.Intersects(box);
            float? rightI = rightRay.Intersects(box);
            float? frontI = frontRay.Intersects(box);
            float? backI = backRay.Intersects(box);

            if (downI != null && downI.Value < 0.01f)
                return Face.BOTTOM;

            if (upI != null && upI.Value < 0.01f)
                return Face.TOP;

            if (frontI != null && frontI.Value < 0.01f)
                return Face.FRONT;

            if (backI != null && backI.Value < 0.01f)
                return Face.BACK;

            if (leftI != null && leftI.Value < 0.01f)
                return Face.LEFT;

            if (rightI != null && rightI.Value < 0.01f)
                return Face.RIGHT;

            return Face.NONE;
        }

        public bool isColliding(BoundingBox box, Camera cam, Vector3 playerDimensions)
        {
            BoundingBox cameraBox = new BoundingBox(
                new Vector3(cam.GetPosition.X - (playerDimensions.X / 2), cam.GetPosition.Y - (playerDimensions.Y), cam.GetPosition.Z - (playerDimensions.Z / 2)),
                new Vector3(cam.GetPosition.X + (playerDimensions.X / 2), cam.GetPosition.Y, cam.GetPosition.Z + (playerDimensions.Z / 2))
            );

            return box.Contains(cameraBox) != ContainmentType.Disjoint;
        }

        public bool isColliding(List<BoundingBox> boxes, Camera cam, Vector3 playerDimensions)
        {
            BoundingBox cameraBox = new BoundingBox(
                new Vector3(cam.GetPosition.X - (playerDimensions.X / 2), cam.GetPosition.Y - (playerDimensions.Y), cam.GetPosition.Z - (playerDimensions.Z / 2)),
                new Vector3(cam.GetPosition.X + (playerDimensions.X / 2), cam.GetPosition.Y, cam.GetPosition.Z + (playerDimensions.Z / 2))
            );

            foreach (BoundingBox box in boxes)
            {
                if (box.Contains(cameraBox) != ContainmentType.Disjoint)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool PointInCube(BoundingBox cube, Vector3 point)
        {
            return CubeContainsPoint(cube, point) == ContainmentType.Contains;
        }

        public static ContainmentType CubeContainsPoint(BoundingBox cube, Vector3 point)
        {
            bool result = (cube.Min.X <= point.X && cube.Max.X >= point.X && cube.Min.Y <= point.Y && cube.Max.Y >= point.Y && cube.Min.Z <= point.Z && cube.Max.Z >= point.Z);
            return result ? ContainmentType.Contains : ContainmentType.Disjoint;
        }

        public static bool Intersects(BoundingBox box, BoundingSphere sphere)
        {
            Vector3 vector = Vector3.Clamp(sphere.Center, box.Min, box.Max);
            float distance = Vector3.DistanceSquared(sphere.Center, vector);
            return distance <= sphere.Radius * sphere.Radius;
        }

        public static bool Intersects(BoundingSphere sphereOne, BoundingSphere sphereTwo)
        {
            float sum = sphereOne.Radius + sphereTwo.Radius;
            return Vector3.DistanceSquared(sphereOne.Center, sphereTwo.Center) <= (sum * sum);
        }

        public static bool Intersects(BoundingBox boxOne, BoundingBox boxTwo)
        {
            bool result = boxOne.Max.X >= boxTwo.Min.X && boxOne.Min.X <= boxTwo.Max.X;
            result = result && boxOne.Max.Y >= boxTwo.Min.Y && boxOne.Min.Y <= boxTwo.Max.Y;
            result = result && boxOne.Max.Z >= boxTwo.Min.Z && boxOne.Min.Z <= boxTwo.Max.Z;
            return result;
        }

        public static BoundingBox Transform(BoundingBox box, Matrix world)
        {
            var vertices = box.GetCorners();
            var minVertex = new Vector3(float.MaxValue);
            var maxVertex = new Vector3(float.MinValue);

            foreach (Vector3 vertex in vertices)
            {
                var transformedVertex = Vector3.Transform(vertex, world);
                minVertex = Vector3.Min(minVertex, transformedVertex);
                maxVertex = Vector3.Max(maxVertex, transformedVertex);
            }

            return new BoundingBox(minVertex, maxVertex);
        }

        public static BoundingBox CreateBoundingBox(Model model, Matrix world)
        {
            BoundingBox result = new BoundingBox();
            foreach (ModelMesh mesh in model.Meshes)
            {
                result = BoundingBox.CreateMerged(result, BoundingBox.CreateFromSphere(mesh.BoundingSphere.Transform(world)));
            }
            return result;
        }
    }
}

using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Helpers.ModellingHelpers
{
    // MODULE: GUM CLOSER (multi-ring organic gum + U-shaped cap)
    public static class GumCloser
    {
        /// <summary>
        /// Cierra el mesh con una encía de varias capas:
        /// - Suaviza primero el loop de borde (gum edge).
        /// - Genera varios "anillos de encía" entre el borde suavizado y una altura base.
        /// - Cada anillo tiene variación radial y vertical suave (ondas bajas).
        /// - Conecta: borde original → ring0 → ring1 → ... → ringN → tapa U (sin vértice central).
        /// </summary>


        public static DMesh3 CloseWithGum(
            DMesh3 input,
            bool isUpper,
            double gumHeight,
            double baseOffset,
            double radialInset,
            int smoothIterations,
            int edgeSmoothIterations)
        {
            var mesh = new DMesh3(input);
            if (gumHeight <= 0)
                return mesh;


            //1) Bounding box y

            double minY = double.MaxValue;
            double maxY = double.MinValue;

            for (int vid = 0; vid < mesh.MaxVertexID; vid++)
            {
                if (!mesh.IsVertex(vid)) continue;
                var v = mesh.GetVertex(vid);
                if (v.y < minY) minY = v.y;
                if (v.y > maxY) maxY = v.y;
            }

            if (double.IsInfinity(minY) || double.IsInfinity(maxY))
                return mesh;

            double sign = isUpper ? +1.0 : -1.0;
            double baseY = isUpper ? (maxY + baseOffset) : (minY - baseOffset);
            double targetY = baseY + sign * gumHeight;

            //2) Boundary loops

            var boundary = new MeshBoundaryLoops(mesh);
            if (boundary.Loops == null || boundary.Loops.Count == 0)
                return mesh;

            foreach (var loop in boundary.Loops)
            {
                IList<int> loopVerts = loop.Vertices;
                int n = loopVerts.Count;
                if (n < 3) continue;

                //Guardarmos posiciones originales de borde
                var loopPos = new Vector3d[n];
                Vector3d center = Vector3d.Zero;
                for (int i = 0; i < n; i++)
                {
                    var v = mesh.GetVertex(loopVerts[i]);
                    loopPos[i] = v;
                    center += v;
                }
                center /= n;

                //Suavizamos el borde antes de extruir (quita serrucho y escalones)
                SmoothRing(loopPos, edgeSmoothIterations);

                // --------------------------------------------
                // Multi-ring: varios anillos entre borde suavizado y base
                // --------------------------------------------

                int numRings = 4; // más anillos = encía más redonda
                Vector3d[][] ringPositions = new Vector3d[numRings][];
                for (int r = 0; r < numRings; r++)
                    ringPositions[r] = new Vector3d[n];

                //ajusta estos para más/menos organicidad
                int radialFreq1 = 2;
                int radialFreq2 = 5;
                int verticalFreq1 = 2;
                int verticalFreq2 = 4;

                for (int i = 0; i < n; ++i)
                {
                    var vEdge = loopPos[i]; // ya suavizado

                    //vector horizontla desde el centro
                    var horiz = new Vector3d(vEdge.x - center.x, 0, vEdge.z - center.z);
                    double horizLenSq = horiz.LengthSquared;
                    if (horizLenSq > 1e-12)
                        horiz.Normalize();
                    else
                        horiz = new Vector3d(1, 0, 0); //fallback

                    double angle = Math.Atan2(vEdge.z - center.z, vEdge.x - center.x);

                    for (int r = 0; r < numRings; ++r)
                    {
                        //t: qué tan lejos estamos del borde hacia la base (0..1)
                        double t = (r + 1.0) / (numRings + 1.0); // e.g. 0.2, 0.4, 0.6, 0.8

                        //interpolamos Y entre borde y altura objetivo
                        double yLerp = (1.0 - t) * vEdge.y + t * targetY;

                        // amplitud de ruido varía por anillo
                        double radialInsetBase = radialInset * gumHeight * t;
                        double radialNoiseAmp = radialInset * gumHeight * (0.10 + 0.05 * r);
                        double verticalNoiseAmp = gumHeight * (0.06 + 0.04 * r);

                        // ruido determinístico para romper periodicidad perfecta
                        double angleNoiseSeed = Math.Sin(angle * 3.73 + r * 1.91);
                        double angleNoise = angleNoiseSeed * 0.5 + 0.5; // 0..1

                        //ruido radial suave (combinación de frecuencias bajas)
                        double radialNoise =
                            radialNoiseAmp *
                            (0.6 * Math.Sin(radialFreq1 * angle + 0.5 * r) +
                            0.4 * Math.Sin(radialFreq2 * angle + 1.3 * r)) *
                            (0.7 + 0.6 * angleNoise);

                        //ruido vertical suave
                        double verticalNoise =
                            verticalNoiseAmp *
                            (0.6 * Math.Cos(verticalFreq1 * angle + 0.2 * r) +
                            0.4 * Math.Sin(verticalFreq2 * angle + 0.9 * r)) *
                            (0.7 + 0.6 * angleNoise);

                        //total inset hacia dentro (+ ruido)
                        double insetAmount = -radialInsetBase * radialNoise;
                        var inset = horiz * insetAmount;

                        double y = yLerp + sign * verticalNoise;

                        ringPositions[r][i] = new Vector3d(
                            vEdge.x + inset.x,
                            y,
                            vEdge.z + inset.z
                        );
                    }
                }

                // Suavizado de cada anillo para quitar artefactos duros
                for (int r = 0; r < numRings; ++r)
                    SmoothRing(ringPositions[r], smoothIterations);


                // Añadir vértices al mesh y guardar sus índices
                int[][] ringIndices = new int[numRings][];
                for (int r = 0; r < numRings; ++r)
                {
                    ringIndices[r] = new int[n];
                    for (int i = 0; i < n; ++i)
                        ringIndices[r][i] = mesh.AppendVertex(ringPositions[r][i]);
                }

                // 3) Conectar: borde original -> ring0 -> ring1 -> ... -> ringN-1
                // Borde original (diente) -> primer anillo suavizado
                ConnectLoops(mesh, loopVerts, ringIndices[0], isUpper);

                //anillos intermedios
                for (int r = 0; r < numRings - 1; ++r)
                    ConnectLoops(mesh, ringIndices[r], ringIndices[r + 1], isUpper);

                // 4) Tapa U usando triangulación por "ear clipping"
                int[] lastRing = ringIndices[numRings - 1];

                // Proyectamos el último anillo al plano XZ
                var lastRing2D = new List<Vector2d>(n);
                for (int i = 0; i < n; ++i)
                {
                    var v = ringPositions[numRings - 1][i];
                    lastRing2D.Add(new Vector2d(v.x, v.z));
                }

                var capTris = EarClipTriangulate(lastRing2D);

                foreach (var tri in capTris)
                {
                    int a = lastRing[tri.a];
                    int b = lastRing[tri.b];
                    int c = lastRing[tri.c];

                    if (isUpper)
                        mesh.AppendTriangle(a, b, c);
                    else
                        mesh.AppendTriangle(a, c, b); // invert orientation for lower
                }
            }

            return mesh;

        }


        /// <summary>
        /// Conecta dos loops del mismo tamaño (A -> B) con quads triangulados.
        /// </summary>

        private static void ConnectLoops(DMesh3 mesh, IList<int> loopA, IList<int> loopB, bool isUpper)
        {
            int n = loopA.Count;
            if (loopB.Count != n) return;

            for (int i = 0; i < n; ++i)
            {
                int iNext = (i + 1) % n;

                int a0 = loopA[i];
                int a1 = loopA[iNext];
                int b0 = loopB[i];
                int b1 = loopB[iNext];

                if (isUpper)
                {
                    mesh.AppendTriangle(a0, a1, b1);
                    mesh.AppendTriangle(a0, b1, b0);
                }
                else
                {
                    mesh.AppendTriangle(a0, b1, a1);
                    mesh.AppendTriangle(a0, b0, b1);
                }

            }
        }

        /// <summary>
        /// Suavizado circular sencillo: mezcla cada punto con sus vecinos.
        /// </summary>

        private static void SmoothRing(Vector3d[] ring, int iterations)
        {
            int n = ring.Length;
            if (n < 3 || iterations <= 0) return;

            var tmp = new Vector3d[n];

            for (int it = 0; it < iterations; ++it)
            {
                for (int i = 0; i < n; ++i)
                    tmp[i] = ring[i];

                for (int i = 0; i < n; ++i)
                {
                    int iPrev = (i - 1 + n) % n;
                    int iNext = (i + 1) % n;

                    ring[i] = 0.3 * tmp[i] + 0.35 * tmp[iPrev] + 0.35 * tmp[iNext];
                }
            }
        }

        // ----------------------------------------------------
        // Ear-clipping triangulation for a simple polygon
        // ----------------------------------------------------

        private static List<Index3i> EarClipTriangulate(IList<Vector2d> poly)
        {
            int n = poly.Count;
            var result = new List<Index3i>();
            if (n < 3) return result;

            var V = new List<int>(n);
            for (int i = 0; i < n; ++i) V.Add(i);

            // Compute signed area to detect orientation
            double area = 0;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                var pi = poly[i];
                var pj = poly[j];
                area += (pj.x * pi.y - pi.x * pj.y);
            }

            bool wantCCW = area > 0;

            int guard = 0;

            while (V.Count > 3 && guard < 10000)
            {
                guard++;
                bool earFound = false;
                int m = V.Count;

                for (int i = 0; i < m; ++i)
                {
                    int iPrev = V[(i - 1 + m) % m];
                    int iCurr = V[i];
                    int iNext = V[(i + 1) % m];

                    var A = poly[iPrev];
                    var B = poly[iCurr];
                    var C = poly[iNext];

                    if (!IsConvex(A, B, C, wantCCW))
                        continue;

                    bool anyInside = false;
                    for (int j = 0; j < m; ++j)
                    {
                        if (j == (i - 1 + m) % m || j == i || j == (i + 1) % m)
                            continue;

                        int idx = V[j];
                        if (PointInTriangle(poly[idx], A, B, C))
                        {
                            anyInside = true;
                            break;
                        }
                    }

                    if (anyInside)
                        continue;

                    // This is an ear
                    result.Add(new Index3i(iPrev, iCurr, iNext));
                    V.RemoveAt(i);
                    earFound = true;
                    break;
                }
                if (!earFound)
                    break; //degenerate polygon
            }

            if (V.Count == 3)
                result.Add(new Index3i(V[0], V[1], V[2]));

            return result;
        }

        private static bool IsConvex(Vector2d a, Vector2d b, Vector2d c, bool wantCCW)
        {
            double cross = (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
            return wantCCW ? cross > 0 : cross < 0;
        }

        private static bool PointInTriangle(Vector2d p, Vector2d a, Vector2d b, Vector2d c)
        {
            //Barycentric sign method
            double d1 = Sign(p, a, b);
            double d2 = Sign(p, b, c);
            double d3 = Sign(p, c, a);

            bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(hasNeg && hasPos);

        }

        private static double Sign(Vector2d p1, Vector2d p2, Vector2d p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

    }
}

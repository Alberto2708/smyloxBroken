using g3;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace SmyloxFirstUI.Helpers.ModellingHelpers
{
    public static class MeshConverters
    {
        public static void Translate(DMesh3 mesh, Vector3d offset)
        {
            for (int vid = 0; vid < mesh.MaxVertexID; vid++)
            {
                if (!mesh.IsVertex(vid)) continue;
                mesh.SetVertex(vid, mesh.GetVertex(vid) + offset);
            }
        }

        public static MeshGeometry3D ToMeshGeometry3D(DMesh3 mesh)
        {
            var builder = new MeshBuilder(false, false);

            for (int tid = 0; tid < mesh.MaxTriangleID; tid++)
            {
                if (!mesh.IsTriangle(tid)) continue;

                var tri = mesh.GetTriangle(tid);
                var a = mesh.GetVertex(tri.a);
                var b = mesh.GetVertex(tri.b);
                var c = mesh.GetVertex(tri.c);

                builder.AddTriangle(
                    new Point3D(a.x, a.y, a.z),
                    new Point3D(b.x, b.y, b.z),
                    new Point3D(c.x, c.y, c.z));
            }
            return builder.ToMesh();
        }

        public static GeometryModel3D CreateGeometryModel(MeshGeometry3D mesh, Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();

            var mat = new DiffuseMaterial(brush);

            return new GeometryModel3D
            {
                Geometry = mesh,
                Material = mat,
                BackMaterial = mat
            };
        }
    }
}

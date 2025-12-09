using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Microsoft.Win32;
using g3;
using System.Collections.Generic;

namespace SmyloxFirstUI.Helpers.ModellingHelpers
{
    public static class MeshIO
    {
        public static DMesh3 LoadStl(string path)
        {
            if(!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            var builder = new DMesh3Builder();
            var reader = new StandardMeshReader { MeshBuilder = builder };

            var options = ReadOptions.Defaults;
            reader.Read(path, options);

            if (builder.Meshes == null || builder.Meshes.Count == 0)
                throw new InvalidOperationException("No meshes were loaded from the STL file");

            return builder.Meshes[0];
        }
    }
}

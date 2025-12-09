using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using g3;
using System.Windows.Media.Media3D;

namespace SmyloxFirstUI.Model.Modelling
{
    public enum JawType
    {
        Upper,
        Lower

    }
    public class JawMeshModel
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DMesh3? Mesh { get; set; }
        public GeometryModel3D? GeometryModel { get; set; }
        public JawType JawType { get; set; }

    }
}

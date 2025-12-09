using HelixToolkit.Wpf;
using SmyloxFirstUI.Model.Modelling;
using System.Windows;

namespace SmyloxFirstUI.Behaviours
{
    public static class HelixViewportBehaviour
    {
        #region ZoomOnMeshChanged

        public static readonly DependencyProperty ZoomOnMeshChangedProperty =
            DependencyProperty.RegisterAttached(
                "ZoomOnMeshChanged",
                typeof(bool),
                typeof(HelixViewportBehaviour),
                new PropertyMetadata(false, OnZoomOnMeshChangedChanged));

        public static bool GetZoomOnMeshChanged(DependencyObject obj)
        {
            return (bool)obj.GetValue(ZoomOnMeshChangedProperty);
        }

        public static void SetZoomOnMeshChanged(DependencyObject obj, bool value)
        {
            obj.SetValue(ZoomOnMeshChangedProperty, value);
        }

        private static void OnZoomOnMeshChangedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HelixViewport3D viewport && (bool)e.NewValue)
            {
                viewport.Loaded += (s, args) => viewport.ZoomExtents();
            }
        }

        #endregion

        #region ResetCameraCommand

        public static readonly DependencyProperty ResetCameraCommandProperty =
            DependencyProperty.RegisterAttached(
                "ResetCameraCommand",
                typeof(System.Windows.Input.ICommand),
                typeof(HelixViewportBehaviour),
                new PropertyMetadata(null, OnResetCameraCommandChanged));

        public static System.Windows.Input.ICommand GetResetCameraCommand(DependencyObject obj)
        {
            return (System.Windows.Input.ICommand)obj.GetValue(ResetCameraCommandProperty);
        }

        public static void SetResetCameraCommand(DependencyObject obj, System.Windows.Input.ICommand value)
        {
            obj.SetValue(ResetCameraCommandProperty, value);
        }

        private static void OnResetCameraCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HelixViewport3D viewport)
            {
                if (e.OldValue is System.Windows.Input.ICommand oldCommand)
                {
                    oldCommand.CanExecuteChanged -= (s, args) => HandleResetCamera(viewport);
                }

                if (e.NewValue is System.Windows.Input.ICommand newCommand)
                {
                    // Execute camera reset when command changes (i.e., when it's executed)
                    newCommand.CanExecuteChanged += (s, args) => HandleResetCamera(viewport);
                }
            }
        }

        private static void HandleResetCamera(HelixViewport3D viewport)
        {
            viewport.ResetCamera();
            viewport.ZoomExtents();
        }

        #endregion

        #region UpperMesh (for auto-zoom)

        public static readonly DependencyProperty UpperMeshProperty =
            DependencyProperty.RegisterAttached(
                "UpperMesh",
                typeof(JawMeshModel),
                typeof(HelixViewportBehaviour),
                new PropertyMetadata(null, OnMeshChanged));

        public static JawMeshModel GetUpperMesh(DependencyObject obj)
        {
            return (JawMeshModel)obj.GetValue(UpperMeshProperty);
        }

        public static void SetUpperMesh(DependencyObject obj, JawMeshModel value)
        {
            obj.SetValue(UpperMeshProperty, value);
        }

        #endregion

        #region LowerMesh (for auto-zoom)

        public static readonly DependencyProperty LowerMeshProperty =
            DependencyProperty.RegisterAttached(
                "LowerMesh",
                typeof(JawMeshModel),
                typeof(HelixViewportBehaviour),
                new PropertyMetadata(null, OnMeshChanged));

        public static JawMeshModel GetLowerMesh(DependencyObject obj)
        {
            return (JawMeshModel)obj.GetValue(LowerMeshProperty);
        }

        public static void SetLowerMesh(DependencyObject obj, JawMeshModel value)
        {
            obj.SetValue(LowerMeshProperty, value);
        }

        private static void OnMeshChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HelixViewport3D viewport && e.NewValue != null)
            {
                // Zoom to fit new mesh
                viewport.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    viewport.ZoomExtents();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        #endregion
    }
}
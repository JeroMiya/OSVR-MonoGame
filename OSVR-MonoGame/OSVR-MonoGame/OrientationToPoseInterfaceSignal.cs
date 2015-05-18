using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSVR.MonoGame
{
    /// <summary>
    /// This class acts as an adapter for an orientation interface. If you need
    /// a PoseInterface but you only have an orientation interface, this will wrap
    /// your orientation interface and act as a PoseInterface, allowing you to specify
    /// the position yourself.
    /// </summary>
    public class OrientationToPoseInterfaceSignal : IInterfaceSignal<PoseReport>
    {
        IInterfaceSignal<Quaternion> orientationSignal;
        public OrientationToPoseInterfaceSignal(IInterfaceSignal<Quaternion> orientationSignal)
        {
            if (orientationSignal == null) { throw new ArgumentNullException("orientationSignal"); }
            this.orientationSignal = orientationSignal;
            orientationSignal.ValueChanged += orientationSignal_ValueChanged;
            Position = Vector3.Zero;
        }

        /// <summary>
        /// The position to use in any PoseReports.
        /// </summary>
        public Vector3 Position { get; set; }

        private PoseReport MakePoseReport(Quaternion rotation)
        {
            return new PoseReport
            {
                Position = Position,
                Rotation = rotation,
            };
        }

        private void orientationSignal_ValueChanged(object sender, Quaternion rotation)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, MakePoseReport(rotation));
            }
        }

        public InterfaceCallbacks Interface
        {
            get { return orientationSignal.Interface; }
        }

        public void Start()
        {
            orientationSignal.Start();
        }

        public void Stop()
        {
            orientationSignal.Stop();
        }

        public PoseReport Value
        {
            get
            {
                return MakePoseReport(orientationSignal.Value);
            }
        }

        public event EventHandler<PoseReport> ValueChanged;
    }
}
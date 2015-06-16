using Microsoft.Xna.Framework;

namespace OSVR.MonoGame
{
    /// <summary>
    /// Report value for the pose callback
    /// </summary>
    public struct XnaPose
    {
        public XnaPose(Vector3 position, Quaternion rotation)
            : this()
        {
            Position = position;
            Rotation = rotation;
        }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
}

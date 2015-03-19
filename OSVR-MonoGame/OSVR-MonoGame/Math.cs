/*
 * Port of Math.cs from OSVR-Unity by Sensics, Inc.
 */
using Microsoft.Xna.Framework;

namespace OSVR
{
    namespace MonoGame
    {
        /// <summary>
        /// Class of static methods for converting from OSVR math/tracking types to MonoGame-native data types,
		/// including coordinate system change as needed.
        /// </summary>
        public class Math
        {
            public static Vector3 ConvertPosition(OSVR.ClientKit.Vec3 vec)
            {
				return new Vector3((float)vec.x, (float)vec.y, (float)vec.z);
            }

            public static Quaternion ConvertOrientation(OSVR.ClientKit.Quaternion quat)
            {
				//// Wikipedia may say quaternions are not handed, but these needed modification.
				//return new Quaternion(-(float)quat.x, -(float)quat.y, (float)quat.z, (float)quat.w);

				// JEB: not sure if these need conversion for MonoGame. Won't know until I get
				// an actual headset.
				return new Quaternion((float)quat.x, (float)quat.y, (float)quat.z, (float)quat.w);
            }

            public static Matrix ConvertPose(OSVR.ClientKit.Pose3 pose)
            {
				var ret = Matrix.CreateFromQuaternion(Math.ConvertOrientation(pose.rotation));
				ret.Translation = Math.ConvertPosition(pose.translation); // This saves one matrix multiply
				return ret;
            }
        }
    }
}
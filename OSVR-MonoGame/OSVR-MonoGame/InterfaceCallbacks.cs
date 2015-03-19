/*
 * Based loosly on InterfaceCallbacks.cs from OSVR-Unity by Sensics, Inc.
 */
using Microsoft.Xna.Framework;
using System;
using System.Runtime.InteropServices;

namespace OSVR
{
    namespace MonoGame
    {
		/// <summary>
		/// Report value for the pose callback
		/// </summary>
		public struct PoseReport
		{
			public PoseReport(Vector3 position, Quaternion rotation) : this()
			{
				Position = position;
				Rotation = rotation;
			}
			public Vector3 Position { get; set; }
			public Quaternion Rotation { get; set; }
		}

		#region Callback (delegate) types
		//public delegate void PoseMatrixCallback(string source, Matrix pose);
		//public delegate void PoseCallback(string source, PoseReport report);
		//public delegate void PositionCallback(string source, Vector3 position);
		//public delegate void OrientationCallback(string source, Quaternion rotation);
		//public delegate void ButtonCallback(string source, bool pressed);
		//public delegate void AnalogCallback(string source, float value);
		public delegate void InterfaceCallback<T>(string source, T value);
		#endregion

        /// <summary>
        /// OSVR Interface, supporting generic callbacks that provide the source path and a MonoGame-native datatype.
        /// </summary>
        public class InterfaceCallbacks
        {
			private OSVR.ClientKit.PoseCallback rawPoseMatrixCallback;
			private InterfaceCallback<Matrix> poseMatrixCallbacks;

			private OSVR.ClientKit.PoseCallback rawPoseCallback;
			private InterfaceCallback<PoseReport> poseCallbacks;

			private OSVR.ClientKit.PositionCallback rawPositionCallback;
			private InterfaceCallback<Vector3> positionCallbacks;

			private OSVR.ClientKit.OrientationCallback rawOrientationCallback;
			private InterfaceCallback<Quaternion> orientationCallbacks;

			private OSVR.ClientKit.ButtonCallback rawButtonCallback;
			private InterfaceCallback<bool> buttonCallbacks;

			private OSVR.ClientKit.AnalogCallback rawAnalogCallback;
			private InterfaceCallback<float> analogCallbacks;

			private OSVR.ClientKit.Interface iface;
			
			private OSVR.MonoGame.IClientKit clientKit;
			public OSVR.MonoGame.IClientKit ClientKit { get { return clientKit; } }

			/// <summary>
			/// The interface path you want to connect to.
			/// </summary>
			private readonly string path;
			public string Path { get { return path; } }

			public InterfaceCallbacks(string path, OSVR.MonoGame.IClientKit clientKit)
			{
				if (path == null)
				{
					throw new ArgumentNullException("path");
				}
				if(path.Length == 0)
				{
					throw new ArgumentException("path must not be blank", "path");
				}
				if(clientKit == null)
				{
					throw new ArgumentNullException("clientKit");
				}
				this.path = path;
				this.clientKit = clientKit;
			}

            public void Start()
            {
                if (null == iface)
                {
                    iface = this.clientKit.Context.getInterface(Path);
                }
            }

            /// <summary>
            /// Used in the end-of-life method, this can also be called manually to free the internal interface.
            /// </summary>
            public void Stop()
            {
                iface = null;
                poseMatrixCallbacks = null;
                poseCallbacks = null;
                positionCallbacks = null;
                orientationCallbacks = null;
                buttonCallbacks = null;
                analogCallbacks = null;
            }

			public void RegisterCallback(InterfaceCallback<Matrix> callback)
			{
				Start(); // make sure the interface is initialized.
				if (null == poseMatrixCallbacks)
				{
					poseMatrixCallbacks = callback;
					rawPoseMatrixCallback = new OSVR.ClientKit.PoseCallback(PoseMatrixCb);
					iface.registerCallback(rawPoseMatrixCallback, System.IntPtr.Zero);
				}
				else
				{
					poseMatrixCallbacks += callback;
				}
			}

            public void RegisterCallback(InterfaceCallback<PoseReport> callback)
            {
                Start(); // make sure the interface is initialized.

				if (null == poseCallbacks)
				{
					poseCallbacks = callback;
					rawPoseCallback = new OSVR.ClientKit.PoseCallback(PoseCb);
					iface.registerCallback(rawPoseCallback, System.IntPtr.Zero);
				}
				else
				{
					poseCallbacks += callback;
				}
            }

			public void RegisterCallback(InterfaceCallback<Vector3> callback)
			{
				Start(); // make sure the interface is initialized.
				if (null == positionCallbacks)
				{
					positionCallbacks = callback;
					rawPositionCallback = new OSVR.ClientKit.PositionCallback(PositionCb);
					iface.registerCallback(rawPositionCallback, System.IntPtr.Zero);
				}
				else
				{
					positionCallbacks += callback;
				}
			}

            public void RegisterCallback(InterfaceCallback<Quaternion> callback)
            {
                Start(); // make sure the interface is initialized.
                if (null == orientationCallbacks)
                {
                    orientationCallbacks = callback;
                    rawOrientationCallback = new OSVR.ClientKit.OrientationCallback(OrientationCb);
                    iface.registerCallback(rawOrientationCallback, System.IntPtr.Zero);
                }
                else
                {
                    orientationCallbacks += callback;
                }
            }

            public void RegisterCallback(InterfaceCallback<bool> callback)
            {
                Start(); // make sure the interface is initialized.
                if (null == buttonCallbacks)
                {
                    buttonCallbacks = callback;
                    rawButtonCallback = new OSVR.ClientKit.ButtonCallback(ButtonCb);
                    iface.registerCallback(rawButtonCallback, System.IntPtr.Zero);
                }
                else
                {
                    buttonCallbacks += callback;
                }
            }

            public void RegisterCallback(InterfaceCallback<float> callback)
            {
                Start(); // make sure the interface is initialized.
                if (null == analogCallbacks)
                {
                    analogCallbacks = callback;
                    rawAnalogCallback = new OSVR.ClientKit.AnalogCallback(AnalogCb);
                    iface.registerCallback(rawAnalogCallback, System.IntPtr.Zero);
                }
                else
                {
                    analogCallbacks += callback;
                }
            }

            /// These wrappers sadly have to be mostly hand-written, despite their similarity, since they convert data types
            /// and also data conventions (into Unity's left-handed coordinate system)
            #region Private wrapper callbacks/trampolines
            /// <summary>
            /// Pose (as position and orientation) wrapper callback, interfacing Managed-OSVR's signatures and more Unity-native datatypes, including coordinate system conversion.
            /// </summary>
            /// <param name="userdata">Unused</param>
            /// <param name="timestamp">Unused</param>
            /// <param name="report">Tracker pose report</param>
            private void PoseCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PoseReport report)
            {
				var poseReport = new PoseReport(
						position: Math.ConvertPosition(report.pose.translation),
						rotation: Math.ConvertOrientation(report.pose.rotation));

                if (null != poseCallbacks)
                {
					poseCallbacks(Path, poseReport);
                }
            }

            /// <summary>
            /// Pose (as a 4x4 matrix) wrapper callback, interfacing Managed-OSVR's signatures and more Unity-native datatypes, including coordinate system conversion.
            /// </summary>
            /// <param name="userdata">Unused</param>
            /// <param name="timestamp">Unused</param>
            /// <param name="report">Tracker pose report</param>
            private void PoseMatrixCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PoseReport report)
            {
                Matrix matPose = Math.ConvertPose(report.pose);
                if (null != poseMatrixCallbacks)
                {
                    poseMatrixCallbacks(Path, matPose);
                }
            }

            /// <summary>
            /// Position wrapper callback, interfacing Managed-OSVR's signatures and more Unity-native datatypes, including coordinate system conversion.
            /// </summary>
            /// <param name="userdata">Unused</param>
            /// <param name="timestamp">Unused</param>
            /// <param name="report">Tracker position report</param>
            private void PositionCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.PositionReport report)
            {
                Vector3 position = Math.ConvertPosition(report.xyz);
                if (null != positionCallbacks)
                {
                    positionCallbacks(Path, position);
                }
            }

            /// <summary>
            /// Orientation wrapper callback, interfacing Managed-OSVR's signatures and more Unity-native datatypes, including coordinate system conversion.
            /// </summary>
            /// <param name="userdata">Unused</param>
            /// <param name="timestamp">Unused</param>
            /// <param name="report">Tracker orientation report</param>
            private void OrientationCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.OrientationReport report)
            {
                Quaternion rotation = Math.ConvertOrientation(report.rotation);
                if (null != orientationCallbacks)
                {
                    orientationCallbacks(Path, rotation);
                }
            }

            /// <summary>
            /// Button wrapper callback, interfacing Managed-OSVR's signatures and more Unity-native datatypes.
            /// </summary>
            /// <param name="userdata">Unused</param>
            /// <param name="timestamp">Unused</param>
            /// <param name="report">Button report</param>
            private void ButtonCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.ButtonReport report)
            {
                bool pressed = (report.state == 1);
                if (buttonCallbacks != null)
                {
                    buttonCallbacks(Path, pressed);
                }
            }

            /// <summary>
            /// Analog wrapper callback, interfacing Managed-OSVR's signatures and more Unity-native datatypes.
            /// </summary>
            /// <param name="userdata">Unused</param>
            /// <param name="timestamp">Unused</param>
            /// <param name="report">Analog report</param>
            private void AnalogCb(System.IntPtr userdata, ref OSVR.ClientKit.TimeValue timestamp, ref OSVR.ClientKit.AnalogReport report)
            {
                float val = (float)report.state;
                if (null != analogCallbacks)
                {
                    analogCallbacks(Path, val);
                }
            }
            #endregion
        }
    }
}

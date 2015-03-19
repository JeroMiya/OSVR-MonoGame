using System;
using Microsoft.Xna.Framework;

namespace OSVR
{
    namespace MonoGame
    {
        /// <summary>
        /// Interface signal for Pose data.
        /// </summary>
        public class PoseSignal : InterfaceSignalBase<PoseReport>
        {
			OSVR.MonoGame.InterfaceCallback<PoseReport> cb;

			public PoseSignal(string path, OSVR.MonoGame.IClientKit clientKit)
				: base(path, clientKit)
			{ }

			public override void Start()
			{
				base.Start();
				cb = new MonoGame.InterfaceCallback<PoseReport>(Callback);
				Interface.RegisterCallback(cb);
			}
        }
    }
}
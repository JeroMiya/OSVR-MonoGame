using System;
using Microsoft.Xna.Framework;

namespace OSVR
{
    namespace MonoGame
    {
        /// <summary>
        /// Interface signal for Position tracking data.
        /// </summary>
        public class PositionSignal : InterfaceSignalBase<Vector3>
        {
			OSVR.MonoGame.InterfaceCallback<Vector3> cb;

			public PositionSignal(string path, OSVR.MonoGame.IClientKit clientKit)
				: base(path, clientKit)
			{ }

			public override void Start()
			{
				base.Start();
				cb = new MonoGame.InterfaceCallback<Vector3>(Callback);
				Interface.RegisterCallback(cb);
			}
        }
    }
}
using Microsoft.Xna.Framework;
using System;

namespace OSVR
{
    namespace MonoGame
    {
        /// <summary>
        /// A 'Signal' is two things - an event source for an Interface value
        /// that changes over time, and the last value reported by the device.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public interface IInterfaceSignal<T>
        {
            InterfaceCallbacks Interface { get; }
            void Start();
            void Stop();
            T Value { get; }
            event EventHandler<T> ValueChanged;
        }

        /// <summary>
        /// Base class for interface signals.
        /// </summary>
        public class InterfaceSignalBase<T> : IInterfaceSignal<T>
        {
            private readonly InterfaceCallbacks iface;
            public InterfaceCallbacks Interface { get { return iface; } }

            private readonly string path;
            public string Path { get { return path; } }

            public InterfaceSignalBase(string path, OSVR.MonoGame.IClientKit clientKit)
            {
                this.path = path;
                this.iface = new InterfaceCallbacks(Path, clientKit);
            }

            public virtual void Start()
            {
                this.iface.Start();
            }

            public virtual void Stop()
            {
                this.iface.Stop();
            }

            protected T value;
            public virtual T Value 
            { 
                get { return value; } 
                protected set
                {
                    this.value = value;
                    OnValueChanged(this, value);
                }
            }

            public event EventHandler<T> ValueChanged;
            protected virtual void OnValueChanged(object sender, T value)
            {
                if(ValueChanged != null)
                {
                    ValueChanged(this, value);
                }
            }

            protected virtual void Callback(string source, T value)
            {
                this.Value = value;
            }
        }
    }
}
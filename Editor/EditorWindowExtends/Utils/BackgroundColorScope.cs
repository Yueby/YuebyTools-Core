using System;
using UnityEngine;

namespace Yueby.EditorWindowExtends.Utils
{
    public struct BackgroundColorScope : IDisposable
    {
        private bool _disposed;
        private readonly Color _previousColor;

        public BackgroundColorScope(Color newColor)
        {
            _disposed = false;
            _previousColor = GUI.backgroundColor;
            GUI.backgroundColor = newColor;
        }

        public BackgroundColorScope(float r, float g, float b, float a = 1f) : this(new Color(r, g, b, a))
        {
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            GUI.backgroundColor = _previousColor;
        }
    }
}
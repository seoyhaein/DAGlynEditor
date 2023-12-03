using Avalonia;

namespace DAGlynEditor
{
    public sealed class SelectionHelper
    {
        private readonly DAGlynEditor _host;
        private Point _startLocation;
        private SelectionType? _selectionType;
        private bool _isRealtime;

        public SelectionHelper(DAGlynEditor host)
            => _host = host;

        /// <summary>Available selection logic.</summary>
        public enum SelectionType
        {
            /// <summary>Replaces the old selection.</summary>
            Replace,

            /// <summary>Removes items from existing selection.</summary>
            Remove,

            /// <summary>Adds items to the current selection.</summary>
            Append,

            /// <summary>Inverts the selection.</summary>
            Invert
        }
    }
}
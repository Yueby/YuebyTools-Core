using UnityEditor;

namespace Yueby.ModalWindow
{
    public class TipsWindowDrawer : ModalEditorWindowDrawer<string>
    {
        public override string Title { get; protected set; } = "Tips";
        private MessageType _messageType;

        public TipsWindowDrawer(string tip, string title = "Tips", MessageType messageType = MessageType.Info, float height = -1) : base()
        {
            _messageType = messageType;
            Title = title;

            if (height > 0)
                position.height = height;

            Data = tip;
        }

        public override void OnDraw()
        {
            EditorGUILayout.HelpBox(Data, _messageType);

        }
    }
}
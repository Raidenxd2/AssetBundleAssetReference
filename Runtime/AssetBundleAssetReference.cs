using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#if ABAR_UIE
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif
#endif

namespace com.raiden.assetbundleassetreference.Runtime
{
    [System.Serializable]
    public class AssetBundleAssetReference : ISerializationCallbackReceiver
    {
        [HideInInspector] public string BundleName;
        [HideInInspector] public string AssetName;

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] private bool IsDirty;
#pragma warning restore 0414

        public Object Asset;
#endif

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            EditorApplication.update += OnAfterDeserializeHandler;
#endif
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            UpdateReference();
#endif
        }

#if UNITY_EDITOR
        private void OnAfterDeserializeHandler()
        {
            EditorApplication.update -= OnAfterDeserializeHandler;
            UpdateReference();
        }

        private void UpdateReference()
        {
            if (Asset != null)
            {
                BundleName = AssetDatabase.GetImplicitAssetBundleName(AssetDatabase.GetAssetPath(Asset.GetInstanceID()));
                AssetName = Asset.name;
                IsDirty = true;
            }
        }
#endif
    }

#if UNITY_EDITOR && ABAR_UIE
    [CustomPropertyDrawer(typeof(AssetBundleAssetReference))]
    [CanEditMultipleObjects]
    public class AssetBundleAssetReferenceUIE : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Create property container element.
            var container = new VisualElement();

            // Create property fields.
            var assetField = new PropertyField(property.FindPropertyRelative("Asset"), property.displayName);

            var isDirtyProperty = property.FindPropertyRelative("IsDirty");
            if (isDirtyProperty.boolValue)
            {
                isDirtyProperty.boolValue = false;
            }

            // Add fields to the container.
            container.Add(assetField);

            return container;
        }
    }
#elif UNITY_EDITOR
    [CustomPropertyDrawer(typeof(AssetBundleAssetReference))]
    [CanEditMultipleObjects]
    public class AssetBundleAssetReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var objectRect = new Rect(position.x, position.y, position.width, position.height);

            EditorGUI.PropertyField(objectRect, property.FindPropertyRelative("Asset"), GUIContent.none);

            var isDirtyProperty = property.FindPropertyRelative("IsDirty");
            if (isDirtyProperty.boolValue)
            {
                isDirtyProperty.boolValue = false;
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
#endif
}
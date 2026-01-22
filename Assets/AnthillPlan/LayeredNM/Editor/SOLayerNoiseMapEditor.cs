using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AnthillPlan.LayeredNM {
    
    [CustomEditor(typeof(SOLayeredNoiseMap))]
    public class SOLayeredNoiseMapEditor : UnityEditor.Editor {
        private SOLayeredNoiseMap _targetScript;
        private VisualElement _errorMessage;
        private PropertyField _bbv;
        private SerializedProperty _displayBbv;
        
        public override VisualElement CreateInspectorGUI() {
            _targetScript = (SOLayeredNoiseMap) target;
            _targetScript?.UpdateBlackboard();
            var root = new VisualElement();
            var errorMsg = new Label();
            errorMsg.style.color = Color.crimson;
            errorMsg.style.backgroundColor = Color.black;
            errorMsg.style.fontSize = new StyleLength(20);
            errorMsg.BindProperty(serializedObject.FindProperty("errorMessage")
                .FindPropertyRelative("message"));
            root.Add(errorMsg);
            
            _bbv = new PropertyField(serializedObject.FindProperty("blackboardVariables"));
            _displayBbv = serializedObject.FindProperty("displayBbv");
            if (!_displayBbv.boolValue) {
                _bbv.style.display = DisplayStyle.None;
            }
            root.Add(_bbv);
            
            Foldout defaultInspectorFoldout = new Foldout {
                text = "Default Inspector Properties"
            };
            root.Add(defaultInspectorFoldout);
            InspectorElement.FillDefaultInspector(defaultInspectorFoldout, serializedObject, this);
            
            root.TrackSerializedObjectValue(serializedObject, OnUpdateTrackedObject);
            return root;
        }

        private void OnUpdateTrackedObject(SerializedObject aSerializedObject) {
            
            ((SOLayeredNoiseMap) target)?.UpdateBlackboard();
            serializedObject.ApplyModifiedProperties();
            if (!_displayBbv.boolValue) {
                _bbv.style.display = DisplayStyle.None;
            }
            else {
                _bbv.style.display = DisplayStyle.Flex; 
            }
            
        }
    }
    
}


using Unity.Properties;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AnthillPlan.LayeredNM {
    
    [CustomEditor(typeof(MBLayeredNoiseMap))]
    public class MBLayeredNoiseMapEditor : UnityEditor.Editor {
        private MBLayeredNoiseMap targetScript;
        private VisualElement errorMessage;
        private PropertyField bbv;
        private SerializedProperty displayBbv;
        
        public override VisualElement CreateInspectorGUI() {
            targetScript = (MBLayeredNoiseMap) target;
            targetScript?.UpdateBlackboard();
            var root = new VisualElement();
            var errorMsg = new Label();
            errorMsg.style.color = Color.red;
            errorMsg.BindProperty(serializedObject.FindProperty("errorMessage")
                .FindPropertyRelative("message"));
            root.Add(errorMsg);
            
            bbv = new PropertyField(serializedObject.FindProperty("blackboardVariables"));
            displayBbv = serializedObject.FindProperty("displayBbv");
            if (!displayBbv.boolValue) {
                bbv.style.display = DisplayStyle.None;
            }
            root.Add(bbv);
            
            Button runGraphButton = new Button(() => { targetScript.RunGraph(); });
            runGraphButton.text = "Run Graph";
            root.Add(runGraphButton);

            Foldout defaultInspectorFoldout = new Foldout {
                text = "Default Inspector Properties"
            };
            root.Add(defaultInspectorFoldout);
            InspectorElement.FillDefaultInspector(defaultInspectorFoldout, serializedObject, this);
            
            root.TrackSerializedObjectValue(serializedObject, OnUpdateTrackedObject);
            return root;
        }

        private void OnUpdateTrackedObject(SerializedObject aSerializedObject) {
            
            ((MBLayeredNoiseMap) target)?.UpdateBlackboard();
            serializedObject.ApplyModifiedProperties();
            //((MBLayeredNoiseMap) target)?.UpdateBlackboard();// this should not be needed but serialization sometimes fails without it
            if (!displayBbv.boolValue) {
                bbv.style.display = DisplayStyle.None;
            }
            else {
                bbv.style.display = DisplayStyle.Flex; 
            }
            
        }
    }
    
}


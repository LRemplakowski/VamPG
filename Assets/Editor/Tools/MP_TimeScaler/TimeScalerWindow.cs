namespace SunsetSystems.Editor
{
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class TimeScalerWindow : OdinEditorWindow
    {
        [SerializeField, HideInInspector] private float max = 2f;
        [SerializeField, HideInInspector] private float scale = 1f;
        [SerializeField, HideInInspector] private float step = 0.1f;

        public void CreateGUI()
        {
            //VisualElement visualElement = new Box();
            //rootVisualElement.Add(visualElement);
            //EditorGUI.BeginChangeCheck();
            //Slider slider = new(0f, 20f);
            //slider.value = 1f;
            //Label label = new($"Time Scale = {slider.value}");
            //visualElement.Add(label);
            //visualElement.Add(slider);
            //if (EditorGUI.EndChangeCheck())
            //{
            //    label.text = $"Time Scale = {slider.value}";
            //    Time.timeScale = slider.value;
            //}
        }

        private float Round(float _value, float _step)
        {
            int _stepsCount = Mathf.RoundToInt(_value / _step);
            return _step * _stepsCount;
        }

        [MenuItem("Tools/Time Scaler", priority = 66)]
        public static void ShowWindow()
        {
            TimeScalerWindow window = GetWindow<TimeScalerWindow>();
            window.titleContent = new("Time Scaler");
        }
    }
}
namespace MegaPixel.Tools.TimeScale
{
    using UnityEditor;
    using UnityEngine;

    public class MP_TimeScalerWindow : MP_EditorWindow
    {
        public override string WindowName => "Time Scaler";
        public override string WindowVersion => "0.1.0";
        public override string Author => "Some guy from FER";
        public override MP_Date LastUpdateDate => new MP_Date(new System.DateTime(2022, 11, 18));
        public override Vector2 MinWindowSize => new Vector2(400f, 300f);

        [SerializeField, HideInInspector] private float max = 2f;
        [SerializeField, HideInInspector] private float scale = 1f;
        [SerializeField, HideInInspector] private float step = 0.1f;

        protected override void drawWindowGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Time Scale =", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
            GUILayout.Label($"{Time.timeScale}", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            scale = GUILayout.HorizontalSlider(Time.timeScale, 0f, max);

            if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)))
            {
                scale = 1f;
            }
            else
            {
                scale = round(scale, step);
            }

            GUILayout.EndHorizontal();

            max = EditorGUILayout.FloatField("Max", max);
            step = EditorGUILayout.FloatField("Step", step);

            if (Time.timeScale != scale)
            {
                Time.timeScale = scale;
            }
        }

        private float round(float _value, float _step)
        {
            int _stepsCount = Mathf.RoundToInt(_value / _step);
            return _step * _stepsCount;
        }

        [MenuItem("MegaPixel/Time Scaler", priority = 66)]
        public static void ShowWindow()
        {
            GetWindow(typeof(MP_TimeScalerWindow));
        }
    }
}
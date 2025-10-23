namespace RoadWearToolToggle {
    using Colossal.Serialization.Entities;
    using Game;
    using Game.Rendering;
    using Game.Tools;
    using UnityEngine;

    public partial class RoadWearToggleSystem : GameSystemBase {
        private RenderingSystem m_RenderingSystem;
        private ToolSystem      m_ToolSystem;
        private Shader          m_Shader;

        /// <inheritdoc/>
        protected override void OnCreate() {
            m_RenderingSystem             =  World.GetExistingSystemManaged<RenderingSystem>();
            m_ToolSystem                  =  World.GetExistingSystemManaged<ToolSystem>();
            m_Shader                      =  Shader.Find("BH/Decals/CurvedDecalDeteriorationShader");
            m_ToolSystem.EventToolChanged += OnToolChanged;

            RoadWearToolToggleMod.Instance.Log.Debug("OnCreate()");
        }

        protected override void OnUpdate() { }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode) {
            var shouldToggle = RoadWearToolToggleMod.Instance.Settings.Enabled;

            if (shouldToggle) {
                m_RenderingSystem.SetShaderEnabled(m_Shader, false);
            }
        }

        private void OnToolChanged(ToolBaseSystem tool) {
            if (tool?.toolID == null) {
                return;
            }

            RoadWearToolToggleMod.Instance.Log.Debug("OnUpdate()");

            var shouldToggle = RoadWearToolToggleMod.Instance.Settings.Enabled;
            var shouldBeOn   = m_ToolSystem.activeTool is NetToolSystem;
            var isOn         = m_RenderingSystem.IsShaderEnabled(m_Shader);

            RoadWearToolToggleMod.Instance.Log.Debug($"{shouldToggle} {shouldBeOn}  {m_Shader} {isOn}");

            if (!shouldToggle || shouldBeOn == isOn) {
                return;
            }

            m_RenderingSystem.SetShaderEnabled(m_Shader, shouldBeOn);
        }

        public void Reset() {
            RoadWearToolToggleMod.Instance.Log.Debug("Reset()");
            m_RenderingSystem.SetShaderEnabled(m_Shader, true);
        }
    }
}

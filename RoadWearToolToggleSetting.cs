using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Input;
using Game.Modding;
using Game.Rendering;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using Unity.Entities;

namespace RoadWearToolToggle
{
    [FileLocation(nameof(RoadWearToolToggle))]
    [SettingsUIGroupOrder(kGroup)]
     public class RoadWearToolToggleSetting : ModSetting
    {
        public const string kSection = "Main";
        public const string kGroup = "Main";

        public RoadWearToolToggleSetting(IMod mod) : base(mod) {

        }

        private bool m_Enabled = true;

        [SettingsUISection(kSection, kGroup)]
        public bool Enabled {
            get => m_Enabled;
            set {
                m_Enabled = value;
                
                if (!value) {
                    World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<RoadWearToggleSystem>().Reset();
                }
            }
        }

        public override void SetDefaults() {
            Enabled = true;
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly RoadWearToolToggleSetting m_RoadWearToolToggleSetting;
        public LocaleEN(RoadWearToolToggleSetting roadWearToolToggleSetting)
        {
            m_RoadWearToolToggleSetting = roadWearToolToggleSetting;
        }
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_RoadWearToolToggleSetting.GetSettingsLocaleID(), "RoadWear Tool Toggle" },
                { m_RoadWearToolToggleSetting.GetOptionTabLocaleID(RoadWearToolToggleSetting.kSection), "Main" },

                { m_RoadWearToolToggleSetting.GetOptionGroupLocaleID(RoadWearToolToggleSetting.kGroup), "Buttons" },

                { m_RoadWearToolToggleSetting.GetOptionLabelLocaleID(nameof(RoadWearToolToggleSetting.Enabled)), "Hide RoadWear when not in Net/Road Tool" },
                { m_RoadWearToolToggleSetting.GetOptionDescLocaleID(nameof(RoadWearToolToggleSetting.Enabled)), $"Enabling this option will hide Road Wear lane markings when not in using net/road tool." },

                { m_RoadWearToolToggleSetting.GetBindingMapLocaleID(), "Mod settings" },
            };
        }

        public void Unload()
        {

        }
    }
}

using System;
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
using UnityEngine;

namespace RoadWearToolToggle
{
    [FileLocation(nameof(RoadWearToolToggle))]
    [SettingsUIGroupOrder(kGroup, kAboutGroup)]
    [SettingsUIShowGroupName(kGroup, kAboutGroup)]
    public class RoadWearToolToggleSetting : ModSetting
    {
        public const  string kGroup      = "Main";
        public const  string kAboutGroup = "About";
        private const string Credit      = "Made with <3 by Luca.";

        public RoadWearToolToggleSetting(IMod mod) : base(mod) {

        }

        private bool m_Enabled = true;

        [SettingsUISection(kGroup)]
        public bool Enabled {
            get => m_Enabled;
            set {
                m_Enabled = value;
                
                if (!value) {
                    World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<RoadWearToggleSystem>().Reset();
                }
            }
        }

        [SettingsUISection(kAboutGroup)]
        public string Version => RoadWearToolToggleMod.Version;

        [SettingsUISection(kAboutGroup)]
        public string InformationalVersion => RoadWearToolToggleMod.InformationalVersion;

        [SettingsUISection(kAboutGroup)]
        public string Credits => Credit;

        [SettingsUISection(kAboutGroup)]
        public bool Github {
            set {
                try {
                    Application.OpenURL($"https://github.com/lucarager/CS2-RoadWearToggle");
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        }

        [SettingsUISection(kAboutGroup)]
        public bool Discord {
            set {
                try {
                    Application.OpenURL($"https://discord.gg/QFxmPa2wCa");
                } catch (Exception e) {
                    Debug.LogException(e);
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

                { m_RoadWearToolToggleSetting.GetOptionGroupLocaleID(RoadWearToolToggleSetting.kGroup), "Settings" },
                { m_RoadWearToolToggleSetting.GetOptionGroupLocaleID(RoadWearToolToggleSetting.kAboutGroup), "About" },

                { m_RoadWearToolToggleSetting.GetOptionLabelLocaleID(nameof(RoadWearToolToggleSetting.Enabled)), "Hide RoadWear when not in Net/Road Tool" },
                { m_RoadWearToolToggleSetting.GetOptionDescLocaleID(nameof(RoadWearToolToggleSetting.Enabled)), $"Enabling this option will hide Road Wear lane markings when not in using net/road tool." },

                { m_RoadWearToolToggleSetting.GetBindingMapLocaleID(), "Mod settings" },

                // About
                { m_RoadWearToolToggleSetting.GetOptionLabelLocaleID(nameof(RoadWearToolToggleSetting.Version)), "Version" },
                { m_RoadWearToolToggleSetting.GetOptionLabelLocaleID(nameof(RoadWearToolToggleSetting.InformationalVersion)), "Informational Version" },
                { m_RoadWearToolToggleSetting.GetOptionLabelLocaleID(nameof(RoadWearToolToggleSetting.Credits)), string.Empty },
                { m_RoadWearToolToggleSetting.GetOptionLabelLocaleID(nameof(RoadWearToolToggleSetting.Github)), "GitHub" },
                { m_RoadWearToolToggleSetting.GetOptionDescLocaleID(nameof(RoadWearToolToggleSetting.Github)), "Opens a browser window to https://github.com/lucarager/CS2-RoadWearToggle" },
                { m_RoadWearToolToggleSetting.GetOptionLabelLocaleID(nameof(RoadWearToolToggleSetting.Discord)), "Discord" },
                { m_RoadWearToolToggleSetting.GetOptionDescLocaleID(nameof(RoadWearToolToggleSetting.Discord)), "Opens link to join the CS:2 Modding Discord" },
            };
        }

        public void Unload()
        {

        }
    }
}

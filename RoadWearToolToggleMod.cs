using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.Localization;
using Colossal.Logging;
using Game;
using Game.Input;
using Game.Modding;
using Game.SceneFlow;
using Game.Serialization;
using Newtonsoft.Json;
using UnityEngine;
using static Game.Input.ProxyBinding;

namespace RoadWearToolToggle
{
    public class RoadWearToolToggleMod : IMod
    {
        /// <summary>
        /// The mod's default actionName.
        /// </summary>
        public const string ModName = "RoadWearToolToggle";

        /// <summary>
        /// An id used for bindings between UI and C#.
        /// </summary>
        public static readonly string Id = "RoadWearToolToggle";

        /// <summary>
        /// Gets the instance reference.
        /// </summary>
        public static RoadWearToolToggleMod Instance {
            get; private set;
        }

        /// <summary>
        /// Gets the mod's settings configuration.
        /// </summary>
        internal RoadWearToolToggleSetting Settings {
            get; private set;
        }

        /// <summary>
        /// Gets the mod's logger.
        /// </summary>
        internal ILog Log {
            get; private set;
        }

        /// <summary>
        /// Gets the mod's version
        /// </summary>
        public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString(4);

        /// <summary>
        /// Gets the mod's informational version
        /// </summary>
        public static string InformationalVersion => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        public void OnLoad(UpdateSystem updateSystem)
        {
            // Set instance reference.
            Instance = this;

            // Initialize logger.
            Log = LogManager.GetLogger(ModName);
#if IS_DEBUG
            Log.Info($"[{Id}] Setting logging level to Debug");
            Log.effectivenessLevel = Level.Debug;
#endif
            Log.Info($"[{Id}] Loading {ModName} version {Assembly.GetExecutingAssembly().GetName().Version}");

            // Initialize Settings
            Settings = new RoadWearToolToggleSetting(this);

            // Load i18n
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(Settings));
            Log.Info($"[{Id}] Loaded en-US.");
            LoadNonEnglishLocalizations();
            Log.Info($"[{Id}] Loaded localization files.");

            // Generate i18n files
#if IS_DEBUG && EXPORT_EN_US
            GenerateLanguageFile();
#endif

            // Register mod settings to game options UI.
            Settings.RegisterInOptionsUI();

            // Load saved settings.
            AssetDatabase.global.LoadSettings("RoadWearToolToggle", Settings, new RoadWearToolToggleSetting(this));

            // Apply input bindings.
            Settings.RegisterKeyBindings();

            // Activate Systems
            updateSystem.UpdateBefore<RoadWearToggleSystem>(SystemUpdatePhase.ToolUpdate);
        }

        private void GenerateLanguageFile() {
            Log.Info($"[{Id}] Exporting localization");
            var localeDict = new LocaleEN(Settings).ReadEntries(new List<IDictionaryEntryError>(), new Dictionary<string, int>()).ToDictionary(pair => pair.Key, pair => pair.Value);
            var str = JsonConvert.SerializeObject(localeDict, Newtonsoft.Json.Formatting.Indented);
            try {
                var path = $@"{GetSourceDirectoryPath()}\Lang\en-US.json";
                Log.Info($"[{Id}] Exporting to {path}");
                File.WriteAllText(path, str);
            } catch (Exception ex) {
                Log.Error(ex.ToString());
            }
        }

        public static string GetSourceDirectoryPath([CallerFilePath] string sourceFilePath = "") {
            return Path.GetDirectoryName(sourceFilePath);
        }

        /// <inheritdoc/>
        public void OnDispose() {
            Log.Info($"[{Id}] Disposing");
            Instance = null;

            // Clear settings menu entry.
            if (Settings != null) {
                Settings.UnregisterInOptionsUI();
                Settings = null;
            }
        }

        private void LoadNonEnglishLocalizations() {
            var thisAssembly  = Assembly.GetExecutingAssembly();
            var resourceNames = thisAssembly.GetManifestResourceNames();

            try {
                Log.Debug($"Reading localizations");

                foreach (var localeID in GameManager.instance.localizationManager.GetSupportedLocales()) {
                    var resourceName = $"{thisAssembly.GetName().Name}.lang.{localeID}.json";
                    if (resourceNames.Contains(resourceName)) {
                        Log.Debug($"Found localization file {resourceName}");
                        try {
                            Log.Debug($"Reading embedded translation file {resourceName}");

                            // Read embedded file.
                            using System.IO.StreamReader reader = new(thisAssembly.GetManifestResourceStream(resourceName));
                            {
                                var entireFile   = reader.ReadToEnd();
                                var varient      = Colossal.Json.JSON.Load(entireFile);
                                var translations = varient.Make<Dictionary<string, string>>();
                                GameManager.instance.localizationManager.AddSource(localeID, new MemorySource(translations));
                            }
                        } catch (Exception e) {
                            // Don't let a single failure stop us.
                            Log.Error(e, $"Exception reading localization from embedded file {resourceName}");
                        }
                    } else {
                        Log.Debug($"Did not find localization file {resourceName}");
                    }
                }
            } catch (Exception e) {
                Log.Error(e, "Exception reading embedded settings localization files");
            }
        }

    }
}

namespace STRINGS
{
	public class UI
	{
		public class SPACEDESTINATIONS
		{
			public class DEBRIS
			{
				public class SATELLITE
				{
					public static LocString NAME = "Satellite";

					public static LocString DESCRIPTION = "No Description";
				}
			}

			public class ASTEROIDS
			{
				public class ROCKYASTEROID
				{
					public static LocString NAME = "Rocky Asteroid";

					public static LocString DESCRIPTION = "No Description";
				}

				public class METALLICASTEROID
				{
					public static LocString NAME = "Metallic Asteroid";

					public static LocString DESCRIPTION = "No Description";
				}

				public class CARBONACEOUSASTEROID
				{
					public static LocString NAME = "Carbon Asteroid";

					public static LocString DESCRIPTION = "No Description";
				}
			}

			public class DWARFPLANETS
			{
				public class ICYDWARF
				{
					public static LocString NAME = "Ice Planet";

					public static LocString DESCRIPTION = "No Description";
				}

				public class ORGANICDWARF
				{
					public static LocString NAME = "Organic Mass";

					public static LocString DESCRIPTION = "No Description";
				}

				public class DUSTYDWARF
				{
					public static LocString NAME = "Dusty Dwarf Planet";

					public static LocString DESCRIPTION = "No Description";
				}
			}

			public class PLANETS
			{
				public class TERRAPLANET
				{
					public static LocString NAME = "Terrestrial Planet";

					public static LocString DESCRIPTION = "No Description";
				}

				public class VOLCANOPLANET
				{
					public static LocString NAME = "Volcanic Planet";

					public static LocString DESCRIPTION = "No Description";
				}
			}

			public class GIANTS
			{
				public class GASGIANT
				{
					public static LocString NAME = "Gas Giant";

					public static LocString DESCRIPTION = "No Description";
				}

				public class ICEGIANT
				{
					public static LocString NAME = "Ice Giant";

					public static LocString DESCRIPTION = "No Description";
				}
			}
		}

		public class SANDBOXTOOLS
		{
			public class SETTINGS
			{
				public class INSTANT_BUILD
				{
					public static LocString NAME = "Instant build mode";

					public static LocString TOOLTIP = "Toggle between placing construction plans and fully built buildings";
				}

				public class BRUSH_SIZE
				{
					public static LocString NAME = "Brush size";

					public static LocString TOOLTIP = "Adjust brush size";
				}

				public class BRUSH_NOISE
				{
					public static LocString NAME = "Noise";

					public static LocString TOOLTIP = "Adjust brush noisiness";
				}

				public class TEMPERATURE
				{
					public static LocString NAME = "Absolute temperature";

					public static LocString TOOLTIP = "Adjust absolute temperature";
				}

				public class TEMPERATURE_ADDITIVE
				{
					public static LocString NAME = "Additive temperature";

					public static LocString TOOLTIP = "Adjust additive temperature";
				}

				public class MASS
				{
					public static LocString NAME = "Absolute mass";

					public static LocString TOOLTIP = "Adjust mass";
				}

				public class DISEASE_COUNT
				{
					public static LocString NAME = "Germ count";

					public static LocString TOOLTIP = "Adjust germ count";
				}

				public class BRUSH
				{
					public static LocString NAME = "Brush";

					public static LocString TOOLTIP = "Paint elements into the world simulation";
				}

				public class SPRINKLE
				{
					public static LocString NAME = "Sprinkle";

					public static LocString TOOLTIP = "Paint elements into the simulation using noise";
				}

				public class FLOOD
				{
					public static LocString NAME = "Fill";

					public static LocString TOOLTIP = "Fill a section of the simulation with the chosen element";
				}

				public class SAMPLE
				{
					public static LocString NAME = "Sample";

					public static LocString TOOLTIP = "Copy the settings from a cell to use with brush tools";
				}

				public class HEATGUN
				{
					public static LocString NAME = "Heat Gun";

					public static LocString TOOLTIP = "Inject thermal energy into the simulation";
				}

				public class SPAWNER
				{
					public static LocString NAME = "Spawner";

					public static LocString TOOLTIP = "Spawn critters, food, equipment, and other entities";
				}

				public class CLEAR_FLOOR
				{
					public static LocString NAME = "Clear Debris";

					public static LocString TOOLTIP = "Delete loose items cluttering the floor";
				}

				public class DESTROY
				{
					public static LocString NAME = "Destroy";

					public static LocString TOOLTIP = "Delete everything in the selected cell(s)";
				}

				public class SPAWN_ENTITY
				{
					public static LocString NAME = "Spawn";
				}

				public class FOW
				{
					public static LocString NAME = "Reveal";

					public static LocString TOOLTIP = "Dispel the Fog of War shrouding the map";
				}
			}

			public class FILTERS
			{
				public class ENTITIES
				{
					public static LocString SPECIAL = "Special";

					public static LocString PLANTS = "Plants";

					public static LocString SEEDS = "Seeds";

					public static LocString CREATURE = "Critters";

					public static LocString CREATURE_EGG = "Eggs";

					public static LocString FOOD = "Foods";

					public static LocString EQUIPMENT = "Equipment";

					public static LocString GEYSERS = "Geysers";

					public static LocString EXPERIMENTS = "Experimental";
				}

				public static LocString BACK = "Back";

				public static LocString COMMON = "Common Substances";

				public static LocString SOLID = "Solids";

				public static LocString LIQUID = "Liquids";

				public static LocString GAS = "Gases";
			}

			public class CLEARFLOOR
			{
				public static LocString DELETED = "Deleted";
			}
		}

		public class DROPDOWN
		{
			public static LocString NONE = "Unassigned";
		}

		public class FRONTEND
		{
			public class DEMO_OVER_SCREEN
			{
				public static LocString TITLE = "Thanks for playing!";

				public static LocString BODY = "Thank you for playing the demo for Oxygen Not Included!\n\nThis game is still in development.\n\nGo to kleigames.com/o2 or ask one of us if you'd like more information.";

				public static LocString BUTTON_EXIT_TO_MENU = "EXIT TO MENU";
			}

			public class CUSTOMGAMESETTINGSSCREEN
			{
				public class SETTINGS
				{
					public class SANDBOXMODE
					{
						public static class LEVELS
						{
							public static class DISABLED
							{
								public static LocString NAME = "Disabled";

								public static LocString TOOLTIP = "Click to enable Sandbox mode";
							}

							public static class ENABLED
							{
								public static LocString NAME = "Enabled";

								public static LocString TOOLTIP = "Click to disable Sandbox mode";
							}
						}

						public static LocString NAME = "Sandbox Mode";

						public static LocString TOOLTIP = "Manipulate and customize the simulation with tools that ignore regular game constraints";
					}

					public class IMMUNESYSTEM
					{
						public static class LEVELS
						{
							public static class COMPROMISED
							{
								public static LocString NAME = "Miserable";

								public static LocString TOOLTIP = "Duplicants are exceptionally vulnerable to infection";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Vulnerable Immune System";
							}

							public static class WEAK
							{
								public static LocString NAME = "Weak";

								public static LocString TOOLTIP = "Duplicants have reduced immune recovery";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Weak Immune System";
							}

							public static class DEFAULT
							{
								public static LocString NAME = "Regular";

								public static LocString TOOLTIP = "Duplicants regain immunity normally";
							}

							public static class STRONG
							{
								public static LocString NAME = "Strong";

								public static LocString TOOLTIP = "Duplicants have increased immune recovery";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Strong Immune System";
							}

							public static class INVINCIBLE
							{
								public static LocString NAME = "None";

								public static LocString TOOLTIP = "Duplicants will never catch a germ-based disease";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Impervious Immune System";
							}
						}

						public static LocString NAME = "Immune Systems";

						public static LocString TOOLTIP = "Affects how resistant Duplicants are to disease";
					}

					public class MORALE
					{
						public static class LEVELS
						{
							public static class VERYHARD
							{
								public static LocString NAME = "Very High";

								public static LocString TOOLTIP = "Duplicants must maintain extremely high Morale ratings to prevent stress";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Low Morale (Very Hard)";
							}

							public static class HARD
							{
								public static LocString NAME = "High";

								public static LocString TOOLTIP = "Duplicants must maintain higher than normal Morale ratings to prevent stress";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Low Morale (Hard)";
							}

							public static class DEFAULT
							{
								public static LocString NAME = "Regular";

								public static LocString TOOLTIP = "Duplicants have normal Morale requirements";
							}

							public static class EASY
							{
								public static LocString NAME = "Low";

								public static LocString TOOLTIP = "Duplicants need only minor Morale levels to fend off stress";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Low Morale (Easy)";
							}

							public static class DISABLED
							{
								public static LocString NAME = "None";

								public static LocString TOOLTIP = "Duplicants will not gain stress, regardless of their Morale rating";
							}
						}

						public static LocString NAME = "Morale Requirements";

						public static LocString TOOLTIP = "Adjusts the minimum Morale rating required to prevent Duplicant stress";
					}

					public class CALORIE_BURN
					{
						public static class LEVELS
						{
							public static class VERYHARD
							{
								public static LocString NAME = "Ravaging Hunger";

								public static LocString TOOLTIP = "Duplicants require near-constant feeding";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Ravaging Hunger (Very Hard)";
							}

							public static class HARD
							{
								public static LocString NAME = "Hungry";

								public static LocString TOOLTIP = "Duplicants require more calories than usual";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Hungry (Hard)";
							}

							public static class DEFAULT
							{
								public static LocString NAME = "Regular";

								public static LocString TOOLTIP = "Duplicants have typical hunger";
							}

							public static class EASY
							{
								public static LocString NAME = "Fasting";

								public static LocString TOOLTIP = "Duplicants get by with fewer calories than usual";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Fasting (Easy)";
							}

							public static class DISABLED
							{
								public static LocString NAME = "Oxygenarian";

								public static LocString TOOLTIP = "Duplicants somehow manage to sustain themselves on nothing at all";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Oxygenarian (No Hunger)";
							}
						}

						public static LocString NAME = "Hunger Rate";

						public static LocString TOOLTIP = "Affects how quickly Duplicants burn calories";
					}

					public class WORLD_CHOICE
					{
						public static LocString NAME = "World(DEBUG)";

						public static LocString TOOLTIP = "(DEBUG FEATURE) Select your starting world.";
					}

					public class STRESS
					{
						public static class LEVELS
						{
							public static class INDOMITABLE
							{
								public static LocString NAME = "Unflappable";

								public static LocString TOOLTIP = "Duplicants will never get stressed";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Unflappable";
							}

							public static class OPTIMISTIC
							{
								public static LocString NAME = "Optimistic";

								public static LocString TOOLTIP = "Duplicants are harder to stress out";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Optimistic";
							}

							public static class DEFAULT
							{
								public static LocString NAME = "Regular";

								public static LocString TOOLTIP = "Duplicants gain and lose stress normally";
							}

							public static class PESSIMISTIC
							{
								public static LocString NAME = "Pessimistic";

								public static LocString TOOLTIP = "Duplicants stress out easily";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Pessimistic";
							}

							public static class DOOMED
							{
								public static LocString NAME = "Fatalistic";

								public static LocString TOOLTIP = "Duplicants stress out at an extremely accelerated rate";

								public static LocString ATTRIBUTE_MODIFIER_NAME = "Fatalistic";
							}
						}

						public static LocString NAME = "Duplicant Mood";

						public static LocString TOOLTIP = "Affects how well Duplicants handle stress";
					}

					public class STRESS_BREAKS
					{
						public static class LEVELS
						{
							public static class DEFAULT
							{
								public static LocString NAME = "Normal";

								public static LocString TOOLTIP = "Duplicants will act out when they reach 100% stress";
							}

							public static class DISABLED
							{
								public static LocString NAME = "Disabled";

								public static LocString TOOLTIP = "Duplicants will not act out at maximum stress";
							}
						}

						public static LocString NAME = "Stress Reactions";

						public static LocString TOOLTIP = "Determines whether Duplicants act out when they reach maximum stress";
					}

					public class WORLDGEN_SEED
					{
						public static LocString NAME = "Worldgen Seed";

						public static LocString TOOLTIP = "This chooses the random parameters of your new world";
					}
				}
			}

			public class MAINMENU
			{
				public static LocString STARTDEMO = "START DEMO";

				public static LocString NEWGAME = "NEW GAME";

				public static LocString RESUMEGAME = "RESUME GAME";

				public static LocString LOADGAME = "LOAD GAME";

				public static LocString SCENARIOS = "SCENARIOS";

				public static LocString TRANSLATIONS = "TRANSLATIONS";

				public static LocString OPTIONS = "OPTIONS";

				public static LocString QUITTODESKTOP = "QUIT";

				public static LocString RESTARTCONFIRM = "Should I really quit?\nAll unsaved progress will be lost.";

				public static LocString QUITCONFIRM = "Should I quit to the main menu?\nAll unsaved progress will be lost.";

				public static LocString DESKTOPQUITCONFIRM = "Should I really quit?\nAll unsaved progress will be lost.";

				public static LocString RESUMEBUTTON_BASENAME = "{0}: Cycle {1}";
			}

			public class NEWGAMESETTINGS
			{
				public class BUTTONS
				{
					public static LocString STANDARDGAME = "Standard Game";

					public static LocString CUSTOMGAME = "Custom Game";

					public static LocString CANCEL = "Cancel";

					public static LocString STARTGAME = "Start Game";
				}

				public static LocString HEADER = "GAME SETTINGS";
			}

			public class MODESELECTSCREEN
			{
				public static LocString HEADER = "GAME MODE";

				public static LocString BLANK_DESC = "Select a playstyle...";

				public static LocString SURVIVAL_TITLE = "SURVIVAL";

				public static LocString SURVIVAL_DESC = "Only you can protect your fragile colony from this harsh and unforgiving universe.";

				public static LocString NOSWEAT_TITLE = "NO SWEAT";

				public static LocString NOSWEAT_DESC = "The cosmos is a glorious place where you can lead your colony into adventure with ease.";
			}

			public class PATCHNOTESSCREEN
			{
				public static LocString TITLE = "SPACE INDUSTRY UPGRADE";

				public static LocString BODY = "<b>Welcome to the Space Industry Upgrade!!</b>\n\n{0}";

				public static LocString PATCHNOTES = "<b>Update Features:</b>\n\n• Expanded Starmap locations with new rare resources to recover\n• New surface creature, the Shove Vole\n• New Molecular Forge for crafting industrial materials from rare space resources\n• Increased automation, new buildings, research and more\n\nPlease view the full patch notes for further details!";

				public static LocString OK_BUTTON = "OK";
			}

			public class LOADSCREEN
			{
				public static LocString TITLE = "LOAD GAME";

				public static LocString TITLE_INSPECT = "LOAD GAME";

				public static LocString DELETEBUTTON = "DELETE";

				public static LocString CONFIRMDELETE = "Are you sure you want to delete {0}?\nYou cannot undo this action.";

				public static LocString SAVEDETAILS = "<b>File:</b> {0}\n\n<b>Save Date:</b>\n{1}\n\n<b>Base Name:</b> {2}\n<b>Duplicants Alive:</b> {3}\n<b>Cycle(s) Survived:</b> {4}";

				public static LocString AUTOSAVEWARNING = "<color=#ff0000>Autosave: This file will get deleted as new autosaves are created</color>";

				public static LocString CORRUPTEDSAVE = "<b><color=#ff0000>Could not load file {0}. Its data may be corrupted.</color></b>";

				public static LocString SAVE_TOO_NEW = "<b><color=#ff0000>Could not load file {0}. File is using build {1}. This build is {2}.</color></b>";

				public static LocString UNSUPPORTED_SAVE_VERSION = "<b><color=#ff0000>This save file is from a previous version of the game and is no longer supported.</color></b>";

				public static LocString MORE_INFO = "More Info";
			}

			public class SAVESCREEN
			{
				public static LocString TITLE = "SAVE SLOTS";

				public static LocString NEWSAVEBUTTON = "New Save";

				public static LocString OVERWRITEMESSAGE = "Are you sure you want to overwrite {0}?";

				public static LocString SAVENAMETITLE = "SAVE NAME";

				public static LocString CONFIRMNAME = "Confirm";

				public static LocString CANCELNAME = "Cancel";

				public static LocString IO_ERROR = "An error occurred trying to save your game. Please ensure there is sufficient disk space.\n\n{0}";

				public static LocString REPORT_BUG = "Report Bug";
			}

			public class MOD_ERRORS
			{
				public class TOOLTIPS
				{
					public static LocString MOD_REQUIRED = "The current save game couldn't load this mod. Unexpected things may happen!";
				}

				public static LocString TITLE = "MOD ERRORS";

				public static LocString DETAILS = "DETAILS";

				public static LocString CLOSE = "CLOSE";

				public static LocString MOD_REQUIRED = "REQUIRED";
			}

			public class MODS
			{
				public class TOOLTIPS
				{
					public static LocString ENABLED = "Enabled";

					public static LocString DISABLED = "Disabled";

					public static LocString MANAGE_STEAM_SUBSCRIPTION = "Manage Steam Subscription";
				}

				public static LocString TITLE = "MODS";

				public static LocString MANAGE = "Subscription";

				public static LocString DRAG_TO_REORDER = "Drag to re-order";

				public static LocString REQUIRES_RESTART = "Mod changes require restart";

				public static LocString FAILED_TO_LOAD = "A mod failed to load and is being disabled:\n\n{0}: {1}\n\n{2}";

				public static LocString DB_CORRUPT = "An error occurred trying to load the Mod Database.\n\n{0}";
			}

			public class PAUSE_SCREEN
			{
				public static LocString TITLE = "PAUSED";

				public static LocString RESUME = "Resume";

				public static LocString LOGBOOK = "Logbook";

				public static LocString OPTIONS = "Options";

				public static LocString SAVE = "Save";

				public static LocString SAVEAS = "Save As";

				public static LocString LOAD = "Load";

				public static LocString QUIT = "Main Menu";

				public static LocString DESKTOPQUIT = "Quit to Desktop";

				public static LocString WORLD_SEED = "World Seed: {0}";
			}

			public class OPTIONS_SCREEN
			{
				public class TOGGLE_SANDBOX_SCREEN
				{
					public static LocString UNLOCK_SANDBOX_WARNING = "Sandbox mode will be enabled for this save file";

					public static LocString CONFIRM = "Enable sandbox mode";

					public static LocString CANCEL = "Cancel";

					public static LocString CONFIRM_SAVE_BACKUP = "Enable sandbox mode, but save a backup first";

					public static LocString BACKUP_SAVE_GAME_APPEND = " (BACKUP)";
				}

				public static LocString TITLE = "OPTIONS";

				public static LocString GRAPHICS = "Graphics";

				public static LocString AUDIO = "Audio";

				public static LocString CONTROLS = "Controls";

				public static LocString UNITS = "Temperature Units";

				public static LocString METRICS = "Data Collection";

				public static LocString LANGUAGE = "Change Language";

				public static LocString WORLD_GEN = "World Generation Key";

				public static LocString RESET_TUTORIAL = "Reset Tutorial Messages";

				public static LocString RESET_TUTORIAL_WARNING = "All tutorial messages will be reset, and\nwill show up again the next time you play the game.";

				public static LocString CREDITS = "Credits";

				public static LocString BACK = "Done";

				public static LocString UNLOCK_SANDBOX = "Unlock Sandbox Mode";

				public static LocString MODS = "MODS";
			}

			public class INPUT_BINDINGS_SCREEN
			{
				public static LocString TITLE = "CUSTOMIZE KEYS";

				public static LocString RESET = "Reset";

				public static LocString APPLY = "Done";

				public static LocString DUPLICATE = "{0} was already bound to {1} and is now unbound.";

				public static LocString UNBOUND_ACTION = "{0} is unbound. Are you sure you want to continue?";

				public static LocString MULTIPLE_UNBOUND_ACTIONS = "You have multiple unbound actions, this may result in difficulty playing the game. Are you sure you want to continue?";

				public static LocString WAITING_FOR_INPUT = "???";
			}

			public class TRANSLATIONS_SCREEN
			{
				public class PREINSTALLED_LANGUAGES
				{
					public static LocString EN = "English (Klei)";

					public static LocString ZH_KLEI = "Chinese (Klei)";

					public static LocString KO_KLEI = "Korean (Klei)";

					public static LocString RU_KLEI = "Russian (Klei)";
				}

				public static LocString TITLE = "TRANSLATIONS";

				public static LocString UNINSTALL = "Uninstall";

				public static LocString PREINSTALLED_HEADER = "Preinstalled Language Packs";

				public static LocString UGC_HEADER = "Subscribed Workshop Language Packs";

				public static LocString UGC_MOD_TITLE_FORMAT = "{0} (workshop)";

				public static LocString ARE_YOU_SURE = "Are you sure you want to uninstall this language pack?";

				public static LocString PLEASE_REBOOT = "Please restart your game for these changes to take effect.";

				public static LocString NO_PACKS = "Steam Workshop";

				public static LocString DOWNLOAD = "Start Download";

				public static LocString INSTALL = "Install";

				public static LocString INSTALLED = "Installed";

				public static LocString NO_STEAM = "Unable to retrieve language list from Steam";
			}

			public class SCENARIOS_MENU
			{
				public static LocString TITLE = "Scenarios";

				public static LocString UNSUBSCRIBE = "Unsubscribe";

				public static LocString UNSUBSCRIBE_CONFIRM = "Are you sure you want to unsubscribe from this scenario?";

				public static LocString LOAD_SCENARIO_CONFIRM = "Load the \"{SCENARIO_NAME}\" scenario?";

				public static LocString LOAD_CONFIRM_TITLE = "LOAD";

				public static LocString SCENARIO_NAME = "Name:";

				public static LocString SCENARIO_DESCRIPTION = "Description";

				public static LocString BUTTON_DONE = "Done";

				public static LocString BUTTON_LOAD = "Load";

				public static LocString BUTTON_WORKSHOP = "Steam Workshop";

				public static LocString NO_SCENARIOS_AVAILABLE = "No scenarios available.\n\nSubscribe to some in the Steam Workshop.";
			}

			public class AUDIO_OPTIONS_SCREEN
			{
				public static LocString TITLE = "AUDIO OPTIONS";

				public static LocString DONE_BUTTON = "Done";

				public static LocString MUSIC_EVERY_CYCLE = "Play background music each morning: ";

				public static LocString MUSIC_EVERY_CYCLE_TOOLTIP = "If enabled, background music will play every cycle instead of every few cycles";

				public static LocString AUTOMATION_SOUNDS_ALWAYS = "Always play automation sounds: ";

				public static LocString AUTOMATION_SOUNDS_ALWAYS_TOOLTIP = "If enabled, automation sound effects will play even when outside of the Automation Overlay";

				public static LocString AUDIO_BUS_MASTER = "Master";

				public static LocString AUDIO_BUS_SFX = "SFX";

				public static LocString AUDIO_BUS_MUSIC = "Music";

				public static LocString AUDIO_BUS_AMBIENCE = "Ambience";

				public static LocString AUDIO_BUS_UI = "UI";
			}

			public class WORLD_GEN_OPTIONS_SCREEN
			{
				public static LocString TITLE = "WORLD GENERATION OPTIONS";

				public static LocString USE_SEED = "Set World Gen Seed";

				public static LocString DONE_BUTTON = "Done";

				public static LocString RANDOM_BUTTON = "Randomize";

				public static LocString RANDOM_BUTTON_TOOLTIP = "Randomize a new world gen seed";

				public static LocString TOOLTIP = "This will override the current world gen seed";
			}

			public class METRICS_OPTIONS_SCREEN
			{
				public static LocString TITLE = "DATA COLLECTION OPTIONS";

				public static LocString ENABLE_BUTTON = "Enable Data Collection: ";

				public static LocString DESCRIPTION = "We require the collection of user data to assist in improving game operations. Players who opt out of data collection will no longer send crash reports and user data to the game team.\n\nFor more details on our privacy policy and how we use the data we collect, please visit our <color=blue><u><b>privacy policy</u></b></color>.";

				public static LocString DONE_BUTTON = "Done";

				public static LocString TOOLTIP = "Toggle data collection.";
			}

			public class UNIT_OPTIONS_SCREEN
			{
				public static LocString TITLE = "TEMPERATURE UNITS";

				public static LocString CELSIUS = "Celsius: ";

				public static LocString CELSIUS_TOOLTIP = "Change temperature unit to Celsius (°C)";

				public static LocString KELVIN = "Kelvin: ";

				public static LocString KELVIN_TOOLTIP = "Change temperature unit to Kelvin (K)";

				public static LocString FAHRENHEIT = "Fahrenheit: ";

				public static LocString FAHRENHEIT_TOOLTIP = "Change temperature unit to Fahrenheit (°F)";
			}

			public class GRAPHICS_OPTIONS_SCREEN
			{
				public static LocString TITLE = "GRAPHICS OPTIONS";

				public static LocString FULLSCREEN = "Fullscreen:";

				public static LocString RESOLUTION = "Resolution:";

				public static LocString APPLYBUTTON = "Apply";

				public static LocString REVERTBUTTON = "Revert";

				public static LocString DONE_BUTTON = "Done";

				public static LocString UI_SCALE = "UI Scale";

				public static LocString ACCEPT_CHANGES = "Accept Changes?";
			}

			public class WORLDGENSCREEN
			{
				public class SIZES
				{
					public static LocString TINY = "Tiny";

					public static LocString SMALL = "Small";

					public static LocString STANDARD = "Standard";

					public static LocString LARGE = "Big";

					public static LocString HUGE = "Colossal";
				}

				public static LocString TITLE = "NEW GAME";

				public static LocString GENERATINGWORLD = "GENERATING WORLD";

				public static LocString SELECTSIZEPROMPT = "A new world is about to be created. Please select its size.";

				public static LocString LOADINGGAME = "LOADING WORLD...";
			}

			public class MINSPECSCREEN
			{
				public static LocString TITLE = "WARNING!";

				public static LocString SIMFAILEDTOLOAD = "A problem occurred loading Oxygen Not Included. This is usually caused by the Visual Studio C++ 2015 runtime being improperly installed on the system. Please exit the game, run Windows Update, and try re-launching Oxygen Not Included.";

				public static LocString BODY = "We've detected that this computer does not meet the minimum requirements to run Oxygen Not Included. While you may continue with your current specs, the game might not run smoothly for you.\n\nPlease be aware that your experience may suffer as a result.";

				public static LocString OKBUTTON = "Okay, thanks!";

				public static LocString QUITBUTTON = "Quit";
			}

			public class SUPPORTWARNINGS
			{
				public static LocString AUDIO_DRIVERS = "A problem occurred initializing your audio device.\nSorry about that!\n\nThis is usually caused by outdated audio drivers.\n\nPlease visit your audio device manufacturer's website to download the latest drivers.";

				public static LocString AUDIO_DRIVERS_MORE_INFO = "More Info";

				public static LocString DUPLICATE_KEY_BINDINGS = "<b>Duplicate key bindings were detected.\nThis may be because your custom key bindings conflicted with a new feature's default key.\nPlease visit the controls screen to ensure your key bindings are set how you like them.</b>\n{0}";

				public static LocString SAVE_DIRECTORY_READ_ONLY = "A problem occurred while accessing your save directory.\nThis may be because your directory is set to read-only.\n\nPlease ensure your save directory is readable as well as writable and re-launch the game.\n{0}";

				public static LocString SAVE_DIRECTORY_INSUFFICIENT_SPACE = "There is insufficient disk space to write to your save directory.\n\nPlease free at least 15 MB to give your saves some room to breathe.\n{0}";

				public static LocString WORLD_GEN_FILES = "A problem occurred while accessing certain game files that will prevent starting new games.\n\nPlease ensure you can modify these files and re-launch the game:\n\n{0}";

				public static LocString IO_UNAUTHORIZED = "An Unauthorized Access Error occurred when trying to write to disk.\nPlease check that you have permissions to write to:\n{0}";

				public static LocString IO_SUFFICIENT_SPACE = "An Insufficient Space Error occurred when trying to write to disk. \n\nPlease free up some space.\n{0}";
			}

			public class SAVEUPGRADEWARNINGS
			{
				public static LocString SUDDENMORALEHELPER = "Welcome to the Expressive Upgrade! This update introduces a new Morale system that replaces Food and Decor Expectations that were found in previous versions of the game.\n\nThe game you are trying to load was created before this system was introduced, and will need to be updated. You may either:\n\n\n1) Enable the new Morale system in this save, removing Food and Decor Expectations. It's possible that when you load your save your old colony won't meet your Duplicants' new Morale needs, so they'll receive a 5 cycle Morale boost to give you time to adjust.\n\n2) Disable Morale in this save. The new Morale mechanics will still be visible, but won't affect your Duplicants' stress. Food and Decor expectations will no longer exist in this save.";

				public static LocString SUDDENMORALEHELPER_TITLE = "MORALE CHANGES";

				public static LocString SUDDENMORALEHELPER_BUFF = "1) Bring on Morale!";

				public static LocString SUDDENMORALEHELPER_DISABLE = "2) Disable Morale";
			}

			public static LocString GAME_VERSION = "Game Version: ";

			public static LocString LOADING = "Loading...";
		}

		public class SANDBOX_TOGGLE
		{
			public static LocString TOOLTIP_LOCKED = "Sandbox mode must be unlocked in the options menu before it can be used.";

			public static LocString TOOLTIP_UNLOCKED = "Toggle Sandbox mode";
		}

		public class ROLES_SCREEN
		{
			public class WIDGET
			{
				public static LocString NUMBER_OF_MASTERS_TOOLTIP = "<b>Duplicants who have mastered this job:</b>{0}";

				public static LocString NO_MASTERS_TOOLTIP = "<b>No Duplicants have mastered this job</b>";
			}

			public class TIER_NAMES
			{
				public static LocString ZERO = "Tier 0";

				public static LocString ONE = "Tier 1";

				public static LocString TWO = "Tier 2";

				public static LocString THREE = "Tier 3";

				public static LocString FOUR = "Tier 4";

				public static LocString FIVE = "Tier 5";

				public static LocString SIX = "Tier 6";

				public static LocString SEVEN = "Tier 7";

				public static LocString EIGHT = "Tier 8";

				public static LocString NINE = "Tier 9";
			}

			public class SLOTS
			{
				public static LocString UNASSIGNED = "Vacant Position";

				public static LocString UNASSIGNED_TOOLTIP = "Click to assign a Duplicant to this job opening";

				public static LocString NOSLOTS = "No slots available";

				public static LocString NO_ELIGIBLE_DUPLICANTS = "No Duplicants meet the requirements for this job";

				public static LocString ASSIGNMENT_PENDING = "(Pending)";

				public static LocString PICK_JOB = "No Job";

				public static LocString PICK_DUPLICANT = "None";
			}

			public class DROPDOWN
			{
				public static LocString NAME_AND_ROLE = "{0} <color=#F44A47FF>({1})</color>";

				public static LocString ALREADY_ROLE = "(Currently {0})";
			}

			public class SIDEBAR
			{
				public static LocString ASSIGNED_DUPLICANTS = "Assigned Duplicants";

				public static LocString UNASSIGNED_DUPLICANTS = "Unassigned Duplicants";

				public static LocString UNASSIGN = "Unassign job";
			}

			public class PRIORITY
			{
				public static LocString TITLE = "Job Priorities";

				public static LocString DESCRIPTION = "{0}s prioritize these work errands: ";

				public static LocString NO_EFFECT = "This job does not affect errand prioritization";
			}

			public class RESUME
			{
				public static LocString TITLE = "Qualifications";

				public static LocString PREVIOUS_ROLES = "PREVIOUS DUTIES";

				public static LocString UNASSIGNED = "Unassigned";

				public static LocString NO_SELECTION = "No Duplicant selected";
			}

			public class PERKS
			{
				public class CAN_DIG_VERY_FIRM
				{
					public static LocString DESCRIPTION = "Trait: " + FormatAsLink("Very Firm Material", "HARDNESS") + " Mining";
				}

				public class CAN_DIG_NEARLY_IMPENETRABLE
				{
					public static LocString DESCRIPTION = "Trait: " + ELEMENTS.KATAIRITE.NAME + " Mining";
				}

				public class CAN_ART
				{
					public static LocString DESCRIPTION = "Trait: Can work " + BUILDINGS.PREFABS.CANVAS.NAME + " and " + BUILDINGS.PREFABS.SCULPTURE.NAME;
				}

				public class CAN_FARM_TINKER
				{
					public static LocString DESCRIPTION = "Trait: " + FormatAsLink("Crop Tending", "PLANTS") + " and " + ITEMS.INDUSTRIAL_PRODUCTS.FARM_STATION_TOOLS.NAME + " Crafting";
				}

				public class CAN_WRANGLE_CREATURES
				{
					public static LocString DESCRIPTION = "Trait: Critter Wrangling";
				}

				public class CAN_USE_RANCH_STATION
				{
					public static LocString DESCRIPTION = "Trait: Grooming Station Training";
				}

				public class CAN_POWER_TINKER
				{
					public static LocString DESCRIPTION = "Trait: " + FormatAsLink("Generator Tuning", "POWER") + " and " + ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.NAME + " Crafting";
				}

				public class CAN_ELECTRIC_GRILL
				{
					public static LocString DESCRIPTION = "Trait: " + BUILDINGS.PREFABS.COOKINGSTATION.NAME + " Cooking";
				}

				public class ADVANCED_RESEARCH
				{
					public static LocString DESCRIPTION = "Trait: " + BUILDINGS.PREFABS.ADVANCEDRESEARCHCENTER.NAME + " Researching";
				}

				public class INTERSTELLAR_RESEARCH
				{
					public static LocString DESCRIPTION = "Trait: " + BUILDINGS.PREFABS.COSMICRESEARCHCENTER.NAME + " Researching";
				}

				public class CAN_STUDY_WORLD_OBJECTS
				{
					public static LocString DESCRIPTION = "Trait: Geographical Analysis";
				}

				public class INCREASED_ART
				{
					public static LocString DESCRIPTION = "+{0} to Creativity";
				}

				public class INCREASED_CONSTRUCTION
				{
					public static LocString DESCRIPTION = "+{0} to Construction";
				}

				public class INCREASE_BOTANIST
				{
					public static LocString DESCRIPTION = "+{0} to Farming";
				}

				public class INCREASE_RANCHING
				{
					public static LocString DESCRIPTION = "+{0} to Ranching";
				}

				public class INCREASED_CARING
				{
					public static LocString DESCRIPTION = "+{0} to Caring";
				}

				public class INCREASED_DIG_SPEED
				{
					public static LocString DESCRIPTION = "+{0} to Digging";
				}

				public class INCREASED_ATHLETICS
				{
					public static LocString DESCRIPTION = "+{0} to Athletics";
				}

				public class INCREASED_CARRY_AMOUNT
				{
					public static LocString DESCRIPTION = "+{0}kg to Carrying Capacity";
				}

				public class INCREASED_MACHINERY
				{
					public static LocString DESCRIPTION = "+{0} to Tinkering";
				}

				public class INCREASED_COOKING
				{
					public static LocString DESCRIPTION = "+{0} to Cooking";
				}

				public class INCREASED_STRENGTH
				{
					public static LocString DESCRIPTION = "+{0} to Strength";
				}

				public class INCREASED_LEARNING
				{
					public static LocString DESCRIPTION = "+{0} to Learning";
				}

				public class MANAGERIAL_DUTIES
				{
					public static LocString DESCRIPTION = "Workload stress";
				}

				public class MANAGED_COLONY
				{
					public static LocString DESCRIPTION = "Colony Stress Alleviater";
				}

				public class EXOSUIT_EXPERTISE
				{
					public static LocString DESCRIPTION = "Trait: " + FormatAsLink("Exosuit", "EXOSUIT") + " Sprinting";
				}

				public class CONVEYOR_BUILD
				{
					public static LocString DESCRIPTION = "Trait: " + BUILDINGS.PREFABS.SOLIDCONDUIT.NAME + " Construction";
				}

				public class CAN_DO_PLUMBING
				{
					public static LocString DESCRIPTION = "Trait: Plumbing";
				}

				public class CAN_USE_ROCKETS
				{
					public static LocString DESCRIPTION = "Trait: Astronaut";
				}

				public class CAN_DO_ASTRONAUT_TRAINING
				{
					public static LocString DESCRIPTION = "Trait: Astronaut-in-Training";
				}

				public static LocString TITLE_BASICTRAINING = "Basic Job Training";

				public static LocString TITLE_MORETRAINING = "Additional Job Training";

				public static LocString NO_PERKS = "This job comes with no training";
			}

			public class ASSIGNMENT_REQUIREMENTS
			{
				public class ELIGIBILITY
				{
					public static LocString ELIGIBLE = "{0} is qualified for the {1} position";

					public static LocString INELIGIBLE = "{0} is <color=#F44A47FF>not qualified</color> for the {1} position";
				}

				public class UNEMPLOYED
				{
					public static LocString NAME = "Unassigned";

					public static LocString DESCRIPTION = "Duplicant must not already have a job assignment";
				}

				public class HAS_COLONY_LEADER
				{
					public static LocString NAME = "Has colony leader";

					public static LocString DESCRIPTION = "A colony leader must be assigned";
				}

				public class HAS_ATTRIBUTE_DIGGING_BASIC
				{
					public static LocString NAME = "Basic Digging";

					public static LocString DESCRIPTION = "Must have at least {0} digging skill";
				}

				public class HAS_ATTRIBUTE_COOKING_BASIC
				{
					public static LocString NAME = "Basic Cooking";

					public static LocString DESCRIPTION = "Must have at least {0} cooking skill";
				}

				public class HAS_ATTRIBUTE_LEARNING_BASIC
				{
					public static LocString NAME = "Basic Learning";

					public static LocString DESCRIPTION = "Must have at least {0} learning skill";
				}

				public class HAS_ATTRIBUTE_LEARNING_MEDIUM
				{
					public static LocString NAME = "Medium Learning";

					public static LocString DESCRIPTION = "Must have at least {0} learning skill";
				}

				public class HAS_EXPERIENCE
				{
					public static LocString NAME = "{0} Experience";

					public static LocString DESCRIPTION = "Mastery of the <b>{0}</b> job";
				}

				public class HAS_COMPLETED_ANY_OTHER_ROLE
				{
					public static LocString NAME = "General Experience";

					public static LocString DESCRIPTION = "Mastery of <b>at least one</b> job";
				}

				public class CHOREGROUP_ENABLED
				{
					public static LocString NAME = "Can perform {0}";

					public static LocString DESCRIPTION = "Capable of performing <b>{0}</b> jobs";
				}

				public static LocString TITLE = "Qualifications";

				public static LocString NONE = "This position has no qualification requirements";

				public static LocString ALREADY_IS_ROLE = "{0} <b>is already</b> assigned to the {1} position";

				public static LocString ALREADY_IS_JOBLESS = "{0} <b>is already</b> unemployed";

				public static LocString MASTERED = "{0} has mastered the {1} position";

				public static LocString WILL_BE_UNASSIGNED = "Note: Assigning {0} to {1} will <color=#F44A47FF>unassign</color> them from {2}";

				public static LocString RELEVANT_ATTRIBUTES = "Relevant skills:";

				public static LocString APTITUDES = "Interests";

				public static LocString RELEVANT_APTITUDES = "Relevant Interests:";

				public static LocString NO_APTITUDE = "None";
			}

			public class EXPECTATIONS
			{
				public class PRIVATE_ROOM
				{
					public static LocString NAME = "Private Bedroom";

					public static LocString DESCRIPTION = "Duplicants in this job would appreciate their own place to unwind";
				}

				public class FOOD_QUALITY
				{
					public class MINOR
					{
						public static LocString NAME = "Standard Food";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire food that meets basic living standards";
					}

					public class MEDIUM
					{
						public static LocString NAME = "Good Food";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire decent food for their efforts";
					}

					public class HIGH
					{
						public static LocString NAME = "Great Food";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire better than average food";
					}

					public class VERY_HIGH
					{
						public static LocString NAME = "Superb Food";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier have a refined taste for food";
					}

					public class EXCEPTIONAL
					{
						public static LocString NAME = "Ambrosial Food";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier expect only the best cuisine";
					}
				}

				public class DECOR
				{
					public class MINOR
					{
						public static LocString NAME = "Minor Decor";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire slightly improved colony decor";
					}

					public class MEDIUM
					{
						public static LocString NAME = "Medium Decor";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire reasonably improved colony decor";
					}

					public class HIGH
					{
						public static LocString NAME = "High Decor";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire a decent increase in colony decor";
					}

					public class VERY_HIGH
					{
						public static LocString NAME = "Superb Decor";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire majorly improved colony decor";
					}

					public class UNREASONABLE
					{
						public static LocString NAME = "Decadent Decor";

						public static LocString DESCRIPTION = "Duplicants employed in this Tier desire unrealistically luxurious improvements to decor";
					}
				}

				public class QUALITYOFLIFE
				{
					public class TIER0
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 0";
					}

					public class TIER1
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 1";
					}

					public class TIER2
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 2";
					}

					public class TIER3
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 3";
					}

					public class TIER4
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 4";
					}

					public class TIER5
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 5";
					}

					public class TIER6
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 6";
					}

					public class TIER7
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 7";
					}

					public class TIER8
					{
						public static LocString NAME = "Morale Requirements";

						public static LocString DESCRIPTION = "Tier 8";
					}
				}

				public static LocString TITLE = "Special Provisions Request";

				public static LocString NO_EXPECTATIONS = "No additional provisions are required to perform this job";
			}

			public static LocString MANAGEMENT_BUTTON = "JOBS";

			public static LocString ROLE_PROGRESS = "<b>Job Experience: {0}/{1}</b>\nDuplicants can become eligible for specialized jobs by maxing their current job experience";

			public static LocString NO_JOB_STATION_WARNING = "Build a Jobs Board to unlock Job Assignments";

			public static LocString AUTO_PRIORITIZE = "Auto-Prioritize:";

			public static LocString AUTO_PRIORITIZE_ENABLED = "Duplicant priorities are automatically reconfigured when they are assigned a new job";

			public static LocString AUTO_PRIORITIZE_DISABLED = "Duplicant priorities can only be changed manually";

			public static LocString EXPECTATION_ALERT_EXPECTATION = "Current Morale: {0}\nJob Morale Needs: {1}";

			public static LocString EXPECTATION_ALERT_JOB = "Current Morale: {0}\n{2} Minimum Morale: {1}";

			public static LocString EXPECTATION_ALERT_TARGET_JOB = "{2}'s Current: {0} Morale\n{3} Minimum Morale: {1}";

			public static LocString EXPECTATION_ALERT_DESC_EXPECTATION = "This Duplicant's Morale is too low to handle the rigors of this position, which will cause them Stress over time.";

			public static LocString EXPECTATION_ALERT_DESC_JOB = "This Duplicant's Morale is too low to handle the assigned job, which will cause them Stress over time.";

			public static LocString EXPECTATION_ALERT_DESC_TARGET_JOB = "This Duplicant's Morale is too low to handle the rigors of this position, which will cause them Stress over time.";

			public static LocString HIGHEST_EXPECTATIONS_TIER = "<b>Highest Expectations</b>";

			public static LocString ADDED_EXPECTATIONS_AMOUNT = " (+{0} Expectation)";
		}

		public class DEBUG_TOOLS
		{
			public class PAINT_ELEMENTS_SCREEN
			{
				public static LocString TITLE = "CELL PAINTER";

				public static LocString ELEMENT = "Element";

				public static LocString MASS_KG = "Mass (kg)";

				public static LocString TEMPERATURE_KELVIN = "Temperature (K)";

				public static LocString DISEASE = "Disease";

				public static LocString DISEASE_COUNT = "Disease Count";

				public static LocString BUILDINGS = "Buildings:";

				public static LocString CELLS = "Cells:";

				public static LocString ADD_FOW_MASK = "Prevent FoW Reveal";

				public static LocString REMOVE_FOW_MASK = "Allow FoW Reveal";

				public static LocString PAINT = "Paint";

				public static LocString SAMPLE = "Sample";

				public static LocString FILL = "Fill";
			}

			public class SAVE_BASE_TEMPLATE
			{
				public class SELECTION_INFO_PANEL
				{
					public static LocString TOTAL_MASS = "Total mass: {0}";

					public static LocString AVERAGE_MASS = "Average cell mass: {0}";

					public static LocString AVERAGE_TEMPERATURE = "Average temperature: {0}";

					public static LocString TOTAL_JOULES = "Total joules: {0}";

					public static LocString JOULES_PER_KILOGRAM = "Joules per kilogram: {0}";
				}

				public static LocString TITLE = "Base and World Tools";

				public static LocString SAVE_TITLE = "Save Selection";

				public static LocString CLEAR_BUTTON = "Clear Floor";

				public static LocString DESTROY_BUTTON = "Destroy";

				public static LocString DECONSTRUCT_BUTTON = "Deconstruct";

				public static LocString CLEAR_SELECTION_BUTTON = "Clear Selection";

				public static LocString DEFAULT_SAVE_NAME = "TemplateSaveName";

				public static LocString MORE = "More";
			}

			public static LocString ENTER_TEXT = string.Empty;

			public static LocString INVALID_LOCATION = "Invalid Location";
		}

		public class WORLDGEN
		{
			public static LocString NOHEADERS = string.Empty;

			public static LocString COMPLETE = "Success! Space adventure awaits.";

			public static LocString FAILED = "Goodness, has this ever gone terribly wrong!";

			public static LocString RESTARTING = "Rebooting...";

			public static LocString LOADING = "Loading world...";

			public static LocString GENERATINGWORLD = "The Galaxy Synthesizer";

			public static LocString CHOOSEWORLDSIZE = "Select the magnitude of your new galaxy.";

			public static LocString USING_PLAYER_SEED = "Using selected world gen seed: ";

			public static LocString CLEARINGLEVEL = "Staring into the void...";

			public static LocString RETRYCOUNT = "Oh dear, let's try that again.";

			public static LocString GENERATESOLARSYSTEM = "Catalyzing Big Bang...";

			public static LocString GENERATESOLARSYSTEM1 = "Catalyzing Big Bang...";

			public static LocString GENERATESOLARSYSTEM2 = "Catalyzing Big Bang...";

			public static LocString GENERATESOLARSYSTEM3 = "Catalyzing Big Bang...";

			public static LocString GENERATESOLARSYSTEM4 = "Catalyzing Big Bang...";

			public static LocString GENERATESOLARSYSTEM5 = "Catalyzing Big Bang...";

			public static LocString GENERATESOLARSYSTEM6 = "Approaching event horizon...";

			public static LocString GENERATESOLARSYSTEM7 = "Approaching event horizon...";

			public static LocString GENERATESOLARSYSTEM8 = "Approaching event horizon...";

			public static LocString GENERATESOLARSYSTEM9 = "Approaching event horizon...";

			public static LocString SETUPNOISE = "BANG!";

			public static LocString BUILDNOISESOURCE = "Sorting quadrillions of atoms...";

			public static LocString BUILDNOISESOURCE1 = "Sorting quadrillions of atoms...";

			public static LocString BUILDNOISESOURCE2 = "Sorting quadrillions of atoms...";

			public static LocString BUILDNOISESOURCE3 = "Ironing the fabric of creation...";

			public static LocString BUILDNOISESOURCE4 = "Ironing the fabric of creation...";

			public static LocString BUILDNOISESOURCE5 = "Ironing the fabric of creation...";

			public static LocString BUILDNOISESOURCE6 = "Taking hot meteor shower...";

			public static LocString BUILDNOISESOURCE7 = "Tightening asteroid belts...";

			public static LocString BUILDNOISESOURCE8 = "Tightening asteroid belts...";

			public static LocString BUILDNOISESOURCE9 = "Tightening asteroid belts...";

			public static LocString GENERATENOISE = "Baking igneous rock...";

			public static LocString GENERATENOISE1 = "Multilayering sediment...";

			public static LocString GENERATENOISE2 = "Multilayering sediment...";

			public static LocString GENERATENOISE3 = "Multilayering sediment...";

			public static LocString GENERATENOISE4 = "Superheating gases...";

			public static LocString GENERATENOISE5 = "Superheating gases...";

			public static LocString GENERATENOISE6 = "Superheating gases...";

			public static LocString GENERATENOISE7 = "Vacuuming out vacuums...";

			public static LocString GENERATENOISE8 = "Vacuuming out vacuums...";

			public static LocString GENERATENOISE9 = "Vacuuming out vacuums...";

			public static LocString NORMALISENOISE = "Interpolating toxic gas...";

			public static LocString WORLDLAYOUT = "Freezing ice formations...";

			public static LocString WORLDLAYOUT1 = "Freezing ice formations...";

			public static LocString WORLDLAYOUT2 = "Freezing ice formations...";

			public static LocString WORLDLAYOUT3 = "Freezing ice formations...";

			public static LocString WORLDLAYOUT4 = "Melting magma...";

			public static LocString WORLDLAYOUT5 = "Melting magma...";

			public static LocString WORLDLAYOUT6 = "Melting magma...";

			public static LocString WORLDLAYOUT7 = "Sprinkling sand...";

			public static LocString WORLDLAYOUT8 = "Sprinkling sand...";

			public static LocString WORLDLAYOUT9 = "Sprinkling sand...";

			public static LocString WORLDLAYOUT10 = "Sprinkling sand...";

			public static LocString COMPLETELAYOUT = "Cooling glass...";

			public static LocString COMPLETELAYOUT1 = "Cooling glass...";

			public static LocString COMPLETELAYOUT2 = "Cooling glass...";

			public static LocString COMPLETELAYOUT3 = "Cooling glass...";

			public static LocString COMPLETELAYOUT4 = "Digging holes...";

			public static LocString COMPLETELAYOUT5 = "Digging holes...";

			public static LocString COMPLETELAYOUT6 = "Digging holes...";

			public static LocString COMPLETELAYOUT7 = "Adding buckets of dirt...";

			public static LocString COMPLETELAYOUT8 = "Adding buckets of dirt...";

			public static LocString COMPLETELAYOUT9 = "Adding buckets of dirt...";

			public static LocString COMPLETELAYOUT10 = "Adding buckets of dirt...";

			public static LocString PROCESSRIVERS = "Pouring rivers...";

			public static LocString CONVERTTERRAINCELLSTOEDGES = "Hardening diamonds...";

			public static LocString PROCESSING = "Embedding metals...";

			public static LocString PROCESSING1 = "Embedding metals...";

			public static LocString PROCESSING2 = "Embedding metals...";

			public static LocString PROCESSING3 = "Burying precious ore...";

			public static LocString PROCESSING4 = "Burying precious ore...";

			public static LocString PROCESSING5 = "Burying precious ore...";

			public static LocString PROCESSING6 = "Burying precious ore...";

			public static LocString PROCESSING7 = "Excavating tunnels...";

			public static LocString PROCESSING8 = "Excavating tunnels...";

			public static LocString PROCESSING9 = "Excavating tunnels...";

			public static LocString BORDERS = "Just adding water...";

			public static LocString BORDERS1 = "Just adding water...";

			public static LocString BORDERS2 = "Staring at the void...";

			public static LocString BORDERS3 = "Staring at the void...";

			public static LocString BORDERS4 = "Staring at the void...";

			public static LocString BORDERS5 = "Avoiding awkward eye contact with the void...";

			public static LocString BORDERS6 = "Avoiding awkward eye contact with the void...";

			public static LocString BORDERS7 = "Avoiding awkward eye contact with the void...";

			public static LocString BORDERS8 = "Avoiding awkward eye contact with the void...";

			public static LocString BORDERS9 = "Avoiding awkward eye contact with the void...";

			public static LocString DRAWWORLDBORDER = "Establishing personal boundaries...";

			public static LocString SETTLESIM = "Infusing oxygen...";

			public static LocString SETTLESIM1 = "Infusing oxygen...";

			public static LocString SETTLESIM2 = "Too much oxygen. Removing...";

			public static LocString SETTLESIM3 = "Too much oxygen. Removing...";

			public static LocString SETTLESIM4 = "Ideal oxygen levels achieved...";

			public static LocString SETTLESIM5 = "Ideal oxygen levels achieved...";

			public static LocString SETTLESIM6 = "Planting space flora...";

			public static LocString SETTLESIM7 = "Planting space flora...";

			public static LocString SETTLESIM8 = "Releasing wildlife...";

			public static LocString SETTLESIM9 = "Releasing wildlife...";

			public static LocString ANALYZINGWORLD = "Shuffling DNA Blueprints...";

			public static LocString ANALYZINGWORLDCOMPLETE = "Tidying up for the Duplicants...";

			public static LocString PLACINGCREATURES = "Building the suspense...";
		}

		public class TOOLTIPS
		{
			public static LocString MANAGEMENTMENU_JOBS = "Manage my Duplicants' work behavior";

			public static LocString MANAGEMENTMENU_CONSUMABLES = "Manage colony diets";

			public static LocString MANAGEMENTMENU_VITALS = "View my Duplicants' vitals";

			public static LocString MANAGEMENTMENU_RESEARCH = "View the Research Tree";

			public static LocString MANAGEMENTMENU_DAILYREPORT = "View each cycle's Colony Report";

			public static LocString MANAGEMENTMENU_REQUIRES_RESEARCH = "Build a Research Station to unlock" + HORIZONTAL_BR_RULE + "Research buildings can be found in the Stations Tab <color=#F44A47>[0]</color> of the Build Menu";

			public static LocString MANAGEMENTMENU_REQUIRES_ROLES_STATION = "Build a Jobs Board to unlock" + HORIZONTAL_BR_RULE + "The Jobs Board can be found in the Stations Tab <color=#F44A47>[0]</color> of the Build Menu";

			public static LocString MANAGEMENTMENU_REQUIRES_TELESCOPE = "Build a " + BUILDINGS.PREFABS.TELESCOPE.NAME + " to unlock" + HORIZONTAL_BR_RULE + "The " + BUILDINGS.PREFABS.TELESCOPE.NAME + " can be found in the Stations Tab <color=#F44A47>[0]</color> of the Build Menu";

			public static LocString MANAGEMENTMENU_CODEX = "Browse entries in my Database";

			public static LocString MANAGEMENTMENU_SCHEDULE = "Adjust the colony's time usage";

			public static LocString MANAGEMENTMENU_STARMAP = "Manage astronaut rocket missions";

			public static LocString OPEN_CODEX_ENTRY = "View full entry in database";

			public static LocString NO_CODEX_ENTRY = "No database entry available";

			public static LocString MANAGEMENTMENU_ROLES = "Manage Duplicants' job assignments";

			public static LocString METERSCREEN_AVGSTRESS = "Highest Stress: {0}";

			public static LocString METERSCREEN_MEALHISTORY = "Calories Available: {0}";

			public static LocString METERSCREEN_POPULATION = "Population: {0}";

			public static LocString METERSCREEN_IMMUNITY_LEVELS = "Immune Systems: {0}";

			public static LocString PLAYBUTTON = "Start";

			public static LocString PAUSEBUTTON = "Pause";

			public static LocString PAUSE = "Pause";

			public static LocString UNPAUSE = "Unpause";

			public static LocString SPEEDBUTTON_SLOW = "Slow speed {0}";

			public static LocString SPEEDBUTTON_MEDIUM = "Medium speed {0}";

			public static LocString SPEEDBUTTON_FAST = "Fast speed {0}";

			public static LocString RED_ALERT_TITLE = "Toggle Red Alert";

			public static LocString RED_ALERT_CONTENT = "Duplicants will work, ignoring schedules and their basic needs\n\nUse in case of emergency";

			public static LocString DISINFECTBUTTON = "Drag to disinfect buildings";

			public static LocString MOPBUTTON = "Drag to mop small spills";

			public static LocString DIGBUTTON = "Drag to set dig errands";

			public static LocString CANCELBUTTON = "Drag to cancel errands";

			public static LocString DECONSTRUCTBUTTON = "Drag to demolish buildings";

			public static LocString ATTACKBUTTON = "Drag to attack poor, wild critters";

			public static LocString CAPTUREBUTTON = "Drag to capture critters";

			public static LocString CLEARBUTTON = "Drag to move debris into storage";

			public static LocString HARVESTBUTTON = "Drag to harvest fullgrown plants";

			public static LocString PRIORITIZEMAINBUTTON = string.Empty;

			public static LocString PRIORITIZEBUTTON = "Drag to set errand Sub-Priority";

			public static LocString CLEANUPMAINBUTTON = "Mop and sweep messy floors";

			public static LocString CANCELDECONSTRUCTIONBUTTON = "Cancel queued orders or deconstruct existing buildings";

			public static LocString HELP_ROTATE_KEY = "Press <color=#F44A47>[{Key}]</color> to Rotate";

			public static LocString HELP_BUILDLOCATION_INVALID_CELL = "Invalid Cell";

			public static LocString HELP_BUILDLOCATION_FLOOR = "Must be built on solid ground";

			public static LocString HELP_BUILDLOCATION_FLOOR_OR_ATTACHPOINT = "Must be built on solid ground or overlapping an {0}";

			public static LocString HELP_BUILDLOCATION_OCCUPIED = "Must be built in unoccupied space";

			public static LocString HELP_BUILDLOCATION_CEILING = "Must be built on the ceiling";

			public static LocString HELP_BUILDLOCATION_INSIDEGROUND = "Must be built in the ground";

			public static LocString HELP_BUILDLOCATION_ATTACHPOINT = "Must be built overlapping an {0}";

			public static LocString HELP_BUILDLOCATION_SPACE = "Must be built on the surface";

			public static LocString HELP_BUILDLOCATION_NOT_IN_TILES = "Cannot be built inside tile";

			public static LocString HELP_BUILDLOCATION_GASPORTS_OBSTRUCTED = "Gas ports are obstructed";

			public static LocString HELP_BUILDLOCATION_LIQUIDPORTS_OBSTRUCTED = "Liquid ports are obstructed";

			public static LocString HELP_BUILDLOCATION_SOLIDPORTS_OBSTRUCTED = "Solid ports are obstructed";

			public static LocString HELP_BUILDLOCATION_WIRE_OBSTRUCTION = "Obstructed by wire";

			public static LocString HELP_BUILDLOCATION_PLATE_OBSTRUCTION = "Obstructed by Tempshift Plate";

			public static LocString HELP_BUILDLOCATION_LOGIC_PORTS_OBSTRUCTED = "Automation ports are obstructed";

			public static LocString HELP_TUBELOCATION_NO_UTURNS = "Can't U-Turn";

			public static LocString HELP_TUBELOCATION_STRAIGHT_BRIDGES = "Can't Turn Here";

			public static LocString HELP_REQUIRES_ROOM = "Must be in a room";

			public static LocString OXYGENOVERLAYSTRING = "Displays ambient oxygen density";

			public static LocString POWEROVERLAYSTRING = "Displays power grid components";

			public static LocString TEMPERATUREOVERLAYSTRING = "Displays ambient temperature";

			public static LocString HEATFLOWOVERLAYSTRING = "Displays comfortable temperatures for Duplicants";

			public static LocString SUITOVERLAYSTRING = "Displays Exosuits and related buildings";

			public static LocString LOGICOVERLAYSTRING = "Displays automation grid components";

			public static LocString ROOMSOVERLAYSTRING = "Displays special purpose rooms and bonuses";

			public static LocString JOULESOVERLAYSTRING = "Displays the thermal energy in each cell";

			public static LocString LIGHTSOVERLAYSTRING = "Displays the visibility radius of light sources";

			public static LocString LIQUIDVENTOVERLAYSTRING = "Displays liquid pipe system components";

			public static LocString GASVENTOVERLAYSTRING = "Displays gas pipe system components";

			public static LocString DECOROVERLAYSTRING = "Displays areas with Morale-boosting decor values";

			public static LocString PRIORITIESOVERLAYSTRING = "Displays errand sub-priority values";

			public static LocString DISEASEOVERLAYSTRING = "Displays areas of disease risk";

			public static LocString NOISE_POLLUTION_OVERLAY_STRING = "Displays ambient noise levels";

			public static LocString CROPS_OVERLAY_STRING = "Displays plant growth progress";

			public static LocString CONVEYOR_OVERLAY_STRING = "Displays conveyor transport components";

			public static LocString REACHABILITYOVERLAYSTRING = "Displays areas accessible by Duplicants";

			public static LocString ENERGYREQUIRED = FormatAsLink("Power", "POWER") + " Required";

			public static LocString ENERGYGENERATED = FormatAsLink("Power", "POWER") + " Produced";

			public static LocString INFOPANEL = "The Info Panel contains an overview of the basic information about my Duplicant";

			public static LocString VITALSPANEL = "The Vitals Panel monitors the status and well being of my Duplicant";

			public static LocString STRESSPANEL = "The Stress Panel offers a detailed look at what is psychology affecting Duplicant psychologically";

			public static LocString STATSPANEL = "The Stats Panel gives me an overview of my Duplicant's individual stats";

			public static LocString ITEMSPANEL = "The Items Panel displays everything this Duplicant is in possession of";

			public static LocString STRESSDESCRIPTION = "Accommodate my Duplicant's needs to manage their " + FormatAsLink("Stress", "STRESS") + ".\n\nLow " + FormatAsLink("Stress", "STRESS") + " can provide a productivity boost, while high " + FormatAsLink("Stress", "STRESS") + " can impair production or even lead to a nervous breakdown.";

			public static LocString ALERTSTOOLTIP = "Alerts provide important information about what's happening in the colony right now";

			public static LocString MESSAGESTOOLTIP = "Messages are events that have happened and tips to help me manage my colony";

			public static LocString NEXTMESSAGESTOOLTIP = "Next message";

			public static LocString CLOSETOOLTIP = "Close";

			public static LocString DISMISSMESSAGE = "Dismiss message";

			public static LocString RECIPE_QUEUE = "Queue 1 {0} for fabrication";

			public static LocString RECIPE_QUEUE_INFINITE = "Continuously fabricate {0} if resources are available";

			public static LocString RED_ALERT_BUTTON_ON = "Enable Red Alert";

			public static LocString RED_ALERT_BUTTON_OFF = "Disable Red Alert";

			public static LocString JOBSSCREEN_PRIORITY = "Urgent errands are always performed before non-urgent errands.\n\nHowever, a busy Duplicant will continue to work on their current errand until it's complete, even if a more urgent errand becomes available.";

			public static LocString JOBSSCREEN_ATTRIBUTES = "The following attributes affect a Duplicant's efficiency at this errand:";

			public static LocString JOBSSCREEN_CANNOTPERFORMTASK = "{0} cannot perform this errand.";

			public static LocString JOBSSCREEN_RELEVANT_ATTRIBUTES = "Relevant Attributes:";

			public static LocString SORTCOLUMN = "Click to sort";

			public static LocString NOMATERIAL = "Not enough materials";

			public static LocString SELECTAMATERIAL = "There are insufficient materials to construct this building";

			public static LocString EDITNAME = "Give this Duplicant a new name";

			public static LocString RANDOMIZENAME = "Randomize this Duplicant's name";

			public static LocString EDITCREATURENAME = "Give this lil' critter a new name";

			public static LocString RANDOMIZECREATURENAME = "Randomize this critter's name";

			public static LocString BASE_VALUE = "Base Value";

			public static LocString MATIERIAL_MOD = "Made out of {0}";

			public static LocString VITALS_CHECKBOX_TEMPERATURE = "This plant's internal temperature is {temperature}";

			public static LocString VITALS_CHECKBOX_PRESSURE = "The current air pressure is {pressure}";

			public static LocString VITALS_CHECKBOX_ATMOSPHERE = "This plant is immersed in {element}";

			public static LocString VITALS_CHECKBOX_ILLUMINATION_DARK = "This plant is currently in the dark";

			public static LocString VITALS_CHECKBOX_ILLUMINATION_LIGHT = "This plant is currently lit";

			public static LocString VITALS_CHECKBOX_FERTILIZER = "{mass} of fertilizer is currently available";

			public static LocString VITALS_CHECKBOX_IRRIGATION = "{mass} of liquid is currently available";

			public static LocString VITALS_CHECKBOX_SUBMERGED_TRUE = "This plant is fully submerged in liquid";

			public static LocString VITALS_CHECKBOX_SUBMERGED_FALSE = "This plant must be submerged in liquid";

			public static LocString VITALS_CHECKBOX_DROWNING_TRUE = "This plant is not drowning";

			public static LocString VITALS_CHECKBOX_DROWNING_FALSE = "This plant is drowning in liquid";

			public static LocString VITALS_CHECKBOX_RECEPTACLE_OPERATIONAL = "This plant is housed in an operational farm plot";

			public static LocString VITALS_CHECKBOX_RECEPTACLE_INOPERATIONAL = "This plant is not housed in an operational farm plot";
		}

		public class STARMAP
		{
			public class DESTINATIONSTUDY
			{
				public static LocString UPPERATMO = "Study upper atmosphere";

				public static LocString LOWERATMO = "Study lower atmosphere";

				public static LocString MAGNETICFIELD = "Study magnetic field";

				public static LocString SURFACE = "Study surface";

				public static LocString SUBSURFACE = "Study subsurface";
			}

			public class COMPONENT
			{
				public static LocString FUEL_TANK = "Fuel Tank";

				public static LocString ROCKET_ENGINE = "Rocket Engine";

				public static LocString CARGO_BAY = "Cargo Bay";
			}

			public class MISSION_STATUS
			{
				public static LocString GROUNDED = "Grounded";

				public static LocString LAUNCHING = "Launching";

				public static LocString WAITING_TO_LAND = "Waiting To Land";

				public static LocString UNDERWAY = "Underway";

				public static LocString GO = "ALL SYSTEMS GO";
			}

			public class LISTTITLES
			{
				public static LocString MISSIONSTATUS = "Mission Status";

				public static LocString LAUNCHCHECKLIST = "Launch Checklist";

				public static LocString MAXRANGE = "Max Range";

				public static LocString MASS = "Mass";

				public static LocString STORAGE = "Storage";

				public static LocString FUEL = "Fuel";

				public static LocString OXIDIZER = "Oxidizer";

				public static LocString PASSENGERS = "Passengers";

				public static LocString RESEARCH = "Research";

				public static LocString ANALYSIS = "Analysis";

				public static LocString WORLDCOMPOSITION = "World Composition";

				public static LocString RESOURCES = "Resources";

				public static LocString MODULES = "Modules";

				public static LocString TYPE = "Type";

				public static LocString DISTANCE = "Distance";
			}

			public class ROCKETWEIGHT
			{
				public static LocString MASS = "Mass: ";

				public static LocString MASSPENALTY = "Mass Penalty: ";

				public static LocString CURRENTMASS = "Current Rocket Mass: ";

				public static LocString CURRENTMASSPENALTY = "Current Weight Penalty: ";
			}

			public class DESTINATIONSELECTION
			{
				public static LocString REACHABLE = "Destination selected";

				public static LocString UNREACHABLE = "Destination beyond reach";

				public static LocString NOTSELECTED = "Destination not selected";
			}

			public class DESTINATIONSELECTION_TOOLTIP
			{
				public static LocString REACHABLE = "Viable destination selected, ready for launch";

				public static LocString UNREACHABLE = "The selected destination is beyond rocket reach";

				public static LocString NOTSELECTED = "Select a destination from the starmap";
			}

			public class HASFOOD
			{
				public static LocString NAME = "Food Loaded";

				public static LocString TOOLTIP = "Sufficient food stores have been loaded, ready for launch";
			}

			public class HASSUIT
			{
				public static LocString NAME = "Has " + EQUIPMENT.PREFABS.ATMO_SUIT.NAME;

				public static LocString TOOLTIP = "An " + EQUIPMENT.PREFABS.ATMO_SUIT.NAME + " has been loaded";
			}

			public class NOSUIT
			{
				public static LocString NAME = "Missing " + EQUIPMENT.PREFABS.ATMO_SUIT.NAME;

				public static LocString TOOLTIP = "Rocket cannot launch without an " + EQUIPMENT.PREFABS.ATMO_SUIT.NAME + " loaded";
			}

			public class NOFOOD
			{
				public static LocString NAME = "Insufficient Food";

				public static LocString TOOLTIP = "Rocket cannot launch without adequate food stores for passengers";
			}

			public class CARGOEMPTY
			{
				public static LocString NAME = "Cargo Bay not empty";

				public static LocString TOOLTIP = "Cargo Bays must be emptied of all materials before launch";
			}

			public class LAUNCHCHECKLIST
			{
				public static LocString ASTRONAUT_TITLE = "Astronaut";

				public static LocString HASASTRONAUT = "Astronaut ready for liftoff";

				public static LocString ASTRONAUGHT = "No Astronaut assigned";

				public static LocString INSTALLED = "Installed";

				public static LocString INSTALLED_TOOLTIP = "{0} installed, ready for launch";

				public static LocString REQUIRED = "Required";

				public static LocString REQUIRED_TOOLTIP = "A {0} must be installed before launch";
			}

			public class FULLTANK
			{
				public static LocString NAME = "Fuel Tank full";

				public static LocString TOOLTIP = "Tank is full, ready to launch";
			}

			public class EMPTYTANK
			{
				public static LocString NAME = "Fuel Tank not full";

				public static LocString TOOLTIP = "Fuel Tank must be filled before launch";
			}

			public class FULLOXIDIZERTANK
			{
				public static LocString NAME = "Oxidizer tank full";

				public static LocString TOOLTIP = "Tank is full, ready to launch";
			}

			public class EMPTYOXIDIZERTANK
			{
				public static LocString NAME = "Oxidizer tank not full";

				public static LocString TOOLTIP = "Oxidizer tank must be filled before launch";
			}

			public class ROCKETSTATUS
			{
				public static LocString STATUS_TITLE = "Rocket Status";

				public static LocString NONE = "NONE";

				public static LocString SELECTED = "SELECTED";

				public static LocString NODESTINATION = "No destination selected";

				public static LocString DESTINATIONVALUE = "None";

				public static LocString NOPASSENGERS = "No passengers";

				public static LocString STATUS = "Status";

				public static LocString TOTAL = "Total";

				public static LocString WEIGHTPENALTY = "Weight Penalty";

				public static LocString TIMEREMAINING = "Time Remaining";
			}

			public class ROCKETSTATS
			{
				public static LocString TOTAL_OXIDIZABLE_FUEL = "Total oxidizable fuel";

				public static LocString TOTAL_OXIDIZER = "Total oxidizer";

				public static LocString TOTAL_FUEL = "Total fuel";

				public static LocString ENGINE_EFFICIENCY = "Main engine efficiency";

				public static LocString OXIDIZER_EFFICIENCY = "Average oxidizer efficiency";

				public static LocString SOLID_BOOSTER = "Solid boosters";

				public static LocString TOTAL_THRUST = "Total thrust";

				public static LocString TOTAL_RANGE = "Total range";

				public static LocString DRY_MASS = "Dry mass";

				public static LocString WET_MASS = "Wet mass";
			}

			public static LocString TITLE = "Starmap";

			public static LocString MANAGEMENT_BUTTON = "STARMAP";

			public static LocString SUBROW = "•  {0}";

			public static LocString UNKNOWN_DESTINATION = "Destination Unknown";

			public static LocString ANALYSIS_AMOUNT = "Analysis {0} Complete";

			public static LocString ANALYSIS_COMPLETE = "ANALYSIS COMPLETE";

			public static LocString NO_ANALYZABLE_DESTINATION_SELECTED = "No destination selected";

			public static LocString UNKNOWN_TYPE = "Type Unknown";

			public static LocString DISTANCE = "{0} km";

			public static LocString MODULE_MASS = "+ {0} t";

			public static LocString MODULE_STORAGE = "{0} / {1}";

			public static LocString ANALYSIS_DESCRIPTION = "Use a Telescope to analyze space destinations.\n\nCompleting analysis on an object will unlock rocket missions to that destination.";

			public static LocString RESEARCH_DESCRIPTION = "Gather Interstellar Research Data using Research Modules.\n\nResearch Modules Installed: {0}";

			public static LocString ROCKET_RENAME_BUTTON_TOOLTIP = "Rename this rocket";

			public static LocString NO_ROCKETS_HELP_TEXT = "Rockets allow you to visit nearby celestial bodies.\n\nEach rocket must have a Command Module, an Engine, and Fuel.\n\nYou can also carry other modules that allow you to gather specific resources from the places you visit.\n\nRemember the more weight a rocket has, the more limited it'll be on the distance it can travel. You can add more fuel to fix that, but fuel will add weight as well.";

			public static LocString CONTAINER_REQUIRED = "{0} installation required to retrieve material";

			public static LocString CAN_CARRY_ELEMENT = "Gathered by: {1}";

			public static LocString CANT_CARRY_ELEMENT = "{0} installation required to retrieve material";

			public static LocString STATUS = "SELECTED";

			public static LocString DISTANCE_OVERLAY = "TOO FAR FOR THIS ROCKET";

			public static LocString COMPOSITION_UNDISCOVERED = "?????????";

			public static LocString COMPOSITION_UNDISCOVERED_TOOLTIP = "Further research required to identify resource" + HORIZONTAL_BR_RULE + "Send a Research Module to this destination for more information";

			public static LocString COMPOSITION_UNDISCOVERED_AMOUNT = "???";

			public static LocString COMPOSITION_SMALL_AMOUNT = "Trace Amount";

			public static LocString ROCKETLIST = "Rocket Hangar";

			public static LocString NO_ROCKETS_TITLE = "NO ROCKETS";

			public static LocString ROCKET_COUNT = "ROCKETS: {0}";

			public static LocString LAUNCH_MISSION = "LAUNCH MISSION";

			public static LocString LAUNCH_ROCKET = "Launch Rocket";

			public static LocString LAND_ROCKET = "Land Rocket";

			public static LocString SEE_ROCKETS_LIST = "See Rockets List";

			public static LocString DEFAULT_NAME = "Rocket";

			public static LocString ANALYZE_DESTINATION = "ANALYZE OBJECT";

			public static LocString SUSPEND_DESTINATION_ANALYSIS = "PAUSE ANALYSIS";

			public static LocString DESTINATIONTITLE = "Destination Status";
		}

		public class CODEX
		{
			public class CODEX_DISCOVERED_MESSAGE
			{
				public static LocString TITLE = "Codex Discovered";

				public static LocString BODY = "Codex Discovered: {codex}";
			}

			public class GEYSERS
			{
				public static LocString DESC = "Geysers and Fumaroles emit elements at variable intervals.They provide a sustainable, though usually low volume, source of material.\n\nThe variable factors of a geyser are:\n\n    • Emission element \n    • Emission temperature \n    • Emission mass \n    • Cycle length \n    • Dormancy duration \n    • Disease emitted";
			}

			public class FOOD
			{
				public static LocString QUALITY = "Quality: {0}";

				public static LocString CALORIES = "Calories: {0}";

				public static LocString SPOILPROPERTIES = "Preserve temperature: {0}\nSpoil time: {1}";

				public static LocString NON_PERISHABLE = "Spoil time: Never";
			}

			public class CATEGORYNAMES
			{
				public static LocString ROOT = FormatAsLink("Index", "HOME");

				public static LocString PLANTS = FormatAsLink("Plants", "PLANTS");

				public static LocString CREATURES = FormatAsLink("Critters", "CREATURES");

				public static LocString EMAILS = FormatAsLink("E-mail", "EMAILS");

				public static LocString JOURNALS = FormatAsLink("Journals", "JOURNALS");

				public static LocString MYLOG = FormatAsLink("My Log", "MYLOG");

				public static LocString RESEARCHNOTES = FormatAsLink("Research Notes", "RESEARCHNOTES");

				public static LocString NOTICES = FormatAsLink("Notices", "NOTICES");

				public static LocString FOOD = FormatAsLink("Food", "FOOD");

				public static LocString BUILDINGS = FormatAsLink("Buildings", "BUILDINGS");

				public static LocString TECH = FormatAsLink("Research", "TECH");

				public static LocString TIPS = FormatAsLink("Tips", "TIPS");

				public static LocString ELEMENTS = FormatAsLink("Elements", "ELEMENTS");

				public static LocString ELEMENTSSOLID = FormatAsLink("Solids", "ELEMENTSSOLID");

				public static LocString ELEMENTSGAS = FormatAsLink("Gases", "ELEMENTSGAS");

				public static LocString ELEMENTSLIQUID = FormatAsLink("Liquids", "ELEMENTSLIQUID");

				public static LocString ELEMENTSOTHER = FormatAsLink("Other", "ELEMENTSOTHER");

				public static LocString GEYSERS = FormatAsLink("Geysers", "GEYSERS");

				public static LocString SYSTEMS = FormatAsLink("Systems", "SYSTEMS");

				public static LocString ROLES = FormatAsLink("Duplicant Jobs", "ROLES");

				public static LocString DISEASE = FormatAsLink("Disease", "DISEASE");
			}

			public static LocString SEARCH_HEADER = "Search Database";

			public static LocString BACK_BUTTON = "Back ({0})";

			public static LocString TIPS = "Tips";

			public static LocString DETAILS = "Details";

			public static LocString RECIPE_ITEM = "{0} x {1}{2}";

			public static LocString RECIPE_FABRICATOR = "{1} ({0} seconds)";

			public static LocString RECIPE_FABRICATOR_HEADER = "Produced by";

			public static LocString TITLE = "DATABASE";

			public static LocString MANAGEMENT_BUTTON = "DATABASE";
		}

		public class DEVELOPMENTBUILDS
		{
			public class ALPHA
			{
				public class MESSAGES
				{
					public static LocString HEADER = "DEVELOPMENT BUILD";

					public static LocString BODY = "Stay up to date by joining our mailing list, or head on over to the forums and join the discussion.";

					public static LocString FORUMBUTTON = "FORUMS";

					public static LocString MAILINGLIST = "NOTIFY ME!";

					public static LocString PATCHNOTES = "PATCH NOTES";

					public static LocString ANIMATION_HEADER = "WATCH THE ANIMATED SHORT!";
				}

				public class LOADING
				{
					public static LocString TITLE = "<b>Welcome to Oxygen Not Included!</b>";

					public static LocString BODY = "This game is in the early stages of development which means you're likely to encounter strange, amusing, and occasionally just downright frustrating bugs.\n\nDuring this time Oxygen Not Included will be receiving regular updates to fix bugs, add features, and introduce additional content, so if you encounter issues or just have suggestions to share, please let us know on our forums: <u>http://forums.kleientertainment.com</u>\n\nA special thanks to those who joined us during our time in Alpha. We value your feedback and thank you for joining us in the development process. We couldn't do this without you.\n\nEnjoy your time in deep space!\n\n- Klei";

					public static LocString CONTINUEBUTTON = "Okay, thanks for the heads up!";
				}

				public class HEALTHY_MESSAGE
				{
					public static LocString CONTINUEBUTTON = "Thanks!";
				}
			}

			public class UPDATES
			{
				public static LocString UPDATES_HEADER = "NEXT UPGRADE LIVE IN";

				public static LocString NOW = "Less than a day";

				public static LocString TWENTY_FOUR_HOURS = "Less than a day";

				public static LocString FINAL_WEEK = "{0} days";

				public static LocString BIGGER_TIMES = "{1} weeks {0} days";
			}

			public static LocString WATERMARK = "DEVELOPMENT BUILD: {0}";

			public static LocString FULL_PATCH_NOTES = "Full Patch Notes";

			public static LocString PREVIOUS_VERSION = "Previous Version";
		}

		public class UNITSUFFIXES
		{
			public class MASS
			{
				public static LocString TONNE = " t";

				public static LocString KILOGRAM = " Kg";

				public static LocString GRAM = " g";

				public static LocString MILLIGRAM = " mg";

				public static LocString MICROGRAM = " mcg";

				public static LocString POUND = " lb";

				public static LocString DRACHMA = " dr";

				public static LocString GRAIN = " gr";
			}

			public class TEMPERATURE
			{
				public static LocString CELSIUS = " " + 186.ToString() + "C";

				public static LocString FAHRENHEIT = " " + 186.ToString() + "F";

				public static LocString KELVIN = " " + 186.ToString() + "K";
			}

			public class CALORIES
			{
				public static LocString CALORIE = " cal";

				public static LocString KILOCALORIE = " kcal";
			}

			public class ELECTRICAL
			{
				public static LocString JOULE = " J";

				public static LocString KILOJOULE = " kJ";

				public static LocString MEGAJOULE = " MJ";

				public static LocString WATT = " W";

				public static LocString KILOWATT = " kW";
			}

			public class HEAT
			{
				public static LocString DTU = " DTU";

				public static LocString KDTU = " kDTU";

				public static LocString DTU_S = " DTU/s";

				public static LocString KDTU_S = " kDTU/s";
			}

			public class DISTANCE
			{
				public static LocString METER = " m";

				public static LocString KILOMETER = " km";
			}

			public class DISEASE
			{
				public static LocString UNITS = " germs";
			}

			public class NOISE
			{
				public static LocString UNITS = " dB";
			}

			public class INFORMATION
			{
				public static LocString KILOBYTE = "kB";

				public static LocString MEGABYTE = "MB";

				public static LocString GIGABYTE = "GB";
			}

			public static LocString SECOND = " s";

			public static LocString PERSECOND = "/s";

			public static LocString PERCYCLE = "/cycle";

			public static LocString UNITS = " units";

			public static LocString PERCENT = "%";
		}

		public class OVERLAYS
		{
			public class OXYGEN
			{
				public class TOOLTIPS
				{
					public static LocString LEGEND1 = "<b>Very Breathable</b>\nHigh oxygen concentrations";

					public static LocString LEGEND2 = "<b>Breathable</b>\nSufficient oxygen concentrations";

					public static LocString LEGEND3 = "<b>Barely Breathable</b>\nLow oxygen concentrations";

					public static LocString LEGEND4 = "<b>Unbreathable</b>\nExtremely low or absent oxygen concentrations" + HORIZONTAL_BR_RULE + "Duplicants will suffocate if trapped in these areas";

					public static LocString LEGEND5 = "<b>Slightly Toxic</b>\nHarmful gas concentration";

					public static LocString LEGEND6 = "<b>Very Toxic</b>\nLethal gas concentration";
				}

				public static LocString NAME = "OXYGEN OVERLAY";

				public static LocString BUTTON = "Oxygen Overlay";

				public static LocString LEGEND1 = "Very Breathable";

				public static LocString LEGEND2 = "Breathable";

				public static LocString LEGEND3 = "Barely Breathable";

				public static LocString LEGEND4 = "Unbreathable";

				public static LocString LEGEND5 = "Slightly Toxic";

				public static LocString LEGEND6 = "Very Toxic";
			}

			public class ELECTRICAL
			{
				public class TOOLTIPS
				{
					public static LocString LEGEND1 = "Displays whether buildings use or generate " + FormatAsLink("Power", "POWER");

					public static LocString LEGEND2 = "<b>Consumer</b>\nThese buildings draw power from a circuit";

					public static LocString LEGEND3 = "<b>Producer</b>\nThese buildings generate power for a circuit";

					public static LocString LEGEND4 = "Displays the power load on wire systems";

					public static LocString LEGEND5 = "<b>Energy Surplus</b>\nThese circuits produce more power than they can consume";

					public static LocString LEGEND6 = "<b>Strained</b>\nThese circuits consume nearly all power they produce, but can still function as intended";

					public static LocString LEGEND7 = "Too much power being drawn from system";

					public static LocString LEGEND8 = "<b>Underpowered</b>\nThese circuits consume more power than they can produce" + HORIZONTAL_BR_RULE + "Buildings may be frequently shut off when connected to underpowered circuits";

					public static LocString LEGEND_SWITCH = "<b>Switch</b>\nActivates or deactivates connected circuits";
				}

				public static LocString NAME = "POWER OVERLAY";

				public static LocString BUTTON = "Power Overlay";

				public static LocString LEGEND1 = "<b>BUILDING POWER</b>";

				public static LocString LEGEND2 = "Consumer";

				public static LocString LEGEND3 = "Producer";

				public static LocString LEGEND4 = "<b>CIRCUIT POWER LOAD</b>";

				public static LocString LEGEND5 = "Energy Surplus";

				public static LocString LEGEND6 = "Strained";

				public static LocString LEGEND7 = "Overloaded";

				public static LocString LEGEND8 = "Underpowered";

				public static LocString DIAGRAM_HEADER = "Energy from the <b>Left Outlet</b> is used by the <b>Right Outlet</b>";

				public static LocString LEGEND_SWITCH = "Switch";
			}

			public class TEMPERATURE
			{
				public class TOOLTIPS
				{
					public static LocString TEMPERATURE = "Temperatures reaching {0}";
				}

				public static LocString NAME = "TEMPERATURE OVERLAY";

				public static LocString BUTTON = "Temperature Overlay";

				public static LocString EXTREMECOLD = "Absolute Zero";

				public static LocString VERYCOLD = "Cold";

				public static LocString COLD = "Chilled";

				public static LocString TEMPERATE = "Temperate";

				public static LocString HOT = "Warm";

				public static LocString VERYHOT = "Hot";

				public static LocString EXTREMEHOT = "Scorching";

				public static LocString MAXHOT = "Molten";
			}

			public class HEATFLOW
			{
				public class TOOLTIPS
				{
					public static LocString COOLING = "<b>Body Heat Loss</b>\nUncomfortably cold" + HORIZONTAL_BR_RULE + "Duplicants lose more heat in these areas than they can absorb\n* Warm Sweaters help Duplicants retain body heat";

					public static LocString NEUTRAL = "<b>Comfort Zone</b>\nComfortable area" + HORIZONTAL_BR_RULE + "Duplicants can regulate their internal temperatures in these areas";

					public static LocString HEATING = "<b>Body Heat Retention</b>\nUncomfortably warm" + HORIZONTAL_BR_RULE + "Duplicants absorb more heat in these areas than they can release\n* Cool Vests help Duplicants shed excess body heat";
				}

				public static LocString NAME = "THERMAL TOLERANCE OVERLAY";

				public static LocString HOVERTITLE = "THERMAL TOLERANCE";

				public static LocString BUTTON = "Thermal Tolerance Overlay";

				public static LocString COOLING = "Body Heat Loss";

				public static LocString NEUTRAL = "Comfort Zone";

				public static LocString HEATING = "Body Heat Retention";
			}

			public class ROOMS
			{
				public static class NOROOM
				{
					public static LocString HEADER = "No Room";

					public static LocString DESC = "Enclose this space with walls and doors to make a room";

					public static LocString TOO_BIG = "<color=#F44A47FF>    • Size: {0} Tiles\n    • Maximum room size: {1} Tiles</color>";
				}

				public class TOOLTIPS
				{
					public static LocString ROOM = "Completed Duplicant bedrooms";

					public static LocString NOROOMS = "Duplicants have nowhere to sleep";
				}

				public static LocString NAME = "ROOM OVERLAY";

				public static LocString BUTTON = "Room Overlay";

				public static LocString ROOM = "Room {0}";

				public static LocString HOVERTITLE = "ROOMS";
			}

			public class JOULES
			{
				public static LocString NAME = "JOULES";

				public static LocString HOVERTITLE = "JOULES";

				public static LocString BUTTON = "Joules Overlay";
			}

			public class LIGHTING
			{
				public class RANGES
				{
					public static LocString NO_LIGHT = "Pitch Black";

					public static LocString VERY_LOW_LIGHT = "Dark";

					public static LocString LOW_LIGHT = "Dim";

					public static LocString MEDIUM_LIGHT = "Well Lit";

					public static LocString HIGH_LIGHT = "Bright";

					public static LocString VERY_HIGH_LIGHT = "Brilliant";

					public static LocString MAX_LIGHT = "Blinding";
				}

				public class TOOLTIPS
				{
					public static LocString NAME = "LIGHT OVERLAY";

					public static LocString LITAREA = "<b>Lit Area</b>\nDuplicants have adequate lighting in these areas";

					public static LocString DARK = "<b>Unlit Area</b>\nDuplicants cannot see in these areas";
				}

				public static LocString NAME = "LIGHT OVERLAY";

				public static LocString BUTTON = "Light Overlay";

				public static LocString LITAREA = "Lit Area";

				public static LocString DARK = "Unlit Area";

				public static LocString HOVERTITLE = "LIGHT";

				public static LocString DESC = "{0} Lux";
			}

			public class CROP
			{
				public class TOOLTIPS
				{
					public static LocString GROWTH_HALTED = "<b>Halted Growth</b>\nSubstandard conditions prevent these plants from growing";

					public static LocString GROWING = "<b>Growing</b>\nThese plants are thriving in their current conditions";

					public static LocString FULLY_GROWN = "<b>Fully Grown</b>\nThese plants have reached maturation" + HORIZONTAL_BR_RULE + "Select the " + FormatAsLink("HARVEST TOOL", "TOOLS") + " <color=#F44A47><b>[Y]</b></color> to batch harvest";
				}

				public static LocString NAME = "FARMING OVERLAY";

				public static LocString BUTTON = "Farming Overlay";

				public static LocString GROWTH_HALTED = "Halted Growth";

				public static LocString GROWING = "Growing";

				public static LocString FULLY_GROWN = "Fully Grown";
			}

			public class LIQUIDPLUMBING
			{
				public class TOOLTIPS
				{
					public static LocString CONNECTED = "Connected to a " + FormatAsLink("Liquid Pipe", BUILDINGS.PREFABS.LIQUIDCONDUIT.NAME);

					public static LocString DISCONNECTED = "Not connected to a " + FormatAsLink("Liquid Pipe", BUILDINGS.PREFABS.LIQUIDCONDUIT.NAME);

					public static LocString CONSUMER = "<b>Output Pipe</b>\nOutputs send liquid into pipes" + HORIZONTAL_BR_RULE + "Must be on the same network as at least one " + FormatAsLink("Intake", "LIQUIDPIPING");

					public static LocString FILTERED = "<b>Filtered Output Pipe</b>\nFiltered Outputs send filtered liquid into pipes" + HORIZONTAL_BR_RULE + "Must be on the same network as at least one " + FormatAsLink("LiquidDestination", "LIQUIDPIPING");

					public static LocString PRODUCER = "<b>Building Intake</b>\nIntakes send liquid into buildings" + HORIZONTAL_BR_RULE + "Must be on the same network as at least one " + FormatAsLink("Output", "LIQUIDPIPING");

					public static LocString NETWORK = "Liquid network {0}";
				}

				public static LocString NAME = "PLUMBING OVERLAY";

				public static LocString BUTTON = "Plumbing Overlay";

				public static LocString CONSUMER = "Output Pipe";

				public static LocString FILTERED = "Filtered Output Pipe";

				public static LocString PRODUCER = "Building Intake";

				public static LocString CONNECTED = "Connected";

				public static LocString DISCONNECTED = "Disconnected";

				public static LocString NETWORK = "Liquid Network {0}";

				public static LocString DIAGRAM_BEFORE_ARROW = "Liquid flows from <b>Output Pipe</b>";

				public static LocString DIAGRAM_AFTER_ARROW = "<b>Building Intake</b>";
			}

			public class GASPLUMBING
			{
				public class TOOLTIPS
				{
					public static LocString CONNECTED = "Connected to a " + FormatAsLink("Gas Pipe", "GASPIPING");

					public static LocString DISCONNECTED = "Not connected to a " + FormatAsLink("Gas Pipe", "GASPIPING");

					public static LocString CONSUMER = "<b>Output Pipe</b>\nOutputs send gas into pipes" + HORIZONTAL_BR_RULE + "Must be on the same network as at least one " + FormatAsLink("Intake", "GASPIPING");

					public static LocString FILTERED = "<b>Filtered Output Pipe</b>\nFiltered Outputs send filtered gas into pipes" + HORIZONTAL_BR_RULE + "Must be on the same network as at least one " + FormatAsLink("Intake", "GASPIPING");

					public static LocString PRODUCER = "<b>Building Intake</b>\nIntakes send gas into buildings" + HORIZONTAL_BR_RULE + "Must be on the same network as at least one " + FormatAsLink("Output", "GASPIPING");

					public static LocString NETWORK = "Gas network {0}";
				}

				public static LocString NAME = "VENTILATION OVERLAY";

				public static LocString BUTTON = "Ventilation Overlay";

				public static LocString CONSUMER = "Output Pipe";

				public static LocString FILTERED = "Filtered Output Pipe";

				public static LocString PRODUCER = "Building Intake";

				public static LocString CONNECTED = "Connected";

				public static LocString DISCONNECTED = "Disconnected";

				public static LocString NETWORK = "Gas Network {0}";

				public static LocString DIAGRAM_BEFORE_ARROW = "Gas flows from <b>Output Pipe</b>";

				public static LocString DIAGRAM_AFTER_ARROW = "<b>Building Intake</b>";
			}

			public class SUIT
			{
				public static LocString NAME = "EXOSUIT OVERLAY";

				public static LocString BUTTON = "Exosuit Overlay";

				public static LocString SUIT_ICON = "Exosuit";

				public static LocString SUIT_ICON_TOOLTIP = "<b>Exosuit</b>\nHighlights the current location of equippable exosuits";
			}

			public class LOGIC
			{
				public abstract class TOOLTIPS
				{
					public static LocString INPUT = "<b>Input Port</b>\nReceives a signal from an automation grid";

					public static LocString OUTPUT = "<b>Output Port</b>\nSends a signal out to an automation grid";

					public static LocString RESET_UPDATE = "<b>Refresh Port</b>\nResets or updates a building when activated";

					public static LocString ONE = "<b>Active</b>\nThis port is currently active";

					public static LocString ZERO = "<b>Standby</b>This port is currently on standby";

					public static LocString DISCONNECTED = "<b>Disconnected</b>\nThis port is not connected to an automation grid";
				}

				public static LocString NAME = "AUTOMATION OVERLAY";

				public static LocString BUTTON = "Automation Overlay";

				public static LocString INPUT = "Input Port";

				public static LocString OUTPUT = "Output Port";

				public static LocString RESET_UPDATE = "Refresh Port";

				public static LocString CIRCUIT_STATUS_HEADER = "GRID STATUS";

				public static LocString ONE = "Active";

				public static LocString ZERO = "Standby";

				public static LocString DISCONNECTED = "DISCONNECTED";
			}

			public class CONVEYOR
			{
				public abstract class TOOLTIPS
				{
					public static LocString OUTPUT = "<b>Loader</b>\nLoads material onto a Conveyor Rail for transport to Receptacles";

					public static LocString INPUT = "<b>Receptacle</b>\nReceives material from a Conveyor Rail and stores it for Duplicant use";
				}

				public static LocString NAME = "CONVEYOR OVERLAY";

				public static LocString BUTTON = "Conveyor Overlay";

				public static LocString OUTPUT = "Loader";

				public static LocString INPUT = "Receptacle";
			}

			public class DECOR
			{
				public class TOOLTIPS
				{
					public static LocString LOWDECOR = "<b>Negative Decor</b>\nArea with insufficient decor values\n* Resources on the floor are considered \"debris\" and will decrease decor";

					public static LocString HIGHDECOR = "<b>Positive Decor</b>\nArea with sufficient decor values\n* Lighting and aesthetically pleasing buildings increase decor";
				}

				public static LocString NAME = "DECOR OVERLAY";

				public static LocString BUTTON = "Decor Overlay";

				public static LocString TOTAL = "Total Decor: ";

				public static LocString ENTRY = "{0} {1} {2}";

				public static LocString COUNT = "({0})";

				public static LocString VALUE = "{0}{1}";

				public static LocString VALUE_ZERO = "{0}{1}";

				public static LocString HEADER_POSITIVE = "Positive Value:";

				public static LocString HEADER_NEGATIVE = "Negative Value:";

				public static LocString LOWDECOR = "Negative Decor";

				public static LocString HIGHDECOR = "Positive Decor";

				public static LocString CLUTTER = "Debris";

				public static LocString LIGHTING = "Lighting";

				public static LocString CLOTHING = "{0}'s Outfit";

				public static LocString HOVERTITLE = "DECOR";
			}

			public class NOISE_POLLUTION
			{
				public class NAMES
				{
					public static LocString PEACEFUL = "Peaceful";

					public static LocString QUIET = "Quiet";

					public static LocString TOSSANDTURN = "Moderate";

					public static LocString WAKEUP = "Noisy";

					public static LocString PASSIVE = "Loud";

					public static LocString ACTIVE = "Cacophonous";

					public static LocString EXTREME = "Painful";
				}

				public class TOOLTIPS
				{
					public static LocString PEACEFUL = "[{0} dB]" + HORIZONTAL_BR_RULE + "Peaceful areas improve Duplicants' quality of sleep and Learning ability.\n\nSoundproofed rooms and areas free of buildings and critters will generally be quieter.";

					public static LocString QUIET = "[>{0} dB]" + HORIZONTAL_BR_RULE + "Quiet areas are necessary for Duplicants to fall asleep.\n\nSoundproofed rooms and areas free of buildings and critters will generally be quieter.";

					public static LocString TOSSANDTURN = "[>{0} dB]" + HORIZONTAL_BR_RULE + "Duplicants will feel unrested if forced to sleep in Moderate noise.\n\nSoundproofed rooms and areas free of buildings and critters will generally be quieter.";

					public static LocString WAKEUP = "[>{0} dB]" + HORIZONTAL_BR_RULE + "Noisy areas will wake Duplicants up during the night, but are tolerable to them during the day.\n\nDuplicant foot traffic, generators, and powered buildings can create high noise levels.";

					public static LocString PASSIVE = "[>{0} dB]" + HORIZONTAL_BR_RULE + "Loud areas will impair Duplicants' Learning abilities and cause minor Stress.\n\nDuplicant foot traffic, generators, and powered buildings can create high noise levels.";

					public static LocString ACTIVE = "[>{0} dB]" + HORIZONTAL_BR_RULE + "Cacophonous noise causes significant Stress and prevents Duplicants from sleeping or researching.\n\nDuplicant foot traffic, generators, and powered buildings can create high noise levels.";

					public static LocString EXTREME = "[>{0} dB]" + HORIZONTAL_BR_RULE + "Painful noise levels are extremely Stressful to Duplicants and will cause them physical pain.\n\nDuplicant foot traffic, generators, and powered buildings can create high noise levels.";

					public static LocString HIGH_NOISE_POLLUTION = "Exposure to loud noises causes Duplicant Stress over time and makes it difficult to sleep or concentrate" + HORIZONTAL_BR_RULE + "High Duplicant traffic and powered buildings can create noisy areas";

					public static LocString LOW_NOISE_POLLUTION = "Quiet areas decrease Stress, ensure Duplicants have quality sleep, and help them to concentrate" + HORIZONTAL_BR_RULE + "Soundproofed rooms and areas free of buildings and critters will generally be quieter.";
				}

				public static LocString NAME = "ACOUSTICS OVERLAY";

				public static LocString BUTTON = "Acoustics Overlay";

				public static LocString TOTAL = "Total";

				public static LocString HOVERTITLE = "Acoustics";

				public static LocString NOTAFFECTING = "Drowned out:";

				public static LocString VALUE = "<color=#{0}>{1} dB</color>";

				public static LocString RANGE = "{0} dB";

				public static LocString LOUDNESS_STRING = " <color=#{0}>({1})</color>";

				public static LocString DESCRIPTION = "Two " + FormatAsLink("Sounds", "SOUND") + " of equal dB will sum for a <b>+3 dB</b> increase.\nA " + FormatAsLink("Sound", "SOUND") + " that is 10 dB quieter will add approximately <b>+0.5 dB</b> to the total signal.\nAnything more than 10 dB quieter will be inaudible.";
			}

			public class PRIORITIES
			{
				public static LocString NAME = "SUB-PRIORITY OVERLAY";

				public static LocString BUTTON = "Sub-Priority Overlay";

				public static LocString ONE = "1 (Low Urgency)";

				public static LocString ONE_TOOLTIP = "Sub-Priority 1";

				public static LocString TWO = "2";

				public static LocString TWO_TOOLTIP = "Sub-Priority 2";

				public static LocString THREE = "3";

				public static LocString THREE_TOOLTIP = "Sub-Priority 3";

				public static LocString FOUR = "4";

				public static LocString FOUR_TOOLTIP = "Sub-Priority 4";

				public static LocString FIVE = "5";

				public static LocString FIVE_TOOLTIP = "Sub-Priority 5";

				public static LocString SIX = "6";

				public static LocString SIX_TOOLTIP = "Sub-Priority 6";

				public static LocString SEVEN = "7";

				public static LocString SEVEN_TOOLTIP = "Sub-Priority 7";

				public static LocString EIGHT = "8";

				public static LocString EIGHT_TOOLTIP = "Sub-Priority 8";

				public static LocString NINE = "9 (High Urgency)";

				public static LocString NINE_TOOLTIP = "Sub-Priority 9";
			}

			public class DISEASE
			{
				public class DISINFECT_THRESHOLD_DIAGRAM
				{
					public static LocString UNITS = "Germs";

					public static LocString MIN_LABEL = "0";

					public static LocString MAX_LABEL = "1m";

					public static LocString THRESHOLD_PREFIX = "Disinfect At:";

					public static LocString TOOLTIP = "Automatically disinfect any building with more than {NumberOfGerms} germs.";

					public static LocString TOOLTIP_DISABLED = "Automatic building disinfection disabled.";
				}

				public static LocString NAME = "GERM OVERLAY";

				public static LocString BUTTON = "Germ Overlay";

				public static LocString HOVERTITLE = "Germ";

				public static LocString INTERNAL_GERMS = "Germ Host";

				public static LocString INTERNAL_GERMS_TOOLTIP = "<b>Germ Host</b>\nGerm hosts may be asymptomatic and show no signs of disease" + HORIZONTAL_BR_RULE + "Duplicants become hosts when germs enter their body through inhalation or ingestion\n* Unlike surface germs, internal germs cannot be washed off";

				public static LocString INFECTION_SOURCE = "Germ Source";

				public static LocString INFECTION_SOURCE_TOOLTIP = "<b>Germ Source</b>\nAreas where germs are produced\n* Placing Wash Basins or Hand Sanitizers near these areas may prevent disease spread";

				public static LocString NO_DISEASE = "Zero surface germs";

				public static LocString DISEASE_NAME_FORMAT = "{0}<color=#{1}></color>";

				public static LocString DISEASE_NAME_FORMAT_NO_COLOR = "{0}";

				public static LocString DISEASE_FORMAT = "{1} [{0}]<color=#{2}></color>";

				public static LocString DISEASE_FORMAT_NO_COLOR = "{1} [{0}]";

				public static LocString CONTAINER_FORMAT = "\n    {0}: {1}";

				public static LocString IMMUNITY = DUPLICANTS.STATS.IMMUNELEVEL.NAME + ": {0}";
			}

			public class CROPS
			{
				public static LocString NAME = "FARMING OVERLAY";

				public static LocString BUTTON = "Farming Overlay";
			}

			public class POWER
			{
				public static LocString WATTS_GENERATED = "Watts Generated";

				public static LocString WATTS_CONSUMED = "Watts Consumed";
			}
		}

		public class TABLESCREENS
		{
			public static LocString DUPLICANT_PROPERNAME = "<b>{0}</b>";

			public static LocString SELECT_DUPLICANT_BUTTON = "Click to select <b>{0}</b>";

			public static LocString GOTO_DUPLICANT_BUTTON = "Double-click to go to <b>{0}</b>";

			public static LocString COLUMN_SORT_BY_NAME = "Sort by name";

			public static LocString COLUMN_SORT_BY_STRESS = "Sort by stress level";

			public static LocString COLUMN_SORT_BY_HITPOINTS = "Sort by hit points";

			public static LocString COLUMN_SORT_BY_IMMUNEPOINTS = "Sort by immune points";

			public static LocString COLUMN_SORT_BY_FULLNESS = "Sort by fullness";

			public static LocString COLUMN_SORT_BY_EATEN_TODAY = "Sort by eaten today";

			public static LocString COLUMN_SORT_BY_EXPECTATIONS = "Sort by quality expectations";
		}

		public class CONSUMABLESSCREEN
		{
			public static LocString TITLE = "CONSUMABLES";

			public static LocString TOOLTIP_TOGGLE_ALL = "Toggle all food permissions colonywide";

			public static LocString TOOLTIP_TOGGLE_COLUMN = "Toggle colonywide <b>{0}</b> permission";

			public static LocString TOOLTIP_TOGGLE_ROW = "Toggle all food permissions for <b>{0}</b>";

			public static LocString NEW_MINIONS_TOOLTIP_TOGGLE_ROW = "Toggle all food permissions for <b>New Duplicants</b>";

			public static LocString NEW_MINIONS_FOOD_PERMISSION_ON = "<b>New Duplicants</b> are <color=#B87194FF>allowed</color> to eat \n<b>{0}</b> by default";

			public static LocString NEW_MINIONS_FOOD_PERMISSION_OFF = "<b>New Duplicants</b> are <color=#B87194FF>not allowed</color> to eat \n<b>{0}</b> by default";

			public static LocString FOOD_PERMISSION_ON = "<b>{0}</b> is <color=#B87194FF>allowed</color> to eat <b>{1}</b>";

			public static LocString FOOD_PERMISSION_OFF = "<b>{0}</b> is <color=#B87194FF>not allowed</color> to eat <b>{1}</b>";

			public static LocString FOOD_CANT_CONSUME = "<b>{0}</b> <color=#B87194FF>physically cannot</color> eat\n<b>{1}</b>";

			public static LocString FOOD_REFUSE = "<b>{0}</b> <color=#B87194FF>refuses</color> to eat\n<b>{1}</b>";

			public static LocString FOOD_AVAILABLE = "Available: {0}";

			public static LocString FOOD_QUALITY = "Morale: {0}";

			public static LocString FOOD_QUALITY_VS_EXPECTATION = HORIZONTAL_RULE + "\nThis food will give {0} Morale if {1} eats it:";
		}

		public class JOBSSCREEN
		{
			public class PRIORITY
			{
				public static LocString VERYHIGH = "Very High";

				public static LocString HIGH = "High";

				public static LocString STANDARD = "Standard";

				public static LocString LOW = "Low";

				public static LocString VERYLOW = "Very Low";

				public static LocString DISABLED = "Disabled";
			}

			public static LocString TITLE = "MANAGE ERRAND PRIORITIES";

			public static LocString TOOLTIP_TOGGLE_ALL = "Set all errand priorities colonywide";

			public static LocString HEADER_TOOLTIP = "<size=16>{Job} Priorities</size>" + HORIZONTAL_BR_RULE + "{Details}\n\nDuplicant priorities supercede colony priorities";

			public static LocString HEADER_DETAILS_TOOLTIP = "{Description}\n\nAffected errands: {ChoreList}";

			public static LocString HEADER_CHANGE_TOOLTIP = "Set the priority for <b>{Job} Errands</b> colonywide\n";

			public static LocString NEW_MINION_ITEM_TOOLTIP = "{Job} Errands are automatically a {Priority} Priority for <b>Arriving Duplicants</b>";

			public static LocString ITEM_TOOLTIP = "{Job} Errands are a {Priority} Priority for <b>{Name}</b>";

			public static LocString MINION_SKILL_TOOLTIP = "{Name}'s {Attribute} skill: ";

			public static LocString ITEM_AUTO_ASSIGNED_TOOLTIP = "As a {Role}, {Name} considers {Job} Errands to be a {Priority} Priority";

			public static LocString TRAIT_DISABLED = "{Name} cannot do {Job} errands because they possess the {Trait} trait";

			public static LocString INCREASE_ROW_PRIORITY_NEW_MINION_TOOLTIP = "Prioritize <b>All Errands</b> for <b>New Duplicants</b>";

			public static LocString DECREASE_ROW_PRIORITY_NEW_MINION_TOOLTIP = "Deprioritize <b>All Errands</b> for <b>New Duplicants</b>";

			public static LocString INCREASE_ROW_PRIORITY_MINION_TOOLTIP = "Prioritize <b>All Errands</b> for <b>{Name}</b>";

			public static LocString DECREASE_ROW_PRIORITY_MINION_TOOLTIP = "Depriotize <b>All Errands</b> for <b>{Name}</b>";

			public static LocString INCREASE_PRIORITY_TUTORIAL = "{Key} Increase priority";

			public static LocString DECREASE_PRIORITY_TUTORIAL = "{Key} Decrease priority";

			public static LocString SORT_TOOLTIP = "Sort by {Job} Errand priorities";

			public static LocString DISABLED_TOOLTIP = "{Name} may not perform {Job} Errands";

			public static LocString OPTIONS = "Options";

			public static LocString TOGGLE_ADVANCED_MODE = "Enable Proximity";

			public static LocString TOGGLE_ADVANCED_MODE_TOOLTIP = "<b>Proximity Errand Settings</b>" + HORIZONTAL_BR_RULE + "Enabling Proximity settings tells my Duplicants to always choose the closest high priority errand to perform.\n\nWhen disabled, Duplicants will choose between two high priority errands based on a hidden priority hierarchy instead.\n\nEnabling Proximity helps cut down on travel time in areas with lots of high-priority errands, and is useful for large colonies.";

			public static LocString RESET_SETTINGS = "Reset Priorities";

			public static LocString RESET_SETTINGS_TOOLTIP = "<b>Reset Priorities</b>" + HORIZONTAL_BR_RULE + "Returns all priorities to their default values.\n\nProximity Enabled: Priorities will be adjusted high-to-low.\n\nProximity Disabled: All priorities will be reset to neutral.";
		}

		public class VITALSSCREEN
		{
			public class CONDITIONS_GROWING
			{
				public class WILD
				{
					public static LocString BASE = "<b>Wild Growth\n[Lifecycle: {0}]</b>";

					public static LocString TOOLTIP = "This plant will take {0} to grow in the wild";
				}

				public class DOMESTIC
				{
					public static LocString BASE = "<b>Domestic Growth\n[Lifecycle: {0}]</b>";

					public static LocString TOOLTIP = "This plant will take {0} to grow domestically";
				}

				public class ADDITIONAL_DOMESTIC
				{
					public static LocString BASE = "<b>Additional Domestic Growth\n[Lifecycle: {0}]</b>";

					public static LocString TOOLTIP = "This plant will take {0} to grow domestically";
				}
			}

			public static LocString HEALTH = "Health";

			public static LocString IMMUNITY = "Immune System";

			public static LocString IMMUNITY_DISEASE = "{0} ({1})";

			public static LocString IMMUNITY_MULTIPLE_DISEASES = "Multiple diseases ({0})";

			public static LocString STRESS = "Stress";

			public static LocString EXPECTATIONS = "Expectations";

			public static LocString CALORIES = "Fullness";

			public static LocString EATEN_TODAY = "Eaten Today";

			public static LocString EATEN_TODAY_TOOLTIP = "Eaten Today" + HORIZONTAL_BR_RULE + "Consumed {0} of food this cycle";

			public static LocString ATMOSPHERE_CONDITION = "Atmosphere:";

			public static LocString SUBMERSION = "Liquid Level";

			public static LocString NOT_DROWNING = "Liquid Level";

			public static LocString FOOD_EXPECTATIONS = "Food Expectation";

			public static LocString FOOD_EXPECTATIONS_TOOLTIP = "This Duplicant desires food that is {0} quality or better";

			public static LocString DECOR_EXPECTATIONS = "Decor Expectation";

			public static LocString DECOR_EXPECTATIONS_TOOLTIP = "This Duplicant desires decor that is {0} or higher";

			public static LocString QUALITYOFLIFE_EXPECTATIONS = "Morale";

			public static LocString QUALITYOFLIFE_EXPECTATIONS_TOOLTIP = "This Duplicant needs a Morale rating of {0}.\n\nCurrent Morale:";
		}

		public class SCHEDULESCREEN
		{
			public static LocString SCHEDULE_EDITOR = "Schedule Editor";

			public static LocString SCHEDULE_NAME_DEFAULT = "Default Schedule";

			public static LocString SCHEDULE_NAME_FORMAT = "Schedule {0}";

			public static LocString SCHEDULE_DROPDOWN_ASSIGNED = "{0} (Assigned)";

			public static LocString SCHEDULE_DROPDOWN_BLANK = "<i>Move Duplicant...</i>";

			public static LocString SCHEDULE_DOWNTIME_MORALE = "Duplicants will receive {0} Morale from the scheduled Downtime shifts";

			public static LocString RENAME_BUTTON_TOOLTIP = "Rename custom schedule";

			public static LocString ALARM_BUTTON_ON_TOOLTIP = "Toggle Notifications" + HORIZONTAL_BR_RULE + "Sounds and notifications will play when shifts change for this schedule.\n\nENABLED\nClick to disable";

			public static LocString ALARM_BUTTON_OFF_TOOLTIP = "Toggle Notifications" + HORIZONTAL_BR_RULE + "No sounds or notifications will play for this schedule.\n\nDISABLED\nClick to enable";

			public static LocString DELETE_BUTTON_TOOLTIP = "Delete Schedule";

			public static LocString PAINT_TOOLS = "Paint Tools:";

			public static LocString ADD_SCHEDULE = "Add New Schedule";

			public static LocString POO = "dar";

			public static LocString DOWNTIME_MORALE = "Downtime Morale: {0}";

			public static LocString ALARM_TITLE_ENABLED = "Alarm On";

			public static LocString ALARM_TITLE_DISABLED = "Alarm Off";

			public static LocString SETTINGS = "Settings";

			public static LocString ALARM_BUTTON = "Shift Alarms";

			public static LocString RESET_SETTINGS = "Reset Shifts";

			public static LocString RESET_SETTINGS_TOOLTIP = "<b>Reset Shifts</b>" + HORIZONTAL_BR_RULE + "Restore this schedule to default shifts";
		}

		public class COLONYLOSTSCREEN
		{
			public static LocString COLONYLOST = "COLONY LOST";

			public static LocString COLONYLOSTDESCRIPTION = "All Duplicants are dead or incapacitated.";

			public static LocString RESTARTPROMPT = "Press <color=#F44A47>[ESC]</color> to return to a previous colony, or begin a new one.";

			public static LocString DISMISSBUTTON = "DISMISS";

			public static LocString QUITBUTTON = "MAIN MENU";
		}

		public class GENESHUFFLERMESSAGE
		{
			public static LocString HEADER = "NEURAL VACILLATION COMPLETE";

			public static LocString BODY_SUCCESS = "Whew! <b>{0}'s</b> brain is still vibrating, but they've never felt better!\n\n<b>{0}</b> acquired the <b>{1}</b> trait.\n\n<b>{1}:</b>\n{2}";

			public static LocString BODY_FAILURE = "The machine attempted to alter this Duplicant, but there's no improving on perfection.\n\n<b>{0}</b> already has all positive traits!";

			public static LocString DISMISSBUTTON = "DISMISS";
		}

		public class CRASHSCREEN
		{
			public static LocString TITLE = "Whoops! We're sorry, but it seems your game has encountered an error.\nIt's okay though - these errors are how we find and fix problems to make our game more fun for everyone.\nIf you use the box below to submit a crash report to us, we can use this information to get the issue sorted out.";

			public static LocString HEADER = "OPTIONAL CRASH DESCRIPTION";

			public static LocString BODY = "Help! A black hole ate my game!";

			public static LocString THANKYOU = "Thank you!\n\nYou're making our game better, one crash at a time.";

			public static LocString UPLOADINFO = "UPLOAD ADDITIONAL INFO ({0})";

			public static LocString REPORTBUTTON = "REPORT CRASH";

			public static LocString REPORTING = "REPORTING, PLEASE WAIT...";

			public static LocString CONTINUEBUTTON = "CONTINUE GAME";

			public static LocString QUITBUTTON = "QUIT TO DESKTOP";

			public static LocString SAVEFAILED = "Save Failed: {0}";

			public static LocString LOADFAILED = "Load Failed: {0}\nSave Version: {1}\nExpected: {2}";
		}

		public class DEMOOVERSCREEN
		{
			public static LocString TIMEREMAINING = "Demo time remaining:";

			public static LocString TIMERTOOLTIP = "Demo time remaining";

			public static LocString TIMERINACTIVE = "Timer inactive";

			public static LocString DEMOOVER = "END OF DEMO";

			public static LocString DESCRIPTION = "Thank you for playing <color=#F44A47>Oxygen Not Included</color>!";

			public static LocString DESCRIPTION_2 = string.Empty;

			public static LocString QUITBUTTON = "RESET";
		}

		public class CREDITSSCREEN
		{
			public class THIRD_PARTY
			{
				public static LocString FMOD = "FMOD Sound System\nCopyright Firelight Technologies";
			}

			public static LocString TITLE = "CREDITS";

			public static LocString CLOSEBUTTON = "CLOSE";
		}

		public class PRIORITYSCREEN
		{
			public static LocString BASIC = "Set the order in which with which pending errands should be done\n\nSub-Priorities will always be overridden by Duplicant Priorities <color=#F44A47>[J]</color>" + HORIZONTAL_BR_RULE + "1: Least Urgent\n9: Most Urgent";

			public static LocString HIGH = string.Empty;

			public static LocString EMERGENCY = string.Empty;

			public static LocString HIGH_TOGGLE = string.Empty;

			public static LocString OPEN_JOBS_SCREEN = "Click to open the Duplicant Priorities screen\n\nDuplicants will pick errands according to their Priorities first, and their Sub-Priorities second";

			public static LocString DIAGRAM = "Duplicants will pick errands according to their Priorities first, and their Sub-Priorities second";

			public static LocString DIAGRAM_TITLE = "PRIORITIES";
		}

		public class RESOURCESCREEN
		{
			public static LocString CATEGORY_TOOLTIP = "Counts all unallocated resources within reach" + HORIZONTAL_BR_RULE + "Click to expand";

			public static LocString AVAILABLE_TOOLTIP = "Available: {0}\n({1} of {2} allocated to pending errands)";
		}

		public class CONFIRMDIALOG
		{
			public static LocString OK = "OK";

			public static LocString CANCEL = "CANCEL";

			public static LocString DIALOG_HEADER = "MESSAGE";
		}

		public class FILE_NAME_DIALOG
		{
			public static LocString ENTER_TEXT = "Enter Text...";
		}

		public class MINION_IDENTITY_SORT
		{
			public static LocString TITLE = "Sort By";

			public static LocString NAME = "Duplicant";

			public static LocString ROLE = "Role";

			public static LocString PERMISSION = "Permission";
		}

		public class UISIDESCREENS
		{
			public class TREEFILTERABLESIDESCREEN
			{
				public static LocString TITLE = "Element Filter";

				public static LocString TITLE_CRITTER = "Critter Filter";

				public static LocString ALLBUTTON = "All";

				public static LocString ALLBUTTONTOOLTIP = "Allow storage of all resource categories in this container";

				public static LocString CATEGORYBUTTONTOOLTIP = "Allow storage of anything in the {0} resource category";

				public static LocString MATERIALBUTTONTOOLTIP = "Add or remove this material from storage";

				public static LocString ONLYALLOWTRANSPORTITEMSBUTTON = "Sweep Only";

				public static LocString ONLYALLOWTRANSPORTITEMSBUTTONTOOLTIP = "Only store objects marked Sweep <color=#F44A47>[K]</color> in this container";
			}

			public class TELESCOPESIDESCREEN
			{
				public static LocString TITLE = "Telescope Configuration";

				public static LocString NO_SELECTED_ANALYSIS_TARGET = "No analysis focus selected.\nOpen the starmap to selected a focus.";

				public static LocString ANALYSIS_TARGET_SELECTED = "Object focus selected.\nAnalysis underway.";

				public static LocString OPENSTARMAPBUTTON = "OPEN STARMAP";

				public static LocString ANALYSIS_TARGET_HEADER = "Object Analysis";
			}

			public class FILTERSIDESCREEN
			{
				public static class UNFILTEREDELEMENTS
				{
					public static LocString GAS = "Gas Output:\nAll";

					public static LocString LIQUID = "Liquid Output:\nAll";
				}

				public static class FILTEREDELEMENT
				{
					public static LocString GAS = "Filtered Gas Output:\n{0}";

					public static LocString LIQUID = "Filtered Liquid Output:\n{0}";
				}

				public static LocString TITLE = "Filter Outputs";

				public static LocString OUTPUTELEMENTHEADER = "Output 1";

				public static LocString SELECTELEMENTHEADER = "Output 2";

				public static LocString NOELEMENTSELECTED = "No element selected";
			}

			public class FABRICATORSIDESCREEN
			{
				public class TOOLTIPS
				{
					public static LocString RECIPERQUIREMENT_SUFFICIENT = "This recipe consumes {1} of an available {2} of {0}";

					public static LocString RECIPERQUIREMENT_INSUFFICIENT = "This recipe requires {1} {0}\nAvailable: {2}";

					public static LocString RECIPEPRODUCT = "This recipe produces {1} {0}";
				}

				public class EFFECTS
				{
					public static LocString OXYGEN_TANK = EQUIPMENT.PREFABS.OXYGEN_TANK.NAME + " ({0})";

					public static LocString OXYGEN_TANK_UNDERWATER = EQUIPMENT.PREFABS.OXYGEN_TANK_UNDERWATER.NAME + " ({0})";

					public static LocString JETSUIT_TANK = EQUIPMENT.PREFABS.JET_SUIT.TANK_EFFECT_NAME + " ({0})";

					public static LocString COOL_VEST = EQUIPMENT.PREFABS.COOL_VEST.NAME + " ({0})";

					public static LocString WARM_VEST = EQUIPMENT.PREFABS.WARM_VEST.NAME + " ({0})";

					public static LocString FUNKY_VEST = EQUIPMENT.PREFABS.FUNKY_VEST.NAME + " ({0})";

					public static LocString RESEARCHPOINT = "{0}: +1";
				}

				public static LocString TITLE = "{0} Recipes";

				public static LocString SUBTITLE = "Recipes";

				public static LocString NORECIPEDISCOVERED = "No discovered recipes";

				public static LocString NORECIPEDISCOVERED_BODY = "Discover new ingredients in the world to unlock some recipes.";

				public static LocString NORECIPESELECTED = "No recipe selected";

				public static LocString SELECTRECIPE = "Select a recipe to fabricate.";

				public static LocString COST = "<b>Ingredients:</b>\n";

				public static LocString RESULTREQUIREMENTS = "<b>Requirements:</b>";

				public static LocString RESULTEFFECTS = "<b>Effects:</b>";

				public static LocString KG = "- {0}: {1}\n";

				public static LocString INFORMATION = "INFORMATION";

				public static LocString CANCEL = "Cancel";

				public static LocString RECIPERQUIREMENT = "{0}: {1} / {2}";

				public static LocString RECIPEPRODUCT = "{0}: {1}";

				public static LocString UNITS_AND_CALS = "{0} [{1}]";

				public static LocString CALS = "{0}";

				public static LocString QUEUED_MISSING_INGREDIENTS_TOOLTIP = "Missing {0} of {1}\n";
			}

			public class GENESHUFFLERSIDESREEN
			{
				public static LocString TITLE = "Neural Vacillator";

				public static LocString COMPLETE = "Something feels different.";

				public static LocString UNDERWAY = "Neural Vacillation in progress.";

				public static LocString CONSUMED = "There are no charges left in this Vacillator.";

				public static LocString CONSUMED_WAITING = "Recharge requested, awaiting delivery by Duplicant.";

				public static LocString BUTTON = "Complete Neural Process";

				public static LocString BUTTON_RECHARGE = "Recharge";

				public static LocString BUTTON_RECHARGE_CANCEL = "Cancel Recharge";
			}

			public class PLANTERSIDESCREEN
			{
				public class TOOLTIPS
				{
					public static LocString PLANTLIFECYCLE = "Duration and number of harvests produced by this plant in a lifetime";

					public static LocString PLANTREQUIREMENTS = "Minimum conditions for basic plant growth";

					public static LocString PLANTEFFECTS = "Additional attributes of this plant";

					public static LocString YIELD = FormatAsLink("{2}", "KCAL") + " produced [" + FormatAsLink("{1}", "KCAL") + " / unit]";

					public static LocString NUMBEROFHARVESTS = "This plant can mature {0} times before the end of its lifecycle";

					public static LocString YIELD_SEED = "Sow to grow more of this plant";

					public static LocString YIELD_SEED_FINAL_HARVEST = "{0}\n\nProduced in the final harvest of the plant's lifecycle";

					public static LocString BONUS_SEEDS = "This plant has a {0} chance to produce new seeds when harvested";
				}

				public static LocString TITLE = "{0} Seeds";

				public static LocString INFORMATION = "INFORMATION";

				public static LocString AWAITINGREQUEST = "PLANT: {0}";

				public static LocString AWAITINGDELIVERY = "AWAITING DELIVERY: {0}";

				public static LocString AWAITINGREMOVAL = "AWAITING DIGGING UP: {0}";

				public static LocString ENTITYDEPOSITED = "PLANTED: {0}";

				public static LocString DEPOSIT = "Plant";

				public static LocString CANCELDEPOSIT = "Cancel";

				public static LocString REMOVE = "Uproot";

				public static LocString CANCELREMOVAL = "Cancel";

				public static LocString SELECT_TITLE = "SELECT";

				public static LocString SELECT_DESC = "Select a seed to plant.";

				public static LocString LIFECYCLE = "<b>Lifecycle</b>:";

				public static LocString PLANTREQUIREMENTS = "<b>Growth Requirements</b>:";

				public static LocString PLANTEFFECTS = "<b>Effects</b>:";

				public static LocString NUMBEROFHARVESTS = "Harvests: {0}";

				public static LocString YIELD = "{0}: {1} ";

				public static LocString YIELD_SINGLE = "{0}";

				public static LocString YIELDPERHARVEST = "{0} {1} per harvest";

				public static LocString TOTALHARVESTCALORIESWITHPERUNIT = "{0} [{1} / unit]";

				public static LocString TOTALHARVESTCALORIES = "{0}";

				public static LocString BONUS_SEEDS = "Base " + FormatAsLink("Seed", "PLANTS") + " Harvest Chance: {0}";

				public static LocString YIELD_SEED = "{1} {0}";

				public static LocString YIELD_SEED_SINGLE = "{0}";

				public static LocString YIELD_SEED_FINAL_HARVEST = "{1} {0} - Final harvest only";

				public static LocString YIELD_SEED_SINGLE_FINAL_HARVEST = "{0} - Final harvest only";

				public static LocString ROTATION_NEED_FLOOR = "<b>Requires upward plot orientation.</b>";

				public static LocString ROTATION_NEED_WALL = "<b>Requires sideways plot orientation.</b>";

				public static LocString ROTATION_NEED_CEILING = "<b>Requires downward plot orientation.</b>";
			}

			public class EGGINCUBATOR
			{
				public static LocString TITLE = "Critter Eggs";

				public static LocString AWAITINGREQUEST = "INCUBATE: {0}";

				public static LocString AWAITINGDELIVERY = "AWAITING DELIVERY: {0}";

				public static LocString AWAITINGREMOVAL = "AWAITING REMOVAL: {0}";

				public static LocString ENTITYDEPOSITED = "INCUBATING: {0}";

				public static LocString DEPOSIT = "Incubate";

				public static LocString CANCELDEPOSIT = "Cancel";

				public static LocString REMOVE = "Remove";

				public static LocString CANCELREMOVAL = "Cancel";

				public static LocString SELECT_TITLE = "SELECT";

				public static LocString SELECT_DESC = "Select an egg to incubate.";
			}

			public class BASICRECEPTACLE
			{
				public static LocString TITLE = "Displayed Object";

				public static LocString AWAITINGREQUEST = "SELECT: {0}";

				public static LocString AWAITINGDELIVERY = "AWAITING DELIVERY: {0}";

				public static LocString AWAITINGREMOVAL = "AWAITING REMOVAL: {0}";

				public static LocString ENTITYDEPOSITED = "DISPLAYING: {0}";

				public static LocString DEPOSIT = "Select";

				public static LocString CANCELDEPOSIT = "Cancel";

				public static LocString REMOVE = "Remove";

				public static LocString CANCELREMOVAL = "Cancel";

				public static LocString SELECT_TITLE = "SELECT OBJECT";

				public static LocString SELECT_DESC = "Select an object to display here.";
			}

			public class LURE
			{
				public static LocString TITLE = "Select Bait";

				public static LocString INFORMATION = "INFORMATION";

				public static LocString AWAITINGREQUEST = "PLANT: {0}";

				public static LocString AWAITINGDELIVERY = "AWAITING DELIVERY: {0}";

				public static LocString AWAITINGREMOVAL = "AWAITING DIGGING UP: {0}";

				public static LocString ENTITYDEPOSITED = "PLANTED: {0}";

				public static LocString ATTRACTS = "Attract {1}s";
			}

			public class ROLESTATION
			{
				public static LocString TITLE = "Job Assignments";

				public static LocString OPENROLESBUTTON = "JOBS";
			}

			public class RESEARCHSIDESCREEN
			{
				public static LocString TITLE = "Select Research";

				public static LocString CURRENTLYRESEARCHING = "Currently Researching";

				public static LocString NOSELECTEDRESEARCH = "No Research selected";

				public static LocString OPENRESEARCHBUTTON = "RESEARCH";
			}

			public class REFINERYSIDESCREEN
			{
				public static LocString RECIPE_FROM_TO = "{0} to {1}";
			}

			public class SEALEDDOORSIDESCREEN
			{
				public static LocString TITLE = "Sealed Door";

				public static LocString LABEL = "This door requires a sample to unlock.";

				public static LocString BUTTON = "SUBMIT BIOSCAN";

				public static LocString AWAITINGBUTTON = "AWAITING BIOSCAN";
			}

			public class ENCRYPTEDLORESIDESCREEN
			{
				public static LocString TITLE = "Encrypted File";

				public static LocString LABEL = "This computer contains encrypted files.";

				public static LocString BUTTON = "ATTEMPT DECRYPTION";

				public static LocString AWAITINGBUTTON = "AWAITING DECRYPTION";
			}

			public class ACCESS_CONTROL_SIDE_SCREEN
			{
				public static LocString TITLE = "Door Access Control";

				public static LocString DOOR_DEFAULT = "Default";

				public static LocString MINION_ACCESS = "Duplicant Access Permissions";

				public static LocString GO_LEFT_ENABLED = "Passing Left through this door is permitted\n\nClick to revoke permission";

				public static LocString GO_LEFT_DISABLED = "Passing Left through this door is not permitted\n\nClick to grant permission";

				public static LocString GO_RIGHT_ENABLED = "Passing Right through this door is permitted\n\nClick to revoke permission";

				public static LocString GO_RIGHT_DISABLED = "Passing Right through this door is not permitted\n\nClick to grant permission";

				public static LocString GO_UP_ENABLED = "Passing Up through this door is permitted\n\nClick to revoke permission";

				public static LocString GO_UP_DISABLED = "Passing Up through this door is not permitted\n\nClick to grant permission";

				public static LocString GO_DOWN_ENABLED = "Passing Down through this door is permitted\n\nClick to revoke permission";

				public static LocString GO_DOWN_DISABLED = "Passing Down through this door is not permitted\n\nClick to grant permission";

				public static LocString SET_TO_DEFAULT = "Click to clear custom permissions";

				public static LocString SET_TO_CUSTOM = "Click to assign custom permissions";

				public static LocString USING_DEFAULT = "Default Access";

				public static LocString USING_CUSTOM = "Custom Access";
			}

			public class ASSIGNABLESIDESCREEN
			{
				public static LocString TITLE = "Assign {0}";

				public static LocString ASSIGNED = "Assigned";

				public static LocString UNASSIGNED = "-";

				public static LocString DISABLED = "Ineligible";

				public static LocString SORT_BY_DUPLICANT = "Duplicant";

				public static LocString SORT_BY_ASSIGNMENT = "Assignment";

				public static LocString ASSIGN_TO_TOOLTIP = "Assign to {0}";

				public static LocString UNASSIGN_TOOLTIP = "Assigned to {0}";

				public static LocString DISABLED_TOOLTIP = "{0} is ineligible for this job assignment";

				public static LocString PUBLIC = "Public";
			}

			public class COMETDETECTORSIDESCREEN
			{
				public static LocString TITLE = "Space Scanner";

				public static LocString ASSIGNED = "Assigned";

				public static LocString UNASSIGNED = "-";

				public static LocString DISABLED = "Ineligible";

				public static LocString SORT_BY_DUPLICANT = "Duplicant";

				public static LocString SORT_BY_ASSIGNMENT = "Assignment";

				public static LocString ASSIGN_TO_TOOLTIP = "Scanning for {0}";

				public static LocString UNASSIGN_TOOLTIP = "Scanning for {0}";

				public static LocString NOTHING = "Nothing";

				public static LocString COMETS = "Meteor Showers";

				public static LocString ROCKETS = "Rocket Landing Ping";
			}

			public class COMMAND_MODULE_SIDE_SCREEN
			{
				public static LocString TITLE = "Launch Conditions";

				public static LocString DESTINATION_BUTTON = "Show Starmap";
			}

			public class EQUIPPABLESIDESCREEN
			{
				public static LocString TITLE = "Equip {0}";

				public static LocString ASSIGNEDTO = "Assigned to: {Assignee}";

				public static LocString UNASSIGNED = "Unassigned";

				public static LocString GENERAL_CURRENTASSIGNED = "(Owner)";
			}

			public class EQUIPPABLE_SIDE_SCREEN
			{
				public static LocString TITLE = "Assign To Duplicant";

				public static LocString CURRENTLY_EQUIPPED = "Currently Equipped:\n{0}";

				public static LocString NONE_EQUIPPED = "None";

				public static LocString EQUIP_BUTTON = "Equip";

				public static LocString DROP_BUTTON = "Drop";

				public static LocString SWAP_BUTTON = "Swap";
			}

			public class TELEPADSIDESCREEN
			{
				public static LocString TITLE = "Duplicant Printing";

				public static LocString NEXTPRODUCTION = "Next Production: {0}";

				public static LocString GAMEOVER = "Colony Lost";
			}

			public class VALVESIDESCREEN
			{
				public static LocString TITLE = "Flow Control";
			}

			public class MANUALGENERATORSIDESCREEN
			{
				public static LocString TITLE = "Battery Recharge Threshold";

				public static LocString CURRENT_THRESHOLD = "Current Threshold: {0}%";

				public static LocString TOOLTIP = "Duplicants will operate this generator when battery charge falls below the selected percentage";
			}

			public class TIME_OF_DAY_SIDE_SCREEN
			{
				public static LocString TITLE = "Time-of-Day Sensor";

				public static LocString TOOLTIP = "This sensor will send a 1 after the Turn On time, and a 0 after the Turn Off time.";

				public static LocString START = "Turn On";

				public static LocString STOP = "Turn Off";
			}

			public class OIL_WELL_CAP_SIDE_SCREEN
			{
				public static LocString TITLE = "Pressure Release Threshold";

				public static LocString TOOLTIP = "Duplicants will release gas buildup in this well when it exceeds the selected percentage";
			}

			public class LOGIC_DELAY_SIDE_SCREEN
			{
				public static LocString TITLE = "Active Buffer Time";

				public static LocString TOOLTIP = "This gate will continue to send an Active signal for {0} seconds after entering Standby";
			}

			public class TIME_RANGE_SIDE_SCREEN
			{
				public static LocString TITLE = "Time Schedule";

				public static LocString ON = "Activation Time";

				public static LocString ON_TOOLTIP = "Activation time determines the time of day this sensor should enter its active duration" + HORIZONTAL_BR_RULE + "This sensor becomes active {0} through the day";

				public static LocString DURATION = "Active Duration";

				public static LocString DURATION_TOOLTIP = "Active duration determines what percentage of the day this sensor will spend in an Active state" + HORIZONTAL_BR_RULE + "This sensor will be active for {0} of the day";
			}

			public class TIMEDSWITCHSIDESCREEN
			{
				public static LocString TITLE = "Time Schedule";

				public static LocString ONTIME = "On Time:";

				public static LocString OFFTIME = "Off Time:";

				public static LocString TIMETODEACTIVATE = "Time until deactivation: {0}";

				public static LocString TIMETOACTIVATE = "Time until activation: {0}";

				public static LocString WARNING = "Switch must be connected to a " + FormatAsLink("Power", "POWER") + " grid";

				public static LocString CURRENTSTATE = "Current State:";

				public static LocString ON = "On";

				public static LocString OFF = "Off";
			}

			public class CAPTURE_POINT_SIDE_SCREEN
			{
				public static LocString TITLE = "Stable Management";

				public static LocString AUTOWRANGLE = "Auto-Wrangle Surplus";

				public static LocString AUTOWRANGLE_TOOLTIP = "A Rancher will automatically wrangle any critters that exceed the population limit or that do not belong in this stable";

				public static LocString LIMIT_TOOLTIP = "Critters exceeding this population limit will automatically be wrangled:";

				public static LocString UNITS_SUFFIX = " Critters";
			}

			public class TEMPERATURESWITCHSIDESCREEN
			{
				public static LocString TITLE = "Temperature Threshold";

				public static LocString CURRENT_TEMPERATURE = "Current Temperature:\n{0}";

				public static LocString ACTIVATE_IF = "Activate if:";

				public static LocString COLDER_BUTTON = "Below";

				public static LocString WARMER_BUTTON = "Above";
			}

			public class THRESHOLD_SWITCH_SIDESCREEN
			{
				public static LocString TITLE = "Pressure Threshold";

				public static LocString CURRENT_VALUE = "Current {0}:\n{1}";

				public static LocString ACTIVATE_IF = "Activate if:";

				public static LocString ABOVE_BUTTON = "Above";

				public static LocString BELOW_BUTTON = "Below";

				public static LocString STATUS_ACTIVE = "Switch Active";

				public static LocString STATUS_INACTIVE = "Switch Inactive";

				public static LocString PRESSURE = "Pressure";

				public static LocString PRESSURE_TOOLTIP_ABOVE = "Switch will be on if the pressure is above {0}";

				public static LocString PRESSURE_TOOLTIP_BELOW = "Switch will be off if the pressure is below {0}";

				public static LocString TEMPERATURE = "Temperature";

				public static LocString TEMPERATURE_TOOLTIP_ABOVE = "Switch will be on if the ambient temperature is above {0}";

				public static LocString TEMPERATURE_TOOLTIP_BELOW = "Switch will be off if the ambient temperature is below {0}";

				public static LocString DISEASE_TITLE = "Germ Threshold";

				public static LocString DISEASE = "Disease";

				public static LocString DISEASE_TOOLTIP_ABOVE = "Switch will be on if the number of germs is above {0}";

				public static LocString DISEASE_TOOLTIP_BELOW = "Switch will be off if the number of germs is below {0}";

				public static LocString DISEASE_UNITS = "germs";
			}

			public class CAPACITY_CONTROL_SIDE_SCREEN
			{
				public static LocString TITLE = "Automated Storage Capacity";

				public static LocString MAX_LABEL = "Max:";
			}

			public class DOOR_TOGGLE_SIDE_SCREEN
			{
				public static LocString TITLE = "Door Setting";

				public static LocString OPEN = "Door is open.";

				public static LocString AUTO = "Door is on auto.";

				public static LocString CLOSE = "Door is locked.";

				public static LocString PENDING_FORMAT = "{0} {1}";

				public static LocString OPEN_PENDING = "Awaiting Duplicant to open door.";

				public static LocString AUTO_PENDING = "Awaiting Duplicant to automate door.";

				public static LocString CLOSE_PENDING = "Awaiting Duplicant to lock door.";

				public static LocString ACCESS_FORMAT = "{0}\n\n{1}";

				public static LocString ACCESS_OFFLINE = "Emergency Access Permissions:\nAll Duplicants are permitted to use this door until " + FormatAsLink("Power", "POWER") + " is restored.";

				public static LocString POI_INTERNAL = "This door cannot be manually controlled.";
			}

			public class ACTIVATION_RANGE_SIDE_SCREEN
			{
				public static LocString NAME = "Breaktime Policy";

				public static LocString ACTIVATE = "Break starts at:";

				public static LocString DEACTIVATE = "Break ends at:";
			}

			public class CAPACITY_SIDE_SCREEN
			{
				public static LocString TOOLTIP = "Adjust the maximum amount that can be stored here";
			}

			public class SUIT_SIDE_SCREEN
			{
				public static LocString TITLE = "Dock Inventory";

				public static LocString CONFIGURATION_REQUIRED = "Configuration Required:";

				public static LocString CONFIG_REQUEST_SUIT = "Deliver Suit";

				public static LocString CONFIG_REQUEST_SUIT_TOOLTIP = "Duplicants will immediately deliver and dock the nearest unequipped suit";

				public static LocString CONFIG_NO_SUIT = "Leave Empty";

				public static LocString CONFIG_NO_SUIT_TOOLTIP = "The next suited Duplicant to pass by will unequip their suit and dock it here";

				public static LocString CONFIG_CANCEL_REQUEST = "Cancel Request";

				public static LocString CONFIG_CANCEL_REQUEST_TOOLTIP = "Cancel this suit delivery";

				public static LocString CONFIG_DROP_SUIT = "Undock Suit";

				public static LocString CONFIG_DROP_SUIT_TOOLTIP = "Disconnect this suit, dropping it on the ground";

				public static LocString CONFIG_DROP_SUIT_NO_SUIT_TOOLTIP = "There is no suit in this building to undock";
			}

			public class AUTOMATABLE_SIDE_SCREEN
			{
				public static LocString TITLE = "Automatable Storage";

				public static LocString ALLOWMANUALBUTTON = "Allow Manual Use";

				public static LocString ALLOWMANUALBUTTONTOOLTIP = "Allow Duplicants to manually manage these storage materials";
			}

			public class STUDYABLE_SIDE_SCREEN
			{
				public static LocString TITLE = "Analyze Natural Feature";

				public static LocString STUDIED_STATUS = "Researchers have completed their analysis and compiled their findings.";

				public static LocString STUDIED_BUTTON = "ANALYSIS COMPLETE";

				public static LocString SEND_STATUS = "Send a researcher to gather data here.\n\nAnalyzing a natural feature takes time, but yields useful results.";

				public static LocString SEND_BUTTON = "ANALYZE";

				public static LocString PENDING_STATUS = "A researcher is in the process of studying this feature.";

				public static LocString PENDING_BUTTON = "CANCEL ANALYSIS";
			}
		}

		public class USERMENUACTIONS
		{
			public class CLEANTOILET
			{
				public static LocString NAME = "Clean Toilet";

				public static LocString TOOLTIP = "Empty waste from this toilet";
			}

			public class CANCELCLEANTOILET
			{
				public static LocString NAME = "Cancel Clean";

				public static LocString TOOLTIP = "Cancel this cleaning order";
			}

			public class CHANGE_ROOM
			{
				public static LocString REQUEST_OUTFIT = "Request Outfit";

				public static LocString REQUEST_OUTFIT_TOOLTIP = "Request outfit to be delivered to this change room";

				public static LocString CANCEL_REQUEST = "Cancel Request";

				public static LocString CANCEL_REQUEST_TOOLTIP = "Cancel outfit request";

				public static LocString DROP_OUTFIT = "Drop Outfit";

				public static LocString DROP_OUTFIT_TOOLTIP = "Drop outfit on floor";
			}

			public class DUMP
			{
				public static LocString NAME = "Empty";

				public static LocString TOOLTIP = "Dump bottle contents onto the floor";

				public static LocString NAME_OFF = "Cancel Empty";

				public static LocString TOOLTIP_OFF = "Cancel this empty order";
			}

			public class TAGFILTER
			{
				public static LocString NAME = "Filter Settings";

				public static LocString TOOLTIP = "Assign materials to storage";
			}

			public class CANCELCONSTRUCTION
			{
				public static LocString NAME = "Cancel Build";

				public static LocString TOOLTIP = "Cancel this build order";
			}

			public class DIG
			{
				public static LocString NAME = "Dig";

				public static LocString TOOLTIP = "Dig out this cell";

				public static LocString TOOLTIP_OFF = "Cancel this dig order";
			}

			public class CANCELMOP
			{
				public static LocString NAME = "Cancel Mop";

				public static LocString TOOLTIP = "Cancel this mop order";
			}

			public class CANCELDIG
			{
				public static LocString NAME = "Cancel Dig";

				public static LocString TOOLTIP = "Cancel this dig order";
			}

			public class UPROOT
			{
				public static LocString NAME = "Uproot";

				public static LocString TOOLTIP = "Convert this plant into a seed";
			}

			public class CANCELUPROOT
			{
				public static LocString NAME = "Cancel Uproot";

				public static LocString TOOLTIP = "Cancel this uproot order";
			}

			public class HARVEST_WHEN_READY
			{
				public static LocString NAME = "Enable Autoharvest";

				public static LocString TOOLTIP = "Automatically harvest this plant when it matures";
			}

			public class CANCEL_HARVEST_WHEN_READY
			{
				public static LocString NAME = "Disable Autoharvest";

				public static LocString TOOLTIP = "Do not automatically harvest this plant";
			}

			public class HARVEST
			{
				public static LocString NAME = "Harvest";

				public static LocString TOOLTIP = "Harvest materials from this plant";

				public static LocString TOOLTIP_DISABLED = "This plant has nothing to harvest";
			}

			public class CANCELHARVEST
			{
				public static LocString NAME = "Cancel Harvest";

				public static LocString TOOLTIP = "Cancel this harvest order";
			}

			public class ATTACK
			{
				public static LocString NAME = "Attack";

				public static LocString TOOLTIP = "Attack this critter";
			}

			public class CANCELATTACK
			{
				public static LocString NAME = "Cancel Attack";

				public static LocString TOOLTIP = "Cancel this attack order";
			}

			public class CAPTURE
			{
				public static LocString NAME = "Wrangle";

				public static LocString TOOLTIP = "Capture this critter alive";
			}

			public class CANCELCAPTURE
			{
				public static LocString NAME = "Cancel Wrangle";

				public static LocString TOOLTIP = "Cancel this wrangle order";
			}

			public class RELEASEELEMENT
			{
				public static LocString NAME = "Empty Building";

				public static LocString TOOLTIP = "Refund all resources currently in use by this building";
			}

			public class DEMOLISH
			{
				public static LocString NAME = "Deconstruct";

				public static LocString TOOLTIP = "Demolish this building and refund all resources";

				public static LocString NAME_OFF = "Cancel Deconstruct";

				public static LocString TOOLTIP_OFF = "Cancel this deconstruct order";
			}

			public class MANUAL_DELIVERY
			{
				public static LocString NAME = "Disable Delivery";

				public static LocString TOOLTIP = "Do not deliver materials to this building";

				public static LocString NAME_OFF = "Enable Delivery";

				public static LocString TOOLTIP_OFF = "Deliver materials to this building";
			}

			public class SELECTRESEARCH
			{
				public static LocString NAME = "Select Research";

				public static LocString TOOLTIP = "Choose a technology from the Research Tree <color=#F44A47>[R]</color>";
			}

			public class RELOCATE
			{
				public static LocString NAME = "Relocate";

				public static LocString TOOLTIP = "Move this building to a new location" + HORIZONTAL_BR_RULE + "Costs no additional resources";

				public static LocString NAME_OFF = "Cancel Relocation";

				public static LocString TOOLTIP_OFF = "Cancel this relocation order";
			}

			public class ENABLEBUILDING
			{
				public static LocString NAME = "Disable Building";

				public static LocString TOOLTIP = "Halt the use of this building" + HORIZONTAL_BR_RULE + "Disabled buildings consume no energy or resources";

				public static LocString NAME_OFF = "Enable Building";

				public static LocString TOOLTIP_OFF = "Resume the use of this building";
			}

			public class READLORE
			{
				public class SEARCH_COMPUTER_SUCCESS
				{
					public static LocString SEARCH1 = "After searching through the computer's database, I managed to piece together some files that piqued my interest.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH2 = "Searching through the computer, I find some recoverable files that are still readable.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH3 = "The computer looks pristine on the outside, but is corrupted internally. Still, I managed to find one uncorrupted file, and have added it to my database.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH4 = "The computer was wiped almost completely clean, except for one file hidden in the recycle bin.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH5 = "I search the computer, storing what useful data I can find in my own memory.\n\nNew Database Entry unlocked.";

					public static LocString SEARCH6 = "This computer is broken and requires some finessing to get working. Still, I recover a handful of interesting files.\n\nNew Database Entry unlocked.";
				}

				public class SEARCH_COMPUTER_FAIL
				{
					public static LocString SEARCH1 = "Unfortunately, the computer's hard drive is irreparably corrupted.";

					public static LocString SEARCH2 = "The computer was wiped clean before I got here. There is nothing to recover.";

					public static LocString SEARCH3 = "Some intact files are available on the computer, but nothing I haven't already discovered elsewhere. I find nothing else.";

					public static LocString SEARCH4 = "The computer has nothing of import.";

					public static LocString SEARCH5 = "Someone's left a solitaire game up. There's nothing else of interest on the computer.\n\nAlso, it looks as though they were about to lose.";

					public static LocString SEARCH6 = "The background on this computer depicts two kittens hugging in a field of daisies. There is nothing else of import to be found.";

					public static LocString SEARCH7 = "The user alphabetized the shortcuts on their desktop. There is nothing else of import to be found.";

					public static LocString SEARCH8 = "The background is a picture of a golden retriever in a science lab. It looks very confused. There is nothing else of import to be found.";

					public static LocString SEARCH9 = "This user never changed their default background. There is nothing else of import to be found. How dull.";
				}

				public class SEARCH_TECHNOLOGY_SUCCESS
				{
					public static LocString SEARCH1 = "I scour the internal systems and find something of interest.\n\nNew Database Entry discovered.";

					public static LocString SEARCH2 = "I see if I can salvage anything from the electronics. I add what I find to my database.\n\nNew Database Entry discovered.";

					public static LocString SEARCH3 = "I look for anything of interest within the abandoned machinery and add what I find to my database.\n\nNew Database Entry discovered.";
				}

				public class SEARCH_OBJECT_SUCCESS
				{
					public static LocString SEARCH1 = "I look around and recover an old file.\n\nNew Database Entry discovered.";

					public static LocString SEARCH2 = "There's a three-ringed binder inside. I scan the surviving documents.\n\nNew Database Entry discovered.";

					public static LocString SEARCH3 = "A discarded journal inside remains mostly intact. I scan the pages of use.\n\nNew Database Entry discovered.";

					public static LocString SEARCH4 = "A single page of a long printout remains legible. I scan it and add it to my database.\n\nNew Database Entry discovered.";

					public static LocString SEARCH5 = "A few loose papers can be found inside. I scan the ones that look interesting.\n\nNew Database Entry discovered.";

					public static LocString SEARCH6 = "I find a memory stick inside and copy its data into my database.\n\nNew Database Entry discovered.";
				}

				public class SEARCH_OBJECT_FAIL
				{
					public static LocString SEARCH1 = "I look around but find nothing of interest.";
				}

				public static LocString NAME = "Inspect";

				public static LocString TOOLTIP = "Recover files from this structure";

				public static LocString GOTODATABASE = "View Entry";

				public static LocString SEARCH_DISPLAY = "The display is still functional. I copy its message into my database.\n\nNew Database Entry discovered.";

				public static LocString SEARCH_POD = "I search my incoming message history and find a single entry. I move the odd message into my database.\n\nNew Database Entry discovered.";

				public static LocString ALREADY_SEARCHED = "I already took everything of interest from this. I can check the Database to re-read what I found.";
			}

			public class OPENPOI
			{
				public static LocString NAME = "Rummage";

				public static LocString TOOLTIP = "Scrounge for usable materials";

				public static LocString NAME_OFF = "Cancel Rummage";

				public static LocString TOOLTIP_OFF = "Cancel this rummage order";
			}

			public class EMPTYSTORAGE
			{
				public static LocString NAME = "Empty Storage";

				public static LocString TOOLTIP = "Eject all resources from this container";

				public static LocString NAME_OFF = "Cancel Empty";

				public static LocString TOOLTIP_OFF = "Cancel this empty order";
			}

			public class COPY_BUILDING_SETTINGS
			{
				public static LocString NAME = "Copy Settings";

				public static LocString TOOLTIP = "Apply the settings and priorities of this building to other buildings of the same type";
			}

			public class CLEAR
			{
				public static LocString NAME = "Sweep";

				public static LocString TOOLTIP = "Put this object away in the nearest storage container";

				public static LocString NAME_OFF = "Cancel Sweeping";

				public static LocString TOOLTIP_OFF = "Cancel this sweep order";
			}

			public class COMPOST
			{
				public static LocString NAME = "Compost";

				public static LocString TOOLTIP = "Mark this object for compost";

				public static LocString NAME_OFF = "Cancel Compost";

				public static LocString TOOLTIP_OFF = "Cancel this compost order";
			}

			public class UNEQUIP
			{
				public static LocString NAME = "Unequip {0}";

				public static LocString TOOLTIP = "Take off and drop this equipment";
			}

			public class QUARANTINE
			{
				public static LocString NAME = "Quarantine";

				public static LocString TOOLTIP = "Isolate this Duplicant\nThe Duplicant will return to their assigned Cot";

				public static LocString TOOLTIP_DISABLED = "No quarantine zone assigned";

				public static LocString NAME_OFF = "Cancel Quarantine";

				public static LocString TOOLTIP_OFF = "Cancel this quarantine order";
			}

			public class DRAWPATHS
			{
				public static LocString NAME = "Show Navigation";

				public static LocString TOOLTIP = "Show all areas within this Duplicant's reach";

				public static LocString NAME_OFF = "Hide Navigation";

				public static LocString TOOLTIP_OFF = "Hide areas within this Duplicant's reach";
			}

			public class MOVETOLOCATION
			{
				public static LocString NAME = "Move To";

				public static LocString TOOLTIP = "Move this Duplicant to a specific location";
			}

			public class FOLLOWCAM
			{
				public static LocString NAME = "Follow Cam";

				public static LocString TOOLTIP = "Track this Duplicant with the camera";
			}

			public class WORKABLE_DIRECTION_BOTH
			{
				public static LocString NAME = " Set Direction: Both";

				public static LocString TOOLTIP = "Select to make Duplicants wash when passing by in either direction";
			}

			public class WORKABLE_DIRECTION_LEFT
			{
				public static LocString NAME = "Set Direction: Left";

				public static LocString TOOLTIP = "Select to make Duplicants wash when passing by from right to left";
			}

			public class WORKABLE_DIRECTION_RIGHT
			{
				public static LocString NAME = "Set Direction: Right";

				public static LocString TOOLTIP = "Select to make Duplicants wash when passing by from left to right";
			}

			public class MANUAL_PUMP_DELIVERY
			{
				public static class ALLOWED
				{
					public static LocString NAME = "Enable Auto-Bottle";

					public static LocString TOOLTIP = "If enabled, Duplicants will deliver bottled liquids to this building directly from Pitcher Pumps";
				}

				public static class DENIED
				{
					public static LocString NAME = "Disable Auto-Bottle";

					public static LocString TOOLTIP = "If disabled, Duplicants will no longer deliver bottled liquids directly from Pitcher Pumps";
				}

				public static class ALLOWED_GAS
				{
					public static LocString NAME = "Enable Auto-Canister";

					public static LocString TOOLTIP = "If enabled, Duplicants will deliver gas canisters to this building directly from Canister Fillers";
				}

				public static class DENIED_GAS
				{
					public static LocString NAME = "Disable Auto-Canister";

					public static LocString TOOLTIP = "If disabled, Duplicants will no longer deliver gas canisters directly from Canister Fillers";
				}
			}

			public class SUIT_MARKER_TRAVERSAL
			{
				public static class ONLY_WHEN_ROOM_AVAILABLE
				{
					public static LocString NAME = "Clearance: Vacancy";

					public static LocString TOOLTIP = "Suited Duplicants may pass only if there is room in an Exosuit Dock to store their suit";
				}

				public static class ALWAYS
				{
					public static LocString NAME = "Clearance: Always";

					public static LocString TOOLTIP = "Suited Duplicants may pass even if there is no room to store their suits" + HORIZONTAL_BR_RULE + "When Exosuit Docks are all full, Duplicants will unequip their suits and drop them on the floor";
				}
			}
		}

		public class BUILDCATEGORIES
		{
			public static class BASE
			{
				public static LocString NAME = FormatAsLink("Base", "BUILDCATEGORYBASE");

				public static LocString TOOLTIP = "Maintain the colony's infrastructure with these homebase basics.";
			}

			public static class CONVEYANCE
			{
				public static LocString NAME = FormatAsLink("Shipping", "BUILDCATEGORYCONVEYANCE");

				public static LocString TOOLTIP = "Effortlessly transport ore and solid materials around my base.";
			}

			public static class OXYGEN
			{
				public static LocString NAME = FormatAsLink("Oxygen", "BUILDCATEGORYOXYGEN");

				public static LocString TOOLTIP = "Everything I need to keep the colony breathing.";
			}

			public static class POWER
			{
				public static LocString NAME = FormatAsLink("Power", "BUILDCATEGORYPOWER");

				public static LocString TOOLTIP = "Need to power the colony? Here's how to do it!";
			}

			public static class FOOD
			{
				public static LocString NAME = FormatAsLink("Food", "BUILDCATEGORYFOOD");

				public static LocString TOOLTIP = "Keep my Duplicants' spirits high and their bellies full.";
			}

			public static class UTILITIES
			{
				public static LocString NAME = FormatAsLink("Utilities", "BUILDCATEGORYUTILITIES");

				public static LocString TOOLTIP = "Heat up and cool down.";
			}

			public static class PLUMBING
			{
				public static LocString NAME = FormatAsLink("Plumbing", "BUILDCATEGORYPLUMBING");

				public static LocString TOOLTIP = "Get the colony's water running and its sewage flowing.";
			}

			public static class HVAC
			{
				public static LocString NAME = FormatAsLink("Ventilation", "BUILDCATEGORYHVAC");

				public static LocString TOOLTIP = "Control the flow of gas in the base.";
			}

			public static class REFINING
			{
				public static LocString NAME = FormatAsLink("Refinement", "BUILDCATEGORYREFINING");

				public static LocString TOOLTIP = "Use the resources I want, filter the ones I don't.";
			}

			public static class ROCKETRY
			{
				public static LocString NAME = FormatAsLink("Rocketry", "BUILDCATEGORYROCKETRY");

				public static LocString TOOLTIP = "Rocketry";
			}

			public static class MEDICAL
			{
				public static LocString NAME = FormatAsLink("Medicine", "BUILDCATEGORYMEDICAL");

				public static LocString TOOLTIP = "A cure for everything but the common cold.";
			}

			public static class FURNITURE
			{
				public static LocString NAME = FormatAsLink("Furniture", "BUILDCATEGORYFURNITURE");

				public static LocString TOOLTIP = "Amenities to keep my Duplicants happy, comfy and efficient.";
			}

			public static class EQUIPMENT
			{
				public static LocString NAME = FormatAsLink("Stations", "BUILDCATEGORYEQUIPMENT");

				public static LocString TOOLTIP = "Unlock new technologies through the power of science!";
			}

			public static class MISC
			{
				public static LocString NAME = FormatAsLink("Decor", "BUILDCATEGORYMISC");

				public static LocString TOOLTIP = "Spruce up my colony with some lovely interior decorating.";
			}

			public static class AUTOMATION
			{
				public static LocString NAME = FormatAsLink("Automation", "BUILDCATEGORYAUTOMATION");

				public static LocString TOOLTIP = "Automate my base with logic circuits and sensors.";
			}
		}

		public class NEWBUILDCATEGORIES
		{
			public static class BASE
			{
				public static LocString NAME = FormatAsLink("Base", "BUILD_CATEGORY_BASE");

				public static LocString TOOLTIP = "Maintain your colony's infrastructure with these homebase basics.";
			}

			public static class INFRASTRUCTURE
			{
				public static LocString NAME = FormatAsLink("Utilities", "BUILD_CATEGORY_INFRASTRUCTURE");

				public static LocString TOOLTIP = "Power, plumbing, and ventilation can all be found here.";
			}

			public static class FOODANDAGRICULTURE
			{
				public static LocString NAME = FormatAsLink("Food", "BUILD_CATEGORY_FOODANDAGRICULTURE");

				public static LocString TOOLTIP = "Keep your Duplicants' spirits high and their bellies full.";
			}

			public static class LOGISTICS
			{
				public static LocString NAME = FormatAsLink("Logistics", "BUILD_CATEGORY_LOGISTICS");

				public static LocString TOOLTIP = "Devices for base automation and material transport.";
			}

			public static class HEALTHANDHAPPINESS
			{
				public static LocString NAME = FormatAsLink("Accommodation", "BUILD_CATEGORY_HEALTHANDHAPPINESS");

				public static LocString TOOLTIP = "Everything a Duplicant needs to stay happy, healthy, and fulfilled.";
			}

			public static class INDUSTRIAL
			{
				public static LocString NAME = FormatAsLink("Industrials", "BUILD_CATEGORY_INDUSTRIAL");

				public static LocString TOOLTIP = "Machinery for oxygen production, heat management, and material refinement.";
			}

			public static class LADDERS
			{
				public static LocString NAME = "Ladders";

				public static LocString BUILDMENUTITLE = "Ladders";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class TILES
			{
				public static LocString NAME = "Tiles";

				public static LocString BUILDMENUTITLE = "Tiles";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class DOORS
			{
				public static LocString NAME = "Doors";

				public static LocString BUILDMENUTITLE = "Doors";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class TRAVELTUBES
			{
				public static LocString NAME = "Transit\nTubes";

				public static LocString BUILDMENUTITLE = "Transit Tubes";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class STORAGE
			{
				public static LocString NAME = "Storage";

				public static LocString BUILDMENUTITLE = "Storage";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class RESEARCH
			{
				public static LocString NAME = "Research";

				public static LocString BUILDMENUTITLE = "Research";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class GENERATORS
			{
				public static LocString NAME = "Generators";

				public static LocString BUILDMENUTITLE = "Generators";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class WIRES
			{
				public static LocString NAME = "Wires";

				public static LocString BUILDMENUTITLE = "Wires";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class POWERCONTROL
			{
				public static LocString NAME = "Power\nRegulation";

				public static LocString BUILDMENUTITLE = "Power Regulation";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class PLUMBINGSTRUCTURES
			{
				public static LocString NAME = "Plumbing";

				public static LocString BUILDMENUTITLE = "Plumbing";

				public static LocString TOOLTIP = "Get your water running and the sewage flowing.";
			}

			public static class PIPES
			{
				public static LocString NAME = "Pipes";

				public static LocString BUILDMENUTITLE = "Pipes";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class VENTILATIONSTRUCTURES
			{
				public static LocString NAME = "Ventilation";

				public static LocString BUILDMENUTITLE = "Ventilation";

				public static LocString TOOLTIP = "Control the flow of gas in your base.";
			}

			public static class CONVEYANCE
			{
				public static LocString NAME = "Ore\nTransport";

				public static LocString BUILDMENUTITLE = "Ore Transport";

				public static LocString TOOLTIP = "Move solids objects around.";
			}

			public static class LOGICWIRING
			{
				public static LocString NAME = "Logic\nWiring";

				public static LocString BUILDMENUTITLE = "Logic Wiring";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class LOGICGATES
			{
				public static LocString NAME = "Logic\nGates";

				public static LocString BUILDMENUTITLE = "Logic Gates";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class LOGICSWITCHES
			{
				public static LocString NAME = "Logic\nSwitches";

				public static LocString BUILDMENUTITLE = "Logic Switches";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class COOKING
			{
				public static LocString NAME = "Cooking";

				public static LocString BUILDMENUTITLE = "Cooking";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class FARMING
			{
				public static LocString NAME = "Farming";

				public static LocString BUILDMENUTITLE = "Farming";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class RANCHING
			{
				public static LocString NAME = "Ranching";

				public static LocString BUILDMENUTITLE = "Ranching";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class HYGIENE
			{
				public static LocString NAME = "Hygiene";

				public static LocString BUILDMENUTITLE = "Hygiene";

				public static LocString TOOLTIP = "Keep your Duplicants clean.";
			}

			public static class MEDICAL
			{
				public static LocString NAME = "Medical";

				public static LocString BUILDMENUTITLE = "Medical";

				public static LocString TOOLTIP = "A cure for everything but the common cold.";
			}

			public static class RECREATION
			{
				public static LocString NAME = "Recreation";

				public static LocString BUILDMENUTITLE = "Recreation";

				public static LocString TOOLTIP = "Everything needed to reduce stress and increase fun.";
			}

			public static class FURNITURE
			{
				public static LocString NAME = "Furniture";

				public static LocString BUILDMENUTITLE = "Furniture";

				public static LocString TOOLTIP = "Amenities to keep your Duplicants happy, comfy and efficient.";
			}

			public static class DECOR
			{
				public static LocString NAME = "Decor";

				public static LocString BUILDMENUTITLE = "Decor";

				public static LocString TOOLTIP = "Spruce up your colony with some lovely interior decorating.";
			}

			public static class OXYGEN
			{
				public static LocString NAME = "Oxygen";

				public static LocString BUILDMENUTITLE = "Oxygen";

				public static LocString TOOLTIP = "Everything you need to keep your colony breathing.";
			}

			public static class UTILITIES
			{
				public static LocString NAME = "Temperature\nControl";

				public static LocString BUILDMENUTITLE = "Temperature Control";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class REFINING
			{
				public static LocString NAME = "Refinement";

				public static LocString BUILDMENUTITLE = "Refinement";

				public static LocString TOOLTIP = "Use the resources you want, filter the ones you don't.";
			}

			public static class EQUIPMENT
			{
				public static LocString NAME = "Stations";

				public static LocString BUILDMENUTITLE = "Stations";

				public static LocString TOOLTIP = "Unlock new technologies through the power of science!";
			}

			public static class CONDUITSENSORS
			{
				public static LocString NAME = "Pipe Sensors";

				public static LocString BUILDMENUTITLE = "Pipe Sensors";

				public static LocString TOOLTIP = string.Empty;
			}

			public static class ROCKETRY
			{
				public static LocString NAME = "Rocketry";

				public static LocString BUILDMENUTITLE = "Rocketry";

				public static LocString TOOLTIP = string.Empty;
			}
		}

		public class TOOLS
		{
			public class SANDBOX
			{
				public class SANDBOX_TOGGLE
				{
					public static LocString NAME = "SANDBOX";
				}

				public class BRUSH
				{
					public static LocString NAME = "Brush";
				}

				public class SPRINKLE
				{
					public static LocString NAME = "Sprinkle";
				}

				public class FLOOD
				{
					public static LocString NAME = "Fill";
				}

				public class MARQUEE
				{
					public static LocString NAME = "Marquee";
				}

				public class SAMPLE
				{
					public static LocString NAME = "Sample";
				}

				public class HEATGUN
				{
					public static LocString NAME = "Heat Gun";
				}

				public class SPAWNER
				{
					public static LocString NAME = "Spawner";
				}

				public class CLEAR_FLOOR
				{
					public static LocString NAME = "Clear Floor";
				}

				public class DESTROY
				{
					public static LocString NAME = "Destroy";
				}

				public class SPAWN_ENTITY
				{
					public static LocString NAME = "Spawn";
				}

				public class FOW
				{
					public static LocString NAME = "Reveal";
				}
			}

			public class GENERIC
			{
				public static LocString BACK = "Back";

				public static LocString UNKNOWN = "UNKNOWN";

				public static LocString BUILDING_HOVER_NAME_FMT = "{Name}    <style=\"hovercard_element\">({Element})</style>";
			}

			public class ATTACK
			{
				public static LocString NAME = "Attack";

				public static LocString TOOLNAME = "Attack tool";

				public static LocString TOOLACTION = "DRAG TO ATTACK";
			}

			public class CAPTURE
			{
				public static LocString NAME = "Wrangle";

				public static LocString TOOLNAME = "Wrangle tool";

				public static LocString TOOLACTION = "DRAG TO TOGGLE WRANGLE";

				public static LocString NOT_CAPTURABLE = "Cannot Wrangle";
			}

			public class BUILD
			{
				public static LocString NAME = "Build {0}";

				public static LocString TOOLNAME = "Build tool";

				public static LocString TOOLACTION = "CLICK TO BUILD";

				public static LocString TOOLACTION_DRAG = "DRAG TO BUILD";
			}

			public class MOVETOLOCATION
			{
				public static LocString NAME = "Move";

				public static LocString TOOLNAME = "Move Here";

				public static LocString TOOLACTION = "CLICK TO MOVE";

				public static LocString UNREACHABLE = "UNREACHABLE";
			}

			public class COPYSETTINGS
			{
				public static LocString NAME = "Paste Settings";

				public static LocString TOOLNAME = "Paste Settings Tool";

				public static LocString TOOLACTION = "DRAG TO PASTE SETTINGS";
			}

			public class DIG
			{
				public static LocString NAME = "Dig";

				public static LocString TOOLNAME = "Dig tool";

				public static LocString TOOLACTION = "DRAG TO DIG";
			}

			public class DISINFECT
			{
				public static LocString NAME = "Disinfect";

				public static LocString TOOLNAME = "Disinfect tool";

				public static LocString TOOLACTION = "DRAG TO DISINFECT";
			}

			public class CANCEL
			{
				public static LocString NAME = "Cancel";

				public static LocString TOOLNAME = "Cancel tool";

				public static LocString TOOLACTION = "DRAG TO CANCEL ACTION";
			}

			public class DECONSTRUCT
			{
				public static LocString NAME = "Deconstruct";

				public static LocString TOOLNAME = "Deconstruct tool";

				public static LocString TOOLACTION = "DRAG TO DECONSTRUCT";
			}

			public class CLEANUPCATEGORY
			{
				public static LocString NAME = "Clean";

				public static LocString TOOLNAME = "Clean Up tools";
			}

			public class PRIORITIESCATEGORY
			{
				public static LocString NAME = "Sub-Priority";
			}

			public class MARKFORSTORAGE
			{
				public static LocString NAME = "Sweep";

				public static LocString TOOLNAME = "Sweep tool";

				public static LocString TOOLACTION = "DRAG TO SWEEP";
			}

			public class MOP
			{
				public static LocString NAME = "Mop";

				public static LocString TOOLNAME = "Mop tool";

				public static LocString TOOLACTION = "DRAG TO MOP";

				public static LocString TOO_MUCH_LIQUID = "Too Much Liquid";

				public static LocString NOT_ON_FLOOR = "Not On Floor";
			}

			public class HARVEST
			{
				public static LocString NAME = "Harvest";

				public static LocString TOOLNAME = "Harvest tool";

				public static LocString TOOLACTION = "DRAG TO TOGGLE HARVEST";
			}

			public class PRIORITIZE
			{
				public static LocString NAME = "Sub-Priority";

				public static LocString TOOLNAME = "Sub-Priority tool";

				public static LocString TOOLACTION = "DRAG TO ADJUST SUB-PRIORITY";

				public static LocString SPECIFIC_PRIORITY = "Set Sub-Priority: {0}";
			}

			public class EMPTY_PIPE
			{
				public static LocString NAME = "Empty Pipe";

				public static LocString TOOLTIP = "Drag to remove contents of pipe";

				public static LocString TOOLNAME = "Empty Pipe Tool";

				public static LocString TOOLACTION = "DRAG TO REMOVE PIPE CONTENTS";
			}

			public class FILTERSCREEN
			{
				public static LocString OPTIONS = "Tool Filter";
			}

			public class FILTERLAYERS
			{
				public static LocString BUILDINGS = "Buildings";

				public static LocString TILES = "Tiles";

				public static LocString WIRES = "Power Wires";

				public static LocString LIQUIDPIPES = "Liquid Pipes";

				public static LocString GASPIPES = "Gas Pipes";

				public static LocString SOLIDCONDUITS = "Conveyor Rails";

				public static LocString DIGPLACER = "Dig Orders";

				public static LocString CLEANANDCLEAR = "Sweep & Mop Orders";

				public static LocString ALL = "All";

				public static LocString HARVEST_WHEN_READY = "Enable Harvest";

				public static LocString DO_NOT_HARVEST = "Disable Harvest";

				public static LocString ATTACK = "Attack";

				public static LocString LOGIC = "Automation";

				public static LocString BACKWALL = "Background Buildings";
			}
		}

		public class DETAILTABS
		{
			public class STATS
			{
				public static LocString NAME = "Skills";

				public static LocString TOOLTIP = "View this Duplicant's attributes, traits, and daily stress";

				public static LocString GROUPNAME_ATTRIBUTES = "ATTRIBUTES";

				public static LocString GROUPNAME_STRESS = "TODAY'S STRESS";

				public static LocString GROUPNAME_EXPECTATIONS = "EXPECTATIONS";

				public static LocString GROUPNAME_TRAITS = "TRAITS";
			}

			public class SIMPLEINFO
			{
				public static LocString NAME = "Status";

				public static LocString TOOLTIP = "View the current status of the selected object";

				public static LocString GROUPNAME_STATUS = "STATUS";

				public static LocString GROUPNAME_DESCRIPTION = "INFORMATION";

				public static LocString GROUPNAME_CONDITION = "CONDITION";

				public static LocString GROUPNAME_REQUIREMENTS = "REQUIREMENTS";

				public static LocString GROUPNAME_RESEARCH = "RESEARCH";

				public static LocString GROUPNAME_LORE = "RECOVERED FILES";

				public static LocString GROUPNAME_FERTILITY = "EGG CHANCES";
			}

			public class DETAILS
			{
				public static LocString NAME = "Properties";

				public static LocString MINION_NAME = "About";

				public static LocString TOOLTIP = "More information";

				public static LocString MINION_TOOLTIP = "More information";

				public static LocString GROUPNAME_DETAILS = "DETAILS";

				public static LocString GROUPNAME_CONTENTS = "CONTENTS";

				public static LocString GROUPNAME_MINION_CONTENTS = "CARRIED ITEMS";

				public static LocString STORAGE_EMPTY = "None";

				public static LocString CONTENTS_MASS = "{0}: {1}";

				public static LocString CONTENTS_TEMPERATURE = "{0} at {1}";

				public static LocString CONTENTS_ROTTABLE = "\n • {0}";

				public static LocString CONTENTS_DISEASED = "\n • {0}";

				public static LocString NET_STRESS = "<b>Today's Net Stress: {0}%</b>";
			}

			public class PERSONALITY
			{
				public class RESUME
				{
					public class APTITUDES
					{
						public static LocString NAME = "<b><size=13>Personal Interests:</size></b>";

						public static LocString TOOLTIP = "{0} enjoys these types of work";
					}

					public class PERKS
					{
						public static LocString NAME = "<b><size=13>Job Training:</size></b>";

						public static LocString TOOLTIP = "These are permanent skills {0} gained from previous jobs";
					}

					public class CURRENT_ROLE
					{
						public static LocString NAME = "<size=13><b>Current Job:</b> {0}</size>";

						public static LocString TOOLTIP = "{0} is currently working as a {1}";

						public static LocString NOJOB_TOOLTIP = "This {0} is... \"between jobs\" at present";
					}

					public class NO_MASTERED_ROLES
					{
						public static LocString NAME = "None";

						public static LocString TOOLTIP = "{0} has not mastered any jobs yet";
					}

					public static LocString MASTERED_ROLES = "<b><size=13>Mastered Jobs:</size></b>";

					public static LocString MASTERED_ROLES_TOOLTIP = "All traits and expectations a Duplicant learns on a job become permanent once that job is mastered." + HORIZONTAL_BR_RULE + "Switching jobs will not remove these traits.";

					public static LocString JOBTRAINING_TOOLTIP = "{0} gained this skill while working as a {1}";
				}

				public class EQUIPMENT
				{
					public static LocString GROUPNAME_ROOMS = "AMENITIES";

					public static LocString GROUPNAME_OWNABLE = "EQUIPMENT";

					public static LocString NO_ASSIGNABLES = "None";

					public static LocString NO_ASSIGNABLES_TOOLTIP = "{0} has not been assigned any buildings of their own";

					public static LocString UNASSIGNED = "Unassigned";

					public static LocString UNASSIGNED_TOOLTIP = "This Duplicant has not been assigned a {0}";

					public static LocString ASSIGNED_TOOLTIP = "{2} has been assigned a {0}" + HORIZONTAL_BR_RULE + "Effects: {1}";

					public static LocString NOEQUIPMENT = "None";

					public static LocString NOEQUIPMENT_TOOLTIP = "{0}'s wearing their Printday Suit and nothing more";
				}

				public static LocString NAME = "Bio";

				public static LocString TOOLTIP = "View this Duplicant's personality, resume, and amenities";

				public static LocString GROUPNAME_BIO = "ABOUT";

				public static LocString GROUPNAME_RESUME = "{0}'S RESUME";
			}

			public class ENERGYCONSUMER
			{
				public static LocString NAME = "Energy";

				public static LocString TOOLTIP = "View how much power this building consumes";
			}

			public class ENERGYWIRE
			{
				public static LocString NAME = "Energy";

				public static LocString TOOLTIP = "View this wire's network";
			}

			public class ENERGYGENERATOR
			{
				public static LocString NAME = "Energy";

				public static LocString TOOLTIP = "Monitor the power this building is generating";

				public static LocString CIRCUITOVERVIEW = "CIRCUIT OVERVIEW";

				public static LocString GENERATORS = "POWER GENERATORS";

				public static LocString CONSUMERS = "POWER CONSUMERS";

				public static LocString BATTERIES = "BATTERIES";

				public static LocString DISCONNECTED = "Not connected to an electrical circuit";

				public static LocString NOGENERATORS = "No generators on this circuit";

				public static LocString NOCONSUMERS = "No consumers on this circuit";

				public static LocString NOBATTERIES = "No batteries on this circuit";

				public static LocString AVAILABLE_JOULES = FormatAsLink("Power", "POWER") + " stored: {0}";

				public static LocString AVAILABLE_JOULES_TOOLTIP = "Amount of power stored in batteries";

				public static LocString WATTAGE_GENERATED = FormatAsLink("Power", "POWER") + " produced: {0}";

				public static LocString WATTAGE_GENERATED_TOOLTIP = "The total amount of power generated by this circuit";

				public static LocString WATTAGE_CONSUMED = FormatAsLink("Power", "POWER") + " consumed: {0}";

				public static LocString WATTAGE_CONSUMED_TOOLTIP = "The total amount of power used by this circuit";

				public static LocString POTENTIAL_WATTAGE_CONSUMED = "Potential power consumed: {0}";

				public static LocString POTENTIAL_WATTAGE_CONSUMED_TOOLTIP = "The total amount of power that can be used by this circuit if all connected buildings are active";

				public static LocString MAX_SAFE_WATTAGE = "Maximum Safe Wattage: {0}";

				public static LocString MAX_SAFE_WATTAGE_TOOLTIP = "Exceeding this value will overload the circuit and can result in damage to wiring and buildings";
			}

			public class DISEASE
			{
				public class IMMUNE_FACTORS
				{
					public static LocString INTERNAL_GERMS = "{0}: {1}";

					public static LocString INTERNAL_GERMS_TOOLTIP = "This Duplicant is currently fighting a {0} infection" + HORIZONTAL_BR_RULE + "Their body is host to {1}";

					public static LocString IMMUNE_ATTACK_RATE = "    • -{0} immunity per {1}";

					public static LocString IMMUNE_ATTACK_RATE_TOOLTIP = "This Duplicant must expend {0} of their immunity to fight {1} of this disease";

					public static LocString IMMUNE_ATTACK_RATE2 = "    • Immune System: {0}";

					public static LocString IMMUNE_ATTACK_RATE2_TOOLTIP = "This Duplicant's immune system is being strained at a rate of {0} by the {1} present on their body";
				}

				public class INFECTION
				{
					public static LocString AID_SYMPTOMS = "Treatable Symptoms:";

					public static LocString AID_SYMPTOMS_TOOLTIP = "These symptoms are eliminated while a Duplicant is in a Med-Bed receiving medical care";

					public static LocString SYMPTOMS = "Other Symptoms:";

					public static LocString SYMPTOMS_TOOLTIP = "These symptoms must run their course and cannot be alleviated by medical care";

					public static LocString SOURCE_SUFFIX = "{0}";

					public static LocString SOURCE_TOOLTIP_SUFFIX = "\n\nThis symptom causes Duplicants to expel germs from their body" + HORIZONTAL_BR_RULE + "They may infect other Duplicants as a result";

					public static LocString DISCLAIMER = "Contracting this illness will have these effects on a Duplicant:\n";

					public static LocString DISCLAIMER_TOOLTIP = "A Duplicant's immune system must be overwhelmed to contract an illness";

					public static LocString DURATION = "Duration:";

					public static LocString DURATION_TOOLTIP = "Duration is the amount of time it takes a Duplicant to fight off this infection";

					public static LocString DURATION_NORMAL = "    {0}";

					public static LocString DURATION_NORMAL_TOOLTIP = "Duplicants recover from this illness in {0}";

					public static LocString DURATION_AIDREQ = "    {0} (Medical care required)";

					public static LocString DURATION_AIDREQ_TOOLTIP = "Treated Duplicants recover from this illness in {0}" + HORIZONTAL_BR_RULE + "Without medical care, they will be infected indefinitely";
				}

				public class DETAILS
				{
					public class GROWTH_FACTORS
					{
						public class SUBSTRATE
						{
							public static LocString GROW = "    • Growing on {0}: {1}";

							public static LocString GROW_TOOLTIP = "Contact with this substance is causing germs to multiply";

							public static LocString NEUTRAL = "    • No change on {0}";

							public static LocString NEUTRAL_TOOLTIP = "Contact with this substance has no effect on germ count";

							public static LocString DIE = "    • Dying on {0}: {1}";

							public static LocString DIE_TOOLTIP = "Contact with this substance is causing germs to die off";
						}

						public class ENVIRONMENT
						{
							public static LocString TITLE = "    • Surrounded by {0}: {1}";

							public static LocString GROW_TOOLTIP = "This atmosphere is causing germs to multiply";

							public static LocString DIE_TOOLTIP = "This atmosphere is causing germs to die off";
						}

						public class TEMPERATURE
						{
							public static LocString TITLE = "    • Current temperature {0}: {1}";

							public static LocString GROW_TOOLTIP = "This temperature is allowing germs to multiply";

							public static LocString DIE_TOOLTIP = "This temperature is causing germs to die off";
						}

						public class PRESSURE
						{
							public static LocString TITLE = "    • Current pressure {0}: {1}";

							public static LocString GROW_TOOLTIP = "Atmospheric pressure is causing germs to multiply";

							public static LocString DIE_TOOLTIP = "Atmospheric pressure is causing germs to die off";
						}

						public class DYING_OFF
						{
							public static LocString TITLE = "    • <b>Dying off: {0}</b>";

							public static LocString TOOLTIP = "Low germ count in this area is causing germs to die rapidly\n\nLess than {0} germs are on this {1} of material.\n({2} germs/Kg)";
						}

						public class OVERPOPULATED
						{
							public static LocString TITLE = "    • <b>Overpopulated: {0}</b>";

							public static LocString TOOLTIP = "Too many germs are present in this area, resulting in rapid die-off until the population stabilizes\n\nA maximum of {0} can be on this {1} of material.\n({2} germs/Kg)";
						}

						public static LocString TITLE = "\nGrowth factors:";

						public static LocString TOOLTIP = "These conditions are contributing to the multiplication of germs";

						public static LocString RATE_OF_CHANGE = "Change rate: {0}";

						public static LocString RATE_OF_CHANGE_TOOLTIP = "Germ count is fluctuating at a rate of {0}";

						public static LocString HALF_LIFE_NEG = "Half life: {0}";

						public static LocString HALF_LIFE_NEG_TOOLTIP = "In {0} the germ count on this object will be halved";

						public static LocString HALF_LIFE_POS = "Doubling time: {0}";

						public static LocString HALF_LIFE_POS_TOOLTIP = "In {0} the germ count on this object will be doubled";

						public static LocString HALF_LIFE_NEUTRAL = "Static";

						public static LocString HALF_LIFE_NEUTRAL_TOOLTIP = "The germ count is neither increasing nor decreasing";
					}

					public static LocString NODISEASE = "No surface germs";

					public static LocString NODISEASE_TOOLTIP = "There are no germs present on this object";

					public static LocString DISEASE_AMOUNT = "{0}: {1}";

					public static LocString DISEASE_AMOUNT_TOOLTIP = "{0} are present on the surface of the selected object";

					public static LocString DEATH_FORMAT = "{0} dead/cycle";

					public static LocString DEATH_FORMAT_TOOLTIP = "Germ count is being reduced by {0}/cycle";

					public static LocString GROWTH_FORMAT = "{0} spawned/cycle";

					public static LocString GROWTH_FORMAT_TOOLTIP = "Germ count is being increased by {0}/cycle";

					public static LocString NEUTRAL_FORMAT = "No change";

					public static LocString NEUTRAL_FORMAT_TOOLTIP = "Germ count is static";
				}

				public static LocString NAME = "Germs";

				public static LocString TOOLTIP = "View the disease risk presented by the selected object";

				public static LocString DISEASE_SOURCE = "DISEASE SOURCE";

				public static LocString IMMUNE_SYSTEM = "GERM HOST";

				public static LocString CURRENT_GERMS = "SURFACE GERMS";

				public static LocString NO_CURRENT_GERMS = "SURFACE GERMS";

				public static LocString GERMS_INFO = "GERM LIFECYCLE";

				public static LocString INFECTION_INFO = "INFECTION DETAILS";

				public static LocString DISEASE_INFO_POPUP_HEADER = "DISEASE INFO: {0}";

				public static LocString DISEASE_INFO_POPUP_BUTTON = "FULL INFO";

				public static LocString DISEASE_INFO_POPUP_TOOLTIP = "View detailed germ and infection info for {0}";
			}

			public class NEEDS
			{
				public static LocString NAME = "Stress";

				public static LocString TOOLTIP = "View this Duplicant's psychological status";

				public static LocString CURRENT_STRESS_LEVEL = "Current " + FormatAsLink("Stress", "STRESS") + " Level: {0}";

				public static LocString OVERVIEW = "Overview";

				public static LocString STRESS_CREATORS = FormatAsLink("Stress", "STRESS") + " Creators";

				public static LocString STRESS_RELIEVERS = FormatAsLink("Stress", "STRESS") + " Relievers";

				public static LocString CURRENT_NEED_LEVEL = "Current Level: {0}";

				public static LocString NEXT_NEED_LEVEL = "Next Level: {0}";
			}

			public class EGG_CHANCES
			{
				public static LocString CHANCE_FORMAT = "{0}: {1}";

				public static LocString CHANCE_FORMAT_TOOLTIP = "This critter has a {1} chance of laying {0}s.\n\nThis probability increases when the creature:\n{2}";

				public static LocString CHANCE_MOD_FORMAT = "    • {0}\n";

				public static LocString CHANCE_FORMAT_TOOLTIP_NOMOD = "This critter has a {1} chance of laying {0}s.";
			}
		}

		public class BUILDINGEFFECTS
		{
			public class TOOLTIPS
			{
				public static LocString OPERATIONREQUIREMENTS = "All requirements must be met in order for this building to operate";

				public static LocString REQUIRESPOWER = "Must be connected to a power grid with at least -{0} of available power";

				public static LocString REQUIRESELEMENT = "Must receive deliveries of {0} to function";

				public static LocString REQUIRESLIQUIDINPUT = "Must receive liquid from a liquid pipe system";

				public static LocString REQUIRESLIQUIDOUTPUT = "Must expel liquid through a liquid pipe system";

				public static LocString REQUIRESLIQUIDOUTPUTS = "Must expel liquid through a liquid pipe system";

				public static LocString REQUIRESGASINPUT = "Must receive gas from a gas pipe system";

				public static LocString REQUIRESGASOUTPUT = "Must expel gas through a gas pipe system";

				public static LocString REQUIRESGASOUTPUTS = "Must expel gas through a gas pipe system";

				public static LocString REQUIRESMANUALOPERATION = "A Duplicant must be present to run this building";

				public static LocString REQUIRESCREATIVITY = "A Duplicant must work on this object to create art";

				public static LocString REQUIRESPOWERGENERATOR = "Must be connected to a power producing generator to function";

				public static LocString REQUIRESSEED = "Must receive a plant seed";

				public static LocString PREFERS_ROOM = "This building gains additional effects or functionality when built inside a {0}";

				public static LocString REQUIRESROOM = "Must be built within a Dedicated Room" + HORIZONTAL_BR_RULE + "Room will become a {0} after construction";

				public static LocString ALLOWS_FERTILIZER = "Allows fertilizer to be delivered to plants";

				public static LocString ALLOWS_IRRIGATION = "Allows liquids to be delivered to plants";

				public static LocString ALLOWS_IRRIGATION_PIPE = "Allows irrigation pipe connection";

				public static LocString ASSIGNEDDUPLICANT = "This amenity may only be used by the Duplicant it is assigned to";

				public static LocString OPERATIONEFFECTS = "The building will produce these effects when its requirements are met";

				public static LocString BATTERYCAPACITY = "Can hold {0} of power when connected to a generator";

				public static LocString BATTERYLEAK = "Imperfect buildings materials mean {0} battery charge will be lost as heat";

				public static LocString STORAGECAPACITY = "Holds up to {0} of material";

				public static LocString ELEMENTEMITTED = "Produces {1} of {0} when in use";

				public static LocString ELEMENTCONSUMED = "Consumes {1} of {0} when in use";

				public static LocString ELEMENTEMITTEDPERUSE = "Produces {1} of {0} per use";

				public static LocString DISEASEEMITTEDPERUSE = "Produces {1} of {0} per use";

				public static LocString DISEASECONSUMEDPERUSE = "Removes {0} of any disease per use";

				public static LocString ELEMENTCONSUMEDPERUSE = "Consumes {1} of {0} per use";

				public static LocString ENERGYCONSUMED = "Draws {0} from the power grid it's connected to";

				public static LocString ENERGYGENERATED = "Produces {0} for the power grid it's connected to";

				public static LocString ENABLESDOMESTICGROWTH = "Accelerates plant growth and maturation";

				public static LocString HEATGENERATED = "Generates {0} per second\n\nSum temperature change is affected by the material attributes of the heated substance:\n    • mass\n    • specific heat capacity\n    • surface area\n    • insulation thickness\n    • thermal conductivity";

				public static LocString HEATCONSUMED = "Dissipates {0} per second\n\nSum temperature change can be affected by the material attributes of the cooled substance:\n    • mass\n    • specific heat capacity\n    • surface area\n    • insulation thickness\n    • thermal conductivity";

				public static LocString FABRICATES = "Fabrication is the production of items and equipment";

				public static LocString PROCESSES = "This building processes raw materials into refined materials";

				public static LocString PROCESSEDITEM = "Refining this material produces {0}";

				public static LocString PLANTERBOX_PENTALTY = "Plants grow more slowly when contained within boxes";

				public static LocString DECORPROVIDED = "Improves Decor values by {0} in a {1} tile radius";

				public static LocString DECORDECREASED = "Decreases Decor values by {0} in a {1} tile radius";

				public static LocString OVERHEAT_TEMP = "Begins overheating at {0} and melts down at {1}";

				public static LocString MINIMUM_TEMP = "Ceases to function when temperatures fall below {0}";

				public static LocString OVER_PRESSURE_MASS = "Ceases to function when the surrounding mass is above {0}";

				public static LocString REFILLOXYGENTANK = "Refills Exosuit Oxygen Tanks with oxygen for reuse";

				public static LocString DUPLICANTMOVEMENTBOOST = "Duplicants walk {0} faster on this tile";

				public static LocString STRESSREDUCEDPERMINUTE = "Removes {0} of Duplicants' Stress for every uninterrupted minute of use";

				public static LocString REMOVESEFFECTSUBTITLE = "Use of this building will remove the listed effects";

				public static LocString REMOVEDEFFECT = "{0}";

				public static LocString ADDED_EFFECT = "Effect being applied:\n\n{0}\n{1}";

				public static LocString GASCOOLING = "Reduces the temperature of piped gases by {0}";

				public static LocString LIQUIDCOOLING = "Reduces the temperature of piped liquids by {0}";

				public static LocString MAX_WATTAGE = "Drawing more than the maximum allowed power can result in damage to the circuit";

				public static LocString RESEARCH_MATERIALS = "This research station consumes {1} of {0} for each research point produced";

				public static LocString PRODUCES_RESEARCH_POINTS = "Produces {0} research";

				public static LocString REMOVES_DISEASE = "Cooking pasteurizes ingredients and removes their disease risk";

				public static LocString DOCTORING = "Doctoring increases existing health benefits and can allow treatment of otherwise stubborn diseases";

				public static LocString RECREATION = "Improves Duplicant Morale during scheduled Downtime";

				public static LocString HEATGENERATED_AIRCONDITIONER = "Generates heat based on the volume, temperature, and specific heat capacity of the pumped gas" + HORIZONTAL_BR_RULE + "Cooling 1 Kg of room temperature oxygen will output {0} per second";

				public static LocString HEATGENERATED_LIQUIDCONDITIONER = "Generates heat based on the volume, temperature, and specific heat capacity of the pumped liquid" + HORIZONTAL_BR_RULE + "Cooling 1 Kg of room temperature water will output {0} per second";

				public static LocString MOVEMENT_BONUS = "Increases the Runspeed of Duplicants";

				public static LocString COOLANT = "{1} of {0} coolant is required to cool off an item produced by this building" + HORIZONTAL_BR_RULE + "Coolant temperature increase is variable and dictated by the amount of energy needed to cool the produced item";

				public static LocString REFINEMENT_ENERGY_HAS_COOLANT = "{0} of heat will be produced to cool off the fabricated item\n\nThis will raise the temperature of the contained {1} by {2}, and heat the containing building";

				public static LocString REFINEMENT_ENERGY_NO_COOLANT = "{0} of heat will be produced to cool off the fabricated item\n\nIf {1} is used for coolant, its temperature will be raised by {2}, and will heat the containing building";

				public static LocString IMPROVED_BUILDINGS = "Tune Ups will improve these buildings:";

				public static LocString IMPROVED_BUILDINGS_ITEM = "{0}";

				public static LocString GEYSER_PRODUCTION = "While erupting, this geyser will produce {0} at a rate of {1}, and at a temperature of {2}.";

				public static LocString GEYSER_DISEASE = "{0} germs are present in the output of this geyser.";

				public static LocString GEYSER_PERIOD = "This geyser will produce for {0} of every {1}";

				public static LocString GEYSER_YEAR_UNSTUDIED = "A researcher must analyze this geyser to determine its geoactive period.";

				public static LocString GEYSER_YEAR_PERIOD = "This geyser will be active for {0} out of every {1}. It will be dormant the rest of the time.";

				public static LocString GEYSER_YEAR_NEXT_ACTIVE = "This geyser will become active in {0}.";

				public static LocString GEYSER_YEAR_NEXT_DORMANT = "This geyser will become dormant in {0}.";

				public static LocString CAPTURE_METHOD_WRANGLE = "This critter can be captured by a Rancher" + HORIZONTAL_BR_RULE + "Mark critters for capture using the Wrangle Tool <color=#F44A47>(N)</color>";

				public static LocString CAPTURE_METHOD_LURE = "This critter can be moved using a Critter Lure";

				public static LocString CAPTURE_METHOD_TRAP = "This critter can be captured using a Critter Trap";

				public static LocString NOISE_POLLUTION_INCREASE = "Produces noise at {0} dB in a {1} tile radius";

				public static LocString NOISE_POLLUTION_DECREASE = "Dampens noise at {0} dB in a {1} tile radius";

				public static LocString ITEM_TEMPERATURE_ADJUST = "Stored items will reach a temperature of {0} over time";

				public static LocString DIET_HEADER = "Creatures will eat and digest only specific materials.";

				public static LocString DIET_CONSUMED = "This critter can typically consume these materials at the following rates:\n\n{Foodlist}";

				public static LocString DIET_PRODUCED = "This critter will \"produce\" the following materials:\n\n{Items}";

				public static LocString SCALE_GROWTH = "This critter can be sheared every {Time} to produce {Amount} of {Item}";

				public static LocString SCALE_GROWTH_ATMO = "This critter can be sheared every {Time} to produce {Amount} of {Item}" + HORIZONTAL_BR_RULE + "It must be kept in {Atmosphere}-rich environments to regrow sheared {Item}";
			}

			public static LocString OPERATIONREQUIREMENTS = "<b>Requirements:</b>";

			public static LocString REQUIRESPOWER = FormatAsLink("Power", "POWER") + ": {0}";

			public static LocString REQUIRESELEMENT = "Supply of {0}";

			public static LocString REQUIRESLIQUIDINPUT = FormatAsLink("Liquid Intake Pipe", "LIQUIDPIPING");

			public static LocString REQUIRESLIQUIDOUTPUT = FormatAsLink("Liquid Output Pipe", "LIQUIDPIPING");

			public static LocString REQUIRESLIQUIDOUTPUTS = "Two " + FormatAsLink("Liquid Output Pipes", "LIQUIDPIPING");

			public static LocString REQUIRESGASINPUT = FormatAsLink("Gas Intake Pipe", "GASPIPING");

			public static LocString REQUIRESGASOUTPUT = FormatAsLink("Gas Output Pipe", "GASPIPING");

			public static LocString REQUIRESGASOUTPUTS = "Two " + FormatAsLink("Gas Output Pipes", "GASPIPING");

			public static LocString REQUIRESMANUALOPERATION = "Duplicant operation";

			public static LocString REQUIRESCREATIVITY = "Duplicant " + FormatAsLink("Creativity", "ARTIST");

			public static LocString REQUIRESPOWERGENERATOR = FormatAsLink("Power", "POWER") + " generator";

			public static LocString REQUIRESSEED = "1 Unplanted " + FormatAsLink("Seed", "PLANTS");

			public static LocString PREFERS_ROOM = "Preferred Room: {0}";

			public static LocString REQUIRESROOM = "Dedicated Room: {0}";

			public static LocString ALLOWS_FERTILIZER = "Plant " + FormatAsLink("Fertilization", "WILTCONDITIONS");

			public static LocString ALLOWS_IRRIGATION = "Plant " + FormatAsLink("Liquid", "WILTCONDITIONS");

			public static LocString ASSIGNEDDUPLICANT = "Duplicant assignment";

			public static LocString CONSUMESANYELEMENT = "Any Element";

			public static LocString ENABLESDOMESTICGROWTH = "Enables " + FormatAsLink("Plant Domestication", "PLANTS");

			public static LocString OPERATIONEFFECTS = "<b>Effects:</b>";

			public static LocString BATTERYCAPACITY = FormatAsLink("Power", "POWER") + " capacity: {0}";

			public static LocString BATTERYLEAK = FormatAsLink("Power", "POWER") + " runoff: {0}";

			public static LocString STORAGECAPACITY = "Storage capacity: {0}";

			public static LocString ELEMENTEMITTED = "{0}: {1}";

			public static LocString ELEMENTCONSUMED = "{0}: {1}";

			public static LocString ELEMENTEMITTEDPERUSE = "{0}: {1} per use";

			public static LocString DISEASEEMITTEDPERUSE = "{0}: {1} per use";

			public static LocString DISEASECONSUMEDPERUSE = "All Diseases: -{0} per use";

			public static LocString ELEMENTCONSUMEDPERUSE = "{0}: {1} per use";

			public static LocString ENERGYCONSUMED = FormatAsLink("Power", "POWER") + " consumed: {0}";

			public static LocString ENERGYGENERATED = FormatAsLink("Power", "POWER") + ": +{0}";

			public static LocString HEATGENERATED = FormatAsLink("Heat", "HEAT") + ": +{0}/s";

			public static LocString HEATCONSUMED = FormatAsLink("Heat", "HEAT") + ": -{0}/s";

			public static LocString HEATGENERATED_AIRCONDITIONER = FormatAsLink("Heat", "HEAT") + ": +{0} (Approximate Value)";

			public static LocString HEATGENERATED_LIQUIDCONDITIONER = FormatAsLink("Heat", "HEAT") + ": +{0} (Approximate Value)";

			public static LocString FABRICATES = "Fabricates";

			public static LocString FABRICATEDITEM = "{1}";

			public static LocString PROCESSES = "Refines:";

			public static LocString PROCESSEDITEM = "{1}";

			public static LocString PLANTERBOX_PENTALTY = "Planter box penalty";

			public static LocString DECORPROVIDED = FormatAsLink("Decor", "DECOR") + ": {1} (Radius: {2} tiles)";

			public static LocString OVERHEAT_TEMP = "Overheat " + FormatAsLink("Temperature", "HEAT") + ": {0}";

			public static LocString MINIMUM_TEMP = "Freeze " + FormatAsLink("Temperature", "HEAT") + ": {0}";

			public static LocString OVER_PRESSURE_MASS = "Over Pressure: {0}";

			public static LocString REFILLOXYGENTANK = "Refills Exosuit " + EQUIPMENT.PREFABS.OXYGEN_TANK.NAME;

			public static LocString DUPLICANTMOVEMENTBOOST = "Runspeed: +{0}";

			public static LocString STRESSREDUCEDPERMINUTE = FormatAsLink("Stress", "STRESS") + ": {0} per minute";

			public static LocString REMOVESEFFECTSUBTITLE = "Cures";

			public static LocString REMOVEDEFFECT = "{0}";

			public static LocString ADDED_EFFECT = "Added Effect: {0}";

			public static LocString GASCOOLING = FormatAsLink("Cooling factor", "HEAT") + ": {0}";

			public static LocString LIQUIDCOOLING = FormatAsLink("Cooling factor", "HEAT") + ": {0}";

			public static LocString MAX_WATTAGE = "Max " + FormatAsLink("Power", "POWER") + ": {0}";

			public static LocString RESEARCH_MATERIALS = "{0}: {1} per " + FormatAsLink("Research", "RESEARCH") + " point";

			public static LocString PRODUCES_RESEARCH_POINTS = "{0}";

			public static LocString HIT_POINTS_PER_CYCLE = FormatAsLink("Health", "Health") + " per cycle: {0}";

			public static LocString KCAL_PER_CYCLE = FormatAsLink("KCal", "FOOD") + " per cycle: {0}";

			public static LocString REMOVES_DISEASE = "Removes disease";

			public static LocString DOCTORING = "Doctoring";

			public static LocString RECREATION = "Recreation";

			public static LocString COOLANT = "Coolant: {1} {0}";

			public static LocString REFINEMENT_ENERGY = "Heat: {0}";

			public static LocString IMPROVED_BUILDINGS = "Improved Buildings";

			public static LocString IMPROVED_BUILDINGS_ITEM = "{0}";

			public static LocString GEYSER_PRODUCTION = "{0}: {1} at {2}";

			public static LocString GEYSER_DISEASE = "Germs: {0}";

			public static LocString GEYSER_PERIOD = "Eruption Period: {0} every {1}";

			public static LocString GEYSER_YEAR_UNSTUDIED = "Active Period: (Requires Analysis)";

			public static LocString GEYSER_YEAR_PERIOD = "Active Period: {0} every {1}";

			public static LocString GEYSER_YEAR_NEXT_ACTIVE = "Next Activity: {0}";

			public static LocString GEYSER_YEAR_NEXT_DORMANT = "Next Dormancy: {0}";

			public static LocString CAPTURE_METHOD_WRANGLE = "Capture Method: Wrangling";

			public static LocString CAPTURE_METHOD_LURE = "Capture Method: Lures";

			public static LocString CAPTURE_METHOD_TRAP = "Capture Method: Traps";

			public static LocString DIET_HEADER = "Digestion:";

			public static LocString DIET_CONSUMED = "    • Diet: {Foodlist}";

			public static LocString DIET_CONSUMED_ITEM = "{Food}: {Amount}";

			public static LocString DIET_PRODUCED = "    • Excretion: {Items}";

			public static LocString DIET_PRODUCED_ITEM = "{Item}: {Percent} of consumed mass";

			public static LocString SCALE_GROWTH = "Shearable {Item}: {Amount} per {Time}";

			public static LocString SCALE_GROWTH_ATMO = "Shearable {Item}: {Amount} per {Time} ({Atmosphere})";

			public static LocString ITEM_TEMPERATURE_ADJUST = "Stored " + FormatAsLink("Temperature", "HEAT") + ": {0}";

			public static LocString NOISE_CREATED = FormatAsLink("Noise", "SOUND") + ": {0} dB (Radius: {1} tiles)";
		}

		public class LOGIC_PORTS
		{
			public static LocString INPUT_PORTS = FormatAsLink("Auto Inputs", "LOGIC") + ": {0}";

			public static LocString OUTPUT_PORTS = FormatAsLink("Auto Outputs", "LOGIC") + ": {0}";

			public static LocString CONTROL_OPERATIONAL = "Enabled/Disabled";
		}

		public class GAMEOBJECTEFFECTS
		{
			public class INSULATED
			{
				public static LocString NAME = "Insulated";

				public static LocString TOOLTIP = "The composition of this structure drastically reduces thermal conductivity";
			}

			public class TOOLTIPS
			{
				public static LocString CALORIES = "+{0}";

				public static LocString FOOD_QUALITY = "Quality: {0}";

				public static LocString COLDBREATHER = "Lowers ambient air temperature";

				public static LocString GROWTHTIME_SIMPLE = "This plant takes {0} to grow";

				public static LocString GROWTHTIME_REGROWTH = "This plant initially takes {0} to grow, but only {1} to mature after first harvest";

				public static LocString GROWTHTIME = "This plant takes {0} to grow";

				public static LocString INITIALGROWTHTIME = "This plant takes {0} to mature again once replanted";

				public static LocString REGROWTHTIME = "This plant takes {0} to mature again once harvested";

				public static LocString EQUIPMENT_MODS = "{Attribute} {Value}";

				public static LocString REQUIRESFERTILIZER = "This plant requires {1} {0} for basic growth";

				public static LocString IDEAL_FERTILIZER = "This plant requires {1} of {0} for basic growth";

				public static LocString REQUIRES_LIGHT = "This plant requires a light source";

				public static LocString REQUIRES_DARKNESS = "This plant requires complete darkness";

				public static LocString REQUIRES_ATMOSPHERE = "This plant must be submerged in one of the following gases: {0}";

				public static LocString REQUIRES_PRESSURE = "Ambient Gas pressure must be at least {0} for basic growth";

				public static LocString IDEAL_PRESSURE = "This plant requires Gas pressures above {0} for basic growth";

				public static LocString REQUIRES_TEMPERATURE = "Internal temperature must be between {0} and {1} for basic growth";

				public static LocString IDEAL_TEMPERATURE = "This plant requires internal temperatures between {0} and {1} for basic growth";

				public static LocString REQUIRES_SUBMERSION = "This plant must be fully submerged in liquid for basic growth";

				public static LocString FOOD_EFFECTS = "Duplicants will gain the following effects from eating this food: {0}";

				public static LocString REQUIRES_RECEPTACLE = "This plant must be housed in a planter box, farm tile or hydroponic farm to grow domestically";

				public static LocString EMITS_LIGHT = "Emits light";

				public static LocString SEED_PRODUCTION_DIG_ONLY = "May be replanted, but will produce no further seeds";

				public static LocString SEED_PRODUCTION_HARVEST = "Harvesting this plant will yield new seeds";

				public static LocString SEED_PRODUCTION_FINAL_HARVEST = "Yields new seeds on the final harvest of its lifecycle";

				public static LocString SEED_PRODUCTION_FRUIT = "Consuming this plant's fruit will yield new seeds";

				public static LocString SEED_REQUIREMENT_CEILING = "This seed must be planted in a downward facing plot\n\nPress <color=#F44A47>[O]</color> while building farm plots to rotate them";

				public static LocString SEED_REQUIREMENT_WALL = "This seed must be planted in a side facing plot\n\nPress <color=#F44A47>[O]</color> while building plots to rotate them";
			}

			public class DAMAGE_POPS
			{
				public static LocString OVERHEAT = "Overheat Damage";

				public static LocString WRONG_ELEMENT = "Wrong Element Damage";

				public static LocString CIRCUIT_OVERLOADED = "Overload Damage";

				public static LocString LIQUID_PRESSURE = "Pressure Damage";

				public static LocString MINION_DESTRUCTION = "Tantrum Damage";

				public static LocString CONDUIT_CONTENTS_FROZE = "Cold Damage";

				public static LocString CONDUIT_CONTENTS_BOILED = "Heat Damage";

				public static LocString MICROMETEORITE = "Micrometeorite Damage";

				public static LocString COMET = "Meteor Damage";

				public static LocString ROCKET = "Rocket Thruster Damage";
			}

			public static LocString CALORIES = "+{0}";

			public static LocString FOOD_QUALITY = "Quality: {0}";

			public static LocString FORGAVEATTACKER = "Forgiveness";

			public static LocString COLDBREATHER = FormatAsLink("Cooling Effect", "HEAT");

			public static LocString LIFECYCLETITLE = "Growth:";

			public static LocString GROWTHTIME_SIMPLE = "Lifecycle: {0}";

			public static LocString GROWTHTIME_REGROWTH = "Domestic growth: {0} / {1}";

			public static LocString GROWTHTIME = "Growth: {0}";

			public static LocString INITIALGROWTHTIME = "Initial Growth: {0}";

			public static LocString REGROWTHTIME = "Regrowth: {0}";

			public static LocString REQUIRES_LIGHT = FormatAsLink("Light", "LIGHT");

			public static LocString REQUIRES_DARKNESS = FormatAsLink("Darkness", "LIGHT");

			public static LocString REQUIRESFERTILIZER = "{0}: {1}";

			public static LocString IDEAL_FERTILIZER = "{0}: {1}";

			public static LocString EQUIPMENT_MODS = "{Attribute} {Value}";

			public static LocString ROTTEN = "Rotten";

			public static LocString REQUIRES_ATMOSPHERE = FormatAsLink("Atmosphere", "ATMOSPHERE") + ":{0}";

			public static LocString REQUIRES_PRESSURE = FormatAsLink("Air", "ATMOSPHERE") + " Pressure: {0} minimum";

			public static LocString IDEAL_PRESSURE = FormatAsLink("Air", "ATMOSPHERE") + " Pressure: {0}";

			public static LocString REQUIRES_TEMPERATURE = FormatAsLink("Temperature", "HEAT") + ": {0} to {1}";

			public static LocString IDEAL_TEMPERATURE = FormatAsLink("Temperature", "HEAT") + ": {0} to {1}";

			public static LocString REQUIRES_SUBMERSION = FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " Submersion";

			public static LocString FOOD_EFFECTS = "Effects:";

			public static LocString EMITS_LIGHT = FormatAsLink("Light Range", "LIGHT") + ": {0} tiles";

			public static LocString DARKNESS = "Darkness";

			public static LocString LIGHT = "Light";

			public static LocString SEED_PRODUCTION_DIG_ONLY = "Consumes 1 " + FormatAsLink("Seed", "PLANTS");

			public static LocString SEED_PRODUCTION_HARVEST = "Harvest yields " + FormatAsLink("Seeds", "PLANTS");

			public static LocString SEED_PRODUCTION_FINAL_HARVEST = "Final harvest yields " + FormatAsLink("Seeds", "PLANTS");

			public static LocString SEED_PRODUCTION_FRUIT = "Fruit produces " + FormatAsLink("Seeds", "PLANTS");

			public static LocString SEED_REQUIREMENT_CEILING = "Plot Orientation: Downward";

			public static LocString SEED_REQUIREMENT_WALL = "Plot Orientation: Sideways";

			public static LocString REQUIRES_RECEPTACLE = "Farm Plot";

			public static LocString PLANT_MARK_FOR_HARVEST = "Autoharvest Enabled";

			public static LocString PLANT_DO_NOT_HARVEST = "Autoharvest Disabled";
		}

		public class ASTEROIDCLOCK
		{
			public static LocString CYCLE = "Cycle";

			public static LocString CYCLES_OLD = "This Colony is {0} Cycle(s) Old";

			public static LocString TIME_PLAYED = "Time Played: {0} hours";

			public static LocString SCHEDULE_BUTTON_TOOLTIP = "Manage Schedule";
		}

		public class ENDOFDAYREPORT
		{
			public class OXYGEN_CREATED
			{
				public static LocString NAME = ELEMENTS.OXYGEN.NAME + " Generation:";

				public static LocString POSITIVE_TOOLTIP = "My colony generated {0} of " + ELEMENTS.OXYGEN.NAME + " over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "My colony consumed {0} of " + ELEMENTS.OXYGEN.NAME + " over the course of the day";
			}

			public class CALORIES_CREATED
			{
				public static LocString NAME = "Calorie Generation:";

				public static LocString POSITIVE_TOOLTIP = "My colony produced {0} of " + FormatAsLink("Food", "FOOD") + " over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "My colony consumed {0} of " + FormatAsLink("Food", "FOOD") + " over the course of the day";
			}

			public class STRESS_DELTA
			{
				public static LocString NAME = FormatAsLink("Stress", "STRESS") + " Change:";

				public static LocString POSITIVE_TOOLTIP = "My colony's total " + FormatAsLink("Stress", "STRESS") + " has increased by {0}";

				public static LocString NEGATIVE_TOOLTIP = "My colony's total " + FormatAsLink("Stress", "STRESS") + " has decreased by {0}";
			}

			public class TRAVEL_TIME
			{
				public static LocString NAME = "Travel Time:";

				public static LocString POSITIVE_TOOLTIP = "My Duplicants spent a total of {0} traveling between errands throughout of the day";
			}

			public class IDLE_TIME
			{
				public static LocString NAME = "Idle Time:";

				public static LocString POSITIVE_TOOLTIP = "My Duplicants spent a total of {0} idling over the course of the day";
			}

			public class TIME_SPENT
			{
				public static LocString NAME = "Time Breakdown:";

				public static LocString POSITIVE_TOOLTIP = "My Duplicants spent a total of {0} doing errands over the course of the day";
			}

			public class ENERGY_USAGE
			{
				public static LocString NAME = FormatAsLink("Power", "POWER") + " Usage:";

				public static LocString POSITIVE_TOOLTIP = "My colony created {0} of " + FormatAsLink("Power", "POWER") + " over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "My colony consumed {0} of " + FormatAsLink("Power", "POWER") + " over the course of the day";
			}

			public class ENERGY_WASTED
			{
				public static LocString NAME = FormatAsLink("Power", "POWER") + " Wasted:";

				public static LocString POSITIVE_TOOLTIP = string.Empty;

				public static LocString NEGATIVE_TOOLTIP = "My colony lost {0} of " + FormatAsLink("Power", "POWER") + " today due to overproduction and battery runoff";
			}

			public class LEVEL_UP
			{
				public static LocString NAME = "Skill Increases:";

				public static LocString TOOLTIP = "My Duplicants gained a total of {0} skill levels today";
			}

			public class TOILET_INCIDENT
			{
				public static LocString NAME = "Restroom Accidents:";

				public static LocString TOOLTIP = "{0} Duplicants couldn't quite reach the toilet in time today";
			}

			public class DISEASE_ADDED
			{
				public static LocString NAME = FormatAsLink("Diseases", "DISEASE") + " Contracted:";

				public static LocString POSITIVE_TOOLTIP = "{0} Duplicant(s) contracted a " + FormatAsLink("Disease", "DISEASE");

				public static LocString NEGATIVE_TOOLTIP = "{0} Duplicant(s) were cured of a " + FormatAsLink("Disease", "DISEASE");
			}

			public class CONTAMINATED_OXYGEN_FLATULENCE
			{
				public static LocString NAME = FormatAsLink("Flatulence", "POLLUTEDOXYGEN") + " Generation:";

				public static LocString POSITIVE_TOOLTIP = "My colony generated {0} of " + StripLinkFormatting(ELEMENTS.CONTAMINATEDOXYGEN.NAME) + " over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "My colony consumed {0} of " + StripLinkFormatting(ELEMENTS.CONTAMINATEDOXYGEN.NAME) + " over the course of the day";
			}

			public class CONTAMINATED_OXYGEN_TOILET
			{
				public static LocString NAME = FormatAsLink("Toilet Emissions: ", StripLinkFormatting(ELEMENTS.CONTAMINATEDOXYGEN.NAME));

				public static LocString POSITIVE_TOOLTIP = "My colony generated {0} of " + ELEMENTS.CONTAMINATEDOXYGEN.NAME + " over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "My colony consumed {0} of " + ELEMENTS.CONTAMINATEDOXYGEN.NAME + " over the course of the day";
			}

			public class CONTAMINATED_OXYGEN_SUBLIMATION
			{
				public static LocString NAME = FormatAsLink("Sublimation", StripLinkFormatting(ELEMENTS.CONTAMINATEDOXYGEN.NAME)) + ":";

				public static LocString POSITIVE_TOOLTIP = "My colony generated {0} of " + ELEMENTS.CONTAMINATEDOXYGEN.NAME + " over the course of the day";

				public static LocString NEGATIVE_TOOLTIP = "My colony consumed {0} of " + ELEMENTS.CONTAMINATEDOXYGEN.NAME + " over the course of the day";
			}

			public class DISEASE_STATUS
			{
				public static LocString NAME = "Disease Status:";

				public static LocString TOOLTIP = "My Duplicants are covered in {0}";
			}

			public class NOTES
			{
				public static LocString NOTE_ENTRY_LINE_ITEM = "{0}\n{1}: {2}";

				public static LocString BUTCHERED = "Butchered for {0}";

				public static LocString BUTCHERED_CONTEXT = "Butchered";

				public static LocString CRAFTED = "Crafted a {0}";

				public static LocString CRAFTED_USED = "{0} used as ingredient";

				public static LocString CRAFTED_CONTEXT = "Crafted";

				public static LocString HARVESTED = "Harvested {0}";

				public static LocString HARVESTED_CONTEXT = "Harvested";

				public static LocString EATEN = "{0} eaten";

				public static LocString ROTTED = "Rotten {0}";

				public static LocString ROTTED_CONTEXT = "Rotted";

				public static LocString GERMS = "On {0}";

				public static LocString TIME_SPENT = "{0}";
			}

			public static LocString REPORT_TITLE = "DAILY REPORTS";

			public static LocString DAY_TITLE = "Cycle {0}";

			public static LocString DAY_TITLE_TODAY = "Cycle {0} - Today";

			public static LocString DAY_TITLE_YESTERDAY = "Cycle {0} - Yesterday";

			public static LocString NOTIFICATION_TITLE = "Cycle {0} report ready";

			public static LocString NOTIFICATION_TOOLTIP = "The daily report for Cycle {0} is ready to view";

			public static LocString NEXT = "Next";

			public static LocString PREV = "Prev";

			public static LocString ADDED = "Added";

			public static LocString REMOVED = "Removed";

			public static LocString NET = "Net";
		}

		public static class SCHEDULEBLOCKTYPES
		{
			public static class EAT
			{
				public static LocString NAME = "Mealtime";

				public static LocString DESCRIPTION = "EAT:\nDuring Mealtime Duplicants will head to their assigned mess halls and eat.";
			}

			public static class SLEEP
			{
				public static LocString NAME = "Sleep";

				public static LocString DESCRIPTION = "SLEEP:\nWhen it's time to sleep, Duplicants will head to their assigned rooms and rest.";
			}

			public static class WORK
			{
				public static LocString NAME = "Work";

				public static LocString DESCRIPTION = "WORK:\nDuring Work hours Duplicants will perform any pending errands in the colony.";
			}

			public static class RECREATION
			{
				public static LocString NAME = "Recreation";

				public static LocString DESCRIPTION = "HAMMER TIME:\nDuring Hammer Time, Duplicants will relieve their " + FormatAsLink("Stress", "STRESS") + " through dance. Please be aware that no matter how hard my Duplicants try, they will absolutely not be able to touch this.";
			}

			public static class HYGIENE
			{
				public static LocString NAME = "Hygiene";

				public static LocString DESCRIPTION = "HYGIENE:\nDuring " + FormatAsLink("Hygiene", "HYGIENE") + " hours Duplicants will head to their assigned washrooms to get cleaned up.";
			}
		}

		public static class SCHEDULEGROUPS
		{
			public static class HYGENE
			{
				public static LocString NAME = "Bathtime";

				public static LocString DESCRIPTION = "During Bathtime shifts my Duplicants will take care of their hygienic needs, such as going to the bathroom, using the shower or washing their hands.\n\nOnce they're all caught up on personal hygiene, Duplicants will head back to work.";

				public static LocString NOTIFICATION_TOOLTIP = "During Bathtime shifts my Duplicants will take care of their hygienic needs, such as going to the bathroom, using the shower or washing their hands.";
			}

			public static class WORKTIME
			{
				public static LocString NAME = "Work";

				public static LocString DESCRIPTION = "During Work shifts my Duplicants must perform the errands I have placed for them throughout the colony.\n\nIt's important when scheduling to maintain a good work-life balance for my Duplicants to maintain their health and prevent Morale loss.";

				public static LocString NOTIFICATION_TOOLTIP = "During Work shifts my Duplicants must perform the errands I've placed for them throughout the colony.";
			}

			public static class RECREATION
			{
				public static LocString NAME = "Downtime";

				public static LocString DESCRIPTION = "During Downtime my Duplicants they may do as they please.\n\nThis may include personal matters like bathroom visits or snacking, or they may choose to engage in leisure activities like socializing with friends.\n\nDowntime increases Duplicant Morale.";

				public static LocString NOTIFICATION_TOOLTIP = "During Downtime shifts my Duplicants they may do as they please.";
			}

			public static class SLEEP
			{
				public static LocString NAME = "Bedtime";

				public static LocString DESCRIPTION = "My Duplicants use Bedtime shifts to rest up after a hard day's work.\n\nScheduling too few bedtime shifts may prevent my Duplicants from regaining enough Stamina to make it through the following day.";

				public static LocString NOTIFICATION_TOOLTIP = "My Duplicants use Bedtime shifts to rest up after a hard day's work.";
			}

			public static LocString TOOLTIP_FORMAT = "{0}" + HORIZONTAL_BR_RULE + "{1}";

			public static LocString MISSINGBLOCKS = "Warning: Scheduling Issues ({0})";

			public static LocString NOTIME = "No {0} shifts allotted";
		}

		public class ELEMENTAL
		{
			public class PRIMARYELEMENT
			{
				public static LocString NAME = "Primary Element: {0}";

				public static LocString TOOLTIP = "The selected object is primarily composed of {0}";
			}

			public class UNITS
			{
				public static LocString NAME = "Stack Units: {0}";

				public static LocString TOOLTIP = "This stack contains {0} units of {1}";
			}

			public class MASS
			{
				public static LocString NAME = "Mass: {0}";

				public static LocString TOOLTIP = "The selected object has a mass of {0}";
			}

			public class TEMPERATURE
			{
				public static LocString NAME = "Temperature: {0}";

				public static LocString TOOLTIP = "The selected object's current temperature is {0}";
			}

			public class DISEASE
			{
				public static LocString NAME = "Disease: {0}";

				public static LocString TOOLTIP = "There are {0} on the selected object";
			}

			public class SHC
			{
				public static LocString NAME = "Specific Heat Capacity: {0}";

				public static LocString TOOLTIP = "{SPECIFIC_HEAT_CAPACITY} is required to heat 1 g of the selected object by 1 {TEMPERATURE_UNIT}";
			}

			public class THERMALCONDUCTIVITY
			{
				public class ADJECTIVES
				{
					public static LocString VALUE_WITH_ADJECTIVE = "{0} ({1})";

					public static LocString VERY_LOW_CONDUCTIVITY = "Highly Insulating";

					public static LocString LOW_CONDUCTIVITY = "Insulating";

					public static LocString MEDIUM_CONDUCTIVITY = "Conductive";

					public static LocString HIGH_CONDUCTIVITY = "Highly Conductive";

					public static LocString VERY_HIGH_CONDUCTIVITY = "Extremely Conductive";
				}

				public static LocString NAME = "Thermal Conductivity: {0}";

				public static LocString TOOLTIP = "This object can conduct heat to other materials at a rate of {THERMAL_CONDUCTIVITY} W for each degree {TEMPERATURE_UNIT} difference\n\nBetween two objects, the rate of heat transfer will be determined by the object with the lowest Thermal Conductivity";
			}

			public class CONDUCTIVITYBARRIER
			{
				public static LocString NAME = "Insulation Thickness: {0}";

				public static LocString TOOLTIP = "Thick insulation reduces an object's Thermal Conductivity";
			}

			public class VAPOURIZATIONPOINT
			{
				public static LocString NAME = "Vaporization Point: {0}";

				public static LocString TOOLTIP = "The selected object will evaporate into a gas at {0}";
			}

			public class MELTINGPOINT
			{
				public static LocString NAME = "Melting Point: {0}";

				public static LocString TOOLTIP = "The selected object will melt into a liquid at {0}";
			}

			public class OVERHEATPOINT
			{
				public static LocString NAME = "Overheat Modifier: {0}";

				public static LocString TOOLTIP = "This building will overheat and take damage if its temperature reaches {0}\n\nBuilding with better building materials can increase overheat temperature";
			}

			public class FREEZEPOINT
			{
				public static LocString NAME = "Freeze Point: {0}";

				public static LocString TOOLTIP = "The selected object will cool into a solid at {0}";
			}

			public class DEWPOINT
			{
				public static LocString NAME = "Condensation Point: {0}";

				public static LocString TOOLTIP = "The selected object will condense into a liquid at {0}";
			}
		}

		public class IMMIGRANTSCREEN
		{
			public static LocString IMMIGRANTSCREENTITLE = "Select a DNA Blueprint";

			public static LocString PROCEEDBUTTON = "Print";

			public static LocString CANCELBUTTON = "Cancel";

			public static LocString REJECTALL = "Reject All";

			public static LocString EMBARK = "EMBARK";

			public static LocString SELECTDUPLICANTS = "Select {0} Duplicants";

			public static LocString SELECTYOURCREW = "CHOOSE THREE DUPLICANTS TO BEGIN";

			public static LocString SHUFFLE = "SHUFFLE";

			public static LocString SHUFFLETOOLTIP = "Shuffle for a different Duplicant";

			public static LocString BACK = "BACK";

			public static LocString CONFIRMATIONTITLE = "Reject All Duplicants?";

			public static LocString CONFIRMATIONBODY = "The Printing Pod will need time to recharge if I return these Duplicants to ooze.";

			public static LocString NAME_YOUR_COLONY = "NAME THE COLONY";

			public static LocString SHUFFLE_COLONY_NAME = "Randomize colony name";
		}

		public class METERS
		{
			public class HEALTH
			{
				public static LocString TOOLTIP = "Health";
			}

			public class BREATH
			{
				public static LocString TOOLTIP = "Oxygen";
			}

			public class FUEL
			{
				public static LocString TOOLTIP = "Fuel";
			}
		}

		public static string HORIZONTAL_RULE = "------------------";

		public static string HORIZONTAL_BR_RULE = "\n" + HORIZONTAL_RULE + "\n";

		public static string POS_INFINITY = "Infinity";

		public static string NEG_INFINITY = "-Infinity";

		public static LocString NAME_WITH_UNITS = "{0} x {1}";

		public static LocString POSITIVE_FORMAT = "+{0}";

		public static LocString NEGATIVE_FORMAT = "-{0}";

		public static LocString FILTER = "Filter";

		public static LocString SPEED_SLOW = "SLOW";

		public static LocString SPEED_MEDIUM = "MEDIUM";

		public static LocString SPEED_FAST = "FAST";

		public static LocString RED_ALERT = "RED ALERT";

		public static LocString JOBS = "PRIORITIES";

		public static LocString CONSUMABLES = "CONSUMABLES";

		public static LocString VITALS = "VITALS";

		public static LocString RESEARCH = "RESEARCH";

		public static LocString ROLES = "JOB ASSIGNMENTS";

		public static LocString RESEARCHPOINTS = "Research points";

		public static LocString SCHEDULE = "SCHEDULE";

		public static LocString REPORT = "REPORTS";

		public static LocString OVERLAYSTITLE = "OVERLAYS";

		public static LocString ALERTS = "ALERTS";

		public static LocString MESSAGES = "MESSAGES";

		public static LocString ACTIONS = "ACTIONS";

		public static LocString QUEUE = "Queue";

		public static LocString BASECOUNT = "Base {0}";

		public static LocString CHARACTERCONTAINER_SKILLS_TITLE = "ATTRIBUTES";

		public static LocString CHARACTERCONTAINER_TRAITS_TITLE = "TRAITS";

		public static LocString CHARACTERCONTAINER_APTITUDES_TITLE = "INTERESTS";

		public static LocString CHARACTERCONTAINER_EXPECTATIONS_TITLE = "ADDITIONAL";

		public static LocString CHARACTERCONTAINER_SKILL_VALUE = " {0} {1}";

		public static LocString CHARACTERCONTAINER_NEED = "{0}: {1}";

		public static LocString CHARACTERCONTAINER_STRESSTRAIT = "Stress Reaction: {0}";

		public static LocString CHARACTERCONTAINER_CONGENITALTRAIT = "Genetic Trait: {0}";

		public static LocString PRODUCTINFO_SELECTMATERIAL = "Select {0}:";

		public static LocString PRODUCTINFO_RESEARCHREQUIRED = "Research required...";

		public static LocString PRODUCTINFO_REQUIRESRESEARCHDESC = "Requires {0} Research";

		public static LocString PRODUCTINFO_APPLICABLERESOURCES = "Required resources:";

		public static LocString PRODUCTINFO_MISSINGRESOURCES_TITLE = "Requires {0}: {1}";

		public static LocString PRODUCTINFO_MISSINGRESOURCES_HOVER = "Missing resource(s)";

		public static LocString PRODUCTINFO_MISSINGRESOURCES_DESC = "{0} has yet to be discovered.";

		public static LocString EQUIPMENTTAB_OWNED = "Owned Items";

		public static LocString EQUIPMENTTAB_HELD = "Held Items";

		public static LocString EQUIPMENTTAB_ROOM = "Assigned Rooms";

		public static LocString JOBSCREEN_PRIORITY = "Priority";

		public static LocString JOBSCREEN_HIGH = "High";

		public static LocString JOBSCREEN_LOW = "Low";

		public static LocString JOBSCREEN_EVERYONE = "Everyone";

		public static LocString JOBSCREEN_DEFAULT = "New Duplicants";

		public static LocString BUILD_REQUIRES_ROLE = "{ROLE}";

		public static LocString BUILD_REQUIRES_ROLE_TOOLTIP = "Duplicants must be trained in the {ROLE} job to construct this building";

		public static LocString VITALSSCREEN_NAME = "Name";

		public static LocString VITALSSCREEN_STRESS = "Stress";

		public static LocString VITALSSCREEN_HEALTH = "Health";

		public static LocString VITALSSCREEN_IMMUNITY = "Immune System";

		public static LocString VITALSSCREEN_CALORIES = "Fullness";

		public static LocString VITALSSCREEN_RATIONS = "Calories / Cycle";

		public static LocString VITALSSCREEN_EATENTODAY = "Eaten Today";

		public static LocString VITALSSCREEN_RATIONS_TOOLTIP = "Set how many calories this Duplicant may consume daily";

		public static LocString VITALSSCREEN_EATENTODAY_TOOLTIP = "The amount of food this Duplicant has eaten this cycle";

		public static LocString VITALSSCREEN_UNTIL_FULL = "Until Full";

		public static LocString RESEARCHSCREEN_UNLOCKSTOOLTIP = "Unlocks: {0}";

		public static LocString RESEARCHSCREEN_FILTER = "Search Tech";

		public static LocString ATTRIBUTELEVEL = "Expertise: Level {0} {1}";

		public static LocString ATTRIBUTELEVEL_SHORT = "Level {0} {1}";

		public static LocString NEUTRONIUMMASS = "Immeasurable";

		public static LocString CALCULATING = "Calculating...";

		public static LocString FORMATDAY = "{0} cycles";

		public static LocString FORMATSECONDS = "{0}s";

		public static LocString DELIVERED = "Delivered: {0} {1}";

		public static LocString PICKEDUP = "Picked Up: {0} {1}";

		public static LocString COPIED_SETTINGS = "Settings Applied";

		public static LocString WELCOMEMESSAGETITLE = "- ALERT -";

		public static LocString WELCOMEMESSAGEBODY = "I've awoken with a starting crew of three Duplicants, but the situation here is not as we expected.\n\nResources are scarce and our Pod is embedded miles beneath the planet's surface.\n\nI must help my Duplicants survive before I investigate what went wrong.";

		public static LocString WELCOMEMESSAGEBEGIN = "BEGIN";

		public static LocString VIEWDUPLICANTS = "Choose a Duplicant";

		public static LocString DUPLICANTPRINTING = "Duplicant Printing";

		public static LocString ASSIGNDUPLICANT = "Assign Duplicant";

		public static LocString CRAFT = "+1";

		public static LocString CRAFT_CONTINUOUS = "CONTINUOUS";

		public static LocString PLACEINRECEPTACLE = "Plant";

		public static LocString REMOVEFROMRECEPTACLE = "Uproot";

		public static LocString CANCELPLACEINRECEPTACLE = "Cancel";

		public static LocString CANCELREMOVALFROMRECEPTACLE = "Cancel";

		public static LocString CHANGEPERSECOND = "Change per second: {0}";

		public static LocString CHANGEPERCYCLE = "Change per cycle: {0}";

		public static LocString MODIFIER_ITEM_TEMPLATE = "    • {0} : {1}";

		public static LocString LISTENTRYSTRING = "     {0}\n";

		public static LocString LISTENTRYSTRINGNOLINEBREAK = "     {0}";

		public static string FormatAsLink(string text, string linkID)
		{
			text = StripLinkFormatting(text);
			linkID = CodexCache.FormatLinkID(linkID);
			return "<link=\"" + linkID + "\">" + text + "</link>";
		}

		public static string ExtractLinkID(string text)
		{
			int num = text.IndexOf("<link=") + 7;
			int num2 = text.IndexOf(">") - 1;
			return text.Substring(num, num2 - num);
		}

		public static string StripLinkFormatting(string text)
		{
			string text2 = text;
			try
			{
				while (text2.Contains("<link="))
				{
					int num = text2.IndexOf("</link>");
					if (num > -1)
					{
						text2 = text2.Remove(num, 7);
					}
					else
					{
						Debug.LogWarningFormat("String has no closing link tag: {0}");
					}
					int num2 = text2.IndexOf("<link=");
					if (num2 != -1)
					{
						text2 = text2.Remove(num2, 7);
					}
					else
					{
						Debug.LogWarningFormat("String has no open link tag: {0}");
					}
					int num3 = text2.IndexOf("\">");
					if (num3 != -1)
					{
						text2 = text2.Remove(num2, num3 - num2 + 2);
					}
					else
					{
						Debug.LogWarningFormat("String has no open link tag: {0}");
					}
				}
				return text2;
			}
			catch
			{
				Debug.Log("STRIP LINK FORMATTING FAILED ON: " + text, null);
				return text;
			}
		}
	}
}

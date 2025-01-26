/*
	This file is part of Kourageous Tourists /L
		© 2020-2025 LisiasT : http://lisias.net <support@lisias.net>
		© 2017-2020 Nikita Makeev (whale_2)

	Kourageous Tourists /L is double licensed, as follows:
		* SKL 1.0 : https://ksp.lisias.net/SKL-1_0.txt
		* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

	And you are allowed to choose the License that better suit your needs.

	Kourageous Tourists /L is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the SKL Standard License 1.0
	along with Kourageous Tourists /L.
	If not, see <https://ksp.lisias.net/SKL-1_0.txt>.

	You should have received a copy of the GNU General Public License 2.0
	along with Kourageous Tourists /L.
	If not, see <https://www.gnu.org/licenses/>.

*/
using System;

using KSPe;

using Globals = KSPe.Globals<KourageousTourists.Startup>;
using Asset = KSPe.IO.Asset<KourageousTourists.Startup>;
using Data = KSPe.IO.Save<KourageousTourists.Startup>;
using KSPe.IO;

namespace KourageousTourists
{
	internal class Settings : KSPe.IO.SaveGameMonitor.SaveGameLoadedListener
	{
		public const String cfgRoot = "KOURAGE";

		private static Settings instance = null;
		internal static Settings Instance = instance ?? (instance = new Settings());

		internal bool noSkyDiving = false;
		internal bool forceTouristsInSandbox = true;
		internal float paraglidingChutePitch = 1.1f;
		internal float paraglidingDeployDelay = 5f;
		internal float paraglidingMaxAirspeed = 100f;
		internal float paraglidingMinAltAGL = 1500f;

		private Settings()
		{
			KSPe.IO.SaveGameMonitor.Instance.Add(this);
		}

		~Settings()
		{
			KSPe.IO.SaveGameMonitor.Instance.Remove(this);
		}

		void SaveGameMonitor.SaveGameLoadedListener.OnSaveGameLoaded(string name)
		{
			Log.dbg("Settings.OnSaveGameLoaded {0}", name);
			this.VerifySaveGameData();
			this.ReadConfig();
		}

		void SaveGameMonitor.SaveGameLoadedListener.OnSaveGameClosed()
		{
			// The savegame was close.
			// The current instance is not valid anymore!
			// Kill ourselves, and let the new generation take over!
			instance = null;
		}

		internal void Read()
		{
			this.ReadConfig();
		}

		// *** Main Configuration file Kourageous.cfg

		private void ReadConfig()
		{
			ConfigNodeWithSteroids config = SaveGameMonitor.Instance.IsValid ? this.ReadConfigFromSaveGame() : ReadConfigFromDefaults();
			if (config == null)
			{
				Log.warn("No config nodes!");
				return;
			}

			this.noSkyDiving = config.GetValue("noSkyDiving", this.noSkyDiving);
			this.forceTouristsInSandbox = config.GetValue("forceTouristsInSandbox", this.forceTouristsInSandbox);
			Log.detail("debug: {0}; noSkydiving: {1}; forceInSB: {2}", Globals.DebugMode, this.noSkyDiving, this.forceTouristsInSandbox);

			this.paraglidingChutePitch = config.GetValue<float>("paraglidingChutePitch", this.paraglidingChutePitch);
			this.paraglidingDeployDelay = config.GetValue<float>("paraglidingDeployDelay", this.paraglidingDeployDelay);
			this.paraglidingMaxAirspeed = config.GetValue<float>("paraglidingMaxAirpseed", this.paraglidingMaxAirspeed);
			this.paraglidingMinAltAGL = config.GetValue<float>("paraglidingMinAltAGL", this.paraglidingMinAltAGL);
			Log.detail("paragliding params: pitch: {0}, delay: {1}, speed: {2}, alt: {3}", this.paraglidingChutePitch, this.paraglidingDeployDelay, this.paraglidingMaxAirspeed, this.paraglidingMinAltAGL);
		}

		private ConfigNodeWithSteroids ReadConfigFromSaveGame()
		{
			// Interesting fact: this code is run **before** the KSP's savegame folders are created when you create a new game!
			Data.ConfigNode settings = Data.ConfigNode.For(Settings.cfgRoot, "Kourage.cfg");
			if (!settings.IsLoadable) return this.ReadConfigFromDefaults();
			return ConfigNodeWithSteroids.from(settings.Load().Node);
		}

		private ConfigNodeWithSteroids ReadConfigFromDefaults()
		{
			Asset.ConfigNode defaults = Asset.ConfigNode.For(Settings.cfgRoot, "Kourage.cfg");
			if (!defaults.IsLoadable)
			{
				Log.error("Where is the default Kourage.cfg? Kourageous Tourists will not work properly without it!");
				return null;
			}
			return ConfigNodeWithSteroids.from(defaults.Load().Node);
		}

		private void VerifySaveGameData()
		{
			// That's the thing - when you create a new game, Settings is instantiated **before** the savegame folder is created
			// on the disk. So ReadConfigFromSaveGame() borks.
			// In order to prevent that, we check if the data is present when this object is being destructed and, if not, create it
			// with default values.
			Data.ConfigNode settings = Data.ConfigNode.For(Settings.cfgRoot, "Kourage.cfg");
			if (!settings.IsLoadable) settings.Save(Asset.ConfigNode.For(Settings.cfgRoot, "Kourage.cfg").Load().Node);
		}
	}
}

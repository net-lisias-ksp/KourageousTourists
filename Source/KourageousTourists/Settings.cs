/*
	This file is part of Kourageous Tourists /L Unleashed
		© 2020-2021 Lisias T : http://lisias.net <support@lisias.net>
		© 2017-2020 Nikita Makeev (whale_2)

	Kourageous Tourists /L is double licensed, as follows:
		* SKL 1.0 : https://ksp.lisias.net/SKL-1_0.txt
		* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

	And you are allowed to choose the License that better suit your needs.

	Kourageous Tourists /L Unleashed is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the SKL Standard License 1.0
	along with Kourageous Tourists /L Unleashed.
	If not, see <https://ksp.lisias.net/SKL-1_0.txt>.

	You should have received a copy of the GNU General Public License 2.0
	along with Kourageous Tourists /L Unleashed.
	If not, see <https://www.gnu.org/licenses/>.

*/
using System;

using Asset = KSPe.IO.Asset<KourageousTourists.Startup>;
using Data = KSPe.IO.Save<KourageousTourists.Startup>;

namespace KourageousTourists
{
	internal class Settings
	{
		private static Settings instance = null;
		internal static Settings Instance = instance ?? (instance = new Settings());

		private Data.ConfigNode settings;
		private Settings()
		{
			GameEvents.onGameStatePostLoad.Add(this.onGameStateCreated);
		}

		~Settings()
		{
			GameEvents.onGameStatePostLoad.Remove(this.onGameStateCreated);
		}

		private void onGameStateCreated(ConfigNode data)
		{
			Log.dbg("onGameStateCreated {0}", data.GetValue("Tittle"));
			// A new savegame was loaded.
			// The current instance is not valid anymore!
			// Kill ourselves, and let the new generation take over!
			instance = null;
		}

		internal ConfigNode Read() => HighLogic.LoadedSceneIsGame ? this.ReadFromSaveGame() : ReadFromDefaults();

		private ConfigNode ReadFromSaveGame()
		{
			this.settings = this.settings??Data.ConfigNode.For(KourageousTouristsAddOn.cfgRoot, "Kourage.cfg");
			if (!this.settings.IsLoadable)
			{
				this.settings.Clear();
				this.settings.Save(this.ReadFromDefaults());
			}
			return this.settings.Load().Node;
		}

		private ConfigNode ReadFromDefaults()
		{
			Asset.ConfigNode defaults = Asset.ConfigNode.For(KourageousTouristsAddOn.cfgRoot, "Kourage.cfg");
			if (!defaults.IsLoadable)
			{
				Log.error("Where is the default Kourage.cfg? Kourageous Tourists will not work properly without it!");
				return null;
			}
			return defaults.Load().Node;
		}
	}
}

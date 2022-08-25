/*
	This file is part of Kourageous Tourists /L
		© 2020-2022 LisiasT : http://lisias.net <support@lisias.net>
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
using System.Collections.Generic;
using System.Linq;

using Asset = KSPe.IO.Asset<KourageousTourists.Startup>;
using Data = KSPe.IO.Save<KourageousTourists.Startup>;
using File = KSPe.IO.File<KourageousTourists.Startup>;

using Contracts;
using KourageousTourists.Contracts;


namespace KourageousTourists
{
	public class TouristFactory
	{
		public const String cfgRoot = "KOURAGE";
		public const String cfgNode = "LEVEL";

		private static TouristFactory instance = null;
		internal static TouristFactory Instance = instance ?? (instance = new TouristFactory());

		public readonly Dictionary<int,ProtoTourist> touristConfig = new Dictionary<int, ProtoTourist>();

		private TouristFactory ()
		{
			GameEvents.onGameStatePostLoad.Add(this.onGameStateCreated);
			this.touristConfig = new Dictionary<int, ProtoTourist> ();
			this.readConfig();
		}

		~TouristFactory()
		{
			GameEvents.onGameStatePostLoad.Remove(this.onGameStateCreated);
		}

		private void onGameStateCreated(ConfigNode data)
		{
			Log.dbg("TouristFactory.onGameStateCreated {0}", data.GetValue("Tittle"));
			// A new savegame was loaded.
			// The current instance is not valid anymore!
			// Kill ourselves, and let the new generation take over!
			instance = null;
		}

		public Tourist createForLevel(int level, ProtoCrewMember crew) {
			Tourist t = new Tourist ();

			ProtoTourist pt = this.createForLevel(level);
			if (null == pt) return t;

			t.level = pt.level;
			t.abilities = pt.abilities;
			t.situations = pt.situations;
			t.celestialBodies = pt.celestialBodies;
			t.srfspeed = pt.srfspeed;
			t.crew = crew;
			t.rnd = new System.Random ();
			t.isSkydiver = isSkyDiver (crew);
			return t;
		}

		public ProtoTourist createForLevel(int level) {
			ProtoTourist r;
			if (!touristConfig.TryGetValue (level, out r)) {
				Log.warn("Can't find config for level " + level);
				return null;
			}
			return r;
		}

		public static bool isSkyDiver(ProtoCrewMember crew) {
			if (Game.Modes.SANDBOX == HighLogic.CurrentGame.Mode) // In Sandbox, every Tourist is a skydiver!
				return true;

			// Check if this kerbal is participating in any skydiving contract
			if (Game.Modes.CAREER != HighLogic.CurrentGame.Mode)
				return false;

			foreach (Contract c in ContractSystem.Instance.Contracts)
			{
				KourageousSkydiveContract contract = c as KourageousSkydiveContract;
				if (contract != null) {
					if (contract.hasTourist (crew.name)) {
						return true;
					}
				}
			}
			return false;
		}

		private void readConfig()
		{
			Log.dbg("reading config");
			{
				ConfigNode config = GameDatabase.Instance.GetConfigNodes(TouristFactory.cfgRoot).FirstOrDefault();
				this.readConfig(config);
			}
			{
				ConfigNode config = HighLogic.LoadedSceneIsGame ? this.ReadConfigFromSaveGame() : ReadConfigFromDefaults();
				this.readConfig(config);
			}
		}

		private ConfigNode ReadConfigFromSaveGame()
		{
			Data.ConfigNode config = Data.ConfigNode.For(Settings.cfgRoot, "KourageLevels.cfg");
			if (!config.IsLoadable)
			{
				config.Clear();
				File.Asset.CopyToSave("KourageLevels.cfg", "KourageLevels.cfg", true);
			}
			return config.Load().Node;
		}

		private ConfigNode ReadConfigFromDefaults()
		{
			Asset.ConfigNode defaults = Asset.ConfigNode.For(Settings.cfgRoot, "KourageLevels.cfg");
			if (!defaults.IsLoadable)
			{
				Log.error("Where is the default KourageLevels.cfg? Kourageous Tourists will not work properly without it!");
				return null;
			}
			return defaults.Load().Node;
		}

		private bool readConfig(ConfigNode config)
		{
			if (config == null) {
				Log.dbg("no config found in game database");
				return false;
			}

			ConfigNode[] nodes = config.GetNodes (TouristFactory.cfgNode);
			foreach (ConfigNode cfg in nodes) {

				String tLvl = cfg.GetValue("touristlevel");
				if (tLvl == null) {
					Log.dbg("tourist config entry has no attribute 'level'");
					return false;
				}

				Log.dbg("lvl={0}", tLvl);
				ProtoTourist t = new ProtoTourist ();
				int lvl;
				if (!Int32.TryParse (tLvl, out lvl)) {
					Log.dbg("Can't parse tourist level as int: {0}", tLvl);
					return false;
				}
				t.level = lvl;

				if (cfg.HasValue("situations"))
					t.situations.AddRange(
						cfg.GetValue ("situations").Replace (" ", "").Split(','));
				t.situations.RemoveAll(str => String.IsNullOrEmpty(str));
				if (cfg.HasValue("bodies"))
					t.celestialBodies.AddRange(
						cfg.GetValue ("bodies").Replace (" ", "").Split (','));
				t.celestialBodies.RemoveAll(str => String.IsNullOrEmpty(str));
				if (cfg.HasValue("abilities"))
					t.abilities.AddRange(
						cfg.GetValue ("abilities").Replace (" ", "").Split (','));
				t.abilities.RemoveAll(str => String.IsNullOrEmpty(str));
				if (cfg.HasValue("srfspeed")) {
					String srfSpeed = cfg.GetValue ("srfspeed");
					Log.dbg("srfspeed = {0}", srfSpeed);
					double spd = 0.0;
					if (Double.TryParse (srfSpeed, out spd))
						t.srfspeed = spd;
					else
						t.srfspeed = Double.NaN;
				}

				Log.dbg("Adding cfg: {0}", t);
				if (this.touristConfig.ContainsKey(lvl)) this.touristConfig.Remove(lvl);
				this.touristConfig.Add (lvl, t);
			}
			return true;
		}
	}
}


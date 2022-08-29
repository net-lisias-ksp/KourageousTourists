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

namespace KourageousTourists.Util
{
	internal class CelestialBodies
	{
		private static CelestialBodies instance = null;
		public static CelestialBodies Instance => instance??(instance = new CelestialBodies());

		public readonly PSystemBody HomeWorld;
		private readonly Dictionary<string, PSystemBody> db = new Dictionary<string, PSystemBody>();

		private CelestialBodies()
		{
			this.build(PSystemManager.Instance.systemPrefab.rootBody);
			this.HomeWorld = this[FlightGlobals.GetHomeBody()];
			#if DEBUG
			{
				Dictionary<string, PSystemBody>.KeyCollection keysCol = this.db.Keys;
				List<string> keysList = new List<string>(keysCol);
				string[] keys = keysList.ToArray();
				Log.dbg("All known bodies: {0}", string.Join("; ", keys));
				Log.dbg("Home body is: {0}", this.HomeWorld.celestialBody.name);
			}
			#endif
		}

		private void build(PSystemBody parent)
		{
			this.db[parent.celestialBody.name] = parent;
			foreach (PSystemBody psb in parent.children)
				this.build(psb);
		}

		public bool Exists(string name) => this.db.ContainsKey(name);
		public PSystemBody this[string bodyname] => this.db[bodyname];
		public PSystemBody this[CelestialBody body] => this.db[body.name];
		public bool IsHome(CelestialBody body) => body.name == this.HomeWorld.celestialBody.name;
		public bool IsHome(PSystemBody body) => body.name == this.HomeWorld.name;
	}
}

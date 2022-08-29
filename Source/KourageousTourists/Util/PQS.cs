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
using System.Diagnostics;

namespace KourageousTourists.Util
{
	internal class PQS
	{
		private static PQS instance = null;
		public static PQS Instance => instance??(instance = new PQS());

		private readonly Dictionary<string,PSystemBody> bodies = new Dictionary<string,PSystemBody>();
		private readonly Dictionary<string,Dictionary<string,PQSCity>> pqsCities = new Dictionary<string,Dictionary<string,PQSCity>>();
		private readonly Dictionary<string,Dictionary<string,PQSCity2>> pqsCities2 = new Dictionary<string,Dictionary<string,PQSCity2>>();

		private PQS() { }
		~PQS() => instance = null;

		public PQSSurfaceObject this[CelestialBody celestialBody, string name]
		{
			get { 
				PSystemBody body = CelestialBodies.Instance[celestialBody.name];
				if (this.existsPqsCity(body, name)) return this.pqsCities[body.celestialBody.name][name];
				if (this.existsPqsCity2(body, name)) return this.pqsCities2[body.celestialBody.name][name];
				throw new IndexOutOfRangeException(string.Format("{0}:{1}", celestialBody.name, name));
			}
		}

		public bool Exists(CelestialBody body, string name)
		{
			PSystemBody pbody = this.findPSystemBody(body, PSystemManager.Instance.systemPrefab.rootBody);
			return this.existsPqsCity(pbody, name)
				|| this.existsPqsCity2(pbody, name)
			;
		}

		private bool existsPqsCity(PSystemBody pbody, string name)
		{
			if (!this.pqsCities.ContainsKey(pbody.name))
				this.pqsCities[pbody.name] = new Dictionary<string, PQSCity>();

			if (this.pqsCities[pbody.name].ContainsKey(name)) return true;

			foreach (PQSCity p in pbody.pqsVersion.GetComponentsInChildren<PQSCity>(true))
			{
				Log.dbg("Checking {0}/{1}", pbody.celestialBody.name, p.name);
				if (!this.pqsCities[pbody.name].ContainsKey(name)) this.pqsCities[pbody.name][p.name] = p;
				if (name == p.name) return true;
			}

			return false;
		}

		private bool existsPqsCity2(PSystemBody pbody, string name)
		{
			if (!this.pqsCities2.ContainsKey(pbody.name))
				this.pqsCities2[pbody.name] = new Dictionary<string, PQSCity2>();

			if (this.pqsCities2[pbody.name].ContainsKey(name)) return true;

			foreach (PQSCity2 p in pbody.pqsVersion.GetComponentsInChildren<PQSCity2>(true))
			{
				Log.dbg("Checking2 {0}/{1}", pbody.celestialBody.name, p.name);
				if (!this.pqsCities2[pbody.name].ContainsKey(name)) this.pqsCities2[pbody.name][p.name] = p;
				if (name == p.name) return true;
			}

			return false;
		}

		private PSystemBody findPSystemBody(CelestialBody body, PSystemBody parent)
		{
			Log.dbg("findPSystemBody {0} from {1}", body.bodyName, parent.celestialBody.bodyName);
			if (!this.bodies.ContainsKey(body.bodyName))
			{
				Log.dbg("findPSystemBody looking for {0}", body.bodyName);
				foreach (PSystemBody psb in parent.children)
					if (body.bodyName == psb.celestialBody.bodyName)
					{
						this.bodies[body.bodyName] = psb;
						break;
					}
					else
					{
						Log.dbg("{0} has {1} children", psb.celestialBody.name, psb.children.Count);
						if (0 != psb.children.Count) foreach(PSystemBody child in psb.children)
							this.findPSystemBody(child.celestialBody, psb);
					}
			}
			return this.bodies[body.bodyName];
		}
	}
}
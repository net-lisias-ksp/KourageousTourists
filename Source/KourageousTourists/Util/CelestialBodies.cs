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

		private readonly Dictionary<string, PSystemBody> db = new Dictionary<string, PSystemBody>();

		private CelestialBodies()
		{
			this.build(PSystemManager.Instance.systemPrefab.rootBody);
		}

		private void build(PSystemBody parent)
		{
			foreach (PSystemBody psb in parent.children)
			{
				this.db[psb.celestialBody.name] = psb;
				this.build(psb);
			}
		}

		public bool Exists(string name) => this.db.ContainsKey(name);
		public PSystemBody this[string index] => this.db[index];
	}
}

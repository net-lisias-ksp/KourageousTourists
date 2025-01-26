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
using UnityEngine;
using KSPe.Annotations;
using Globals = KSPe.Globals<KourageousTourists.Startup>;

namespace KourageousTourists.Util
{
	[KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
	internal class PQSDumper : MonoBehaviour
	{
		[UsedImplicitly]
		private void Start()
		{
			if(Globals.DebugMode) this.DumpPqs();
		}

		private void DumpPqs()
		{
			Log.force("Dumping all PQS artifacts:");
			this.DumpPqs(PSystemManager.Instance.systemPrefab.rootBody);
		}

		private void DumpPqs(PSystemBody parent)
		{
			foreach (PSystemBody psb in parent.children)
			{
				this.DumpPqs(psb, (PQSMod)null);
				this.DumpPqs(psb, (PQSCity)null);
				this.DumpPqs(psb, (PQSCity2)null);
				this.DumpPqs(psb);
			}
		}

		private void DumpPqs(PSystemBody psb, PQSMod dummy)
		{
			if (null == psb.pqsVersion) return;
			PQSMod[] all = psb.pqsVersion.GetComponentsInChildren<PQSCity2>(true);
			if(null == all) return;
			foreach(PQSMod m in all)
				Log.force("Body {0} : PQSMod {1}", psb.celestialBody.name, m.name);
		}

		private void DumpPqs(PSystemBody psb, PQSCity dummy)
		{
			if (null == psb.pqsVersion) return;
			PQSCity[] all = psb.pqsVersion.GetComponentsInChildren<PQSCity>(true);
			if(null == all) return;
			foreach(PQSCity m in all)
				Log.force("Body {0} : PQSCity {1}", psb.celestialBody.name, m.name);
		}

		private void DumpPqs(PSystemBody psb, PQSCity2 dummy)
		{
			if (null == psb.pqsVersion) return;
			PQSCity2[] all = psb.pqsVersion.GetComponentsInChildren<PQSCity2>(true);
			if(null == all) return;
			foreach(PQSCity2 m in all)
				Log.force("Body {0} : PQSCity2 {1}", psb.celestialBody.name, m.name);
		}
	}
}

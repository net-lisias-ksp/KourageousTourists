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
using System.Linq;

using KSPe;

using UnityEngine;

namespace KourageousTourists.Contracts
{
	public class KourageousAnomalyParameter: KourageousParameter 
	{
		protected string anomalyName;
		protected string anomalyDisplayName;
		protected float minAnomalyDistance = 50;

		public KourageousAnomalyParameter() : base() {}

		public KourageousAnomalyParameter(
			CelestialBody target, string kerbal, string anomaly, string displayName) : base(target, kerbal) {

			this.anomalyName = anomaly;
			this.anomalyDisplayName = displayName;

			setDistance ();
		}

		protected void setDistance() {
			ConfigNodeWithSteroids config = ConfigNodeWithSteroids.from(
											GameDatabase.Instance.GetConfigNodes(KourageousContract.cfgRoot).FirstOrDefault()
										);

			if (config != null) {
				this.minAnomalyDistance = config.GetValue<float>("anomalyDiscoveryDistance", this.minAnomalyDistance);
			}
			else
				Log.warn("no config found in game database");
		}

		protected override string GetHashString() {
			return "walk" + this.targetBody.bodyName + this.tourist;
		}

		protected override string GetTitle() {
			return String.Format ("Take photo of {0} from the surface of {1} near the {2}",
				tourist, targetBody.bodyName, anomalyDisplayName);
		}

		protected override string GetMessageComplete() {
			return String.Format ("{0} was pictured on the surface of {1} in the vicinity of {2}",
				tourist, targetBody.bodyName, anomalyDisplayName);
		}

		protected override void OnRegister() {
			KourageousTouristsAddOn.selfieListeners.Add (onSelfieTaken);
		}

		protected override void OnUnregister() {
			KourageousTouristsAddOn.selfieListeners.Remove (onSelfieTaken);
		}

		private void onSelfieTaken() {
			Log.dbg("onSelfieTaken");
			foreach (Vessel v in FlightGlobals.VesselsLoaded) {
				if (
					v.mainBody == targetBody &&
					v.GetVesselCrew().Count == 1 &&
					v.GetVesselCrew () [0].name.Equals (tourist) &&
					v.situation == Vessel.Situations.LANDED &&
					v.srfSpeed < 0.1f
				) {
					Log.dbg("checking for {0} at {1}", tourist, anomalyName);
					if (this.isNearbyAnomaly (v, anomalyName)) {
						base.SetComplete ();
					}
					break;
				}
			}
		}

		private bool isNearbyAnomaly(Vessel v, string anomalyName) {
			// FIXME: Can we have objects with same names, but on different bodies?
			// FIXME: So far I think we can.
			GameObject[] obj = UnityEngine.Object.FindObjectsOfType<GameObject>();
			foreach (GameObject anomalyObj in obj) {
				
				Component[] c = anomalyObj.GetComponents<PQSCity> ();
				if (c == null || c.Length == 0)
					continue;
				Log.dbg("has pqscity: {0}", anomalyObj.name);
				PQSCity pqscity = (PQSCity)c [0];
				if (pqscity == null)
					continue;

				if (!pqscity.sphere.isAlive)
					continue;

				Transform tr = anomalyObj.GetComponent<Transform> ();

				if (!anomalyObj.name.Equals (anomalyName))
					continue;
				if (tr == null)
					return false;
				float dist1 = Vector3.Distance (v.transform.position, tr.position);
				Log.dbg("distance: {0}; min dist: {1}", dist1, minAnomalyDistance);
				if (dist1 < this.minAnomalyDistance)
					return true;
			}
			return false;
		}
	
		protected override void OnLoad (ConfigNode node)
		{
			base.OnLoad (node);
			this.anomalyName = String.Copy(node.GetValue ("anomalyName"));
			KourageousAnomalyContract.Reload();
			this.anomalyDisplayName =  KourageousAnomalyContract.Instance.anomalies [targetBody.name + ":" + anomalyName].anomalyDescription;
			Log.dbg("display name: {0}", anomalyDisplayName);
			setDistance ();
		}

		protected override void OnSave (ConfigNode node)
		{
			base.OnSave (node);
			node.AddValue ("anomalyName", anomalyName);
		}
	}
}


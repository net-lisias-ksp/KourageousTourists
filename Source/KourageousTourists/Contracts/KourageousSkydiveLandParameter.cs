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

using Contracts;

namespace KourageousTourists.Contracts
{
	public class KourageousSkydiveLandParameter : KourageousParameter
	{
		public KourageousSkydiveLandParameter () : base() {}

		public KourageousSkydiveLandParameter(CelestialBody target, String kerbal) : base(target, kerbal) {}

		protected override string GetHashString() {
			return "jump" + this.targetBody.bodyName + this.tourist;
		}

		protected override string GetTitle() {
			return String.Format ("Let {0} safely land on {1} with the parachute",
				tourist, targetBody.bodyName);
		}

		protected override void OnRegister() {
			Log.detail("setting event onVesselSituationChange");
			GameEvents.onVesselSituationChange.Add (OnSituationChange);
		}

		protected override void OnUnregister() {
			GameEvents.onVesselSituationChange.Remove (OnSituationChange);
		}

		private void OnSituationChange(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> data) {
			if (!data.host.isEVA)
				return;
			
			checkCompletion (data.host);
		}
		
		private void checkCompletion(Vessel v) {

			foreach(ProtoCrewMember c in v.GetVesselCrew())
				Log.detail("param vessel crew: {0}",c.name);
			
			// Check that this tourist has already jumped out
			Log.detail("Checking jump parameter: {0}", Root.GetParameter(0).State);

			if (Root.GetParameter(0).State != ParameterState.Complete)
			{
				return;
			}
			
			if (v.isEVA &&
			    v.mainBody == targetBody &&
			    v.GetVesselCrew().Count == 1 &&
			    v.GetVesselCrew () [0].name.Equals(tourist) &&
			    v.situation == Vessel.Situations.LANDED)
				base.SetComplete ();
		}
	}
}


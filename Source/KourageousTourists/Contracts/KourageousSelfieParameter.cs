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

namespace KourageousTourists.Contracts
{
	public class KourageousSelfieParameter: KourageousParameter 
	{

		public KourageousSelfieParameter() : base() {}

		public KourageousSelfieParameter(CelestialBody target, string kerbal) : base(target, kerbal) {}

		protected override string GetHashString() {
			return "walk" + this.targetBody.bodyName + this.tourist;
		}

		protected override string GetTitle() {
			return String.Format ("Take photo of {0} from the surface of {1}",
				tourist, targetBody.bodyName);
		}

		protected override string GetMessageComplete() {
			return String.Format ("{0} was pictured on the surface of {1}",
				tourist, targetBody.bodyName);
		}

		protected override void OnRegister() {
			KourageousTouristsAddOn.selfieListeners.Add (onSelfieTaken);
		}

		protected override void OnUnregister() {
			KourageousTouristsAddOn.selfieListeners.Remove (onSelfieTaken);
		}

		private void onSelfieTaken() {
			
			foreach (Vessel v in FlightGlobals.VesselsLoaded) {
				if (v.isEVA &&
					v.mainBody == targetBody &&
					v.GetVesselCrew().Count == 1 &&
					v.GetVesselCrew () [0].name.Equals (tourist) &&
					v.situation == Vessel.Situations.LANDED) {

					base.SetComplete ();
					break;
				}
			}
		}
	}

}


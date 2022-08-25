/*
	This file is part of Kourageous Tourists /L
		© 2020-2022 LisiasT : http://lisias.net <support@lisias.net>

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
using System.Collections;

using UnityEngine;

using Log = KourageousTourists.Log;

namespace KourageousTourists.KSP.Chute.Stock14
{
	public class Chutes : ChuteSupport.Interface
	{
		public Chutes(){}

		public bool hasChute(Vessel v)
		{
			return v.evaController.part.Modules.Contains("ModuleEvaChute");
		}

		public IEnumerator deployChute(Vessel v, float paraglidingDeployDelay, float paraglidingChutePitch) {
			Log.detail("Priming chute - KSP14");
			if (!hasChute(v)) {
				Log.detail("No ModuleEvaChute!!! Oops...");
				yield break;
			}
			Log.detail("checking chute module...");
			ModuleEvaChute chuteModule = (ModuleEvaChute)v.evaController.part.Modules ["ModuleEvaChute"];
			Log.detail("deployment state: {0}; enabled: {1}", chuteModule.deploymentSafeState, chuteModule.enabled);
			chuteModule.deploymentSafeState = ModuleParachute.deploymentSafeStates.UNSAFE; // FIXME: is it immediate???

			Log.detail("counting {0} sec...", paraglidingDeployDelay);
			yield return new WaitForSeconds (paraglidingDeployDelay);
			Log.detail("Deploying chute");
			chuteModule.Deploy ();

			// Set low forward pitch so uncontrolled kerbal doesn't gain lot of speed
			chuteModule.chuteDefaultForwardPitch = paraglidingChutePitch;
		}
	}
}

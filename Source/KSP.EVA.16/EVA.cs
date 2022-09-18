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
using System.Collections.Generic;

using Log = KourageousTourists.Log;
using PList = KourageousTourists.EVASupport.ProcessingLists;
using Lists = KourageousTourists.EVASupport.ProcessingLists.Lists;

namespace KourageousTourists.KSP.EVA.Stock16
{
	public class ExtraVehicularActivity : EVASupport.Interface
	{
		public ExtraVehicularActivity(){}

		private static readonly PList PLIST = new PList(
				new Lists(	// Actions
					new HashSet<string>() { // WhiteList
						"OnDeboardSeat",			// For CommandSeats

						"Deploy Chute",				// For Stock Chutes
						"Cut Chute",

						"Deploy chute",				// For Real Chute
						"Cut main chute",
						"Arm parachute",
						"Disarm parachute",
					}
					, PList.DUMMY			// BlackList
				)
				,
				new Lists(	// Events
					new HashSet<string>() { // WhiteList
						"OnDeboardSeat",			// For CommandSeats

						"ChangeHelmet",				// Helmet & Necklace
						"ChangeNeckRing",

						"LowerVisor",				// Visor
						"RaiseVisor",

						"PlantFlag",

						"Deploy",					// Stock Parachutes
						"Repack",
						"Disarm",

						"GUIDeploy",				// For Real Chute
						"GUIArm",
						"GUIDisarm",
						"GUICut",
						"GUIRepack",
						"GUIToggleWindow",
					}
					, new HashSet<string>() { // BlackList
						"MakeReference"
					}
				)
				,
				new Lists(	// Modules
					PList.DUMMY					// WhileList
					, new HashSet<string>() {	// BlackList
						"ModuleScienceExperiment"
					}
				)
			);

		PList EVASupport.Interface.PL => PLIST;

		bool EVASupport.Interface.isHelmetOn(Vessel v)
		{
			List<ProtoCrewMember> roster = v.GetVesselCrew ();
			if (0 == roster.Count) return false;
			ProtoCrewMember crew = roster[0];
			return crew.hasHelmetOn;
		}

		void EVASupport.Interface.equipHelmet(Vessel v)
		{
			KerbalEVA evaCtl;
			if (null == (evaCtl = v.evaController)) return;
			evaCtl.ToggleHelmet(true);
		}

		void EVASupport.Interface.removeHelmet(Vessel v)
		{
			KerbalEVA evaCtl;
			if (null == (evaCtl = v.evaController)) return;
			evaCtl.ToggleHelmet(false);
		}
	}
}

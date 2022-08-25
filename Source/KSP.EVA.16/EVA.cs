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

namespace KourageousTourists.KSP.EVA.Stock16
{
	public class ExtraVehicularActivity : EVASupport.Interface
	{
		public ExtraVehicularActivity(){}

		private static readonly HashSet<string> EVENT_WHITELIST = new HashSet<string>() {
			"ChangeHelmet", "ChangeNeckRing", "OnDeboardSeat"
		};

		private static readonly HashSet<string> EVENT_BLACKLIST = new HashSet<string>() {
			"MakeReference"
		};

		public void disableEvaEvents(Vessel v, bool isEvaEnabled)
		{
			KerbalEVA evaCtl;
			if (null == (evaCtl = v.evaController)) return;
			this.disableEvaEvents(evaCtl, isEvaEnabled);
		}

		public void disableEvaEvents(Part p, bool isEvaEnabled)
		{
			KerbalEVA evaCtl;
			if (null == (evaCtl = p.Modules.GetModule<KerbalEVA>())) return;
			this.disablePartEvents(p, isEvaEnabled);
			this.disableEvaEvents(evaCtl, isEvaEnabled);
		}

		public bool isHelmetOn(Vessel v)
		{
			List<ProtoCrewMember> roster = v.GetVesselCrew ();
			if (0 == roster.Count) return false;
			ProtoCrewMember crew = roster[0];
			return crew.hasHelmetOn;
		}

		public void equipHelmet(Vessel v)
		{
			KerbalEVA evaCtl;
			if (null == (evaCtl = v.evaController)) return;
			evaCtl.ToggleHelmet(true);
		}

		public void removeHelmet(Vessel v)
		{
			KerbalEVA evaCtl;
			if (null == (evaCtl = v.evaController)) return;
			evaCtl.ToggleHelmet(false);
		}

		private void disablePartEvents(Part p, bool isEvaEnabled)
		{
			foreach (BaseEvent e in p.Events) {
				// Preserving the actions needed for EVA. These events should not be preserved if the Tourist can't EVA!
				if (isEvaEnabled && EVENT_WHITELIST.Contains(e.name)) continue;

				// Everything not in the Black List will stay
				if (!EVENT_BLACKLIST.Contains(e.name)) continue;

				Log.dbg("disabling event {0} -- {1}", e.name, e.guiName);
				e.guiActive = false;
				e.guiActiveUnfocused = false;
				e.guiActiveUncommand = false;
			}
		}

		private void disableEvaEvents(KerbalEVA evaCtl, bool isEvaEnabled)
		{
			foreach (BaseEvent e in evaCtl.Events) {
				// Preserving the actions needed for EVA. These events should not be preserved if the Tourist can't EVA!
				if (isEvaEnabled && EVENT_WHITELIST.Contains(e.name)) continue;

				Log.dbg("disabling event {0} -- {1}", e.name, e.guiName);
				e.guiActive = false;
				e.guiActiveUnfocused = false;
				e.guiActiveUncommand = false;
			}

			// ModuleScienceExperiment is only supported on KSP 1.7 **with** Breaking Ground installed, but it does not hurt
			// saving a DLL on the package by shoving the code here.
			foreach (PartModule m in evaCtl.part.Modules) {
				if (!m.ClassName.Equals ("ModuleScienceExperiment"))
					continue;
				Log.dbg("science module id: {0}", ((ModuleScienceExperiment)m).experimentID);
				// Disable all science
				foreach (BaseEvent e in m.Events) {
					Log.dbg("disabling event {0}", e.guiName);
					e.guiActive = false;
					e.guiActiveUnfocused = false;
					e.guiActiveUncommand = false;
				}

				foreach (BaseAction a in m.Actions)
				{
					Log.dbg("disabling action {0}", a.guiName);
					a.active = false;
				}
			}
		}

	}
}

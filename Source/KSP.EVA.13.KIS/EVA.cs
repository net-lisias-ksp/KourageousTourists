﻿/*
	This file is part of Kourageous Tourists /L Unleashed
		© 2020-2021 Lisias T : http://lisias.net <support@lisias.net>

	Kourageous Tourists /L is double licensed, as follows:
		* SKL 1.0 : https://ksp.lisias.net/SKL-1_0.txt
		* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

	And you are allowed to choose the License that better suit your needs.

	Kourageous Tourists /L Unleashed is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the SKL Standard License 1.0
	along with Kourageous Tourists /L Unleashed.
	If not, see <https://ksp.lisias.net/SKL-1_0.txt>.

	You should have received a copy of the GNU General Public License 2.0
	along with Kourageous Tourists /L Unleashed.
	If not, see <https://www.gnu.org/licenses/>.

*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Log = KourageousTourists.Log;

namespace KourageousTourists.KSP.EVA.KIS13
{
	public class ExtraVehicularActivity : EVASupport.Interface
	{
		public ExtraVehicularActivity(){}

		private static readonly HashSet<string> EVENT_WHITELIST = new HashSet<string>() {
			"ToggleInventoryEvent" // KIS Inventory GUI toggle
		};

		private static readonly HashSet<string> EVENT_BLACKLIST = new HashSet<string>() {
			"MakeReference"
		};

		public void disableEvaEvents(Vessel v, bool isEvaEnabled)
		{
			if (null == v.evaController) return;

			KerbalEVA evaCtl = v.evaController;

			foreach (BaseEvent e in evaCtl.Events) {
				// Preserving the actions needed for EVA. These events should not be preserved if the Tourist can't EVA!
				if (EVENT_WHITELIST.Contains(e.name)) continue;

				Log.dbg("disabling event {0} -- {1}", e.name, e.guiName);
				e.guiActive = false;
				e.guiActiveUnfocused = false;
				e.guiActiveUncommand = false;
			}

			// ModuleScienceExperiment is only supported on KSP 1.7 **with** Breaking Ground installed, but it does not hurt
			// saving a DLL by shoving the code here.
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

		public void disableEvaEvents(Part p, bool isEvaEnabled)
		{
			this.disablePartEvents(p, isEvaEnabled);
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

		public bool isHelmetOn(Vessel v)
		{
			KIS.ModuleKISInventory m = v.FindPartModuleImplementing<KIS.ModuleKISInventory>();
			if (null == m) return true; // No KIS, so we should be using the helmet.
			return m.helmetEquipped;
		}

		public void equipHelmet(Vessel v)
		{
			KIS.ModuleKISInventory m = v.FindPartModuleImplementing<KIS.ModuleKISInventory>();
			if (null == m) return;
			if (m.helmetEquipped) return;
			m.SetHelmet(true);
		}

		public void removeHelmet(Vessel v)
		{
			KIS.ModuleKISInventory m = v.FindPartModuleImplementing<KIS.ModuleKISInventory>();
			if (null == m) return;
			if (!m.helmetEquipped) return;
			m.SetHelmet(false);
		}
	}
}
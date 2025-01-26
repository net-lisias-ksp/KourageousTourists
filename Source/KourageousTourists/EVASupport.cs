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
using System.Collections;
using System.Collections.Generic;

namespace KourageousTourists
{
	public class EVASupport
	{
		public class ProcessingLists
		{
			public static readonly HashSet<string> DUMMY = new HashSet<string>();
			public class Lists
			{
				public readonly HashSet<string> WHITELIST;
				public readonly HashSet<string> BLACKLIST;
				public Lists(HashSet<string> whiteList, HashSet<string> blackList)
				{
					this.WHITELIST = whiteList;
					this.BLACKLIST = blackList;
				}
			}
			public readonly Lists ACTION;
			public readonly Lists EVENT;
			public readonly Lists MODULE;

			public ProcessingLists(Lists a, Lists e, Lists m)
			{
				this.ACTION = a;
				this.EVENT = e;
				this.MODULE = m;
			}
		}

		public interface Interface
		{
			bool isHelmetOn(Vessel v);
			void equipHelmet(Vessel v);
			void removeHelmet(Vessel v);
			ProcessingLists PL { get; }
		}

		internal static readonly Interface Instance;
		private static Interface GetInstance()
		{
			Log.dbg("Looking for {0}", typeof(Interface).Name);
			Interface r = (Interface)KSPe.Util.SystemTools.Interface.CreateInstanceByInterface(typeof(Interface));
			if (null == r) Log.error("No realisation for the EVASupport Interface found! We are doomed!");
			return r;
		}
		static EVASupport()
		{
			Instance = GetInstance();
		}

		internal static void adjustEvaEvents(Vessel v, bool isEvaAllowed)
		{
			KerbalEVA evaCtl;
			if (null == (evaCtl = v.evaController)) return;
			Common.adjustEvaEvents(evaCtl, isEvaAllowed, false, Instance.PL);
			Common.adjustEvaModules(evaCtl.part.Modules, isEvaAllowed, false, Instance.PL);
		}

		internal static void adjustEvaEvents(Part p, bool isEvaAllowed)
		{
			Common.adjustPartEvents(p, isEvaAllowed, false, Instance.PL);
		}

		private static class Common
		{
			internal static void adjustPartEvents(Part p, bool isEvaAllowed, bool isActive, ProcessingLists PL)
			{
				foreach (BaseEvent e in p.Events) {
					// Preserving the actions needed for EVA. These events should not be preserved if the Tourist can't EVA!
					if (isEvaAllowed && PL.EVENT.WHITELIST.Contains(e.name))
					{
						Log.dbg("preserving event {0} -- {1}", e.name, e.guiName);
						continue;
					}

					// Everything not in the Black List will stay
					if (!PL.EVENT.BLACKLIST.Contains(e.name))
					{
						Log.dbg("preserving event {0} -- {1}", e.name, e.guiName);
						continue;
					}

					Common.set(e, isActive);
				}
			}

			internal static void adjustEvaEvents(KerbalEVA evaCtl, bool isEvaAllowed, bool isActive, ProcessingLists PL)
			{
				foreach (BaseEvent e in evaCtl.Events)
				{
					// Preserving the actions needed for EVA. These events should not be preserved if the Tourist can't EVA!
					if (isEvaAllowed && PL.EVENT.WHITELIST.Contains(e.name))
					{
						Log.dbg("preserving event {0} -- {1}", e.name, e.guiName);
						continue;
					}

					// Everything not in the Black List will stay
					if (!PL.EVENT.BLACKLIST.Contains(e.name))
					{
						Log.dbg("preserving event {0} -- {1}", e.name, e.guiName);
						continue;
					}

					set(e, isActive);
				}
			}

			internal static void adjustEvaModules(PartModuleList modules, bool isEvaAllowed, bool isActive, ProcessingLists PL)
			{
				foreach (PartModule m in modules)
				{
					// Preserving the modules needed for EVA. These modules should not be preserved if the Tourist can't EVA!
					if (isEvaAllowed && PL.MODULE.WHITELIST.Contains(m.name))
					{
						Log.dbg("preserving module {0}", m.name);
						continue;
					}

					// Everything in the Black List will be bluntly deactivated
					if (PL.MODULE.BLACKLIST.Contains(m.name))
					{
						dbg(isActive, "module {0} id:{1}", m.name, m.GetInstanceID());
						foreach (BaseEvent e in m.Events) set(e, isActive);
						foreach (BaseAction a in m.Actions) set(a, isActive);
						continue;
					}

					dbg(isActive, "selectivelly module {0} id:{1}", m.name, m.GetInstanceID());
					foreach (BaseEvent e in m.Events)
					{
						bool v = isEvaAllowed && PL.EVENT.WHITELIST.Contains(e.name);
						v &= !PL.EVENT.BLACKLIST.Contains(e.name);
						set(e, v);
					}
					foreach (BaseAction a in m.Actions)
					{
						bool v = isEvaAllowed && PL.ACTION.WHITELIST.Contains(a.name);
						v &= !PL.ACTION.BLACKLIST.Contains(a.name);
						set(a, v);
					}
				}
			}

			internal static void set(BaseEvent e, bool isActive)
			{
				dbg(isActive, "event {0} -- {1}", e.name, e.guiName);
				e.guiActive = isActive;
				e.guiActiveUnfocused = isActive;
				e.guiActiveUncommand = isActive;
			}

			internal static void set(BaseAction a, bool isActive)
			{
				dbg(isActive, "action {0}", a.guiName);
				a.active = isActive;
			}

			private static void dbg(bool isActive, string msg, params object[] @params)
			{
				Log.dbg((isActive ? "enabling " : "disabling ") + msg, @params);
			}
		}
	}
}

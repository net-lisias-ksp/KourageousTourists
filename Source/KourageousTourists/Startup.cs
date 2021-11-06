/*
	This file is part of Kourageous Tourists /L Unleashed
		© 2020-2021 Lisias T : http://lisias.net <support@lisias.net>
		© 2017-2020 Nikita Makeev (whale_2)

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
using System;

using UnityEngine;

namespace KourageousTourists
{
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	internal class Startup : MonoBehaviour
	{
		private void Start()
		{
			Log.force("Version {0}", Version.Text);

			try
			{
				KSPe.Util.Installation.Check<Startup>(typeof(Version));
			}
			catch (KSPe.Util.InstallmentException e)
			{
				Log.error(e, this);
				KSPe.Common.Dialogs.ShowStopperAlertBox.Show(e);
			}
		}

		private void Awake()
		{
			string path = KSPe.IO.File<Startup>.Asset.Solve("dlls");
			Log.detail("Startup.Awake() {0}", path);
			KSPe.Util.SystemTools.Assembly.AddSearchPath(path);
			try
			{
				this.LoadSupportForChutes();
				this.LoadSupportForEVA();
			}
			catch (Exception e)
			{
				Log.error(e, this);
				GUI.ShowStopperAlertBox.Show(e.Message);
			}
		}

		private void LoadSupportForChutes()
		{
			if (KSPe.Util.KSP.Version.Current >= KSPe.Util.KSP.Version.FindByVersion(1,4,0))
			{
				if (null != Type.GetType("RealChute.RealChuteModule, RealChute", false))
				{
					Log.info("Loading Chute Support for KSP >= 1.4 and Real Chutes");
					KSPe.Util.SystemTools.Assembly.LoadAndStartup("KourageousTourists.KSP.Chute.14.RealChute");
				}
				else
				{
					Log.info("Loading Chute Support for KSP 1.4 Stock");
					KSPe.Util.SystemTools.Assembly.LoadAndStartup("KourageousTourists.KSP.Chute.14");
				}
			}
			else if (KSPe.Util.KSP.Version.Current >= KSPe.Util.KSP.Version.FindByVersion(1,3,0))
			{
				if (null != Type.GetType("RealChute.RealChuteModule, RealChute", false))
				{
					Log.info("Loading Chute Support for KSP 1.3.x and Real Chutes");
					KSPe.Util.SystemTools.Assembly.LoadAndStartup("KourageousTourists.KSP.Chute.13.RealChute");
				}
				else throw new NotSupportedException("You need to install RealChutes on KSP 1.3 for playing Kourageous Tourists /L");
			}
			else throw new NotSupportedException("Your current KSP installment is not supported by Kourageous Tourists /L");
		}

		private void LoadSupportForEVA()
		{
			if (KSPe.Util.KSP.Version.Current >= KSPe.Util.KSP.Version.FindByVersion(1,6,0))
			{
				Log.info("Loading EVA Support for [KSP >= 1.6]");
				KSPe.Util.SystemTools.Assembly.LoadAndStartup("KourageousTourists.KSP.EVA.16");
			}
			else if (KSPe.Util.KSP.Version.Current >= KSPe.Util.KSP.Version.FindByVersion(1,3,0))
			{
				if (null != Type.GetType("KIS.KIS, KIS", false)) // check!
				{
					Log.info("Loading EVA Support for [1.3 <= KSP < 1.6] and KIS");
					KSPe.Util.SystemTools.Assembly.LoadAndStartup("KourageousTourists.KSP.EVA.13.KIS");
				}
				else
				{
					Log.info("Loading Chute Support for [1.3 <= KSP < 1.6] Stock");
					KSPe.Util.SystemTools.Assembly.LoadAndStartup("KourageousTourists.KSP.EVA.13");
				}
			}
			else throw new NotSupportedException("Your current KSP installment is not supported by Kourageous Tourists /L");
		}
	}
}

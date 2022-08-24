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
			Log.detail("Startup.Awake() {0}");
			using (KSPe.Util.SystemTools.Assembly.Loader<Startup> a = new KSPe.Util.SystemTools.Assembly.Loader<Startup>())
			{
				try
				{
					this.LoadSupportForChutes(a);
					this.LoadSupportForEVA(a);
				}
				catch (System.DllNotFoundException e)
				{
					Log.error(e.ToString());
					GUI.MissingDLLAlertBox.Show(e.Message);
				}
				catch (System.Exception e)
				{
					Log.error(e.ToString());
					GUI.ShowStopperAlertBox.Show(e.Message);
				}
			}
		}

		private void LoadSupportForChutes(KSPe.Util.SystemTools.Assembly.Loader<Startup> a)
		{
			if (KSPe.Util.KSP.Version.Current >= KSPe.Util.KSP.Version.FindByVersion(1,4,0))
			{
				if (KSPe.Util.SystemTools.TypeFinder.ExistsByQualifiedName("RealChute.RealChuteModule"))
				{
					Log.info("Loading Chute Support for KSP >= 1.4 and Real Chutes");
					a.LoadAndStartup("KourageousTourists.KSP.Chute.14.RealChute");
				}
				else
				{
					Log.info("Loading Chute Support for KSP 1.4 Stock");
					a.LoadAndStartup("KourageousTourists.KSP.Chute.14");
				}
			}
			else if (KSPe.Util.KSP.Version.Current >= KSPe.Util.KSP.Version.FindByVersion(1,3,0))
			{
				if (KSPe.Util.SystemTools.TypeFinder.ExistsByQualifiedName("RealChute.RealChuteModule"))
				{
					Log.info("Loading Chute Support for KSP 1.3.x and Real Chutes");
					a.LoadAndStartup("KourageousTourists.KSP.Chute.13.RealChute");
				}
				else throw new DllNotFoundException("You need to install RealChutes on KSP 1.3 for playing Kourageous Tourists /L");
			}
			else throw new NotSupportedException("Your current KSP installment is not supported by Kourageous Tourists /L");
		}

		private void LoadSupportForEVA(KSPe.Util.SystemTools.Assembly.Loader<Startup> a)
		{
			if (KSPe.Util.KSP.Version.Current >= KSPe.Util.KSP.Version.FindByVersion(1,6,0))
			{
				Log.info("Loading EVA Support for [KSP >= 1.6]");
				a.LoadAndStartup("KourageousTourists.KSP.EVA.16");
			}
			else if (KSPe.Util.KSP.Version.Current >= KSPe.Util.KSP.Version.FindByVersion(1,3,0))
			{
				if (null != Type.GetType("KIS.KIS, KIS", false)) // check!
				{
					Log.info("Loading EVA Support for [1.3 <= KSP < 1.6] and KIS");
					a.LoadAndStartup("KourageousTourists.KSP.EVA.13.KIS");
				}
				else
				{
					Log.info("Loading Chute Support for [1.3 <= KSP < 1.6] Stock");
					a.LoadAndStartup("KourageousTourists.KSP.EVA.13");
				}
			}
			else throw new NotSupportedException("Your current KSP installment is not supported by Kourageous Tourists /L");
		}
	}
}

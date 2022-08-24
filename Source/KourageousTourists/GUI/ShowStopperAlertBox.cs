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
using UnityEngine;

namespace KourageousTourists.GUI
{
	internal static class ShowStopperAlertBox
	{
		private static readonly string MSG = @"Unfortunately Kourageous Tourists /L will not run on your current installment!

The problem detected is that {0}.";

		private static readonly string AMSG = @"Install all the dependencies for your current installment.";

		internal static void Show(string reason)
		{
			KSPe.Common.Dialogs.ShowStopperAlertBox.Show(
				string.Format(MSG, reason),
				AMSG,
				() => { Application.Quit(); }
			);
			Log.detail("\"Houston, we have a Problem!\" was displayed about {0}", reason);
		}
	}
}
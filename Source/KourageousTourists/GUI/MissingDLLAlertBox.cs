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
﻿using System;
using UnityEngine;
using KSPe.UI;

namespace KourageousTourists.GUI
{
	internal static class MissingDLLAlertBox
	{
		private static readonly string MSG = @"Unfortunately Kourageous Tourists /L didn't found needed DLLs.

There's no safe way to proceed, without the support DLLs Kourageous Tourists /L will not work properly, and your craft files and savegames will be corrupted for sure!!

Missing Class: {0}";

		private static readonly string AMSG = @"reinstall Kourageous Tourists /L from a trusted Distribution Channel (KSP will close).";

		internal static void Show(string msg) {
			KSPe.Common.Dialogs.ShowStopperAlertBox.Show(
				string.Format(MSG, msg),
				AMSG,
				() => { Application.Quit(); }
			);
			Log.force("\"Houston, we have a Problem!\" about Missing DLLs was displayed");
		}
	}
}
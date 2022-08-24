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

using Contracts;

namespace KourageousTourists.Contracts
{
	public abstract class KourageousParameter : ContractParameter
	{
		protected CelestialBody targetBody;
		protected string tourist;

		public KourageousParameter() {
			this.targetBody = Planetarium.fetch.Home;
			this.tourist = "Unknown";
		}

		public KourageousParameter(CelestialBody target, String kerbal) {
			this.targetBody = target;
			this.tourist = String.Copy(kerbal);
		}

		protected override void OnLoad (ConfigNode node)
		{
			int bodyID = int.Parse(node.GetValue ("targetBody"));
			foreach (CelestialBody body in FlightGlobals.Bodies)
				if (body.flightGlobalsIndex == bodyID) {
					targetBody = body;
					break;
				}

			this.tourist = String.Copy(node.GetValue ("tourist"));
		}

		protected override void OnSave (ConfigNode node)
		{
			int bodyID = targetBody.flightGlobalsIndex;
			node.AddValue ("targetBody", bodyID);
			node.AddValue ("tourist", tourist);
		}
	}
}


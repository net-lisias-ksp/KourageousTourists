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
using System.Collections.Generic;

namespace KourageousTourists
{
	public class ProtoTourist
	{
		public int level { get; set; }
		public List<String> abilities { get; set; }
		public List<String> situations { get; set; }
		public List<String> celestialBodies { get; set; }
		public double srfspeed { get; set; }
		public bool isSkydiver { get; set; }

		public ProtoTourist ()
		{
			abilities = new List<String> ();
			situations = new List<String> ();
			celestialBodies = new List<String> ();
		}

		public override String ToString()
		{
			return (String.Format("Tourist: < lvl={0}, abilities: [{1}], situations: [{2}], bodies: [{3}], speed: {4:F2}, skydiver: {5} >",
				level, 
				String.Join(", ", abilities.ToArray()),
				String.Join(", ", situations.ToArray()),
				String.Join(", ", celestialBodies.ToArray()), 
				srfspeed, isSkydiver));
		}
	}
}


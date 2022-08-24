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
using System.Collections;

namespace KourageousTourists
{
	public class EVASupport
	{
		public interface Interface
		{
			bool isHelmetOn(Vessel v);
			void equipHelmet(Vessel v);
			void removeHelmet(Vessel v);
			void disableEvaEvents(Vessel v, bool isEvaEnabled);
			void disableEvaEvents(Part v, bool isEvaEnabled);
		}

		internal static readonly Interface INSTANCE;
		private static Interface GetInstance()
		{
			Log.dbg("Looking for {0}", typeof(Interface).Name);
			foreach(System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
				foreach(System.Type type in assembly.GetTypes())
					foreach(System.Type ifc in type.GetInterfaces() )
					{
						Log.dbg("Checking {0} {1} {2}", assembly, type, ifc);
						if ("KourageousTourists.EVASupport+Interface" == ifc.ToString())
						{
							Log.dbg("Found it! {0}", ifc);
							object r = System.Activator.CreateInstance(type);
							Log.dbg("Type of result {0}", r.GetType());
							return (Interface)r;
						}
					}
			Log.error("No realisation for the abstract Interface found! We are doomed!");
			return (Interface) null;
		}
		static EVASupport()
		{
			INSTANCE = GetInstance();
		}

	}
}

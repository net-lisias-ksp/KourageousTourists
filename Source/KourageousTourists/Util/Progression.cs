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
namespace KourageousTourists.Util
{
	public static class Progression
	{
		internal static bool IsComplete(CelestialBody targetBody, string accomplishment)
		{
			foreach (KSPAchievements.CelestialBodySubtree p in ProgressTracking.Instance.celestialBodyNodes)
			{
				if (targetBody.name != p.Body.name) continue;

				switch (accomplishment)
				{
					case "BaseConstruction":		return null != p.baseConstruction && p.baseConstruction.IsComplete;
					case "CrewTransfer":			return null != p.crewTransfer && p.crewTransfer.IsComplete;
					case "Docking":					return null != p.docking && p.docking.IsComplete;
					case "Escape":					return null != p.escape && p.escape.IsComplete;
					case "FlagPlant":				return null != p.flagPlant && p.flagPlant.IsComplete;
					case "Flight":					return CelestialBodies.Instance.IsHome(targetBody)
														|| (null != p.flight && p.flight.IsComplete)
														;
					case "Flyby":					return CelestialBodies.Instance.IsHome(targetBody)
														|| (null != p.flyBy && p.flyBy.IsComplete)
														;
					case "Landing":					return null != p.landing && p.landing.IsComplete;
					case "Orbit":					return null != p.orbit && p.orbit.IsComplete;
					case "Rendezvous":				return null != p.rendezvous && p.rendezvous.IsComplete;
					case "ReturnFromFlyBy":			return null != p.returnFromFlyby && p.returnFromFlyby.IsComplete;
					case "ReturnFromOrbit":			return null != p.returnFromOrbit && p.returnFromOrbit.IsComplete;
					case "ReturnFromSurface":		return null != p.returnFromSurface && p.returnFromSurface.IsComplete;
					case "Science":					return null != p.science && p.science.IsComplete;
					case "Spacewalk":				return null != p.spacewalk && p.spacewalk.IsComplete;
					case "SplashDown":				return null != p.splashdown && p.splashdown.IsComplete;
					case "StationConstruction":		return null != p.stationConstruction && p.stationConstruction.IsComplete;
					case "Suborbit":				return CelestialBodies.Instance.IsHome(targetBody)
														|| (null != p.suborbit && p.suborbit.IsComplete)
														;
					case "SurfaceEVA":				return null != p.surfaceEVA && p.surfaceEVA.IsComplete;

					default: return false;
				}
			}
			return false;
		}
	}
}

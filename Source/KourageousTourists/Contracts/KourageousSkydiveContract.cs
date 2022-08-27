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
using System.Collections.Generic;
using System.Linq;

namespace KourageousTourists.Contracts
{
	public class KourageousSkydiveContract : KourageousContract
	{
		public KourageousSkydiveContract() : base()
		{
			IgnoresWeight = true;
			this.maxTourists = 6;
		}

		protected override bool ConfigureContract()
		{
			base.ConfigureContract(); // Ignore the return
			this.achievementsRequired.Add("TowerBuzz");
			return !Settings.Instance.noSkyDiving;
;
		}

		protected override void GenerateTourist(ProtoCrewMember tourist)
		{
			KourageousSkydiveJumpParameter jumpParameter = new KourageousSkydiveJumpParameter(targetBody, tourist.name);
			jumpParameter.FundsCompletion = 0.0;
			jumpParameter.FundsFailure = 0.0;
			jumpParameter.ReputationCompletion = 0.0f;
			jumpParameter.ReputationFailure = 0.0f;
			jumpParameter.ScienceCompletion = 0.0f;
			AddParameter(jumpParameter);

			KourageousSkydiveLandParameter landParameter = new KourageousSkydiveLandParameter(targetBody, tourist.name);
			landParameter.FundsCompletion = 1000.0;
			landParameter.FundsFailure = 0.0;
			landParameter.ReputationCompletion = 0.0f;
			landParameter.ReputationFailure = 0.0f;
			landParameter.ScienceCompletion = 0.0f;
			AddParameter(landParameter);
		}

		protected override void GenerateContract()
			//System.Type contractType, Contract.ContractPrestige difficulty, int seed, State state)
		{
			this.SetExpiry();
			this.SetScience(0.0f, targetBody);
			this.SetDeadline(targetBody);
			this.SetReputation(2, 5, targetBody);
			this.SetFunds(500, 2000, 15000, targetBody);
		}

		protected override List<CelestialBody> getSelectableBodies()
		{
			List<CelestialBody> allBodies = getCelestialBodyList(true).Where(
					b => b.atmosphere)
					.ToList();

			Log.dbg("skydive bodies: {0}", String.Join(", ", allBodies.Select(b => b.ToString()).ToArray()));
			return allBodies;
		}

		protected override void OnAccepted() {
			Log.detail("entered: body= {0}", targetBody.bodyName);
			foreach (ProtoCrewMember tourist in tourists) {
				HighLogic.CurrentGame.CrewRoster.AddCrewMember (tourist);
				Log.detail("adding to roster: {0}", tourist.name);
			}
		}

		protected override string GetTitle () {
			return String.Format("Let {0} experience skydiving on {1}",
				getProperTouristWordLc(), targetBody.bodyName);
		}

		protected override string GetDescription() {
			return String.Format (
				"{0} just want to experience skydiving anywhere on {1}. Please note, that " +
				"for safety reasons air speed for jumping out should not exceed {2} m/s, " +
				"and altitude must be at least {3} m above the ground level. " +
				"The skydiver should land on a solid ground. {4}",
				getProperTouristWord (), targetBody.bodyName,
				Settings.Instance.paraglidingMaxAirspeed,
				Settings.Instance.paraglidingMinAltAGL,
				trainingHint(targetBody.bodyName, null, "EVA")
			);
		}

		protected override string GetSynopsys() {
			return String.Format (
				"Let {0} to jump out of the aircraft somewhere on {1}.",
				getProperTouristWordLc(), targetBody.bodyName
			);
		}

		protected override string MessageCompleted ()
		{
			return String.Format ("Skydiving charter for {0} was a success! This time your parachute riggers " +
				"made everything right.",
				getProperTouristWordLc()
			);
		}
	}


}


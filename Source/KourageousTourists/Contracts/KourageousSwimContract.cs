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

using FinePrint.Contracts.Parameters;

namespace KourageousTourists.Contracts
{
	public class KourageousSwimContract : KourageousContract
	{
		public KourageousSwimContract () : base () { }

		protected override bool ConfigureContract()
		{
			base.ConfigureContract(); // Ignore the return
			this.celestialBodyAccomplishmentsRequired.Add("SplashDown");
			this.celestialBodyAccomplishmentsRequired.Add("SurfaceEVA");
			this.celestialBodyAccomplishmentsRequired.Add("ReturnFromSurface");
			return true;
		}

		protected override void GenerateTourist(ProtoCrewMember tourist)
		{
			// TODO: Add support for gender for 1.3 build
			KerbalTourParameter itinerary = new KerbalTourParameter(tourist.name, tourist.gender);
			// TODO: Add difficulty multiplier
			itinerary.FundsCompletion = 25000.0;
			itinerary.ReputationCompletion = 0.0f;
			itinerary.ReputationFailure = 0.0f;
			itinerary.ScienceCompletion = 0.0f;
			this.AddParameter(itinerary);

			KerbalDestinationParameter dstParameter = new KerbalDestinationParameter(
				targetBody, FlightLog.EntryType.Land, tourist.name
			);
			dstParameter.FundsCompletion = 1000.0f;
			dstParameter.FundsFailure = 0.0f;
			dstParameter.ReputationCompletion = 0.0f;
			dstParameter.ReputationFailure = 0.0f;
			dstParameter.ScienceCompletion = 0.0f;
			/*dstParameter.NestToParent (itinerary);
			dstParameter.CreateID ();
			dstParameter.AddParameter (new Contracts.Parameters.LandOnBody (targetBody));*/
			itinerary.AddParameter(dstParameter);

			KourageousSwimParameter swimParameter = new KourageousSwimParameter(targetBody, tourist.name);
			swimParameter.FundsCompletion = 3000.0;
			swimParameter.FundsFailure = 0.0;
			swimParameter.ReputationCompletion = 0.0f;
			swimParameter.ReputationFailure = 0.0f;
			swimParameter.ScienceCompletion = 0.0f;
			itinerary.AddParameter(swimParameter);
		}

		protected override void GenerateContract()
			//System.Type contractType, Contract.ContractPrestige difficulty, int seed, State state)
		{
			this.SetExpiry();
			this.SetScience(0.0f, targetBody);
			this.SetDeadline(targetBody);
			this.SetReputation(2, 5, targetBody);
			this.SetFunds(2000, 7000, 18000, targetBody);
		}

		protected override List<CelestialBody> getSelectableBodies()
		{
			List<CelestialBody> allBodies = this.getCelestialBodyList(false).Where(
					b => b.ocean)
					.ToList();

			Log.dbg("swim bodies: {0}", String.Join(", ", allBodies.Select(b => b.ToString()).ToArray()));
			return allBodies;
		}

		protected override void OnAccepted() {
			Log.dbg("entered: KourageousSwimContract OnAccepted body={0}", targetBody.bodyName);
			foreach (ProtoCrewMember tourist in tourists) {
				HighLogic.CurrentGame.CrewRoster.AddCrewMember (tourist);
				Log.dbg("adding to roster: {0}", tourist.name);
			}
		}

		protected override string GetTitle () {
			return String.Format("Let {0} swim on the hydrosphere of {1}",
				this.getProperTouristWord(), targetBody.bodyName);
		}

		protected override string GenerateDescription()
		{
			return String.Format(
				"{0} want to get their feet wet by swimming on {1}. "
				+ "Ferry them there, make them walk the plank into the waters and return them safely (sound and dry). {2}"
				, this.getProperTouristWord()
				, targetBody.bodyName
				, trainingHint(targetBody.bodyName, "SPLASHED", "EVA")
			);
		}

		protected override string GetSynopsys() {
			return String.Format(
				"Ferry {0} to {1} and let them swim on the ocean.",
				this.getProperTouristWord(), targetBody.bodyName
			);
		}

		protected override string MessageCompleted ()
		{
			return String.Format("You have successfully returned {0} from the oceans of {1}. "
				+ "They are pretty impressed with the beauty and vastness of that seas and had nothing but good memories from the journey.",
				this.getProperTouristWord()
				, targetBody.bodyName
			);
		}
	}

}


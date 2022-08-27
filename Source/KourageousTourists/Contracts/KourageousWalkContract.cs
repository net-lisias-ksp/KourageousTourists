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
	public class KourageousWalkContract : KourageousContract
	{
		public KourageousWalkContract () : base () {}

		protected override bool ConfigureContract()
		{
			base.ConfigureContract(); // Ignore the return
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

			KourageousWalkParameter walkParameter = new KourageousWalkParameter(targetBody, tourist.name);
			walkParameter.FundsCompletion = 3000.0;
			walkParameter.FundsFailure = 0.0;
			walkParameter.ReputationCompletion = 0.0f;
			walkParameter.ReputationFailure = 0.0f;
			walkParameter.ScienceCompletion = 0.0f;
			itinerary.AddParameter(walkParameter);
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
			List<CelestialBody> allBodies = getCelestialBodyList(false).Where(
					b => b.hasSolidSurface)
					.ToList();

			Log.dbg("swim bodies: {0}", String.Join(", ", allBodies.Select(b => b.ToString()).ToArray()));
			return allBodies;
		}

		protected override void OnAccepted() {
			Log.dbg("entered: KourageousWalkContract OnAccepted body={0}", targetBody.bodyName);
			foreach (ProtoCrewMember tourist in tourists) {
				HighLogic.CurrentGame.CrewRoster.AddCrewMember (tourist);
				Log.dbg("adding to roster: {0}", tourist.name);
			}
		}

		protected override string GetTitle () {
			return String.Format("Let {0} walk on the surface of {1}",
				getProperTouristWordLc(), targetBody.bodyName);
		}

		protected override string GetDescription() {
			return String.Format (
				"{0} want to practice their moon-walk by performing a real {1}-walk. Ferry them " +
				"there, let them out and return safely. {2}", getProperTouristWord(), targetBody.bodyName,
				trainingHint(targetBody.bodyName, "LANDED", "EVA"));
		}

		protected override string GetSynopsys() {
			return String.Format (
				"Ferry {0} to {1} and let them walk on the surface.",
				getProperTouristWordLc(), targetBody.bodyName
			);
		}

		protected override string MessageCompleted ()
		{
			return String.Format ("You have successfully returned {0} from the surface of {1}. They are pretty " +
			"impressed and had nothing but good time and brought back lot of memories",
				getProperTouristWordLc(), targetBody.bodyName
			);
		}
	}

}


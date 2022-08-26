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
		public KourageousSwimContract () : base () {}

		protected override bool Generate()
			//System.Type contractType, Contract.ContractPrestige difficulty, int seed, State state)
		{
			Log.dbg("entered KourageousSwimContract Generate");

			targetBody = selectNextCelestialBody(this.getSelectableBodies());
			if (targetBody == null)
				return false;

			this.numTourists = UnityEngine.Random.Range (1, 5);
			Log.dbg("num tourists: {0}", numTourists);
			for (int i = 0; i < this.numTourists; i++) {
				ProtoCrewMember tourist = CrewGenerator.RandomCrewMemberPrototype(ProtoCrewMember.KerbalType.Tourist);

				tourists.Add (tourist);
				Log.dbg("generated: {0}", tourist.name);

				// TODO: Add support for gender for 1.3 build
				KerbalTourParameter itinerary = new KerbalTourParameter(tourist.name, tourist.gender);
				// TODO: Add difficulty multiplier
				itinerary.FundsCompletion = 25000.0;
				itinerary.ReputationCompletion = 0.0f;
				itinerary.ReputationFailure = 0.0f;
				itinerary.ScienceCompletion = 0.0f;
				this.AddParameter (itinerary);

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
				itinerary.AddParameter (dstParameter);

				KourageousSwimParameter swimParameter = new KourageousSwimParameter(targetBody, tourist.name);
				swimParameter.FundsCompletion = 3000.0;
				swimParameter.FundsFailure = 0.0;
				swimParameter.ReputationCompletion = 0.0f;
				swimParameter.ReputationFailure = 0.0f;
				swimParameter.ScienceCompletion = 0.0f;
				itinerary.AddParameter (swimParameter);
			}

			GenerateHashString ();

			base.SetExpiry();
			base.SetScience(0.0f, targetBody);
			this.SetDeadline(targetBody);
			base.SetReputation(2, 5, targetBody);
			base.SetFunds(2000, 7000, 18000, targetBody);

			return true;
		}

		protected override List<CelestialBody> getSelectableBodies()
		{
			List<CelestialBody> allBodies = this.getCelestialBodyList(true).Where(
					b => b.ocean)
					.ToList();
			allBodies.Add(Planetarium.fetch.Home);

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

		protected override void GenerateHashString() {
			string hash = "swimcntrct-" + targetBody.bodyName;
			foreach (ProtoCrewMember tourist in this.tourists)
				hash += tourist.name;
			this.hashString = hash;
		}

		protected override string GetTitle () {
			return String.Format("Let {0} swim on the hydrosphere of {1}",
				getProperTouristWordLc(), targetBody.bodyName);
		}

		protected override string GetDescription() {
			return String.Format(
				"{0} want to get their feet wet by swimming on {1}. "
				+ "Ferry them there, make them walk the plank into the waters and return them safely (sound and dry). {2}"
				, getProperTouristWord()
				, targetBody.bodyName
				, trainingHint(targetBody.bodyName, "SPLASHED", "EVA")
			);
		}

		protected override string GetSynopsys() {
			return String.Format(
				"Ferry {0} to {1} and let them swim on the ocean.",
				getProperTouristWordLc(), targetBody.bodyName
			);
		}

		protected override string MessageCompleted ()
		{
			return String.Format("You have successfully returned {0} from the oceans of {1}. "
				+ "They are pretty impressed with the beauty and vastness of that seas and had nothing but good memories from the journey.",
				getProperTouristWordLc()
				, targetBody.bodyName
			);
		}
	}

}


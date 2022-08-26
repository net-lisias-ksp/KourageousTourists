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

	public class KourageousAnomaly
	{
		public string name { get; set; }
		public string anomalyDescription { get; set; }
		public CelestialBody body { get; set; }
		public string contractDescription { get; set; }
		public string contractSynopsis { get; set; }
		public string contractCompletion { get; set; }
		public float payoutModifier;

		public void Save(ConfigNode node) {
			node.AddValue ("anomaly", body.name + ":" + name);
		}

		public static KourageousAnomaly Load(ConfigNode node, Dictionary<String, KourageousAnomaly> anomalies) {
			string anomalyName = node.GetValue ("anomaly");
			return (KourageousAnomaly)anomalies [anomalyName].MemberwiseClone ();
		}
	}

	public class KourageousAnomalyContract : KourageousContract
	{
		public const string cfgNode = "ANOMALY";

		private static KourageousAnomalyContract instance = null;
		internal static KourageousAnomalyContract Instance = instance ?? (instance = new KourageousAnomalyContract());

		internal readonly Dictionary<String, KourageousAnomaly> anomalies = new Dictionary<String, KourageousAnomaly>();
		protected KourageousAnomaly chosenAnomaly;
		protected static float anomalyDiscoveryDistance = 50.0f;

		public KourageousAnomalyContract () : base ()
		{
			this.readAnomalyConfig();
		}

		public static void Reload()
		{
			instance = null;
		}

		private void reReadAnomalyConfig()
		{
			this.anomalies.Clear();
			this.readAnomalyConfig();
		}

		private void readAnomalyConfig()
		{
			ConfigNode config = GameDatabase.Instance.GetConfigNodes(KourageousContract.cfgRoot).FirstOrDefault();
			if (config == null)
				return;

			Util.PQS pqs = new Util.PQS();

			{ 
				String distanceNode = config.GetValue("anomalyDiscoveryDistance");
				if (distanceNode != null) {
					try {
						anomalyDiscoveryDistance = (float)Convert.ToDouble(distanceNode);
					}
					catch(Exception e) {
						Log.error(e, "KourageousAnomalyContract readAnomalyConfig");
					}
				}
			}

			ConfigNode[] nodes = config.GetNodes (cfgNode);
			foreach (ConfigNode node in nodes)
			{
				Log.dbg("cfg node: {0}", node);

				KourageousAnomaly anomaly = new KourageousAnomaly ();

				{ 
					String name = node.GetValue("name");
					if (name == null)
						continue;
					anomaly.name = name;
				}
				Log.dbg("anomaly name: {0}", anomaly.name);

				{ 
					String bodyStr = node.GetValue ("body");
					Log.dbg("anomaly body: {0}", bodyStr);

					foreach (CelestialBody b in FlightGlobals.Bodies) {
						Log.dbg("list body name: {0}", b.name);
						if (b.name.Equals (bodyStr)) {
							anomaly.body = b;
							break;
						}
					}
				}
				Log.dbg("anomaly body obj: {0}", anomaly.body);

				if (anomaly.body == null) continue;
				if(!pqs.exists(anomaly.body, anomaly.name))	continue;

				{ 
					String anomalyDescription = node.GetValue ("anomalyDescription");
					if (anomalyDescription == null) continue;
					anomaly.anomalyDescription = anomalyDescription;
				}
				{ 
					String contractDescription = node.GetValue ("contractDescription");
					if (contractDescription == null) continue;
					anomaly.contractDescription = contractDescription;
				}
				{
					String contractSynopsis = node.GetValue ("contractSynopsis");
					if (contractSynopsis == null)
						continue;
					anomaly.contractSynopsis = contractSynopsis;
				}
				{ 
					String payoutModifierStr = node.GetValue ("payoutModifier");
					Log.dbg("payout modifier str: {0}", payoutModifierStr);
					if (payoutModifierStr == null)
						continue;
					float payoutModifier = 1.0f;
					try {
						payoutModifier = (float)Convert.ToDouble(payoutModifierStr);
						Log.dbg("payout modifier: {0}", payoutModifier);
					}
					catch(Exception e) {
						Log.error(e, "readAnomalyConfig");
					}
					anomaly.payoutModifier = payoutModifier;
				}
				anomalies.Add(anomaly.body + ":" + anomaly.name, anomaly);
				Log.dbg("added: {0}", anomaly.body + ":" + anomaly.name);
			}
		}

		protected KourageousAnomaly chooseAnomaly(CelestialBody body) {

			Log.dbg("entered KourageousAnomallyContract chooseAnomaly");
			reReadAnomalyConfig ();
			Log.dbg("anomalies: {0}, distance: {1}", anomalies.Count, anomalyDiscoveryDistance);

			List<KourageousAnomaly> chosen = new List<KourageousAnomaly> ();
			foreach (KeyValuePair<string, KourageousAnomaly> entry in anomalies)
				if (entry.Value.body.name.Equals (body.name))
					chosen.Add (entry.Value);

			Log.dbg("chosen: {0}, cnt: {1}", chosen, chosen.Count);
			if (chosen.Count == 0)
				return null;

			Random rnd = new Random ();
			return chosen [rnd.Next (chosen.Count)];
		}

		protected override bool Generate()
			//System.Type contractType, Contract.ContractPrestige difficulty, int seed, State state)
		{
			Log.dbg("entered KourageousAnomallyContract Generate");

			targetBody = this.selectNextCelestialBody(this.getSelectableBodies());
			if (targetBody == null)
				return false;

			chosenAnomaly = chooseAnomaly (targetBody);
			if (chosenAnomaly == null)
				return false;

			this.difficultyMultiplier = chosenAnomaly.payoutModifier;
			this.numTourists = UnityEngine.Random.Range (2, 5);
			Log.dbg("num tourists: {0}", numTourists);
			for (int i = 0; i < this.numTourists; i++) {
				ProtoCrewMember tourist = CrewGenerator.RandomCrewMemberPrototype (ProtoCrewMember.KerbalType.Tourist);

				this.tourists.Add (tourist);
				Log.dbg("generated: {0}", tourist.name);

				// TODO: Add support for gender for 1.3 build
				KerbalTourParameter itinerary = new KerbalTourParameter (tourist.name, tourist.gender);
				// TODO: Add difficulty multiplier
				itinerary.FundsCompletion = 25000.0;
				itinerary.ReputationCompletion = 0.0f;
				itinerary.ReputationFailure = 0.0f;
				itinerary.ScienceCompletion = 0.0f;
				this.AddParameter (itinerary);

				KerbalDestinationParameter dstParameter = new KerbalDestinationParameter (
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


				KourageousAnomalyParameter anomalyParameter = new KourageousAnomalyParameter (
					targetBody, tourist.name, chosenAnomaly.name, chosenAnomaly.anomalyDescription
				);
				anomalyParameter.FundsCompletion = 1300.0;
				anomalyParameter.FundsFailure = 0.0;
				anomalyParameter.ReputationCompletion = 1.0f;
				anomalyParameter.ReputationFailure = 1.0f;
				anomalyParameter.ScienceCompletion = 0.0f;
				itinerary.AddParameter (anomalyParameter);
			}

			GenerateHashString ();

			base.SetExpiry ();
			base.SetScience (0.0f, targetBody);
			this.SetDeadline(targetBody);
			base.SetReputation (2, 5, targetBody);
			base.SetFunds (
				3000,
				9000,
				21000,
				targetBody);

			return true;
		}

		protected override void OnAccepted() {
			Log.dbg("entered: KourageousAnomallyContract Generate body={0}", targetBody.bodyName);
			foreach (ProtoCrewMember tourist in tourists) {
				HighLogic.CurrentGame.CrewRoster.AddCrewMember (tourist);
				Log.dbg("adding to roster: {0}", tourist.name);
			}
		}

		protected override void GenerateHashString() {
			string hash = "selfiecntrct-" + targetBody.bodyName;
			foreach (ProtoCrewMember tourist in this.tourists)
				hash += tourist.name;
			this.hashString = hash;
		}

		protected override string GetTitle () {
			Log.dbg("entered: KourageousAnomallyContract GetTitle anomaly={0}", chosenAnomaly);
			return String.Format("Visit {0} with {1}",
				chosenAnomaly.anomalyDescription,  getProperTouristWordLc());
		}

		protected override string GetDescription() {
			return KourageousContract.tokenize (
				chosenAnomaly.contractDescription, getProperTouristWord(), anomalyDiscoveryDistance,
				trainingHint(chosenAnomaly.body.bodyName, "LANDED", "EVA", true));
		}

		protected override string GetSynopsys() {
			return KourageousContract.tokenize (
				chosenAnomaly.contractSynopsis, getProperTouristWordLc(), anomalyDiscoveryDistance);
		}

		protected override string MessageCompleted ()
		{
			return KourageousContract.tokenize (chosenAnomaly.contractCompletion,
				getProperTouristWordLc (), anomalyDiscoveryDistance);
		}

		protected override void OnSave (ConfigNode node) {
			base.OnSave (node);
			chosenAnomaly.Save(node);
		}

		protected override void OnLoad(ConfigNode node) {
			base.OnLoad (node);
			reReadAnomalyConfig ();
			chosenAnomaly = KourageousAnomaly.Load (node, anomalies);
		}
	}
}


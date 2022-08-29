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

using KourageousTourists.Util;

using KSPe;

namespace KourageousTourists.Contracts
{

	public class KourageousAnomaly
	{
		public string name { get; internal set; }
		public string anomalyDescription { get; internal set; }
		public CelestialBody body { get; internal set; }
		public string contractDescription { get; internal set; }
		public string contractSynopsis { get; internal set; }
		public string contractCompletion { get; internal set; }
		public float payoutModifier { get; internal set; }
		public bool reputationBonus { get; internal set; }
		public float anomalyDiscoveryDistance { get; internal set; }
		public string touristSituation { get; internal set; }
		public string touristAbility { get; internal set; }
		public string[] achievementsRequired { get; internal set; }
		public string poi { get; internal set; } 

		public void Save(ConfigNode node) {
			string bodyName = node.GetValue("body");
			node.AddValue("anomalyBody", this.body.name);
			node.AddValue("anomalyName", this.name);
		}

		public static KourageousAnomaly Load(ConfigNode node) {
			if (node.HasValue("anomaly"))
			{	// Try to salvage a savegame with older versions of KT!
				string name = node.GetValue("anomaly");
				string[] names = name.Split(':');
				return (KourageousAnomaly)Database.Instance[names[0], names[1]].MemberwiseClone();
			}

			string bodyName = node.GetValue("anomalyBody");
			string anomalyName = node.GetValue("anomalyName");
			return (KourageousAnomaly)Database.Instance[bodyName, anomalyName].MemberwiseClone();
		}

		public override string ToString()
			=> string.Format("{1} in {0}", this.body.name, this.name);
	}

	internal class Database
	{
		private static Database instance = null;
		internal static Database Instance = instance ?? (instance = new Database());

		private readonly Dictionary<String, KourageousAnomaly> anomalies = new Dictionary<String, KourageousAnomaly>();
		public float anomalyDiscoveryDistance	{ get; private set; } = 50.0f;
		public string achievementsRequiredDef	{ get; private set; } = "";
		public string touristSituation			{ get; private set; } = "LANDED";
		public string touristAbility			{ get; private set; } = "EVA";

		private Database() => this.readAnomalyConfig();
		~Database() => instance = null;

		public KourageousAnomaly this[string targetBodyName, string anomalyName]
			=> this.anomalies[CelestialBodies.Instance[targetBodyName] + ":" + anomalyName];

		public KourageousAnomaly this[CelestialBody targetBody, string anomalyName]
			=> this.anomalies[targetBody.name + ":" + anomalyName];

		private static readonly Random RND = new Random ();
		public KourageousAnomaly chooseAnomaly(CelestialBody body)
		{
			Log.dbg("entered KourageousAnomallyContract chooseAnomaly");
			Log.dbg("anomalies: {0}, distance: {1}", anomalies.Count, anomalyDiscoveryDistance);

			List<KourageousAnomaly> chosen = new List<KourageousAnomaly> ();
			foreach (KeyValuePair<string, KourageousAnomaly> entry in anomalies)
				if (entry.Value.body.name.Equals (body.name))
					chosen.Add (entry.Value);

			Log.dbg("chosen: {0}, cnt: {1}", chosen, chosen.Count);
			if (chosen.Count == 0)
				return null;

			return chosen[RND.Next (chosen.Count)];
		}

		private void readAnomalyConfig()
		{
			ConfigNode configRoot = GameDatabase.Instance.GetConfigNodes(KourageousContract.cfgRoot).FirstOrDefault();
			if (null == configRoot) return;

			ConfigNodeWithSteroids config = ConfigNodeWithSteroids.from(configRoot);
			Util.PQS pqs = Util.PQS.Instance;

			this.anomalyDiscoveryDistance = config.GetValue<float>("anomalyDiscoveryDistance", this.anomalyDiscoveryDistance);
			this.achievementsRequiredDef = config.GetValue<string>("achievementsRequired", this.achievementsRequiredDef);

			ConfigNode[] nodes = config.GetNodes(KourageousAnomalyContract.cfgNode);
			foreach (ConfigNode configNode in nodes)
			{
				Log.dbg("cfg node: {0}", configNode);
				ConfigNodeWithSteroids node = ConfigNodeWithSteroids.from(configNode);

				KourageousAnomaly anomaly = new KourageousAnomaly ();

				{
					if (!node.HasValue("name")) continue;
					anomaly.name = node.GetValue<string>("name");
				}
				Log.dbg("anomaly name: {0}", anomaly.name);

				{ 
					if (!node.HasValue("body")) continue;
					String body = node.GetValue<string>("body");
					Log.dbg("anomaly body: {0}", body);
					if (!Util.CelestialBodies.Instance.Exists(body)) continue;
					anomaly.body = Util.CelestialBodies.Instance[body].celestialBody;
				}
				Log.dbg("anomaly body obj: {0}", anomaly.body);

				if(!pqs.Exists(anomaly.body, anomaly.name))	continue;

				{ 
					if (!node.HasValue("anomalyDescription")) continue;
					anomaly.anomalyDescription = node.GetValue<string>("anomalyDescription");
				}
				{ 
					if (!node.HasValue("contractDescription")) continue;
					anomaly.contractDescription = node.GetValue<string>("contractDescription");
				}
				{
					if (!node.HasValue("contractSynopsis")) continue;
					anomaly.contractSynopsis = node.GetValue<string>("contractSynopsis");
				}

				anomaly.payoutModifier = node.GetValue<float>("payoutModifier", 1.0f);
				Log.dbg("payout modifier: {0}", anomaly.payoutModifier);

				anomaly.reputationBonus = node.GetValue<bool>("reputationBonus", false);
				Log.dbg("anomaly reputationBonus: {0}", anomaly.reputationBonus);

				anomaly.anomalyDiscoveryDistance = node.GetValue<float>("anomalyDiscoveryDistance", anomalyDiscoveryDistance);
				Log.dbg("anomaly anomalyDiscoveryDistance: {0}", anomaly.anomalyDiscoveryDistance);

				anomaly.touristSituation = node.GetValue<string>("touristSituation", this.touristSituation);
				Log.dbg("anomaly touristSituation: {0}", anomaly.touristSituation);

				anomaly.touristAbility = node.GetValue<string>("touristSituation", this.touristAbility);
				Log.dbg("anomaly touristAbility: {0}", anomaly.touristSituation);

				{ 
					string achievementsRequired = node.GetValue<string>("achievementsRequired", this.achievementsRequiredDef);
					anomaly.achievementsRequired = achievementsRequired.Split(',');
				}
				Log.dbg("anomaly achievementsRequired: {0}", anomaly.achievementsRequired);

				anomaly.poi = node.GetValue<string>("poi", "");
				Log.dbg("anomaly poi: {0}", anomaly.poi);

				anomalies.Add(anomaly.body.name + ":" + anomaly.name, anomaly);
				Log.dbg("added: {0}", anomaly.body.name + ":" + anomaly.name);
			}
		}

    }

	public class KourageousAnomalyContract : KourageousContract
	{
		public const string cfgNode = "ANOMALY";

		protected KourageousAnomaly chosenAnomaly;

		public KourageousAnomalyContract () : base()
		{
			this.minTourists = 2;
		}

		protected override bool ConfigureContract()
		{
			base.ConfigureContract(); // Ignore the return
			this.achievementsRequired.Add("PointOfInterest");

			chosenAnomaly = Database.Instance.chooseAnomaly(targetBody);
			if (chosenAnomaly == null) return false;

			this.achievementsRequired.UnionWith(chosenAnomaly.achievementsRequired);
			this.achievementsRequired.Add(chosenAnomaly.poi);
			this.difficultyMultiplier = chosenAnomaly.payoutModifier;

			return this.MeetRequirements();	// Check it again to reflect the anomaly's requirements.
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

			KourageousAnomalyParameter anomalyParameter = new KourageousAnomalyParameter(
				targetBody, tourist.name, chosenAnomaly.name, chosenAnomaly.anomalyDescription
			);
			anomalyParameter.FundsCompletion = 1300.0;
			anomalyParameter.FundsFailure = 0.0;
			anomalyParameter.ReputationCompletion = 1.0f;
			anomalyParameter.ReputationFailure = 1.0f;
			anomalyParameter.ScienceCompletion = 0.0f;
			itinerary.AddParameter(anomalyParameter);
		}

		protected override void GenerateContract()
			//System.Type contractType, Contract.ContractPrestige difficulty, int seed, State state)
		{
			this.SetExpiry();
			this.SetScience(0.0f, targetBody);
			this.SetDeadline(targetBody);
			this.SetReputation(
				(this.chosenAnomaly.reputationBonus ? (float)(this.numTourists / 2.5) : 2)
				, (this.chosenAnomaly.reputationBonus ? this.numTourists / 2 : 5)
				, targetBody);
			this.SetFunds(
				3000,
				9000,
				21000,
				targetBody);
		}

		protected override List<CelestialBody> getSelectableBodies() => this.getCelestialBodyList(true);

		protected override void OnAccepted() {
			Log.dbg("entered: KourageousAnomallyContract Generate body={0}", targetBody.bodyName);
			foreach (ProtoCrewMember tourist in tourists) {
				HighLogic.CurrentGame.CrewRoster.AddCrewMember (tourist);
				Log.dbg("adding to roster: {0}", tourist.name);
			}
		}

		protected override string GetTitle () {
			Log.dbg("entered: KourageousAnomallyContract GetTitle anomaly={0}", this.chosenAnomaly);
			return String.Format("Visit {0} with {1}",
				this.chosenAnomaly.anomalyDescription,  getProperTouristWordLc());
		}

		protected override string GetDescription() {
			return KourageousContract.tokenize (
				chosenAnomaly.contractDescription, getProperTouristWord(), chosenAnomaly.anomalyDiscoveryDistance,
				trainingHint(chosenAnomaly.body.bodyName, chosenAnomaly.touristSituation, chosenAnomaly.touristAbility, true));
		}

		protected override string GetSynopsys() {
			return KourageousContract.tokenize (
				chosenAnomaly.contractSynopsis, getProperTouristWordLc(), this.chosenAnomaly.anomalyDiscoveryDistance);
		}

		protected override string MessageCompleted ()
		{
			return KourageousContract.tokenize (chosenAnomaly.contractCompletion,
				getProperTouristWordLc (), this.chosenAnomaly.anomalyDiscoveryDistance);
		}

		protected override void OnSave (ConfigNode node) {
			base.OnSave (node);
			chosenAnomaly.Save(node);
		}

		protected override void OnLoad(ConfigNode node) {
			base.OnLoad (node);
			this.chosenAnomaly = KourageousAnomaly.Load(node);
		}
	}
}


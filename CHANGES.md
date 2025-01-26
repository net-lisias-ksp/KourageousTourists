# Kourageous Tourists /L :: Changes

* 2025-0125: 0.6.0.2 (Lisias) for KSP >= 1.3
	+ Updating libraries.
* 2022-1128: 0.6.0.1 (Lisias) for KSP >= 1.3
	+ Replaces KSPe external dependency with embedded KSPe.Light
	+ Kourageous Tourists /L goes gold!  **#HURRAY!!**
* 2022-0829: 0.6.0.0 (Lisias) for KSP >= 1.3 **BETA**
	+ First release under Lisias' stewardship! **#HURRAY!!**
	+ Enhancements since the last official release:
		- Full support from KSP 1.3.1 to the latest. #HONEST!!
			- Support for RealChutes
			- For KSP < 1.6, KIS is needed to allow removing Helmets.
		- Added a new ability: SkyDive
		- Rebalance of the abilities for each Tourist Level
		- Allowing Tourists to remove the helmet when they can EVA on KSP >= 1.6
		- ***MOAR CONTRACTS***
			- Swimming contracts for Bodies with Ocean
			- And lots of Anomalies added to Contracts. (more to come)
			- No Spoilers!!
				- Anomaly Contracts are only issued **after** you scan then on KerbNet!
			- No-nonsense requirements
				- You will not be offered a Contract to a Celestial Body unless you had gone there (and came back in one piece!) 
				- You must had landed there in order to get a "Walk" contract.
				- You must had splashed there in order to get a "Swim" contract.
			- Rep Farming Contracts
				- Low payment, heavy work contracts for recovering your reputation!
					- if you are lucky to get them!
			- Configurable requirements (Points of Interest and Celestial Body Accomplishments).
	+ Bug fixes:
		- Memory Leaks removed!
		- Mitigating (not properly fixing, however) an issue where the Selfies where being taken too soon and not registering the Kerbal's emotions.
		- Surviving KSP's Upgrade(Update?) Pipeline
			- Tourists restrictions are not lost anymore on loading savegames
		- Tourists on External Command Seats (or similar parts) are now correctly handled.
			- Including the ability to take selfies. :) 
		- Contracts for absent Anomalies are not issued anymore
			- Planet Packs (as JNSQ) that remove/change the Anomalies are not a problem anymore
				- Contracts tailored for Planet Packs are now possible!  
	+ Known Issues:
		- SkyDivers will *poof* if they go beyound the Physics Range of the focused vessel. [#3](https://github.com/net-lisias-ksp/KourageousTourists/issues/3)
		- The Contract payments don't scale with the difficulty, they are a flat rate. [#4](https://github.com/net-lisias-ksp/KourageousTourists/issues/4)
		- Support for EVA Fuel/Enhancements/Follower are temporarily disabled. [#8](https://github.com/net-lisias-ksp/KourageousTourists/issues/8)
	+ Closes issues:
		- [#2](https://github.com/net-lisias-ksp/KourageousTourists/issues/2) Prevent creating Contracts for absent Anomalies

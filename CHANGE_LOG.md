# Kourageous Tourists /L :: Change Log

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
* 2022-0627: 0.5.3.1 (Lisias) for KSP >= 1.3
	+ Finally implementing full support for Real Chutes.
	+ Fixing some mistakes on handling savegames and Module Manager 
* 2021-1105: 0.5.3.0 (Lisias) for KSP >= 1.3
	+ Updating the code to the new KSPe v2.4 facilites
	+ Moving the Settings to the current game's `saves` folder, allowing customisations per savegame.
* 2021-0220: 0.5.2.2 (Lisias) for KSP >= 1.3
	+ Add Support for every KSP Version since 1.3
		- Support for RealChutes is WorkInProgress, but you can try it nevertheless on KSP 1.3... ]:->
		- For KSP < 1.6, KIS is needed to allow removing Helmets.
	+ A lot of bug fixes
		- Allowing Tourists to remove the helmet when they can EVA as it was allowed using KIS on KSP < 1.6 
		- Surviving KSP's Upgrade(Update?) Pipeline
			- Tourists restrictions are not lost anymore on loading savegames
		- Tourists on External Command Seats (or similar parts) are now correctly handled.
			- Including the ability to take selfies. :) 
		- Trying to synchronise the Kerbal's reactions to the Selfie's shuttle.
		- Pull Requests to the upstream.
			- [#3](https://github.com/whale2/KourageousTourists/pull/3) 
			- [#4](https://github.com/whale2/KourageousTourists/pull/4)
			- [#6](https://github.com/whale2/KourageousTourists/pull/6)
	+ Merging upstream changes and fixes:
		- 0.5.2
			- Sky Diving Contracts only available after buzzing the tower
			- new config options: noSkyDiving & forceTouristsInSandbox
		- 0.5.1
			- bug fixes
		- 0.5.0
			- Sky Diving contracts.
	+ **DEVIATIONS** from the Mainstream
		- Added a new ability: SkyDive
		- Changes on the abilities for each Tourist Level
		- Moving the Kourage.cfg to the `PluginData` folder on the user's installment
			- Automatically created from template at first run.
		- Check [template](https://github.com/net-lisias-kspu/KourageousTourists/blob/master/GameData/net.lisias.ksp/KourageousTourists/PluginData/Kourage.cfg) for changes.
* 2020-1025: 0.5.2.1 (Lisias) for KSP >= 1.3
	+ ***WITHDRAWN*** due lame mistake on packaging 
* 2020-0530: 0.5.2 (whale_2) for KSP 1.9.1
	+ This release fixes some minor bugs, but also makes sky diving contracts unlock only after you've buzzed the tower.
	+ Added two config options:
	+ noSkyDiving - to completely disable skydiving contracts
	+ forceTouristsInSandbox - for enabling the mod in sandbox mode (normally disabled)
* 2020-0528: 0.5.1 (whale_2) for KSP 1.9.1
	+ Fix stupid bug when everyone has left the vessel and contract conditions should be checked.
* 2020-0524: 0.5.0 (whale_2) for KSP 1.9.1
	+ Update for latest KSP and SkyDiving contracts!
	+ That's it - the mod is updated for recent KSP versions (1.7 - 1.9) and now you can enjoy throwing kerbals out of your aircraft or helicopter or even a rocket. Parachute will be deployed automatically. However, you should keep them in the vicinity, otherwise the game won't let them land safely.
* 2020-0116: 0.4.1.1 (Lisias) for KSP >= 1.4
	+ Replicating [fix](https://github.com/whale2/KourageousTourists/pull/2) from [takoss](https://github.com/takoss)
		- Just found it after fixing it myself, but that guy detected and fixed it before me, so I think he deserved be mentioned! :)
	+ Adding KSPe facilities
		- Logging
		- Abstract file system
		- Installmment checks
	+ Changing the debug mode to read the setting from `<PLUGINDATA>/Debug.cfg`
		- It's a deviation from the mainstream. Beware. 
	+ Mitigating (not properly fixing, however) an issue where the Selfies where being taken too soon and not registering the Kerbal's emotions.
		- [Pull request](https://github.com/whale2/KourageousTourists/pull/3) applied to upstream.
	+ Moving the whole thing to `net-lisias-ksp/KourageousTourists` "vendor hierarchy" to prevent clashes to the upstream.
* 2018-0401: 0.4.1 (whale2) for KSP ['1.3.1', '1.3']
	+ Bugfix release.
	+ Fixed stupid bug when contracts was not displayed properly in Mission Control
	+ Fixed situation when everyone left the vessel for excursion.
* 2017-1102: 0.4.0 (whale2) for KSP ['1.3.1', '1.3']
	+ Hotfix release
	+ Bugfixes:
		- Fixed ugly bug in FixedUpdate that was spamming log file.
		- Fixed UFO being on Kerbin instead of Mun
	+ [KourageousTourists-0.4.0.zip](https://github.com/whale2/KourageousTourists/files/1439604/KourageousTourists-0.4.0.zip)
* 2017-1006: 0.3 (whale2) for KSP 1.3 PRE-RELEASE
	+ Release Candidate.
	+ Features:
	+ New contract - some tourists want to get a picture of themselves in front of some unusual objects.
* 2017-0814: 0.2 (whale2) for KSP 1.3 PRE-RELEASE
	+ This is still pre-release, however most things should work.
	+ Fixes:
		- Fixed contract persistence between saves and loads
	+ Features:
		- New contract - some tourists want to get a picture of themselves in some extrakerbestrial environment.
* 2017-0809: 0.1 (whale2) for KSP 1.3 PRE-RELEASE
	+ This is the first pre-release, be warned.
	+ Features:
		- Tourists are granted EVA permission in different situations based on their level
		- When tourist goes on EVA, Jetpack fuel is drained out on lower levels
		- Tourists can take photos; when photo is taken, all kerbals in the scene are likely to express some emotions. Photos can be found in standard 'Screenshot' folder.
		- One contract type is available - it requires to let some tourists to set foot on celestial bodies you have visited
	+ Compatibility:
	+ The mod was tested and found to be compatible with:
		- EVA Follower
		- EVA Enhancements
	+ Issues:
		- EVA Fuel; At the moment fuel, transferred from the vessel, a tourists goes on EVA from, will be lost.
	+ Patch for EVA Fuel was discussed with its current maintainer and PR was sent.
op

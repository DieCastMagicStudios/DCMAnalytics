
/* 
 * Copyright 2017 Die-Cast Magic Studios, LLC.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DieCastMagic.AnalyticsIntegration {
	
	/// <summary>
	/// A class containing logic for interfacing Iris Burning's gameplay with
	/// the telemetry backend provided by GameAnalytics.
	/// </summary>
	public sealed class DCMAnalytics {
		
		/// <summary>
		/// The PlayerPrefs key string used to define a player's opt-in
		/// preference for telemetry.
		/// </summary>
		private static readonly string OPT_IN_PLAYERPREFS = "AnalyticsOptIn";
		
		/// <summary>
		/// The GameAnalytics namespace string used for defining multiplayer events.
		/// </summary>
		private static readonly string MODE_MULTIPLAYER = "multiplayer";
		
		/// <summary>
		/// The GameAnalytics namespace string used for defining singleplayer events.
		/// </summary>
		private static readonly string MODE_SINGLEPLAYER = "singleplayer";
		
		/// <summary>
		/// An internal variable used for determining if the player has opted 
		/// in to telemetry or not.
		/// </summary>
		private static bool playerOptedIn = true;
		
		/// <summary>
		/// An internal variable used to determine if Init() has been called
		/// for this session.
		/// </summary>
		private static bool doneInit = false;
		
		/// <summary>
		/// An internal toggle used to enable/disable debug printouts to the
		/// developer console.
		/// </summary>
		private static bool debugEvents = false;
		
		/// <summary>
		/// An internal holding of the current Favor required to meet the win
		/// condition, set automatically in DCMAnalytics.Init().
		/// </summary>
		private static int winCond = 5000;
		
		/// <summary>
		/// An internal list of telemetry events that have occurred since the
		/// last DCMAnalytics.Init() call.
		/// </summary>
		private static List<string> eventLog = new List<string>();
		
		/// <summary>
		/// A property used for defining whether or not a player has opted in or
		/// not.  If Init() has not been called, this always returns false. Also
		/// automatically sets the needed PlayerPrefs key to the opt-in value.
		/// </summary>
		public static bool OptIn {
			get {
				if (!doneInit) return false;
				return playerOptedIn;
			}
			
			set {
				PlayerPrefs.SetInt(OPT_IN_PLAYERPREFS, BoolToInt(true));
				playerOptedIn = value;
			}
		}
		
		/// <summary>
		/// Returns true if and only if Init() has been called for this session.
		/// </summary>
		public static bool IsReady {
			get {
				return doneInit;
			}
		}
		
		/// <summary>
		/// The current Favor needed to meet the win condition.
		/// </summary>
		public static int WinConditionFavor {
			get {
				return winCond;
			}
		}
		
		/// <summary>
		/// Initializes the DCMAnalytics wrapper for this session.  This should
		/// be called at least once during the course of the game--for example,
		/// when the game is first started to the main menu--but it should never
		/// be called more than once per match after that.
		/// </summary>
		public static void Init(int winCondition = 5000) {
			#if !UNITY_EDITOR
			// Verify playerprefs contains information about opt-in.
			// If not, autopopulate the field, and if so, retrieve the value.
			if (!PlayerPrefs.HasKey(OPT_IN_PLAYERPREFS)) {
				PlayerPrefs.SetInt(OPT_IN_PLAYERPREFS, BoolToInt(true));
			} else {
				playerOptedIn = IntToBool(PlayerPrefs.GetInt(OPT_IN_PLAYERPREFS));
			}
			
			winCond = winCondition;
			doneInit = true;
			#endif
		}
		
		/// <summary>
		/// Records a win/loss/in-progress event to GameAnalytics based on
		/// the specified favor and the specified event.
		/// </summary>
		public static void RecordMatchEvent(AMatchEvent evt, int favor) {
			#if !UNITY_EDITOR
			// Privacy and initialization first.  If the player doesn't want
			// data collection, they do not have to have it.
			if (!doneInit || !playerOptedIn) return;
			
			GA_Progression.GAProgressionStatus status;
			switch (evt) {
				case AMatchEvent.Start:
					status = GAProgressionStatus.GAProgressionStatusStart;
					break;
				case AMatchEvent.Win:
					status = GAProgressionStatus.GAProgressionStatusComplete;
					break;
				case AMatchEvent.Loss:
					status = GAProgressionStatus.GAProgressionStatusFail;
					break;
			}
			
			Scene current = SceneManager.GetActiveScene();
			GameAnalytics.NewProgressionEvent(status, current.name,
				ModeString(), favor);
			AppendLog("PROGRESSION," + status
			+ ":" + current.name
			+ ":" + ModeString()
			+ ":" + favor);
			#endif
		}
		
		/// <summary>
		/// Records an event for when an ingame resource is obtained or lost,
		/// how it was lost, and where it came from.
		/// </summary>
		public static void RecordResourceEvent(AResource whichResource,
			float amount, AResourceType how, AResourceAcquisition fromWhat) {
			#if !UNITY_EDITOR
			// Privacy and initialization first.  If the player doesn't want
			// data collection, they do not have to have it.
			if (!doneInit || !playerOptedIn || amount == 0f) return;
			
			GameAnalytics.NewResourceEvent (
				amount > 0 ? GA_Resource.GAResourceFlowType.GAResourceFlowTypeSource :
                GA_Resource.GAResourceFlowType.GAResourceFlowTypeSink,
				whichResource.ToAnalyticsString(),
				amount,
				how.ToAnalyticsString(),
				fromWhat.ToAnalyticsString());
			
			AppendLog("RESOURCE," + whichResource.ToAnalyticsString()
				+ ":" + amount
				+ ":" + how.ToAnalyticsString()
				+ ":" + fromWhat.ToAnalyticsString());
			#endif
		}
		
		/// <summary>
		/// Records a basic key-value event containing a float value, keyed by
		/// currentSceneName:[multiplayer|singleplayer]:[eventName].
		/// </summary>
		public static void RecordSceneFloatEvent(string eventName, float value) {
			#if !UNITY_EDITOR
			// Privacy and initialization first.  If the player doesn't want
			// data collection, they do not have to have it.
			if (!doneInit || !playerOptedIn) return;
			if (eventName.Contains(":")) {
				DeveloperConsole.Warn("Attempted to post namespaced "
					+ "event to analytics.  Ignoring.");
				return;
			} 
			
			Scene current = SceneManager.GetActiveScene();
			if (current.buildIndex <= 0) return; // title screen or assetbundle
			string evt = current.name + ":" + ModeString() + ":"
				
			GameAnalytics.NewDesignEvent (evt, value);
			AppendLog("FLOAT:" + evt + "," + value);
			
			if (debugEvents) {
				DeveloperConsole.Print("Positional Analytics: " + evt);
			}
			#endif
		}
		
		/// <summary>
		/// Records a basic key-value event for POSITIONAL events.  Iris Burning
		/// generally has very few (if any) over-under areas on a map, so a
		/// positional event is posted as 2D: two separate events with a shared 
		/// UID between the two events so that they can be later cross-referenced
		/// for the purposes of heatmapping.
		/// 
		/// Event namespacing is sent in the following format:
		/// 
		/// currentSceneName:[multiplayer|singleplayer]:eventName:[x|y]:uid
		///
		/// </summary>
		public static void RecordScenePositionalEvent(string eventName, Vector2 position) {
			#if !UNITY_EDITOR
			// Privacy and initialization first.  If the player doesn't want
			// data collection, they do not have to have it.
			if (!doneInit || !playerOptedIn) return;
			if (eventName.Contains(":")) {
				DeveloperConsole.Warn("Attempted to post namespaced positional"
					+ "event to analytics.  Ignoring.");
				return;
			} 
			
			Scene current = SceneManager.GetActiveScene();
			if (current.buildIndex <= 0) return; // title screen or assetbundle
			string uid = Hash(Time.time.ToString() + eventName
				+ position.x.ToString() + position.y.ToString());
			string evtX = current.name + ":" + ModeString() + ":"
				+ eventName + ":x:" + uid;
			string evtY = current.name + ":" + ModeString() + ":"
				+ eventName + ":y:" + uid;
			GameAnalytics.NewDesignEvent (evtX, position.x);
			GameAnalytics.NewDesignEvent (evtY, position.y);
			
			if (debugEvents) {
				DeveloperConsole.Print("Positional Analytics: " + evtX);
				DeveloperConsole.Print("Positional Analytics: " + evtY);
			}
			
			AppendLog("POSITIONX:" + evtX + "," + position.x);
			AppendLog("POSITIONY:" + evtY + "," + position.y);
			
			#endif
		}
		
		/// <summary>
		/// Returns the game's current multiplayer/singleplayer state in about
		/// GameAnalytics-friendly string format.
		/// </summary>
		private static string ModeString() {
			return (PhotonNetwork.isConnectedAndReady && PhotonNetwork.inRoom) ?
				MODE_MULTIPLAYER : MODE_SINGLEPLAYER;
		}
		
		/// <summary>
		/// Converts a bool to a playerprefs-friendly int.
		/// </summary>
		private static int BoolToInt(bool b) {
			return b ? 1 : 0;
		}
		
		/// <summary>
		/// Converts an int to a bool where a value of 0 = false, but all other
		/// values are considered true.
		/// </summary>
		private static bool IntToBool(int i) {
			return i == 0 ? 0 : 1;
		}
		
		/// <summary>
		/// Returns a hex string of the SHA-1 digest output of the given string.
		/// </summary>
		private static string Hash(string input) {
			using (SHA1Managed sha1 = new SHA1Managed()) {
				var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
				var sb = new StringBuilder(hash.Length * 2)
				foreach (byte b in hash) sb.Append(b.ToString("x2"));
				return sb.ToString();
			}
		}
		
		/// <summary>
		/// Returns the local system newline.
		/// </summary>
		private static string GetNewline() {
			return System.Environment.NewLine;
		}
		
		/// <summary>
		/// Appends to the current log.
		/// </summary>
		private static void AppendLog(string s) {
			if (s == null || s.Trim() == "") return;
			eventLog.Add(s);
		}
		
		/// <summary>
		/// Flushes the contents of the current log.
		/// </summary>
		private static void FlushLog() {
			eventLog.Clear();
		}
		
		/// <summary>
		/// Writes the current event log to a publicly accessible location and
		/// flushes its contents.
		/// </summary>
		public static void WriteLogfile() {
			if (eventLog.Count == 0) return;
			const string persistent = Application.persistentDataPath;
			string filename = persistent + "DCMAnalytics_"
				+ (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)))
				.TotalSeconds.ToString() + "_" + Mathf.Round(Time.time)
				.ToString() + ".txt";
			if (!File.Exists(filename)) {
				StreamWriter sw = new StreamWriter(filename);
				sw.Write(GetCSVLog());
			}
			
			FlushLog();
		}
		
		
		
		/// <summary>
		/// Converts the current event log to a CSV-compliant string, or null
		/// if no events have been posted.
		/// </summary>
		public static string GetCSVLog() {
			if (eventLog.Count == 0) return null;
			string ret = "key,value" + GetNewline();
			foreach (string entry in eventLog) ret += (entry + GetNewline());
			return ret;
		}
	}
	
}

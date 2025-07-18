/*******************************************************************************
The content of this file includes portions of the proprietary AUDIOKINETIC Wwise
Technology released in source code form as part of the game integration package.
The content of this file may not be used without valid licenses to the
AUDIOKINETIC Wwise Technology.
Note that the use of the game engine is subject to the Unity(R) Terms of
Service at https://unity3d.com/legal/terms-of-service
 
License Usage
 
Licensees holding valid licenses to the AUDIOKINETIC Wwise Technology may use
this file in accordance with the end user license agreement provided with the
software or, alternatively, in accordance with the terms contained
in a written agreement between you and Audiokinetic Inc.
Copyright (c) 2025 Audiokinetic Inc.
*******************************************************************************/

﻿#if (UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS) && !UNITY_EDITOR
public partial class AkCommonUserSettings
{
	partial void SetSampleRate(AkPlatformInitSettings settings)
	{
		settings.uSampleRate = m_SampleRate;
	}

	protected partial string GetPluginPath()
	{
		return null;
	}
}
#endif

public class AkiOSSettings : AkWwiseInitializationSettings.PlatformSettings
{
#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoadMethod]
	private static void AutomaticPlatformRegistration()
	{
		if (UnityEditor.AssetDatabase.IsAssetImportWorkerProcess())
		{
			return;
		}

		RegisterPlatformSettingsClass<AkiOSSettings>("iOS");
	}
#endif // UNITY_EDITOR

	public AkiOSSettings()
	{
		// Valid for iOS not necessarily for tvOS
		SetUseGlobalPropertyValue("UserSettings.m_MainOutputSettings.m_PanningRule", false);
		SetUseGlobalPropertyValue("UserSettings.m_MainOutputSettings.m_ChannelConfig.m_ChannelConfigType", false);
		SetUseGlobalPropertyValue("UserSettings.m_MainOutputSettings.m_ChannelConfig.m_ChannelMask", false);
		IgnorePropertyValue("AdvancedSettings.m_SuspendAudioDuringFocusLoss");
		IgnorePropertyValue("AdvancedSettings.m_RenderDuringFocusLoss");
	}

	protected override AkCommonUserSettings GetUserSettings()
	{
		return UserSettings;
	}

	protected override AkCommonAdvancedSettings GetAdvancedSettings()
	{
		return AdvancedSettings;
	}

	protected override AkCommonCommSettings GetCommsSettings()
	{
		return CommsSettings;
	}

	[System.Serializable]
	public class PlatformAdvancedSettings : AkCommonAdvancedSettings
	{
		public enum Category
		{
			Ambient,
			SoloAmbient,
			PlayAndRecord,
			Playback
		}

		[UnityEngine.Tooltip("The IDs of the iOS audio session categories, useful for defining app-level audio behaviours such as inter-app audio mixing policies and audio routing behaviours.These IDs are functionally equivalent to the corresponding constants defined by the iOS audio session service back-end (AVAudioSession). Refer to Xcode documentation for details on the audio session categories.")]
		public Category m_AudioSessionCategory = Category.SoloAmbient;

		public enum CategoryOptions
		{
			MixWithOthers = 1,
			DuckOthers = 2,
			AllowBluetooth = 4,
			DefaultToSpeaker = 8,
			AllowBluetoothA2DP = 0x20
		}

		[UnityEngine.Tooltip("The IDs of the iOS audio session category options, used for customizing the audio session category features. These IDs are functionally equivalent to the corresponding constants defined by the iOS audio session service back-end (AVAudioSession). Refer to Xcode documentation for details on the audio session category options.")]
		[AkEnumFlag(typeof(CategoryOptions))]
		public CategoryOptions m_AudioSessionCategoryOptions = CategoryOptions.DuckOthers;

		public enum Mode
		{
			Default,
			VoiceChat,
			GameChat,
			VideoRecording,
			Measurement,
			MoviePlayback,
			VideoChat,
		}

		[UnityEngine.Tooltip("The IDs of the iOS audio session modes, used for customizing the audio session for typical app types. These IDs are functionally equivalent to the corresponding constants defined by the iOS audio session service back-end (AVAudioSession). Refer to Xcode documentation for details on the audio session category options.")]
		public Mode m_AudioSessionMode = Mode.Default;
		
		public enum RouteSharingPolicy
		{
			Default = 0,
			LongFormAudio = 1,
			LongFormVideo = 3,
		}

		[UnityEngine.Tooltip("Determines which audio routes are permitted for the audio session controlled by Wwise. These policies only apply for the \"Playback\" audio session category. These IDs are funtionally equivalent to the corresponding constants defined by the iOS audio session service backend (AVAudioSession). Refer to Xcode documentation for details on the audio session route-sharing policies.")]
		public RouteSharingPolicy m_AudioSessionRouteSharingPolicy = RouteSharingPolicy.Default;
		
		[UnityEngine.Tooltip("Number of Apple Spatial Audio point sources to allocate for 3D audio use (each point source is a system audio object).")]
		public uint NumSpatialAudioPointSources = 128;
		
		[UnityEngine.Tooltip("Print debug information related to audio device initialization in the system log.")]
		public bool VerboseSystemOutput = false;

		public override void CopyTo(AkPlatformInitSettings settings)
		{
#if (UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS) && !UNITY_EDITOR
			settings.audioSession.eCategory = (AkAudioSessionCategory)m_AudioSessionCategory;
			settings.audioSession.eCategoryOptions = (AkAudioSessionCategoryOptions)m_AudioSessionCategoryOptions;
			settings.audioSession.eMode = (AkAudioSessionMode)m_AudioSessionMode;
			settings.audioSession.eRouteSharingPolicy = (AkAudioSessionRouteSharingPolicy)m_AudioSessionRouteSharingPolicy;
			settings.uNumSpatialAudioPointSources = NumSpatialAudioPointSources;
			settings.bVerboseSystemOutput = VerboseSystemOutput;
#endif
		}
	}

	[UnityEngine.HideInInspector]
	public AkCommonUserSettings UserSettings = new AkCommonUserSettings
	{
		m_MainOutputSettings = new AkCommonOutputSettings
		{
			m_PanningRule = AkCommonOutputSettings.PanningRule.Headphones
		},
	};

	[UnityEngine.HideInInspector]
	public PlatformAdvancedSettings AdvancedSettings;

	[UnityEngine.HideInInspector]
	public AkCommonCommSettings CommsSettings;
}


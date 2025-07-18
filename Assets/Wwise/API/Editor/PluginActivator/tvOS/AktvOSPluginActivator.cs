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

#if UNITY_EDITOR
using UnityEditor;
using System.IO;

[UnityEditor.InitializeOnLoad]
public class AktvOSPluginActivator : AkPlatformPluginActivator
{
	// For tvOS, we use the plugin info for iOS, since they share banks.
	public override string WwisePlatformName => "iOS";
	public override string PluginDirectoryName => "tvOS";
	public override string DSPDirectoryPath => Path.Combine("tvOS", PlayerSettings.tvOS.sdkVersion == tvOSSdkVersion.Device ? "appletvos" : "appletvsimulator", "DSP");
	public override string StaticPluginRegistrationName => "AktvOSPlugins";
	public override string StaticPluginDefine => "AK_IOS";
	public override bool RequiresStaticPluginRegistration => true;

	static AktvOSPluginActivator()
	{
		if (UnityEditor.AssetDatabase.IsAssetImportWorkerProcess())
		{
			return;
		}

		AkPluginActivator.RegisterPlatformPluginActivator(BuildTarget.tvOS, new AktvOSPluginActivator());
	}
	
	private const int ARCH_INDEX = 1;
	private const int CONFIG_INDEX = 2;
	public override AkPluginActivator.PluginImporterInformation GetPluginImporterInformation(PluginImporter pluginImporter)
	{
		var parts = GetPluginPathParts(pluginImporter.assetPath);
		return new AkPluginActivator.PluginImporterInformation
		{
			PluginConfig = parts[CONFIG_INDEX],
			PluginArch = parts[ARCH_INDEX]
		};
	}

	internal override bool ConfigurePlugin(PluginImporter pluginImporter, AkPluginActivator.PluginImporterInformation pluginImporterInformation)
	{
		pluginImporter.SetPlatformData(UnityEditor.BuildTarget.tvOS, "CPU", "AnyCPU");
		if (pluginImporterInformation.PluginArch == "appletvos" && PlayerSettings.tvOS.sdkVersion != tvOSSdkVersion.Device)
		{
			// In case the plugin was previously activated, we want to ensure we deactivate it
			return false;
		}
		if (pluginImporterInformation.PluginArch == "appletvsimulator" && PlayerSettings.tvOS.sdkVersion != tvOSSdkVersion.Simulator)
		{
			// In case the plugin was previously activated, we want to ensure we deactivate it
			return false;
		}
		return true;
	}
}
#endif
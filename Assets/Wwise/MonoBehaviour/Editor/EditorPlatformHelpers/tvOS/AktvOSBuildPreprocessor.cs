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

﻿#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
public class AktvOSBuildPreprocessor
{
	static AktvOSBuildPreprocessor()
	{
		if (UnityEditor.AssetDatabase.IsAssetImportWorkerProcess())
		{
			return;
		}

		var buildConfig = new AkBuildPreprocessor.PlatformConfiguration
		{
			WwisePlatformName = "iOS" // iOS and tvOS share the same banks
		};
		AkBuildPreprocessor.RegisterBuildTarget(UnityEditor.BuildTarget.tvOS, buildConfig);
		WwiseSetupWizard.AddBuildTargetGroup(UnityEditor.BuildTargetGroup.tvOS);
	}
}
#endif
#if UNITY_TVOS && ! UNITY_EDITOR
//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (https://www.swig.org).
// Version 4.3.0
//
// Do not make changes to this file unless you know what you are doing - modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public enum AkAudioAPI {
  AkAudioAPI_AVAudioEngine = 1 << 0,
  AkAudioAPI_AudioUnit = 1 << 1,
  AkAudioAPI_Default = AkAudioAPI_AVAudioEngine|AkAudioAPI_AudioUnit
}
#endif // #if UNITY_TVOS && ! UNITY_EDITOR
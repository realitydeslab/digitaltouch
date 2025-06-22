# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity visionOS colocated multiplayer boilerplate project that enables multiple Apple Vision Pro devices to share synchronized mixed reality experiences. The project uses Unity Netcode for GameObjects with specialized spatial and temporal synchronization systems.

## Key Technologies

- **Unity 6000.1.4f1** with visionOS support
- **Unity Netcode for GameObjects 2.4.2** for multiplayer networking
- **Unity PolySpatial 2.3.1** for visionOS native rendering
- **Unity XR Hands** for hand tracking and gesture recognition
- **HoloKit Image Tracking Relocalization** for AR marker-based spatial alignment
- **Multipeer Connectivity Transport** for device-to-device networking

## Core Architecture

### Networking Layer
- **NetworkLauncher.cs** - Entry point for host/client connections
- **NetworkLogger.cs** - Connection event logging
- Uses Unity's NetworkManager singleton for core multiplayer functionality

### Hand Tracking System
- **HandTrackingManager.cs** - Central coordinator using XRHandSubsystem, manages 26 hand joints per hand
- **Hand.cs** - Individual hand representation with joint transforms
- **HandGestureManager.cs** - Recognizes fisting, facing-self, and facing-down gestures using geometric calculations

### Spatial Synchronization (Core Innovation)
- **TimestampSynchronizer.cs** - Ping-based clock synchronization between devices with RTT compensation
- **FingerSyncManager.cs** - Two-phase process: collects timestamped hand poses, then calculates rotation/translation using least-squares fitting
- **FingerSyncTrigger.cs** - UI controller that triggers sync when user holds fist for >1 second

### Network Hand Representation
- **HandJointSynchronizer.cs** - Spawns networked hand joint objects
- **NetworkHandJoint.cs** - Synchronizes individual joint positions using NetworkVariables, color-coded by owner

### Relocalization
- **RelocalizationManager.cs** - Wrapper for HoloKit image tracking system
- Uses AR markers for initial spatial alignment between devices

## Development Workflow

### Build Configuration
- Main scene: `Assets/Scenes/Test.unity`
- Target platforms: visionOS, Editor (for testing)
- Build settings configured in `ProjectSettings/EditorBuildSettings.asset`

### Testing
- Use Unity Editor Play Mode with Multiplayer Play Mode package for multi-instance testing
- ParrelSync package enables multiple editor instances for local multiplayer testing
- In-game console (Yuchen package) available for runtime debugging

### Debugging
- **DebugManager.cs** provides editor-only testing capabilities
- Network events logged through NetworkLogger
- Hand tracking states visible through HandTrackingManager events

## Key Dependencies

### Package Sources
- OpenUPM registry for `org.realitydeslab.netcode.transport.multipeer-connectivity`
- Git packages:
  - ParrelSync: `https://github.com/VeriorPies/ParrelSync.git`
  - In-Game Console: `https://github.com/yuchenz27/unity-ingame-console.git`
  - Image Tracking: `https://github.com/holoi/com.holoi.xr.image-tracking-relocalization.git`

### Critical Synchronization Parameters
- **TimestampSynchronizer**: Standard deviation threshold < 0.1f for stability
- **FingerSyncManager**: Position stability < 0.02m, rotation < 36° for valid sync
- **HandGestureManager**: Fist detection within 0.15m of wrist position

## Architectural Patterns

### Event-Driven Design
- Uses UnityEvent for loose coupling between systems
- HandTrackingManager fires events for tracking state changes
- Gesture recognition triggers sync operations

### Spatial Mathematics
- Least-squares fitting for device alignment
- Centroid calculation and cross-correlation for rotation estimation
- Real-time standard deviation validation for stability

### Network Ownership
- Host acts as authority for spatial calculations
- Clients request historical poses at synchronized timestamps
- Color coding differentiates between host (red) and client (blue) representations

## Common Development Tasks

When modifying synchronization algorithms, ensure you understand the timing dependencies between TimestampSynchronizer and FingerSyncManager. Changes to gesture thresholds in HandGestureManager will affect sync trigger sensitivity.

All networked objects must inherit from NetworkBehaviour and use proper ServerRpc/ClientRpc patterns. The project relies heavily on Unity's coordinate system transformations between local and world space.
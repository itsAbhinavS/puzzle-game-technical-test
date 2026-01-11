# Loop Energy – Grid-Based Puzzle Game (Unity)

## Short Overview

**Loop Energy** is a grid-based puzzle game inspired by classic energy loop mechanics. The core gameplay revolves around rotating grid nodes to form a complete and valid energy loop. Players must align connections correctly to allow energy to flow seamlessly from the source through all nodes.

The current implementation includes **three fully playable and polished levels**, designed to progressively introduce and reinforce the core mechanics while demonstrating visual polish and system architecture.

---

## Implemented Features

- Grid-based puzzle mechanics centered on rotating nodes to complete energy loops
- Three polished and fully playable levels with progressive difficulty
- Complete scene flow:
  - Animated intro
  - Level selection menu
  - Individual gameplay levels
- Local save system for persistent progress
  - Completed levels are saved
  - Levels unlock sequentially
- Visual polish and player feedback:
  - Glow effects for energy flow
  - DOTween-based UI and object animations
  - Particle effects on puzzle completion

---

## Overall Architecture and Flow

The project follows a **modular and decoupled architecture**, separating global systems, scene controllers, and gameplay logic to ensure maintainability and scalability.

### Global Systems (Singleton-Based)

These systems persist across scenes and manage cross-scene responsibilities:

- **SaveSystem.cs**
  - Handles local progress data
  - Manages completed levels and unlock states

- **SceneLoader.cs**
  - Manages smooth asynchronous scene transitions
  - Uses coroutines for loading, fade-in, fade-out, and loading feedback

- **AudioManager.cs**
  - Centralized control for background music and sound effects

---

## Scene Controllers

### Intro Scene
- Controlled by **IntroManager.cs**
- Plays a short animated intro sequence
- Automatically transitions to the Menu scene

### Menu Scene
- Managed by **HomeManager.cs**
- Responsibilities include:
  - Level selection
  - Locked and unlocked level state display
  - Audio settings management

### Level Scenes
Each level scene is composed of clearly separated gameplay components:
- Input handling
- Gameplay logic validation
- UI and visual feedback

This separation ensures clean responsibilities and easier maintenance.

---

## Gameplay Flow (Event-Driven)

The gameplay logic is implemented using an event-driven architecture:

1. Player taps to rotate individual grid nodes
2. **GridNode.cs** notifies the central gameplay logic of state changes
3. **GameLevelLogics.cs** validates:
   - Grid alignment
   - Energy flow correctness
4. Upon successful puzzle completion:
   - **LevelManager.cs** is notified
   - The win sequence is triggered (visual effects, animations, save update)

This flow ensures clear communication between systems and a predictable game lifecycle.

---

## Key Technical & Gameplay Features

- Tap-to-rotate grid node interaction with immediate input response
- Energy loop validation system for correct node alignment
- Visual feedback system:
  - Glow effects
  - DOTween animations
  - Particle effects
- Sequential level unlocking with persistent local save data
- Smooth asynchronous scene loading with fade transitions
- Centralized audio management system
- Resolution-independent UI optimized for portrait screen sizes
- DOTween-based animations for all UI transitions and fade effects

---

## Project Status

This project is currently a **polished gameplay prototype**, demonstrating:
- Core puzzle mechanics
- Clean and scalable architecture
- Event-driven gameplay flow
- Visual and interaction polish

The project is designed to be easily expandable with additional levels, mechanics, and visual enhancements.

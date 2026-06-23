# ARPG Prototype

A third-person Action RPG combat prototype developed in Unity, focused on responsive melee combat, lock-on targeting, enemy combat coordination, hit detection, skills, and state machine driven gameplay.

This repository contains selected C# gameplay systems from the prototype. It is intended as a code sample for gameplay programming, not as the full Unity project.

## Playable Build

[![Play ARPG Prototype](https://img.shields.io/badge/Play%20on-itch.io-FA5C5C?style=for-the-badge&logo=itch.io&logoColor=white)](https://nxvermind.itch.io/arpg-prototype)

## Gameplay Focus

The prototype was built around real-time melee combat inspired by modern Action RPGs. The main goal was to create a responsive combat loop where the player can chain attacks, dodge, parry, lock on to enemies, use skills, and fight multiple enemies without every enemy attacking at the same time.

## Main Systems

## Player Combat

The player combat system includes:

- Light and heavy attack chains
- Combo windows based on Animator.normalizedTime
- Input buffering for queued attacks
- Attack data stored in ScriptableObject nodes
- Target attraction during attacks
- Hitstop on successful hits
- Direction-based blood VFX
- Lock-on aware rotation and targeting

Relevant scripts:

- AttackNode.cs
- ComboSystem.cs
- PlayerGroundAttackState.cs
- WeaponHitBox.cs
- PlayerHitReceiver.cs
- ShowBloodVFX.cs

## Lock-On Targeting

The lock-on system allows the player to select and track enemies during combat. It uses camera direction, angle checks, distance scoring, and target replacement when the current locked enemy dies.

Relevant scripts:

- TargetingSystem.cs
- LockOnTargetLogic.cs
- LockOnTargetSystem.cs
- EnemyDetector.cs

## Enemy Combat Coordination

Enemies are coordinated through an attack role system so that groups do not attack randomly all at once. The coordinator assigns attacker and waiting roles, handles melee and ranged enemies separately, applies cooldowns, and replaces attackers when needed.

Relevant scripts:

- AttackCoordinatorSystem.cs
- EnemyAttackParticipant.cs
- EnemyCombatSystem.cs
- EnemyAttackState.cs
- EnemyVision.cs

## Enemy Repositioning

Enemies can reposition around the player using slot-based positioning and NavMesh validation. The system evaluates possible positions around the player and avoids overlapping enemy slots.

Relevant scripts:

- EnemyReposition.cs
- RepositionManager.cs

## State Machine Architecture

Both player and enemies use a finite state machine architecture. States are created once through factories and reused during gameplay to avoid unnecessary runtime allocations.

Relevant scripts:

- State.cs
- StateMachine.cs
- PlayerStateFactory.cs
- EnemyStateFactory.cs
- EliteEnemyStateFactory.cs

## Shared Gameplay Systems

Several gameplay systems are shared between player and enemies:

- Facing and rotation handling
- Impulse-based movement
- Curved, linear, and parabolic motion
- Animation handling wrapper
- Collision correction helpers
- Hitstop and time scaling

Relevant scripts:

- FacingSystem.cs
- ImpulseSystem.cs
- MotionSystem.cs
- AnimationHandler.cs
- CollisionResolver.cs
- TimeScaler.cs

## UI Model-Presenter-View

The player UI uses a simple MVP structure to separate gameplay values from the visual UI representation.

Relevant scripts:

- PlayerModel.cs
- PlayerMVP.cs
- PlayerPresenter.cs
- PlayerView.cs

## Credits

All gameplay code in this repository was written by me.

Character models, animations, VFX, and sound effects used in the playable prototype are third-party assets, including both free and paid assets.

## Notes

This repository only includes selected gameplay scripts from the prototype. Some dependencies, assets, animations, VFX, and third-party assets are not included.

The prototype is still under active development, and the systems will continue to be improved over time.

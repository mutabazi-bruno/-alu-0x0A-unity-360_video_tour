# Extended Immersive Tour: 360° VR Campus Experience

A Unity VR project for Meta Quest. It takes my original Intranet 360° video tour and turns it into one bigger experience with three scenes: a main menu, the Intranet tour, and a new campus tour built from three 360° photos I shot myself on the Insta360 ONE X2.

**Demo video:** https://youtu.be/hrfjGfawvt4

**Capture evidence (original camera files / screenshots):** https://drive.google.com/drive/folders/18oWQgj6uQxZHSaUhLgPV6PzFeV7HitoL?usp=sharing


## What it is

You put the headset on and start in the **main menu**. It has a 360° video playing in the background and two options:

- **Intranet Tour.** My original 360° video tour, now its own scene. Four video rooms (Living Room, Cantina, Cube, Mezzanine) with hotspots, info points, background music and fades.
- **Custom Campus Tour.** A new tour through three real spots on campus:
  1. **Main Hall**, the ground floor where you start
  2. **Upper Hall**, the floor above
  3. **Chill Center**, the lounge, reached from Upper Hall

Each campus room has hotspot buttons to the connected rooms and an info point you can open to read about the space.

## Scenes

| Scene | What's in it |
|---|---|
| `MainMenuScene` | Title, two tour buttons, 360° video background, music |
| `IntranetTourScene` | The original Intranet project plus a navigation panel |
| `CustomCampusTourScene` | Three 360° photo rooms, hotspots, info points, navigation panel |

## Navigation

| From | Can go to |
|---|---|
| Main Menu | Intranet Tour, Custom Campus Tour |
| Intranet Tour | Main Menu, Custom Campus Tour |
| Custom Campus Tour | Main Menu, Intranet Tour |

In both tours a small panel sits just below eye level with the scene buttons. If you turn far away from it, it slowly swings back in front of you, so you never have to hunt for it.

### Why the campus tour is shaped this way

I didn't want it to feel like jumping between random photos. Movement follows the real building: Main Hall connects to Upper Hall, and Upper Hall connects to Chill Center. There's no shortcut from Main Hall straight to Chill Center because you can't do that in real life. This is enforced in `PanoramaNavigator.cs` (each room has a list of connected rooms), not just by leaving a button out.

Every scene change and every room change fades to black first. Hard cuts are one of the easiest ways to make someone feel sick in VR.

## Controls

- Look around with the headset.
- Point a controller at a button and pull the trigger.
- Walking and teleporting are turned off on purpose. Each room is a fixed 360° view.

## Scripts

| Script | What it does |
|---|---|
| `SceneFader` | Creates itself when the app starts, stays alive between scenes, and draws a black fade right in front of the headset camera |
| `SceneNavigator` | Goes on a button. Loads a scene through the fader |
| `HotspotNavigator` | Intranet tour. Switches video rooms, gives each sphere its own render texture, and waits for the first video frame before fading back in |
| `PanoramaNavigator` | Campus tour. Switches photo rooms and blocks moves between rooms that aren't connected |
| `HotspotPlacement` | Places a hotspot using yaw, pitch and distance sliders so it lines up with doors and stairs in the photo |
| `HeadFollowPanel` | Keeps menus reachable without glueing them to your face |
| `InfoBoxToggle` | Opens and closes info panels |
| `MenuBackground360` | Menu background. Takes a 360° video or photo |

The 360° spheres follow the headset position every frame, so leaning or shifting doesn't pull you out of the centre of the image.

## Built with

- Unity 6 (6000.4), URP, Android build for Meta Quest 2
- XR Interaction Toolkit
- TextMeshPro
- Insta360 ONE X2 for all three campus photos

## How to run it

1. Open the project in Unity 6000.4 and switch platform to Android.
2. Check the scene list order in Build Profiles: `MainMenuScene`, `IntranetTourScene`, `CustomCampusTourScene`.
3. Plug in a Quest with developer mode on and hit **Build And Run**.

The first IL2CPP build is slow (20+ minutes on my laptop). Builds after that are much faster.

## Bugs worth mentioning

**Floating or falling in the headset.** In early testing the camera would either hover above the floor or drop straight through it once locomotion started. Gravity was fighting the locomotion system. I turned off **Use Gravity** on the locomotion setup, and the rig stayed exactly where it's placed. This is a standing experience with no uneven ground, so gravity wasn't doing anything useful.

**Dark hole at the top and bottom of the photos.** Looking straight up or down showed a small dark spot. Near the poles of a sphere the triangles get tiny, and Unity was sampling a very low mip level there, which is basically the whole photo blurred into one dark blob. Turning mipmaps off and setting the wrap mode to clamp on the panorama photos fixed it in all three rooms.

**Build getting cancelled.** My first Android build failed after 40 minutes with a `TaskCanceledException`. Nothing was wrong with the code. The build was being interrupted during the final packaging step, so I excluded the project folder from Windows Defender and switched IL2CPP to faster build settings.

## Credits

- Background music: https://pixabay.com/music/search/tech%20live%20by%20kevin%20macleod/

## Author

Samuel Kwizera Ihimbazwe, African Leadership University. Built for the Extended 360 VR Tour assignment.

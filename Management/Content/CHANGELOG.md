# CHANGELOG

### Latest Release
**0.9.6**
- Added emote sound effects, that play when Babbler encounters emote speech like [Sigh] or [Clears Throat]. These come with over 80 sounds: burps, chuckles, throat clears, coughs, farts, gasps, heavy breathing, hiccups, sighs, sneezes, whistles, and yawns. They come in one theme for now: Realistic. You can also add your own themes, instructions are in the readme. They can also be disabled entirely.
- Added a comprehensive audio occlusion system for Babbler speech and emote effects. This will adjust the volume and position of sounds coming through walls, doors, etc.
- Added some incidental emote sound effects that are not associated with dialog, described below.
- NPCs can now burp while eating, depending on their personality, whether they are home, and their drunkenness.
- NPCs can now fart in the restroom, depending on their personality, whether they are home, and their drunkenness.
- NPCs can now hiccup as they walk while drunk.
- NPCs can now chuckle and gasp while watching TV, depending on their extraversion.
- NPCs can now whistle in the shower, depending on their extraversion and creativity.
- Added a JSON file that can do string replacements on certain dialog lines to make them sound better. For example, changing brrr to burr for TTS engines to pronounce better. Instructions for adding your own replacements are in the readme.
- Configuring Babbler has become complicated, so I've added a Template option in the general settings. Assigning this value will reset all of your settings to one of several presets, like Text To Speech, Animal Crossing, Undertale, or Banjo Kazooie, without you needing to change all the options yourself.
- Fixed issue with ellipsis (...) causing dialog to not play.
- When shouting all caps, Babbler now converts the dialog to lower case. It will still play louder, but this will help stop TTS engines from interpreting words like WHO as acronyms.
- Dialog was quieter than I remember it, so I've tweaked the default volumes a bit.
- Dependencies have been updated.
- Babbler has been recompiled against Shadows of Doubt 1.0.

---------

**0.9.5**
- Fixed a bug with Synthesis that was causing 8 out of 11 of the default Windows voices to not be detected. If you use Synthesis and have all of the default Windows voice packs installed, you will see a huge bump in voice variety.
- Added options to Synthesis mode to create a whitelist or blacklist of speech synthesis voices. This allows you to block or allow certain voices if you want. Instructions for how to use these new options are down in the configuration section of the README.
- Fix individual citizens changing their voice characteristics if you close and open the game again. Voices stay persistent now.
- Added more aggressive detection of "emote text" that we don't want to speak, for example: (Heavy Breathing).
- Fixed a memory leak caused by Synthesis speakers not properly "pooling".
- Improved and hardened a config upgrade process that was throwing errors for some users.
- Updated SOD.Common to version 2.0.0.

---------

**0.9.4**
- Updated SOD.Common to version 1.1.8.

---------

**0.9.3**
- Fix Babbler behaving strangely during the intro "Dead of Night" scenes and cutscenes.
- Babbler now ignores player dialogue and player internal monologues.
- Patched spot where Babbler could rarely orphan a dialog speaker, which would cause it to spam errors.

---------

**0.9.2**
- Force Babbler to load first, fixing crashes when Synthesis mode is active. This will cause the config file to be named "AAAA_Babbler" now.
- Babbler will attempt to rename your old config file to "AAAA_Babbler" to preserve any settings you set up in older versions.
- Made Synthesis the default option again, now that it no longer crashes the game with other mods installed.
- Added configuration options to Synthesis for minimum and maximum speech speeds, which are persistent from citizen to citizen.
- The "operator" on the phone, like when you call for numbers or enforcers now has a persistent voice. Their voice changes once every eight hours.
- If you walk away from the phone while talking to a person giving you a job, they now continue speaking with the right voice.
- If Babbler cannot locate the citizen speaking some dialog it will fall back to a random voice and print an error clearly now.

---------

**0.9.1**
- Synthesis mode has an issue where it only works if Babbler is the only mod installed.
- Disabling it by default in favor of Phonetic mode until I can figure out why that is happening.
- It should still work if no other mods are installed, so I'm leaving the option in.

---------

**0.9.0**
- Added Synthesis mode, which uses text to speech. This is now the default, because it's great.
- Added Droning mode, which is inspired by the speech in "Undertale". It's phonetic but only using one repeating syllable.
- Added eight more sets of phonetic voices, which work in both Phonetic and Droning mode.
- Added a lot of configuration options.
- Added a volume increase when NPCs "shout" in all caps.
- Fixed some errors the plugin was throwing by moving initialization of FMOD into the main menu.

---------

**0.8.1**
- Fix Thunderstore breaking my directory structure, add some more sanity checks.

---------

### Initial Release
**0.8.0**
- Citizens now babble when they talk, whether it's straight to you, near you, or over the phone.
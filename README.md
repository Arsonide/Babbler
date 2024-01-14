# Babbler

## What is it?

This plugin aims to make Shadows of Doubt more immersive by having NPCs you encounter speak audibly, whether you are talking to them directly, over the phone, or just hearing them nearby. There are two modes of speech: Synthesis, where they speak understandably using text to speech, and Blurbs, where they speak in gibberish similar to the game "Animal Crossing", or "The Sims", or "Undertale". The default is Synthesis, because I believe it sounds better, but both modes work and are highly configurable. I will outline how configuring them works in a configuration section below.

## A Note On Gender Identity
In my effort to create an inclusive and immersive experience, I've implemented comprehensive gender identity options in this mod. While the default settings align with the unmodified settings of Shadows of Doubt, extensive configurability is provided, allowing players to tailor these aspects to their preferences. My goal is to represent everyone respectfully, ensuring a diverse and authentic experience for all. As always I welcome any player feedback.

## Installation

### r2modman or Thunderstore Mod Manager installation

If you are using [r2modman](https://thunderstore.io/c/shadows-of-doubt/p/ebkr/r2modman/) or [Thunderstore Mod Manager](https://www.overwolf.com/oneapp/Thunderstore-Thunderstore_Mod_Manager) for installation, simply download the mod from the Online tab.

### Manual installation

Follow these steps:

1. Download BepInEx from the official repository.
2. Extract the downloaded files into the same folder as the "Shadows of Doubt.exe" executable.
3. Launch the game, load the main menu, and then exit the game.
4. Download the latest version of the plugin from the Releases page. Unzip the files and place them in corresponding directories within "Shadows of Doubt\BepInEx...". Also, download the [SOD.Common](https://thunderstore.io/c/shadows-of-doubt/p/Venomaus/SODCommon/) dependency.
5. Start the game.

## Usage and features

### Features
* NPCs will have voices now, when they speak (either near you or to you), that speech will be something you can hear audibly, as either intelligible speech or unintelligible gibberish depending on what your settings are.
* NPCs talking to you on the phone will be given a band pass filter so they sound tinny, like they are talking over a phone!
* The plugin detects when NPCs are "shouting" in all caps and raises the volume of their voices.
* The plugin attempts to filter out "emote speech" like sleeping and sneezing.

### Configuration

In r2modman you should see "Config editor" on the left, in the "Other" section. If you click on that and then open up BepInEx\config\Babbler.cfg and click "Edit config", you will have many options available to you to configure Babbler to your liking, we'll go over them briefly. Just click "Save" when you are done. Note that the plugin caches a lot of things when it starts, so it's a good idea to restart the game after modifying these, or modify them before you launch the game.

---
- **General - Enabled**: Another method of enabling and disabling Babbler.
- **General - Mode**: Determines whether citizens will talk with text to speech synthesis or phonetic blurbs.
- **General - Distort Phone Speech**: When enabled, a band pass is applied to phones to make them sound a little tinnier, like phones.
---
- **Gender - Female Threshold**: Increase for more female voices, decrease for less, defaults to what the stock game uses for citizens.
- **Gender - Male Threshold**: Decrease for more male voices, increase for less, defaults to what the stock game uses for citizens.
- **Gender - Gender Diversity**: Adds a random element to voice gender selection, increase for more diverse voices.
---
- **Volume - Conversational Volume**: How loud voices will be when you are speaking directly to a person.
- **Volume - Overheard Volume**: How loud voices that you overhear nearby will be when you are not talking directly to them.
- **Volume - Shout Volume**: How loud overheard voices that are "shouting" (in all caps) will be.
- **Volume - Phone Volume**: How loud voices will be when you are talking with a person over the phone.
---
- **Synthesis - Synthesis Pitch Male Minimum**: Lowest possible pitch for male voices in Synthesis mode.
- **Synthesis - Synthesis Pitch Male Maximum**: Highest possible pitch for male voices in Synthesis mode.
- **Synthesis - Synthesis Pitch Female Minimum**: Lowest possible pitch for female voices in Synthesis mode.
- **Synthesis - Synthesis Pitch Female Maximum**: Highest possible pitch for female voices in Synthesis mode.
- **Synthesis - Synthesis Pitch Non-Binary Minimum**: Lowest possible pitch for non-binary voices in Synthesis mode.
- **Synthesis - Synthesis Pitch Non-Binary Maximum**: Highest possible pitch for non-binary voices in Synthesis mode.
---
- **Blurbs - Blurbs Pitch Male Minimum**: Lowest possible pitch for male voices in Blurbs mode.
- **Blurbs - Blurbs Pitch Male Maximum**: Highest possible pitch for male voices in Blurbs mode.
- **Blurbs - Blurbs Pitch Female Minimum**: Lowest possible pitch for female voices in Blurbs mode.
- **Blurbs - Blurbs Pitch Female Maximum**: Highest possible pitch for female voices in Blurbs mode.
- **Blurbs - Blurbs Pitch Non-Binary Minimum**: Lowest possible pitch for non-binary voices in Blurbs mode.
- **Blurbs - Blurbs Pitch Non-Binary Maximum**: Highest possible pitch for non-binary voices in Blurbs mode.
---

### Adding Voices (Synthesis Mode)

Adding new voices in Synthesis Mode is as easy as installing them on your Windows machine. To do this, go to **Settings->Time & Language->Speech**. At the bottom under **Manage voices** you can install new voices. At the time of this writing there are five English packs with eleven voices in them. The more voices you add, the more diverse your NPCs will sound.

Note that any time your available voices change (when you install new ones or uninstall old ones) it will cause the voices of NPCs in existing cities to change to accommodate the newly available voices.

### Adding Voices (Blurbs Mode)

It is possible to add new voices to Blurbs mode, but it is a bit more advanced. I will be adding more Blurbs to the standard installation of Babbler soon, but if you want to dig into adding your own, here are basic instructions.

If you locate where the mod is installed under **%AppData%\r2modmanPlus-local\ShadowsOfDoubt**, there is a subdirectory called "Blurbs". In that subdirectory is a folder for each installed voice. The naming conventions on the folders is important because it helps the mod load the syllables. In the folder for each voice, you will find wav files for each syllable. Again, the naming conventions here are important, the wav file will be named **something_syllable.wav**. After the underscore is the text that this syllable will be played for.

Just like adding voices for Synthesis Mode, changing the number of available Blurbs voices will cause the NPCs in existing games to change their voices to accomodate the newly available voices.
 
## License

All code in this repo is distributed under the MIT License. Feel free to use, modify, and distribute as needed. Attribution and licenses for all third party libraries and assets used in the creation of Babbler can be found in **Attribution.txt**.
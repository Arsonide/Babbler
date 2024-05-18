# Babbler

## What is it?

This mod aims to make Shadows of Doubt more immersive by having NPCs you encounter speak audibly, whether you are talking to them directly, over the phone, or just hearing them nearby. There are three modes of speech: Synthesis where they speak understandably using text to speech, Phonetic, where they speak in phonetic gibberish similar to the game "Animal Crossing", and Droning where they speak in repetitive gibberish similar to the game "Undertale". The default is Synthesis, because I believe it sounds better, but all three modes work, are set up with decent defaults so they are easy to activate, and are highly configurable. I will outline how to activate and configure them in sections below.

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
4. Download the latest version of the mod from the Releases page on either Thunderstore or Github. Unzip the files and place them in corresponding directories within "Shadows of Doubt\BepInEx...". Also, download the [SOD.Common](https://thunderstore.io/c/shadows-of-doubt/p/Venomaus/SODCommon/) dependency.
5. Start the game.

## Usage and features

### Features
* NPCs will have voices now, when they speak (either near you or to you), that speech will be something you can hear audibly, as either intelligible speech or unintelligible gibberish depending on what your settings are.
* All NPCs are given deterministic voices, in all three modes. This means their voices are persistent and you can recognize NPCs by their distinctive sound.
* NPCs talking to you on the phone will be given a band pass filter so they sound tinny, like they are talking over a phone!
* The mod detects when NPCs are "shouting" in all caps and raises the volume of their voices.
* The mod attempts to filter out "emote speech" like sleeping and sneezing.

### Configuration (Basic)

If you read anything in this document, read this section.

In r2modman you should see "Config editor" on the left, in the "Other" section. If you click on that and then open up BepInEx\config\AAAA_Babbler.cfg and click "Edit config", you will have many options available to you to configure Babbler to your liking. Just click "Save" when you are done.

> If you do not see AAAA_Babbler.cfg in your options, click "Start modded" to launch the game, then close both the game and r2modman. When you launch r2modman again, the config file will be there.

> Note that the mod caches a lot of things when it starts, so it's a good idea to modify these settings before you launch the game, not while the game is running.

> Also note that the config starts with "AAAA". The reason for this is that it forces Babbler to initialize before other mods, which it needs to do.

There are three modes in Babbler:
* **Synthesis**: Uses text to speech to audibly produce intelligible speech.
* **Phonetic**: Plays phonemes in sequence, and sounds a bit like Animal Crossing speech.
* **Droning**: Plays one phoneme repetitively, and sounds a bit like Undertale speech.

All three of these modes have extensive configuration options, but sensible defaults, meaning if you just want to get into the game, you only need to change one setting: **Mode**. You will find **Mode** in the **General** section of the configuration panel in r2modman. Just set it to one of these three options and save your configuration. All of the other settings have been set up for you.

### Configuration (Advanced)

You will find a ton of other settings in the configuration panel to tweak how NPCs sound to your liking, they are categorized very well. We will go through each category below, and what the options in them do.

---
#### 1. General
- **General - Enabled**: Another method of enabling and disabling Babbler.
- **General - Mode**: Determines whether citizens will talk with text to speech synthesis, phonetic sounds, or monosyllabic droning.
- **General - Version**: Babbler uses this to reset your configuration between major versions. Don't modify it or it will reset your configuration!
- **General - Distort Phone Speech**: When enabled, a band pass is applied to phones to make them sound a little tinnier, like phones.
---
#### 2. Gender
- **Gender - Female Threshold**: Increase for more female voices, decrease for less, defaults to what the stock game uses for citizens.
- **Gender - Male Threshold**: Decrease for more male voices, increase for less, defaults to what the stock game uses for citizens.
- **Gender - Gender Diversity**: Adds a random element to voice gender selection, increase for more diverse voices.
---
#### 3. Volume
- **Volume - Conversational Volume**: How loud voices will be when you are speaking directly to a person.
- **Volume - Overheard Volume**: How loud voices that you overhear nearby will be when you are not talking directly to them.
- **Volume - Phone Volume**: How loud voices will be when you are talking with a person over the phone.
- **Volume - Conversational Shout Multiplier**: When speaking in all caps, how much to multiply the normal conversational volume.
- **Volume - Overheard Shout Multiplier**: When speaking in all caps, how much to multiply the normal overheard volume.
- **Volume - Phone Shout Multiplier**: When speaking in all caps, how much to multiply the normal phone volume.
---
#### 4. Synthesis
- **Synthesis - Voice Filter Type**: Determines which installed voices on Windows Babbler will use. Set to "Everything" for all installed voices, "Blacklist" to block some, or "Whitelist" to only allow some.
- **Synthesis - Voice Filter**: If filter type is set to blacklist or whitelist, this is where you put the names you want to filter for in, separated by semicolons. The names are case-insensitive and flexible, so "david" matches "Microsoft David", etc.
- **Synthesis - Min Speed**: Lowest possible speed for speech. Zero being the standard speed.
- **Synthesis - Max Speed**: Highest possible speed for speech. Zero being the standard speed.
- **Synthesis - Min Pitch Male**: Lowest possible pitch (relative percent) for male voices.
- **Synthesis - Max Pitch Male**: Highest possible pitch (relative percent) for male voices.
- **Synthesis - Min Pitch Female**: Lowest possible pitch (relative percent) for female voices.
- **Synthesis - Max Pitch Female**: Highest possible pitch (relative percent) for female voices.
- **Synthesis - Min Pitch Non-Binary**: Lowest possible pitch (relative percent) for non-binary voices.
- **Synthesis - Max Pitch Non-Binary**: Highest possible pitch (relative percent) for non-binary voices.
---
#### 5. Phonetic
- **Phonetic - Speech Delay**: The delay of each phoneme is its length plus this. Negative numbers cause overlapping phonemes.
- **Phonetic - Chance Delay Variance**: This is the chance for any citizen to speak with variations in their phoneme delay.
- **Phonetic - Min Delay Variance**: A value between the min and max delay variance is chosen to add to the speech delay to create variations in it.
- **Phonetic - Max Delay Variance**: A value between the min and max delay variance is chosen to add to the speech delay to create variations in it.
- **Phonetic - Chance Pitch Variance**: This is the chance for any citizen to speak with variations in their phoneme pitch.
- **Phonetic - Min Pitch Variance**: A value between the min and max pitch variance is chosen to multiply with the phoneme pitch to create variations of it.
- **Phonetic - Max Pitch Variance**: A value between the min and max pitch variance is chosen to multiply with the phoneme pitch to create variations of it.
- **Phonetic - Min Frequency Male**: Lowest possible frequency (in hertz) for male voices.
- **Phonetic - Max Frequency Male**: Highest possible frequency (in hertz) for male voices.
- **Phonetic - Min Frequency Female**: Lowest possible frequency (in hertz) for female voices.
- **Phonetic - Max Frequency Female**: Highest possible frequency (in hertz) for female voices.
- **Phonetic - Min Frequency Non-Binary**: Lowest possible frequency (in hertz) for non-binary voices.
- **Phonetic - Max Frequency Non-Binary**: Highest possible frequency (in hertz) for non-binary voices.
---
#### 6. Droning
- **Droning - Valid Phonemes**: Citizens pick one phoneme to repeat, that is chosen from these phonemes.
- **Droning - Speech Delay**: The delay of each phoneme is its length plus this. Negative numbers cause overlapping phonemes.
- **Droning - Chance Delay Variance**: This is the chance for any citizen to speak with variations in their phoneme delay.
- **Droning - Min Delay Variance**: A value between the min and max delay variance is chosen to add to the speech delay to create variations in it.
- **Droning - Max Delay Variance**: A value between the min and max delay variance is chosen to add to the speech delay to create variations in it.
- **Droning - Chance Pitch Variance**: This is the chance for any citizen to speak with variations in their phoneme pitch.
- **Droning - Min Pitch Variance**: A value between the min and max pitch variance is chosen to multiply with the phoneme pitch to create variations of it.
- **Droning - Max Pitch Variance**: A value between the min and max pitch variance is chosen to multiply with the phoneme pitch to create variations of it.
- **Droning - Min Frequency Male**: Lowest possible frequency (in hertz) for male voices.
- **Droning - Max Frequency Male**: Highest possible frequency (in hertz) for male voices.
- **Droning - Min Frequency Female**: Lowest possible frequency (in hertz) for female voices.
- **Droning - Max Frequency Female**: Highest possible frequency (in hertz) for female voices.
- **Droning - Min Frequency Non-Binary**: Lowest possible frequency (in hertz) for non-binary voices.
- **Droning - Max Frequency Non-Binary**: Highest possible frequency (in hertz) for non-binary voices.
---

### Adding Voices (Synthesis Mode)

Adding new voices in Synthesis Mode is as easy as installing them on your Windows 10 / Windows 11 machine. To do this, go to **Settings->Time & Language->Speech**. At the bottom under **Manage voices** you can install new voices. At the time of this writing there are five English packs with eleven voices in them. The more voices you add, the more diverse your NPCs will sound, so I recommend installing as many as you can, because it's great.

Note that any time your available voices change (when you install new ones or uninstall old ones) it will cause the voices of NPCs in existing cities to change to accommodate the newly available voices.

### Adding Voices (Phonetic Mode)

It is possible to add new voices to Phonetic mode, but it is a bit more advanced. Phonetic voices are also used for Droning mode. The mod comes with many different phonetic voices, but this is basic instructions on adding even more. First you need to find your mod directory, which should look something like **%AppData%\r2modmanPlus-local\ShadowsOfDoubt\profiles\\<profile_name\>\BepInEx\plugins\Arsonide-Babbler**

> You will have to replace <profile_name> with the name of your profile in r2modman.

If you locate where the mod is installed under, there is a subdirectory called "Phonemes". In that subdirectory is a folder for each installed voice. The naming conventions on the folders is important because it helps the mod load the phonemes. It would be in the format **Name_Frequency**. The frequency is the natural frequency of the phonemes in the voice you are adding, which is needed to adjust its pitch.

In the folder for each voice, you will find wav files for each phoneme. Again, the naming conventions here are important, the wav file will be named **something_phoneme.wav**. If "something" is "phonetic", then the 1-2 characters after the underscore will be the letters that the phoneme is played for.

If "something" is "symbol", then it will be used in very specific circumstances depending on what is after the underscore, normally for spaces and punctuation marks. See the voices that ship with the mod for examples.

Just like adding voices for Synthesis Mode, changing the number of available Phonetic voices will cause the NPCs in existing games to change their voices to accommodate the newly available voices.
 
## License

All code in this project is distributed under the MIT License. Feel free to use, modify, and distribute as needed. That license can be found in **License.txt**. Attribution and licenses for all third party libraries and assets used in the creation of Babbler can be found in **Attribution.txt**.
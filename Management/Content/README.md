# Babbler

## What is it?

This mod aims to make Shadows of Doubt more immersive by having NPCs you encounter speak and emote audibly, whether you are talking to them directly, over the phone, or just hearing them nearby. There are three modes of speech: Synthesis where they speak understandably using text to speech, Phonetic, where they speak in phonetic gibberish similar to the game "Animal Crossing", and Droning where they speak in repetitive gibberish similar to the game "Undertale". The default is Synthesis, because I believe it sounds better, but all three modes work, are set up with decent defaults so they are easy to activate, and are highly configurable. There are also "templates" that allow you to easily configure Babbler to sound like many games. I will outline how to activate and configure them in sections below.

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
* The mod can recognize "emote speech" like "[Sneeze]" and play sound effects associated with those emotes. Right now 12 emotes are recognized.
* The mod also comes with many "incidental" sound effects not tied to dialog, like NPCs burping while eating, or laughing while watching TV.

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

You can also configure Babbler to sound like specific games by assigning **Template** in the **General** section. This exists to simplify configuration, and when it is assigned, the next time Babbler launches it will discard any other existing settings and configure many settings in Babbler for you, to sound like specific games. **Template** must be assigned to one of the following values to do this for you:
- **TextToSpeech**: This is the default Babbler ships with.
- **AnimalCrossing**: This configures Babbler to have gibberish speech formed out of phonemes, like Animal Crossing.
- **Undertale**: This configures Babbler to have gibberish speech formed out of repetitive phonemes, like Undertale.
- **Minions**: This is phonemes like Animal Crossing, but they have wider tonal shifts, making them sound more "musical", like Minions from Despicable Me.
- **BanjoKazooie**: This is droning like Undertale, but they have wider tonal shifts, making them sound more "musical", like in Banjo Kazooie.

> In case you missed it, assigning Template will DISCARD ANY OTHER CONFIGURATION OPTIONS the next time you launch the game. It's like loading a profile. After it loads that profile, it will reset back to "None", allowing you to further configure Babbler if you want to dive into the advanced settings below.

### Configuration (Advanced)

You will find a ton of other settings in the configuration panel to tweak how NPCs sound to your liking, they are categorized very well. We will go through each category below, and what the options in them do.

> If you find the amount of options to be insane, you're probably right, but I believe in giving as much control over this stuff to you, the user, as possible.

---
#### 1. General
- **General - Enabled**: Another method of enabling and disabling Babbler.
- **General - Mode**: Determines whether citizens will talk with text to speech synthesis, phonetic sounds, or monosyllabic droning.
- **General - Template**: If this anything other than None, the next time you launch the game, settings will be reset and many adjusted to match that template.
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
- **Volume - Conversational Shout Volume**: How loud shouts in all caps will be when you are speaking directly to a person.
- **Volume - Conversational Emotes Volume**: How loud emote sound effects will be when you are speaking directly to a person.
- **Volume - Overheard Volume**: How loud voices that you overhear nearby will be when you are not talking directly to them.
- **Volume - Overheard Shout Volume**: How loud shouts in all caps that you overhear nearby will be when you are not talking directly to them.
- **Volume - Overheard Emotes Volume**: How loud emote sound effects that you overhear nearby will be when you are not talking directly to them.
- **Volume - Phone Volume**: How loud voices will be when you are talking with a person over the phone.
- **Volume - Phone Shout Volume**: How loud shouts in all caps will be when you are talking with a person over the phone.
- **Volume - Phone Emotes Volume**: How loud emote sound effects will be when you are talking with a person over the phone.
- **Volume - Open Door Occlusion Multiplier**: When sounds go through an open door, multiply their volume by this.
- **Volume - Closed Door Occlusion Multiplier**: When sounds go through a closed door, multiply their volume by this.
- **Volume - Vent Occlusion Multiplier**: When sounds go through vent grating, multiply their volume by this.
- **Volume - Distant Occlusion Multiplier**: When sounds are audible but far away, multiply their volume by this.
- **Volume - Occlusion Enabled**: Whether or not to process audio occlusion on Babbler sounds. Disabling will improve performance but is not advised as you will hear through walls.
- **Volume - Occlusion Node Range**: How many nodes away you hear sounds. Higher means more sounds, less performance. Lower means less sounds, more performance.
- **Volume - Occlusion Vent Range**: How many vent ducts sounds can go through. Higher means more sounds, less performance. Lower means less sounds, more performance.
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
#### 7. Emotes
- **Emotes - Enabled**: Whether emote sound effects (like sneezing, sighing, or coughing) are enabled.
- **Emotes - Theme**: Which theme to load and play for emote sound effects. Babbler comes with \"Realistic\" and \"Abstract\" but you can add more in the Emotes directory.
- **Emotes - Min Stagger**: NPCs often emote at the same time, this is the minimum seconds to stagger their emote sounds a bit.
- **Emotes - Max Stagger**: NPCs often emote at the same time, this is the maximum seconds to stagger their emote sounds a bit.
- **Emotes - Use Pitch Shifts**: Whether or not to shift the pitch of emote sound effects to try and match an NPC's voice.
- **Emotes - Min Frequency Male**: Lowest possible frequency (in hertz) for male emote sound effects.
- **Emotes - Max Frequency Male**: Highest possible frequency (in hertz) for male emote sound effects.
- **Emotes - Min Frequency Female**: Lowest possible frequency (in hertz) for female emote sound effects.
- **Emotes - Max Frequency Female**: Highest possible frequency (in hertz) for female emote sound effects.
- **Emotes - Min Frequency Non-Binary**: Lowest possible frequency (in hertz) for non-binary emote sound effects.
- **Emotes - Max Frequency Non-Binary**: Highest possible frequency (in hertz) for non-binary emote sound effects.
---
#### 8. Incidentals
- **Incidentals - Enabled**: Use random "incidental" emotes, that are not tied to actual dialog. (Like burps, farts, and hiccups.)
- **Incidentals - Range**: How far away you can hear incidental emotes sound effects, in meters.
- **Incidentals - Min Drunk For Hiccups**: The lowest amount an NPC can be drunk before they start hiccuping.
- **Incidentals - Min Burp Chance**: The minimum chance for NPCs to burp when they finish eating. Set min and max to zero to disable burps specifically.
- **Incidentals - Max Burp Chance**: The maximum chance for NPCs to burp when they finish eating. Set min and max to zero to disable burps specifically.
- **Incidentals - Min Fart Chance**: The minimum chance for NPCs to fart when performing bathroom functions. Set min and max to zero to disable burps specifically.
- **Incidentals - Max Fart Chance**: The maximum chance for NPCs to fart when performing bathroom functions. Set min and max to zero to disable burps specifically.
- **Incidentals - Min Hiccup Chance**: The minimum chance for NPCs to hiccup as they walk drunk - smaller because it is evaluated more often. Set min and max to zero to disable hiccups specifically.
- **Incidentals - Max Hiccup Chance**: The maximum chance for NPCs to hiccup as they walk drunk - smaller because it is evaluated more often. Set min and max to zero to disable hiccups specifically.
- **Incidentals - Min Whistle Chance**: The minimum chance for NPCs to whistle while they shower. Set min and max to zero to disable whistling specifically.
- **Incidentals - Max Whistle Chance**: The maximum chance for NPCs to whistle while they shower. Set min and max to zero to disable whistling specifically.
- **Incidentals - Min TV Chance**: The minimum chance for NPCs to make various noises while watching TV. Set min and max to zero to disable these noises specifically.
- **Incidentals - Max TV Chance**: The maximum chance for NPCs to make various noises while watching TV. Set min and max to zero to disable these noises specifically.
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

### Adding Emote Sounds and Themes
Let's start by navigating to the emote directory, which should look something like **%AppData%\r2modmanPlus-local\ShadowsOfDoubt\profiles\\<profile_name\>\BepInEx\plugins\Arsonide-Babbler\Emotes**. In that directory you will find subdirectories for each theme. By default, Babbler comes with one theme: Realistic. You can add more themes by adding more folders here. The name of the folder is the name of the theme. To activate a theme in Babbler, just type its name into the **Theme** option in the configuration settings, under the **Emotes** category.

For now, let's open the "Realistic" theme by going into that folder, to learn how to add more types of emotes and more sounds for them. Once you are in a theme folder, you are greeted with ... more folders! In this directory, the names of the folders ***must*** match the emote dialog in game. For example, a directory name of "Sneeze" will match the dialog "[Sneeze]". So by naming these directories, you can add entirely new emotes if you want. Go ahead and open Sneeze now so we can see what an emote looks like inside, and how to add sound variations for one.

Finally, we are to the actual WAV files that play in-game. You can add more here if you want, but notice they follow a very specific naming pattern with three sections separated by underscores. Let's look at one specific sound: "sneeze_female_347.wav":
- **sneeze**: The first section is purely a label, it does nothing and is for organizational purposes.
- **female**: The second section is either "male" or "female" to play for civilians of those genders. Putting anything else here will classify the sound as non-binary.
- **347**: The third and final section is supposed to be the natural pitch (in hertz) of this sound. When emote pitch shifting is enabled, Babbler will use this value to try and pitch shift the sound closer to the pitch of a civilian's voice. However, I have found that this does not work very well, so it's off by default. Thus, unless you're enabling pitch shifting and experimenting with it, this can be any number you like. If you are using pitch shifting, make sure it matches the natural pitch of this sound. You can make this number "###" to specifically disable pitch shifting on this sound. This will override the configuration settings.

Every sound in the emote's folder is considered when playing emotes for a civilian. If there is more than one that matches their gender, they will pick one at random. So you can add multiple variations of the same sound. Non-Binary citizens will first look for non-binary samples, then fall back to whichever gender matches their gender scale more closely if none are found.

### Adding String Replacements
There's certain dialog that just sounds weird. Babbler tries to filter this dialog most of the time, but there's another option now: we can replace them with dialog that sounds better. For example, the line "Brrr..." sounds better in Synthesis mode when pronounced "Burr..."

In your Babbler directory, which can be found at **%AppData%\r2modmanPlus-local\ShadowsOfDoubt\profiles\\<profile_name\>\BepInEx\plugins\Arsonide-Babbler**, you will find "Replacements.json". Simply open this file and add the line you want to replace and the replacement text. Just note that it must be properly formatted JSON, and how to format JSON is beyond the scope of this document.

Replacements also do not operate on individual words, they operate on whole dialog lines. This is for performance reasons, we don't want to have to process every word at runtime. So we cannot just replace "Brrr", we have to get the whole line: "Brrr..."

## License

All code in this project is distributed under the MIT License. Feel free to use, modify, and distribute as needed. That license can be found in **License.txt**. Attribution and licenses for all third party libraries and assets used in the creation of Babbler can be found in **Attribution.txt**.
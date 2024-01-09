# Babbler

## What is it?

This plugin aims to make NPC speech more immersive by having them "babble" (think Animal Crossing, or The Sims) when they talk. This can be either when they are talking near you or in direct conversations with them. There are configuration settings to control when and how they babble.

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
* NPCs will have voices now, when they speak (either near you or to you), that speech will be something you can hear audibly, although it will be unintelligible babble.

### Configuration

In r2modman you should see "Config editor" on the left, in the "Other" section. If you click on that and then open up BepInEx\config\Babbler.cfg and click "Edit config", you will have many options available to you to configure Babbler to your liking, we'll go over them briefly. Just click "Save" when you are done.
- **Enabled**: This can be set to false to disable the plugin entirely.
- **Minimum Pitch**: This is the lowest pitch a deep voice can play at.
- **Maximum Pitch**: This is the highest pitch a voice can play at.
- **Syllable Speed**: The more you raise this, the faster people will babble.
- **Distort Phone Speech**: By default phone speech has a band pass filter to make it sound like phone speech, if you want to turn this off, set this option to false.
- **Conversational Volume**: This goes from 0-1 and determines how loud people babble when they are speaking directly to you in a dialogue.
- **Phone Volume**: This goes from 0-1 and determines how loud people babble when they speak to you over the phone.
- **Overheard Volume**: This goes from 0-1 and determines how loud people babble when they talk near you but not necessarily directly with you.

## License

All code in this repo is distributed under the MIT License. Feel free to use, modify, and distribute as needed. The license for the sounds in the sounds directory can be found in sounds/Attribution.txt.
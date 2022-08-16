# Feature : Death replay / Replay
When you die, a small clip of your death is going to be saved in the `My Games` folder of your `Documents`.

You can also manually save a small clip of the previous seconds of the game.

## How it works
The death replay is fully automatic, so you have nothing to do. When you die, you'll have a small video clip saved at this default location `Documents/My Games/Menagerie/replays/deaths`

![image](https://user-images.githubusercontent.com/25111613/184516478-ec0c1327-df6b-4d8f-a32c-7511c2e13329.png)

By default, it saves the approximativaly the last 10 seconds before you die and the next 2 seconds after you've die, for a total clip length of ~12 seconds.

The manual replay creation is easily made using the button on the overlay or hit the `F6` shortcut on your keyboard.

![image](https://user-images.githubusercontent.com/25111613/184740566-eeea5c6e-0e5c-4666-af9e-fd06e2dc84e2.png)


The replays are saved at this default location `Documents/My Games/Menagerie/replays`.

![image](https://user-images.githubusercontent.com/25111613/184516512-500e7ccf-179c-409e-b589-c7816145e301.png)

By default, it saves approximativaly the last 30 seconds before you click the button.

## Advanced configurations
You can fine tune some settings for the replay in the `settings.json` file that is located in `Documents/My Games/Menagerie` folder. This file stored the settings of the application. There are some settings that are not editable in the UI simply because you usually don't need to change them, so be carefull changing values there.

In the `settings.json` you have 3 sections related to recording and replays that you can edit.

![image](https://user-images.githubusercontent.com/25111613/184516539-1ab45771-d4ea-459f-87f9-32aa40228b34.png)


| Variable name | Description | Safe to edit | Safe value range |
|---------------|:------------|:------------:|:-----------------|
| Recording.Enabled | Enable or disabled the recording. **Require a restart to take effect** | Yes (Need restart) | true / false |
| Recording.Crf | Video quality. Don't put this too high, because it can consumme a lot of disk space and CPU usage if set too high | Yes | 0-51 |
| Recording.FrameRate | The FPS at which the video is recorded. It's dependant on your machine's power. Usually 30 FPS is good enough quality while still make sure to be ressource efficient in term of recording. | Yes | 1-60 |
| Recording.ClipDuration | Length of the buffer video stream while recording. A value to low will cause higher disk, memory and CPU usage. A value to high will cause higher disk space usage. If you disk is low on free space, you can lower this value a bit. | Yes | 60-1200 |
| Recording.ClipSaveDelay | Delay before the actual clip is saved. Usefull to have the 2 seconds of clip after you've died for example. | Yes | 0-10 |
| Recording.OutputPath | The location where the replays are stored when saved. You can use system variables in the path. | Yes | Any valid non-network path
| Recording.ClipSavePadding | Safety value to ensure the final clip doesn't miss on important while reading from the video stream. | **No** | 1 |
| Recording.NbClipsToKeep | Amount of buffers to keep while recording. | **No** | 5 |
| Recording.CleanupTimeout | Amount of time a buffer can be alive without being utilized. | **No** | 1800 |


| Variable name | Description | Safe to edit | Safe value range |
|---------------|:------------|:------------:|:-----------------|
| DeathReplay.OutputPath | The location where the replays are stored when saved. You can use system variables in the path. | Yes | Any valid non-network path |
| DeathReplay.Duration | The duration to look back before the death | Yes | 1-60 |

| Variable name | Description | Safe to edit | Safe value range |
|---------------|:------------|:------------:|:-----------------|
| Replay.OutputPath | The location where the replays are stored when saved. You can use system variables in the path. | Yes | Any valid non-network path |
| Replay.Duration | The duration to look back before the button click on the UI | Yes | 1-60 |






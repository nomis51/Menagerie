# Menagerie
![logo](https://user-images.githubusercontent.com/25111613/103430530-0f114d80-4b93-11eb-9937-884259718529.png)
A Path of Exile trade manger to simplify and speed up your trading experience.

## Requirements
- [.NET Framework 4.7.2+](https://dotnet.microsoft.com/download/dotnet-framework)

## Usage
- Go to the [Releases](https://github.com/nomis51/Menagerie/releases/latest) page
- Download and extract Menagerie-<version here>.zip file somewhere on your computer
- Run Menagerie.exe

## Features
- Handles incoming trade offers workflow (From the moment someone whisper you until the trade is completed)
- Handles outgoing trade offers workflow (From the moment you whisper someone until the trade is completed)
- Display offers in a simple and convenient way
- Multiple hotkeys/shortcuts for the most used actions (e.g. Inviting a player to your party, sending a trade request in game, etc.)
- Auto-whisper (e.g. Say "Thanks" once a trade is completed)
- Auto-kick (e.g. Remove the buyer from your party when the trade is completed)
- Player detection (e.g. the buyer has join your area)
- Custom keyboard shortcuts (Currently has the "go to hideout" shortcut built-in) (WIP)
- Auto-hide when PoE is not focused
- Can ignore offers out of your league or offers about sold items
- Sound effects and notifications
- Configurable whispers with internal variables (e.g. item name, player name, price, etc.)
- Handle aditionnal comment in trade whispers and displayed in the tooltip. There's also a yellow star on the offer to notice that there's are notes in the tooltip

![2020_12_31_16_24_20_YoloCursorOverlay](https://user-images.githubusercontent.com/25111613/103426087-d4a0b400-4b84-11eb-987e-23adc7e1ec91.png)

![2020-12-31 17_58_51-YoloCursorOverlay](https://user-images.githubusercontent.com/25111613/103430421-e5a3f200-4b91-11eb-9050-3f2c369e095c.png)

- Switch league, configure the app or quit with the system tray icon :

![blur-2020-12-30 19_53_33-YoloCursorOverlay](https://user-images.githubusercontent.com/25111613/103388931-d06b8c80-4ad9-11eb-94db-2a26386b9e75.png)

## Showcase

### Incoming offers
When you receive a trade offer whisper, it is diplayed onto the overlay just above the XP bar.

![2020-12-30 19_11_38-Overlay](https://user-images.githubusercontent.com/25111613/103387647-e0cc3900-4ad2-11eb-8d51-4f06981ce0b5.png)

There are addtional informations in a tooltip when you mouse hover the item name of the offer.
The informations are :

- The item name (full length this time without "...")
- The name of the buyer
- The time and elapsed time
- The league
- The stash tab name (e.g. tab "The tab name here") (WIP)
- The position of the item in that tab (e.g. Left: 4, Top: 5) (WIP)

![2020-12-30 19_13_11-Overlay](https://user-images.githubusercontent.com/25111613/103387685-16712200-4ad3-11eb-8338-e13678efa5e0.png)

#### Actions before you invite the player to your party
Whisper that you're ```Busy``` :

![2020-12-30 19_15_50-Overlay](https://user-images.githubusercontent.com/25111613/103387751-736cd800-4ad3-11eb-9514-f2cc69f7d213.png)

Dismiss the offer :

![2020-12-30 19_16_26-Overlay](https://user-images.githubusercontent.com/25111613/103387764-88496b80-4ad3-11eb-9a5d-558784ddbe8b.png)

Whisper that the item is ```Sold``` with ```Ctrl + Click``` on the offer :

![2020-12-30 19_17_22-Overlay](https://user-images.githubusercontent.com/25111613/103387787-ab741b00-4ad3-11eb-8d81-74faecb4dd79.png)

Whisper the player to ask him if he's ```Still interested ?``` in your item with ```Ctrl + Shift + Click``` on the offer :

![2020-12-30 19_17_22-Overlay](https://user-images.githubusercontent.com/25111613/103387787-ab741b00-4ad3-11eb-8d81-74faecb4dd79.png)

Highlight the related item in you stash (first select the relevant tab) with ```Shift + Click``` on the offer :

![2020-12-30 19_20_10-YoloCursorOverlay](https://user-images.githubusercontent.com/25111613/103387863-145b9300-4ad4-11eb-94de-8200142c3be4.png)

```Invite``` the player to your ```Party``` with a simple ```Click``` on the offer :

![2020-12-30 19_17_22-Overlay](https://user-images.githubusercontent.com/25111613/103387787-ab741b00-4ad3-11eb-8d81-74faecb4dd79.png)

A green border appear to notice that the player has been invited.

![2020-12-30 19_25_30-Overlay](https://user-images.githubusercontent.com/25111613/103387992-cd21d200-4ad4-11eb-89df-fb9cba452228.png)

#### Actions after you've invited the player to your party
```Re-invite``` the player to your ```Party``` (Usefull if the buyer doesn't accept the request in time) :

![2020-12-30 19_26_19-Overlay](https://user-images.githubusercontent.com/25111613/103388026-ea56a080-4ad4-11eb-9891-297f676856c8.png)

```Dismiss``` the offer (this time it also ```kick``` the buyer out of your party) :

![2020-12-30 19_26_45-Overlay](https://user-images.githubusercontent.com/25111613/103388038-f93d5300-4ad4-11eb-8b14-439f53bb2189.png)

You have access to the same commands ```Ctrl + Click```, ```Ctrl + Shift + Click``` and ```Shift + Click``` at this point either.

When the player ```join``` your hideout, a ```User``` icon appear on the offer.

![2020-12-30 19_27_45-Overlay](https://user-images.githubusercontent.com/25111613/103388066-1e31c600-4ad5-11eb-955c-16083fe37bc4.png)

You can then send a ```Trade Request``` to the player with a simple ```Click``` on the offer :

![2020-12-30 19_28_55-Overlay](https://user-images.githubusercontent.com/25111613/103388097-46212980-4ad5-11eb-8b65-c49db04b0084.png)

An orange border appear to notice that the trade request have been sent.

![2020-12-30 19_29_54-Overlay](https://user-images.githubusercontent.com/25111613/103388133-68b34280-4ad5-11eb-9b77-e22549b55184.png)

Once the trade is ```completed```, a ```Thanks``` whisper is automatically sent to the player and he's ```kicked out``` of your party

You can also ```remove all``` incoming offers at any time with the ```trash can``` button :

![2020-12-30 19_07_04-YoloCursorOverlay](https://user-images.githubusercontent.com/25111613/103387546-68fe0e80-4ad2-11eb-8773-8cb90416e0c5.png)

### Outgoing offers
Support whisper templates of [www.pathofexile.com](www.pathofexile.com) and [poe.trade](poe.trade) (WIP).

Simply click on the "Whisper" button available on the trade website.

![a](https://user-images.githubusercontent.com/25111613/88486523-517f9000-cf4c-11ea-88ef-423140dd6ade.png)

The whisper is automatically sent to the related player in-game.

![a](https://user-images.githubusercontent.com/25111613/88486586-b63aea80-cf4c-11ea-8c06-f91b4e13b956.png)

And the offer is displayed onto the overlay, on the right side of the screen.

![2020-12-30 19_38_04-New Issue · nomis51_Menagerie](https://user-images.githubusercontent.com/25111613/103388364-9482f800-4ad6-11eb-835d-f6f35f432e2a.png)

You have severals informations (in a tooltip aswell) available such as :

- The name of the item
- The name of the player
- The time
- The price (Usefull if you have multiple items with different prices)
- The offers are ordered by price (Lowest price on top)
- The offers are ordered by time

![2020-12-30 19_42_20-YoloCursorOverlay](https://user-images.githubusercontent.com/25111613/103388464-28ed5a80-4ad7-11eb-9da4-10e2958eb08b.png)

You can ```join``` the ```hideout``` of the player (after you've joined his party) :

![2020-12-30 19_43_03-YoloCursorOverlay](https://user-images.githubusercontent.com/25111613/103388491-3f93b180-4ad7-11eb-9bec-6d700e80cbdd.png)

You can send a ```Trade Request``` to the player :

![2020-12-30 19_43_28-YoloCursorOverlay](https://user-images.githubusercontent.com/25111613/103388505-54704500-4ad7-11eb-84a8-b12dffc90ed6.png)

You can dismiss the offer :

![2020-12-30 19_44_04-YoloCursorOverlay](https://user-images.githubusercontent.com/25111613/103388515-6651e800-4ad7-11eb-911c-aabd5ceea07e.png)

Once the trade is ```completed```, a ```Thanks``` whisper is automatically sent to the player and (WIP) you automatically ```leave the party```

You can also at any time ```Remove``` all outgoing offers

![2020-12-30 19_09_39-YoloCursorOverlay](https://user-images.githubusercontent.com/25111613/103387602-9f3b8e00-4ad2-11eb-9ea7-581607d71453.png)

## Todos
- Trades statistics
- Languages support
- Do some work on the (WIP) elements of the repo

## Keyboard shortcuts
- ```F5``` : Go to your hideout
- ```Ctrl + Click``` (on an offer) : Invite or trade with a player (depends the situation)
- ```Ctrl + Shift + Click``` (on an offer) : Send the "Are you still interested ?" whisper
- ```Ctrl + Click``` (on an offer) : Send the "Sold" whisper
- ```Shift + Click``` (on an offer) : Highlight the item in your stash (you need to first open your stash and select the relevant tab)

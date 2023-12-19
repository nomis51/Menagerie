# Feature : Incoming offers
When someone wants to buy something from you.
## How it works
When you receive a whisper from someone along the line of `Hi, I would like to buy you...`, the application will display an `Offer` onto the overlay just above the XP bar.

![image](https://user-images.githubusercontent.com/25111613/184173055-63acc9f0-7398-405a-b4db-17a2580c106e.png)


There are addtional informations in a tooltip when you mouse hover the `item name` of the offer.
The informations are :

- The time and elapsed time
- The name of the buyer
- The item name (full length this time without "...")
- The price
- The league
- The stash tab name (e.g. tab "The tab name here")
- The position of the item in that tab (e.g. Left: 4, Top: 5)
- Price conversions
  - When dealing with exalted orbs, the tooltip will provide full chaos conversion and exalted + chaos conversion based on current poe.ninja prices

![Untitled](https://user-images.githubusercontent.com/25111613/184736032-00ec5275-62a3-45c6-800a-86a0b4ba2f13.png)


### Actions before you invite the player to your party :
Whisper that you're ```Busy``` :

![image](https://user-images.githubusercontent.com/25111613/184173787-84ca663a-b64d-4fb3-a3ee-4c80e87aa81a.png)

`Dismiss` the offer :

![image](https://user-images.githubusercontent.com/25111613/184173970-4e7afb3a-83ad-4f4c-a779-4be32cf6d345.png)

Whisper that the item is ```Sold``` with ```Ctrl + Click``` on the offer :

![Untitled](https://user-images.githubusercontent.com/25111613/184736211-02de84aa-06b2-4436-b804-6742c0bf681a.png)


Whisper the player to ask him if he's ```Still interested?``` in buyinh your item with ```Ctrl + Shift + Click``` on the offer :

![Untitled](https://user-images.githubusercontent.com/25111613/184736211-02de84aa-06b2-4436-b804-6742c0bf681a.png)

`Highlight` the related item in you stash (first select the relevant tab) with ```Shift + Click``` on the offer :

![Untitled](https://user-images.githubusercontent.com/25111613/184736211-02de84aa-06b2-4436-b804-6742c0bf681a.png)

See the `item highlighting` section of the documentation for more details.

Open the chat ready to whisper the player with `Shift  + Click` on the offer : 

![Untitled](https://user-images.githubusercontent.com/25111613/184736211-02de84aa-06b2-4436-b804-6742c0bf681a.png)

```Invite``` the player to your ```Party``` with a simple ```Click``` on the offer :

![Untitled](https://user-images.githubusercontent.com/25111613/184736211-02de84aa-06b2-4436-b804-6742c0bf681a.png)

A `green` border appear to notice that the player has been `invited`.

![Untitled](https://user-images.githubusercontent.com/25111613/184736445-96a52225-cc9a-4dee-a269-d46c36a627a8.png)

### Actions after you've invited the player to your party : 
```Re-invite``` the player to your ```Party``` (usefull if the buyer doesn't accept the request in time) :

![image](https://user-images.githubusercontent.com/25111613/184174776-1d262802-a84b-4f05-9360-8871f0642d73.png)

```Dismiss``` the offer (this time it also ```kick``` the buyer out of your party) :

![image](https://user-images.githubusercontent.com/25111613/184193695-36fb0ba5-6bfc-4b0e-8438-5f0cc62790ca.png)

You have still access to the same commands `Sold`, `Still interested?` and `Highlight`.

When the player ```join``` your hideout, a ```user``` icon appear on the offer and the border will turn `yellow`.

![image](https://user-images.githubusercontent.com/25111613/184194105-e161bc2d-72b7-45bd-9016-31574e601546.png)

You can then send a ```Trade Request``` to the player with a simple ```Click``` on the offer. The border will then become `orange`, which means that a trade request have been sent. If the `trade request` is `cancelled` in any way, the border will go back to the `yellow` color, until you try to send a `trade request` again.

![Untitled](https://user-images.githubusercontent.com/25111613/184736538-af10e865-2479-497f-931d-edf442894a52.png)


Once the trade is ```completed```, a ```Thanks``` whisper can be automatically sent to the buyer and he's ```kicked out``` of your party

You can also ```remove all visible``` offers at any time with the ```trash can``` button :

![image](https://user-images.githubusercontent.com/25111613/184736678-c54f1789-3f4f-4016-a3a0-5c5eaa667014.png)


You can customize everything in the `Incoming trades` section of the `Settings`

![image](https://user-images.githubusercontent.com/25111613/184737097-7a5bda8d-fa60-45f1-8ac5-ec501906a606.png)


Notice that there are some specials variables you can use in the whispers template to inject informations.

Those variables are : 

- `{item}` to print the `item name`
- `{price}` to print the `price of the time` (e.g. 5 chaos)
- `{location}` to print your `current location` (e.g. Hall of Grandmasters, Celestial Hideout, Glacier map, etc.)

![image](https://user-images.githubusercontent.com/25111613/184197753-2f1334b2-02f8-4a31-975f-c9f9087d3b58.png)
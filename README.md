Car Trumps
==========

Car Trumps is a Windows Phone 8 example game, based on a well-known children's
card game. Basically the aim of the game is to select one property from a card
in your hand (e.g., the car's top speed) which you think can beat the opponent's
selection. This is a two player game, and requires two phones with NFC support.

The game starts when the players touch the phones together. A random hand is
given to both players from the deck. Both players select a card from their
decks. Then the other one of the players has to choose a property to compare.
The comparison is executed by touching the devices together. The winner gets the
opponent's losing card. The game ends when either player runs out of cards.

This example application is hosted in GitHub:
https://github.com/Microsoft/car-trumps

For more information on implementation and porting, visit Lumia
Developer's Library:
http://developer.nokia.com/Resources/Library/Lumia/#!code-examples/car-trumps.html

This project is compatible with Windows Phone 8 and has been developed with
Microsoft Visual Studio Express 2012 for Windows Phone.


1. Instructions
--------------------------------------------------------------------------------

To build the application you need to have Windows 8 and Windows Phone SDK 8.0 or
later installed.

Using the Windows Phone 8 SDK:

1. Open the SLN file: File > Open Project, select the file `CarTrumps.sln`
2. Select the target 'Device'.
3. Press F5 to build the project and run it on the device.

Please see the official documentation for
deploying and testing applications on Windows Phone devices:
http://msdn.microsoft.com/en-us/library/gg588378%28v=vs.92%29.aspx


2. Implementation
--------------------------------------------------------------------------------

**Important files and classes:**

* `Nfc/NfcManager.cs`: Wrapper for the ProximityDevice. Handles submission and
  subscription of NFC messages. The class contains two methods used by
  `MainPage` and `GamePage`;
    * `DealCards()`: Sends a DealCards message, which starts a new game.
      The message contains ids of opponent's cards, and a random id, which
      is used to decide which player's randomised cards are used.
    * `ShowCard()`: Sends a ShowCard message. The message contains selected
      card id and property. 
* `Nfc/NfcInitiationMessage.cs`: This message type is used in the custom
  initiation protocol to decide the master/slave relationship between the two
  devices before the game is started.
* `Nfc/NfcMessage.cs`: Container for a NFC message, handles (de)serialization.
  In practice the binary message sent via NFC is a XML document.
* `Model/CardModel.cs`: Model for the pages. Contains also simple game logic;
  shuffling of the cards, winner decision, etc.
* `MainPage.xaml.cs`: The game starts from and ends at the main page. `Mainpage`
  handles the `DealCards` message, which starts a new game.
* `GamePage.xaml.cs`: The in-game page. Handles the ShowCard message. The lower
  part of the page consists of a Panorama holding the card properties. The top
  part is reserved for the car's image.


**Required capabilities:**

* `ID_CAP_NETWORKING`
* `ID_CAP_PROXIMITY`


3. Known issues
--------------------------------------------------------------------------------

* Wrong card may be selected, if the touch between the phones is too weak.


4. License
--------------------------------------------------------------------------------

See the license text file delivered with this project. The license file is also
available online at
https://github.com/Microsoft/car-trumps/blob/master/Licence.txt

All the car images were taken from [Wikipedia](http://en.wikipedia.org). Here is a
complete list of the used images (public domain):

* http://en.wikipedia.org/wiki/File:2005-07_Volvo_V70.jpg
* http://en.wikipedia.org/wiki/File:Volvo_V70R.jpg
* http://en.wikipedia.org/wiki/File:Volvo-850-wagon-rear.jpg
* http://en.wikipedia.org/wiki/File:1988-1991_Volvo_240_GL_station_wagon_(2011-06-15)_01.jpg
* http://en.wikipedia.org/wiki/File:Volvo-XC60-DC.jpg
* http://en.wikipedia.org/wiki/File:3rd_Volvo_XC70_--_11-26-2011.jpg
* http://en.wikipedia.org/wiki/File:2nd_Volvo_XC70_--_03-16-2012.JPG
* http://en.wikipedia.org/wiki/File:2011-2012_Volvo_C30_--_01-07-2012.jpg
* http://en.wikipedia.org/wiki/File:2000-2002_Volvo_S40_2.0_sedan_(2011-11-17).jpg
* http://en.wikipedia.org/wiki/File:2005-07_Volvo_V70.jpg

GNU Free Documentation License:
http://en.wikipedia.org/wiki/en:GNU_Free_Documentation_License


5. Version history
--------------------------------------------------------------------------------

* Version 1.0: The initial release.

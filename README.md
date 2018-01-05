# Support tekmunkey via Patreon

If you'd like to support my free software developments, my patreon can be found at:

https://www.patreon.com/tekmunkey

# dotNetExtensions

The dotNetExtensions project is a compilation of various helper functions and methods that extend the value of DotNET builtin or base classes such as strings, arrays, and the BitConverter.  I started compiling the various helper functions (from various projects where they were defined) recently, where before I was mainly in the habit of copying code from one project into another and then customizing it as needed.

What's important for consumers to understand about this particular project is that it is a library.  That means it gets referenced along with other projects, and with Visual Studio that means I tend to open it as part and parcel with a Solution, then reference the *Project* rather than just the DLL itself, and during development of that totally separate project I tend to modify and update code in the library too.  Then when I build and deploy (and gitpost) that separate Solution, for example the fileDiffer or the textWrapper, the new version of the dotNetExtensions DLL gets built and included with the distro and the reference to the dotNetExtensions Project gets included with the git source repo, but the dotNetExtensions repo itself may not get updated.

So if you download one of those projects that references this one, and you download this one and get a reference to it and things seem out of whack, **__please let me know as soon as possible__** so I can post a repo update of the dotNetExtensions Project.  I'm only human and stuff like this can in fact slip my addled, medication side-effected mind.

## bitwise

All the GetBytes() functions (for short, int, and long values) and then back ToIntXX() functions but with Network Byte Order in mind.

## arrayExtensions

As of this writing, only contains array.push and array.pop which work about how you'd expect them to, if you know what push and pop functions are.  Push adds an object of any given type to the end of an array, automagically resizing the array on your behalf, while pop removes (and returns) an object of any given type from the end of an array, again resizing it for you.  

These utility functions are mostly intended to make handling arrays a bit handier, since they're preferable to Lists and other members of the Collections and subordinate namespace(s) and they're all over in DotNET builtin collections like TextBox.Lines etc, but they're a bit of a bear to work with.

## encodingExtensions

DotNET offers a paltry stab at Text Encoding via the System.Text.Encoding namespace, and does it badly.  You get ASCII and UTF-8 and "Unicode" (UTF-16) and "BigEndianUnicode" (UTF-16 BE) and UTF-32 (Little Endian *only*) and that's garbage.  DotNET was originally developed as a learning tool and just because Microsoft figured out they could make a mint full of money with Visual Studio and all kinds of crappy addons and low-quality modules like Sharepoint and VSTO (which shoddily attempts and fails to replaced COM Interop for MS Office Addins), they quit marketing it as something for newbs and started marketing it as something for legit programmers.  Now there's this whole crop of know-nothings running around who think it makes sense to stab blindly in the dark at 3 or 4 text encodings and give up if none of them work.

So in annoyance I went ahead and started my encodingExtensions project, originally planning just to provide UTF-32 with Big and Little Endian functionality.  Then I decided I might as well do everything if I was doing anything, so I started at the bottom with UTF-8 (literally 2 days ago in the afternoon) and about 36 hours later I'm posting up a fully functional UTF-8 and UTF-16 with some features I'm pretty proud of, and to be frank I'm a little perturbed how simple it was for little old me to provide them when Microsoft is this huge conglomerate with hundreds of programmers who didn't bother to do jack or smack.

My UTF-16 implementation, for example, provides a function 'isBigEndian' which if it returns a True or False value has fairly fully validated that the data provided is UTF-16 data by sampling and decoding 512 characters (single and multi-word) from the provided data array.  This function also *platform agnostically* provides an accurate gauge as to whether the data it's scanned is stored in Big or Little Endian format, with or without a Byte Order Marker (BOM), and whether or not that BOM is accurate.  In other words this should work the same way on Big and Little Endian systems, although I haven't ever actually heard of a Big Endian system running a DotNET Framework.  Any errors in the data itself, meaning UTF-16 decode errors, cause the function to raise a trappable exception.  

So I have a single "decode" function for UTF-16 that identifies BE or LE data automagically and "just plain works" and I have a single "encode" function that you pass a data array and a boolean "encodeLittleEndian" parameter as TRUE into if you want little endian, or FALSE if you want big endian, and it does its job splendidly.  That's a far cry from DotNET's builtin where you would have to use 2 completely different System.Text.Encoder classes if you wanted BE or LE encoding.  Once I get UTF-32 finished, I imagine I will be able to easily identify any type of Unicode based on a data scan the same way I'm identifying BE/LE in UTF-16 data, which is again something DotNET not only doesn't provide but the people they have flagged as "Microsoft Employees" on the MSDN and TechNET Forums are making out like it's just plain impossible.  I've found posts where people with those flags are literally advising people to just cast around in the dark with various Encoding objects until/unless they find one that works and if they don't, well, then it's probably not encoded text at all (nevermind that UTF-32 BE and about a hundred other encoding schemes aren't even available in System.Text.Encoding)!

All of this would probably be a lot more useful if embedded in something that inherits from FileStream or StreamReader, but at the end of the day (or maybe I should say "the headline here" or "the bottom line" is) I'm just completely appalled at Microsoft and DotNET.  This didn't take me just a few hours to write and test out because I'm so darn smart; it took me just a few hours to write and test out because Unicode is so darn simple.

## stringExtensions

As of this writing, only contains a high-performance text wrapping function but one that actually works - unlike literally every other one I've seen.  This one will actually respect user pre-formatting such as linebreaks you put in before you called the wrapping function in the first place, and a whole lot more.

+ Provides a default collection of good characters to break lines on, assuming you don't mind breaking on spaces, dashes, and underscores, or allows you to define your own collection of good breaking chars on each callout.

+ Allows left and right side line decorations, where the line deco strings you pass into the function are accounted for by the code *within the lineWidth you specified* so that each line of wrapped text is individually decorated on the left and/or right as desired, and still wrapped appropriately.  For example, if your left line decoration is '##  ' (4 characters) and your text is 40 characters and your maximum line width is 30 characters long, each line of text will contain a leading '##  ' followed by 26 characters of text.

+ Allows you to actually *wrap strictly to* your desired line width, not just *randomly limited by* your desired line width, so for example you can use my code to build a nicely formatted ASCII text box for a telnet server marquee or some such.

+ When you choose to wrap strictly (using the forceWidth option), you can specify one or more characters via the padString parameter, for snazzy art-like padding strings.  

+ When you choose to wrap strictly, you can choose to apply your padding on on the right side (left-justifying your text), on the left side (right-justifying your text), or on both sides (centering your text) of each wrapped line.  Padding is inserted between line decorations.

```c#
// A snazzy boxBorder at 78 columns total, filled with # characters
string boxBorder = dotNetExtensions.stringExtensions.getPaddedLine(string.Empty, 78, true, @"#", 0);
// A wrappable Lorem Ipsum - notice that blank lines are given full line-width padding and are boxed themselves with line deco on the left and right sides
string mystring = "\r\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\r\n";
// A demo array so we can kill 2 birds with 1 stone
string[] boxStrings = new string[0];
// Demonstrating arrayExtensions.push to shove a snazzy box-top border in
dotNetExtensions.arrayExtensions.push<string>(ref boxStrings, boxBorder);
// Converting our wrappable string into boxed lines at 78 columns each
foreach (string s in dotNetExtensions.stringExtensions.getWrappedLines(mystring, 78, @"## ", @" ##", null, true, @" ", 0))
{
    // pushing in each wrap-boxed Lorem Ipsum line
    dotNetExtensions.arrayExtensions.push<string>(ref boxStrings, s);
}
// And finishing off by pushing in a box-bottom border
dotNetExtensions.arrayExtensions.push<string>(ref boxStrings, boxBorder);
//
// Of course you can also leave off the line decos and pass the forceWidth parameter a false, and then you just get standard wrapping where each line breaks 
// on an appropriate character (such as space, dash, or underscore), and no padding occurs.
//
```

```
##############################################################################
##                                                                          ##
## Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod  ##
##  tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim    ##
## veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea  ##
## commodo consequat. Duis aute irure dolor in reprehenderit in voluptate   ##
##    velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint     ##
##   occaecat cupidatat non proident, sunt in culpa qui officia deserunt    ##
##                       mollit anim id est laborum.                        ##
##                                                                          ##
##############################################################################
```
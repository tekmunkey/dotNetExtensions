# Support tekmunkey via Patreon

If you'd like to support my free software developments, my patreon can be found at:

https://www.patreon.com/tekmunkey

# dotNetExtensions

The dotNetExtensions project is a compilation of various helper functions and methods that extend the value of DotNET builtin or base classes such as strings, arrays, and the BitConverter.  I started compiling the various helper functions (from various projects where they were defined) recently, where before I was mainly in the habit of copying code from one project into another and then customizing it as needed.

What's important for consumers to understand about this particular project is that it is a library.  That means it gets referenced along with other projects, and with Visual Studio that means I tend to open it as part and parcel with a Solution, then reference the *Project* rather than just the DLL itself, and during development of that totally separate project I tend to modify and update code in the library too.  Then when I build and deploy (and gitpost) that separate Solution, for example the fileDiffer or the textWrapper, the new version of the dotNetExtensions DLL gets built and included with the distro and the reference to the dotNetExtensions Project gets included with the git source repo, but the dotNetExtensions repo itself may not get updated.

So if you download one of those projects that references this one, and you download this one and get a reference to it and things seem out of whack, **__please let me know as soon as possible__** so I can post a repo update of the dotNetExtensions Project.  I'm only human and stuff like this can in fact slip my addled, medication side-effected mind.

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
            foreach (string s in dotNetExtensions.stringExtensions.getWrappedLines(mystring, 78, @"##  ", @"  ##", null, true, @" ", 0))
            {
                // pushing in each wrap-boxed Lorem Ipsum line
                dotNetExtensions.arrayExtensions.push<string>(ref boxStrings, s);
            }
            // And finishing off by pushing in a box-bottom border
            dotNetExtensions.arrayExtensions.push<string>(ref boxStrings, boxBorder);
            //
            // Of course you can also leave off the line decos and pass the forceWidth parameter a false, and then you just get standard wrapping
            //
```


##############################################################################
##                                                                          ##
##     Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do      ##
##   eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim    ##
##   ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut     ##
##        aliquip ex ea commodo consequat. Duis aute irure dolor in         ##
##   reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla    ##
##    pariatur. Excepteur sint occaecat cupidatat non proident, sunt in     ##
##          culpa qui officia deserunt mollit anim id est laborum.          ##
##                                                                          ##
##############################################################################


## bitconverterExtensions

All the GetBytes() functions (for short, int, and long values) and then back ToIntXX() functions but with Network Byte Order in mind.

## arrayExtensions

As of this writing, only contains array.push and array.pop which work about how you'd expect them to, if you know what push and pop functions are.  Push adds an object of any given type to the end of an array, automagically resizing the array on your behalf, while pop removes (and returns) an object of any given type from the end of an array, again resizing it for you.  These utility functions are mostly intended to make handling arrays a bit handier, since they're preferable to Lists and other members of the Collections and subordinate namespace(s) and they're all over in DotNET builtin collections like TextBox.Lines etc, but they're a bit of a bear to work with.
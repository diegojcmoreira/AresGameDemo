// PlayerController.h
// Read the necessary input for play controller (keyboard and mouse)
#include <stdlib.h>
#include <string>
using namespace std;
#include <X11/Xlib.h>
#include <X11/XKBlib.h>
#include <X11/keysym.h>
#include "Server.h"
#include <map>
#include <bits/stdc++.h> 

//lib to read player input and prepare it to be sended to client
class ControllerInput {

        private: 

            Display *display;
            XEvent xevent;
            Window window;

            // keeps a signal of what keys ares pressed(1)/release(0) to send to unity;
            
        public:
            signed char keySignal[8]; //[a|d|w|s|space|left button|mouse.deltaX|mouse.deltaY]

            //Inicialize components for input listener
            ControllerInput(): keySignal { 0, 0, 0, 0, 0, 0, 0, 0} {
                if( (display = XOpenDisplay(NULL)) == NULL )
                        exit(-1);

                window = DefaultRootWindow(display);
                XAllowEvents(display, AsyncBoth, CurrentTime);
            

            }

            //Listener for the keyboard and mouse inputs
            void InputListener(Server server, bool* endService){
                int width, height, snum;
                snum = DefaultScreen(display);
                width = DisplayWidth(display, snum);
                height = DisplayHeight(display, snum);
                char buffer[100] = {0};

                

                string key_name[] = {"left", "second (or middle)", "right", "fourth", "fivth"};


                // Grab control off the keyboard and Mouse, reads inputs and send them to client
                XGrabKeyboard(display, 
                            window,
                            True, 
                            GrabModeAsync,
                            GrabModeAsync, 
                            CurrentTime);
    

                XGrabPointer(display, 
                            window,
                            1, 
                            PointerMotionMask | ButtonPressMask | ButtonReleaseMask, 
                            GrabModeAsync,
                            GrabModeAsync, 
                            None,
                            None,
                            CurrentTime);

                
                string mensagem;
                KeySym keysym;



                char keyState[32];

                map<char, int> keyCodes = {{'a', XKeysymToKeycode(display, XK_a),},
                                    {'d', XKeysymToKeycode(display, XK_d),},
                                    {'w', XKeysymToKeycode(display, XK_w),},
                                    {'s', XKeysymToKeycode(display, XK_s),},
                                    {'p', XKeysymToKeycode(display, XK_space),}};//sPace
                
                map<char, int> keyIndex = {{'a', 0,},
                                    {'d', 1,},
                                    {'w', 2,},
                                    {'s', 3,},
                                    {'p', 4,}};//sPace


                
    


                XWarpPointer(display, None, window, 0, 0, 0, 0, 100, 100);
                int last_y = 100;
                int last_x = 100;
                while(true) {

                    if (*endService){
                        XUngrabKeyboard(display, CurrentTime);
                        XUngrabPointer(display, CurrentTime);
                        return;
                    }
                    
                    XNextEvent(display, &xevent);
                    XQueryKeymap(display, keyState);

                    //reads the status of all keyboard keys and save the info of the keys neccessary for the game
                    for (char key : {'a','s','d','w','p'}) {
                        if(keyState[keyCodes[key]/8]&(0x1<<(keyCodes[key]%8)))
                            keySignal[keyIndex[key]] = 1;
                        else
                            keySignal[keyIndex[key]] = 0;
                    }

                    //calculate the delta position of the mouse pointer
                    keySignal[6] = xevent.xmotion.x-last_x;
                    keySignal[7] = last_y- xevent.xmotion.y;
                    last_x = xevent.xmotion.x;
                    last_y = xevent.xmotion.y;


                    // check if cursor is on the edges of screen and sent it to the middle of screen
                    if (xevent.xmotion.x > (width-50) ||
                        xevent.xmotion.x < 50 ||
                        xevent.xmotion.y > (height - 50) ||
                        xevent.xmotion.y < 50) {

                        XWarpPointer(display, None, window, 0, 0, 0, 0, (int)(width/2), (int)(height/2));
                        int last_y = (int)(height/2);
                        int last_x = (int)(width/2);

                    }

                    switch (xevent.type) {
                        case ButtonPress:
                            if(xevent.xbutton.button == 1)
                                keySignal[5] = 1;

                            break;
                        case ButtonRelease:
                            if(xevent.xbutton.button == 1)
                                keySignal[5] = 0;

                            break;
                        case KeyPress:
                            keysym = XkbKeycodeToKeysym( display, xevent.xkey.keycode, 0, (xevent.xkey.state & ShiftMask) ? 1 : 0);
                            switch(keysym) {
                                case XK_Escape:
                                
                                    //If Escape was press, exit listener
                                    XUngrabKeyboard(display, CurrentTime);
                                    XUngrabPointer(display, CurrentTime);
                                    server.SendData("exit");
                                    return;
                            }

                            break;
                        

                    }
                    // send inputs to client
                    server.SendData(keySignal, sizeof(keySignal));
                    memset(keySignal, 0, sizeof(keySignal));
                    


                }
                

            }
                
};
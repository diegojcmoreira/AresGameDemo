#include <stdio.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <string.h>
#include <string>
using namespace std;
#include <X11/Xlib.h>
#include <X11/XKBlib.h>
#include <X11/keysym.h>
#include "PlayerController.h"
#include <unistd.h>
#include <thread>
#include <chrono>

//Start server 

int main(int argc, char **argv) {
    auto begin = std::chrono::high_resolution_clock::now();

    bool finalizeExec = false;


    Server server("127.0.0.1", 8081);

    printf("Server started\n");

    server.Listen();

    thread waitFinalThread(server.waitGameEnd, server, &finalizeExec);


    ControllerInput controller;

    controller.InputListener(server, &finalizeExec);
    waitFinalThread.join();

    auto end = std::chrono::high_resolution_clock::now();
    auto elapsed = std::chrono::duration_cast<std::chrono::nanoseconds>(end - begin);
    printf("Time Elapsed: %.3f seconds.\n", elapsed.count() * 1e-9);
    
    return 0;
}





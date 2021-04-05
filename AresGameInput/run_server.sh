#!/bin/sh
g++ Server.cpp -lX11 -pthread -o  Server
rm *.o
./Server
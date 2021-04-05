#include <iostream>
#include <fstream>
#include <stdio.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <string.h>
#include <unistd.h>
#include <thread>
using namespace std;

#define PORT 8081

//lib for server control

class Server {
    private:
        char buffer[50] = {0};
        
        int server;
        FILE *logFile;
        time_t now;


    public:
        struct sockaddr_in servAddr = {0};
        struct sockaddr_in cliAddr = {0};
        
        int sockfd = socket(AF_INET, SOCK_DGRAM, 0);
        bool finishExecution;

        Server(string serverAddress, int _port){
            
            if(sockfd == -1)
            {
                perror("failed to create socket");
                exit(EXIT_FAILURE);
            }

            if(inet_pton(AF_INET, serverAddress.c_str(), &servAddr.sin_addr)<=0)  {
                printf("\nInvalid address/ Address not supported \n");
                exit(-1);
            }
            
            servAddr.sin_family = AF_INET;
            servAddr.sin_port = htons(_port);
            
            int responseCode = bind(sockfd, (const struct sockaddr *)&servAddr, 
                sizeof(servAddr));
                
            if(responseCode == -1)
            {
                perror("failed to bind");
                close(sockfd);
                exit(EXIT_FAILURE);
            }


                finishExecution = false;
                logFile = fopen("lofFile.txt", "ab");
        


        
        }

        void Listen(){
        
            // Listen for welcome message from the client to establish connection
            socklen_t len = sizeof(cliAddr);


            
            //Waiting connection message
            int n = recvfrom(sockfd, (char *)buffer, 50, MSG_WAITALL,
                (struct sockaddr*)&cliAddr,&len);
            
            


            now = time(0);
            fprintf(logFile, "%s,%s\n",ctime(&now),buffer  );
            memset(buffer, 0, sizeof(buffer));
            

        }

        // Functions to send data to client
        int SendData(string data){
            sendto(sockfd, (const char *)data.c_str(), data.length(), 
                MSG_CONFIRM, (const struct sockaddr *) &cliAddr,
                    sizeof(cliAddr));
            now = time(0);
            fprintf(logFile, "%s,%s\n",ctime(&now),data.c_str() );
            
            return 0;

        }

        int SendData(bool* data, long int len){
            sendto(sockfd, (const char *)data, len, 
                MSG_CONFIRM, (const struct sockaddr *) &cliAddr,
                    sizeof(cliAddr));
            now = time(0);
            fprintf(logFile, "%s,%s\n",ctime(&now),ToString(data).c_str()  );
            
            return 0;

        }

        int SendData(char* data, long int len){
            sendto(sockfd, (const char *)data, len, 
                MSG_CONFIRM, (const struct sockaddr *) &cliAddr,
                    sizeof(cliAddr));
            now = time(0);
            fprintf(logFile, "%s,%s\n",ctime(&now),data );
            
            return 0;

        }

        int SendData(signed char* data, long int len){
            sendto(sockfd, (const char *)data, len, 
                MSG_CONFIRM, (const struct sockaddr *) &cliAddr,
                    sizeof(cliAddr));
            now = time(0);
            fprintf(logFile, "%s,%s\n",ctime(&now),ToString(data).c_str() );
            
            return 0;

        }

        // thread that waits for the end game message from the client
        static void waitGameEnd(Server _server, bool* finalizeExec){
            
            char buffer[100] = {0};
            int flag = 0;
            int n = 0;
            
            Server endServer("127.0.0.1", 8089);
            endServer.Listen();

            endServer.SendData("Connection Established");
            int shotsFired;
            int shotsHit;
            
            socklen_t len = sizeof(endServer.cliAddr);
            while(!(flag == -9)){
                n = recvfrom(endServer.sockfd, (char *)buffer, 100, MSG_WAITFORONE,
                (struct sockaddr*)&endServer.cliAddr,&len);
            
                int* receivedArray = (int*) buffer;

                flag = receivedArray[0];
                shotsFired = receivedArray[1];
                shotsHit = receivedArray[2];
                
                

            }
            printf("Shots Fired: %d\n", shotsFired);
            printf("Hits: %d\n", shotsHit);

            // flag to sinalize the end game
            *finalizeExec = true;

            close(endServer.sockfd);
            close(_server.sockfd);
            pthread_exit(NULL);

        }

        string ToString(signed char* data){
            int i;
            string s = "";
            for (i = 0; i < sizeof(data); i++) {
                s = s + to_string(data[i]);
            }
            return s;

        }

        string ToString(int* data){
            int i;
            string s = "";
            for (i = 0; i < sizeof(data); i++) {
                s = s + to_string(data[i]);
            }
            return s;

        }

        string ToString(bool* data){
            int i;
            string s = "";
            for (i = 0; i < sizeof(data); i++) {
                s = s + to_string(data[i]);
            }
            return s;

        }

};
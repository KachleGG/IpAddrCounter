# IpAddrCounter
A counter for IPv4 Network Addresses, Subneting and Subneting with a variable number of hosts.

## Instructions:
 The app is compiled for Windows-x64, linux-x64 and MacOS-x64 operating systems.
 Each of the executable files is one of them and they are labeled

## Source code
 You can change the source code as you wish but not advertise or sell the app without the owners(my) premission.
 You can also compile the code for a different platform if you want to.
 Compile with commands from the directory ```IpAddrCounter/src/IpAddr```

 If you are paranoid and do not trust me you can compile the code on your own with commands:
   - Windows x64: ```dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true```

   - Windows arm: ```dotnet publish -c Release -r win-arm --self-contained true /p:PublishSingleFile=true```

   - Linux x64: ```dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true```

   - Linux arm: ```dotnet publish -c Release -r linux-arm --self-contained true /p:PublishSingleFile=true```

   - masOS: ```dotnet publish -c Release -r osx-x64 --self-contained true /p:PublishSingleFile=true```

 The executables will be in ```IpAddrCounter/src/IpAddr/bin/Release/net8.0/{your_selected_platform}/publish/```

#### Disclaimer
The information provided herein is for educational and informational purposes only and should not be considered professional advice. By accessing this content, you acknowledge that you are solely responsible for your actions and decisions, including those related to assembling, modifying, or using any project, code, or components suggested herein.

We are not liable for any damages, injuries, or losses resulting from the use or misuse of this information, including but not limited to assembly processes, operational malfunctions, or unintended consequences. It is your responsibility to ensure compliance with all applicable local laws, regulations, and safety guidelines when undertaking any project. Proceed at your own risk